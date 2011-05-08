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
			if (!ArgumentsUtil.IsAnyByRef(parameters))
			{
				return; //saving the need to create locals if there is no need
			}

			var arguments = StoreInvocationArgumentsInLocal(emitter, invocation);

			for (var i = 0; i < parameters.Length; i++)
			{
				if (!parameters[i].ParameterType.IsByRef)
				{
					continue;
				}

				emitter.CodeBuilder.AddStatement(AssignArgument(dereferencedArguments, i, arguments));
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