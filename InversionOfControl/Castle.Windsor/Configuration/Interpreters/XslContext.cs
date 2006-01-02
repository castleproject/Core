// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.Windsor.Configuration.Interpreters
{
	using System;
	using System.Collections;
	using System.Diagnostics;
	using System.Reflection;
	using System.Xml.XPath;

	public class XslContext
	{
		private Hashtable flags;

		public XslContext()
		{
			flags = new Hashtable();
		}

		public void Add(String key)
		{
			Debug.Assert(key != null);

			flags.Add(GetCanonicalKey(key), true);
		}

		public bool Contains(String key)
		{
			if (key == null) return false;

			return flags.Contains(GetCanonicalKey(key));
		}

		public void Remove(String key)
		{
			if (key != null)
			{
				flags.Remove(GetCanonicalKey(key));
			}
		}

		public object ProcessChoose(XPathNodeIterator nodeList)
		{
			bool isValidNode = false;

			while(nodeList.MoveNext())
			{
				if (nodeList.Current.Name == "when")
				{
					String key = nodeList.Current.GetAttribute("defined", "");

					if (Contains(key))
					{
						isValidNode = true;
					}
				}
				else if (nodeList.Current.Name == "otherwise")
				{
					isValidNode = true;
				}

				if (isValidNode) return GetNodeList(new object[] {nodeList.Current});
			}

			return null;
		}

		private string GetCanonicalKey(string key)
		{
			return key.Trim().ToUpper();
		}

		private XPathNodeIterator GetNodeList(IList nodes)
		{
			return new XPathNodeIteratorAdapter(nodes);
		}
	}

	public class XPathNodeIteratorAdapter : XPathNodeIterator
	{
		private readonly IList nodes;
		private int currentIndex;

		public XPathNodeIteratorAdapter(IList nodes)
		{
			this.nodes = nodes;
		}

		protected XPathNodeIteratorAdapter(IList nodes, int index)
		{
			this.nodes = nodes;
			this.currentIndex = index;
		}

		public override XPathNodeIterator Clone()
		{
			return new XPathNodeIteratorAdapter(nodes, currentIndex);
		}

		public override bool MoveNext()
		{
			if (currentIndex == nodes.Count) return false;

			currentIndex++;

			return true;
		}

		public override XPathNavigator Current
		{
			get { return (XPathNavigator) nodes[currentIndex - 1]; }
		}

		public override int CurrentPosition
		{
			get { return currentIndex; }
		}

		public override int Count
		{
			get { return nodes.Count; }
		}
	}
}