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

	internal class XmlNodeList<T> : IList<T>,
		IConfigurable<IXmlNode>
	{
		private readonly List<XmlCollectionItem> items;
		private readonly IXmlCursor cursor;
		private readonly IXmlCollectionAccessor accessor;
		private readonly IDictionaryAdapter parentObject;

		public XmlNodeList(
			IXmlNode node,
			IDictionaryAdapter parentObject,
			IXmlCollectionAccessor accessor)
		{
			items = new List<XmlCollectionItem>();

			this.cursor       = accessor.SelectCollectionItems(node, true);
			this.accessor     = accessor;
			this.parentObject = parentObject;

			while (cursor.MoveNext())
				items.Add(new XmlCollectionItem(cursor.Save()));
		}

		public void Configure(IXmlNode node)
		{
			items.Add(new XmlCollectionItem(node));
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public int Count
		{
			get { return items.Count; }
		}

		public T this[int index]
		{
			get
			{
				var item = items[index];
				if (!item.HasValue)
					items[index] = item = item.WithValue(GetValue(item.Node));
				return (T) item.Value;
			}
			set
			{
				var item = items[index];
				SetValue(item.Node, value);
				items[index] = item.WithValue(value);
			}
		}

		public bool Contains(T item)
		{
			return IndexOf(item) >= 0;
		}

		public int IndexOf(T item)
		{
			var comparer = EqualityComparer<T>.Default;

			for (var i = 0; i < Count; i++)
				if (comparer.Equals(this[i], item))
					return i;

			return -1;
		}

		public void CopyTo(T[] array, int index)
		{
			for (int i = 0, j = index; i < Count; i++, j++)
				array[j] = this[i];
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

		public void Add(T value)
		{
			cursor.MoveToEnd();
			items.Add(Create(value));
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

		private XmlCollectionItem Create(T value)
		{
			var type = (null == value)
				? typeof(T)
				: value.GetType();

			cursor.Create(type);
			var item = new XmlCollectionItem(cursor.Save(), value);
			SetValue(item.Node, value);
			return item;
		}

		public bool Remove(T item)
		{
			var index = IndexOf(item);
			if (index < 0) return false;
			RemoveAt(index);
			return true;
		}

		public void RemoveAt(int index)
		{
			cursor.MoveTo(items[index].Node);
			cursor.Remove();
			items.RemoveAt(index);
		}

		public void Clear()
		{
			cursor.Reset();
			cursor.RemoveToEnd();
			items.Clear();
		}

		private object GetValue(IXmlNode node)
		{
			return accessor.Serializer.GetValue(node, parentObject, accessor);
		}

		private void SetValue(IXmlNode node, object value)
		{
			accessor.Serializer.SetValue(node, accessor, value);
		}
	}
}
