// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

	internal static class AttributeUtil
	{
		public static CustomAttributeInfo CreateInfo(CustomAttributeData attribute)
		{
			Debug.Assert(attribute != null, "attribute != null");

			object[] constructorArgs = GetArguments(attribute.ConstructorArguments);

			PropertyInfo[] properties;
			object[] propertyValues;
			FieldInfo[] fields;
			object[] fieldValues;
			GetSettersAndFields(
				attribute.AttributeType,
				attribute.NamedArguments, out properties, out propertyValues, out fields, out fieldValues);

			return new CustomAttributeInfo(attribute.Constructor,
			                               constructorArgs,
			                               properties,
			                               propertyValues,
			                               fields,
			                               fieldValues);
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

			if (argument.ArgumentType.IsArray && value is IList<CustomAttributeTypedArgument> values)
			{
				// `CustomAttributeInfo` represents array values as `ReadOnlyCollection<CustomAttributeTypedArgument>`,
				// but `CustomAttributeBuilder` will require plain arrays, so we need a (recursive) conversion:
				var arguments = GetArguments(values);
				var array = new object[arguments.Length];
				arguments.CopyTo(array, 0);
				return array;
			}

			return value;
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
			}

			properties = propertyList.ToArray();
			propertyValues = propertyValuesList.ToArray();
			fields = fieldList.ToArray();
			fieldValues = fieldValuesList.ToArray();
		}

		public static IEnumerable<CustomAttributeInfo> GetNonInheritableAttributes(this MemberInfo member)
		{
			Debug.Assert(member != null, "member != null");
			var attributes = member.CustomAttributes;

			foreach (var attribute in attributes)
			{
				var attributeType = attribute.AttributeType;
				if (ShouldSkipAttributeReplication(attributeType, ignoreInheritance: false))
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
							(member is TypeInfo) ? "" : ("." + member.Name),
							typeof(AttributesToAvoidReplicating).FullName);
					throw new NotSupportedException(message, e);
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

			var attributes = parameter.CustomAttributes;

			var ignoreInheritance = parameter.Member is ConstructorInfo;

			foreach (var attribute in attributes)
			{
				var attributeType = attribute.AttributeType;

				if (ShouldSkipAttributeReplication(attributeType, ignoreInheritance))
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
		private static bool ShouldSkipAttributeReplication(Type attribute, bool ignoreInheritance)
		{
			if (attribute.IsPublic == false && attribute.IsNestedPublic == false)
			{
				return true;
			}

			if (AttributesToAvoidReplicating.ShouldAvoid(attribute))
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

			if (!ignoreInheritance)
			{
				var attrs = attribute.GetCustomAttributes<AttributeUsageAttribute>(true).ToArray();
				if (attrs.Length != 0)
				{
					return attrs[0].Inherited;
				}

				return true;
			}

			return false;
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