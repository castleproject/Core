// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Internal;

	[Serializable]
	public class AttributeDisassembler : IAttributeDisassembler
	{
		public CustomAttributeBuilder Disassemble(Attribute attribute)
		{
			var type = attribute.GetType();

			try
			{
				ConstructorInfo ctor;
				var ctorArgs = GetConstructorAndArgs(type, attribute, out ctor);
				var replicated = (Attribute)Activator.CreateInstance(type, ctorArgs);
				PropertyInfo[] properties;
				var propertyValues = GetPropertyValues(type, out properties, attribute, replicated);
				FieldInfo[] fields;
				var fieldValues = GetFieldValues(type, out fields, attribute, replicated);
				return new CustomAttributeBuilder(ctor, ctorArgs, properties, propertyValues, fields, fieldValues);
			}
			catch (Exception ex)
			{
				// there is no real way to log a warning here...
				return HandleError(type, ex);
			}
		}

		/// <summary>
		///   Handles error during disassembly process
		/// </summary>
		/// <param name = "attributeType">Type of the attribute being disassembled</param>
		/// <param name = "exception">Exception thrown during the process</param>
		/// <returns>usually null, or (re)throws the exception</returns>
		protected virtual CustomAttributeBuilder HandleError(Type attributeType, Exception exception)
		{
			// ouch...
			var message = "DynamicProxy was unable to disassemble attribute " + attributeType.Name +
			              " using default AttributeDisassembler. " +
			              string.Format("To handle the disassembly process properly implement the {0} interface, ",
			                            typeof(IAttributeDisassembler)) +
			              "and register your disassembler to handle this type of attributes using " +
			              typeof(AttributeUtil).Name + ".AddDisassembler<" + attributeType.Name + ">(yourDisassembler) method";
			throw new ProxyGenerationException(message, exception);
		}

		private static object[] GetConstructorAndArgs(Type attributeType, Attribute attribute, out ConstructorInfo ctor)
		{
			ctor = attributeType.GetConstructors()[0];

			var constructorParams = ctor.GetParameters();
			if (constructorParams.Length == 0)
			{
				return new object[0];
			}

			return InitializeConstructorArgs(attributeType, attribute, constructorParams);
		}

		private static object[] GetPropertyValues(Type attType, out PropertyInfo[] properties, Attribute original,
		                                          Attribute replicated)
		{
			var propertyCandidates = GetPropertyCandidates(attType);

			var selectedValues = new List<object>(propertyCandidates.Count);
			var selectedProperties = new List<PropertyInfo>(propertyCandidates.Count);
			foreach (var property in propertyCandidates)
			{
				var originalValue = property.GetValue(original, null);
				var replicatedValue = property.GetValue(replicated, null);
				if (AreAttributeElementsEqual(originalValue, replicatedValue))
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
		///   Here we try to match a constructor argument to its value.
		///   Since we can't get the values from the assembly, we use some heuristics to get it.
		///   a/ we first try to match all the properties on the attributes by name (case insensitive) to the argument
		///   b/ if we fail we try to match them by property type, with some smarts about convertions (i,e: can use Guid for string).
		/// </summary>
		private static object[] InitializeConstructorArgs(Type attributeType, Attribute attribute, ParameterInfo[] parameters)
		{

			var args = new object[parameters.Length];
			for (var i = 0; i < args.Length; i++)
			{
				args[i] = GetArgumentValue(attributeType, attribute, parameters[i]);
			}
			return args;
		}

		private static object GetArgumentValue(Type attributeType, Attribute attribute, ParameterInfo parameter)
		{
			var properties = attributeType.GetProperties();
			//first try to find a property with 
			foreach (var property in properties)
			{
				if (property.CanRead == false && property.GetIndexParameters().Length != 0)
				{
					continue;
				}

				if (String.Compare(property.Name, parameter.Name, StringComparison.CurrentCultureIgnoreCase) == 0)
				{
					return ConvertValue(property.GetValue(attribute, null), parameter.ParameterType);
				}
			}

			PropertyInfo bestMatch = null;
			//now we try to find it by type
			foreach (var property in properties)
			{
				if (property.CanRead == false && property.GetIndexParameters().Length != 0)
				{
					continue;
				}
				bestMatch = ReplaceIfBetterMatch(parameter, property, bestMatch);
			}
			if (bestMatch != null)
			{
				return ConvertValue(bestMatch.GetValue(attribute, null), parameter.ParameterType);
			}
			return GetDefaultValueFor(parameter);
		}

		/// <summary>
		///   We have the following rules here.
		///   Try to find a matching type, failing that, if the parameter is string, get the first property (under the assumption that
		///   we can convert it.
		/// </summary>
		private static PropertyInfo ReplaceIfBetterMatch(ParameterInfo parameterInfo, PropertyInfo propertyInfo,
		                                                 PropertyInfo bestMatch)
		{
			var notBestMatch = bestMatch == null || bestMatch.PropertyType != parameterInfo.ParameterType;
			if (propertyInfo.PropertyType == parameterInfo.ParameterType && notBestMatch)
			{
				return propertyInfo;
			}
			if (parameterInfo.ParameterType == typeof(string) && notBestMatch)
			{
				return propertyInfo;
			}
			return bestMatch;
		}

		/// <summary>
		///   Attributes can only accept simple types, so we return null for null,
		///   if the value is passed as string we call to string (should help with converting), 
		///   otherwise, we use the value as is (enums, integer, etc).
		/// </summary>
		private static object ConvertValue(object obj, Type paramType)
		{
			if (obj == null)
			{
				return null;
			}
			if (paramType == typeof(String))
			{
				return obj.ToString();
			}
			return obj;
		}

		private static object GetDefaultValueFor(ParameterInfo parameter)
		{
			var type = parameter.ParameterType;
			if (type == typeof(bool))
			{
				return false;
			}
			if (type.IsEnum)
			{
				return Enum.ToObject(type, 0);
			}
			if (type == typeof(char))
			{
				return Char.MinValue;
			}
			if (type.IsPrimitive)
			{
				return 0;
			}
			if(type.IsArray && parameter.IsDefined(typeof(ParamArrayAttribute), true))
			{
				return Array.CreateInstance(type.GetElementType(), 0);
			}

			return null;
		}

		private static List<PropertyInfo> GetPropertyCandidates(Type attributeType)
		{
			var propertyCandidates = new List<PropertyInfo>();

			foreach (var pi in attributeType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
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

		public bool Equals(AttributeDisassembler other)
		{
			return !ReferenceEquals(null, other);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}
			if (ReferenceEquals(this, obj))
			{
				return true;
			}
			if (obj.GetType() != typeof(AttributeDisassembler))
			{
				return false;
			}
			return Equals((AttributeDisassembler)obj);
		}

		public override int GetHashCode()
		{
			return GetType().GetHashCode();
		}
	}
}