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
	using Castle.DynamicProxy.Generators;

	public static class AttributeUtil
	{
		private static IDictionary<Type,IAttributeDisassembler> disassemblers = new Dictionary<Type, IAttributeDisassembler>();

		/// <summary>
		/// Registers custom disassembler to handle disassembly of specified type of attributes.
		/// </summary>
		/// <typeparam name="TAttribute">Type of attributes to handle</typeparam>
		/// <param name="disassembler">Disassembler converting existing instances of Attributes to CustomAttributeBuilders</param>
		public static void AddDisassembler<TAttribute>(IAttributeDisassembler disassembler) where TAttribute : Attribute
		{
			if (disassembler == null) throw new ArgumentNullException("disassembler");

			disassemblers[typeof (TAttribute)] = disassembler;
		}

#if !SLIVERLIGHT
		public static CustomAttributeBuilder CreateBuilder(CustomAttributeData attribute)
		{
			Debug.Assert(attribute != null, "attribute != null");

			PropertyInfo[] properties;
			object[] propertyValues;
			FieldInfo[] fields;
			object[] fieldValues;
			GetSettersAndFields(attribute.NamedArguments, out properties, out propertyValues, out fields, out fieldValues);

			return new CustomAttributeBuilder(attribute.Constructor,
			                                  GetCtorArguments(attribute.ConstructorArguments),
			                                  properties,
			                                  propertyValues,
			                                  fields,
			                                  fieldValues);
		}

		private static object[] GetCtorArguments(IList<CustomAttributeTypedArgument> constructorArguments)
		{
			var arguments = new object[constructorArguments.Count];
			for (int i = 0; i < constructorArguments.Count; i++)
			{
				arguments[i] = constructorArguments[i].Value;
			}

			return arguments;
		}
#else
#warning CustomAttributeData is internal in Silverlight
#endif

		public static IEnumerable<CustomAttributeBuilder> GetNonInheritableAttributes(MemberInfo member)
		{
			Debug.Assert(member != null, "member != null");
			var attributes =
#if SILVERLIGHT
				member.GetCustomAttributes(false) as Attribute[];
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
				if (ShouldSkipAttributeReplication(attributeType)) continue;

				yield return CreateBuilder(attribute);
			}
		}

		/// <summary>
		/// Attributes should be replicated if they are non-inheritable,
		/// but there are some special cases where the attributes means
		/// something to the CLR, where they should be skipped.
		/// </summary>
		private static bool ShouldSkipAttributeReplication(Type attribute)
		{
			if (SpecialCaseAttributThatShouldNotBeReplicated(attribute))
				return true;

			object[] attrs = attribute.GetCustomAttributes(typeof(AttributeUsageAttribute), true);

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
			ConstructorInfo constructor = typeof (TAttribute).GetConstructor(Type.EmptyTypes);
			Debug.Assert(constructor != null, "constructor != null");

			return new CustomAttributeBuilder(constructor, new object[0]);
		}

		public static CustomAttributeBuilder CreateBuilder(Type attribute, object[] constructorArguments)
		{
			Debug.Assert(attribute != null, "attribute != null");
			Debug.Assert(typeof (Attribute).IsAssignableFrom(attribute), "typeof(Attribute).IsAssignableFrom(attribute)");
			Debug.Assert(constructorArguments != null, "constructorArguments != null");

			ConstructorInfo constructor = attribute.GetConstructor(GetTypes(constructorArguments));
			Debug.Assert(constructor != null, "constructor != null");

			return new CustomAttributeBuilder(constructor, constructorArguments);
		}

		// NOTE: Use other overloads if possible. This method is here to support Silverlight and legacy scenarios.
		internal static CustomAttributeBuilder CreateBuilder(Attribute attribute)
		{
			var type = attribute.GetType();

			IAttributeDisassembler disassembler;
			if(disassemblers.TryGetValue(type,out disassembler))
			{
				return disassembler.Disassemble(attribute);
			}
			return new AttributeDisassembler().Disassemble(attribute);
		}

		private static Type[] GetTypes(object[] objects)
		{
			var types = new Type[objects.Length];
			for (int i = 0; i < types.Length; i++)
			{
				types[i] = objects[i].GetType();
			}
			return types;
		}

		private static void GetSettersAndFields(IList<CustomAttributeNamedArgument> namedArguments,
		                                        out PropertyInfo[] properties, out object[] propertyValues,
		                                        out FieldInfo[] fields, out object[] fieldValues)
		{
			var propertyList = new List<PropertyInfo>();
			var propertyValuesList = new List<object>();
			var fieldList = new List<FieldInfo>();
			var fieldValuesList = new List<object>();
			foreach (CustomAttributeNamedArgument argument in namedArguments)
			{
				switch (argument.MemberInfo.MemberType)
				{
					case MemberTypes.Property:
						propertyList.Add(argument.MemberInfo as PropertyInfo);
						propertyValuesList.Add(argument.TypedValue.Value);
						break;
					case MemberTypes.Field:
						fieldList.Add(argument.MemberInfo as FieldInfo);
						fieldValuesList.Add(argument.TypedValue.Value);
						break;
					default:
						throw new ArgumentException(string.Format("Unexpected member type {0} in custom attribute.",
						                                          argument.MemberInfo.MemberType));
				}
			}

			properties = propertyList.ToArray();
			propertyValues = propertyValuesList.ToArray();
			fields = fieldList.ToArray();
			fieldValues = fieldValuesList.ToArray();
		}

#if !SILVERLIGHT
		[Serializable]
#endif
		private class AttributeDisassembler : IAttributeDisassembler
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
					string message = "Dynamic Proxy was unable to disassemble attribute " + type.Name +
					                 " using default AttributeDisassembler. " +
					                 "To handle the disassembly process properly implement the IAttributeDisassembler interface, " +
					                 "and register your disassembler to handle this type of attributes using " +
					                 typeof (AttributeUtil).Name + ".AddDisassembler<" + type.Name + ">(yourDisassembler) method";
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
				var ctorArgs = new object[0];

				ci = attType.GetConstructors()[0];

				ParameterInfo[] constructorParams = ci.GetParameters();

				if (constructorParams.Length != 0)
				{
					ctorArgs = new object[constructorParams.Length];

					InitializeConstructorArgs(attType, attribute, ctorArgs, constructorParams);
				}

				return ctorArgs;
			}

			private static object[] GetPropertyValues(Type attType, out PropertyInfo[] properties, Attribute original,
			                                          Attribute replicated)
			{
				List<PropertyInfo> propertyCandidates = GetPropertyCandidates(attType);

				var selectedValues = new List<object>(propertyCandidates.Count);
				var selectedProperties = new List<PropertyInfo>(propertyCandidates.Count);
				foreach (PropertyInfo property in propertyCandidates)
				{
					object originalValue = property.GetValue(original, null);
					object replicatedValue = property.GetValue(replicated, null);
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
				FieldInfo[] fieldsCandidates = attType.GetFields(BindingFlags.Public | BindingFlags.Instance);

				var selectedValues = new List<object>(fieldsCandidates.Length);
				var selectedFields = new List<FieldInfo>(fieldsCandidates.Length);
				foreach (FieldInfo field in fieldsCandidates)
				{
					object originalValue = field.GetValue(original);
					object replicatedValue = field.GetValue(replicated);
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

			public bool Equals(AttributeDisassembler other)
			{
				return !ReferenceEquals(null, other);
			}

			public override bool Equals(object obj)
			{
				if (ReferenceEquals(null, obj)) return false;
				if (ReferenceEquals(this, obj)) return true;
				if (obj.GetType() != typeof (AttributeDisassembler)) return false;
				return Equals((AttributeDisassembler) obj);
			}

			public override int GetHashCode()
			{
				return GetType().GetHashCode();
			}
		}
	}
}