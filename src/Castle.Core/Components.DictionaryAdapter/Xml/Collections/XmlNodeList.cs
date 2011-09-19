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

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;

	internal class XmlNodeList<T> : IBindingList<T>, IBindingList, IXmlCollection, IEditableObject, IRevertibleChangeTracking
	{
		private List<XmlCollectionItem<T>> items;
		private List<XmlCollectionItem<T>> snapshot;
		private int addedIndex   = -1;
		private int changedIndex = -1;

		private readonly IXmlCursor cursor;
		private readonly IXmlCollectionAccessor accessor;
		private readonly IXmlNode parentNode;
		private readonly IDictionaryAdapter parentObject;
		private PropertyChangedEventHandler propertyHandler;

		private static PropertyDescriptorCollection itemProperties;

		public XmlNodeList(
			IXmlNode parentNode,
			IDictionaryAdapter parentObject,
			IXmlCollectionAccessor accessor)
		{
			items = new List<XmlCollectionItem<T>>();

			this.accessor     = accessor;
			this.cursor       = accessor.SelectCollectionItems(parentNode, true);
			this.parentNode   = parentNode;
			this.parentObject = parentObject;

			while (cursor.MoveNext())
				items.Add(new XmlCollectionItem<T>(cursor.Save()));
		}

		public int Count
		{
			get { return items.Count; }
		}

		public IBindingList AsBindingList
		{
			get { return this; }
		}

		IXmlNode IXmlCollection.Node
		{
			get { return parentNode; }
		}

		bool IBindingList<T>.AllowEdit                   { get { return true;  } }
		bool IBindingList<T>.AllowNew                    { get { return true;  } }
		bool IBindingList<T>.AllowRemove                 { get { return true;  } }
		bool IBindingList<T>.SupportsChangeNotification  { get { return true;  } }
		bool IBindingList<T>.SupportsSearching           { get { return false; } }
		bool IBindingList<T>.SupportsSorting             { get { return false; } }
		bool IBindingList<T>.IsSorted                    { get { return false; } }
		PropertyDescriptor IBindingList<T>.SortProperty  { get { return null;  } }
		ListSortDirection  IBindingList<T>.SortDirection { get { return ListSortDirection.Ascending; } }

		bool IBindingList.AllowEdit                   { get { return true;  } }
		bool IBindingList.AllowNew                    { get { return true;  } }
		bool IBindingList.AllowRemove                 { get { return true;  } }
		bool IBindingList.SupportsChangeNotification  { get { return true;  } }
		bool IBindingList.SupportsSearching           { get { return false; } }
		bool IBindingList.SupportsSorting             { get { return false; } }
		bool IBindingList.IsSorted                    { get { return false; } }
		PropertyDescriptor IBindingList.SortProperty  { get { return null;  } }
		ListSortDirection  IBindingList.SortDirection { get { return ListSortDirection.Ascending; } }

		bool IRaiseItemChangedEvents.RaisesItemChangedEvents { get { return true;  } }

		bool IList.IsFixedSize          { get { return false; } }
		bool IList.IsReadOnly           { get { return false; } }
		bool ICollection<T>.IsReadOnly  { get { return false; } }
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot     { get { return this;  } }

		public T this[int index]
		{
			get
			{
				var item = items[index];
				if (!item.HasValue)
					items[index] = item = item.WithValue(GetValue(item.Node));
				return item.Value;
			}
			set
			{
				var item = items[index];
				SetValue(item.Node, ref value);
				items[index] = item.WithValue(value);
			}
		}

		object IList.this[int index]
		{
			get { return this[index]; }
			set { this[index] = (T) value; }
		}

		public bool Contains(T item)
		{
			return IndexOf(item) >= 0;
		}

		bool IList.Contains(object value)
		{
			return Contains((T) value);
		}

		public int IndexOf(T item)
		{
			var comparer = EqualityComparer<T>.Default;

			for (var i = 0; i < Count; i++)
				if (comparer.Equals(this[i], item))
					return i;

			return -1;
		}

		int IList.IndexOf(object value)
		{
			return IndexOf((T) value);
		}

		public void CopyTo(T[] array, int index)
		{
			for (int i = 0, j = index; i < Count; i++, j++)
				array[j] = this[i];
		}

		void ICollection.CopyTo(Array array, int index)
		{
			CopyTo((T[]) array, index);
		}

		public IEnumerator<T> GetEnumerator()
		{
			for (var i = 0; i < Count; i++)
				yield return this[i];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public T AddNew()
		{
			cursor.MoveToEnd();
			cursor.Create(typeof(T));

			var node   = cursor.Save();
			var value  = GetValue(node);
			var item   = new XmlCollectionItem<T>(node, value);

			addedIndex = items.Count;
			items.Add(item);
			AttachPropertyChanged(value);
			return (T) value;
		}

		object IBindingList.AddNew()
		{
			return AddNew();
		}

		public void EndNew(int index)
		{
			if (addedIndex == index)
				addedIndex = -1;
		}

		public void CancelNew(int index)
		{
			if (addedIndex == index && addedIndex >= 0)
			{
				RemoveAt(addedIndex);
				addedIndex = -1;
			}
		}

		public void Add(T value)
		{
			cursor.MoveToEnd();
			items.Add(Create(value));
		}

		int IList.Add(object value)
		{
			Add((T) value);
			return IndexOf((T) value);
		}

		public void Insert(int index, T value)
		{
			if (index == Count)
				Add(value);
			else
			{
				cursor.MoveTo(items[index].Node);
				items.Insert(index, Create(value));
			}
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, (T) value);
		}

		private XmlCollectionItem<T> Create(T value)
		{
			cursor.Create(GetTypeOrDefault(value));
			var node = cursor.Save();
			SetValue(node, ref value);
			AttachPropertyChanged(value);
			return new XmlCollectionItem<T>(node, value);
		}

		public bool Remove(T item)
		{
			var index = IndexOf(item);
			if (index < 0) return false;
			RemoveAt(index);
			return true;
		}

		void IList.Remove(object value)
		{
			Remove((T) value);
		}

		public void RemoveAt(int index)
		{
			var item = items[index];
			DetachPropertyChanged(item.Value);
			cursor.MoveTo(item.Node);
			cursor.Remove();
			items.RemoveAt(index);
		}

		public void Clear()
		{
			foreach (var item in items)
				DetachPropertyChanged(item.Value);
			cursor.Reset();
			cursor.RemoveAllNext();
			items.Clear();
		}

		void IXmlCollection.Replace(IEnumerable source)
		{
			Clear();
			foreach (T value in source)
				Add(value);
		}

		private T GetValue(IXmlNode node)
		{
			return (T) accessor.Serializer.GetValue(node, parentObject, accessor);
		}

		private void SetValue(IXmlNode node, ref T value)
		{
			object obj = value;
			accessor.Serializer.SetValue(node, parentObject, accessor, ref obj);
			value = (T) obj;
		}

		private static Type GetTypeOrDefault(T value)
		{
			return (null == value)
				? typeof(T)
				: value.GetComponentType();
		}

		void IBindingList<T>.AddIndex(PropertyDescriptor property)
		{
			// Do nothing
		}

		void IBindingList.AddIndex(PropertyDescriptor property)
		{
			// Do nothing
		}

		void IBindingList<T>.RemoveIndex(PropertyDescriptor property)
		{
			// Do nothing
		}

		void IBindingList.RemoveIndex(PropertyDescriptor property)
		{
			// Do nothing
		}

		int IBindingList<T>.Find(PropertyDescriptor property, object key)
		{
			throw Error.NotSupported();
		}

		int IBindingList.Find(PropertyDescriptor property, object key)
		{
			throw Error.NotSupported();
		}

		void IBindingList<T>.ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			throw Error.NotSupported();
		}

		void IBindingList.ApplySort(PropertyDescriptor property, ListSortDirection direction)
		{
			throw Error.NotSupported();
		}

		void IBindingList<T>.RemoveSort()
		{
			throw Error.NotSupported();
		}

		void IBindingList.RemoveSort()
		{
			throw Error.NotSupported();
		}

		public bool IsChanged
		{
			get
			{
				if (snapshot == null)
					return false;
				if (snapshot.Count != items.Count)
					return true;

				var a = items   .GetEnumerator();
				var b = snapshot.GetEnumerator();

				while (a.MoveNext() && b.MoveNext())
				{
					if (!ReferenceEquals(a.Current, b.Current))
						return true;

					var tracked = a.Current.Value as IChangeTracking;
					if (tracked != null && tracked.IsChanged)
						return true;
				}

				return false;
			}
		}

		public void BeginEdit()
		{
			if (snapshot == null)
				snapshot = new List<XmlCollectionItem<T>>(items);
		}

		public void EndEdit()
		{
			snapshot = null;
		}

		public void CancelEdit()
		{
			if (snapshot != null)
			{
				items = snapshot;
				snapshot = null;
				OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
			}
		}

		public void AcceptChanges()
		{
			BeginEdit();
		}

		public void RejectChanges()
		{
			CancelEdit();
		}

		private void AttachPropertyChanged(T value)
		{
			if (typeof(T).IsValueType)
				return;

			var notifier = value as INotifyPropertyChanged;
			if (notifier == null)
				return;

			if (propertyHandler == null)
				propertyHandler = HandlePropertyChanged;

			notifier.PropertyChanged += propertyHandler;
		}

		private void DetachPropertyChanged(T value)
		{
			if (typeof(T).IsValueType)
				return;

			var notifier = value as INotifyPropertyChanged;
			if (notifier == null || propertyHandler == null)
				return;

			notifier.PropertyChanged -= propertyHandler;
		}

		private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			T item;

			if (!CanHandle(sender, e))
				return;
			if (!TryGetChangedItem(sender, out item))
				return;
			if (!TryGetChangedIndex(item))
				return;

			var property = GetChangedProperty(e);
			var change = new ListChangedEventArgs(ListChangedType.ItemChanged, changedIndex, property);
			OnListChanged(change);
		}

		private bool CanHandle(object sender, PropertyChangedEventArgs e)
		{
			if (sender == null || e == null || string.IsNullOrEmpty(e.PropertyName))
			{
				NotifyListReset();
				return false;
			}
			return true;
		}

		private bool TryGetChangedItem(object sender, out T item)
		{
			try
			{
				item = (T) sender;
				return true;
			}
			catch (InvalidCastException)
			{
				NotifyListReset();
				item = default(T);
				return false;
			}
		}

		private bool TryGetChangedIndex(T item)
		{
			var isSameItem
				=  changedIndex >= 0
				&& changedIndex <  items.Count
				&& EqualityComparer<T>.Default.Equals(this[changedIndex], item);
			if (isSameItem)
				return true;
			
			changedIndex = IndexOf(item);
			if (changedIndex >= 0)
				return true;

			DetachPropertyChanged(item);
			NotifyListReset();
			return false;
		}

		private static PropertyDescriptor GetChangedProperty(PropertyChangedEventArgs e)
		{
			if (itemProperties == null)
				itemProperties = TypeDescriptor.GetProperties(typeof(T));

			return itemProperties.Find(e.PropertyName, true);
		}

		public event ListChangedEventHandler ListChanged;
		protected virtual void OnListChanged(ListChangedEventArgs args)
		{
			if (ListChanged != null)
				ListChanged(this, args);
		}

		protected virtual void NotifyListReset()
		{
			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}
	}
}
