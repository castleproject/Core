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

namespace Castle.Components.DictionaryAdapter.Tests
{
	using System;
	using System.Xml;
	using System.Xml.XPath;
	using NUnit.Framework.Constraints;

	internal class XmlEquivalent : Constraint
	{
		private readonly XmlElement expected;

		public static XmlEquivalent To(XmlElement element)
		{
			if (element == null)
				throw new ArgumentNullException("element");

			return new XmlEquivalent(element);
		}

		public static XmlEquivalent To(XmlDocument document)
		{
			if (document == null)
				throw new ArgumentNullException("document");

			return XmlEquivalent.To(document.DocumentElement);
		}

		public static XmlEquivalent To(IXPathNavigable source)
		{
			if (source == null)
				throw new ArgumentNullException("source");

			return XmlEquivalent.To(GetElement(source));
		}

		public static XmlEquivalent To(string xml)
		{
			if (xml == null)
				throw new ArgumentNullException("xml");

			return XmlEquivalent.To(ParseXml(xml));
		}

		public static XmlEquivalent To(params string[] xml)
		{
			if (xml == null)
				throw new ArgumentNullException("xml");

			return XmlEquivalent.To(string.Concat(xml));
		}

		protected XmlEquivalent(XmlElement expected)
		{
			this.expected = expected;
		}

		public sealed override bool Matches(object actual)
		{
			this.actual = actual;

			XmlElement element;
			return TryGetElement(actual, out element)
				&& Matches(element);
		}

		protected virtual bool Matches(XmlElement actual)
		{
			return XmlStructureComparer.Default.Equals(expected, actual);
		}

		public override void WriteDescriptionTo(MessageWriter writer)
		{
			writer.WritePredicate("equivalent XML structure to");
			writer.WriteExpectedValue(expected.OuterXml);
		}

		public override void WriteActualValueTo(MessageWriter writer)
		{
			XmlElement element;
			if (TryGetElement(actual, out element))
				writer.WriteActualValue(element.OuterXml);
			else
				base.WriteActualValueTo(writer);
		}

		private static bool TryGetElement(object obj, out XmlElement element)
		{
			element = obj as XmlElement;
			if (element != null)
				return true;

			var document = obj as XmlDocument;
			if (document != null && (element = document.DocumentElement) != null)
				return true;

			var source = obj as IXPathNavigable;
			if (source != null && (element = GetElement(source)) != null)
				return true;

			var xml = obj as string;
			if (xml != null && (element = ParseXml(xml)) != null)
				return true;

			return false;
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
