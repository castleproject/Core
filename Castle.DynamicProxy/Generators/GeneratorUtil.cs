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

namespace Castle.DynamicProxy.Generators
{
	using System.Reflection;

	using Castle.Core;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Tokens;

	public static class GeneratorUtil
	{
		private static bool IsInterfaceMethodForExplicitImplementation(MethodToGenerate methodToGenerate)
		{
			return methodToGenerate.Method.DeclaringType.IsInterface &&
				   methodToGenerate.MethodOnTarget.IsFinal;
		}

		public static Pair<MethodAttributes, string> ObtainClassMethodAttributes(MethodToGenerate methodToGenerate)
		{
			var methodInfo = methodToGenerate.Method;
			MethodAttributes attributes = MethodAttributes.Virtual;
			string name;
			if (IsInterfaceMethodForExplicitImplementation(methodToGenerate))
			{
				name = methodInfo.DeclaringType.Name + "." + methodInfo.Name;
				attributes |= MethodAttributes.Public |
							  MethodAttributes.HideBySig |
							  MethodAttributes.NewSlot |
							  MethodAttributes.Final;
			}
			else
			{
				if (methodInfo.IsFinal)
				{
					attributes |= MethodAttributes.NewSlot;
				}

				if (methodInfo.IsPublic)
				{
					attributes |= MethodAttributes.Public;
				}

				if (methodInfo.IsHideBySig)
				{
					attributes |= MethodAttributes.HideBySig;
				}
				if (InternalsHelper.IsInternal(methodInfo) && InternalsHelper.IsInternalToDynamicProxy(methodInfo.DeclaringType.Assembly))
				{
					attributes |= MethodAttributes.Assembly;
				}
				if (methodInfo.IsFamilyAndAssembly)
				{
					attributes |= MethodAttributes.FamANDAssem;
				}
				else if (methodInfo.IsFamilyOrAssembly)
				{
					attributes |= MethodAttributes.FamORAssem;
				}
				else if (methodInfo.IsFamily)
				{
					attributes |= MethodAttributes.Family;
				}
				name = methodInfo.Name;
			}

			if (methodToGenerate.Standalone == false)
			{
				attributes |= MethodAttributes.SpecialName;
			}
			return new Pair<MethodAttributes, string>(attributes, name);
		}

		public static Pair<MethodAttributes, string> ObtainInterfaceMethodAttributes(MethodToGenerate methodToGenerate)
		{
			var methodInfo = methodToGenerate.Method;
			var name = methodInfo.DeclaringType.Name + "." + methodInfo.Name;
			var attributes = MethodAttributes.Virtual |
							 MethodAttributes.Public |
							 MethodAttributes.HideBySig |
							 MethodAttributes.NewSlot |
							 MethodAttributes.Final;

			if (methodToGenerate.Standalone == false)
			{
				attributes |= MethodAttributes.SpecialName;
			}
			return new Pair<MethodAttributes, string>(attributes, name);
		}

		public static void CopyOutAndRefParameters(TypeReference[] dereferencedArguments, LocalReference invocation, MethodInfo method, MethodEmitter emitter)
		{
			var parameters = method.GetParameters();
			if (!ArgumentsUtil.IsAnyByRef(parameters))
			{
				return; //saving the need to create locals if there is no need
			}

			var arguments = StoreInvocationArgumentsInLocal(emitter, invocation);

			for (int i = 0; i < parameters.Length; i++)
			{
				if (!parameters[i].ParameterType.IsByRef) continue;

				emitter.CodeBuilder.AddStatement(AssignArgument(dereferencedArguments, i, arguments));
			}
		}

		private static AssignStatement AssignArgument(TypeReference[] dereferencedArguments, int i, LocalReference invocationArgs)
		{
			return new AssignStatement(dereferencedArguments[i], Argument(i, invocationArgs, dereferencedArguments));
		}

		private static ConvertExpression Argument(int i, LocalReference invocationArgs, TypeReference[] arguments)
		{
			return new ConvertExpression(arguments[i].Type, new LoadRefArrayElementExpression(i, invocationArgs));
		}

		private static LocalReference StoreInvocationArgumentsInLocal(MethodEmitter emitter, LocalReference invocation)
		{
			var invocationArgs = emitter.CodeBuilder.DeclareLocal(typeof(object[]));
			emitter.CodeBuilder.AddStatement(GetArguments(invocationArgs, invocation));
			return invocationArgs;
		}

		private static AssignStatement GetArguments(LocalReference invocationArgs, LocalReference invocation)
		{
			return new AssignStatement(invocationArgs, new MethodInvocationExpression(invocation, InvocationMethods.GetArguments));
		}
	}
}