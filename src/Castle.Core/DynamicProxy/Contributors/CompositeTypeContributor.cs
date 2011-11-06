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

	using Castle.Core.Logging;
	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Internal;

	public abstract class CompositeTypeContributor : ITypeContributor
	{
		protected readonly INamingScope namingScope;

		protected readonly ICollection<Type> interfaces = new HashSet<Type>();
		
		private ILogger logger = NullLogger.Instance;
		private readonly ICollection<MetaProperty> properties = new TypeElementCollection<MetaProperty>();
		private readonly ICollection<MetaEvent> events = new TypeElementCollection<MetaEvent>();
		private readonly ICollection<MetaMethod> methods = new TypeElementCollection<MetaMethod>();

		protected CompositeTypeContributor(INamingScope namingScope)
		{
			this.namingScope = namingScope;
		}

		public ILogger Logger
		{
			get { return logger; }
			set { logger = value; }
		}

		public void CollectElementsToProxy(IProxyGenerationHook hook, MetaType model)
		{
			foreach (var collector in CollectElementsToProxyInternal(hook))
			{
				foreach (var method in collector.Methods)
				{
					model.AddMethod(method);
					methods.Add(method);
				}
				foreach (var @event in collector.Events)
				{
					model.AddEvent(@event);
					events.Add(@event);
				}
				foreach (var property in collector.Properties)
				{
					model.AddProperty(property);
					properties.Add(property);
				}
			}
		}

		protected abstract IEnumerable<MembersCollector> CollectElementsToProxyInternal(IProxyGenerationHook hook);

		public virtual void Generate(ClassEmitter @class, ProxyGenerationOptions options)
		{
			foreach (var method in methods)
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

			foreach (var property in properties)
			{
				ImplementProperty(@class, property, options);
			}

			foreach (var @event in events)
			{
				ImplementEvent(@class, @event, options);
			}
		}

		public void AddInterfaceToProxy(Type @interface)
		{
			Debug.Assert(@interface != null, "@interface == null", "Shouldn't be adding empty interfaces...");
			Debug.Assert(@interface.IsInterface, "@interface.IsInterface", "Should be adding interfaces only...");
			Debug.Assert(!interfaces.Contains(@interface), "!interfaces.ContainsKey(@interface)",
			             "Shouldn't be adding same interface twice...");

			interfaces.Add(@interface);
		}

		private void ImplementEvent(ClassEmitter emitter, MetaEvent @event, ProxyGenerationOptions options)
		{
			@event.BuildEventEmitter(emitter);
			ImplementMethod(@event.Adder, emitter, options, @event.Emitter.CreateAddMethod);
			ImplementMethod(@event.Remover, emitter, options, @event.Emitter.CreateRemoveMethod);
		}

		private void ImplementProperty(ClassEmitter emitter, MetaProperty property, ProxyGenerationOptions options)
		{
			property.BuildPropertyEmitter(emitter);
			if (property.CanRead)
			{
				ImplementMethod(property.Getter, emitter, options, property.Emitter.CreateGetMethod);
			}

			if (property.CanWrite)
			{
				ImplementMethod(property.Setter, emitter, options, property.Emitter.CreateSetMethod);
			}
		}

		protected abstract MethodGenerator GetMethodGenerator(MetaMethod method, ClassEmitter @class,
		                                                      ProxyGenerationOptions options,
		                                                      OverrideMethodDelegate overrideMethod);

		private void ImplementMethod(MetaMethod method, ClassEmitter @class, ProxyGenerationOptions options,
		                             OverrideMethodDelegate overrideMethod)
		{
			{
				var generator = GetMethodGenerator(method, @class, options, overrideMethod);
				if (generator == null)
				{
					return;
				}
				var proxyMethod = generator.Generate(@class, options, namingScope);
				foreach (var attribute in method.Method.GetNonInheritableAttributes())
				{
					proxyMethod.DefineCustomAttribute(attribute);
				}
			}
		}
	}
}