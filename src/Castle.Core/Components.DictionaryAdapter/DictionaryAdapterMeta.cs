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

namespace Castle.Components.DictionaryAdapter
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Diagnostics;

	[DebuggerDisplay("Type: {Type.FullName,nq}")]
	public class DictionaryAdapterMeta
	{
		private IDictionary extendedProperties;
		private readonly Func<DictionaryAdapterInstance, IDictionaryAdapter> creator;

		public DictionaryAdapterMeta(Type type, Type implementation, object[] behaviors, IDictionaryMetaInitializer[] metaInitializers, 
			                         IDictionaryInitializer[] initializers, IDictionary<String, PropertyDescriptor> properties,
									 IDictionaryAdapterFactory factory, Func<DictionaryAdapterInstance, IDictionaryAdapter> creator)
		{
			Type = type;
			Implementation = implementation;
			Behaviors = behaviors;
			MetaInitializers = metaInitializers;
			Initializers = initializers;
			Properties = properties;
			Factory = factory;
			this.creator = creator;

			InitializeMeta();
		}

		public Type Type { get; private set; }

		public Type Implementation { get; private set; }

		public object[] Behaviors { get; private set; }

		public IDictionaryAdapterFactory Factory { get; private set; }

		public IDictionary<string, PropertyDescriptor> Properties { get; private set; }

		public IDictionaryMetaInitializer[] MetaInitializers { get; private set; }

		public IDictionaryInitializer[] Initializers { get; private set; }

		public IDictionary ExtendedProperties
		{
			get
			{
				if (extendedProperties == null)
				{
					extendedProperties = new Dictionary<object, object>();
				}
				return extendedProperties;
			}
		}

		public PropertyDescriptor CreateDescriptor()
		{
			var metaInitializers   = MetaInitializers;
			var sharedAnnotations  = CollectSharedBehaviors(Behaviors,    metaInitializers);
			var sharedInitializers = CollectSharedBehaviors(Initializers, metaInitializers);

			var descriptor = (sharedAnnotations != null)
				? new PropertyDescriptor(sharedAnnotations.ToArray())
				: new PropertyDescriptor();

			descriptor.AddBehaviors(metaInitializers);

			if (sharedInitializers != null)
#if DOTNET40
				descriptor.AddBehaviors(sharedInitializers);
#else
				descriptor.AddBehaviors(sharedInitializers.Cast<IDictionaryBehavior>());
#endif

			return descriptor;
		}

		private static List<T> CollectSharedBehaviors<T>(T[] source, IDictionaryMetaInitializer[] predicates)
		{
			var results = null as List<T>;

			foreach (var candidate in source)
			{
				foreach (var predicate in predicates)
				{
					if (predicate.ShouldHaveBehavior(candidate))
					{
						if (results == null)
							results = new List<T>(source.Length);

						results.Add(candidate);
						break; // next candidate
					}
				}
			}

			return results;
		}

		public DictionaryAdapterMeta GetAdapterMeta(Type type)
		{
			return Factory.GetAdapterMeta(type, this);
		}

		public object CreateInstance(IDictionary dictionary, PropertyDescriptor descriptor)
		{
			var instance = new DictionaryAdapterInstance(dictionary, this, descriptor, Factory);
			return creator(instance);
		}

		private void InitializeMeta()
		{
			foreach (var metaInitializer in MetaInitializers)
			{
				metaInitializer.Initialize(Factory, this);
			}
		}
	}
}
