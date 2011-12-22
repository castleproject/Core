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
	using Castle.Core;

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
			MergePropertyOverrides(meta);
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

		private void MergePropertyOverrides(DictionaryAdapterMeta meta)
		{
			Properties = new Dictionary<string, PropertyDescriptor>();

			var initializers = new HashSet<IDictionaryInitializer>(ReferenceEqualityComparer<IDictionaryInitializer>.Instance);

			foreach (var property in meta.Properties)
			{
				var propertyDescriptor = new PropertyDescriptor(property.Value, true);

				if (Descriptor != null)
					propertyDescriptor.AddBehaviors(Descriptor.Behaviors);

				foreach (var initializer in propertyDescriptor.Initializers)
					initializers.Add(initializer);

				Properties.Add(property.Key, propertyDescriptor);
			}

			Initializers = initializers.ToArray();
		}
	}
}
