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
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

#if FEATURE_BINDINGLIST // Until support for other platforms is verified
namespace Castle.Components.DictionaryAdapter
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using SCM = System.ComponentModel;

	/// <summary>
	///   Provides a generic collection that supports data binding.
	/// </summary>
	/// <remarks>
	///   This class wraps the CLR <see cref="System.ComponentModel.BindingList{T}"/>
	///   in order to implement the Castle-specific <see cref="IBindingList{T}"/>.
	/// </remarks>
	/// <typeparam name="T">The type of elements in the list.</typeparam>
	public class BindingList<T> : IBindingList<T>, IList
	{
		private readonly SCM.BindingList<T> list;

		/// <summary>
		///   Initializes a new instance of the <see cref="BindingList{T}"/> class
		///   using default values.
		/// </summary>
		public BindingList()
		{
			this.list = new SCM.BindingList<T>();
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="BindingList{T}"/> class
		///   with the specified list.
		/// </summary>
		/// <param name="list">
		///   An <see cref="IList{T}"/> of items
		///   to be contained in the <see cref="BindingList{T}"/>.
		/// </param>
		public BindingList(IList<T> list)
		{
			this.list = new SCM.BindingList<T>(list);
		}

		/// <summary>
		///   Initializes a new instance of the <see cref="BindingList{T}"/> class
		///   wrapping the specified <see cref="System.ComponentModel.BindingList{T}"/> instance.
		/// </summary>
		/// <param name="list">
		///   A <see cref="System.ComponentModel.BindingList{T}"/>
		///   to be wrapped by the <see cref="BindingList{T}"/>.
		/// </param>
		public BindingList(SCM.BindingList<T> list)
		{
			if (list == null)
				throw new ArgumentNullException("list");

			this.list = list;
		}

		public SCM.BindingList<T> InnerList
		{
			get { return list; }
		}

		public SCM.IBindingList AsBindingList
		{
			get { return list; }
		}

		public int Count
		{
			get { return list.Count; }
		}

		bool ICollection<T>.IsReadOnly
		{
			get { return ((ICollection<T>) list).IsReadOnly; }
		}

		bool IList.IsReadOnly
		{
			get { return ((IList) list).IsReadOnly; }
		}

		bool IList.IsFixedSize
		{
			get { return ((IList) list).IsFixedSize; }
		}

		bool ICollection.IsSynchronized
		{
			get { return ((ICollection) list).IsSynchronized; }
		}

		object ICollection.SyncRoot
		{
			get { return ((ICollection) list).SyncRoot; }
		}

		public bool AllowNew
		{
			get { return list.AllowNew; }
			set { list.AllowNew = value; }
		}

		public bool AllowEdit
		{
			get { return list.AllowEdit; }
			set { list.AllowEdit = value; }
		}

		public bool AllowRemove
		{
			get { return list.AllowRemove; }
			set { list.AllowRemove = value; }
		}

		public bool RaiseListChangedEvents
		{
			get { return list.RaiseListChangedEvents; }
			set { list.RaiseListChangedEvents = value; }
		}

		bool SCM.IRaiseItemChangedEvents.RaisesItemChangedEvents
		{
			get { return ((SCM.IRaiseItemChangedEvents) list).RaisesItemChangedEvents; }
		}

		bool IBindingList<T>.SupportsChangeNotification
		{
			get { return AsBindingList.SupportsChangeNotification; }
		}

		bool IBindingList<T>.SupportsSearching
		{
			get { return AsBindingList.SupportsSearching; }
		}

		bool IBindingList<T>.SupportsSorting
		{
			get { return AsBindingList.SupportsSorting; }
		}

		bool IBindingList<T>.IsSorted
		{
			get { return AsBindingList.IsSorted; }
		}

		SCM.PropertyDescriptor IBindingList<T>.SortProperty
		{
			get { return AsBindingList.SortProperty; }
		}

		SCM.ListSortDirection IBindingList<T>.SortDirection
		{
			get { return AsBindingList.SortDirection; }
		}

		int IBindingList<T>.Find(SCM.PropertyDescriptor property, object key)
		{
			return AsBindingList.Find(property, key);
		}

		void IBindingList<T>.AddIndex(SCM.PropertyDescriptor property)
		{
			AsBindingList.AddIndex(property);
		}

		void IBindingList<T>.RemoveIndex(SCM.PropertyDescriptor property)
		{
			AsBindingList.RemoveIndex(property);
		}

		void IBindingList<T>.ApplySort(SCM.PropertyDescriptor property, SCM.ListSortDirection direction)
		{
			AsBindingList.ApplySort(property, direction);
		}

		void IBindingList<T>.RemoveSort()
		{
			AsBindingList.RemoveSort();
		}

		public event SCM.AddingNewEventHandler AddingNew
		{
			add    { list.AddingNew += value; }
			remove { list.AddingNew -= value; }
		}

		public event SCM.ListChangedEventHandler ListChanged
		{
			add    { list.ListChanged += value; }
			remove { list.ListChanged -= value; }
		}

		public T this[int index]
		{
			get { return list[index]; }
			set { list[index] = value; }
		}

		object IList.this[int index]
		{
			get { return ((IList) list)[index]; }
			set { ((IList) list)[index] = value; }
		}

		public bool Contains(T item)
		{
			return list.Contains(item);
		}

		bool IList.Contains(object value)
		{
			return ((IList) list).Contains(value);
		}

		public int IndexOf(T item)
		{
			return list.IndexOf(item);
		}

		int IList.IndexOf(object value)
		{
			return ((IList) list).IndexOf(value);
		}

		public void CopyTo(T[] array, int index)
		{
			list.CopyTo(array, index);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			((IList) list).CopyTo(array, index);
		}

		public IEnumerator<T> GetEnumerator()
		{
			return list.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return list.GetEnumerator();
		}

		public T AddNew()
		{
			return list.AddNew();
		}

		public void CancelNew(int index)
		{
			list.CancelNew(index);
		}

		public void EndNew(int index)
		{
			list.EndNew(index);
		}

		public void Add(T item)
		{
			list.Add(item);
		}

		int IList.Add(object item)
		{
			return ((IList) list).Add(item);
		}

		public void Insert(int index, T item)
		{
			list.Insert(index, item);
		}

		void IList.Insert(int index, object item)
		{
			((IList) list).Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			list.RemoveAt(index);
		}

		public bool Remove(T item)
		{
			return list.Remove(item);
		}

		void IList.Remove(object item)
		{
			((IList) list).Remove(item);
		}

		public void Clear()
		{
			list.Clear();
		}

		public void ResetBindings()
		{
			list.ResetBindings();
		}

		public void ResetItem(int index)
		{
			list.ResetItem(index);
		}
	}
}
#endif
