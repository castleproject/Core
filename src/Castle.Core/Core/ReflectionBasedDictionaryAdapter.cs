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

namespace Castle.Core
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;

	/// <summary>
	/// Readonly implementation of <see cref="IDictionary"/> which uses an anonymous object as its source. Uses names of properties as keys, and property values as... well - values. Keys are not case sensitive.
	/// </summary>
	public sealed class ReflectionBasedDictionaryAdapter : IDictionary
	{
		private readonly Dictionary<string, object> properties =
			new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

		/// <summary>
		///   Initializes a new instance of the <see cref = "ReflectionBasedDictionaryAdapter" /> class.
		/// </summary>
		/// <param name = "target">The target.</param>
		public ReflectionBasedDictionaryAdapter(object target)
		{
			if (target == null)
			{
				throw new ArgumentNullException("target");
			}
			Read(properties, target);
		}

		/// <summary>
		///   Gets the number of elements contained in the <see cref = "T:System.Collections.ICollection" />.
		/// </summary>
		/// <returns>The number of elements contained in the <see cref = "T:System.Collections.ICollection" />.</returns>
		public int Count
		{
			get { return properties.Count; }
		}

		/// <summary>
		///   Gets a value indicating whether access to the <see cref = "T:System.Collections.ICollection" /> is synchronized (thread safe).
		/// </summary>
		/// <returns>true if access to the <see cref = "T:System.Collections.ICollection" /> is synchronized (thread safe); otherwise, false.</returns>
		public bool IsSynchronized
		{
			get { return false; }
		}

		/// <summary>
		///   Gets an object that can be used to synchronize access to the <see cref = "T:System.Collections.ICollection" />.
		/// </summary>
		/// <returns>An object that can be used to synchronize access to the <see cref = "T:System.Collections.ICollection" />.</returns>
		public object SyncRoot
		{
			get { return properties; }
		}

		/// <summary>
		///   Gets a value indicating whether the <see cref = "T:System.Collections.IDictionary" /> object is read-only.
		/// </summary>
		/// <returns>true if the <see cref = "T:System.Collections.IDictionary" /> object is read-only; otherwise, false.</returns>
		public bool IsReadOnly
		{
			get { return true; }
		}

		/// <summary>
		///   Gets or sets the <see cref = "Object" /> with the specified key.
		/// </summary>
		public object this[object key]
		{
			get
			{
				object value;
				properties.TryGetValue(key.ToString(), out value);
				return value;
			}
			set { throw new NotImplementedException(); }
		}

		/// <summary>
		///   Gets an <see cref = "T:System.Collections.ICollection" /> object containing the keys of the <see
		///    cref = "T:System.Collections.IDictionary" /> object.
		/// </summary>
		/// <returns>An <see cref = "T:System.Collections.ICollection" /> object containing the keys of the <see
		///    cref = "T:System.Collections.IDictionary" /> object.</returns>
		public ICollection Keys
		{
			get { return properties.Keys; }
		}

		/// <summary>
		///   Gets an <see cref = "T:System.Collections.ICollection" /> object containing the values in the <see
		///    cref = "T:System.Collections.IDictionary" /> object.
		/// </summary>
		/// <returns>An <see cref = "T:System.Collections.ICollection" /> object containing the values in the <see
		///    cref = "T:System.Collections.IDictionary" /> object.</returns>
		public ICollection Values
		{
			get { return properties.Values; }
		}

		/// <summary>
		///   Gets a value indicating whether the <see cref = "T:System.Collections.IDictionary" /> object has a fixed size.
		/// </summary>
		/// <returns>true if the <see cref = "T:System.Collections.IDictionary" /> object has a fixed size; otherwise, false.</returns>
		bool IDictionary.IsFixedSize
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		///   Adds an element with the provided key and value to the <see cref = "T:System.Collections.IDictionary" /> object.
		/// </summary>
		/// <param name = "key">The <see cref = "T:System.Object" /> to use as the key of the element to add.</param>
		/// <param name = "value">The <see cref = "T:System.Object" /> to use as the value of the element to add.</param>
		/// <exception cref = "T:System.ArgumentNullException">
		///   <paramref name = "key" /> is null. </exception>
		/// <exception cref = "T:System.ArgumentException">An element with the same key already exists in the <see
		///    cref = "T:System.Collections.IDictionary" /> object. </exception>
		/// <exception cref = "T:System.NotSupportedException">The <see cref = "T:System.Collections.IDictionary" /> is read-only.-or- The <see
		///    cref = "T:System.Collections.IDictionary" /> has a fixed size. </exception>
		public void Add(object key, object value)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Removes all elements from the <see cref = "T:System.Collections.IDictionary" /> object.
		/// </summary>
		/// <exception cref = "T:System.NotSupportedException">The <see cref = "T:System.Collections.IDictionary" /> object is read-only. </exception>
		public void Clear()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Determines whether the <see cref = "T:System.Collections.IDictionary" /> object contains an element with the specified key.
		/// </summary>
		/// <param name = "key">The key to locate in the <see cref = "T:System.Collections.IDictionary" /> object.</param>
		/// <returns>
		///   true if the <see cref = "T:System.Collections.IDictionary" /> contains an element with the key; otherwise, false.
		/// </returns>
		/// <exception cref = "T:System.ArgumentNullException">
		///   <paramref name = "key" /> is null. </exception>
		public bool Contains(object key)
		{
			return properties.ContainsKey(key.ToString());
		}

		/// <summary>
		///   Removes the element with the specified key from the <see cref = "T:System.Collections.IDictionary" /> object.
		/// </summary>
		/// <param name = "key">The key of the element to remove.</param>
		/// <exception cref = "T:System.ArgumentNullException">
		///   <paramref name = "key" /> is null. </exception>
		/// <exception cref = "T:System.NotSupportedException">The <see cref = "T:System.Collections.IDictionary" /> object is read-only.-or- The <see
		///    cref = "T:System.Collections.IDictionary" /> has a fixed size. </exception>
		public void Remove(object key)
		{
		}

		/// <summary>
		///   Returns an enumerator that iterates through a collection.
		/// </summary>
		/// <returns>
		///   An <see cref = "T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.
		/// </returns>
		public IEnumerator GetEnumerator()
		{
			return new DictionaryEntryEnumeratorAdapter(properties.GetEnumerator());
		}

		/// <summary>
		///   Copies the elements of the <see cref = "T:System.Collections.ICollection" /> to an <see cref = "T:System.Array" />, starting at a particular <see
		///    cref = "T:System.Array" /> index.
		/// </summary>
		/// <param name = "array">The one-dimensional <see cref = "T:System.Array" /> that is the destination of the elements copied from <see
		///    cref = "T:System.Collections.ICollection" />. The <see cref = "T:System.Array" /> must have zero-based indexing.</param>
		/// <param name = "index">The zero-based index in <paramref name = "array" /> at which copying begins.</param>
		/// <exception cref = "T:System.ArgumentNullException">
		///   <paramref name = "array" /> is null. </exception>
		/// <exception cref = "T:System.ArgumentOutOfRangeException">
		///   <paramref name = "index" /> is less than zero. </exception>
		/// <exception cref = "T:System.ArgumentException">
		///   <paramref name = "array" /> is multidimensional.-or- <paramref name = "index" /> is equal to or greater than the length of <paramref
		///    name = "array" />.-or- The number of elements in the source <see cref = "T:System.Collections.ICollection" /> is greater than the available space from <paramref
		///    name = "index" /> to the end of the destination <paramref name = "array" />. </exception>
		/// <exception cref = "T:System.ArgumentException">The type of the source <see cref = "T:System.Collections.ICollection" /> cannot be cast automatically to the type of the destination <paramref
		///    name = "array" />. </exception>
		void ICollection.CopyTo(Array array, int index)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		///   Returns an <see cref = "T:System.Collections.IDictionaryEnumerator" /> object for the <see
		///    cref = "T:System.Collections.IDictionary" /> object.
		/// </summary>
		/// <returns>
		///   An <see cref = "T:System.Collections.IDictionaryEnumerator" /> object for the <see
		///    cref = "T:System.Collections.IDictionary" /> object.
		/// </returns>
		IDictionaryEnumerator IDictionary.GetEnumerator()
		{
			return new DictionaryEntryEnumeratorAdapter(properties.GetEnumerator());
		}

		/// <summary>
		///   Reads values of properties from <paramref name = "valuesAsAnonymousObject" /> and inserts them into <paramref
		///    name = "targetDictionary" /> using property names as keys.
		/// </summary>
		public static void Read(IDictionary targetDictionary, object valuesAsAnonymousObject)
		{
			var targetType = valuesAsAnonymousObject.GetType();
			foreach (var property in GetReadableProperties(targetType))
			{
				var value = GetPropertyValue(valuesAsAnonymousObject, property);
				targetDictionary[property.Name] = value;
			}
		}

		private static object GetPropertyValue(object target, PropertyInfo property)
		{
			return property.GetValue(target, null);
		}

		private static IEnumerable<PropertyInfo> GetReadableProperties(Type targetType)
		{
			return targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(IsReadable);
		}

		private static bool IsReadable(PropertyInfo property)
		{
			return property.CanRead && property.GetIndexParameters().Length == 0;
		}

		private class DictionaryEntryEnumeratorAdapter : IDictionaryEnumerator
		{
			private readonly IDictionaryEnumerator enumerator;
			private KeyValuePair<string, object> current;

			public DictionaryEntryEnumeratorAdapter(IDictionaryEnumerator enumerator)
			{
				this.enumerator = enumerator;
			}

			public DictionaryEntry Entry
			{
				get { return new DictionaryEntry(Key, Value); }
			}

			public object Key
			{
				get { return current.Key; }
			}

			public object Value
			{
				get { return current.Value; }
			}

			public object Current
			{
				get { return new DictionaryEntry(Key, Value); }
			}

			public bool MoveNext()
			{
				var moved = enumerator.MoveNext();

				if (moved)
				{
					current = (KeyValuePair<string, object>)enumerator.Current;
				}

				return moved;
			}

			public void Reset()
			{
				enumerator.Reset();
			}
		}
	}
}