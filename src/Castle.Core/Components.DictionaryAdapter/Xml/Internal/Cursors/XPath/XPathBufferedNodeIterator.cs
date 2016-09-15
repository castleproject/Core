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

#if FEATURE_DICTIONARYADAPTER_XML
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Collections.Generic;
	using System.Xml.XPath;

	internal class XPathBufferedNodeIterator : XPathNodeIterator
	{
		private readonly IList<XPathNavigator> items;
		private int index;

		public XPathBufferedNodeIterator(XPathNodeIterator iterator)
		{
			items = new List<XPathNavigator>();
			do items.Add(iterator.Current.Clone());
			while (iterator.MoveNext());
		}

		private XPathBufferedNodeIterator(XPathBufferedNodeIterator iterator)
		{
			items = iterator.items;
			index = iterator.index;
		}

		public override int CurrentPosition
		{
			get { return index; }
		}

		public override int Count
		{
			get { return items.Count - 1; }
		}

		public bool IsEmpty
		{
			get { return items.Count == 1; }
		}

		public override XPathNavigator Current
		{
			get { return items[index]; }
		}

		public void Reset()
		{
			index = 0;
		}

		public override bool MoveNext()
		{
			if (++index < items.Count)
				return true;
			if (index > items.Count)
				index--;
			return false;
		}

		public void MoveToEnd()
		{
			index = items.Count;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathBufferedNodeIterator(this);
		}
	}
}
#endif
