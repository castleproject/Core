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
		private readonly int _tablesize;
		private readonly Entry[] _table;
		private readonly object[] _values;
		private readonly string[] _keys;

		/// <summary>
		/// Initializes a new instance of the <see cref="DictionaryAdapter"/> class.
		/// </summary>
		/// <param name="names">The names.</param>
		/// <param name="values">The values.</param>
		public DictionaryAdapter(String[] names, object[] values)
		{
			_tablesize = names.Length;
			_table = new Entry[_tablesize];
			_values = values;
			_keys = names;

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

		/// <summary>
		/// Simple link list entry
		/// </summary>
		public class Entry
		{
			readonly int index;
			readonly String key; 
			Entry nextEntry;

			/// <summary>
			/// Initializes a new instance of the <see cref="Entry"/> class.
			/// </summary>
			/// <param name="key">The key.</param>
			/// <param name="index">The index.</param>
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

			/// <summary>
			/// Finds the specified key.
			/// </summary>
			/// <param name="key">The key.</param>
			/// <returns></returns>
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

		/// <summary>
		/// Determines whether the <see cref="T:System.Collections.IDictionary"></see> object contains an element with the specified key.
		/// </summary>
		/// <param name="key">The key to locate in the <see cref="T:System.Collections.IDictionary"></see> object.</param>
		/// <returns>
		/// true if the <see cref="T:System.Collections.IDictionary"></see> contains an element with the key; otherwise, false.
		/// </returns>
		/// <exception cref="T:System.ArgumentNullException">key is null. </exception>
		public bool Contains(object key)
		{
			return GetValuesIndexByKey(key) != -1;
		}

		/// <summary>
		/// Adds an element with the provided key and value to the <see cref="T:System.Collections.IDictionary"></see> object.
		/// </summary>
		/// <param name="key">The <see cref="T:System.Object"></see> to use as the key of the element to add.</param>
		/// <param name="value">The <see cref="T:System.Object"></see> to use as the value of the element to add.</param>
		/// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.IDictionary"></see> object. </exception>
		/// <exception cref="T:System.ArgumentNullException">key is null. </exception>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IDictionary"></see> is read-only.-or- The <see cref="T:System.Collections.IDictionary"></see> has a fixed size. </exception>
		public void Add(object key, object value)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Removes all elements from the <see cref="T:System.Collections.IDictionary"></see> object.
		/// </summary>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IDictionary"></see> object is read-only. </exception>
		public void Clear()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Returns an <see cref="T:System.Collections.IDictionaryEnumerator"></see> object for the <see cref="T:System.Collections.IDictionary"></see> object.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IDictionaryEnumerator"></see> object for the <see cref="T:System.Collections.IDictionary"></see> object.
		/// </returns>
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Removes the element with the specified key from the <see cref="T:System.Collections.IDictionary"></see> object.
		/// </summary>
		/// <param name="key">The key of the element to remove.</param>
		/// <exception cref="T:System.NotSupportedException">The <see cref="T:System.Collections.IDictionary"></see> object is read-only.-or- The <see cref="T:System.Collections.IDictionary"></see> has a fixed size. </exception>
		/// <exception cref="T:System.ArgumentNullException">key is null. </exception>
		public void Remove(object key)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets an <see cref="T:System.Collections.ICollection"></see> object containing the keys of the <see cref="T:System.Collections.IDictionary"></see> object.
		/// </summary>
		/// <value></value>
		/// <returns>An <see cref="T:System.Collections.ICollection"></see> object containing the keys of the <see cref="T:System.Collections.IDictionary"></see> object.</returns>
		public ICollection Keys
		{
			get			
			{
				return _keys;
			}
		}

		/// <summary>
		/// Gets an <see cref="T:System.Collections.ICollection"></see> object containing the values in the <see cref="T:System.Collections.IDictionary"></see> object.
		/// </summary>
		/// <value></value>
		/// <returns>An <see cref="T:System.Collections.ICollection"></see> object containing the values in the <see cref="T:System.Collections.IDictionary"></see> object.</returns>
		public ICollection Values
		{
			get { throw new NotSupportedException(); }
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"></see> object is read-only.
		/// </summary>
		/// <value></value>
		/// <returns>true if the <see cref="T:System.Collections.IDictionary"></see> object is read-only; otherwise, false.</returns>
		public bool IsReadOnly
		{
			get { return true; }
		}

		/// <summary>
		/// Gets a value indicating whether the <see cref="T:System.Collections.IDictionary"></see> object has a fixed size.
		/// </summary>
		/// <value></value>
		/// <returns>true if the <see cref="T:System.Collections.IDictionary"></see> object has a fixed size; otherwise, false.</returns>
		public bool IsFixedSize
		{
			get { return true; }
		}

		/// <summary>
		/// Gets or sets the <see cref="System.Object"/> with the specified key.
		/// </summary>
		/// <value></value>
		public object this[object key]
		{
			get 
			{
				if (_values == null || _values.Length.Equals(0))
					return null;

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

		/// <summary>
		/// Copies the elements of the <see cref="T:System.Collections.ICollection"></see> to an <see cref="T:System.Array"></see>, starting at a particular <see cref="T:System.Array"></see> index.
		/// </summary>
		/// <param name="array">The one-dimensional <see cref="T:System.Array"></see> that is the destination of the elements copied from <see cref="T:System.Collections.ICollection"></see>. The <see cref="T:System.Array"></see> must have zero-based indexing.</param>
		/// <param name="index">The zero-based index in array at which copying begins.</param>
		/// <exception cref="T:System.ArgumentNullException">array is null. </exception>
		/// <exception cref="T:System.ArgumentOutOfRangeException">index is less than zero. </exception>
		/// <exception cref="T:System.ArgumentException">array is multidimensional.-or- index is equal to or greater than the length of array.-or- The number of elements in the source <see cref="T:System.Collections.ICollection"></see> is greater than the available space from index to the end of the destination array. </exception>
		/// <exception cref="T:System.InvalidCastException">The type of the source <see cref="T:System.Collections.ICollection"></see> cannot be cast automatically to the type of the destination array. </exception>
		public void CopyTo(Array array, int index)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Gets the number of elements contained in the <see cref="T:System.Collections.ICollection"></see>.
		/// </summary>
		/// <value></value>
		/// <returns>The number of elements contained in the <see cref="T:System.Collections.ICollection"></see>.</returns>
		public int Count
		{
			get { return _tablesize; }
		}

		/// <summary>
		/// Gets an object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.
		/// </summary>
		/// <value></value>
		/// <returns>An object that can be used to synchronize access to the <see cref="T:System.Collections.ICollection"></see>.</returns>
		public object SyncRoot
		{
			get { return this; }
		}

		/// <summary>
		/// Gets a value indicating whether access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe).
		/// </summary>
		/// <value></value>
		/// <returns>true if access to the <see cref="T:System.Collections.ICollection"></see> is synchronized (thread safe); otherwise, false.</returns>
		public bool IsSynchronized
		{
			get { return false; }
		}

		/// <summary>
		/// Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		/// An <see cref="T:System.Collections.IEnumerator"></see> object that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator GetEnumerator()
		{
			throw new NotSupportedException();
		}
	}
}
