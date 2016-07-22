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

#if FEATURE_DICTIONARYADAPTER_XML
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Collections.Generic;
	using System.Xml;
	using System.Xml.Serialization;
	using System.Xml.XPath;

	public static class SysXmlExtensions
	{
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

		public static bool IsXsiType(this XmlAttribute attribute)
		{
		    return attribute.LocalName    == Xsi.Type.LocalName
		        && attribute.NamespaceURI == Xsi.NamespaceUri;
		}
	}
}
#endif
