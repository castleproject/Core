// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using System.Reflection;
	using System.Reflection.Emit;

	class CustomAttributeUtil
	{
		public static CustomAttributeBuilder CreateCustomAttribute(Attribute attribute)
		{
			Type attType = attribute.GetType();

			ConstructorInfo ci;
			
			object[] ctorArgs = GetConstructorAndArgs(attType, out ci);

			PropertyInfo[] properties;

			object[] propertyValues = GetPropertyValues(attType, out properties, attribute);

			FieldInfo[] fields;

			object[] fieldValues = GetFieldValues(attType, out fields, attribute);

			return new CustomAttributeBuilder(ci, ctorArgs, properties, propertyValues, fields, fieldValues);
		}

		private static object[] GetConstructorAndArgs(Type attType, out ConstructorInfo ci)
		{
			object[] ctorArgs = new object[0];

			ci = attType.GetConstructors()[0];

			ParameterInfo[] constructorParams = ci.GetParameters();
			
			if (constructorParams.Length != 0)
			{
				ctorArgs = new object[constructorParams.Length];

				InitializeArgs(ctorArgs, constructorParams);
			}
			
			return ctorArgs;
		}

		private static object[] GetPropertyValues(Type attType, out PropertyInfo[] properties, Attribute attribute)
		{
			ArrayList selectedProps = new ArrayList();

			foreach(PropertyInfo pi in attType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
			{
				if (pi.CanRead && pi.CanWrite)
				{
					selectedProps.Add(pi);
				}
			}

			properties = (PropertyInfo[]) selectedProps.ToArray(typeof(PropertyInfo));

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

		private static void InitializeArgs(object[] args, ParameterInfo[] parameterInfos)
		{
			for(int i=0; i < args.Length; i++)
			{
				Type paramType = parameterInfos[i].ParameterType;

				args[i] = GetDefaultValueFor(paramType);
			}
		}

		private static object GetDefaultValueFor(Type type)
		{
			if (type == typeof(bool))
			{
				return false;
			}
			else if (type.IsEnum)
			{
				return Enum.GetValues(type).GetValue(0);
			}
			else if (type == typeof(char))
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
