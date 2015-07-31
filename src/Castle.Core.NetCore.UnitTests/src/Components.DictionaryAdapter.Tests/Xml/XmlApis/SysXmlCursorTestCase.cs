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

#if !SILVERLIGHT && !MONO && !NETCORE // Until support for other platforms is verified
namespace CastleTests.Components.DictionaryAdapter.Xml.Tests
{
	using System;
	using System.Xml;
	using Castle.Components.DictionaryAdapter.Xml;
	using Castle.Components.DictionaryAdapter.Tests;
	using System.Xml.Serialization;
	using System.Runtime.Serialization;
	using Xunit;

		public class SysXmlCursorTestCase
	{
		[Fact]
		public void Constructor_RequiresNode()
		{
			Assert.Throws<ArgumentNullException>(() =>
				new SysXmlCursor(null, KnownTypes, NamespaceSource.Instance, CursorFlags.Elements));
		}

		[Fact]
		public void Constructor_RequiresKnownTypes()
		{
			var xml = Xml("<X/>");

			Assert.Throws<ArgumentNullException>(() =>
				new SysXmlCursor(xml, null, NamespaceSource.Instance, CursorFlags.Elements));
		}

		[Fact]
		public void Iterate_WhenEmpty()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.AllNodes);

			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_WhenAtEnd()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.AllNodes);
			cursor.MoveNext();

			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_AllNodes_WhenNoMatchExists()
		{
			var xml    = Xml("<X A='a'> foo <A/> bar </X>");
			var cursor = Cursor(xml, CursorFlags.AllNodes);

			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_AllNodes_WhenOneMatchExists_AsAttribute()
		{
			var xml    = Xml("<X A='a' Item='1' B='b'> <A/> </X>");
			var cursor = Cursor(xml, CursorFlags.AllNodes);

			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.Name.LocalName,  Is.EqualTo(ItemType.Name.LocalName));
			Assert.That(cursor.Value,      Is.EqualTo("1"));
			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_AllNodes_WhenOneMatchExists_AsElement()
		{
			var xml    = Xml("<X A='a'> <A/> <Item>1</Item> <B/> </X>");
			var cursor = Cursor(xml, CursorFlags.AllNodes);

			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.Name.LocalName,  Is.EqualTo(ItemType.Name.LocalName));
			Assert.That(cursor.Value,      Is.EqualTo("1"));
			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_AllNodes_WhenMultipleMatchesExist_InSingleMode()
		{
			var xml    = Xml("<X A='a' Item='2' B='b'> <A/> <Item>1</Item> <B/> </X>");
			var cursor = Cursor(xml, CursorFlags.AllNodes);

			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_AllNodes_WhenMultipleMatchesExist_InMultipleMode()
		{
			var xml    = Xml("<X A='a' Item='2' B='b'> <A/> <Item>1</Item> <B/> </X>");
			var cursor = Cursor(xml, CursorFlags.AllNodes | CursorFlags.Multiple);

			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.Name.LocalName,  Is.EqualTo(ItemType.Name.LocalName));
			Assert.That(cursor.Value,      Is.EqualTo("1"));
			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.Name.LocalName,  Is.EqualTo(ItemType.Name.LocalName));
			Assert.That(cursor.Value,      Is.EqualTo("2"));
			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_Element_WhenNoMatchExists()
		{
			var xml    = Xml("<X Item='-'> foo <A/> bar </X>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_Element_WhenOneMatchExists()
		{
			var xml    = Xml("<X Item='-'> <A/> <Item>1</Item> <B/> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.Name.LocalName,  Is.EqualTo(ItemType.Name.LocalName));
			Assert.That(cursor.Value,      Is.EqualTo("1"));
			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_Element_WhenMultipleMatchesExist_InSingleMode()
		{
			var xml    = Xml("<X Item='-'> <A/> <Item>1</Item> <B/> <Item>2</Item> <C/> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_Element_WhenMultipleMatchesExist_InMultipleMode()
		{
			var xml    = Xml("<X Item='-'> <A/> <Item>1</Item> <B/> <Item>2</Item> <C/> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements | CursorFlags.Multiple);

			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.Name.LocalName,  Is.EqualTo(ItemType.Name.LocalName));
			Assert.That(cursor.Value,      Is.EqualTo("1"));
			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.Name.LocalName,  Is.EqualTo(ItemType.Name.LocalName));
			Assert.That(cursor.Value,      Is.EqualTo("2"));
			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_Attribute_WhenNoMatchExists()
		{
			var xml    = Xml("<X A='a'> <Item>-</Item> </X>");
			var cursor = Cursor(xml, CursorFlags.Attributes);

			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_Attribute_WhenOneMatchExists()
		{
			var xml    = Xml("<X A='a' Item='1' B='b'> <Item>-</Item> </X>");
			var cursor = Cursor(xml, CursorFlags.Attributes);

			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.Name.LocalName,  Is.EqualTo(ItemType.Name.LocalName));
			Assert.That(cursor.Value,      Is.EqualTo("1"));
			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Reset()
		{
			var xml    = Xml("<X> <Item>1</Item> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.Name.LocalName,  Is.EqualTo(ItemType.Name.LocalName));
			Assert.That(cursor.Value,      Is.EqualTo("1"));
			Assert.That(cursor.MoveNext(), Is.False);

			cursor.Reset();

			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.Name.LocalName,  Is.EqualTo(ItemType.Name.LocalName));
			Assert.That(cursor.Value,      Is.EqualTo("1"));
			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void MoveTo_NotSysXmlNode_Fails()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			Assert.Throws<InvalidOperationException>(() =>
				cursor.MoveTo(new DummyXmlNode()));
		}

		[Fact]
		public void MoveTo_NotARecognizedNode_Fails()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			var wrongNode = Xml("<Q/>");

			Assert.Throws<InvalidOperationException>(() =>
				cursor.MoveTo(wrongNode));
		}

		[Fact]
		public void MoveTo_RecognizedNode_Succeeds_ForElement()
		{
			var xml    = Xml("<X> <Item>1</Item> <Other>2</Other> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements | CursorFlags.Multiple);

			cursor.MoveNext();
			var node = cursor.Save();
			cursor.MoveToEnd();

			cursor.MoveTo(node);

			Assert.That(cursor.Name.LocalName,  Is.EqualTo(ItemType.Name.LocalName));
			Assert.That(cursor.Value,      Is.EqualTo("1"));
			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void MoveTo_RecognizedNode_Succeeds_ForAttribute()
		{
			var xml    = Xml("<X Item='1' Other='2'/>");
			var cursor = Cursor(xml, CursorFlags.Attributes | CursorFlags.Multiple);

			cursor.MoveNext();
			cursor.MoveNext();
			var node = cursor.Save();
			cursor.MoveToEnd();

			cursor.MoveTo(node);

			Assert.That(cursor.Name.LocalName,  Is.EqualTo(OtherType.Name.LocalName));
			Assert.That(cursor.Value,      Is.EqualTo("2"));
			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void MoveToEnd()
		{
			var xml    = Xml("<X> <Item>1</Item> <Other>2</Other> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements | CursorFlags.Multiple);

			cursor.MoveNext();
			cursor.MoveToEnd();

			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void AsVirtual_WhenParentIsRealNode()
		{
			var xml = Xml("<X/>");
			IXmlCursor cursor;
			cursor = xml.SelectChildren(KnownTypes, NamespaceSource.Instance, CursorFlags.Elements | CursorFlags.Mutable);
			cursor.MoveNext();
			var node = (IXmlNode) cursor;

			node.Value = "1";

			Assert.That(xml.GetNode(), XmlEquivalent.To("<X> <Item>1</Item> </X>"));
		}

		[Fact]
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

			Assert.That(xml.GetNode(), XmlEquivalent.To("<X> <Item><Item>1</Item></Item> </X>"));
		}

		[Fact]
		public void Save()
		{
			var xml    = Xml("<X> <Item>1</Item> <Other>2</Other> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements | CursorFlags.Multiple);

			cursor.MoveNext();
			var node = cursor.Save();
			cursor.MoveNext();

			Assert.That(node.Name.LocalName,  Is.EqualTo(ItemType.Name.LocalName));
			Assert.That(node.Value,      Is.EqualTo("1"));
		}

		[Fact]
		public void MakeNext_WhenAtNode_Coerces()
		{
			var xml    = Xml("<X> <Item>1</Item> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements | CursorFlags.Multiple);

			cursor.MakeNext(OtherType.ClrType);

			Assert.That(cursor.Name.LocalName, Is.EqualTo(OtherType.Name.LocalName));
			Assert.That(xml.GetNode(), XmlEquivalent.To("<X> <Other/> </X>"));
		}

		[Fact]
		public void MakeNext_WhenAtEnd_Creates()
		{
			var xml    = Xml("<X> <Item>1</Item> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements | CursorFlags.Multiple);

			cursor.MoveNext();
			cursor.MakeNext(OtherType.ClrType);

			Assert.That(cursor.Name.LocalName, Is.EqualTo(OtherType.Name.LocalName));
			Assert.That(xml.GetNode(), XmlEquivalent.To("<X> <Item>1</Item> <Other/> </X>"));
		}

		[Fact]
		public void Create_BeforeFirstItem_Fails()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			Assert.Throws<InvalidOperationException>(() =>
				cursor.Create(ItemType.ClrType));
		}

		[Fact]
		public void Create_BeforeEnd_IsInsert_ForElement()
		{
			var xml    = Xml("<X> <Other>2</Other> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements);
			cursor.MoveNext();

			cursor.Create(ItemType.ClrType);
			cursor.Value = "1";

			Assert.That(cursor.Name.LocalName,  Is.EqualTo(ItemType.Name.LocalName));
			Assert.That(cursor.Value,      Is.EqualTo("1"));
			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.Name.LocalName,  Is.EqualTo(OtherType.Name.LocalName));
			Assert.That(cursor.Value,      Is.EqualTo("2"));
			Assert.That(cursor.MoveNext(), Is.False);

			Assert.That(xml.GetNode(), XmlEquivalent.To("<X> <Item>1</Item> <Other>2</Other> </X>"));
		}

		[Fact]
		public void Create_BeforeEnd_IsInsert_ForAttribute()
		{
			var xml    = Xml("<X Other='2'/>");
			var cursor = Cursor(xml, CursorFlags.Attributes);

			cursor.MoveNext();
			cursor.Create(ItemType.ClrType);
			cursor.Value = "1";

			Assert.That(cursor.Name.LocalName,  Is.EqualTo(ItemType.Name.LocalName));
			Assert.That(cursor.Value,      Is.EqualTo("1"));
			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.Name.LocalName,  Is.EqualTo(OtherType.Name.LocalName));
			Assert.That(cursor.Value,      Is.EqualTo("2"));
			Assert.That(cursor.MoveNext(), Is.False);

			Assert.That(xml.GetNode(), XmlEquivalent.To("<X Item='1' Other='2'/>"));
		}

		[Fact]
		public void Create_AtEnd_IsAppend_ForElement()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			cursor.MoveNext();
			cursor.Create(ItemType.ClrType);
			cursor.Value = "1";

			Assert.That(cursor.Name.LocalName,  Is.EqualTo(ItemType.Name.LocalName));
			Assert.That(cursor.Value,      Is.EqualTo("1"));
			Assert.That(cursor.MoveNext(), Is.False);

			Assert.That(xml.GetNode(), XmlEquivalent.To("<X> <Item>1</Item> </X>"));
		}

		[Fact]
		public void Create_AtEnd_IsAppend_ForAttribute()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.Attributes);

			cursor.MoveNext();
			cursor.Create(ItemType.ClrType);
			cursor.Value = "1";

			Assert.That(cursor.Name.LocalName,  Is.EqualTo(ItemType.Name.LocalName));
			Assert.That(cursor.Value,      Is.EqualTo("1"));
			Assert.That(cursor.MoveNext(), Is.False);

			Assert.That(xml.GetNode(), XmlEquivalent.To("<X Item='1'/>"));
		}

		[Fact]
		public void Create_UnknownType()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			cursor.MoveNext();
			Assert.Throws<SerializationException>(() =>
				cursor.Create(typeof(UnknownType)));
		}

		[Fact]
		public void Coerce_WhenBeforeFirstItem()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			Assert.Throws<InvalidOperationException>(() =>
				cursor.Coerce(OtherType.ClrType));
		}

		[Fact]
		public void Coerce_WhenAfterLastItem()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.Elements);
			cursor.MoveNext();

			Assert.Throws<InvalidOperationException>(() =>
				cursor.Coerce(OtherType.ClrType));
		}

		[Fact]
		public void Coerce_WhenItemIsCompatible()
		{
			var xml    = Xml("<X> <Item/> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			cursor.MoveNext();
			cursor.Coerce(ItemType.ClrType);

			Assert.That(cursor.Name.LocalName,  Is.EqualTo(ItemType.Name.LocalName));
			Assert.That(cursor.MoveNext(), Is.False);
			Assert.That(xml.GetNode(), XmlEquivalent.To("<X> <Item/> </X>"));
		}

		[Fact]
		public void Coerce_WhenItemDiffersInXsiType()
		{
			var xml    = Xml("<X> <Item/> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			cursor.MoveNext();
			cursor.Coerce(OtherType.ClrType);

			Assert.That(cursor.Name.LocalName,  Is.EqualTo(OtherType.Name.LocalName));
			Assert.That(cursor.MoveNext(), Is.False);
			Assert.That(xml.GetNode(), XmlEquivalent.To("<X> <Other/> </X>"));
		}

		[Fact]
		public void Coerce_WhenItemDiffersInLocalNameOrNamespaceUri()
		{
			var xml    = Xml("<X> <Item/> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			cursor.MoveNext();
			cursor.Coerce(OtherType.ClrType);

			Assert.That(cursor.Name.LocalName,  Is.EqualTo(OtherType.Name.LocalName));
			Assert.That(cursor.MoveNext(), Is.False);
			Assert.That(xml.GetNode(), XmlEquivalent.To("<X> <Other/> </X>"));
		}

		[Fact]
		public void Coerce_UnknownType()
		{
			var xml    = Xml("<X> <Item/> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			cursor.MoveNext();
			Assert.Throws<SerializationException>(() =>
				cursor.Coerce(typeof(UnknownType)));
		}

		[Fact]
		public void Remove_WhenBeforeFirstItem_Fails()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.Elements);

			Assert.Throws<InvalidOperationException>(() =>
				cursor.Remove());
		}

		[Fact]
		public void Remove_WhenAfterLastItem_Fails()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, CursorFlags.Elements);
			cursor.MoveNext();

			Assert.Throws<InvalidOperationException>(() =>
				cursor.Remove());
		}

		[Fact]
		public void Remove_WhenAtItem_RemovesItem_ForElement()
		{
			var xml    = Xml("<X> <Item/> <Other/> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements | CursorFlags.Multiple);

			cursor.MoveNext();
			cursor.Remove();

			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.Name.LocalName,  Is.EqualTo(OtherType.Name.LocalName));
			Assert.That(cursor.MoveNext(), Is.False);

			Assert.That(xml.GetNode(), XmlEquivalent.To("<X> <Other/> </X>"));
		}

		[Fact]
		public void Remove_WhenAtItem_RemovesItem_ForAttribute()
		{
			var xml    = Xml("<X Item='1' Other='2'/>");
			var cursor = Cursor(xml, CursorFlags.Attributes | CursorFlags.Multiple);

			cursor.MoveNext();
			cursor.Remove();

			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.Name.LocalName,  Is.EqualTo(OtherType.Name.LocalName));
			Assert.That(cursor.MoveNext(), Is.False);

			Assert.That(xml.GetNode(), XmlEquivalent.To("<X Other='2'/>"));
		}

		[Fact]
		public void RemoveToEnd()
		{
			var xml    = Xml("<X> <Item>1</Item> <Item>2</Item> <Item>3</Item> </X>");
			var cursor = Cursor(xml, CursorFlags.Elements | CursorFlags.Multiple);

			cursor.MoveNext();
			cursor.RemoveAllNext();

			Assert.That(xml.GetNode(), XmlEquivalent.To("<X> <Item>1</Item> </X>"));
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

		[TestFixtureSetUp]
		public virtual void OneTimeSetUp()
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
#endif
