// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;

#if SILVERLIGHT
	using Castle.Core.Extensions;
#endif

	internal delegate GenericTypeParameterBuilder[] ApplyGenArgs(String[] argumentNames);

	internal class GenericUtil
	{
		public static void PopulateGenericArguments(
			AbstractTypeEmitter parentEmitter,
			Dictionary<String, GenericTypeParameterBuilder> name2GenericType)
		{
			if (parentEmitter.GenericTypeParams == null) return;

			foreach (GenericTypeParameterBuilder genType in parentEmitter.GenericTypeParams)
			{
				name2GenericType.Add(genType.Name, genType);
			}
		}

		public static GenericTypeParameterBuilder[] CopyGenericArguments(
			MethodInfo methodToCopyGenericsFrom,
			TypeBuilder builder,
			Dictionary<String, GenericTypeParameterBuilder> name2GenericType)
		{
			return
				CopyGenericArguments(methodToCopyGenericsFrom, name2GenericType,
				                       builder.DefineGenericParameters);
		}

		public static GenericTypeParameterBuilder[] CopyGenericArguments(
			MethodInfo methodToCopyGenericsFrom, 
			MethodBuilder builder,
			Dictionary<String, GenericTypeParameterBuilder> name2GenericType)
		{
			return
				CopyGenericArguments(methodToCopyGenericsFrom, name2GenericType,
				                       builder.DefineGenericParameters);
		}

		private static GenericTypeParameterBuilder[] CopyGenericArguments(
			MethodInfo methodToCopyGenericsFrom,
			Dictionary<String, GenericTypeParameterBuilder> name2GenericType,
			ApplyGenArgs genericParameterGenerator)
		{
			var originalGenericArguments = methodToCopyGenericsFrom.GetGenericArguments();
			if (originalGenericArguments.Length == 0)
			{
				return null;
			}

			var argumentNames = GetArgumentNames(originalGenericArguments);
			var newGenericParameters = genericParameterGenerator(argumentNames);

			for (int i = 0; i < newGenericParameters.Length; i++)
			{
				try
				{
					var attributes = originalGenericArguments[i].GenericParameterAttributes;
					var types = originalGenericArguments[i].GetGenericParameterConstraints();

					newGenericParameters[i].SetGenericParameterAttributes(attributes);

#if SILVERLIGHT
						Type[] interfacesConstraints = Castle.Core.Extensions.SilverlightExtensions.FindAll(types, delegate(Type type) { return type.IsInterface; });

						Type baseClassConstraint = Castle.DynamicProxy.SilverlightExtensions.Extensions.Find(types, delegate(Type type) { return type.IsClass; });
#else
					var interfacesConstraints = Array.FindAll(types, type => type.IsInterface);
					var baseClassConstraint = Array.Find(types, type => type.IsClass);
#endif

					if (interfacesConstraints.Length != 0)
					{
						for (int j = 0; j < interfacesConstraints.Length; ++j)
						{
							interfacesConstraints[j] =
								AdjustConstraintToNewGenericParameters(interfacesConstraints[j], methodToCopyGenericsFrom,
								                                       originalGenericArguments, newGenericParameters);
						}
						newGenericParameters[i].SetInterfaceConstraints(interfacesConstraints);
					}

					if (baseClassConstraint != null)
					{
						baseClassConstraint = AdjustConstraintToNewGenericParameters(baseClassConstraint, methodToCopyGenericsFrom,
						                                                             originalGenericArguments, newGenericParameters);
						newGenericParameters[i].SetBaseTypeConstraint(baseClassConstraint);
					}
					CopyNonInheritableAttributes(newGenericParameters[i], originalGenericArguments[i]);
				}
				catch (NotSupportedException)
				{
					// Doesnt matter

					newGenericParameters[i].SetGenericParameterAttributes(GenericParameterAttributes.None);
				}

				name2GenericType[argumentNames[i]] = newGenericParameters[i];
			}

			return newGenericParameters;
		}

		private static void CopyNonInheritableAttributes(GenericTypeParameterBuilder newGenericParameter, Type originalGenericArgument)
		{
			foreach (var attribute in AttributeUtil.GetNonInheritableAttributes(originalGenericArgument))
			{
				newGenericParameter.SetCustomAttribute(attribute);
			}
		}

		private static string[] GetArgumentNames(Type[] originalGenericArguments)
		{
			String[] argumentNames = new String[originalGenericArguments.Length];

			for (int i = 0; i < argumentNames.Length; i++)
			{
				argumentNames[i] = originalGenericArguments[i].Name;
			}
			return argumentNames;
		}

		private static Type AdjustConstraintToNewGenericParameters(
			Type constraint, MethodInfo methodToCopyGenericsFrom, Type[] originalGenericParameters, GenericTypeParameterBuilder[] newGenericParameters)
		{
			if (constraint.IsGenericType)
			{
				Type[] genericArgumentsOfConstraint = constraint.GetGenericArguments();

				for (int i = 0; i < genericArgumentsOfConstraint.Length; ++i)
				{
					genericArgumentsOfConstraint[i] =
							AdjustConstraintToNewGenericParameters(genericArgumentsOfConstraint[i], methodToCopyGenericsFrom, originalGenericParameters, newGenericParameters);
				}
				return constraint.GetGenericTypeDefinition().MakeGenericType(genericArgumentsOfConstraint);
			}
			else if (constraint.IsGenericParameter)
			{
				// Determine the source of the parameter
				if (constraint.DeclaringMethod != null)
				{
					// constraint comes from the method
					int index = Array.IndexOf(originalGenericParameters, constraint);
					Trace.Assert(index != -1, 
							"When a generic method parameter has a constraint on another method parameter, both parameters must be declared on the same method.");
						return newGenericParameters[index];
				}
				else // parameter from surrounding type
				{
					Trace.Assert(constraint.DeclaringType.IsGenericTypeDefinition);
					Trace.Assert(methodToCopyGenericsFrom.DeclaringType.IsGenericType 
							&& constraint.DeclaringType == methodToCopyGenericsFrom.DeclaringType.GetGenericTypeDefinition (),
							"When a generic method parameter has a constraint on a generic type parameter, the generic type must be the declaring typer of the method.");

					int index = Array.IndexOf(constraint.DeclaringType.GetGenericArguments(), constraint);
					Trace.Assert (index != -1, "The generic parameter comes from the given type.");
					return methodToCopyGenericsFrom.DeclaringType.GetGenericArguments() [index]; // these are the actual, concrete types
				}
			}
			else
			{
				return constraint;
			}
		}

		public static Type[] ExtractParametersTypes(
			ParameterInfo[] baseMethodParameters,
			Dictionary<String, GenericTypeParameterBuilder> name2GenericType)
		{
			Type[] newParameters = new Type[baseMethodParameters.Length];

			for (int i = 0; i < baseMethodParameters.Length; i++)
			{
				ParameterInfo param = baseMethodParameters[i];
				Type paramType = param.ParameterType;

				newParameters[i] = ExtractCorrectType(paramType, name2GenericType);
			}

			return newParameters;
		}

		public static Type ExtractCorrectType(
			Type paramType,
			Dictionary<string, GenericTypeParameterBuilder> name2GenericType)
		{
			if (paramType.IsArray)
			{
				int rank = paramType.GetArrayRank();

				Type underlyingType = paramType.GetElementType();

				if (underlyingType.IsGenericParameter)
				{
					GenericTypeParameterBuilder genericType;
					if (name2GenericType.TryGetValue(underlyingType.Name, out genericType) == false)
						return paramType;

					if (rank == 1)
					{
						return genericType.MakeArrayType();
					}
					return genericType.MakeArrayType(rank);
				}
				if (rank == 1)
				{
					return underlyingType.MakeArrayType();
				}
				return underlyingType.MakeArrayType(rank);
			}

			if (paramType.IsGenericParameter)
			{
				GenericTypeParameterBuilder value;
				if (name2GenericType.TryGetValue(paramType.Name, out value))
					return value;
			}

			return paramType;
		}
	}
}
