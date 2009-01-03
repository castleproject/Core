// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy
{
	using System.Collections.Generic;
	using System.Collections;

	// Found an implementation here
	// http://bytes.com/groups/net-c/534886-thread-safe-dictionary
	// 
	// Only change was exposing the SyncRoot property.
	// 
	public class ThreadSafeDictionary<TKey, TValue> : IDictionary<TKey, TValue>
	{
		private readonly object syncRoot = new object();
		private Dictionary<TKey, TValue> d = new Dictionary<TKey, TValue>();

		#region IDictionary<TKey, TValue> Members

		public void Add(TKey key, TValue value)
		{
			lock (syncRoot)
			{
				d.Add(key, value);
			}
		}

		public bool ContainsKey(TKey key)
		{
			return d.ContainsKey(key);
		}

		public ICollection<TKey> Keys
		{
			get
			{
				lock (syncRoot)
				{
					return d.Keys;
				}
			}
		}

		public bool Remove(TKey key)
		{
			lock (syncRoot)
			{
				return d.Remove(key);
			}
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			lock (syncRoot)
			{
				return d.TryGetValue(key, out value);
			}
		}

		public ICollection<TValue> Values
		{
			get
			{
				lock (syncRoot)
				{
					return d.Values;
				}
			}
		}

		public TValue this[TKey key]
		{
			get
			{
				return d[key];
			}
			set
			{
				lock (syncRoot)
				{
					d[key] = value;
				}
			}
		}

		#endregion

		#region ICollection<KeyValuePair<TKey,TValue>Members

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			lock (syncRoot)
			{
				((ICollection<KeyValuePair<TKey, TValue>>)d).Add(item);
			}
		}

		public void Clear()
		{
			lock (syncRoot)
			{
				d.Clear();
			}
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return ((ICollection<KeyValuePair<TKey, TValue>>)d).Contains(item);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int
		arrayIndex)
		{
			lock (syncRoot)
			{
				((ICollection<KeyValuePair<TKey, TValue>>)d).CopyTo(array,
				arrayIndex);
			}
		}

		public int Count
		{
			get
			{
				return d.Count;
			}
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			lock (syncRoot)
			{
				return ((ICollection<KeyValuePair<TKey, TValue>>)d).Remove(item);
			}
		}

		#endregion

		#region IEnumerable<KeyValuePair<TKey,TValue>>Members

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return ((ICollection<KeyValuePair<TKey, TValue>>)d).GetEnumerator();
		}

		#endregion

		#region IEnumerable Members

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((System.Collections.IEnumerable)d).GetEnumerator();
		}

		#endregion

		public object SyncRoot
		{
			get { return syncRoot; }
		}
	}
}
