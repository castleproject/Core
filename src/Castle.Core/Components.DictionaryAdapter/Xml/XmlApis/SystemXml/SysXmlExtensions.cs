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

#if !SILVERLIGHT
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Collections.Generic;
	using System.Xml;
	using System.Xml.Serialization;
#if !SL3
	using System.Xml.XPath;
#endif

	public static class SysXmlExtensions
	{
		public static string GetAttribute(this XmlNode node, string localName, string namespaceUri)
		{
			var attribute = node.Attributes[localName, namespaceUri];
			if (attribute == null)
				return null;

			var value = attribute.Value;
			if (string.IsNullOrEmpty(value))
				return null;

			return value;
		}

		public static void SetAttribute(this XmlNode node, string localName, string namespaceUri, string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				node.Attributes.RemoveNamedItem(localName, namespaceUri);
			}
			else
			{
				var attribute = node.Attributes[localName, namespaceUri];
				if (attribute == null)
				{
					attribute = node.OwnerDocument.CreateAttribute(null, localName, namespaceUri);
					node.Attributes.Append(attribute);
				}
				attribute.Value = value;
			}
		}

		public static bool HasAttribute(this XmlNode node, string localName, string namespaceUri, string value)
		{
			var attribute = node.Attributes[localName, namespaceUri];
			return attribute != null
				&& attribute.Value == value;
		}

		public static void DefineNamespace(this XmlElement node, string prefix, string namespaceUri)
		{
			var attribute = node.OwnerDocument.CreateAttribute(Xmlns.Prefix, prefix, Xmlns.NamespaceUri);
			attribute.Value = namespaceUri;
			node.SetAttributeNode(attribute);
		}

		public static bool IsNamespace(this XmlAttribute attribute)
		{
			return attribute.Prefix == Xmlns.Prefix ||
			(
				string.IsNullOrEmpty(attribute.Prefix) &&
				attribute.LocalName == Xmlns.Prefix
			);
		}

		public static XmlElement FindRoot(this XmlElement node)
		{
			for (;;)
			{
				var next = node.ParentNode as XmlElement;
				if (next == null) return node;
				node = next;
			}
		}
	}
}
#endif
