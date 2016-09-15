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
	using System.Xml;

	public class SysXmlSubtreeIterator : SysXmlNode, IXmlIterator
	{
		private State state;

		public SysXmlSubtreeIterator(IXmlNode parent, IXmlNamespaceSource namespaces)
			: base(namespaces, parent)
		{
			if (null == parent)
				throw Error.ArgumentNull("parent");

			var source = parent.RequireRealizable<XmlNode>();
			if (source.IsReal)
				node = source.Value;

			type = typeof(object);
		}

		public bool MoveNext()
		{
			switch (state)
			{
				case State.Initial: return MoveToInitial();
				case State.Current: return MoveToSubsequent();
				default:            return false;
			}
		}

		private bool MoveToInitial()
		{
			if (node == null)
				return false;

			state = State.Current;
			return true;
		}

		private bool MoveToSubsequent()
		{
			if (MoveToElement(node.FirstChild))
				return true;

			for (; node != null; node = node.ParentNode)
				if (MoveToElement(node.NextSibling))
					return true;

			state = State.End;
			return false;
		}

		private bool MoveToElement(XmlNode node)
		{
			for (; node != null; node = node.NextSibling)
				if (node.NodeType == XmlNodeType.Element)
					return SetNext(node);

			return false;
		}

		private bool SetNext(XmlNode node)
		{
			this.node = node;
			return true;
		}

		public override IXmlNode Save()
		{
			return new SysXmlNode(node, type, Namespaces);
		}

		private enum State
		{
			Initial,
			Current,
			End
		}
	}
}
#endif
