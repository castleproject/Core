// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.DynamicProxy
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;
	using Core.Interceptor;

#if !SILVERLIGHT
	[Serializable]
#endif
	public class DefaultAttributeDisassembler : IAttributeDisassembler
	{
		public CustomAttributeBuilder Disassemble(Attribute attribute)
		{
			Type type = attribute.GetType();

			ConstructorInfo ctor;
			object[] ctorArgs = GetConstructorAndArgs(type, attribute, out ctor);

			PropertyInfo[] properties;
			object[] propertyValues;

			FieldInfo[] fields;
			object[] fieldValues;
			
			try
			{
				var replicated = (Attribute) Activator.CreateInstance(type, ctorArgs);
				propertyValues = GetPropertyValues(type, out properties, attribute, replicated);
				fieldValues = GetFieldValues(type, out fields, attribute, replicated);
			}
			catch (Exception)
			{
				// ouch...
				var message = "There was an error trying to replicate non-inheritable attribute " + type.Name +
				              " using default attribute disassembler. " +
				              "Use custom implementation of IAttributeDisassembler (passed as 'AttributeDisassembler' property of ProxyGenerationOptions) to replicate this attribute.";
				throw new ProxyGenerationException(message);
			}

			// here we are going to try to initialize the attribute with the collected arguments
			// if we are good (created successfuly) we return it, otherwise, it is ignored.
			try
			{
				return new CustomAttributeBuilder(ctor, ctorArgs, properties, propertyValues, fields, fieldValues);
			}
			catch
			{
				// there is no real way to log a warning here...
				Trace.WriteLine(@"Dynamic Proxy 2: Unable to find matching parameters for replicating attribute " + type.FullName +
				                ".");
				return null;
			}
		}

		private static object[] GetConstructorAndArgs(Type attType, Attribute attribute, out ConstructorInfo ci)
		{
			object[] ctorArgs = new object[0];

			ci = attType.GetConstructors()[0];

			ParameterInfo[] constructorParams = ci.GetParameters();

			if (constructorParams.Length != 0)
			{
				ctorArgs = new object[constructorParams.Length];

				InitializeConstructorArgs(attType, attribute, ctorArgs, constructorParams);
			}

			return ctorArgs;
		}

		private static object[] GetPropertyValues(Type attType, out PropertyInfo[] properties, Attribute original, Attribute replicated)
		{
			var propertyCandidates = GetPropertyCandidates(attType);

			var selectedValues = new List<object>(propertyCandidates.Count);
			var selectedProperties = new List<PropertyInfo>(propertyCandidates.Count);
			foreach (var property in propertyCandidates)
			{
				var originalValue = property.GetValue(original, null);
				var replicatedValue = property.GetValue(replicated, null);
				if(AreAttributeElementsEqual(originalValue,replicatedValue))
				{
					//this property has default value so we skip it
					continue;
				}

				selectedProperties.Add(property);
				selectedValues.Add(originalValue);
			}

			properties = selectedProperties.ToArray();
			return selectedValues.ToArray();
		}

		private static object[] GetFieldValues(Type attType, out FieldInfo[] fields, Attribute original, Attribute replicated)
		{
			var fieldsCandidates = attType.GetFields(BindingFlags.Public | BindingFlags.Instance);

			var selectedValues = new List<object>(fieldsCandidates.Length);
			var selectedFields = new List<FieldInfo>(fieldsCandidates.Length);
			foreach (var field in fieldsCandidates)
			{
				var originalValue = field.GetValue(original);
				var replicatedValue = field.GetValue(replicated);
				if (AreAttributeElementsEqual(originalValue, replicatedValue))
				{
					//this field has default value so we skip it
					continue;
				}

				selectedFields.Add(field);
				selectedValues.Add(originalValue);
			}

			fields = selectedFields.ToArray();
			return selectedValues.ToArray();
		}

		/// <summary>
		/// Here we try to match a constructor argument to its value.
		/// Since we can't get the values from the assembly, we use some heuristics to get it.
		/// a/ we first try to match all the properties on the attributes by name (case insensitive) to the argument
		/// b/ if we fail we try to match them by property type, with some smarts about convertions (i,e: can use Guid for string).
		/// </summary>
		private static void InitializeConstructorArgs(Type attType, Attribute attribute, object[] args,
		                                              ParameterInfo[] parameterInfos)
		{
			for (int i = 0; i < args.Length; i++)
			{
				args[i] = GetArgValue(attType, attribute, parameterInfos[i]);
			}
		}

		private static object GetArgValue(Type attType, Attribute attribute, ParameterInfo parameterInfo)
		{
			Type paramType = parameterInfo.ParameterType;

			PropertyInfo[] propertyInfos = attType.GetProperties();
			//first try to find a property with 
			foreach (PropertyInfo propertyInfo in propertyInfos)
			{
				if (propertyInfo.CanRead == false && propertyInfo.GetIndexParameters().Length != 0)
				{
					continue;
				}

				if (string.Compare(propertyInfo.Name, parameterInfo.Name, StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					return ConvertValue(propertyInfo.GetValue(attribute, null), paramType);
				}
			}


			PropertyInfo bestMatch = null;
			//now we try to find it by type
			foreach (PropertyInfo propertyInfo in propertyInfos)
			{
				if (propertyInfo.CanRead == false && propertyInfo.GetIndexParameters().Length != 0)
					continue;
				bestMatch = ReplaceIfBetterMatch(parameterInfo, propertyInfo, bestMatch);
			}
			if (bestMatch != null)
			{
				return ConvertValue(bestMatch.GetValue(attribute, null), paramType);
			}
			return GetDefaultValueFor(paramType);
		}

		/// <summary>
		/// We have the following rules here.
		/// Try to find a matching type, failing that, if the parameter is string, get the first property (under the assumption that
		/// we can convert it.
		/// </summary>
		private static PropertyInfo ReplaceIfBetterMatch(ParameterInfo parameterInfo, PropertyInfo propertyInfo,
		                                                 PropertyInfo bestMatch)
		{
			bool notBestMatch = bestMatch == null || bestMatch.PropertyType != parameterInfo.ParameterType;
			if (propertyInfo.PropertyType == parameterInfo.ParameterType && notBestMatch)
				return propertyInfo;
			if (parameterInfo.ParameterType == typeof (string) && notBestMatch)
				return propertyInfo;
			return bestMatch;
		}

		/// <summary>
		/// Attributes can only accept simple types, so we return null for null,
		/// if the value is passed as string we call to string (should help with converting), 
		/// otherwise, we use the value as is (enums, integer, etc).
		/// </summary>
		private static object ConvertValue(object obj, Type paramType)
		{
			if (obj == null)
				return null;
			if (paramType == typeof (String))
				return obj.ToString();
			return obj;
		}

		private static object GetDefaultValueFor(Type type)
		{
			if (type == typeof (bool))
			{
				return false;
			}
			if (type.IsEnum)
			{
#if SILVERLIGHT
				return Castle.DynamicProxy.SilverlightExtensions.EnumHelper.GetValues(type).GetValue(0);
#else
				return Enum.GetValues(type).GetValue(0);
#endif

			}
			if (type == typeof (char))
			{
				return char.MinValue;
			}
			if (type.IsPrimitive)
			{
				return 0;
			}

			return null;
		}
        
		private static List<PropertyInfo> GetPropertyCandidates(Type attributeType)
		{
			var propertyCandidates = new List<PropertyInfo>();

			foreach (PropertyInfo pi in attributeType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				if (pi.CanRead && pi.CanWrite)
				{
					propertyCandidates.Add(pi);
				}
			}

			return propertyCandidates;
		}

		private static bool AreAttributeElementsEqual(object first, object second)
		{

			//we can have either System.Type, string or numeric type
			if (first == null)
			{
				return second == null;
			}

			//let's try string
			var firstString = first as string;
			if (firstString != null)
			{
				return AreStringsEqual(firstString, second as string);
			}

			//by now we should only be left with numeric types
			return first.Equals(second);
		}

		private static bool AreStringsEqual(string first, string second)
		{
			Debug.Assert(first != null, "first != null");
			return first.Equals(second, StringComparison.Ordinal);
		}

		public bool Equals(DefaultAttributeDisassembler other)
		{
			return !ReferenceEquals(null, other);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != typeof (DefaultAttributeDisassembler)) return false;
			return Equals((DefaultAttributeDisassembler) obj);
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode();
		}
	}
}