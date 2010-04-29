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

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;

	public class ClassProxyWithTargetTargetContributor : CompositeTypeContributor
	{
		private readonly Type targetType;
		private readonly IList<MethodInfo> methodsToSkip;

		public ClassProxyWithTargetTargetContributor(Type targetType, IList<MethodInfo> methodsToSkip, INamingScope namingScope)
			: base(namingScope)
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

		protected override MethodGenerator GetMethodGenerator(MetaMethod method, ClassEmitter @class, ProxyGenerationOptions options, OverrideMethodDelegate overrideMethod)
		{
			if (methodsToSkip.Contains(method.Method)) return null;

			if (!method.Proxyable)
			{
				return new MinimialisticMethodGenerator(method,
				                                        overrideMethod);
			}

			var invocation = GetInvocationType(method, @class, options);

			return new MethodWithInvocationGenerator(method,
			                                         @class.GetField("__interceptors"),
			                                         invocation,
			                                         (c, m) => c.GetField("__target").ToExpression(),
			                                         overrideMethod);
		}

		private Type GetInvocationType(MetaMethod method, ClassEmitter @class, ProxyGenerationOptions options)
		{
			var scope = @class.ModuleScope;
			var invocationInterfaces = new[] { typeof(IInvocation) };

			var key = new CacheKey(method.Method, CompositionInvocationTypeGenerator.BaseType, invocationInterfaces, null);

			// no locking required as we're already within a lock

			var invocation = scope.GetFromCache(key);
			if (invocation != null)
			{
				return invocation;
			}
			invocation = BuildInvocationType(method, @class, options);

			scope.RegisterInCache(key, invocation);

			return invocation;
		}

		private Type BuildInvocationType(MetaMethod method, ClassEmitter @class, ProxyGenerationOptions options)
		{
			if (!method.HasTarget)
			{
				return new InheritanceInvocationTypeGenerator(targetType,
				                                        method,
				                                        null)
					.Generate(@class, options, namingScope)
					.BuildType();
			}
			return new CompositionInvocationTypeGenerator(method.Method.DeclaringType,
			                                            method,
			                                            method.Method,
			                                            false)
				.Generate(@class, options, namingScope)
				.BuildType();
		}
	}
}