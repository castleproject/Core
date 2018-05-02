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
	using System.Xml.Serialization;
	using System.Runtime.Serialization;

	[TestFixture]
	public class SysXmlCursorTestCase
	{
		[Test]
		public void Constructor_RequiresNode()
		{
			Assert.Throws<ArgumentNullException>(() =>
				new SysXmlCursor(null, KnownTypes, NamespaceSource.Instance, CursorFlags.Elements));
		}

		[Test]
		public void Constructor_RequiresKnownTypes()
		{
			var xml = Xml("<X/>");

			Assert.Throws<ArgumentNullException>(() =>
				new SysXmlCursor(xml, null, NamespaceSource.Instance, CursorFlags.Elements));
		}

		[Test]
		public void Iterate_WhenEmpty()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.AllNodes);

			Assert.False(cursor.MoveNext());
		}

		[Test]
		public void Iterate_WhenAtEnd()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.AllNodes);
			cursor.MoveNext();

			Assert.False(cursor.MoveNext());
		}

		[Test]
		public void Iterate_AllNodes_WhenNoMatchExists()
		{
			var xml    = Xml("<X A='a'> foo <A/> bar </X>");
			var cursor = Cursor(xml, CursorFlags.AllNodes);

			Assert.False(cursor.MoveNext());
		}

		[Test]
		public void Iterate_AllNodes_WhenOneMatchExists_AsAttribute()
		{
			var xml    = Xml("<X A='a' Item='1' B='b'> <A/> </X>");
			var cursor = Cursor(xml, CursorFlags.AllNodes);

			Assert.True(cursor.MoveNext());
			Assert.AreEqual(ItemType.Name.LocalName, cursor.Name.LocalName);
			Assert.AreEqual("1", cursor.Value);
			Assert.False(cursor.MoveNext());
		}

		[Test]
		public void Iterate_AllNodes_WhenOneMatchExists_AsElement()
		{
			var xml    = Xml("<X A='a'> <A/> <Item>1</Item> <B/> </X>");
			var cursor = Cursor(xml, CursorFlags.AllNodes);

			Assert.True(cursor.MoveNext());
			Assert.AreEqual(ItemType.Name.LocalName, cursor.Name.LocalName);
			Assert.AreEqual("1", cursor.Value);
			Assert.False(cursor.MoveNext());
		}

		[Test]
		public void Iterate_AllNodes_WhenMultipleMatchesExist_InSingleMode()
		{
			var xml    = Xml("<X A='a' Item='2' B='b'> <A/> <Item>1</Item> <B/> </X>");
			var cursor = Cursor(xml, CursorFlags.AllNodes);

			Assert.False(cursor.MoveNext());
		}

		[Test]
		public void Iterate_AllNodes_WhenMultipleMatchesExist_InMultipleMode()
		{
			var xml    = Xml("<X A='a' Item='2' B='b'> <A/> <Item>1</Item> <B/> </X>");
			var cursor = Cursor(xml, CursorFlags.AllNodes | CursorFlags.Multiple);

			Assert.True(cursor.MoveNext());
			Assert.AreEqual(ItemType.Name.LocalName, cursor.Name.LocalName);
			Assert.AreEqual("1", cursor.Value);
			Assert.True(cursor.MoveNext());
			Assert.AreEqual(ItemType.Name.LocalName, cursor.Name.LocalName);
			Assert.AreEqual("2", cursor.Value);
			Assert.False(cursor.MoveNext());
		}

		[Test]
		public void Iterate_Element_WhenNoMatchExists()
		{
			var xml    = Xml("<X Item='-'> foo <A/> bar </X>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			Assert.False(cursor.MoveNext());
		}

		[Test]
		public void Iterate_Element_WhenOneMatchExists()
		{
			var xml    = Xml("<X Item='-'> <A/> <Item>1</Item> <B/> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			Assert.True(cursor.MoveNext());
			Assert.AreEqual(ItemType.Name.LocalName, cursor.Name.LocalName);
			Assert.AreEqual("1", cursor.Value);
			Assert.False(cursor.MoveNext());
		}

		[Test]
		public void Iterate_Element_WhenMultipleMatchesExist_InSingleMode()
		{
			var xml    = Xml("<X Item='-'> <A/> <Item>1</Item> <B/> <Item>2</Item> <C/> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			Assert.False(cursor.MoveNext());
		}

		[Test]
		public void Iterate_Element_WhenMultipleMatchesExist_InMultipleMode()
		{
			var xml    = Xml("<X Item='-'> <A/> <Item>1</Item> <B/> <Item>2</Item> <C/> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements | CursorFlags.Multiple);

			Assert.True(cursor.MoveNext());
			Assert.AreEqual(ItemType.Name.LocalName, cursor.Name.LocalName);
			Assert.AreEqual("1", cursor.Value);
			Assert.True(cursor.MoveNext());
			Assert.AreEqual(ItemType.Name.LocalName, cursor.Name.LocalName);
			Assert.AreEqual("2", cursor.Value);
			Assert.False(cursor.MoveNext());
		}

		[Test]
		public void Iterate_Attribute_WhenNoMatchExists()
		{
			var xml    = Xml("<X A='a'> <Item>-</Item> </X>");
			var cursor = Cursor(xml, CursorFlags.Attributes);

			Assert.False(cursor.MoveNext());
		}

		[Test]
		public void Iterate_Attribute_WhenOneMatchExists()
		{
			var xml    = Xml("<X A='a' Item='1' B='b'> <Item>-</Item> </X>");
			var cursor = Cursor(xml, CursorFlags.Attributes);

			Assert.True(cursor.MoveNext());
			Assert.AreEqual(ItemType.Name.LocalName, cursor.Name.LocalName);
			Assert.AreEqual("1", cursor.Value);
			Assert.False(cursor.MoveNext());
		}

		[Test]
		public void Reset()
		{
			var xml    = Xml("<X> <Item>1</Item> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			Assert.True(cursor.MoveNext());
			Assert.AreEqual(ItemType.Name.LocalName, cursor.Name.LocalName);
			Assert.AreEqual("1", cursor.Value);
			Assert.False(cursor.MoveNext());

			cursor.Reset();

			Assert.True(cursor.MoveNext());
			Assert.AreEqual(ItemType.Name.LocalName, cursor.Name.LocalName);
			Assert.AreEqual("1", cursor.Value);
			Assert.False(cursor.MoveNext());
		}

		[Test]
		public void MoveTo_NotSysXmlNode_Fails()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			Assert.Throws<InvalidOperationException>(() =>
				cursor.MoveTo(new DummyXmlNode()));
		}

		[Test]
		public void MoveTo_NotARecognizedNode_Fails()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			var wrongNode = Xml("<Q/>");

			Assert.Throws<InvalidOperationException>(() =>
				cursor.MoveTo(wrongNode));
		}

		[Test]
		public void MoveTo_RecognizedNode_Succeeds_ForElement()
		{
			var xml    = Xml("<X> <Item>1</Item> <Other>2</Other> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements | CursorFlags.Multiple);

			cursor.MoveNext();
			var node = cursor.Save();
			cursor.MoveToEnd();

			cursor.MoveTo(node);

			Assert.AreEqual(ItemType.Name.LocalName, cursor.Name.LocalName);
			Assert.AreEqual("1", cursor.Value);
			Assert.True(cursor.MoveNext());
			Assert.False(cursor.MoveNext());
		}

		[Test]
		public void MoveTo_RecognizedNode_Succeeds_ForAttribute()
		{
			var xml    = Xml("<X Item='1' Other='2'/>");
			var cursor = Cursor(xml, CursorFlags.Attributes | CursorFlags.Multiple);

			cursor.MoveNext();
			cursor.MoveNext();
			var node = cursor.Save();
			cursor.MoveToEnd();

			cursor.MoveTo(node);

			Assert.AreEqual(OtherType.Name.LocalName, cursor.Name.LocalName);
			Assert.AreEqual("2", cursor.Value);
			Assert.False(cursor.MoveNext());
		}

		[Test]
		public void MoveToEnd()
		{
			var xml    = Xml("<X> <Item>1</Item> <Other>2</Other> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements | CursorFlags.Multiple);

			cursor.MoveNext();
			cursor.MoveToEnd();

			Assert.False(cursor.MoveNext());
		}

		[Test]
		public void AsVirtual_WhenParentIsRealNode()
		{
			var xml = Xml("<X/>");
			IXmlCursor cursor;
			cursor = xml.SelectChildren(KnownTypes, NamespaceSource.Instance, CursorFlags.Elements | CursorFlags.Mutable);
			cursor.MoveNext();
			var node = (IXmlNode) cursor;

			node.Value = "1";

			CustomAssert.AreXmlEquivalent("<X> <Item>1</Item> </X>", xml.GetNode());
		}

		[Test]
		public void AsVirtual_WhenParentIsVirtualNode()
		{
			var xml = Xml("<X/>");
			IXmlCursor cursor;
			cursor = xml.SelectChildren(KnownTypes, NamespaceSource.Instance, CursorFlags.Elements | CursorFlags.Mutable);
			cursor.MoveNext();
			cursor = cursor.SelectChildren(KnownTypes, NamespaceSource.Instance, CursorFlags.Elements | CursorFlags.Mutable);
			cursor.MoveNext();
			var node = (IXmlNode) cursor;

			node.Value = "1";

			CustomAssert.AreXmlEquivalent("<X> <Item><Item>1</Item></Item> </X>", xml.GetNode());
		}

		[Test]
		public void Save()
		{
			var xml    = Xml("<X> <Item>1</Item> <Other>2</Other> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements | CursorFlags.Multiple);

			cursor.MoveNext();
			var node = cursor.Save();
			cursor.MoveNext();

			Assert.AreEqual(ItemType.Name.LocalName, node.Name.LocalName);
			Assert.AreEqual("1", node.Value);
		}

		[Test]
		public void MakeNext_WhenAtNode_Coerces()
		{
			var xml    = Xml("<X> <Item>1</Item> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements | CursorFlags.Multiple);

			cursor.MakeNext(OtherType.ClrType);

			Assert.AreEqual(OtherType.Name.LocalName, cursor.Name.LocalName);
			CustomAssert.AreXmlEquivalent("<X> <Other/> </X>", xml.GetNode());
		}

		[Test]
		public void MakeNext_WhenAtEnd_Creates()
		{
			var xml    = Xml("<X> <Item>1</Item> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements | CursorFlags.Multiple);

			cursor.MoveNext();
			cursor.MakeNext(OtherType.ClrType);

			Assert.AreEqual(OtherType.Name.LocalName, cursor.Name.LocalName);
			CustomAssert.AreXmlEquivalent("<X> <Item>1</Item> <Other/> </X>", xml.GetNode());
		}

		[Test]
		public void Create_BeforeFirstItem_Fails()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			Assert.Throws<InvalidOperationException>(() =>
				cursor.Create(ItemType.ClrType));
		}

		[Test]
		public void Create_BeforeEnd_IsInsert_ForElement()
		{
			var xml    = Xml("<X> <Other>2</Other> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements);
			cursor.MoveNext();

			cursor.Create(ItemType.ClrType);
			cursor.Value = "1";

			Assert.AreEqual(ItemType.Name.LocalName, cursor.Name.LocalName);
			Assert.AreEqual("1", cursor.Value);
			Assert.True(cursor.MoveNext());
			Assert.AreEqual(OtherType.Name.LocalName, cursor.Name.LocalName);
			Assert.AreEqual("2", cursor.Value);
			Assert.False(cursor.MoveNext());

			CustomAssert.AreXmlEquivalent("<X> <Item>1</Item> <Other>2</Other> </X>", xml.GetNode());
		}

		[Test]
		public void Create_BeforeEnd_IsInsert_ForAttribute()
		{
			var xml    = Xml("<X Other='2'/>");
			var cursor = Cursor(xml, CursorFlags.Attributes);

			cursor.MoveNext();
			cursor.Create(ItemType.ClrType);
			cursor.Value = "1";

			Assert.AreEqual(ItemType.Name.LocalName, cursor.Name.LocalName);
			Assert.AreEqual("1", cursor.Value);
			Assert.True(cursor.MoveNext());
			Assert.AreEqual(OtherType.Name.LocalName, cursor.Name.LocalName);
			Assert.AreEqual("2", cursor.Value);
			Assert.False(cursor.MoveNext());

			CustomAssert.AreXmlEquivalent("<X Item='1' Other='2'/>", xml.GetNode());
		}

		[Test]
		public void Create_AtEnd_IsAppend_ForElement()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			cursor.MoveNext();
			cursor.Create(ItemType.ClrType);
			cursor.Value = "1";

			Assert.AreEqual(ItemType.Name.LocalName, cursor.Name.LocalName);
			Assert.AreEqual("1", cursor.Value);
			Assert.False(cursor.MoveNext());

			CustomAssert.AreXmlEquivalent("<X> <Item>1</Item> </X>", xml.GetNode());
		}

		[Test]
		public void Create_AtEnd_IsAppend_ForAttribute()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.Attributes);

			cursor.MoveNext();
			cursor.Create(ItemType.ClrType);
			cursor.Value = "1";

			Assert.AreEqual(ItemType.Name.LocalName, cursor.Name.LocalName);
			Assert.AreEqual("1", cursor.Value);
			Assert.False(cursor.MoveNext());

			CustomAssert.AreXmlEquivalent("<X Item='1'/>", xml.GetNode());
		}

		[Test]
		public void Create_UnknownType()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			cursor.MoveNext();
			Assert.Throws<SerializationException>(() =>
				cursor.Create(typeof(UnknownType)));
		}

		[Test]
		public void Coerce_WhenBeforeFirstItem()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			Assert.Throws<InvalidOperationException>(() =>
				cursor.Coerce(OtherType.ClrType));
		}

		[Test]
		public void Coerce_WhenAfterLastItem()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.Elements);
			cursor.MoveNext();

			Assert.Throws<InvalidOperationException>(() =>
				cursor.Coerce(OtherType.ClrType));
		}

		[Test]
		public void Coerce_WhenItemIsCompatible()
		{
			var xml    = Xml("<X> <Item/> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			cursor.MoveNext();
			cursor.Coerce(ItemType.ClrType);

			Assert.AreEqual(ItemType.Name.LocalName, cursor.Name.LocalName);
			Assert.False(cursor.MoveNext());
			CustomAssert.AreXmlEquivalent("<X> <Item/> </X>", xml.GetNode());
		}

		[Test]
		public void Coerce_WhenItemDiffersInXsiType()
		{
			var xml    = Xml("<X> <Item/> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			cursor.MoveNext();
			cursor.Coerce(OtherType.ClrType);

			Assert.AreEqual(OtherType.Name.LocalName, cursor.Name.LocalName);
			Assert.False(cursor.MoveNext());
			CustomAssert.AreXmlEquivalent("<X> <Other/> </X>", xml.GetNode());
		}

		[Test]
		public void Coerce_WhenItemDiffersInLocalNameOrNamespaceUri()
		{
			var xml    = Xml("<X> <Item/> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			cursor.MoveNext();
			cursor.Coerce(OtherType.ClrType);

			Assert.AreEqual(OtherType.Name.LocalName, cursor.Name.LocalName);
			Assert.False(cursor.MoveNext());
			CustomAssert.AreXmlEquivalent("<X> <Other/> </X>", xml.GetNode());
		}

		[Test]
		public void Coerce_UnknownType()
		{
			var xml    = Xml("<X> <Item/> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			cursor.MoveNext();
			Assert.Throws<SerializationException>(() =>
				cursor.Coerce(typeof(UnknownType)));
		}

		[Test]
		public void Remove_WhenBeforeFirstItem_Fails()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			Assert.Throws<InvalidOperationException>(() =>
				cursor.Remove());
		}

		[Test]
		public void Remove_WhenAfterLastItem_Fails()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.Elements);
			cursor.MoveNext();

			Assert.Throws<InvalidOperationException>(() =>
				cursor.Remove());
		}

		[Test]
		public void Remove_WhenAtItem_RemovesItem_ForElement()
		{
			var xml    = Xml("<X> <Item/> <Other/> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements | CursorFlags.Multiple);

			cursor.MoveNext();
			cursor.Remove();

			Assert.True(cursor.MoveNext());
			Assert.AreEqual(OtherType.Name.LocalName, cursor.Name.LocalName);
			Assert.False(cursor.MoveNext());

			CustomAssert.AreXmlEquivalent("<X> <Other/> </X>", xml.GetNode());
		}

		[Test]
		public void Remove_WhenAtItem_RemovesItem_ForAttribute()
		{
			var xml    = Xml("<X Item='1' Other='2'/>");
			var cursor = Cursor(xml, CursorFlags.Attributes | CursorFlags.Multiple);

			cursor.MoveNext();
			cursor.Remove();

			Assert.True(cursor.MoveNext());
			Assert.AreEqual(OtherType.Name.LocalName, cursor.Name.LocalName);
			Assert.False(cursor.MoveNext());

			CustomAssert.AreXmlEquivalent("<X Other='2'/>", xml.GetNode());
		}

		[Test]
		public void RemoveToEnd()
		{
			var xml    = Xml("<X> <Item>1</Item> <Item>2</Item> <Item>3</Item> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements | CursorFlags.Multiple);

			cursor.MoveNext();
			cursor.RemoveAllNext();

			CustomAssert.AreXmlEquivalent("<X> <Item>1</Item> </X>", xml.GetNode());
		}

		protected static SysXmlNode Xml(params string[] xml)
		{
			var document = new XmlDocument();
			document.LoadXml(string.Concat(xml));
			return new SysXmlNode(document.DocumentElement, typeof(object), NamespaceSource.Instance);
		}

		protected static SysXmlCursor Cursor(SysXmlNode node, CursorFlags flags)
		{
			return new SysXmlCursor(node, KnownTypes, NamespaceSource.Instance, flags);
		}

		public SysXmlCursorTestCase()
		{
			KnownTypes.Add(ItemType,  true);
			KnownTypes.Add(OtherType, true);
		}

		protected static readonly XmlKnownType
			ItemType  = new XmlKnownType("Item",  null, null, null, typeof(_TypeA)),
			OtherType = new XmlKnownType("Other", null, null, null, typeof(_TypeB));

		protected static readonly XmlKnownTypeSet
			KnownTypes = new XmlKnownTypeSet(typeof(_TypeA));

		private class UnknownType { }
		private class _TypeA          { }
		private class _TypeB : _TypeA { }
	}
}
