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
	using System.Linq;
	using System.Reflection;

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Tokens;

	public class ClassProxyWithTargetTargetContributor : CompositeTypeContributor
	{
		private readonly IList<MethodInfo> methodsToSkip;
		private readonly Type targetType;

		public ClassProxyWithTargetTargetContributor(Type targetType, IList<MethodInfo> methodsToSkip,
		                                             INamingScope namingScope)
			: base(namingScope)
		{
			this.targetType = targetType;
			this.methodsToSkip = methodsToSkip;
		}

		protected override IEnumerable<MembersCollector> CollectElementsToProxyInternal(IProxyGenerationHook hook)
		{
			Debug.Assert(hook != null, "hook != null");

			var targetItem = new WrappedClassMembersCollector(targetType) { Logger = Logger };
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

		protected override MethodGenerator GetMethodGenerator(MetaMethod method, ClassEmitter @class,
		                                                      ProxyGenerationOptions options,
		                                                      OverrideMethodDelegate overrideMethod)
		{
			if (methodsToSkip.Contains(method.Method))
			{
				return null;
			}

			if (!method.Proxyable)
			{
				return new MinimialisticMethodGenerator(method,
				                                        overrideMethod);
			}

			if (IsDirectlyAccessible(method) == false)
			{
				return IndirectlyCalledMethodGenerator(method, @class, options, overrideMethod);
			}

			var invocation = GetInvocationType(method, @class, options);

			return new MethodWithInvocationGenerator(method,
			                                         @class.GetField("__interceptors"),
			                                         invocation,
			                                         (c, m) => c.GetField("__target").ToExpression(),
			                                         overrideMethod,
			                                         null);
		}

		private Type BuildInvocationType(MetaMethod method, ClassEmitter @class, ProxyGenerationOptions options)
		{
			if (!method.HasTarget)
			{
				return new InheritanceInvocationTypeGenerator(targetType,
				                                              method,
				                                              null, null)
					.Generate(@class, options, namingScope)
					.BuildType();
			}
			return new CompositionInvocationTypeGenerator(method.Method.DeclaringType,
			                                              method,
			                                              method.Method,
			                                              false,
			                                              null)
				.Generate(@class, options, namingScope)
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
			                                                    new FieldReference(InvocationMethods.Target));
		}

		private Type GetDelegateType(MetaMethod method, ClassEmitter @class, ProxyGenerationOptions options)
		{
			var scope = @class.ModuleScope;
			var key = new CacheKey(
				typeof(Delegate),
				targetType,
				new[] { method.MethodOnTarget.ReturnType }
					.Concat(ArgumentsUtil.GetTypes(method.MethodOnTarget.GetParameters())).
					ToArray(),
				null);

			var type = scope.GetFromCache(key);
			if (type != null)
			{
				return type;
			}

			type = new DelegateTypeGenerator(method, targetType)
				.Generate(@class, options, namingScope)
				.BuildType();

			scope.RegisterInCache(key, type);

			return type;
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

		private MethodGenerator IndirectlyCalledMethodGenerator(MetaMethod method, ClassEmitter proxy,
		                                                        ProxyGenerationOptions options,
		                                                        OverrideMethodDelegate overrideMethod)
		{
			var @delegate = GetDelegateType(method, proxy, options);
			var contributor = GetContributor(@delegate, method);
			var invocation = new CompositionInvocationTypeGenerator(targetType, method, null, false, contributor)
				.Generate(proxy, options, namingScope)
				.BuildType();
			return new MethodWithInvocationGenerator(method,
			                                         proxy.GetField("__interceptors"),
			                                         invocation,
			                                         (c, m) => c.GetField("__target").ToExpression(),
			                                         overrideMethod,
			                                         contributor);
		}

		private bool IsDirectlyAccessible(MetaMethod method)
		{
			return method.MethodOnTarget.IsPublic;
		}
	}
}