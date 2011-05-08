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
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	public class MixinContributor : CompositeTypeContributor
	{
		private readonly bool canChangeTarget;
		private readonly IList<Type> empty = new List<Type>();
		private readonly IDictionary<Type, FieldReference> fields = new Dictionary<Type, FieldReference>();
		private readonly GetTargetExpressionDelegate getTargetExpression;

		public MixinContributor(INamingScope namingScope, bool canChangeTarget)
			: base(namingScope)
		{
			this.canChangeTarget = canChangeTarget;
			getTargetExpression = BuildGetTargetExpression();
		}

		public IEnumerable<FieldReference> Fields
		{
			get { return fields.Values; }
		}

		public void AddEmptyInterface(Type @interface)
		{
			Debug.Assert(@interface != null, "@interface == null", "Shouldn't be adding empty interfaces...");
			Debug.Assert(@interface.IsInterface, "@interface.IsInterface", "Should be adding interfaces only...");
			Debug.Assert(!interfaces.Contains(@interface), "!interfaces.Contains(@interface)",
			             "Shouldn't be adding same interface twice...");
			Debug.Assert(!empty.Contains(@interface), "!empty.Contains(@interface)",
			             "Shouldn't be adding same interface twice...");
			empty.Add(@interface);
		}

		public override void Generate(ClassEmitter @class, ProxyGenerationOptions options)
		{
			foreach (var @interface in interfaces)
			{
				fields[@interface] = BuildTargetField(@class, @interface);
			}

			foreach (var emptyInterface in empty)
			{
				fields[emptyInterface] = BuildTargetField(@class, emptyInterface);
			}

			base.Generate(@class, options);
		}

		protected override IEnumerable<MembersCollector> CollectElementsToProxyInternal(IProxyGenerationHook hook)
		{
			foreach (var @interface in interfaces)
			{
				var item = new InterfaceMembersCollector(@interface);
				item.CollectMembersToProxy(hook);
				yield return item;
			}
		}

		protected override MethodGenerator GetMethodGenerator(MetaMethod method, ClassEmitter @class,
		                                                      ProxyGenerationOptions options,
		                                                      OverrideMethodDelegate overrideMethod)
		{
			if (!method.Proxyable)
			{
				return new ForwardingMethodGenerator(method,
				                                     overrideMethod,
				                                     (c, i) => fields[i.DeclaringType]);
			}

			var invocation = GetInvocationType(method, @class, options);
			return new MethodWithInvocationGenerator(method,
			                                         @class.GetField("__interceptors"),
			                                         invocation,
			                                         getTargetExpression,
			                                         overrideMethod,
			                                         null);
		}

		private GetTargetExpressionDelegate BuildGetTargetExpression()
		{
			if (!canChangeTarget)
			{
				return (c, m) => fields[m.DeclaringType].ToExpression();
			}

			return (c, m) => new NullCoalescingOperatorExpression(
			                 	new AsTypeReference(c.GetField("__target"), m.DeclaringType).ToExpression(),
			                 	fields[m.DeclaringType].ToExpression());
		}

		private FieldReference BuildTargetField(ClassEmitter @class, Type type)
		{
			var name = "__mixin_" + type.FullName.Replace(".", "_");
			return @class.CreateField(namingScope.GetUniqueName(name), type);
		}

		private Type GetInvocationType(MetaMethod method, ClassEmitter emitter, ProxyGenerationOptions options)
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

			var invocation = scope.GetFromCache(key);
			if (invocation != null)
			{
				return invocation;
			}

			invocation = new CompositionInvocationTypeGenerator(method.Method.DeclaringType,
			                                                    method,
			                                                    method.Method,
			                                                    canChangeTarget,
			                                                    null)
				.Generate(emitter, options, namingScope)
				.BuildType();

			scope.RegisterInCache(key, invocation);

			return invocation;
		}
	}
}