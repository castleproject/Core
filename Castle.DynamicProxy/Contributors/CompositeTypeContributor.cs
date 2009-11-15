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

	public abstract class CompositeTypeContributor: ITypeContributor
	{
		protected readonly INamingScope namingScope;
		protected readonly IList<Type> interfaces = new List<Type>();
		protected readonly ICollection<MembersCollector> targets = new List<MembersCollector>();

		protected CompositeTypeContributor(INamingScope namingScope)
		{
			this.namingScope = namingScope;
		}

		public abstract void CollectElementsToProxy(IProxyGenerationHook hook);

		public virtual void Generate(ClassEmitter @class, ProxyGenerationOptions options)
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

		public void AddInterfaceToProxy(Type @interface)
		{
			Debug.Assert(@interface != null, "@interface == null", "Shouldn't be adding empty interfaces...");
			Debug.Assert(@interface.IsInterface, "@interface.IsInterface", "Should be adding interfaces only...");
			Debug.Assert(!interfaces.Contains(@interface), "!interfaces.ContainsKey(@interface)", "Shouldn't be adding same interface twice...");


			interfaces.Add(@interface);
		}

		private void ImplementEvent(ClassEmitter emitter, EventToGenerate @event, ProxyGenerationOptions options)
		{
			@event.BuildEventEmitter(emitter);
			ImplementMethod(@event.Adder, emitter, options, @event.Emitter.CreateAddMethod);
			ImplementMethod(@event.Remover, emitter, options, @event.Emitter.CreateRemoveMethod);

		}

		private void ImplementProperty(ClassEmitter emitter, PropertyToGenerate property, ProxyGenerationOptions options)
		{
			property.BuildPropertyEmitter(emitter);
			if (property.CanRead)
			{
				ImplementMethod(property.Getter, emitter, options,
				                (name, atts) => property.Emitter.CreateGetMethod(name, atts));
			}

			if (property.CanWrite)
			{
				ImplementMethod(property.Setter, emitter, options,
				                (name, atts) => property.Emitter.CreateSetMethod(name, atts));

			}
		}

		protected abstract MethodGenerator GetMethodGenerator(MethodToGenerate method, ClassEmitter @class,
		                                           ProxyGenerationOptions options, CreateMethodDelegate createMethod);

		private void ImplementMethod(MethodToGenerate method, ClassEmitter @class, ProxyGenerationOptions options,
		                                        CreateMethodDelegate createMethod)
		{
			{
				var generator = GetMethodGenerator(method, @class, options, createMethod);
				if (generator == null) return;
				var proxyMethod = generator.Generate(@class, options, namingScope);
				foreach (var attribute in AttributeUtil.GetNonInheritableAttributes(method.Method))
				{
					proxyMethod.DefineCustomAttribute(attribute);
				}
			}
		}
	}
}