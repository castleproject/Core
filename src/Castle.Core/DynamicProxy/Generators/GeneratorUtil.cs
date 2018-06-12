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

namespace Castle.DynamicProxy.Generators
{
	using System;
	using System.Linq;
	using System.Reflection;

	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Tokens;

	public static class GeneratorUtil
	{
		public static void CopyOutAndRefParameters(TypeReference[] dereferencedArguments, LocalReference invocation,
		                                           MethodInfo method, MethodEmitter emitter)
		{
			var parameters = method.GetParameters();
			if (!parameters.Any(IsByRef))
			{
				return; //saving the need to create locals if there is no need
			}

			var arguments = StoreInvocationArgumentsInLocal(emitter, invocation);

			for (var i = 0; i < parameters.Length; i++)
			{
				if (IsByRef(parameters[i]) && !IsReadOnly(parameters[i]))
				{
					emitter.CodeBuilder.AddStatement(AssignArgument(dereferencedArguments, i, arguments));
				}
			}

			bool IsByRef(ParameterInfo parameter)
			{
				return parameter.ParameterType.GetTypeInfo().IsByRef;
			}

			bool IsReadOnly(ParameterInfo parameter)
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
				// The above points inform the following detection logic: First, we rely on an IL
				// `[in]` modifier being present. This is a "fast guard" against non-`in` parameters:
				if ((parameter.Attributes & (ParameterAttributes.In | ParameterAttributes.Out)) == ParameterAttributes.In)
				{
					// Here we perform the actual check. We don't rely on cmods because support
					// for them is at current too unreliable in general, and because we wouldn't
					// be saving much time anyway.
					if (parameter.GetCustomAttributes(false).Any(IsIsReadOnlyAttribute))
					{
						return true;
					}
				}

				return false;

				bool IsIsReadOnlyAttribute(object attribute)
				{
					// The comparison by name is intentional; any assembly could define that attribute.
					// See explanation in comment above.
					return attribute.GetType().FullName == "System.Runtime.CompilerServices.IsReadOnlyAttribute";
				}
			}
		}

		private static ConvertExpression Argument(int i, LocalReference invocationArgs, TypeReference[] arguments)
		{
			return new ConvertExpression(arguments[i].Type, new LoadRefArrayElementExpression(i, invocationArgs));
		}

		private static AssignStatement AssignArgument(TypeReference[] dereferencedArguments, int i,
		                                              LocalReference invocationArgs)
		{
			return new AssignStatement(dereferencedArguments[i], Argument(i, invocationArgs, dereferencedArguments));
		}

		private static AssignStatement GetArguments(LocalReference invocationArgs, LocalReference invocation)
		{
			return new AssignStatement(invocationArgs, new MethodInvocationExpression(invocation, InvocationMethods.GetArguments));
		}

		private static LocalReference StoreInvocationArgumentsInLocal(MethodEmitter emitter, LocalReference invocation)
		{
			var invocationArgs = emitter.CodeBuilder.DeclareLocal(typeof(object[]));
			emitter.CodeBuilder.AddStatement(GetArguments(invocationArgs, invocation));
			return invocationArgs;
		}
	}
}