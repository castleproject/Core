using System;
using System.Collections.Generic;

namespace Castle.Components.DictionaryAdapter.Xml
{
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
