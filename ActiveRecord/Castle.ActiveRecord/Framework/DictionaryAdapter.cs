// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Framework
{
	using System;
	using System.Collections;

	/// <summary>
	/// Maps keys to position in the values array. 
	/// Basically key -> index
	/// </summary>
	public class DictionaryAdapter : IDictionary
	{
		private int _tablesize;
		private Entry[] _table;
		private object[] _values;

		public DictionaryAdapter(String[] names, object[] values)
		{
			_tablesize = names.Length;
			_table = new Entry[_tablesize];
			_values = values;

			for(int i=0; i<_tablesize; i++)
			{
				InternalAdd(names[i], i);
			}
		}

		private void InternalAdd(String key, int index)
		{
			int position = CalculateHash(key);
			System.Diagnostics.Debug.Assert(position >= 0);
			
			if (_table[position] == null)
			{
				_table[position] = new Entry(key, index);
			}
			else
			{
				_table[position].AddEntry(new Entry(key, index));
			}
		}

		private int CalculateHash(String key)
		{
			uint result = (uint)key.GetHashCode();
			return (int) Math.Abs(result % _tablesize);
		}

		public class Entry
		{
			int index;
			String key; 
			Entry nextEntry;

			public Entry(String key, int index)
			{
				this.key = key;
				this.index = index;
			}

			internal void AddEntry(Entry entry)
			{
				Entry walker = this;

				while(walker.nextEntry != null)
				{
					walker = walker.nextEntry;
				}

				walker.nextEntry = entry;
			}

			public int Find(string key)
			{
				Entry walker = this;

				while(walker != null && !walker.key.Equals(key))
				{
					walker = walker.nextEntry;
				}

				return walker != null ? walker.index : -1;
			}
		}

		public bool Contains(object key)
		{
			return GetValuesIndexByKey(key) != -1;
		}

		public void Add(object key, object value)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			throw new NotSupportedException();
		}

		public void Remove(object key)
		{
			throw new NotSupportedException();
		}

		public ICollection Keys
		{
			get { throw new NotSupportedException(); }
		}

		public ICollection Values
		{
			get { throw new NotSupportedException(); }
		}

		public bool IsReadOnly
		{
			get { return true; }
		}

		public bool IsFixedSize
		{
			get { return true; }
		}

		public object this[object key]
		{
			get 
			{
				int index = GetValuesIndexByKey(key);

				if (index >= 0)
				{
					return _values[index];
				}

				return null;
			}
			set
			{
				int index = GetValuesIndexByKey(key);

				if (index >= 0)
				{
					_values[index] = value;
				}
				else
				{
					throw new ArgumentException(
						"DictionaryAdapter: Could not find value related to key {" + key + "}. " + 
						"This column is probably not mapped on ActiveRecord");
				}	
			}
		}

		private int GetValuesIndexByKey(object key)
		{
			int pos = CalculateHash(key.ToString());
	
			if (_table[pos] == null) return -1;

			return _table[pos].Find(key.ToString());
		}

		public void CopyTo(Array array, int index)
		{
			throw new NotSupportedException();
		}

		public int Count
		{
			get { return _tablesize; }
		}

		public object SyncRoot
		{
			get { return this; }
		}

		public bool IsSynchronized
		{
			get { return false; }
		}

		public IEnumerator GetEnumerator()
		{
			throw new NotSupportedException();
		}
	}
}
