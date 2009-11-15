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
	using System.Reflection;
	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;

	public class InterfaceProxyTargetContributor: ITypeContributor
	{
		private readonly Type targetType;
		private readonly IDictionary<Type, InterfaceMapping> interfaces = new Dictionary<Type, InterfaceMapping>();
		private readonly ICollection<MembersCollector> targets = new List<MembersCollector>();
		private readonly bool canChangeTarget;
		private readonly INamingScope namingScope;

		public InterfaceProxyTargetContributor(Type targetType, bool canChangeTarget, INamingScope namingScope)
		{
			this.targetType = targetType;
			this.namingScope = namingScope;
			this.canChangeTarget = canChangeTarget;
		}

		public void AddInterfaceToProxy(Type @interface)
		{
			// TODO: this impl is identcal to ClassProxyTargetContributor
			Debug.Assert(@interface != null, "@interface == null", "Shouldn't be adding empty interfaces...");
			Debug.Assert(@interface.IsInterface, "@interface.IsInterface", "Should be adding interfaces only...");
			Debug.Assert(!interfaces.ContainsKey(@interface), "!interfaces.ContainsKey(@interface)", "Shouldn't be adding same interface twice...");
			Debug.Assert(@interface.IsAssignableFrom(targetType), "@interface.IsAssignableFrom(targetType)",
						 "Shouldn't be adding mapping to interface that target does not implement...");

			
			interfaces.Add(@interface, GetMapping(@interface));
			
		}

		protected virtual InterfaceMapping GetMapping(Type @interface)
		{
			return targetType.GetInterfaceMap(@interface);
		}

		public void CollectElementsToProxy(IProxyGenerationHook hook)
		{
			Debug.Assert(hook != null, "hook != null");

			foreach (var mapping in interfaces)
			{
				var item = new InterfaceMembersOnClassCollector(mapping.Key, this, false, mapping.Value);
				item.CollectMembersToProxy(hook);
				targets.Add(item);
			}
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

		private void ImplementEvent(ClassEmitter @class, EventToGenerate @event, ProxyGenerationOptions options)
		{
			@event.BuildEventEmitter(@class);
			var adder = @event.Adder;
			ImplementMethod(adder, @class, options, @event.Emitter.CreateAddMethod);
			var remover = @event.Remover;
			ImplementMethod(remover, @class, options, @event.Emitter.CreateRemoveMethod);

		}

		private void ImplementProperty(ClassEmitter @class, PropertyToGenerate property, ProxyGenerationOptions options)
		{
			property.BuildPropertyEmitter(@class);
			if (property.CanRead)
			{
				var method = property.Getter;
				ImplementMethod(method, @class, options,
								(name, atts) => property.Emitter.CreateGetMethod(name, atts));
			}

			if (property.CanWrite)
			{
				var method = property.Setter;
				ImplementMethod(method, @class, options,
								(name, atts) => property.Emitter.CreateSetMethod(name, atts));
			}
		}

		private void ImplementMethod(MethodToGenerate method, ClassEmitter @class, ProxyGenerationOptions options, CreateMethodDelegate createMethod)
		{
			MethodGenerator generator;
			if (method.Proxyable)
			{
				Type invocation = new InterfaceInvocationTypeGenerator(method.Method.DeclaringType,
				                                                       method,
				                                                       method.Method,
				                                                       canChangeTarget)
					.Generate(@class, options, namingScope).BuildType();

				var interceptors = @class.GetField("__interceptors");

				generator = new InterfaceMethodGenerator(method,
				                                         invocation,
				                                         interceptors,
				                                         createMethod,
				                                         (c, m) => c.GetField("__target").ToExpression());
			}
			else
			{
				generator = new ForwardingMethodGenerator(method,
				                                          createMethod,
				                                          (c, m) => c.GetField("__target"));
			}
			var proxyMethod = generator.Generate(@class, options, namingScope);
			foreach (var attribute in AttributeUtil.GetNonInheritableAttributes(method.Method))
			{
				proxyMethod.DefineCustomAttribute(attribute);
			}
		}
	}
}