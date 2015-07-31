﻿// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
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

#if !SILVERLIGHT && !MONO && !NETCORE // Until support for other platforms is verified
namespace CastleTests.Components.DictionaryAdapter.Xml.Tests
{
	using System;
	using System.Xml;
	using Castle.Components.DictionaryAdapter.Tests;
	using Castle.Components.DictionaryAdapter.Xml;
	using Xunit;

		public abstract class XmlNodeTestCase
	{
		[Fact]
		public void ClrType()
		{
			var node = NodeForElement("<X/>");

			Assert.That(node.ClrType, Is.EqualTo(typeof(T)));
		}

		[Fact]
		public void LocalName_WhenNotQualified()
		{
			var node = NodeForElement("<X/>");

			Assert.That(node.Name.LocalName, Is.EqualTo("X"));
		}

		[Fact]
		public void LocalName_WhenQualified()
		{
			var node = NodeForElement("<a:X xmlns:a='urn:a'/>");

			Assert.That(node.Name.LocalName, Is.EqualTo("X"));
		}

		[Fact]
		public void NamespaceUri_WhenNoNamespace()
		{
			var node = NodeForElement("<X/>");

			Assert.That(node.Name.NamespaceUri, Is.Empty);
		}

		[Fact]
		public void NamespaceUri_WhenDefaultNamespace()
		{
			var node = NodeForElement("<X xmlns='urn:a'/>");

			Assert.That(node.Name.NamespaceUri, Is.EqualTo("urn:a"));
		}

		[Fact]
		public void NamespaceUri_WhenExplicitNamespace()
		{
			var node = NodeForElement("<a:X xmlns:a='urn:a'/>");

			Assert.That(node.Name.NamespaceUri, Is.EqualTo("urn:a"));
		}

		[Fact]
		public void XsiType_OfElement_WhenXsiTypeAttributeIsNotPresent()
		{
			var node = NodeForElement("<X/>");

			Assert.That(node.XsiType, Is.EqualTo(XmlName.Empty));
		}

		[Fact]
		public void XsiType_OfElement_WhenXsiTypeAttributeIsPresent()
		{
			var node = NodeForElement("<X xsi:type='p:T' xmlns:p='urn:a' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'/>");

			Assert.That(node.XsiType, Is.EqualTo(new XmlName("T", "urn:a")));
		}

		[Fact]
		public void XsiType_OfAttribute()
		{
			var node = NodeForAttribute("<X A='a'/>");

			Assert.That(node.XsiType, Is.EqualTo(XmlName.Empty));
		}

		[Fact]
		public void IsTypeProperties_OfElement()
		{
			var node = NodeForElement("<X/>");

			Assert.That(node.IsElement,   Is.True);
			Assert.That(node.IsAttribute, Is.False);
		}

		[Fact]
		public void IsTypeProperties_OfAttribute()
		{
			var node = NodeForAttribute("<X A='a'/>");

			Assert.That(node.IsElement,   Is.False);
			Assert.That(node.IsAttribute, Is.True);
		}

		[Fact]
		public void IsTypeProperties_OfRoot()
		{
			var node = NodeForRoot();

			Assert.That(node.IsElement,   Is.False);
			Assert.That(node.IsAttribute, Is.False);
		}

		[Fact]
		public void IsNil_OfElement_WhenXsiNilAttributeIsNotPresent()
		{
			var node = NodeForElement("<X/>");

			Assert.That(node.IsNil, Is.False);
		}

		[Fact]
		public void IsNil_OfElement_WhenXsiNilAttributeIsPresent()
		{
			var node = NodeForElement("<X xsi:nil='true' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'/>");

			Assert.That(node.IsNil, Is.True);
		}

		[Fact]
		public void IsNil_OfElement_WhenSetToTrue()
		{
		    var node = NodeForElement("<X> <Y/> </X>");

		    node.IsNil = true;

		    Assert.That(node.Xml, XmlEquivalent.To("<X xsi:nil='true' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'/>"));
		}

		[Fact]
		public void IsNil_OfElement_WhenSetToFalse()
		{
		    var node = NodeForElement("<X> <Y/> </X>");

		    node.IsNil = false;

		    Assert.That(node.Xml, XmlEquivalent.To("<X> <Y/> </X>"));
		}

		[Fact]
		public void IsNil_OfElement_WhenValueSet()
		{
			var node = NodeForElement("<X xsi:nil='true' xmlns:xsi='http://www.w3.org/2001/XMLSchema-instance'/>");

			node.Value = "V";

			Assert.That(node.Xml, XmlEquivalent.To("<X>V</X>"));
		}

		[Fact]
		public void IsNil_OfAttribute()
		{
			var node = NodeForAttribute("<X A='a'/>");

			Assert.That(node.IsNil, Is.False);
		}

		//[Test]
		//public void IsNil_Attribute_WhenSetToTrue()
		//{
		//    var node = NodeForAttribute("<X A='a'/>");

		//    Assert.Throws<InvalidOperationException>(() =>
		//        node.IsNil = true);

		//    Assert.That(node.Xml, Is.EqualTo("A='a'") | Is.EqualTo("A=\"a\""));
		//}

		[Fact]
		public void Value_OfElement_WhenEmpty()
		{
			var node = NodeForElement("<X/>");

			Assert.That(node.Value, Is.Empty);
		}

		[Fact]
		public void Value_OfElement_WithContent()
		{
			var node = NodeForElement("<X>a</X>");

			Assert.That(node.Value, Is.EqualTo("a"));
		}

		[Fact]
		public void Value_OfElement_WhenSet()
		{
			var node = NodeForElement("<X/>");

			node.Value = "a";

			Assert.That(node.Xml, XmlEquivalent.To("<X>a</X>"));
		}

		[Fact]
		public void Value_OfAttribute()
		{
			var node = NodeForAttribute("<X A='a'/>");

			Assert.That(node.Value, Is.EqualTo("a"));
		}

		[Fact]
		public void Value_OfAttribute_WhenSet()
		{
			var node = NodeForAttribute("<X A=''/>");

			node.Value = "a";

			Assert.That(node.Xml, Is.EqualTo("A='a'") | Is.EqualTo("A=\"a\""));
		}

		[Fact]
		public void SelectSelf()
		{
			var node = NodeForElement("<X/>");

			var cursor = node.SelectSelf(typeof(T));

			Assert.That(cursor,                Is.Not.Null);
			Assert.That(cursor.MoveNext(),     Is.True);
			Assert.That(cursor.Name.LocalName, Is.EqualTo("X"));
			Assert.That(cursor.MoveNext(),     Is.False);
		}

		[Fact]
		public void SelectChildren()
		{
			var node = NodeForElement("<X> <A/> </X>");

			var cursor = node.SelectChildren(KnownTypes, NamespaceSource.Instance, CursorFlags.Elements);

			Assert.That(cursor,            Is.Not.Null);
			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.Name.LocalName,  Is.EqualTo("A"));
			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Select()
		{
			var node = NodeForElement("<X> <A/> </X>");
			var path = XPathCompiler.Compile("A");

			var cursor = node.Select(path, IncludedTypes, NamespaceSource.Instance, CursorFlags.Elements);

			Assert.That(cursor,            Is.Not.Null);
			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.Name.LocalName,  Is.EqualTo("A"));
			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Evaluate()
		{
			var node = NodeForElement("<X> <A/> </X>");
			var path = XPathCompiler.Compile("count(A)");

			var value = node.Evaluate(path);

			Assert.That(value, Is.EqualTo(1));
		}

		[Fact]
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

		[Fact]
		public void WriteChildren()
		{
			var node = NodeForElement("<X/>");

			using (var writer = node.WriteChildren())
			{
				writer.WriteStartElement("A");
			}

			Assert.That(node.Xml, XmlEquivalent.To("<X> <A/> </X>"));
		}

		[Fact]
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
				KnownTypes.Add(new XmlKnownType("A", null, null, null, typeof(T)), true);
			}

			if (IncludedTypes == null)
			{
				IncludedTypes = new MockXmlIncludedTypeMap();
				IncludedTypes.DefaultClrType = typeof(T);
				IncludedTypes.InnerSet.Add(new XmlIncludedType("T", string.Empty, typeof(T)));
			}
		}

		protected static XmlKnownTypeSet        KnownTypes;
		protected static MockXmlIncludedTypeMap IncludedTypes;
		protected sealed class T { }
	}
}
#endif
