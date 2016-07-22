// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.DictionaryAdapter
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Reflection;

	using SysPropertyDescriptor = System.ComponentModel.PropertyDescriptor;

	[DebuggerDisplay("Count = {Count}, Adapter = {Adapter}")]
	[DebuggerTypeProxy(typeof(ListProjectionDebugView<>))]
	public class ListProjection<T> :
		IBindingList<T>, // Castle
#if !FEATURE_BINDINGLIST
		IList,
#else
		IBindingList,    // System
#endif
		IEditableObject,
		IRevertibleChangeTracking,
		ICollectionProjection,
		ICollectionAdapterObserver<T>
	{
		private readonly ICollectionAdapter<T> adapter;
		private int addNewIndex  = NoIndex;
		private int addedIndex   = NoIndex;
		private int suspendLevel = 0;
#if FEATURE_BINDINGLIST
		private int changedIndex = NoIndex;
		private PropertyChangedEventHandler propertyHandler;
		private static PropertyDescriptorCollection itemProperties;
#endif
		private const int NoIndex = -1;

		public ListProjection(ICollectionAdapter<T> adapter)
		{
			if (adapter == null)
				throw new ArgumentNullException("adapter");

			this.adapter = adapter;
			adapter.Initialize(this);
		}

		public int Count
		{
			get { return adapter.Count; }
		}

#if FEATURE_BINDINGLIST
		public IBindingList AsBindingList
		{
			get { return this; }
		}
#endif

		public ICollectionAdapter<T> Adapter
		{
			get { return adapter; }
		}

		public IEqualityComparer<T> Comparer
		{
			get { return adapter.Comparer ?? EqualityComparer<T>.Default; }
		}

		// Generic IBindingList Properties
		bool IBindingList<T>.AllowEdit                      { get { return true;  } }
		bool IBindingList<T>.AllowNew                       { get { return true;  } }
		bool IBindingList<T>.AllowRemove                    { get { return true;  } }
		bool IBindingList<T>.SupportsChangeNotification     { get { return true;  } }
		bool IBindingList<T>.SupportsSearching              { get { return false; } }
		bool IBindingList<T>.SupportsSorting                { get { return false; } }
		bool IBindingList<T>.IsSorted                       { get { return false; } }
		SysPropertyDescriptor IBindingList<T>.SortProperty  { get { return null;  } }
#if FEATURE_LISTSORT
		ListSortDirection     IBindingList<T>.SortDirection { get { return ListSortDirection.Ascending; } }
#endif

#if FEATURE_BINDINGLIST
		// System IBindingList Properties
		bool IBindingList.AllowEdit                      { get { return true;  } }
		bool IBindingList.AllowNew                       { get { return true;  } }
		bool IBindingList.AllowRemove                    { get { return true;  } }
		bool IBindingList.SupportsChangeNotification     { get { return true;  } }
		bool IBindingList.SupportsSearching              { get { return false; } }
		bool IBindingList.SupportsSorting                { get { return false; } }
		bool IBindingList.IsSorted                       { get { return false; } }
		SysPropertyDescriptor IBindingList.SortProperty  { get { return null;  } }
		ListSortDirection     IBindingList.SortDirection { get { return ListSortDirection.Ascending; } }

		// Other Binding Properties
		bool IRaiseItemChangedEvents.RaisesItemChangedEvents { get { return true; } }
#endif

		// IList Properties
		bool   IList.IsFixedSize          { get { return false; } }
		bool   IList.IsReadOnly           { get { return false; } }
		bool   ICollection<T>.IsReadOnly  { get { return false; } }
		bool   ICollection.IsSynchronized { get { return false; } }
		object ICollection.SyncRoot       { get { return this;  } }

		public virtual bool Contains(T item)
		{
			return IndexOf(item) >= 0;
		}

		bool IList.Contains(object item)
		{
			return Contains((T) item);
		}

		public int IndexOf(T item)
		{
			var count    = Count;
			var comparer = Comparer;

			for (var i = 0; i < count; i++)
				if (comparer.Equals(this[i], item))
					return i;

			return -1;
		}

		int IList.IndexOf(object item)
		{
			return IndexOf((T) item);
		}

		public void CopyTo(T[] array, int index)
		{
			var count = Count;

			for (int i = 0, j = index; i < count; i++, j++)
				array[j] = this[i];
		}

		void ICollection.CopyTo(Array array, int index)
		{
			CopyTo((T[]) array, index);
		}

		public IEnumerator<T> GetEnumerator()
		{
			var count = Count;

			for (var i = 0; i < count; i++)
				yield return this[i];
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public T this[int index]
		{
			get { return adapter[index]; }
			set { adapter[index] = value; }
		}

		object IList.this[int index]
		{
			get { return this[index]; }
			set { this[index] = (T) value; }
		}

		public void Replace(IEnumerable<T> items)
		{
			(this as ICollectionProjection).Replace(items);
		}

		void ICollectionProjection.Replace(IEnumerable items)
		{
		    SuspendEvents();
		    try
		    {
		        Clear();
		        foreach (T item in items)
		            Add(item);
		    }
		    finally
		    {
		        ResumeEvents();
		    }
		}

		protected virtual bool OnReplacing(T oldValue, T newValue)
		{
			return true;
		}

		bool ICollectionAdapterObserver<T>.OnReplacing(T oldValue, T newValue)
		{
			return OnReplacing(oldValue, newValue);
		}

		protected virtual void OnReplaced(T oldValue, T newValue, int index)
		{
			DetachPropertyChanged(oldValue);
			AttachPropertyChanged(newValue);
			NotifyListChanged(ListChangedType.ItemChanged, index);
		}

		void ICollectionAdapterObserver<T>.OnReplaced(T oldValue, T newValue, int index)
		{
			OnReplaced(oldValue, newValue, index);
		}

		public virtual T AddNew()
		{
			var item = (T) adapter.AddNew();
			addNewIndex = addedIndex;
			return item;
		}

#if FEATURE_BINDINGLIST
		object IBindingList.AddNew()
		{
			return AddNew();
		}
#endif

		public bool IsNew(int index)
		{
			return index == addNewIndex
				&& index >= 0;
		}

		public virtual void EndNew(int index)
		{
			if (IsNew(index))
				addNewIndex = NoIndex;
		}

		public virtual void CancelNew(int index)
		{
			if (IsNew(index))
			{
				RemoveAt(addNewIndex);
				addNewIndex = NoIndex;
			}
		}

		public virtual bool Add(T item)
		{
			return adapter.Add(item);
		}

		void ICollection<T>.Add(T item)
		{
			Add(item);
		}

		int IList.Add(object item)
		{
			Add((T) item);
			return addedIndex;
		}

		public void Insert(int index, T item)
		{
			if (index < 0)
				throw new ArgumentOutOfRangeException("index");

			var count = Count;
			if (index > count)
				throw new ArgumentOutOfRangeException("index");

			EndNew(addNewIndex);
			if (index == count)
				adapter.Add(item);
			else
				adapter.Insert(index, item);
		}

		void IList.Insert(int index, object item)
		{
			Insert(index, (T) item);
		}

		protected virtual bool OnInserting(T value)
		{
			return true;
		}

		bool ICollectionAdapterObserver<T>.OnInserting(T value)
		{
			return OnInserting(value);
		}

		protected virtual void OnInserted(T newValue, int index)
		{
			addedIndex = index;
			AttachPropertyChanged(newValue);
			NotifyListChanged(ListChangedType.ItemAdded, index);
		}

		void ICollectionAdapterObserver<T>.OnInserted(T newValue, int index)
		{
			OnInserted(newValue, index);
		}

		public virtual bool Remove(T item)
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

		public virtual void RemoveAt(int index)
		{
			EndNew(addNewIndex);
			adapter.Remove(index);
		}

		public virtual void Clear()
		{
			EndNew(addNewIndex);
			adapter.Clear();
			NotifyListReset();
		}

		void ICollectionProjection.ClearReferences()
		{
			adapter.ClearReferences();
		}

		protected virtual void OnRemoving(T oldValue)
		{
			DetachPropertyChanged(oldValue);
		}

		void ICollectionAdapterObserver<T>.OnRemoving(T oldValue)
		{
			OnRemoving(oldValue);
		}

		protected virtual void OnRemoved(T oldValue, int index)
		{
			NotifyListChanged(ListChangedType.ItemDeleted, index);
		}

		void ICollectionAdapterObserver<T>.OnRemoved(T oldValue, int index)
		{
			OnRemoved(oldValue, index);
		}

		public bool IsChanged
		{
			get
			{
				if (adapter.HasSnapshot == false)
					return false;

				var count = Count;
				if (adapter.SnapshotCount != count)
					return true;

				var comparer = Comparer;
				for (var i = 0; i < count; i++)
				{
					var currentItem  = adapter.GetCurrentItem (i);
					var snapshotItem = adapter.GetSnapshotItem(i);

					if (comparer.Equals(currentItem, snapshotItem) == false)
						return true;

					var tracked = currentItem as IChangeTracking;
					if (tracked != null && tracked.IsChanged)
						return true;
				}

				return false;
			}
		}

		public void BeginEdit()
		{
			if (!adapter.HasSnapshot)
				adapter.SaveSnapshot();
		}

		public void EndEdit()
		{
			adapter.DropSnapshot();
		}

		public void CancelEdit()
		{
			if (adapter.HasSnapshot)
			{
				adapter.LoadSnapshot();
				adapter.DropSnapshot();
				NotifyListReset();
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

#if !FEATURE_BINDINGLIST
		[Conditional("NOP")]
		private void AttachPropertyChanged(T value) { }

		[Conditional("NOP")]
		private void DetachPropertyChanged(T value) { }
#else
		private void AttachPropertyChanged(T value)
		{
			if (typeof(T).GetTypeInfo().IsValueType)
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
			if (typeof(T).GetTypeInfo().IsValueType)
				return;

			var notifier = value as INotifyPropertyChanged;
			if (notifier == null || propertyHandler == null)
				return;

			notifier.PropertyChanged -= propertyHandler;
		}

		private void HandlePropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			T item;
			var notify
				=  EventsEnabled
				&& CanHandle(sender, e)
				&& TryGetChangedItem(sender, out item)
				&& TryGetChangedIndex(item);

			if (notify)
			{
				var property = GetChangedProperty(e);
				var change   = new ListChangedEventArgs(ListChangedType.ItemChanged, changedIndex, property);
				OnListChanged(change);
			}
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
				&& changedIndex <  Count
				&& Comparer.Equals(this[changedIndex], item);
			if (isSameItem)
				return true;
			
			changedIndex = IndexOf(item);
			if (changedIndex >= 0)
				return true;

			DetachPropertyChanged(item);
			NotifyListReset();
			return false;
		}

		private static SysPropertyDescriptor GetChangedProperty(PropertyChangedEventArgs e)
		{
			if (itemProperties == null)
				itemProperties = TypeDescriptor.GetProperties(typeof(T));

			return itemProperties.Find(e.PropertyName, true);
		}
#endif

#if FEATURE_BINDINGLIST
		public event ListChangedEventHandler ListChanged;
		protected virtual void OnListChanged(ListChangedEventArgs args)
		{
			var handler = ListChanged;
			if (handler != null)
				handler(this, args);
		}
#endif

#if !FEATURE_BINDINGLIST
		protected enum ListChangedType
		{
			ItemAdded,
			ItemChanged,
			ItemDeleted
		}

		[Conditional("NOP")]
		protected void NotifyListChanged(ListChangedType type, int index) { }

		[Conditional("NOP")]
		protected void NotifyListReset() { }
#else
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
#endif

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

		void IBindingList<T>.AddIndex(SysPropertyDescriptor property)
		{
			// Do nothing
		}

#if FEATURE_BINDINGLIST
		void IBindingList.AddIndex(SysPropertyDescriptor property)
		{
			// Do nothing
		}
#endif

		void IBindingList<T>.RemoveIndex(SysPropertyDescriptor property)
		{
			// Do nothing
		}

#if FEATURE_BINDINGLIST
		void IBindingList.RemoveIndex(SysPropertyDescriptor property)
		{
			// Do nothing
		}
#endif

		int IBindingList<T>.Find(SysPropertyDescriptor property, object key)
		{
			throw new NotSupportedException();
		}

#if FEATURE_BINDINGLIST
		int IBindingList.Find(SysPropertyDescriptor property, object key)
		{
			throw new NotSupportedException();
		}
#endif

#if FEATURE_LISTSORT
		void IBindingList<T>.ApplySort(SysPropertyDescriptor property, ListSortDirection direction)
		{
			throw new NotSupportedException();
		}
#endif

#if FEATURE_BINDINGLIST
		void IBindingList.ApplySort(SysPropertyDescriptor property, ListSortDirection direction)
		{
			throw new NotSupportedException();
		}
#endif

		void IBindingList<T>.RemoveSort()
		{
			throw new NotSupportedException();
		}

#if FEATURE_BINDINGLIST
		void IBindingList.RemoveSort()
		{
			throw new NotSupportedException();
		}
#endif
	}
}
