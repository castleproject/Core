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
	using System.Reflection;

	using Castle.Core.Logging;
	using Castle.DynamicProxy.Generators;
	using Castle.DynamicProxy.Generators.Emitters;
	using Castle.DynamicProxy.Internal;

	internal abstract class CompositeTypeContributor : ITypeContributor
	{
		protected readonly INamingScope namingScope;

		protected readonly ICollection<Type> interfaces = new HashSet<Type>();
		
		private ILogger logger = NullLogger.Instance;
		private readonly List<MetaProperty> properties = new List<MetaProperty>();
		private readonly List<MetaEvent> events = new List<MetaEvent>();
		private readonly List<MetaMethod> methods = new List<MetaMethod>();

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
			Debug.Assert(hook != null);
			Debug.Assert(model != null);

			var sink = new MembersCollectorSink(model, this);

			foreach (var collector in GetCollectors())
			{
				collector.CollectMembersToProxy(hook, sink);
			}
		}

		protected abstract IEnumerable<MembersCollector> GetCollectors();

		public virtual void Generate(ClassEmitter @class)
		{
			foreach (var method in methods)
			{
				if (!method.Standalone)
				{
					continue;
				}

				ImplementMethod(method,
				                @class,
				                @class.CreateMethod);
			}

			foreach (var property in properties)
			{
				ImplementProperty(@class, property);
			}

			foreach (var @event in events)
			{
				ImplementEvent(@class, @event);
			}
		}

		public void AddInterfaceToProxy(Type @interface)
		{
			Debug.Assert(@interface != null, "@interface == null", "Shouldn't be adding empty interfaces...");
			Debug.Assert(@interface.IsInterface || @interface.IsDelegateType(), "@interface.IsInterface || @interface.IsDelegateType()", "Should be adding interfaces or delegate types only...");
			Debug.Assert(!interfaces.Contains(@interface), "!interfaces.ContainsKey(@interface)",
			             "Shouldn't be adding same interface twice...");

			interfaces.Add(@interface);
		}

		private void ImplementEvent(ClassEmitter emitter, MetaEvent @event)
		{
			@event.BuildEventEmitter(emitter);
			ImplementMethod(@event.Adder, emitter, @event.Emitter.CreateAddMethod);
			ImplementMethod(@event.Remover, emitter, @event.Emitter.CreateRemoveMethod);
		}

		private void ImplementProperty(ClassEmitter emitter, MetaProperty property)
		{
			property.BuildPropertyEmitter(emitter);
			if (property.CanRead)
			{
				ImplementMethod(property.Getter, emitter, property.Emitter.CreateGetMethod);
			}

			if (property.CanWrite)
			{
				ImplementMethod(property.Setter, emitter, property.Emitter.CreateSetMethod);
			}
		}

		protected abstract MethodGenerator GetMethodGenerator(MetaMethod method, ClassEmitter @class,
		                                                      OverrideMethodDelegate overrideMethod);

		private void ImplementMethod(MetaMethod method, ClassEmitter @class,
		                             OverrideMethodDelegate overrideMethod)
		{
			{
				var generator = GetMethodGenerator(method, @class, overrideMethod);
				if (generator == null)
				{
					return;
				}
				var proxyMethod = generator.Generate(@class, namingScope);
				foreach (var attribute in method.Method.GetNonInheritableAttributes())
				{
					proxyMethod.DefineCustomAttribute(attribute.Builder);
				}
			}
		}

		private sealed class MembersCollectorSink : IMembersCollectorSink
		{
			private readonly MetaType model;
			private readonly CompositeTypeContributor contributor;

			public MembersCollectorSink(MetaType model, CompositeTypeContributor contributor)
			{
				this.model = model;
				this.contributor = contributor;
			}

			// You may have noticed that most contributors do not query `MetaType` at all,
			// but only their own collections. So perhaps you are wondering why collected
			// type elements are added to `model` at all, and not just to `contributor`?
			//
			// TL;DR: This prevents member name collisions in the generated proxy type.
			//
			// `MetaType` uses `TypeElementCollection`s internally, which switches members
			// to explicit implementation whenever a name collision with a previously added
			// member occurs.
			//
			// It would be pointless to do this at the level of the individual contributor,
			// because name collisions could still occur across several contributors. This
			// is why they all share the same `MetaType` instance.

			public void Add(MetaEvent @event)
			{
				model.AddEvent(@event);
				contributor.events.Add(@event);
			}

			public void Add(MetaMethod method)
			{
				model.AddMethod(method);
				contributor.methods.Add(method);
			}

			public void Add(MetaProperty property)
			{
				model.AddProperty(property);
				contributor.properties.Add(property);
			}
		}
	}
}