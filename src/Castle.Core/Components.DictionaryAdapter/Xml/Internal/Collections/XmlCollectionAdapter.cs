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

#if FEATURE_DICTIONARYADAPTER_XML
namespace Castle.Components.DictionaryAdapter.Xml
{
    using System;
    using System.Collections.Generic;

	internal class XmlCollectionAdapter<T> : ICollectionAdapter<T>, IXmlNodeSource
    {
        private List<XmlCollectionItem<T>>  items;
        private List<XmlCollectionItem<T>>  snapshot;
		private ICollectionAdapterObserver<T> advisor;

        private readonly IXmlCursor             cursor;
        private readonly IXmlCollectionAccessor accessor;
        private readonly IXmlNode               parentNode;
        private readonly IDictionaryAdapter     parentObject;
        private readonly XmlReferenceManager    references;

        public XmlCollectionAdapter(
            IXmlNode parentNode,
            IDictionaryAdapter parentObject,
            IXmlCollectionAccessor accessor)
        {
            items = new List<XmlCollectionItem<T>>();

            this.accessor     = accessor;
            this.cursor       = accessor.SelectCollectionItems(parentNode, true);
            this.parentNode   = parentNode;
            this.parentObject = parentObject;
            this.references   = XmlAdapter.For(parentObject).References;

            while (cursor.MoveNext())
                items.Add(new XmlCollectionItem<T>(cursor.Save()));
        }

        public IXmlNode Node
        {
            get { return parentNode; }
        }

        public XmlReferenceManager References
        {
            get { return references; }
        }

		public int Count
		{
			get { return items.Count; }
		}

		public void Initialize(ICollectionAdapterObserver<T> advisor)
		{
			this.advisor = advisor;
		}

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
		        cursor.MoveTo(item.Node);
		        SetValue(cursor, item.Value, ref value);

		        if (advisor.OnReplacing(item.Value, value))
		        {
					// Commit the replacement
		            items[index] = item.WithValue(value);
					advisor.OnReplaced(item.Value, value, index);
		        }
		        else
		        {
					// Rollback the replacement
		            var oldValue = item.Value;
		            SetValue(cursor, value, ref oldValue);
		            items[index] = item.WithValue(oldValue);
		        }
		    }
		}

		public T AddNew()
		{
			cursor.MoveToEnd();
			cursor.Create(typeof(T));

			var node  = cursor.Save();
			var value = GetValue(node);
			var index = items.Count;

			CommitInsert(index, node, value, true);
			return (T) value;
		}

		public bool Add(T value)
		{
			return InsertCore(Count, value, append: true);
		}

		public bool Insert(int index, T value)
		{
			return InsertCore(index, value, append: false);
		}

		private bool InsertCore(int index, T value, bool append)
		{
			if (append)
				cursor.MoveToEnd();
			else
				cursor.MoveTo(items[index].Node);

			cursor.Create(GetTypeOrDefault(value));
			var node = cursor.Save();
			SetValue(cursor, default(T), ref value);

			return advisor.OnInserting(value)
				? CommitInsert(index, node, value, append)
				: RollbackInsert();
		}

		private bool CommitInsert(int index, IXmlNode node, T value, bool append)
		{
			var item = new XmlCollectionItem<T>(node, value);

			if (append)
				items.Add(item);
			else
				items.Insert(index, item);

			advisor.OnInserted(value, index);
			return true;
		}

		private bool RollbackInsert()
		{
			cursor.Remove();
			return false;
		}

		public void Remove(int index)
		{
			var item = items[index];
			OnRemoving(item);

			cursor.MoveTo(item.Node);
			cursor.Remove();
			items.RemoveAt(index);

			advisor.OnRemoved(item.Value, index);
		}

		public void Clear()
		{
			foreach (var item in items)
				OnRemoving(item);

			cursor.Reset();
			cursor.RemoveAllNext();
			items.Clear();

			// Don't call OnRemoved. Caller is already going to fire a Reset shortly.
		}

		public void ClearReferences()
		{
			if (accessor.IsReference)
				foreach (var item in items)
					references.OnAssigningNull(item.Node, item.Value);
		}

		private void OnRemoving(XmlCollectionItem<T> item)
		{
			advisor.OnRemoving(item.Value);

			if (accessor.IsReference)
				references.OnAssigningNull(item.Node, item.Value);
		}

		private T GetValue(IXmlNode node)
		{
			return (T) (accessor.GetValue(node, parentObject, references, true, true) ?? default(T));
		}

		private void SetValue(IXmlCursor cursor, object oldValue, ref T value)
		{
			object obj = value;
			accessor.SetValue(cursor, parentObject, references, true, oldValue, ref obj);
			value = (T) (obj ?? default(T));
		}

		private static Type GetTypeOrDefault(T value)
		{
			return (null == value)
				? typeof(T)
				: value.GetComponentType();
		}

		public IEqualityComparer<T> Comparer
		{
			get { return null; }
		}

		public bool HasSnapshot
		{
			get { return snapshot != null; }
		}

		public int SnapshotCount
		{
			get { return snapshot.Count; }
		}

		public T GetCurrentItem(int index)
		{
			return items[index].Value;
		}

		public T GetSnapshotItem(int index)
		{
			return snapshot[index].Value;
		}

		public void SaveSnapshot()
		{
			snapshot = new List<XmlCollectionItem<T>>(items);
		}

		public void LoadSnapshot()
		{
			items = snapshot;
		}

		public void DropSnapshot()
		{
			snapshot = null;
		}
	}
}
#endif
