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
	using System.Diagnostics;
	using System.Reflection;

	using Castle.Core.Interceptor;
	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;

	public class InterfaceProxyTargetContributor : CompositeTypeContributor
	{
		private readonly Type proxyTargetType;
		private readonly bool canChangeTarget;

		public InterfaceProxyTargetContributor(Type proxyTargetType, bool canChangeTarget, INamingScope namingScope)
			: base(namingScope)
		{
			this.proxyTargetType = proxyTargetType;
			this.canChangeTarget = canChangeTarget;
		}

		public override void CollectElementsToProxy(IProxyGenerationHook hook)
		{
			Debug.Assert(hook != null, "hook != null");

			foreach (var @interface in interfaces)
			{
				var item = new InterfaceMembersOnClassCollector(@interface, this, false, GetMapping(@interface));
				item.CollectMembersToProxy(hook);
				targets.Add(item);
			}
		}

		protected virtual InterfaceMapping GetMapping(Type @interface)
		{
			return proxyTargetType.GetInterfaceMap(@interface);
		}

		protected override MethodGenerator GetMethodGenerator(MethodToGenerate method, ClassEmitter @class, ProxyGenerationOptions options, CreateMethodDelegate createMethod)
		{
			if (!method.Proxyable)
			{
				return new ForwardingMethodGenerator(method,
				                                     createMethod,
				                                     (c, m) => c.GetField("__target"));
			}

			Type invocation = GetInvocationType(method, @class, options);

			return new InterfaceMethodGenerator(method,
			                                    invocation,
			                                    @class.GetField("__interceptors"),
			                                    createMethod,
			                                    (c, m) => c.GetField("__target").ToExpression());
		}

		private Type GetInvocationType(MethodToGenerate method, ClassEmitter emitter, ProxyGenerationOptions options)
		{
			var scope = emitter.ModuleScope;

			Type[] invocationInterfaces = Type.EmptyTypes;
			if(canChangeTarget)
			{
				invocationInterfaces = new[] { typeof(IChangeProxyTarget) };
			}

			var key = new CacheKey(method.Method, invocationInterfaces, null);

			// no locking required as we're already within a lock

			var invocation = scope.GetFromCache(key);
			if (invocation != null)
			{
				return invocation;
			}

			invocation = new InterfaceInvocationTypeGenerator(method.Method.DeclaringType,
			                                                  method,
			                                                  method.Method,
			                                                  canChangeTarget)
				.Generate(emitter, options, namingScope).BuildType();

			scope.RegisterInCache(key, invocation);

			return invocation;
		}
	}
}