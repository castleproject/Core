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
	using System.Collections;
	using System.Collections.Specialized;

	public class DictionaryAdapterInstance
	{
		private HybridDictionary extendedProperties;

		public DictionaryAdapterInstance(IDictionary dictionary, PropertyDescriptor descriptor, 
										 IDictionaryAdapterFactory factory)
		{
			Dictionary = dictionary;
			Descriptor = descriptor;
			Factory = factory;
		}

		internal int? OldHashCode { get; set; }

		public IDictionary Dictionary { get; private set; }

		public PropertyDescriptor Descriptor { get; private set; }

		public IDictionaryAdapterFactory Factory { get; private set; }

		public IDictionaryEqualityHashCodeStrategy EqualityHashCodeStrategy { get; set; }

		public IDictionaryCreateStrategy CreateStrategy { get; set; }

		public IDictionary ExtendedProperties
		{
			get
			{
				if (extendedProperties == null)
					extendedProperties = new HybridDictionary();
				return extendedProperties;
			}
		}
	}
}
