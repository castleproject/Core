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
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Tokens;

	public class ClassProxyTargetContributor : CompositeTypeContributor
	{
		private readonly IList<MethodInfo> methodsToSkip;
		private readonly Type targetType;

		public ClassProxyTargetContributor(Type targetType, IList<MethodInfo> methodsToSkip, INamingScope namingScope)
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

			if (ExplicitlyImplementedInterfaceMethod(method))
			{
#if SILVERLIGHT
				return null;
#else
				return ExplicitlyImplementedInterfaceMethodGenerator(method, @class, options, overrideMethod);
#endif
			}

			var invocation = GetInvocationType(method, @class, options);

			return new MethodWithInvocationGenerator(method,
			                                         @class.GetField("__interceptors"),
			                                         invocation,
			                                         (c, m) => new TypeTokenExpression(targetType),
			                                         overrideMethod,
			                                         null);
		}

		private Type BuildInvocationType(MetaMethod method, ClassEmitter @class, ProxyGenerationOptions options)
		{
			var methodInfo = method.Method;
			if (!method.HasTarget)
			{
				return new InheritanceInvocationTypeGenerator(targetType,
				                                              method,
				                                              null, null)
					.Generate(@class, options, namingScope)
					.BuildType();
			}
			var callback = CreateCallbackMethod(@class, methodInfo, method.MethodOnTarget);
			return new InheritanceInvocationTypeGenerator(callback.DeclaringType,
			                                              method,
			                                              callback, null)
				.Generate(@class, options, namingScope)
				.BuildType();
		}

		private MethodBuilder CreateCallbackMethod(ClassEmitter emitter, MethodInfo methodInfo, MethodInfo methodOnTarget)
		{
			var targetMethod = methodOnTarget ?? methodInfo;
			var callBackMethod = emitter.CreateMethod(namingScope.GetUniqueName(methodInfo.Name + "_callback"), targetMethod);

			if (targetMethod.IsGenericMethod)
			{
				targetMethod = targetMethod.MakeGenericMethod(callBackMethod.GenericTypeParams);
			}

			var exps = new Expression[callBackMethod.Arguments.Length];
			for (var i = 0; i < callBackMethod.Arguments.Length; i++)
			{
				exps[i] = callBackMethod.Arguments[i].ToExpression();
			}

			// invocation on base class

			callBackMethod.CodeBuilder.AddStatement(
				new ReturnStatement(
					new MethodInvocationExpression(SelfReference.Self,
					                               targetMethod,
					                               exps)));

			return callBackMethod.MethodBuilder;
		}

		private bool ExplicitlyImplementedInterfaceMethod(MetaMethod method)
		{
			return method.MethodOnTarget.IsPrivate;
		}

		private MethodGenerator ExplicitlyImplementedInterfaceMethodGenerator(MetaMethod method, ClassEmitter @class,
		                                                                      ProxyGenerationOptions options,
		                                                                      OverrideMethodDelegate overrideMethod)
		{
			var @delegate = GetDelegateType(method, @class, options);
			var contributor = GetContributor(@delegate, method);
			var invocation = new InheritanceInvocationTypeGenerator(targetType, method, null, contributor)
				.Generate(@class, options, namingScope)
				.BuildType();
			return new MethodWithInvocationGenerator(method,
			                                         @class.GetField("__interceptors"),
			                                         invocation,
			                                         (c, m) => new TypeTokenExpression(targetType),
			                                         overrideMethod,
			                                         contributor);
		}

		private IInvocationCreationContributor GetContributor(Type @delegate, MetaMethod method)
		{
			if (@delegate.IsGenericType == false)
			{
				return new InvocationWithDelegateContributor(@delegate, targetType, method, namingScope);
			}
			return new InvocationWithGenericDelegateContributor(@delegate,
			                                                    method,
			                                                    new FieldReference(InvocationMethods.ProxyObject));
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
			// NOTE: No caching since invocation is tied to this specific proxy type via its invocation method
			return BuildInvocationType(method, @class, options);
		}
	}
}