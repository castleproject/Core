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

namespace Castle.Core.Internal
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	internal class WeakKeyDictionary<TKey, TValue> : IDictionary<TKey, TValue>
		where TKey : class
	{
		private readonly Dictionary<object, TValue> dictionary;
		private readonly WeakKeyComparer<TKey> comparer;
		private KeyCollection keys;

		public WeakKeyDictionary()
			: this(0, EqualityComparer<TKey>.Default) { }

		public WeakKeyDictionary(int capacity)
			: this(capacity, EqualityComparer<TKey>.Default) { }

		public WeakKeyDictionary(IEqualityComparer<TKey> comparer)
			: this(0, comparer) { }

		public WeakKeyDictionary(int capacity, IEqualityComparer<TKey> comparer)
		{
			this.comparer   = new WeakKeyComparer<TKey>(comparer);
			this.dictionary = new Dictionary<object, TValue>(capacity, this.comparer);
		}

		public int Count
		{
			get { return dictionary.Count; }
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
		{
			get { return false; }
		}

		public ICollection<TKey> Keys
		{
			get { return keys ?? (keys = new KeyCollection(dictionary.Keys)); }
		}

		public ICollection<TValue> Values
		{
			get { return dictionary.Values; }
		}

		public TValue this[TKey key]
		{
			get { return dictionary[key]; }
			set { dictionary[comparer.Wrap(key)] = value; }
		}

		public bool ContainsKey(TKey key)
		{
			return dictionary.ContainsKey(key);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
		{
			TValue candidate;
			return dictionary.TryGetValue(item.Key, out candidate)
				&& EqualityComparer<TValue>.Default.Equals(candidate, item.Value);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return dictionary.TryGetValue(key, out value);
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			foreach (var wrapped in dictionary)
			{
			    var item = new KeyValuePair<TKey, TValue>(
					comparer.Unwrap(wrapped.Key),
					wrapped.Value);
			    if (item.Key != null)
			        yield return item;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int index)
		{
			foreach (var item in this)
				array[index++] = item;
		}

		public void Add(TKey key, TValue value)
		{
			dictionary.Add(comparer.Wrap(key), value);
		}

		void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
		{
			Add(item.Key, item.Value);
		}

		public bool Remove(TKey key)
		{
			return dictionary.Remove(key);
		}

		bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
		{
			ICollection<KeyValuePair<TKey, TValue>> collection = this;
			return collection.Contains(item)
				&& dictionary.Remove(item.Key);
		}

		public void Clear()
		{
			dictionary.Clear();
		}

		public void TrimDeadObjects()
		{
			var removals = null as List<object>;

			foreach (var key in dictionary.Keys)
			{
				if (comparer.Unwrap(key) == null)
				{
					if (removals == null)
						removals = new List<object>();
					removals.Add(key);
				}
			}

			if (removals != null)
				foreach (var key in removals)
					dictionary.Remove(key);
		}

		private class KeyCollection : ICollection<TKey>
		{
			private readonly ICollection<object> keys;

			public KeyCollection(ICollection<object> keys)
			{
				this.keys = keys;
			}

			public int Count
			{
				get { return keys.Count; }
			}

			bool ICollection<TKey>.IsReadOnly
			{
				get { return true; }
			}

			public bool Contains(TKey item)
			{
				return keys.Contains(item);
			}

			public IEnumerator<TKey> GetEnumerator()
			{
				foreach (var key in keys)
				{
					var target = (TKey) ((WeakKey) key).Target;
					if (target != null)
						yield return target;
				}
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public void CopyTo(TKey[] array, int index)
			{
				foreach (var key in this)
					array[index++] = key;
			}

			void ICollection<TKey>.Add(TKey item)
			{
				throw ReadOnlyCollectionError();
			}

			bool ICollection<TKey>.Remove(TKey item)
			{
				throw ReadOnlyCollectionError();
			}

			void ICollection<TKey>.Clear()
			{
				throw ReadOnlyCollectionError();
			}

			private static Exception ReadOnlyCollectionError()
			{
				return new NotSupportedException("The collection is read-only.");
			}
		}
	}
}
