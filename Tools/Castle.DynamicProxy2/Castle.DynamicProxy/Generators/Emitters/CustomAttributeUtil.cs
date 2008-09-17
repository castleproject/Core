// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Collections;
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;

	internal class CustomAttributeUtil
	{
		public static CustomAttributeBuilder CreateCustomAttribute(Attribute attribute)
		{
			Type attType = attribute.GetType();

			ConstructorInfo ci;

			object[] ctorArgs = GetConstructorAndArgs(attType, attribute, out ci);

			PropertyInfo[] properties;

			object[] propertyValues = GetPropertyValues(attType, out properties, attribute);

			FieldInfo[] fields;

			object[] fieldValues = GetFieldValues(attType, out fields, attribute);

			// here we are going to try to initialize the attribute with the collected arguments
			// if we are good (created successfuly) we return it, otherwise, it is ignored.
			try
			{
				Activator.CreateInstance(attType, ctorArgs);
				return new CustomAttributeBuilder(ci, ctorArgs, properties, propertyValues, fields, fieldValues);
			}
			catch
			{
				// there is no real way to log a warning here...
				Trace.WriteLine(@"Dynamic Proxy 2: Unable to find matching parameters for replicating attribute " + attType.FullName +
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

		private static object[] GetPropertyValues(Type attType, out PropertyInfo[] properties, Attribute attribute)
		{
			ArrayList selectedProps = new ArrayList();

			foreach (PropertyInfo pi in attType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				if (pi.CanRead && pi.CanWrite)
				{
					selectedProps.Add(pi);
				}
			}

			properties = (PropertyInfo[]) selectedProps.ToArray(typeof (PropertyInfo));

			object[] propertyValues = new object[properties.Length];

			for (int i = 0; i < properties.Length; i++)
			{
				PropertyInfo propInfo = properties[i];
				propertyValues[i] = propInfo.GetValue(attribute, null);
			}

			return propertyValues;
		}

		private static object[] GetFieldValues(Type attType, out FieldInfo[] fields, Attribute attribute)
		{
			fields = attType.GetFields(BindingFlags.Public | BindingFlags.Instance);

			object[] values = new object[fields.Length];

			for (int i = 0; i < fields.Length; i++)
			{
				FieldInfo fieldInfo = fields[i];
				values[i] = fieldInfo.GetValue(attribute);
			}

			return values;
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

				if (string.Compare(propertyInfo.Name, parameterInfo.Name, true) == 0)
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
			else if (type.IsEnum)
			{
				return Enum.GetValues(type).GetValue(0);
			}
			else if (type == typeof (char))
			{
				return char.MinValue;
			}
			else if (type.IsPrimitive)
			{
				return 0;
			}

			return null;
		}
	}
}