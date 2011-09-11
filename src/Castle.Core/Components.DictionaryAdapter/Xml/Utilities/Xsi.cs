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
#if !SILVERLIGHT
	using System.Xml;
#endif

	public static class Xsi
	{
#if !SILVERLIGHT
		public static string GetXsiType(this XmlNode node)
		{
			return node.GetAttribute(TypeLocalName, NamespaceUri);
		}

		public static void SetXsiType(this XmlNode node, string type)
		{
			node.SetAttribute(TypeLocalName, NamespaceUri, type);
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

		public const string
			Prefix         = "xsi",
			TypeLocalName  = "type",
			NilLocalName   = "nil",
			NilValue       = "true",
			NamespaceUri   = "http://www.w3.org/2001/XMLSchema-instance";
	}
}
