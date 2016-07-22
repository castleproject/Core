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

	public static class XRef
	{
		public static string GetId(this IXmlNode node)
		{
			return node.GetAttribute(XRef.Id);
		}

		public static void SetId(this IXmlCursor node, string id)
		{
			node.SetAttribute(XRef.Id, id);
		}

		public static string GetReference(this IXmlNode node)
		{
			return node.GetAttribute(XRef.Ref);
		}

		public static void SetReference(this IXmlCursor cursor, string id)
		{
			cursor.SetAttribute(XRef.Ref, id);
		}

		public const string
			Prefix       = "x",
			NamespaceUri = "urn:schemas-castle-org:xml-reference";

		public static readonly XmlName
			Id  = new XmlName("id",  NamespaceUri),
			Ref = new XmlName("ref", NamespaceUri);

		internal static readonly XmlNamespaceAttribute
			Namespace = new XmlNamespaceAttribute(NamespaceUri, Prefix) { Root = true };
	}
}
#endif
