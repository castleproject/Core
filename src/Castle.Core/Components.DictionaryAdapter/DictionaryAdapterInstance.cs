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

	public class DictionaryAdapterInstance
	{
		private IDictionary extendedProperties;
		private List<IDictionaryCopyStrategy> copyStrategies;

		public DictionaryAdapterInstance(IDictionary dictionary, DictionaryAdapterMeta meta,
										 PropertyDescriptor descriptor, IDictionaryAdapterFactory factory)
		{
			Dictionary = dictionary;
			Descriptor = descriptor;
			Factory = factory;

			Properties = meta.Properties;
			Initializers = meta.Initializers;
			MergeBehaviorOverrides(meta);
		}

		internal int? OldHashCode { get; set; }

		public IDictionary Dictionary { get; private set; }

		public PropertyDescriptor Descriptor { get; private set; }

		public IDictionaryAdapterFactory Factory { get; private set; }

		public IDictionaryInitializer[] Initializers { get; private set; }

		public IDictionary<string, PropertyDescriptor> Properties { get; private set; }

		public IDictionaryEqualityHashCodeStrategy EqualityHashCodeStrategy { get; set; }

		public IDictionaryCreateStrategy CreateStrategy { get; set; }

		public IDictionaryCoerceStrategy CoerceStrategy { get; set; }

		public IEnumerable<IDictionaryCopyStrategy> CopyStrategies
		{
			get
			{
				return copyStrategies ?? Enumerable.Empty<IDictionaryCopyStrategy>();
			}
		}

		public void AddCopyStrategy(IDictionaryCopyStrategy copyStrategy)
		{
			if (copyStrategy == null)
				throw new ArgumentNullException("copyStrategy");

			if (copyStrategies == null)
				copyStrategies = new List<IDictionaryCopyStrategy>();

			copyStrategies.Add(copyStrategy);
		}

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

		private void MergeBehaviorOverrides(DictionaryAdapterMeta meta)
		{
			if (Descriptor == null) return;

			var typeDescriptor = Descriptor as DictionaryDescriptor;

			if (typeDescriptor != null)
			{
				Initializers = Initializers.Prioritize(typeDescriptor.Initializers).ToArray();
			}

			Properties = new Dictionary<string, PropertyDescriptor>();

			foreach (var property in meta.Properties)
			{
				var propertyDescriptor = property.Value;

				var propertyOverride = new PropertyDescriptor(propertyDescriptor, false)
					.AddKeyBuilders(propertyDescriptor.KeyBuilders.Prioritize(Descriptor.KeyBuilders))
					.AddGetters(propertyDescriptor.Getters.Prioritize(Descriptor.Getters))
					.AddSetters(propertyDescriptor.Setters.Prioritize(Descriptor.Setters));

				Properties.Add(property.Key, propertyOverride);
			}
		}
	}
}
