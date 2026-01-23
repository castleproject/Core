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

namespace Castle.DynamicProxy.Generators
{
	using System.Reflection;

	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Internal;
	using Castle.DynamicProxy.Tokens;

	internal static class GeneratorUtil
	{
		public static void CopyOutAndRefParameters(Reference[] dereferencedArguments, LocalReference invocation,
		                                           MethodInfo method, MethodEmitter emitter)
		{
			var parameters = method.GetParameters();

			// Create it only if there are byref writable arguments.
			LocalReference arguments = null;

			for (var i = 0; i < parameters.Length; i++)
			{
				if (parameters[i].IsByRef && !parameters[i].IsReadOnly)
				{
					if (arguments == null)
					{
						arguments = StoreInvocationArgumentsInLocal(emitter, invocation);
					}

#if FEATURE_BYREFLIKE
					var dereferencedParameterType = parameters[i].ParameterType.GetElementType();
					if (dereferencedParameterType.IsByRefLikeSafe())
					{
						// The argument value in the invocation `Arguments` array is an `object`
						// and cannot be converted back to its original by-ref-like type.
						// We need to replace it with some other value.

						// For now, we just substitute the by-ref-like type's default value:
						if (parameters[i].IsOut)
						{
							emitter.CodeBuilder.AddStatement(new AssignStatement(dereferencedArguments[i], new DefaultValueExpression(dereferencedParameterType)));
						}
						else
						{
							// ... except when we're dealing with a `ref` parameter. Unlike with `out`,
							// where we would be expected to definitely assign to it, we are free to leave
							// the original incoming value untouched. For now, that's likely the better
							// interim solution than unconditionally resetting.
						}
					}
					else
#endif
					{
						emitter.CodeBuilder.AddStatement(AssignArgument(dereferencedArguments, i, arguments));
					}
				}
			}
		}

		private static ConvertExpression Argument(int i, LocalReference invocationArgs, Reference[] arguments)
		{
			return new ConvertExpression(arguments[i].Type, new ArrayElementReference(invocationArgs, i));
		}

		private static AssignStatement AssignArgument(Reference[] dereferencedArguments, int i,
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