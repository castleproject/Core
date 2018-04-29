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

	using Castle.Components.DictionaryAdapter.Tests;

	using NUnit.Framework;

	[TestFixture]
	public abstract class XmlNodeTestCase
	{
		[Test]
		public void ClrType()
		{
			var node = NodeForElement("<X/>");

			Assert.AreEqual(typeof(T), node.ClrType);
		}

		[Test]
		public void LocalName_WhenNotQualified()
		{
			var node = NodeForElement("<X/>");

			Assert.AreEqual("X", node.Name.LocalName);
		}

		[Test]
		public void LocalName_WhenQualified()
		{
			var node = NodeForElement("<a:X xmlns:a='urn:a'/>");

			Assert.AreEqual("X", node.Name.LocalName);
		}

		[Test]
		public void NamespaceUri_WhenNoNamespace()
		{
			var node = NodeForElement("<X/>");

			Assert.IsEmpty(node.Name.NamespaceUri);
		}

		[Test]
		public void NamespaceUri_WhenDefaultNamespace()
		{
			var node = NodeForElement("<X xmlns='urn:a'/>");

			Assert.AreEqual("urn:a", node.Name.NamespaceUri);
		}

		[Test]
		public void NamespaceUri_WhenExplicitNamespace()
		{
			var node = NodeForElement("<a:X xmlns:a='urn:a'/>");

			Assert.AreEqual("urn:a", node.Name.NamespaceUri);
		}

		[Test]
		public void XsiType_OfElement_WhenXsiTypeAttributeIsNotPresent()
		{
			var node = NodeForElement("<X/>");

			Assert.AreEqual(XmlName.Empty, node.XsiType);
		}

		[Test]
		public void XsiType_OfElement_WhenXsiTypeAttributeIsPresent()
		{
			var node = NodeForElement("<X xsi:type='p:T' xmlns:p='urn:a' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'/>");

			Assert.AreEqual(new XmlName("T", "urn:a"), node.XsiType);
		}

		[Test]
		public void XsiType_OfAttribute()
		{
			var node = NodeForAttribute("<X A='a'/>");

			Assert.AreEqual(XmlName.Empty, node.XsiType);
		}

		[Test]
		public void IsTypeProperties_OfElement()
		{
			var node = NodeForElement("<X/>");

			Assert.True(node.IsElement);
			Assert.False(node.IsAttribute);
		}

		[Test]
		public void IsTypeProperties_OfAttribute()
		{
			var node = NodeForAttribute("<X A='a'/>");

			Assert.False(node.IsElement);
			Assert.True(node.IsAttribute);
		}

		[Test]
		public void IsTypeProperties_OfRoot()
		{
			var node = NodeForRoot();

			Assert.False(node.IsElement);
			Assert.False(node.IsAttribute);
		}

		[Test]
		public void IsNil_OfElement_WhenXsiNilAttributeIsNotPresent()
		{
			var node = NodeForElement("<X/>");

			Assert.False(node.IsNil);
		}

		[Test]
		public void IsNil_OfElement_WhenXsiNilAttributeIsPresent()
		{
			var node = NodeForElement("<X xsi:nil='true' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'/>");

			Assert.True(node.IsNil);
		}

		[Test]
		public void IsNil_OfElement_WhenSetToTrue()
		{
			var node = NodeForElement("<X> <Y/> </X>");

			node.IsNil = true;

			CustomAssert.AreXmlEquivalent("<X xsi:nil='true' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'/>", node.Xml);
		}

		[Test]
		public void IsNil_OfElement_WhenSetToFalse()
		{
			var node = NodeForElement("<X> <Y/> </X>");

			node.IsNil = false;

			CustomAssert.AreXmlEquivalent("<X> <Y/> </X>", node.Xml);
		}

		[Test]
		public void IsNil_OfElement_WhenValueSet()
		{
			var node = NodeForElement("<X xsi:nil='true' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'/>");

			node.Value = "V";

			CustomAssert.AreXmlEquivalent("<X>V</X>", node.Xml);
		}

		[Test]
		public void IsNil_OfAttribute()
		{
			var node = NodeForAttribute("<X A='a'/>");

			Assert.False(node.IsNil);
		}

		//[Test]
		//public void IsNil_Attribute_WhenSetToTrue()
		//{
		//    var node = NodeForAttribute("<X A='a'/>");

		//    Assert.Throws<InvalidOperationException>(() =>
		//        node.IsNil = true);

		//    Assert.That(node.Xml, Is.EqualTo("A='a'") | Is.EqualTo("A=\"a\""));
		//}

		[Test]
		public void Value_OfElement_WhenEmpty()
		{
			var node = NodeForElement("<X/>");

			Assert.IsEmpty(node.Value);
		}

		[Test]
		public void Value_OfElement_WithContent()
		{
			var node = NodeForElement("<X>a</X>");

			Assert.AreEqual("a", node.Value);
		}

		[Test]
		public void Value_OfElement_WhenSet()
		{
			var node = NodeForElement("<X/>");

			node.Value = "a";

			CustomAssert.AreXmlEquivalent("<X>a</X>", node.Xml);
		}

		[Test]
		public void Value_OfAttribute()
		{
			var node = NodeForAttribute("<X A='a'/>");

			Assert.AreEqual("a", node.Value);
		}

		[Test]
		public void Value_OfAttribute_WhenSet()
		{
			var node = NodeForAttribute("<X A=''/>");

			node.Value = "a";

			Assert.AreEqual("A=\"a\"", node.Xml);
		}

		[Test]
		public void SelectSelf()
		{
			var node = NodeForElement("<X/>");

			var cursor = node.SelectSelf(typeof(T));

			Assert.NotNull(cursor);
			Assert.True(cursor.MoveNext());
			Assert.AreEqual("X", cursor.Name.LocalName);
			Assert.False(cursor.MoveNext());
		}

		[Test]
		public void SelectChildren()
		{
			var node = NodeForElement("<X> <A/> </X>");

			var cursor = node.SelectChildren(KnownTypes, NamespaceSource.Instance, CursorFlags.Elements);

			Assert.NotNull(cursor);
			Assert.True(cursor.MoveNext());
			Assert.AreEqual("A", cursor.Name.LocalName);
			Assert.False(cursor.MoveNext());
		}

		[Test]
		public void Select()
		{
			var node = NodeForElement("<X> <A/> </X>");
			var path = XPathCompiler.Compile("A");

			var cursor = node.Select(path, IncludedTypes, NamespaceSource.Instance, CursorFlags.Elements);

			Assert.NotNull(cursor);
			Assert.True(cursor.MoveNext());
			Assert.AreEqual("A", cursor.Name.LocalName);
			Assert.False(cursor.MoveNext());
		}

		[Test]
		public void Evaluate()
		{
			var node = NodeForElement("<X> <A/> </X>");
			var path = XPathCompiler.Compile("count(A)");

			var value = node.Evaluate(path);

			Assert.AreEqual(1.0, value);
		}

		[Test]
		public void ReadSubtree()
		{
			var node = NodeForElement("<X> <A/> </X>");

			using (var reader = node.ReadSubtree())
			{
				Assert.True(reader.Read());
				Assert.AreEqual(XmlNodeType.Element, reader.NodeType);
				Assert.AreEqual("X", reader.LocalName);

				Assert.True(reader.Read());
				Assert.AreEqual(XmlNodeType.Element, reader.NodeType);
				Assert.AreEqual("A", reader.LocalName);

				Assert.True(reader.Read());
				Assert.AreEqual(XmlNodeType.EndElement, reader.NodeType);
				Assert.AreEqual("X", reader.LocalName);

				Assert.False(reader.Read());
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

			CustomAssert.AreXmlEquivalent("<X> <A/> </X>", node.Xml);
		}

		[Test]
		public void WriteAttributes()
		{
			var node = NodeForElement("<X/>");

			using (var writer = node.WriteAttributes())
			{
				writer.WriteAttributeString("A", "a");
			}

			CustomAssert.AreXmlEquivalent("<X A='a'/>", node.Xml);
		}

		protected abstract IXmlNode NodeForElement  (params string[] xml);
		protected abstract IXmlNode NodeForAttribute(params string[] xml);
		protected abstract IXmlNode NodeForRoot();

		public void OneTimeSetUp()
		{
			if (KnownTypes == null)
			{
				KnownTypes = new XmlKnownTypeSet(typeof(T));
				KnownTypes.Add(new XmlKnownType("A", null, null, null, typeof(T)), true);
			}

			if (IncludedTypes == null)
			{
				IncludedTypes = new MockXmlIncludedTypeMap();
				IncludedTypes.DefaultClrType = typeof(T);
				IncludedTypes.InnerSet.Add(new XmlIncludedType("T", string.Empty, typeof(T)));
			}
		}

		protected XmlNodeTestCase()
		{
			OneTimeSetUp();
		}

		protected static XmlKnownTypeSet        KnownTypes;
		protected static MockXmlIncludedTypeMap IncludedTypes;
		protected sealed class T { }
	}
}
