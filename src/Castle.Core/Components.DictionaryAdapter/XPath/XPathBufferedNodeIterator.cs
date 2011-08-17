using System.Collections.Generic;
using System.Xml.XPath;

namespace Castle.Components.DictionaryAdapter
{
	internal class XPathBufferedNodeIterator : XPathNodeIterator
	{
		private readonly IList<XPathNavigator> _items;
		private int _index;

		public XPathBufferedNodeIterator(XPathNodeIterator iterator)
		{
			_items = new List<XPathNavigator>();
			do _items.Add(iterator.Current.Clone());
			while (iterator.MoveNext());
		}

		private XPathBufferedNodeIterator(XPathBufferedNodeIterator iterator)
		{
			_items = iterator._items;
			_index = iterator._index;
		}

		public override int CurrentPosition
		{
			get { return _index; }
		}

		public override int Count
		{
			get { return _items.Count - 1; }
		}

		public override XPathNavigator Current
		{
			get { return _items[_index]; }
		}

		public override bool MoveNext()
		{
			return ++_index < _items.Count;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathBufferedNodeIterator(this);
		}
	}
}
