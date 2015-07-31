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

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;

	using Castle.Core.Internal;
	using Castle.DynamicProxy.Internal;

	internal delegate GenericTypeParameterBuilder[] ApplyGenArgs(String[] argumentNames);

	internal class GenericUtil
	{
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

		public static Type ExtractCorrectType(Type paramType, Dictionary<string, GenericTypeParameterBuilder> name2GenericType)
		{
			if (paramType.GetTypeInfo().IsArray)
			{
				var rank = paramType.GetArrayRank();

				var underlyingType = paramType.GetElementType();

				if (underlyingType.GetTypeInfo().IsGenericParameter)
				{
					GenericTypeParameterBuilder genericType;
					if (name2GenericType.TryGetValue(underlyingType.Name, out genericType) == false)
					{
						return paramType;
					}

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

			if (paramType.GetTypeInfo().IsGenericParameter)
			{
				GenericTypeParameterBuilder value;
				if (name2GenericType.TryGetValue(paramType.Name, out value))
				{
#if NETCORE
					return value.MakeGenericType(); 
#else
					return value;
#endif
				}
			}

			return paramType;
		}

		public static Type[] ExtractParametersTypes(
			ParameterInfo[] baseMethodParameters,
			Dictionary<String, GenericTypeParameterBuilder> name2GenericType)
		{
			var newParameters = new Type[baseMethodParameters.Length];

			for (var i = 0; i < baseMethodParameters.Length; i++)
			{
				var param = baseMethodParameters[i];
				var paramType = param.ParameterType;

				newParameters[i] = ExtractCorrectType(paramType, name2GenericType);
			}

			return newParameters;
		}

		public static Dictionary<string, GenericTypeParameterBuilder> GetGenericArgumentsMap(AbstractTypeEmitter parentEmitter)
		{
			if (parentEmitter.GenericTypeParams == null || parentEmitter.GenericTypeParams.Length == 0)
			{
				return new Dictionary<string, GenericTypeParameterBuilder>(0);
			}

			var name2GenericType = new Dictionary<string, GenericTypeParameterBuilder>(parentEmitter.GenericTypeParams.Length);
			foreach (var genType in parentEmitter.GenericTypeParams)
			{
				name2GenericType.Add(genType.Name, genType);
			}
			return name2GenericType;
		}

		private static Type AdjustConstraintToNewGenericParameters(
			Type constraint, MethodInfo methodToCopyGenericsFrom, Type[] originalGenericParameters,
			GenericTypeParameterBuilder[] newGenericParameters)
		{
			if (constraint.GetTypeInfo().IsGenericType)

			{
				var genericArgumentsOfConstraint = constraint.GetGenericArguments();

				for (var i = 0; i < genericArgumentsOfConstraint.Length; ++i)
				{
					genericArgumentsOfConstraint[i] =
						AdjustConstraintToNewGenericParameters(genericArgumentsOfConstraint[i], methodToCopyGenericsFrom,
						                                       originalGenericParameters, newGenericParameters);
				}
				return constraint.GetGenericTypeDefinition().MakeGenericType(genericArgumentsOfConstraint);
			}
			else if (constraint.GetTypeInfo().IsGenericParameter)
			{
				// Determine the source of the parameter
#if NETCORE
				if (constraint.GetTypeInfo().DeclaringMethod != null)
#else
				if (constraint.DeclaringMethod != null)
#endif
				{
					// constraint comes from the method
					var index = Array.IndexOf(originalGenericParameters, constraint);
					Trace.Assert(index != -1,
					             "When a generic method parameter has a constraint on another method parameter, both parameters must be declared on the same method.");
#if NETCORE
					return newGenericParameters[index].AsType();
#else
					return newGenericParameters[index];
#endif
				}
				else // parameter from surrounding type
				{
					Trace.Assert(constraint.DeclaringType.GetTypeInfo().IsGenericTypeDefinition);
					Trace.Assert(methodToCopyGenericsFrom.DeclaringType.GetTypeInfo().IsGenericType

					             && constraint.DeclaringType == methodToCopyGenericsFrom.DeclaringType.GetGenericTypeDefinition(),
					             "When a generic method parameter has a constraint on a generic type parameter, the generic type must be the declaring typer of the method.");

					var index = Array.IndexOf(constraint.DeclaringType.GetGenericArguments(), constraint);
					Trace.Assert(index != -1, "The generic parameter comes from the given type.");
					return methodToCopyGenericsFrom.DeclaringType.GetGenericArguments()[index]; // these are the actual, concrete types
				}
			}
			else
			{
				return constraint;
			}
		}

		private static Type[] AdjustGenericConstraints(MethodInfo methodToCopyGenericsFrom,
		                                               GenericTypeParameterBuilder[] newGenericParameters,
		                                               Type[] originalGenericArguments,
		                                               Type[] constraints)
		{
			// HACK: the mono runtime has a strange bug where assigning to the constraints
			//       parameter and returning it throws, so we'll create a new array.
			//       System.ArrayTypeMismatchException : Source array type cannot be assigned to destination array type.
			Type[] adjustedConstraints = new Type[constraints.Length];
			for (var i = 0; i < constraints.Length; i++)
			{
				adjustedConstraints[i] = AdjustConstraintToNewGenericParameters(constraints[i],
					methodToCopyGenericsFrom, originalGenericArguments, newGenericParameters);
			}
			return adjustedConstraints;
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

			for (var i = 0; i < newGenericParameters.Length; i++)
			{
				try
				{
#if NETCORE
					var attributes = originalGenericArguments[i].GetTypeInfo().GenericParameterAttributes;
					newGenericParameters[i].SetGenericParameterAttributes(attributes);
					var constraints = AdjustGenericConstraints(methodToCopyGenericsFrom, newGenericParameters, originalGenericArguments, originalGenericArguments[i].GetTypeInfo().GetGenericParameterConstraints());
#else
					var attributes = originalGenericArguments[i].GenericParameterAttributes;
					newGenericParameters[i].SetGenericParameterAttributes(attributes);
					var constraints = AdjustGenericConstraints(methodToCopyGenericsFrom, newGenericParameters, originalGenericArguments, originalGenericArguments[i].GetGenericParameterConstraints());
#endif
					newGenericParameters[i].SetInterfaceConstraints(constraints);
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

		private static void CopyNonInheritableAttributes(GenericTypeParameterBuilder newGenericParameter,
		                                                 Type originalGenericArgument)
		{
#if NETCORE
			foreach (var attribute in originalGenericArgument.GetTypeInfo().GetNonInheritableAttributes())
#else
			foreach (var attribute in originalGenericArgument.GetNonInheritableAttributes())
#endif
			{
				newGenericParameter.SetCustomAttribute(attribute);
			}
		}

		private static string[] GetArgumentNames(Type[] originalGenericArguments)
		{
			var argumentNames = new String[originalGenericArguments.Length];

			for (var i = 0; i < argumentNames.Length; i++)
			{
				argumentNames[i] = originalGenericArguments[i].Name;
			}
			return argumentNames;
		}
	}
}
