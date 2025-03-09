﻿// Copyright 2004-2021 Castle Project - http://www.castleproject.org/
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

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Tokens;

	internal sealed class ClassProxyWithTargetTargetContributor : CompositeTypeContributor
	{
		private readonly Type targetType;

		public ClassProxyWithTargetTargetContributor(Type targetType, INamingScope namingScope)
			: base(namingScope)
		{
			this.targetType = targetType;
		}

		protected override IEnumerable<MembersCollector> GetCollectors()
		{
			var targetItem = new WrappedClassMembersCollector(targetType) { Logger = Logger };
			yield return targetItem;

			foreach (var @interface in interfaces)
			{
				var item = new InterfaceMembersOnClassCollector(@interface, true,
					targetType.GetInterfaceMap(@interface)) { Logger = Logger };
				yield return item;
			}
		}

		protected override MethodGenerator GetMethodGenerator(MetaMethod method, ClassEmitter @class,
		                                                      OverrideMethodDelegate overrideMethod)
		{
			if (method.Ignore)
			{
				return null;
			}

			var methodIsDirectlyAccessible = IsDirectlyAccessible(method);

			if (!method.Proxyable)
			{
				if (methodIsDirectlyAccessible)
				{
					return new ForwardingMethodGenerator(method, overrideMethod, (c, m) => c.GetField("__target"));
				}
				else
				{
					return IndirectlyCalledMethodGenerator(method, @class, overrideMethod, skipInterceptors: true);
				}
			}

			if (!methodIsDirectlyAccessible)
			{
				return IndirectlyCalledMethodGenerator(method, @class, overrideMethod);
			}

			var invocation = GetInvocationType(method, @class);

			return new MethodWithInvocationGenerator(method,
			                                         @class.GetField("__interceptors"),
			                                         invocation,
			                                         (c, m) => c.GetField("__target"),
			                                         overrideMethod,
			                                         null);
		}

		private Type BuildInvocationType(MetaMethod method, ClassEmitter @class)
		{
			if (!method.HasTarget)
			{
				return new InheritanceInvocationTypeGenerator(targetType,
				                                              method,
				                                              null, null)
					.Generate(@class, namingScope)
					.BuildType();
			}
			return new CompositionInvocationTypeGenerator(method.Method.DeclaringType,
			                                              method,
			                                              method.Method,
			                                              false,
			                                              null)
				.Generate(@class, namingScope)
				.BuildType();
		}

		private IInvocationCreationContributor GetContributor(Type @delegate, MetaMethod method)
		{
			if (@delegate.IsGenericType == false)
			{
				return new InvocationWithDelegateContributor(@delegate, targetType, method, namingScope);
			}
			return new InvocationWithGenericDelegateContributor(@delegate,
			                                                    method,
			                                                    new FieldReference(InvocationMethods.CompositionInvocationTarget));
		}

		private Type GetDelegateType(MetaMethod method, ClassEmitter @class)
		{
			var scope = @class.ModuleScope;
			var key = new CacheKey(
				typeof(Delegate),
				targetType,
				GetCacheKeyTypes(method),
				null);

			return scope.TypeCache.GetOrAddWithoutTakingLock(key, _ =>
				new DelegateTypeGenerator(method, targetType)
				.Generate(@class, namingScope)
				.BuildType());
		}

		private Type GetInvocationType(MetaMethod method, ClassEmitter @class)
		{
			var scope = @class.ModuleScope;
			var invocationInterfaces = new[] { typeof(IInvocation) };

			var key = new CacheKey(method.Method, CompositionInvocationTypeGenerator.BaseType, invocationInterfaces, null);

			// no locking required as we're already within a lock

			return scope.TypeCache.GetOrAddWithoutTakingLock(key, _ => BuildInvocationType(method, @class));
		}

		private MethodGenerator IndirectlyCalledMethodGenerator(MetaMethod method, ClassEmitter proxy,
		                                                        OverrideMethodDelegate overrideMethod,
		                                                        bool skipInterceptors = false)
		{
			var @delegate = GetDelegateType(method, proxy);
			var contributor = GetContributor(@delegate, method);
			var invocation = new CompositionInvocationTypeGenerator(targetType, method, null, false, contributor)
				.Generate(proxy, namingScope)
				.BuildType();
			return new MethodWithInvocationGenerator(method,
			                                         skipInterceptors ? NullExpression.Instance : proxy.GetField("__interceptors"),
			                                         invocation,
			                                         (c, m) => c.GetField("__target"),
			                                         overrideMethod,
			                                         contributor);
		}

		private static bool IsDirectlyAccessible(MetaMethod method)
		{
			return method.MethodOnTarget.IsPublic;
		}
	}
}