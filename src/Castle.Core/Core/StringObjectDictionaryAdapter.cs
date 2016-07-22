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

namespace Castle.Core
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	public sealed class StringObjectDictionaryAdapter : IDictionary<string, object>
	{
		private readonly IDictionary dictionary;

		public StringObjectDictionaryAdapter(IDictionary dictionary)
		{
			this.dictionary = dictionary;
		}

		bool IDictionary<string, object>.ContainsKey(string key)
		{
			return dictionary.Contains(key);
		}

		void IDictionary<string, object>.Add(string key, object value)
		{
			throw new NotImplementedException();
		}

		bool IDictionary<string, object>.Remove(string key)
		{
			throw new NotImplementedException();
		}

		bool IDictionary<string, object>.TryGetValue(string key, out object value)
		{
			value = null;
			if (dictionary.Contains(key))
			{
				value = dictionary[key];
				return true;
			}
			else
			{
				return false;
			}
		}

		object IDictionary<string, object>.this[string key]
		{
			get { return dictionary[key]; }
			set { throw new NotImplementedException(); }
		}

		ICollection<string> IDictionary<string, object>.Keys
		{
			get
			{
				string[] keys = new string[Count];
				dictionary.Keys.CopyTo(keys, 0);
				return keys;
			}
		}

		ICollection<object> IDictionary<string, object>.Values
		{
			get
			{
				object[] values = new object[Count];
				dictionary.Values.CopyTo(values, 0);
				return values;
			}
		}

		void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
		{
			throw new NotImplementedException();
		}

		bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
		{
			throw new NotImplementedException();
		}

		void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
		{
			throw new NotImplementedException();
		}

		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
		{
			return new EnumeratorAdapter(this);
		}

		public bool Contains(object key)
		{
			return dictionary.Contains(key);
		}

		public void Add(object key, object value)
		{
			dictionary.Add(key, value);
		}

		public void Clear()
		{
			dictionary.Clear();
		}

		public void Remove(object key)
		{
			dictionary.Remove(key);
		}

		public object this[object key]
		{
			get { return dictionary[key]; }
			set { dictionary[key] = value; }
		}

		public ICollection Keys
		{
			get { return dictionary.Keys; }
		}

		public ICollection Values
		{
			get { return dictionary.Values; }
		}

		public bool IsReadOnly
		{
			get { return dictionary.IsReadOnly; }
		}

		public bool IsFixedSize
		{
			get { return dictionary.IsFixedSize; }
		}

		public void CopyTo(Array array, int index)
		{
			dictionary.CopyTo(array, index);
		}

		public int Count
		{
			get { return dictionary.Count; }
		}

		public object SyncRoot
		{
			get { return dictionary.SyncRoot; }
		}

		public bool IsSynchronized
		{
			get { return dictionary.IsSynchronized; }
		}

		public IEnumerator GetEnumerator()
		{
			return ((IEnumerable) dictionary).GetEnumerator();
		}

		internal class EnumeratorAdapter : IEnumerator<KeyValuePair<string, object>>
		{
			private readonly StringObjectDictionaryAdapter adapter;
			private IEnumerator<string> keyEnumerator;
			private string currentKey;
			private object currentValue;

			public EnumeratorAdapter(StringObjectDictionaryAdapter adapter)
			{
				this.adapter = adapter;
				keyEnumerator = ((IDictionary<string, object>) adapter).Keys.GetEnumerator();
			}

			public bool MoveNext()
			{
				if (keyEnumerator.MoveNext())
				{
					currentKey = keyEnumerator.Current;
					currentValue = adapter[currentKey];
					return true;
				}

				return false;
			}

			public void Reset()
			{
				keyEnumerator.Reset();
			}

			public object Current
			{
				get { return new KeyValuePair<string, object>(currentKey, currentValue); }
			}

			KeyValuePair<string, object> IEnumerator<KeyValuePair<string, object>>.Current
			{
				get { return new KeyValuePair<string, object>(currentKey, currentValue); }
			}

			public void Dispose()
			{
				GC.SuppressFinalize(this);
			}
		}
	}
}
