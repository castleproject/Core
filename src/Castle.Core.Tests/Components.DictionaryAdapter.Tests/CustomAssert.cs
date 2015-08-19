// Copyright 2004-2015 Castle Project - http://www.castleproject.org/
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

#if !SILVERLIGHT // Until support for other platforms is verified
namespace Castle.Components.DictionaryAdapter.Tests
{
	using System;
	using System.Xml;
	using System.Xml.XPath;

	using NUnit.Framework;

	internal static class CustomAssert
	{
		public static void AreXmlEquivalent(XmlElement expected, XmlElement actual)
		{
			if (expected == null) throw new ArgumentNullException("expected");
			if (actual == null) throw new ArgumentNullException("actual");

			Assert.True(Matches(expected, actual));
		}

		public static void AreXmlEquivalent(string expected, XmlElement actual)
		{
			if (expected == null) throw new ArgumentNullException("expected");
			if (actual == null) throw new ArgumentNullException("actual");

			AreXmlEquivalent(GetElement(expected), actual);
		}

		public static void AreXmlEquivalent(string expected, XmlDocument actual)
		{
			if (expected == null) throw new ArgumentNullException("expected");
			if (actual == null) throw new ArgumentNullException("actual");

			AreXmlEquivalent(GetElement(expected), GetElement(actual));
		}

		public static void AreXmlEquivalent(string expected, IXPathNavigable actual)
		{
			if (expected == null) throw new ArgumentNullException("expected");
			if (actual == null) throw new ArgumentNullException("actual");

			AreXmlEquivalent(GetElement(expected), GetElement(actual));
		}

		public static void AreXmlEquivalent(string expected, string actual)
		{
			if (expected == null) throw new ArgumentNullException("expected");
			if (actual == null) throw new ArgumentNullException("actual");

			AreXmlEquivalent(GetElement(expected), GetElement(actual));
		}

		public static void AreXmlEquivalent(XmlDocument expected, XmlDocument actual)
		{
			if (expected == null) throw new ArgumentNullException("expected");
			if (actual == null) throw new ArgumentNullException("actual");

			AreXmlEquivalent(GetElement(expected), GetElement(actual));
		}

		private static bool Matches(XmlElement expected, XmlElement actual)
		{
			return XmlStructureComparer.Default.Equals(expected, actual);
		}

		private static XmlElement GetElement(object obj)
		{
			var element = obj as XmlElement;
			if (element != null)
				return element;

			var document = obj as XmlDocument;
			if (document != null && (element = document.DocumentElement) != null)
				return element;

			var source = obj as IXPathNavigable;
			if (source != null && (element = GetElement(source)) != null)
				return element;

			var xml = obj as string;
			if (xml != null && (element = ParseXml(xml)) != null)
				return element;

			return null;
		}

		private static XmlElement ParseXml(string xml)
		{
			var document = new XmlDocument();
			document.LoadXml(xml);
			return document.DocumentElement;
		}

		private static XmlElement GetElement(IXPathNavigable source)
		{
			var hasNode = source as IHasXmlNode;
			return (null == hasNode ? null : hasNode.GetNode() as XmlElement)
				?? ParseXml(source.CreateNavigator().OuterXml);
		}
	}
}
#endif
