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
	using System.Linq;
	using System.Reflection;
	using System.Reflection.Emit;

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
	using Castle.DynamicProxy.Internal;
	using Castle.DynamicProxy.Tokens;

	internal class ClassProxyTargetContributor : CompositeTypeContributor
	{
		private readonly Type targetType;

		public ClassProxyTargetContributor(Type targetType, INamingScope namingScope)
			: base(namingScope)
		{
			this.targetType = targetType;
		}

		protected override IEnumerable<MembersCollector> GetCollectors()
		{
			var targetItem = new ClassMembersCollector(targetType) { Logger = Logger };
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

			if (!method.Proxyable)
			{
				return new MinimalisticMethodGenerator(method, overrideMethod);
			}

			if (ExplicitlyImplementedInterfaceMethod(method))
			{
				return ExplicitlyImplementedInterfaceMethodGenerator(method, @class, overrideMethod);
			}

			var invocation = GetInvocationType(method, @class);

			GetTargetExpressionDelegate getTargetTypeExpression = (c, m) => new TypeTokenExpression(targetType);

			return new MethodWithInvocationGenerator(method,
			                                         @class.GetField("__interceptors"),
			                                         invocation,
			                                         getTargetTypeExpression,
			                                         getTargetTypeExpression,
			                                         overrideMethod,
			                                         null);
		}

		private Type BuildInvocationType(MetaMethod method, ClassEmitter @class)
		{
			var methodInfo = method.Method;
			if (!method.HasTarget)
			{
				return new InheritanceInvocationTypeGenerator(targetType,
				                                              method,
				                                              null, null)
					.Generate(@class, namingScope)
					.BuildType();
			}
			var callback = CreateCallbackMethod(@class, methodInfo, method.MethodOnTarget);
			return new InheritanceInvocationTypeGenerator(callback.DeclaringType,
			                                              method,
			                                              callback, null)
				.Generate(@class, namingScope)
				.BuildType();
		}

		private MethodBuilder CreateCallbackMethod(ClassEmitter emitter, MethodInfo methodInfo, MethodInfo methodOnTarget)
		{
			var targetMethod = methodOnTarget ?? methodInfo;
			var callBackMethod = emitter.CreateMethod(namingScope.GetUniqueName(methodInfo.Name + "_callback"), targetMethod);

			if (targetMethod.IsGenericMethod)
			{
				targetMethod = targetMethod.MakeGenericMethod(callBackMethod.GenericTypeParams.AsTypeArray());
			}

			// invocation on base class

			callBackMethod.CodeBuilder.AddStatement(
				new ReturnStatement(
					new MethodInvocationExpression(SelfReference.Self,
					                               targetMethod,
					                               callBackMethod.Arguments)));

			return callBackMethod.MethodBuilder;
		}

		private bool ExplicitlyImplementedInterfaceMethod(MetaMethod method)
		{
			return method.MethodOnTarget.IsPrivate;
		}

		private MethodGenerator ExplicitlyImplementedInterfaceMethodGenerator(MetaMethod method, ClassEmitter @class,
		                                                                      OverrideMethodDelegate overrideMethod)
		{
			var @delegate = GetDelegateType(method, @class);
			var contributor = GetContributor(@delegate, method);
			var invocation = new InheritanceInvocationTypeGenerator(targetType, method, null, contributor)
				.Generate(@class, namingScope)
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

		private Type GetDelegateType(MetaMethod method, ClassEmitter @class)
		{
			var scope = @class.ModuleScope;
			var key = new CacheKey(
				typeof(Delegate),
				targetType,
				new[] { method.MethodOnTarget.ReturnType }
					.Concat(ArgumentsUtil.GetTypes(method.MethodOnTarget.GetParameters())).
					ToArray(),
				null);

			return scope.TypeCache.GetOrAddWithoutTakingLock(key, _ =>
				new DelegateTypeGenerator(method, targetType)
				.Generate(@class, namingScope)
				.BuildType());
		}

		private Type GetInvocationType(MetaMethod method, ClassEmitter @class)
		{
			// NOTE: No caching since invocation is tied to this specific proxy type via its invocation method
			return BuildInvocationType(method, @class);
		}
	}
}