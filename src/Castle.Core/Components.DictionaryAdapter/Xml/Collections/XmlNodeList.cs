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

#if !SILVERLIGHT
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	internal class XmlNodeList<T> : IList<T>,
		IConfigurable<XmlTypedNode>
	{
		private readonly List<XmlCollectionItem> items;
		private readonly IXmlCollectionAccessor accessor;
		private readonly IDictionaryAdapter parentObject;
		private XmlIterator iterator;
		private XmlTypedNode parentNode;

		public XmlNodeList(
			XmlIterator iterator,
			IDictionaryAdapter parentObject,
			IXmlCollectionAccessor accessor)
		{
			items = new List<XmlCollectionItem>();

			this.accessor     = accessor;
			this.iterator     = iterator;
			this.parentObject = parentObject;
		}

		public XmlNodeList(
			XmlTypedNode parentNode,
			IDictionaryAdapter parentObject,
			IXmlCollectionAccessor accessor)
		{
			items = new List<XmlCollectionItem>();

			this.accessor     = accessor;
			this.parentNode   = parentNode;
			this.parentObject = parentObject;

			accessor.GetCollectionItems(parentNode.Node, parentObject, this);
		}

		public void Configure(XmlTypedNode node)
		{
			items.Add(new XmlCollectionItem(node));
		}

		private XmlTypedNode ParentNode
		{
			get
			{
				if (parentNode.Node == null)
				{
					iterator.Create(typeof(IList<T>));
					parentNode = iterator.Current;
				}
				return parentNode;
			}
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
					items[index] = item = item.WithValue(
						accessor.Serializer.GetValue(item.Node, parentObject, accessor));
				return (T) item.Value;
			}
			set
			{
				var item = items[index];
				accessor.Serializer.SetValue(item.Node, accessor, value);
				items[index] = item = item.WithValue(value);
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
			var node = accessor.AddCollectionItem(ParentNode, parentObject, value);
			items.Add(new XmlCollectionItem(node, value));

			//var node = nodes[Count - 1];
			//var nav = node.Node.Clone();
			//nav.InsertElementAfter(null, "", "", null); // TODO
			//nav.MoveToNext();
			//node = new XmlCollectionItem(new XmlTypedNode(nav, item.GetType()));
		}

		public void Insert(int index, T value)
		{
			if (index == Count)
				Add(value);
			else
			{
				var node = accessor.InsertCollectionItem(items[index].Node, parentObject, value);
				items.Insert(index, new XmlCollectionItem(node, value));

				//var node = nodes[index];
				//var xpn = node.Node.Clone();
				//var kt = (null != item)
				//    ? accessor.GetXmlKnownType(item.GetType())
				//    : accessor;
				//xpn.InsertElementBefore(null, kt.LocalName, kt.NamespaceUri, null); // TODO
				//xpn.MoveToPrevious();
				//node = new XmlCollectionItem(new XmlTypedNode(xpn, item.GetType()));
			}
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
			accessor.RemoveCollectionItem(items[index].Node);
			items.RemoveAt(index);
//			node.Node.DeleteSelf();
		}

		public void Clear()
		{
			if (parentNode.Node != null)
				accessor.RemoveAllCollectionItems(parentNode);
			//for (var i = 0; i < Count; i++)
			//    nodes[i].Node.DeleteSelf();
			items.Clear();
		}
	}
}
#endif
