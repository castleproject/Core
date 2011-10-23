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

namespace Castle.DynamicProxy.Contributors
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;

	public class DelegateProxyTargetContributor : CompositeTypeContributor
	{
		private readonly Type targetType;

		public DelegateProxyTargetContributor(Type targetType, INamingScope namingScope) : base(namingScope)
		{
			this.targetType = targetType;
		}

		protected override IEnumerable<MembersCollector> CollectElementsToProxyInternal(IProxyGenerationHook hook)
		{
			Debug.Assert(hook != null, "hook != null");
			var targetItem = new DelegateMembersCollector(targetType) { Logger = Logger };
			targetItem.CollectMembersToProxy(hook);
			yield return targetItem;
		}

		protected override MethodGenerator GetMethodGenerator(MetaMethod method, ClassEmitter @class,
		                                                      ProxyGenerationOptions options,
		                                                      OverrideMethodDelegate overrideMethod)
		{
			var invocation = GetInvocationType(method, @class, options);
			return new MethodWithInvocationGenerator(method,
			                                         @class.GetField("__interceptors"),
			                                         invocation,
			                                         (c, m) => c.GetField("__target").ToExpression(),
			                                         overrideMethod,
			                                         null);
		}

		private Type GetInvocationType(MetaMethod method, ClassEmitter emitter, ProxyGenerationOptions options)
		{
			var scope = emitter.ModuleScope;
			var key = new CacheKey(method.Method, CompositionInvocationTypeGenerator.BaseType, null, null);

			// no locking required as we're already within a lock
			var invocation = scope.GetFromCache(key);
			if (invocation != null)
			{
				return invocation;
			}

			invocation = new CompositionInvocationTypeGenerator(method.Method.DeclaringType,
			                                                    method,
			                                                    method.Method,
			                                                    false,
			                                                    null)
				.Generate(emitter, options, namingScope)
				.BuildType();

			scope.RegisterInCache(key, invocation);

			return invocation;
		}
	}
}