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

namespace Castle.Components.DictionaryAdapter.Xml.Tests
{
	using System;
	using System.Xml;

	using NUnit.Framework;

	[TestFixture]
	public class XPathNodeTestCase : XmlNodeTestCase
	{
		[Test]
		public void Constructor_RequiresNode()
		{
			Assert.Throws<ArgumentNullException>(() =>
				new XPathNode(null, typeof(T), NamespaceSource.Instance));
		}

		[Test]
		public void Constructor_RequiresType()
		{
			Assert.Throws<ArgumentNullException>(() =>
				new XPathNode(new XmlDocument().CreateNavigator(), null, NamespaceSource.Instance));
		}

		protected override IXmlNode NodeForElement(params string[] xml)
		{
			return new XPathNode(Xml(xml).CreateNavigator(), typeof(T), NamespaceSource.Instance);
		}

		protected override IXmlNode NodeForAttribute(params string[] xml)
		{
			return new XPathNode(Xml(xml).Attributes[0].CreateNavigator(), typeof(T), NamespaceSource.Instance);
		}

		protected override IXmlNode NodeForRoot()
		{
			return new XPathNode(new XmlDocument().CreateNavigator(), typeof(T), NamespaceSource.Instance);
		}

		private static XmlElement Xml(params string[] xml)
		{
			var document = new XmlDocument();
			document.LoadXml(string.Concat(xml));
			return document.DocumentElement;
		}
	}
}
