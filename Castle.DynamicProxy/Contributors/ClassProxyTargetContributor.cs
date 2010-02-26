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

namespace Castle.DynamicProxy.Contributors
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	public class ClassProxyTargetContributor : CompositeTypeContributor
	{
		private readonly Type targetType;
		private readonly IList<MethodInfo> methodsToSkip;

		public ClassProxyTargetContributor(Type targetType, IList<MethodInfo> methodsToSkip, INamingScope namingScope):base(namingScope)
		{
			this.targetType = targetType;
			this.methodsToSkip = methodsToSkip;
		}

		protected override IEnumerable<MembersCollector> CollectElementsToProxyInternal(IProxyGenerationHook hook)
		{
			Debug.Assert(hook != null, "hook != null");

			var targetItem = new ClassMembersCollector(targetType) { Logger = Logger };
			targetItem.CollectMembersToProxy(hook);
			yield return targetItem;

			foreach (var @interface in interfaces)
			{
				var item = new InterfaceMembersOnClassCollector(@interface,
				                                                true,
				                                                targetType.GetInterfaceMap(@interface)) { Logger = Logger };
				item.CollectMembersToProxy(hook);
				yield return item;
			}
		}

		protected override MethodGenerator GetMethodGenerator(MetaMethod method, ClassEmitter @class, ProxyGenerationOptions options, CreateMethodDelegate createMethod)
		{
			if (methodsToSkip.Contains(method.Method)) return null;

			if (!method.Proxyable)
			{
				return new MinimialisticMethodGenerator(method,
				                                        createMethod);
			}

			var invocation = GetInvocationType(method, @class, options);

			return new MethodWithInvocationGenerator(method,
			                                         @class.GetField("__interceptors"),
			                                         invocation,
			                                         (c, m) => new TypeTokenExpression(targetType),
			                                         createMethod);
		}

		private Type GetInvocationType(MetaMethod method, ClassEmitter @class, ProxyGenerationOptions options)
		{
			// NOTE: No caching since invocation is tied to this specific proxy type via its invocation method
			return BuildInvocationType(method, @class, options);
		}

		private Type BuildInvocationType(MetaMethod method, ClassEmitter @class, ProxyGenerationOptions options)
		{
			var methodInfo = method.Method;
			if (!method.HasTarget)
			{
				return new ClassInvocationTypeGenerator(targetType,
				                                        method,
				                                        null)
					.Generate(@class, options, namingScope)
					.BuildType();
			}
			var callback = CreateCallbackMethod(@class, methodInfo, method.MethodOnTarget);
			return new ClassInvocationTypeGenerator(callback.DeclaringType,
			                                        method,
			                                        callback)
				.Generate(@class, options, namingScope)
				.BuildType();
		}

		private MethodBuilder CreateCallbackMethod(ClassEmitter emitter, MethodInfo methodInfo, MethodInfo methodOnTarget)
		{
			MethodInfo targetMethod = methodOnTarget ?? methodInfo;

			// MethodBuild creation

			MethodEmitter callBackMethod = emitter.CreateMethod(namingScope.GetUniqueName(methodInfo.Name + "_callback"));

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