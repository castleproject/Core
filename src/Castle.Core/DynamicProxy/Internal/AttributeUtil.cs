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
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Generators;

	public static class AttributeUtil
	{
		private static readonly IDictionary<Type, IAttributeDisassembler> disassemblers =
			new Dictionary<Type, IAttributeDisassembler>();

		private static IAttributeDisassembler fallbackDisassembler = new AttributeDisassembler();

		public static IAttributeDisassembler FallbackDisassembler
		{
			get { return fallbackDisassembler; }
			set { fallbackDisassembler = value; }
		}

		/// <summary>
		///   Registers custom disassembler to handle disassembly of specified type of attributes.
		/// </summary>
		/// <typeparam name = "TAttribute">Type of attributes to handle</typeparam>
		/// <param name = "disassembler">Disassembler converting existing instances of Attributes to CustomAttributeBuilders</param>
		/// <remarks>
		///   When disassembling an attribute Dynamic Proxy will first check if an custom disassembler has been registered to handle attributes of that type, 
		///   and if none is found, it'll use the <see cref = "FallbackDisassembler" />.
		/// </remarks>
		public static void AddDisassembler<TAttribute>(IAttributeDisassembler disassembler) where TAttribute : Attribute
		{
			if (disassembler == null)
			{
				throw new ArgumentNullException("disassembler");
			}

			disassemblers[typeof(TAttribute)] = disassembler;
		}

		public static CustomAttributeBuilder CreateBuilder(CustomAttributeData attribute)
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

			return new CustomAttributeBuilder(constructor,
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

		public static IEnumerable<CustomAttributeBuilder> GetNonInheritableAttributes(this MemberInfo member)
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

				CustomAttributeBuilder builder;
				try
				{
					builder = CreateBuilder(attribute);
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
				if (builder != null)
				{
					yield return builder;
				}
			}
		}

		public static IEnumerable<CustomAttributeBuilder> GetNonInheritableAttributes(this ParameterInfo parameter)
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

				var builder = CreateBuilder(attribute);
				if (builder != null)
				{
					yield return builder;
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

		public static CustomAttributeBuilder CreateBuilder<TAttribute>() where TAttribute : Attribute, new()
		{
			var constructor = typeof(TAttribute).GetConstructor(Type.EmptyTypes);
			Debug.Assert(constructor != null, "constructor != null");

			return new CustomAttributeBuilder(constructor, new object[0]);
		}

		public static CustomAttributeBuilder CreateBuilder(Type attribute, object[] constructorArguments)
		{
			Debug.Assert(attribute != null, "attribute != null");
			Debug.Assert(typeof(Attribute).IsAssignableFrom(attribute), "typeof(Attribute).IsAssignableFrom(attribute)");
			Debug.Assert(constructorArguments != null, "constructorArguments != null");

			var constructor = attribute.GetConstructor(GetTypes(constructorArguments));
			Debug.Assert(constructor != null, "constructor != null");

			return new CustomAttributeBuilder(constructor, constructorArguments);
		}

		// NOTE: Use other overloads if possible. This method is here to support legacy scenarios.
		internal static CustomAttributeBuilder CreateBuilder(Attribute attribute)
		{
			var type = attribute.GetType();

			IAttributeDisassembler disassembler;
			if (disassemblers.TryGetValue(type, out disassembler))
			{
				return disassembler.Disassemble(attribute);
			}
			return FallbackDisassembler.Disassemble(attribute);
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