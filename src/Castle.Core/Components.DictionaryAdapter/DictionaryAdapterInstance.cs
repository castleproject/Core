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
			Factory    = factory;

			List<IDictionaryBehavior> behaviors;

			if (null == descriptor || null == (behaviors = descriptor.BehaviorsInternal))
			{
				Initializers = meta.Initializers;
				Properties   = MergeProperties(meta.Properties);
			}
			else
			{
				Initializers = MergeInitializers(meta.Initializers, behaviors);
				Properties   = MergeProperties(meta.Properties, behaviors);
			}
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

		private static IDictionaryInitializer[] MergeInitializers(
			IDictionaryInitializer[] source, List<IDictionaryBehavior> behaviors)
		{
			int index, count;
			IDictionaryInitializer initializer;
			var result = null as List<IDictionaryInitializer>;

			count = source.Length;
			for (index = 0; index < count; index++)
				PropertyDescriptor.MergeBehavior(ref result, source[index]);

			count = behaviors.Count;
			for (index = 0; index < count; index++)
				if (null != (initializer = behaviors[index] as IDictionaryInitializer))
					PropertyDescriptor.MergeBehavior(ref result, initializer);

			return result == null
				? NoInitializers
				: result.ToArray();
		}

		private static IDictionary<string, PropertyDescriptor> MergeProperties(
			IDictionary<string, PropertyDescriptor> source)
		{
			var properties = new Dictionary<string, PropertyDescriptor>();

			foreach (var sourceProperty in source)
			{
				properties[sourceProperty.Key] = new PropertyDescriptor(sourceProperty.Value, true);
			}

			return properties;
		}

		private static IDictionary<string, PropertyDescriptor> MergeProperties(
			IDictionary<string, PropertyDescriptor> source, List<IDictionaryBehavior> behaviors)
		{
			int index, count = behaviors.Count;
			var properties = new Dictionary<string, PropertyDescriptor>();

			foreach (var sourceProperty in source)
			{
				var property = new PropertyDescriptor(sourceProperty.Value, true);

				for (index = 0; index < count; index++)
					property.AddBehavior(behaviors[index]);

				properties[sourceProperty.Key] = property;
			}

			return properties;
		}

		private static readonly IDictionaryInitializer[]
			NoInitializers = { };
	}
}
