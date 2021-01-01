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

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;

	internal class InterfaceProxyWithoutTargetContributor : CompositeTypeContributor
	{
		private readonly GetTargetExpressionDelegate getTargetExpression;
		protected bool canChangeTarget = false;

		public InterfaceProxyWithoutTargetContributor(INamingScope namingScope, GetTargetExpressionDelegate getTarget)
			: base(namingScope)
		{
			getTargetExpression = getTarget;
		}

		protected override IEnumerable<MembersCollector> CollectElementsToProxyInternal(IProxyGenerationHook hook)
		{
			Debug.Assert(hook != null, "hook != null");
			foreach (var @interface in interfaces)
			{
				var item = new InterfaceMembersCollector(@interface);
				item.CollectMembersToProxy(hook);
				yield return item;
			}
		}

		protected override MethodGenerator GetMethodGenerator(MetaMethod method, ClassEmitter @class,
		                                                      OverrideMethodDelegate overrideMethod)
		{
			if (!method.Proxyable)
			{
				return new MinimialisticMethodGenerator(method, overrideMethod);
			}

			var invocation = GetInvocationType(method, @class);
			return new MethodWithInvocationGenerator(method,
			                                         @class.GetField("__interceptors"),
			                                         invocation,
			                                         getTargetExpression,
			                                         overrideMethod,
			                                         null);
		}

		private Type GetInvocationType(MetaMethod method, ClassEmitter emitter)
		{
			var scope = emitter.ModuleScope;
			Type[] invocationInterfaces;
			if (canChangeTarget)
			{
				invocationInterfaces = new[] { typeof(IInvocation), typeof(IChangeProxyTarget) };
			}
			else
			{
				invocationInterfaces = new[] { typeof(IInvocation) };
			}
			var key = new CacheKey(method.Method, CompositionInvocationTypeGenerator.BaseType, invocationInterfaces, null);

			// no locking required as we're already within a lock

			return scope.TypeCache.GetOrAddWithoutTakingLock(key, _ =>
				new CompositionInvocationTypeGenerator(method.Method.DeclaringType,
				                                       method,
				                                       method.Method,
				                                       canChangeTarget,
				                                       null)
				.Generate(emitter, namingScope)
				.BuildType());
		}
	}
}