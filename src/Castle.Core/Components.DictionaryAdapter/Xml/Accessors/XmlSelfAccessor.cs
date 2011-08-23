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

	public class XmlSelfAccessor : XmlPropertyAccessor
	{
		public XmlSelfAccessor(Type type)
			: base(type) { }

		protected override Iterator<XPathNavigator> SelectPropertyNode(XPathNavigator node, bool create)
		{
			return new SingleIterator<XPathNavigator>(node);
		}

		protected override Iterator<XPathNavigator> SelectCollectionNode(XPathNavigator node, bool create)
		{
			return SelectPropertyNode(node, create);
		}

		protected override Iterator<XPathNavigator> SelectCollectionItems(XPathNavigator node, bool create)
		{
			var name = GetLocalName(PropertyType.GetElementType());
			return new XmlIterator(node, name, null, true);
		}
	}
}
#endif
