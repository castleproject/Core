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
	using System.Collections.Generic;
	using System.Diagnostics;

	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;

	public class InterfaceProxyWithoutTargetContributor : ITypeContributor
	{
		private readonly IList<MembersCollector> targets = new List<MembersCollector>();
		private readonly IList<Type> interfaces = new List<Type>();
		private readonly INamingScope namingScope;
		private readonly GetTargetExpressionDelegate getTargetExpression;

		public InterfaceProxyWithoutTargetContributor(INamingScope namingScope, GetTargetExpressionDelegate getTarget)
		{
			this.namingScope = namingScope;
			getTargetExpression = getTarget;
		}

		public void CollectElementsToProxy(IProxyGenerationHook hook)
		{
			Debug.Assert(hook != null, "hook != null");
			foreach (var @interface in interfaces)
			{
				var item = new InterfaceMembersCollector(@interface);
				item.CollectMembersToProxy(hook);
				targets.Add(item);
			}
		}


		public void AddInterfaceMapping(Type @interface)
		{
			// TODO: this method is likely to be moved to the interface
			Debug.Assert(@interface != null, "@interface == null", "Shouldn't be adding empty interfaces...");
			Debug.Assert(@interface.IsInterface, "@interface.IsInterface", "Should be adding interfaces only...");
			Debug.Assert(!interfaces.Contains(@interface), "!interfaces.Contains(@interface)", "Shouldn't be adding same interface twice...");
			interfaces.Add(@interface);
		}

		public void Generate(ClassEmitter @class, ProxyGenerationOptions options)
		{
			foreach (var target in targets)
			{
				foreach (var method in target.Methods)
				{
					if (!method.Standalone)
					{
						continue;
					}

					ImplementMethod(method,
									@class,
									options,
									@class.CreateMethod);
				}

				foreach (var property in target.Properties)
				{
					ImplementProperty(@class, property, options);
				}

				foreach (var @event in target.Events)
				{
					ImplementEvent(@class, @event, options);
				}
			}
		}

		private void ImplementEvent(ClassEmitter emitter, EventToGenerate @event, ProxyGenerationOptions options)
		{
			@event.BuildEventEmitter(emitter);
			var method = @event.Adder;
			ImplementMethod(method, emitter, options, @event.Emitter.CreateAddMethod);
			var method1 = @event.Remover;
			ImplementMethod(method1, emitter, options, @event.Emitter.CreateRemoveMethod);

		}

		private void ImplementProperty(ClassEmitter emitter, PropertyToGenerate property, ProxyGenerationOptions options)
		{
			property.BuildPropertyEmitter(emitter);
			if (property.CanRead)
			{
				var method = property.Getter;
				ImplementMethod(method, emitter, options,
								(name, atts) => property.Emitter.CreateGetMethod(name, atts));
			}

			if (property.CanWrite)
			{
				var method = property.Setter;
				ImplementMethod(method, emitter, options,
								(name, atts) => property.Emitter.CreateSetMethod(name, atts));
			}
		}

		private void ImplementMethod(MethodToGenerate method, ClassEmitter @class, ProxyGenerationOptions options, CreateMethodDelegate createMethod)
		{
			MethodGenerator generator;
			if (method.Proxyable)
			{
				var invocation = GetInvocationType(method, @class, options);

				generator = new MethodWithInvocationGenerator(method,
				                                              @class.GetField("__interceptors"),
				                                              invocation,
				                                              getTargetExpression,
				                                              createMethod,
				                                              GeneratorUtil.ObtainInterfaceMethodAttributes);
			}
			else
			{
				generator = new MinimialisticMethodGenerator(method,
				                                             createMethod,
				                                             GeneratorUtil.ObtainInterfaceMethodAttributes);
			}

			var proxiedMethod = generator.Generate(@class, options, namingScope);
			foreach (var attribute in AttributeUtil.GetNonInheritableAttributes(method.Method))
			{
				proxiedMethod.DefineCustomAttribute(attribute);
			}
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