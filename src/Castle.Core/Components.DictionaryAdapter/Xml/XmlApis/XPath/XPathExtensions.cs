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
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if !SL3
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Xml;
	using System.Xml.XPath;

	public static class XPathExtensions
	{
		public static XPathNavigator CreateNavigatorSafe(this IXPathNavigable source)
		{
            if (source == null)
                throw Error.ArgumentNull("source");
			return source.CreateNavigator();
		}

		public static bool HasAttribute(this XPathNavigator node, string localName, string namespaceUri, string value)
		{
			var attributeValue = node.GetAttribute(localName, namespaceUri);
			return attributeValue == value;
		}

		public static string GetAttributeOrNull(this XPathNavigator node, string localName, string namespaceUri)
		{
			if (!node.MoveToAttribute(localName, namespaceUri))
				return null;
			var value = node.Value;
			node.MoveToParent();
			return string.IsNullOrEmpty(value) ? null : value;
		}

		public static void SetAttribute(this XPathNavigator node, string localName, string namespaceUri, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				if (node.MoveToAttribute(localName, namespaceUri))
					node.DeleteSelf();
			}
			else if (node.MoveToAttribute(localName, namespaceUri))
			{
				node.SetValue(value);
				node.MoveToParent();
			}
			else
			{
				var prefix = node.LookupPrefix(namespaceUri);
				node.CreateAttribute(prefix, localName, namespaceUri, value);
			}
		}

		public static bool MoveToLastChild(this XPathNavigator navigator)
		{
			if (!navigator.MoveToFirstChild())
				return false;

			while (navigator.MoveToNext()) { }

			return true;
		}

		public static bool MoveToLastAttribute(this XPathNavigator navigator)
		{
			if (!navigator.MoveToFirstAttribute())
				return false;

			while (navigator.MoveToNextAttribute()) { }

			return true;
		}

		public static void DeleteChildren(this XPathNavigator node)
		{
			while (node.MoveToFirstChild())
				node.DeleteSelf();
			while (node.MoveToFirstAttribute())
				node.DeleteSelf();
		}
	}
}
#endif
