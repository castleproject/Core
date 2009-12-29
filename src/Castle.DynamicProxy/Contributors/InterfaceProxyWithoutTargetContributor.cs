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

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;

	public class InterfaceProxyWithoutTargetContributor : CompositeTypeContributor
	{
		private readonly GetTargetExpressionDelegate getTargetExpression;

		public InterfaceProxyWithoutTargetContributor(INamingScope namingScope, GetTargetExpressionDelegate getTarget)
			: base(namingScope)
		{
			getTargetExpression = getTarget;
		}

		public override void CollectElementsToProxy(IProxyGenerationHook hook)
		{
			Debug.Assert(hook != null, "hook != null");
			foreach (var @interface in interfaces)
			{
				var item = new InterfaceMembersCollector(@interface);
				item.CollectMembersToProxy(hook);
				targets.Add(item);
			}
		}

		protected override MethodGenerator GetMethodGenerator(MethodToGenerate method, ClassEmitter @class, ProxyGenerationOptions options, CreateMethodDelegate createMethod)
		{
			if (!method.Proxyable)
			{
				return new MinimialisticMethodGenerator(method,
														createMethod,
														GeneratorUtil.ObtainInterfaceMethodAttributes);
			}

			var invocation = GetInvocationType(method, @class, options);
			return new MethodWithInvocationGenerator(method,
													 @class.GetField("__interceptors"),
													 invocation,
													 getTargetExpression,
													 createMethod,
													 GeneratorUtil.ObtainInterfaceMethodAttributes);
		}

		private Type GetInvocationType(MethodToGenerate method, ClassEmitter emitter, ProxyGenerationOptions options)
		{
			var scope = emitter.ModuleScope;
			var key = new CacheKey(method.Method, InterfaceInvocationTypeGenerator.BaseType, null, null);

			// no locking required as we're already within a lock
			var invocation = scope.GetFromCache(key);
			if (invocation != null)
			{
				return invocation;
			}

			invocation = new InterfaceInvocationTypeGenerator(method.Method.DeclaringType,
															  method,
															  method.Method,
															  false)
				.Generate(emitter, options, namingScope).BuildType();

			scope.RegisterInCache(key, invocation);

			return invocation;
		}
	}
}