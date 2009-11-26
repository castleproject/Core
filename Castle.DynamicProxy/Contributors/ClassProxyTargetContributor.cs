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

namespace Castle.DynamicProxy.Contributors
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;
	using Generators;
	using Generators.Emitters;
	using Generators.Emitters.SimpleAST;

	public class ClassProxyTargetContributor : CompositeTypeContributor
	{
		private readonly Type targetType;
		private readonly IList<MethodInfo> methodsToSkip;

		public ClassProxyTargetContributor(Type targetType, IList<MethodInfo> methodsToSkip, INamingScope namingScope):base(namingScope)
		{
			this.targetType = targetType;
			this.methodsToSkip = methodsToSkip;
		}



		public override void CollectElementsToProxy(IProxyGenerationHook hook)
		{
			Debug.Assert(hook != null, "hook != null");

			var targetItem = new ClassMembersCollector(targetType, this);
			targetItem.CollectMembersToProxy(hook);
			targets.Add(targetItem);

			foreach (var @interface in interfaces)
			{
				var item = new InterfaceMembersOnClassCollector(@interface,
				                                                this,
				                                                true,
				                                                targetType.GetInterfaceMap(@interface));
				item.CollectMembersToProxy(hook);
				targets.Add(item);
			}

		}

		protected override MethodGenerator GetMethodGenerator(MethodToGenerate method, ClassEmitter @class, ProxyGenerationOptions options, CreateMethodDelegate createMethod)
		{
			if (methodsToSkip.Contains(method.Method)) return null;

			if (!method.Proxyable)
			{
				return new MinimialisticMethodGenerator(method,
				                                        createMethod,
				                                        GeneratorUtil.ObtainClassMethodAttributes);
			}

			var methodInfo = method.Method;
			var callback = default(MethodInfo);
			var targetForInvocation = targetType;
			if (!method.MethodOnTarget.IsAbstract && !IsExplicitInterfaceImplementation(method.MethodOnTarget))
			{
				callback = CreateCallbackMethod(@class, methodInfo, method.MethodOnTarget);
				targetForInvocation = callback.DeclaringType;
			}
			var invocation = new ClassInvocationTypeGenerator(targetForInvocation,
			                                                  method,
			                                                  callback).Generate(@class, options, namingScope);

			return new MethodWithCallbackGenerator(targetType, method, invocation, @class.GetField("__interceptors"), createMethod);
		}

		private bool IsExplicitInterfaceImplementation(MethodInfo methodInfo)
		{
			return methodInfo.IsPrivate && methodInfo.IsFinal;
		}

		private MethodBuilder CreateCallbackMethod(ClassEmitter emitter, MethodInfo methodInfo, MethodInfo methodOnTarget)
		{
			MethodInfo targetMethod = methodOnTarget ?? methodInfo;

			// MethodBuild creation

			MethodAttributes attributes = MethodAttributes.Family;

			MethodEmitter callBackMethod = emitter.CreateMethod(namingScope.GetUniqueName(methodInfo.Name + "_callback"), attributes);

			callBackMethod.CopyParametersAndReturnTypeFrom(targetMethod, emitter);

			// Generic definition

			if (targetMethod.IsGenericMethod)
			{
				targetMethod = targetMethod.MakeGenericMethod(callBackMethod.GenericTypeParams);
			}

			// Parameters exp

			Expression[] exps = new Expression[callBackMethod.Arguments.Length];

			for (int i = 0; i < callBackMethod.Arguments.Length; i++)
			{
				exps[i] = callBackMethod.Arguments[i].ToExpression();
			}

			// invocation on base class

			callBackMethod.CodeBuilder.AddStatement(
				new ReturnStatement(new MethodInvocationExpression(SelfReference.Self, targetMethod, exps)));

			return callBackMethod.MethodBuilder;
		}
	}
}