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

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
#if !SILVERLIGHT
	using System.Xml;
#endif
#if !SL3
	using System.Xml.XPath;
#endif

	public static class Xsi
	{
#if !SILVERLIGHT		
		public static XmlName GetXsiType(this XmlNode node)
		{
			var type = node.GetAttribute(TypeLocalName, NamespaceUri);
			if (type == null)
				return XmlName.Empty;

			var xsiType = XmlName.ParseQName(type);
			if (xsiType.NamespaceUri != null)
			{
				var namespaceUri = node.GetNamespaceOfPrefix(xsiType.NamespaceUri);
				xsiType = xsiType.WithNamespaceUri(namespaceUri);
			}
			return xsiType;
		}

		public static void SetXsiType(this XmlNode node, XmlName xsiType)
		{
			if (xsiType.NamespaceUri != null)
			{
				var prefix = node.GetPrefixOfNamespace(xsiType.NamespaceUri);
				xsiType = xsiType.WithNamespaceUri(prefix);
			}
			node.SetAttribute(TypeLocalName, NamespaceUri, xsiType.ToString());
		}

		public static bool HasXsiType(this XmlNode node, string type)
		{
			return node.HasAttribute(TypeLocalName, NamespaceUri, type);
		}

		public static void SetXsiNil(this XmlNode node, bool nil)
		{
			if (nil) node.RemoveAll();
			node.SetAttribute(NilLocalName, NamespaceUri, nil ? NilValue : null);
		}

		public static bool IsXsiNil(this XmlNode node)
		{
			return node.HasAttribute(NilLocalName, NamespaceUri, NilValue);
		}

		public static bool IsXsiType(this XmlAttribute attribute)
		{
			return attribute.LocalName    == TypeLocalName
				&& attribute.NamespaceURI == NamespaceUri;
		}
#endif
#if !SL3
		public static XmlName GetXsiType(this XPathNavigator node)
		{
			var text = node.GetAttributeOrNull(TypeLocalName, NamespaceUri);
			if (text == null)
				return XmlName.Empty;

			var xsiType = XmlName.ParseQName(text);
			if (xsiType.NamespaceUri == null)
				return xsiType;

			var namespaceUri = node.LookupNamespace(xsiType.NamespaceUri);
			return xsiType.WithNamespaceUri(namespaceUri);
		}

		public static void SetXsiType(this XPathNavigator node, XmlName xsiType)
		{
			var prefix = node.LookupPrefix(xsiType.NamespaceUri);
			xsiType = xsiType.WithNamespaceUri(prefix);

			node.SetAttribute(TypeLocalName, NamespaceUri, xsiType.ToString());
		}

		public static bool IsXsiNil(this XPathNavigator node)
		{
			return node.HasAttribute(NilLocalName, NamespaceUri, NilValue);
		}

		public static void SetXsiNil(this XPathNavigator node, bool nil)
		{
			if (nil) node.DeleteChildren();
			node.SetAttribute(NilLocalName, NamespaceUri, nil ? NilValue : null);
		}
#endif

		public const string
			Prefix         = "xsi",
			TypeLocalName  = "type",
			NilLocalName   = "nil",
			NilValue       = "true",
			NamespaceUri   = "http://www.w3.org/2001/XMLSchema-instance";

		public static readonly XmlNamespaceAttribute
			Attribute = new XmlNamespaceAttribute(NamespaceUri, Prefix) { Root = true };
	}
}
