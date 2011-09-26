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

namespace CastleTests.Components.DictionaryAdapter.Tests.Xml
{
	using System;
	using System.Xml;
	using Castle.Components.DictionaryAdapter.Tests;
	using Castle.Components.DictionaryAdapter.Xml;
	using NUnit.Framework;
	using Castle.Components.DictionaryAdapter;

	[TestFixture]
	public abstract class XmlNodeTestCase
	{
		[Test]
		public void ClrType()
		{
			var node = NodeForElement("<X/>");

			Assert.That(node.ClrType, Is.EqualTo(typeof(T)));
		}

		[Test]
		public void LocalName_WhenNotQualified()
		{
			var node = NodeForElement("<X/>");

			Assert.That(node.Name.LocalName, Is.EqualTo("X"));
		}

		[Test]
		public void LocalName_WhenQualified()
		{
			var node = NodeForElement("<a:X xmlns:a='urn:a'/>");

			Assert.That(node.Name.LocalName, Is.EqualTo("X"));
		}

		[Test]
		public void NamespaceUri_WhenNoNamespace()
		{
			var node = NodeForElement("<X/>");

			Assert.That(node.Name.NamespaceUri, Is.Empty);
		}

		[Test]
		public void NamespaceUri_WhenDefaultNamespace()
		{
			var node = NodeForElement("<X xmlns='urn:a'/>");

			Assert.That(node.Name.NamespaceUri, Is.EqualTo("urn:a"));
		}

		[Test]
		public void NamespaceUri_WhenExplicitNamespace()
		{
			var node = NodeForElement("<a:X xmlns:a='urn:a'/>");

			Assert.That(node.Name.NamespaceUri, Is.EqualTo("urn:a"));
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

			Assert.That(node.Xml, XmlEquivalent.To("<X xsi:nil='true' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'/>"));
		}

		[Test]
		public void IsNil_OfElement_WhenSetToFalse()
		{
			var node = NodeForElement("<X> <Y/> </X>");

			node.IsNil = false;

			Assert.That(node.Xml, XmlEquivalent.To("<X> <Y/> </X>"));
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

			Assert.That(node.Xml, Is.EqualTo("A='a'") | Is.EqualTo("A=\"a\""));
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

			Assert.That(node.Xml, XmlEquivalent.To("<X>a</X>"));
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

			Assert.That(node.Xml, Is.EqualTo("A='a'") | Is.EqualTo("A=\"a\""));
		}

		[Test]
		public void SelectSelf()
		{
			var node = NodeForElement("<X/>");

			var cursor = node.SelectSelf(typeof(T));

			Assert.That(cursor,            Is.Not.Null);
			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.Name.LocalName,  Is.EqualTo("X"));
			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Test]
		public void SelectChildren()
		{
			var node = NodeForElement("<X> <A/> </X>");

			var cursor = node.SelectChildren(KnownTypes, CursorFlags.Elements);

			Assert.That(cursor,            Is.Not.Null);
			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.Name.LocalName,  Is.EqualTo("A"));
			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Test]
		public void Select()
		{
			var node = NodeForElement("<X> <A/> </X>");
			var path = XPathCompiler.Compile("A");

			var cursor = node.Select(path, IncludedTypes, CursorFlags.Elements);

			Assert.That(cursor,            Is.Not.Null);
			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.Name.LocalName,  Is.EqualTo("A"));
			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Test]
		public void Evaluate()
		{
			var node = NodeForElement("<X> <A/> </X>");
			var path = XPathCompiler.Compile("count(A)");

			var value = node.Evaluate(path);

			Assert.That(value, Is.EqualTo(1));
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
		public void WriteChildren()
		{
			var node = NodeForElement("<X/>");

			using (var writer = node.WriteChildren())
			{
				writer.WriteStartElement("A");
			}

			Assert.That(node.Xml, XmlEquivalent.To("<X> <A/> </X>"));
		}

		[Test]
		public void WriteAttributes()
		{
			var node = NodeForElement("<X/>");

			using (var writer = node.WriteAttributes())
			{
				writer.WriteAttributeString("A", "a");
			}

			Assert.That(node.Xml, XmlEquivalent.To("<X A='a'/>"));
		}

		protected abstract IXmlNode NodeForElement  (params string[] xml);
		protected abstract IXmlNode NodeForAttribute(params string[] xml);
		protected abstract IXmlNode NodeForRoot();

		[TestFixtureSetUp]
		public void OneTimeSetUp()
		{
			if (KnownTypes == null)
			{
				KnownTypes = new XmlKnownTypeSet(typeof(T));
				KnownTypes.Add(new XmlKnownType("A", null, null, null, typeof(T)));
			}

			if (IncludedTypes == null)
			{
				IncludedTypes = new XmlIncludedTypeSet();
			}
		}

		protected static XmlKnownTypeSet    KnownTypes;
		protected static XmlIncludedTypeSet IncludedTypes;
		protected sealed class T { }
	}
}
