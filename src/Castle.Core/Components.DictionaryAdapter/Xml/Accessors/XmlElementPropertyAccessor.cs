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
	using System.Xml.XPath;

	public class XmlElementPropertyAccessor : XmlPropertyAccessor
	{
		private readonly string localName;
		private readonly string namespaceUri;

		public XmlElementPropertyAccessor(Type type, string localName, string namespaceUri)
			: base(type)
		{
			this.localName    = localName;
			this.namespaceUri = namespaceUri;
		}

		public string LocalName
		{
			get { return localName; }
		}

		public string NamespaceUri
		{
			get { return namespaceUri; }
		}

		protected override Iterator<XPathNavigator> SelectPropertyNode(XPathNavigator node, bool create)
		{
			return new XmlElementIterator(node, localName, namespaceUri, false);
		}

		protected override Iterator<XPathNavigator> SelectCollectionNode(XPathNavigator node, bool create)
		{
			return new SingleIterator<XPathNavigator>(node);
		}

		protected override Iterator<XPathNavigator> SelectCollectionItems(XPathNavigator node, bool create)
		{
			return new XmlElementIterator(node, localName, namespaceUri, true);
		}
	}
}
#endif
