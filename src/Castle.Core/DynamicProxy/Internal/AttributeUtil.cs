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

namespace Castle.DynamicProxy.Internal
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Linq;
	using System.Reflection;

	using Castle.DynamicProxy.Generators;

	public static class AttributeUtil
	{
		public static CustomAttributeInfo CreateInfo(CustomAttributeData attribute)
		{
			Debug.Assert(attribute != null, "attribute != null");

			// .NET Core does not provide CustomAttributeData.Constructor, so we'll implement it
			// by finding a constructor ourselves
			Type[] constructorArgTypes;
			object[] constructorArgs;
			GetArguments(attribute.ConstructorArguments, out constructorArgTypes, out constructorArgs);
#if FEATURE_LEGACY_REFLECTION_API
			var constructor = attribute.Constructor;
#else
			var constructor = attribute.AttributeType.GetConstructor(constructorArgTypes);
#endif

			PropertyInfo[] properties;
			object[] propertyValues;
			FieldInfo[] fields;
			object[] fieldValues;
			GetSettersAndFields(
#if FEATURE_LEGACY_REFLECTION_API
				null,
#else
				attribute.AttributeType,
#endif
				attribute.NamedArguments, out properties, out propertyValues, out fields, out fieldValues);

			return new CustomAttributeInfo(constructor,
			                               constructorArgs,
			                               properties,
			                               propertyValues,
			                               fields,
			                               fieldValues);
		}

		private static void GetArguments(IList<CustomAttributeTypedArgument> constructorArguments,
			out Type[] constructorArgTypes, out object[] constructorArgs)
		{
			constructorArgTypes = new Type[constructorArguments.Count];
			constructorArgs = new object[constructorArguments.Count];
			for (var i = 0; i < constructorArguments.Count; i++)
			{
				constructorArgTypes[i] = constructorArguments[i].ArgumentType;
				constructorArgs[i] = ReadAttributeValue(constructorArguments[i]);
			}
		}

		private static object[] GetArguments(IList<CustomAttributeTypedArgument> constructorArguments)
		{
			var arguments = new object[constructorArguments.Count];
			for (var i = 0; i < constructorArguments.Count; i++)
			{
				arguments[i] = ReadAttributeValue(constructorArguments[i]);
			}

			return arguments;
		}

		private static object ReadAttributeValue(CustomAttributeTypedArgument argument)
		{
			var value = argument.Value;
			if (argument.ArgumentType.GetTypeInfo().IsArray == false)
			{
				return value;
			}
			//special case for handling arrays in attributes
			var arguments = GetArguments((IList<CustomAttributeTypedArgument>)value);
			var array = new object[arguments.Length];
			arguments.CopyTo(array, 0);
			return array;
		}

		private static void GetSettersAndFields(Type attributeType, IEnumerable<CustomAttributeNamedArgument> namedArguments,
			out PropertyInfo[] properties, out object[] propertyValues,
			out FieldInfo[] fields, out object[] fieldValues)
		{
			var propertyList = new List<PropertyInfo>();
			var propertyValuesList = new List<object>();
			var fieldList = new List<FieldInfo>();
			var fieldValuesList = new List<object>();
			foreach (var argument in namedArguments)
			{
#if FEATURE_LEGACY_REFLECTION_API
				if (argument.MemberInfo.MemberType == MemberTypes.Field)
				{
					fieldList.Add(argument.MemberInfo as FieldInfo);
					fieldValuesList.Add(ReadAttributeValue(argument.TypedValue));
				}
				else
				{
					propertyList.Add(argument.MemberInfo as PropertyInfo);
					propertyValuesList.Add(ReadAttributeValue(argument.TypedValue));
				}
#else
				if (argument.IsField)
				{
					fieldList.Add(attributeType.GetField(argument.MemberName));
					fieldValuesList.Add(ReadAttributeValue(argument.TypedValue));
				}
				else
				{
					propertyList.Add(attributeType.GetProperty(argument.MemberName));
					propertyValuesList.Add(ReadAttributeValue(argument.TypedValue));
				}
#endif
			}

			properties = propertyList.ToArray();
			propertyValues = propertyValuesList.ToArray();
			fields = fieldList.ToArray();
			fieldValues = fieldValuesList.ToArray();
		}

		public static IEnumerable<CustomAttributeInfo> GetNonInheritableAttributes(this MemberInfo member)
		{
			Debug.Assert(member != null, "member != null");
#if FEATURE_LEGACY_REFLECTION_API
			var attributes = CustomAttributeData.GetCustomAttributes(member);
#else
			var attributes = member.CustomAttributes;
#endif

			foreach (var attribute in attributes)
			{
#if FEATURE_LEGACY_REFLECTION_API
				var attributeType = attribute.Constructor.DeclaringType;
#else
				var attributeType = attribute.AttributeType;
#endif
				if (ShouldSkipAttributeReplication(attributeType))
				{
					continue;
				}

				CustomAttributeInfo info;
				try
				{
					info = CreateInfo(attribute);
				}
				catch (ArgumentException e)
				{
					var message =
						string.Format(
							"Due to limitations in CLR, DynamicProxy was unable to successfully replicate non-inheritable attribute {0} on {1}{2}. " +
							"To avoid this error you can chose not to replicate this attribute type by calling '{3}.Add(typeof({0}))'.",
							attributeType.FullName,
							member.DeclaringType.FullName,
#if FEATURE_LEGACY_REFLECTION_API
							(member is Type) ? "" : ("." + member.Name),
#else
							(member is TypeInfo) ? "" : ("." + member.Name),
#endif
							typeof(AttributesToAvoidReplicating).FullName);
					throw new ProxyGenerationException(message, e);
				}
				if (info != null)
				{
					yield return info;
				}
			}
		}

		public static IEnumerable<CustomAttributeInfo> GetNonInheritableAttributes(this ParameterInfo parameter)
		{
			Debug.Assert(parameter != null, "parameter != null");

#if FEATURE_LEGACY_REFLECTION_API
			var attributes = CustomAttributeData.GetCustomAttributes(parameter);
#else
			var attributes = parameter.CustomAttributes;
#endif

			foreach (var attribute in attributes)
			{
#if FEATURE_LEGACY_REFLECTION_API
				var attributeType = attribute.Constructor.DeclaringType;
#else
				var attributeType = attribute.AttributeType;
#endif

				if (ShouldSkipAttributeReplication(attributeType))
				{
					continue;
				}

				var info = CreateInfo(attribute);
				if (info != null)
				{
					yield return info;
				}
			}
		}

		/// <summary>
		///   Attributes should be replicated if they are non-inheritable,
		///   but there are some special cases where the attributes means
		///   something to the CLR, where they should be skipped.
		/// </summary>
		private static bool ShouldSkipAttributeReplication(Type attribute)
		{
			if (attribute.GetTypeInfo().IsPublic == false)
			{
				return true;
			}

			if (SpecialCaseAttributeThatShouldNotBeReplicated(attribute))
			{
				return true;
			}

			// Later, there might be more special cases requiring attribute replication,
			// which might justify creating a `SpecialCaseAttributeThatShouldBeReplicated`
			// method and an `AttributesToAlwaysReplicate` class. For the moment, `Param-
			// ArrayAttribute` is the only special case, so keep it simple for now:
			if (attribute == typeof(ParamArrayAttribute))
			{
				return false;
			}

			var attrs = attribute.GetTypeInfo().GetCustomAttributes<AttributeUsageAttribute>(true).ToArray();
			if (attrs.Length != 0)
			{
				return attrs[0].Inherited;
			}

			return true;
		}

		private static bool SpecialCaseAttributeThatShouldNotBeReplicated(Type attribute)
		{
			return AttributesToAvoidReplicating.ShouldAvoid(attribute);
		}

		public static CustomAttributeInfo CreateInfo<TAttribute>() where TAttribute : Attribute, new()
		{
			var constructor = typeof(TAttribute).GetConstructor(Type.EmptyTypes);
			Debug.Assert(constructor != null, "constructor != null");

			return new CustomAttributeInfo(constructor, new object[0]);
		}

		public static CustomAttributeInfo CreateInfo(Type attribute, object[] constructorArguments)
		{
			Debug.Assert(attribute != null, "attribute != null");
			Debug.Assert(typeof(Attribute).IsAssignableFrom(attribute), "typeof(Attribute).IsAssignableFrom(attribute)");
			Debug.Assert(constructorArguments != null, "constructorArguments != null");

			var constructor = attribute.GetConstructor(GetTypes(constructorArguments));
			Debug.Assert(constructor != null, "constructor != null");

			return new CustomAttributeInfo(constructor, constructorArguments);
		}

		private static Type[] GetTypes(object[] objects)
		{
			var types = new Type[objects.Length];
			for (var i = 0; i < types.Length; i++)
			{
				types[i] = objects[i].GetType();
			}
			return types;
		}
	}
}