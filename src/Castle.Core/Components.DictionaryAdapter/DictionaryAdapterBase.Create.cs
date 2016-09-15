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
	using System.Collections.Specialized;

	public abstract partial class DictionaryAdapterBase : IDictionaryCreate
	{
		public T Create<T>()
		{
			return Create<T>(new HybridDictionary());
		}

		public object Create(Type type)
		{
			return Create(type, new HybridDictionary());
		}

		public T Create<T>(IDictionary dictionary)
		{
			return (T)Create(typeof(T), dictionary ?? new HybridDictionary());
		}

		public object Create(Type type, IDictionary dictionary)
		{
			if (This.CreateStrategy != null)
			{
				var created = This.CreateStrategy.Create(this, type, dictionary);
				if (created != null) return created;
			}
			dictionary = dictionary ?? new HybridDictionary();
			return This.Factory.GetAdapter(type, dictionary, This.Descriptor);
		}

		public T Create<T>(Action<T> init)
		{
			return Create<T>(new HybridDictionary(), init);
		}

		public T Create<T>(IDictionary dictionary, Action<T> init)
		{
			var adapter = Create<T>(dictionary ?? new HybridDictionary());
			if (init != null) init(adapter);
			return adapter;
		}
	}
}
