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

#if !SILVERLIGHT
		public static CustomAttributeBuilder CreateBuilder(CustomAttributeData attribute)
		{
			Debug.Assert(attribute != null, "attribute != null");

			PropertyInfo[] properties;
			object[] propertyValues;
			FieldInfo[] fields;
			object[] fieldValues;
			GetSettersAndFields(attribute.NamedArguments, out properties, out propertyValues, out fields, out fieldValues);
			var constructorArgs = GetArguments(attribute.ConstructorArguments);
			return new CustomAttributeBuilder(attribute.Constructor,
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
			if (argument.ArgumentType.IsArray == false)
			{
				return value;
			}
			//special case for handling arrays in attributes
			var arguments = GetArguments((IList<CustomAttributeTypedArgument>)value);
			var array = Array.CreateInstance(argument.ArgumentType.GetElementType() ?? typeof(object), arguments.Length);
			arguments.CopyTo(array, 0);
			return array;
		}

		private static void GetSettersAndFields(IEnumerable<CustomAttributeNamedArgument> namedArguments,
		                                        out PropertyInfo[] properties, out object[] propertyValues,
		                                        out FieldInfo[] fields, out object[] fieldValues)
		{
			var propertyList = new List<PropertyInfo>();
			var propertyValuesList = new List<object>();
			var fieldList = new List<FieldInfo>();
			var fieldValuesList = new List<object>();
			foreach (var argument in namedArguments)
			{
				switch (argument.MemberInfo.MemberType)
				{
					case MemberTypes.Property:
						propertyList.Add(argument.MemberInfo as PropertyInfo);
						propertyValuesList.Add(ReadAttributeValue(argument.TypedValue));
						break;
					case MemberTypes.Field:
						fieldList.Add(argument.MemberInfo as FieldInfo);
						fieldValuesList.Add(ReadAttributeValue(argument.TypedValue));
						break;
					default:
						// NOTE: can this ever happen?
						throw new ArgumentException(string.Format("Unexpected member type {0} in custom attribute.",
						                                          argument.MemberInfo.MemberType));
				}
			}

			properties = propertyList.ToArray();
			propertyValues = propertyValuesList.ToArray();
			fields = fieldList.ToArray();
			fieldValues = fieldValuesList.ToArray();
		}
#else
		// CustomAttributeData is internal in Silverlight
#endif

		public static IEnumerable<CustomAttributeBuilder> GetNonInheritableAttributes(this MemberInfo member)
		{
			Debug.Assert(member != null, "member != null");
			var attributes =
#if SILVERLIGHT
				member.GetCustomAttributes(false);
#else
				CustomAttributeData.GetCustomAttributes(member);
#endif

			foreach (var attribute in attributes)
			{
				var attributeType =
#if SILVERLIGHT
				attribute.GetType();
#else
					attribute.Constructor.DeclaringType;
#endif
				if (ShouldSkipAttributeReplication(attributeType))
				{
					continue;
				}

				CustomAttributeBuilder builder;
				try
				{
					builder = CreateBuilder(attribute
#if SILVERLIGHT
					as Attribute
#endif
						);
				}
				catch (ArgumentException e)
				{
					var message =
						string.Format(
							"Due to limitations in CLR, DynamicProxy was unable to successfully replicate non-inheritable attribute {0} on {1}{2}. To avoid this error you can chose not to replicate this attribute type by calling '{3}.Add(typeof({0}))'.",
							attributeType.FullName, (member.ReflectedType == null) ? "" : member.ReflectedType.FullName,
							(member is Type) ? "" : ("." + member.Name), typeof(AttributesToAvoidReplicating).FullName);
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
			var attributes =
#if SILVERLIGHT
				parameter.GetCustomAttributes(false);
#else
				CustomAttributeData.GetCustomAttributes(parameter);
#endif

			foreach (var attribute in attributes)
			{
				var attributeType =
#if SILVERLIGHT
				attribute.GetType();
#else
					attribute.Constructor.DeclaringType;
#endif
				if (ShouldSkipAttributeReplication(attributeType))
				{
					continue;
				}

				var builder = CreateBuilder(attribute
#if SILVERLIGHT
					as Attribute
#endif
					);
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
			if (attribute.IsPublic == false)
			{
				return true;
			}

			if (SpecialCaseAttributThatShouldNotBeReplicated(attribute))
			{
				return true;
			}

			var attrs = attribute.GetCustomAttributes(typeof(AttributeUsageAttribute), true);

			if (attrs.Length != 0)
			{
				var usage = (AttributeUsageAttribute)attrs[0];

				return usage.Inherited;
			}

			return true;
		}

		private static bool SpecialCaseAttributThatShouldNotBeReplicated(Type attribute)
		{
			return AttributesToAvoidReplicating.Contains(attribute);
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

		// NOTE: Use other overloads if possible. This method is here to support Silverlight and legacy scenarios.
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