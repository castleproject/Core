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

namespace Castle.DynamicProxy.Contributors
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Reflection;

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Internal;

	internal class InterfaceProxyWithoutTargetContributor : CompositeTypeContributor
	{
		protected readonly GetTargetExpressionDelegate getTarget;
		protected bool canChangeTarget = false;

		public InterfaceProxyWithoutTargetContributor(INamingScope namingScope, GetTargetExpressionDelegate getTarget)
			: base(namingScope)
		{
			this.getTarget = getTarget;
		}

		protected override IEnumerable<MembersCollector> GetCollectors()
		{
			foreach (var @interface in interfaces)
			{
				var item = new InterfaceMembersCollector(@interface);
				yield return item;
			}
		}

		protected override MethodGenerator GetMethodGenerator(MetaMethod method, ClassEmitter @class,
		                                                      OverrideMethodDelegate overrideMethod)
		{
			if (!method.Proxyable)
			{
				return new MinimalisticMethodGenerator(method, overrideMethod);
			}

			var invocation = GetInvocationType(method, @class);
			return new MethodWithInvocationGenerator(method,
			                                         @class.GetField("__interceptors"),
			                                         invocation,
			                                         getTarget,
			                                         overrideMethod,
			                                         null);
		}

		private Type GetInvocationType(MetaMethod method, ClassEmitter emitter)
		{
			var methodInfo = method.Method;

			if (canChangeTarget == false)
			{
				if (!method.HasTarget)
				{
					// We do not need to generate a custom invocation type because no custom implementation
					// for `InvokeMethodOnTarget` will be needed (proceeding to target isn't possible here):
					return typeof(InterfaceMethodWithoutTargetInvocation);
				}
				else
				{
					// We end up here for interface methods with a default implementation:
					Debug.Assert(methodInfo.DeclaringType.IsInterface && methodInfo.IsAbstract == false);

					// This allows proceeding to a interface method's default implementation.
					// The code has been copied over from `ClassProxyTargetContributor`.
					var callback = CreateCallbackMethod(emitter, methodInfo, method.MethodOnTarget);
					return new InheritanceInvocationTypeGenerator(callback.DeclaringType, method, callback, null)
					       .Generate(emitter, namingScope)
					       .BuildType();
				}
			}

			Debug.Assert(canChangeTarget);

			var scope = emitter.ModuleScope;
			Type[] invocationInterfaces = new[] { typeof(IInvocation), typeof(IChangeProxyTarget) };
			var key = new CacheKey(methodInfo, CompositionInvocationTypeGenerator.BaseType, invocationInterfaces, null);

			// no locking required as we're already within a lock

			return scope.TypeCache.GetOrAddWithoutTakingLock(key, _ =>
				new CompositionInvocationTypeGenerator(methodInfo.DeclaringType,
				                                       method,
				                                       methodInfo,
				                                       canChangeTarget,
				                                       null)
				.Generate(emitter, namingScope)
				.BuildType());
		}

		private MethodInfo CreateCallbackMethod(ClassEmitter emitter, MethodInfo methodInfo, MethodInfo methodOnTarget)
		{
			var targetMethod = methodOnTarget ?? methodInfo;
			var callBackMethod = emitter.CreateMethod(namingScope.GetUniqueName(methodInfo.Name + "_callback"), targetMethod);

			if (targetMethod.IsGenericMethod)
			{
				targetMethod = targetMethod.MakeGenericMethod(callBackMethod.GenericTypeParams.AsTypeArray());
			}

			// invocation on base interface

			callBackMethod.CodeBuilder.AddStatement(
				new ReturnStatement(
					new MethodInvocationExpression(ThisExpression.Instance, targetMethod, callBackMethod.Arguments)));

			return callBackMethod.MethodBuilder;
		}
	}
}