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

namespace CastleTests.Components.DictionaryAdapter.Tests.Xml.Cursors
{
	using System;
	using System.Xml;
	using System.Xml.Serialization;
	using Castle.Components.DictionaryAdapter.Tests;
	using Castle.Components.DictionaryAdapter.Xml;
	using NUnit.Framework;

	[TestFixture]
	public class SysXmlNodeTestCase
	{
		[Test]
		public void Constructor_RequiresNode()
		{
			Assert.Throws<ArgumentNullException>(() =>
				new SysXmlNode(null, typeof(T)));
		}

		[Test]
		public void ClrType()
		{
			var node = new SysXmlNode(new XmlDocument(), typeof(T));

			Assert.That(node.ClrType, Is.EqualTo(typeof(T)));
		}

		[Test]
		public void LocalName_WhenNotQualified()
		{
			var node = NodeForElement("<X/>");

			Assert.That(node.LocalName, Is.EqualTo("X"));
		}

		[Test]
		public void LocalName_WhenQualified()
		{
			var node = NodeForElement("<a:X xmlns:a='urn:a'/>");

			Assert.That(node.LocalName, Is.EqualTo("X"));
		}

		[Test]
		public void NamespaceUri_WhenNoNamespace()
		{
			var node = NodeForElement("<X/>");

			Assert.That(node.NamespaceUri, Is.Empty);
		}

		[Test]
		public void NamespaceUri_WhenDefaultNamespace()
		{
			var node = NodeForElement("<X xmlns='urn:a'/>");

			Assert.That(node.NamespaceUri, Is.EqualTo("urn:a"));
		}

		[Test]
		public void NamespaceUri_WhenExplicitNamespace()
		{
			var node = NodeForElement("<a:X xmlns:a='urn:a'/>");

			Assert.That(node.NamespaceUri, Is.EqualTo("urn:a"));
		}

		[Test]
		public void XsiType_OfElement_WhenXsiTypeAttributeIsNotPresent()
		{
			var node = NodeForElement("<X/>");

			Assert.That(node.XsiType, Is.Null);
		}

		[Test]
		public void XsiType_OfElement_WhenXsiTypeAttributeIsPresent()
		{
			var node = NodeForElement("<X xsi:type='Q' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'/>");

			Assert.That(node.XsiType, Is.EqualTo("Q"));
		}

		[Test]
		public void XsiType_OfAttribute()
		{
			var node = NodeForAttribute("<X A='a'/>");

			Assert.That(node.XsiType, Is.Null);
		}

		[Test]
		public void IsTypeProperties_OfElement()
		{
			var node = NodeForElement("<X/>");

			Assert.That(node.IsElement,   Is.True);
			Assert.That(node.IsAttribute, Is.False);
			Assert.That(node.IsRoot,      Is.False);
		}

		[Test]
		public void IsTypeProperties_OfAttribute()
		{
			var node = NodeForAttribute("<X A='a'/>");

			Assert.That(node.IsElement,   Is.False);
			Assert.That(node.IsAttribute, Is.True);
			Assert.That(node.IsRoot,      Is.False);
		}

		[Test]
		public void IsTypeProperties_OfRoot()
		{
			var node = NodeForRoot();

			Assert.That(node.IsElement,   Is.False);
			Assert.That(node.IsAttribute, Is.False);
			Assert.That(node.IsRoot,      Is.True);
		}

		[Test]
		public void IsNil_OfElement_WhenXsiNilAttributeIsNotPresent()
		{
			var node = NodeForElement("<X/>");

			Assert.That(node.IsNil, Is.False);
		}

		[Test]
		public void IsNil_OfElement_WhenXsiNilAttributeIsPresent()
		{
			var node = NodeForElement("<X xsi:nil='true' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'/>");

			Assert.That(node.IsNil, Is.True);
		}

		[Test]
		public void IsNil_OfElement_WhenSetToTrue()
		{
			var node = NodeForElement("<X> <Y/> </X>");

			node.IsNil = true;

			Assert.That(node.GetNode(), XmlEquivalent.To("<X xsi:nil='true' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'/>"));
		}

		[Test]
		public void IsNil_OfElement_WhenSetToFalse()
		{
			var node = NodeForElement("<X> <Y/> </X>");

			node.IsNil = false;

			Assert.That(node.GetNode(), XmlEquivalent.To("<X> <Y/> </X>"));
		}

		[Test]
		public void IsNil_OfAttribute()
		{
			var node = NodeForAttribute("<X A='a'/>");

			Assert.That(node.IsNil, Is.False);
		}

		[Test]
		public void IsNil_Attribute_WhenSet()
		{
			var node = NodeForAttribute("<X A='a'/>");

			Assert.Throws<InvalidOperationException>(() =>
				node.IsNil = false);

			var xml = Xml("<X A='a'/>").Attributes[0];
			Assert.That(node.GetNode().OuterXml, Is.EqualTo(xml.OuterXml));
		}

		[Test]
		public void Value_OfElement_WhenEmpty()
		{
			var node = NodeForElement("<X/>");

			Assert.That(node.Value, Is.Empty);
		}

		[Test]
		public void Value_OfElement_WithContent()
		{
			var node = NodeForElement("<X>a</X>");

			Assert.That(node.Value, Is.EqualTo("a"));
		}

		[Test]
		public void Value_OfElement_WhenSet()
		{
			var node = NodeForElement("<X/>");

			node.Value = "a";

			Assert.That(node.GetNode(), XmlEquivalent.To("<X>a</X>"));
		}

		[Test]
		public void Value_OfAttribute()
		{
			var node = NodeForAttribute("<X A='a'/>");

			Assert.That(node.Value, Is.EqualTo("a"));
		}

		[Test]
		public void Value_OfAttribute_WhenSet()
		{
			var node = NodeForAttribute("<X A=''/>");

			node.Value = "a";

			var xml = Xml("<X A='a'/>").Attributes[0];
			Assert.That(node.GetNode().OuterXml, Is.EqualTo(xml.OuterXml));
		}

		[Test]
		public void SelectSelf()
		{
			var node = NodeForElement("<X/>");

			var cursor = node.SelectSelf();

			Assert.That(cursor,            Is.Not.Null & Is.InstanceOf<XmlSelfCursor>());
			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.LocalName,  Is.EqualTo("X"));
			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Test]
		public void SelectChildren()
		{
			var node = NodeForElement("<X> <A/> </X>");
			var knownType = new XmlKnownType("A", null, typeof(T));

			var cursor = node.SelectChildren(knownType, CursorFlags.Elements);

			Assert.That(cursor,            Is.Not.Null & Is.InstanceOf<SysXmlCursor>());
			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.LocalName,  Is.EqualTo("A"));
			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Test]
		public void ReadSubtree()
		{
			var node = NodeForElement("<X> <A/> </X>");

			using (var reader = node.ReadSubtree())
			{
				Assert.That(reader.Read(),    Is.EqualTo(true));
				Assert.That(reader.NodeType,  Is.EqualTo(XmlNodeType.Element));
				Assert.That(reader.LocalName, Is.EqualTo("X"));

				Assert.That(reader.Read(),    Is.EqualTo(true));
				Assert.That(reader.NodeType,  Is.EqualTo(XmlNodeType.Element));
				Assert.That(reader.LocalName, Is.EqualTo("A"));

				Assert.That(reader.Read(),    Is.EqualTo(true));
				Assert.That(reader.NodeType,  Is.EqualTo(XmlNodeType.EndElement));
				Assert.That(reader.LocalName, Is.EqualTo("X"));

				Assert.That(reader.Read(),    Is.EqualTo(false));
			}
		}

		[Test]
		public void WriteSubtree()
		{
			var node = NodeForElement("<X/>");

			using (var writer = node.WriteChildren())
			{
				writer.WriteStartElement("A");
			}

			Assert.That(node.GetNode(), XmlEquivalent.To("<X> <A/> </X>"));
		}

		private static SysXmlNode NodeForElement(params string[] xml)
		{
			return new SysXmlNode(Xml(xml), typeof(T));
		}

		private static SysXmlNode NodeForAttribute(params string[] xml)
		{
			return new SysXmlNode(Xml(xml).Attributes[0], typeof(T));
		}

		private static SysXmlNode NodeForRoot()
		{
			return new SysXmlNode(new XmlDocument(), typeof(T));
		}

		private static XmlElement Xml(params string[] xml)
		{
			var document = new XmlDocument();
			document.LoadXml(string.Concat(xml));
			return document.DocumentElement;
		}

		private sealed class T { }
	}
}
