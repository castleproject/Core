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

namespace Commons.Collections
{
	using System;
	using System.Collections;

	/// <summary>
	/// A keyed list with a fixed maximum size which removes
	/// the least recently used entry if an entry is added when full.
	/// </summary>
	[Serializable]
	public class LRUMap : ICollection, IDictionary, IEnumerable
	{
		private Hashtable objectTable = new Hashtable();
		private ArrayList objectList = new ArrayList();

		/// <summary>
		/// Default maximum size 
		/// </summary>
		protected internal const int DEFAULT_MAX_SIZE = 100;

		/// <summary>
		/// Maximum size 
		/// </summary>
		[NonSerialized] private int maxSize;

		public LRUMap() : this(DEFAULT_MAX_SIZE)
		{
		}

		public LRUMap(Int32 maxSize)
		{
			this.maxSize = maxSize;
		}

		public virtual void Add(object key, object value)
		{
			if (objectList.Count == maxSize)
			{
				RemoveLRU();
			}

			objectTable.Add(key, value);
			objectList.Insert(0, new DictionaryEntry(key, value));
		}

		public virtual void Clear()
		{
			objectTable.Clear();
			objectList.Clear();
		}

		public virtual bool Contains(object key)
		{
			return objectTable.Contains(key);
		}

		public virtual void CopyTo(Array array, int idx)
		{
			objectTable.CopyTo(array, idx);
		}

		public virtual void Remove(object key)
		{
			objectTable.Remove(key);
			objectList.RemoveAt(IndexOf(key));
		}

		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new KeyedListEnumerator(objectList);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new KeyedListEnumerator(objectList);
		}

		public virtual int Count
		{
			get { return objectList.Count; }
		}

		public virtual bool IsFixedSize
		{
			get { return true; }
		}

		public virtual bool IsReadOnly
		{
			get { return false; }
		}

		public virtual bool IsSynchronized
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the maximum size of the map (the bound).
		/// </summary>
		public Int32 MaxSize
		{
			get { return maxSize; }
		}

		//	public object this[int idx] {
		//	    get { return ((DictionaryEntry) objectList[idx]).Value; }
		//	    set {
		//		if (idx < 0 || idx >= Count)
		//		    throw new ArgumentOutOfRangeException ("index");
		//
		//		object key = ((DictionaryEntry) objectList[idx]).Key;
		//		objectList[idx] = new DictionaryEntry (key, value);
		//		objectTable[key] = value;
		//	    }
		//	}

		public virtual object this[object key]
		{
			get
			{
				MoveToMRU(key);
				return objectTable[key];
			}
			set
			{
				if (objectTable.Contains(key))
				{
					Remove(key);
				}
				Add(key, value);
			}
		}

		public virtual ICollection Keys
		{
			get
			{
				ArrayList retList = new ArrayList();
				for(int i = 0; i < objectList.Count; i++)
				{
					retList.Add(((DictionaryEntry) objectList[i]).Key);
				}
				return retList;
			}
		}

		public virtual ICollection Values
		{
			get
			{
				ArrayList retList = new ArrayList();
				for(int i = 0; i < objectList.Count; i++)
				{
					retList.Add(((DictionaryEntry) objectList[i]).Value);
				}
				return retList;
			}
		}

		public virtual object SyncRoot
		{
			get { return this; }
		}

		public void AddAll(IDictionary dictionary)
		{
			foreach(DictionaryEntry entry in dictionary)
			{
				Add(entry.Key, entry.Value);
			}
		}

		private int IndexOf(object key)
		{
			for(int i = 0; i < objectList.Count; i++)
			{
				if (((DictionaryEntry) objectList[i]).Key.Equals(key))
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>
		/// Remove the least recently used entry (the last one in the list)
		/// </summary>
		private void RemoveLRU()
		{
			Object key = ((DictionaryEntry) objectList[objectList.Count - 1]).Key;
			objectTable.Remove(key);
			objectList.RemoveAt(objectList.Count - 1);
		}

		private void MoveToMRU(Object key)
		{
			Int32 i = IndexOf(key);

			// only move if found and not already first
			if (i > 0)
			{
				Object value = objectList[i];
				objectList.RemoveAt(i);
				objectList.Insert(0, value);
			}
		}

		// Returns a thread-safe wrapper for a LRUMap.
		//
		public static LRUMap Synchronized(LRUMap table)
		{
			if (table == null)
			{
				throw new ArgumentNullException("table");
			}
			return new SyncLRUMap(table);
		}

		// Synchronized wrapper for LRUMap
		[Serializable()]
		private class SyncLRUMap : LRUMap, IDictionary, IEnumerable
		{
			protected LRUMap _table;

			internal SyncLRUMap(LRUMap table)
			{
				_table = table;
			}

//	    internal SyncLRUMap(SerializationInfo info, StreamingContext context) : base (info, context) {
//		_table = (LRUMap)info.GetValue("ParentTable", typeof(LRUMap));
//		if (_table==null) {
//		    throw new SerializationException(Environment.GetResourceString("Serialization_InsufficientState"));
//		}
//	    }


			/*================================GetObjectData=================================
	     **Action: Return a serialization info containing a reference to _table.  We need
	     **        to implement this because our parent HT does and we don't want to actually
	     **        serialize all of it's values (just a reference to the table, which will then
	     **        be serialized separately.)
	     **Returns: void
	     **Arguments: info -- the SerializationInfo into which to store the data.
	     **           context -- the StreamingContext for the current serialization (ignored)
	     **Exceptions: ArgumentNullException if info is null.
	     ==============================================================================*/
//	    public override void GetObjectData(SerializationInfo info, StreamingContext context) {
//		if (info==null) {
//		    throw new ArgumentNullException("info");
//		}
//		info.AddValue("ParentTable", _table, typeof(Hashtable));
//	    }

			public override int Count
			{
				get { return _table.Count; }
			}

			public override bool IsReadOnly
			{
				get { return _table.IsReadOnly; }
			}

			public override bool IsFixedSize
			{
				get { return _table.IsFixedSize; }
			}

			public override bool IsSynchronized
			{
				get { return true; }
			}

			public override Object this[Object key]
			{
				get
				{
					lock (_table.SyncRoot)
					{
						return _table[key];
					}
				}
				set
				{
					lock (_table.SyncRoot)
					{
						_table[key] = value;
					}
				}
			}

			public override Object SyncRoot
			{
				get { return _table.SyncRoot; }
			}

			public override void Add(Object key, Object value)
			{
				lock(_table.SyncRoot)
				{
					_table.Add(key, value);
				}
			}

			public override void Clear()
			{
				lock(_table.SyncRoot)
				{
					_table.Clear();
				}
			}

			public override bool Contains(Object key)
			{
				return _table.Contains(key);
			}

//	    public override bool ContainsKey(Object key) {
//		return _table.ContainsKey(key);
//	    }

//	    public override bool ContainsValue(Object key) {
//		return _table.ContainsValue(key);
//	    }

			public override void CopyTo(Array array, int arrayIndex)
			{
				_table.CopyTo(array, arrayIndex);
			}



			IDictionaryEnumerator IDictionary.GetEnumerator()
			{
				return ((IDictionary) _table).GetEnumerator();
			}

//	    protected override int GetHash(Object key) {
//		return _table.GetHash(key);
//	    }

//	    protected override bool KeyEquals(Object item, Object key) {
//		return _table.KeyEquals(item, key);
//	    }

			public override ICollection Keys
			{
				get
				{
					lock(_table.SyncRoot)
					{
						return _table.Keys;
					}
				}
			}

			public override ICollection Values
			{
				get
				{
					lock(_table.SyncRoot)
					{
						return _table.Values;
					}
				}
			}

			public override void Remove(Object key)
			{
				lock(_table.SyncRoot)
				{
					_table.Remove(key);
				}
			}

			/*==============================OnDeserialization===============================
	     **Action: Does nothing.  We have to implement this because our parent HT implements it,
	     **        but it doesn't do anything meaningful.  The real work will be done when we
	     **        call OnDeserialization on our parent table.
	     **Returns: void
	     **Arguments: None
	     **Exceptions: None
	     ==============================================================================*/
//	    public override void OnDeserialization(Object sender) {
//		return;
//	    }
		}
	}
}