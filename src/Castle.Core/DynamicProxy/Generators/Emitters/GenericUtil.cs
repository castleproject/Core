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

namespace Castle.DynamicProxy.Generators.Emitters
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;

	using Castle.Core.Internal;
	using Castle.DynamicProxy.Internal;

	internal delegate GenericTypeParameterBuilder[] ApplyGenArgs(string[] argumentNames);

	internal class GenericUtil
	{
		public static GenericTypeParameterBuilder[] CopyGenericArguments(
			MethodInfo methodToCopyGenericsFrom,
			TypeBuilder builder)
		{
			var _ = new Dictionary<string, GenericTypeParameterBuilder>();
			return CopyGenericArguments(methodToCopyGenericsFrom, _, builder.DefineGenericParameters);
		}

		public static GenericTypeParameterBuilder[] CopyGenericArguments(
			MethodInfo methodToCopyGenericsFrom,
			MethodBuilder builder)
		{
			var _ = new Dictionary<string, GenericTypeParameterBuilder>();
			return CopyGenericArguments(methodToCopyGenericsFrom, _, builder.DefineGenericParameters);
		}

		private static Type AdjustConstraintToNewGenericParameters(
			Type constraint, MethodInfo methodToCopyGenericsFrom, Type[] originalGenericParameters,
			GenericTypeParameterBuilder[] newGenericParameters)
		{
			if (constraint.IsGenericType)
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
			else if (constraint.IsGenericParameter)
			{
				// Determine the source of the parameter
				if (constraint.DeclaringMethod != null)
				{
					// constraint comes from the method
					var index = Array.IndexOf(originalGenericParameters, constraint);
					Trace.Assert(index != -1,
					             "When a generic method parameter has a constraint on another method parameter, both parameters must be declared on the same method.");
					return newGenericParameters[index];
				}
				else // parameter from surrounding type
				{
					Trace.Assert(constraint.DeclaringType.IsGenericTypeDefinition);
					Trace.Assert(methodToCopyGenericsFrom.DeclaringType.IsGenericType
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
			Dictionary<string, GenericTypeParameterBuilder> name2GenericType,
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
					var attributes = originalGenericArguments[i].GenericParameterAttributes;
					newGenericParameters[i].SetGenericParameterAttributes(attributes);
					var constraints = AdjustGenericConstraints(methodToCopyGenericsFrom, newGenericParameters, originalGenericArguments, originalGenericArguments[i].GetGenericParameterConstraints());

					newGenericParameters[i].SetInterfaceConstraints(constraints);
					CopyNonInheritableAttributes(newGenericParameters[i], originalGenericArguments[i]);
				}
				catch (NotSupportedException)
				{
					// Doesn't matter

					newGenericParameters[i].SetGenericParameterAttributes(GenericParameterAttributes.None);
				}

				name2GenericType[argumentNames[i]] = newGenericParameters[i];
			}

			return newGenericParameters;
		}

		private static void CopyNonInheritableAttributes(GenericTypeParameterBuilder newGenericParameter,
		                                                 Type originalGenericArgument)
		{
			foreach (var attribute in originalGenericArgument.GetNonInheritableAttributes())
			{
				newGenericParameter.SetCustomAttribute(attribute.Builder);
			}
		}

		private static string[] GetArgumentNames(Type[] originalGenericArguments)
		{
			var argumentNames = new string[originalGenericArguments.Length];

			for (var i = 0; i < argumentNames.Length; i++)
			{
				argumentNames[i] = originalGenericArguments[i].Name;
			}
			return argumentNames;
		}
	}
}
