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
	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Generators.Emitters.SimpleAST;

	public class MixinContributor : MixinContributorBase
	{
		private const bool CanChangeTarget = false;
		private readonly InterfaceMapping mapping;
		private readonly INamingScope namingScope;
		private MembersCollector target;

		public MixinContributor(Type targetType,Type @interface, INamingScope namingScope)
		{
			if (targetType == null) throw new ArgumentNullException("targetType");
			if (@interface == null) throw new ArgumentNullException("interface");

			Debug.Assert(@interface.IsInterface, "@interface.IsInterface", "Should be adding mapping only...");
			Debug.Assert(@interface.IsAssignableFrom(targetType), "@interface.IsAssignableFrom(targetType)",
						 "Shouldn't be adding mapping to interface that target does not implement...");

			this.namingScope = namingScope;

			this.mapping = targetType.GetInterfaceMap(@interface);

			mixinInterface = @interface;
		}

		public override void CollectElementsToProxy(IProxyGenerationHook hook)
		{
			Debug.Assert(hook != null, "hook != null");
			// TODO: once tokens for method on target are obtained dynamically
			// this should be changed to InterfaceMembersCollector
			var item = new InterfaceMembersOnClassCollector(mixinInterface, this, false, mapping);
			item.CollectMembersToProxy(hook);
			target = item;

		}

		public override void Generate(ClassEmitter @class, ProxyGenerationOptions options)
		{
			field = BuildTargetField(@class, mixinInterface);

			foreach (var method in target.Methods)
			{
				if (!method.Standalone)
				{
					continue;
				}

				ImplementMethod(method,
				                @class,
				                options,
				                @class.CreateMethod,
				                field);
			}

			foreach (var property in target.Properties)
			{
				ImplementProperty(@class, property, options, field);
			}

			foreach (var @event in target.Events)
			{
				ImplementEvent(@class, @event, options, field);
			}

		}


		private void ImplementEvent(ClassEmitter emitter, EventToGenerate @event, ProxyGenerationOptions options, FieldReference target)
		{
			@event.BuildEventEmitter(emitter);
			var adder = @event.Adder;
			ImplementMethod(adder, emitter, options, @event.Emitter.CreateAddMethod, target);
			var remover = @event.Remover;
			ImplementMethod(remover, emitter, options, @event.Emitter.CreateRemoveMethod, target);

		}

		private void ImplementProperty(ClassEmitter emitter, PropertyToGenerate property, ProxyGenerationOptions options, FieldReference target)
		{
			property.BuildPropertyEmitter(emitter);
			if (property.CanRead)
			{
				var getter = property.Getter;
				ImplementMethod(getter, emitter, options,
								(name, atts) => property.Emitter.CreateGetMethod(name, atts), target);
			}

			if (property.CanWrite)
			{
				var setter = property.Setter;
				ImplementMethod(setter, emitter, options,
								(name, atts) => property.Emitter.CreateSetMethod(name, atts), target);
			}
		}

		private void ImplementMethod(MethodToGenerate method, ClassEmitter emitter, ProxyGenerationOptions options, CreateMethodDelegate createMethod, Reference target)
		{
			MethodGenerator generator;
			if (method.Proxyable)
			{
				var invocation = new InvocationTypeGenerator(method.Method.DeclaringType,
				                                             method,
				                                             method.Method,
				                                             CanChangeTarget)
					.Generate(emitter, options, namingScope);

				var interceptors = emitter.GetField("__interceptors");

				generator = new InterfaceMethodGenerator(method,
				                                         invocation,
				                                         interceptors,
				                                         createMethod,
				                                         (c, i) => target.ToExpression());
			}
			else
			{
				generator = new ForwardingMethodGenerator(method,
				                                          createMethod,
				                                          (c, i) => target);
			}
			var proxyMethod = generator.Generate(emitter, options, namingScope);
			foreach (var attribute in AttributeUtil.GetNonInheritableAttributes(method.Method))
			{
				proxyMethod.DefineCustomAttribute(attribute);
			}
		}
	}
}