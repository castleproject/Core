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
		private int suspendLevel =  0;

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

		bool IRaiseItemChangedEvents.RaisesItemChangedEvents { get { return true; } }

		bool IList.IsFixedSize          { get { return false; } }
		bool IList.IsReadOnly           { get { return false; } }
		bool ICollection<T>.IsReadOnly  { get { return false; } }
		bool ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot     { get { return this;  } }

		public T this[int index]
		{
			get { return GetValueAt(index); }
			set { SetValueAt(index, value); }
		}

		object IList.this[int index]
		{
			get { return this[index]; }
			set { this[index] = (T) value; }
		}

		public virtual bool Contains(T item)
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

		private T GetValueAt(int index)
		{
			var item = items[index];

			if (!item.HasValue)
				items[index] = item = item.WithValue(GetValue(item.Node));

			return item.Value;
		}

		private void SetValueAt(int index, T value)
		{
			var item = items[index];
			SetValue(item.Node, ref value);

			if (ShouldReplace(item.Value, value))
			{
				items[index] = item.WithValue(value);
				DetachPropertyChanged(item.Value);
				AttachPropertyChanged(value);
				NotifyListChanged(ListChangedType.ItemChanged, index);
			}
			else
			{
				value = (T) item.Value;
				SetValue(item.Node, ref value);
				items[index] = item.WithValue(value);
			}
		}

		protected virtual bool ShouldReplace(T oldValue, T newValue)
		{
			return true;
		}

		public virtual T AddNew()
		{
			cursor.MoveToEnd();
			cursor.Create(typeof(T));

			var node   = cursor.Save();
			var value  = GetValue(node);
			addedIndex = items.Count;

			CommitInsert(addedIndex, node, value, true);
			return (T) value;
		}

		object IBindingList.AddNew()
		{
			return AddNew();
		}

		public bool IsNew(int index)
		{
			return index == addedIndex
				&& index >= 0;
		}

		public virtual void EndNew(int index)
		{
			if (IsNew(index))
				addedIndex = -1;
		}

		public virtual void CancelNew(int index)
		{
			if (IsNew(index))
			{
				RemoveCore(addedIndex);
				addedIndex = -1;
			}
		}

		public virtual bool Add(T value)
		{
			return InsertCore(Count, value, true);
		}

		void ICollection<T>.Add(T value)
		{
			Add(value);
		}

		int IList.Add(object value)
		{
			Add((T) value);
			return IndexOf((T) value);
		}

		public virtual bool Insert(int index, T value)
		{
			if (index < 0 || index > Count)
				throw Error.ArgumentOutOfRange("index");

			EndNew(addedIndex);
			return InsertCore(index, value, index == Count);
		}

		void IList<T>.Insert(int index, T value)
		{
			Insert(index, value);
		}

		void IList.Insert(int index, object value)
		{
			Insert(index, (T) value);
		}

		private bool InsertCore(int index, T value, bool append)
		{
			if (append)
				cursor.MoveToEnd();
			else
				cursor.MoveTo(items[index].Node);

			cursor.Create(GetTypeOrDefault(value));
			var node = cursor.Save();
			SetValue(node, ref value);

			return ShouldAdd(value)
				? CommitInsert(index, node, value, append)
				: RollbackInsert();
		}

		private bool CommitInsert(int index, IXmlNode node, T value, bool append)
		{
			var item = new XmlCollectionItem<T>(node, value);

			if (append)
				items.Insert(index, item);
			else
				items.Add(item);

			AttachPropertyChanged(value);
			NotifyListChanged(ListChangedType.ItemAdded, index);
			return true;
		}

		private bool RollbackInsert()
		{
			cursor.Remove();
			return false;
		}

		protected virtual bool ShouldAdd(T value)
		{
			return true;
		}

		public virtual bool Remove(T item)
		{
			var index = IndexOf(item);
			if (index < 0) return false;
			RemoveCore(index);
			return true;
		}

		void IList.Remove(object value)
		{
			Remove((T) value);
		}

		public virtual void RemoveAt(int index)
		{
			RemoveCore(index);
		}

		private void RemoveCore(int index)
		{
			EndNew(addedIndex);
			var item = items[index];
			DetachPropertyChanged(item.Value);
			cursor.MoveTo(item.Node);
			cursor.Remove();
			items.RemoveAt(index);
			NotifyListChanged(ListChangedType.ItemDeleted, index);
		}

		public virtual void Clear()
		{
			EndNew(addedIndex);
			foreach (var item in items)
				DetachPropertyChanged(item.Value);
			cursor.Reset();
			cursor.RemoveAllNext();
			items.Clear();
			NotifyListReset();
		}

		void IXmlCollection.Replace(IEnumerable source)
		{
			SuspendEvents();
			try
			{
				Clear();
				foreach (T value in source)
					Add(value);
			}
			finally
			{
				ResumeEvents();
			}
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

		protected void NotifyListChanged(ListChangedType type, int index)
		{
			if (EventsEnabled)
				OnListChanged(new ListChangedEventArgs(type, index));
		}

		protected void NotifyListReset()
		{
			if (EventsEnabled)
				OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		public bool EventsEnabled
		{
			get { return suspendLevel == 0; }
		}

		public void SuspendEvents()
		{
			suspendLevel++;
		}

		public bool ResumeEvents()
		{
			var enabled 
				=    suspendLevel == 0
				|| --suspendLevel == 0;

			if (enabled)
				NotifyListReset();

			return enabled;
		}
	}
}
