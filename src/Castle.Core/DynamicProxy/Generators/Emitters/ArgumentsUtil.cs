// Copyright 2004-2026 Castle Project - http://www.castleproject.org/
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
	using System.Linq;
	using System.Reflection;
	using System.Runtime.InteropServices;

	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	internal static class ArgumentsUtil
	{
		extension(ParameterInfo parameter)
		{
			public bool IsByRef
			{
				get
				{
					return parameter.ParameterType.IsByRef;
				}
			}

			public bool IsReadOnly
			{
				get
				{
					// C# `in` parameters are also by-ref, but meant to be read-only.
					// The section "Metadata representation of in parameters" on the following page
					// defines how such parameters are marked:
					//
					// https://github.com/dotnet/csharplang/blob/master/proposals/csharp-7.2/readonly-ref.md
					//
					// This poses three problems for detecting them:
					//
					//  * The C# Roslyn compiler marks `in` parameters with an `[in]` IL modifier,
					//    but this isn't specified, nor is it used uniquely for `in` params.
					//
					//  * `System.Runtime.CompilerServices.IsReadOnlyAttribute` is not defined on all
					//    .NET platforms, so the compiler sometimes recreates that type in the same
					//    assembly that contains the method having an `in` parameter. In other words,
					//    it's an attribute one must check for by name (which is slow, as it implies
					//    use of a `GetCustomAttributes` enumeration instead of a faster `IsDefined`).
					//
					//  * A required custom modifier `System.Runtime.InteropServices.InAttribute`
					//    is always present in those cases relevant for DynamicProxy (proxyable methods),
					//    but not all targeted platforms support reading custom modifiers. Also,
					//    support for cmods is generally flaky (at this time of writing, mid-2018).
					//
					// The above points inform the following detection logic:

					// First, fast-guard against non-`in` params by checking for an IL `[in]` modifier:
					if ((parameter.Attributes & (ParameterAttributes.In | ParameterAttributes.Out)) != ParameterAttributes.In)
					{
						return false;
					}

					// Second, check for the required modifiers (hoping for good modreq support):
					if (parameter.GetRequiredCustomModifiers().Any(x => x == typeof(InAttribute)))
					{
						return true;
					}

					// Third, check for `IsReadOnlyAttribute` by name (see explanation above).
					// This check is likely the slowest (despite being accurate) so we do it last:
					if (parameter.GetCustomAttributes(false).Any(x => x.GetType().FullName == "System.Runtime.CompilerServices.IsReadOnlyAttribute"))
					{
						return true;
					}

					return false;
				}
			}
		}

		extension(ParameterInfo[] parameters)
		{
			public ArgumentReference[] ConvertToArgumentReferences()
			{
				if (parameters.Length == 0) return [];

				var arguments = new ArgumentReference[parameters.Length];

				for (var i = 0; i < parameters.Length; ++i)
				{
					arguments[i] = new ArgumentReference(parameters[i].ParameterType, i + 1);
				}

				return arguments;
			}

			public Type[] GetTypes()
			{
				if (parameters.Length == 0) return [];

				var types = new Type[parameters.Length];
				for (var i = 0; i < parameters.Length; i++)
				{
					types[i] = parameters[i].ParameterType;
				}
				return types;
			}
		}

		extension(Type[] parameterTypes)
		{
			public ArgumentReference[] ConvertToArgumentReferences()
			{
				if (parameterTypes.Length == 0) return [];

				var arguments = new ArgumentReference[parameterTypes.Length];

				for (var i = 0; i < parameterTypes.Length; ++i)
				{
					arguments[i] = new ArgumentReference(parameterTypes[i], i + 1);
				}

				return arguments;
			}
		}

		public static Type[] InitializeAndConvert(ArgumentReference[] arguments)
		{
			if (arguments.Length == 0) return [];

			var types = new Type[arguments.Length];

			for (var i = 0; i < arguments.Length; ++i)
			{
				arguments[i].Position = i + 1;
				types[i] = arguments[i].Type;
			}

			return types;
		}
	}
}