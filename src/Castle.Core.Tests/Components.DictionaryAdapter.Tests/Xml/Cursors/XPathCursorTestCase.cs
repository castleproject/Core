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

namespace CastleTests.Components.DictionaryAdapter.Xml.Tests
{
	using System;
	using System.Xml;
	using System.Xml.XPath;
	using Castle.Components.DictionaryAdapter.Xml;
	using NUnit.Framework;

	[TestFixture]
	public abstract class XPathCursorTestCase
	{
		[Test]
		public void Iterate_WhenEmpty()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, "A", ItemType, CursorFlags.AllNodes);

			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Test]
		public void Iterate_WhenAtEnd()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, "A", ItemType, CursorFlags.AllNodes);
			cursor.MoveNext();

			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Test]
		public void Iterate_Element_WhenNoMatchExists()
		{
		    var xml    = Xml("<X Item='-'> foo <A/> bar </X>");
		    var cursor = Cursor(xml, "Item", ItemType, CursorFlags.None);

		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Test]
		public void Iterate_Element_WhenOneMatchExists()
		{
		    var xml    = Xml("<X Item='-'> <A/> <Item>1</Item> <B/> </X>");
		    var cursor = Cursor(xml, "Item", ItemType, CursorFlags.None);

		    Assert.That(cursor.MoveNext(), Is.True);
		    Assert.That(cursor.LocalName,  Is.EqualTo(ItemType.LocalName));
		    Assert.That(cursor.Value,      Is.EqualTo("1"));
		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Test]
		public void Iterate_Element_WhenMultipleMatchesExist_InSingleMode()
		{
		    var xml    = Xml("<X Item='-'> <A/> <Item>1</Item> <B/> <Item>2</Item> <C/> </X>");
		    var cursor = Cursor(xml, "Item", ItemType, CursorFlags.None);

		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Test]
		public void Iterate_Element_WhenMultipleMatchesExist_InMultipleMode()
		{
		    var xml    = Xml("<X Item='-'> <A/> <Item>1</Item> <B/> <Item>2</Item> <C/> </X>");
		    var cursor = Cursor(xml, "Item", ItemType, CursorFlags.Multiple);

		    Assert.That(cursor.MoveNext(), Is.True);
		    Assert.That(cursor.LocalName,  Is.EqualTo(ItemType.LocalName));
		    Assert.That(cursor.Value,      Is.EqualTo("1"));
		    Assert.That(cursor.MoveNext(), Is.True);
		    Assert.That(cursor.LocalName,  Is.EqualTo(ItemType.LocalName));
		    Assert.That(cursor.Value,      Is.EqualTo("2"));
		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Test]
		public void Iterate_Attribute_WhenNoMatchExists()
		{
		    var xml    = Xml("<X A='a'> <Item>-</Item> </X>");
		    var cursor = Cursor(xml, "@Item", ItemType, CursorFlags.None);

		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Test]
		public void Iterate_Attribute_WhenOneMatchExists()
		{
		    var xml    = Xml("<X A='a' Item='1' B='b'> <Item>-</Item> </X>");
		    var cursor = Cursor(xml, "@Item", ItemType, CursorFlags.None);

		    Assert.That(cursor.MoveNext(), Is.True);
		    Assert.That(cursor.LocalName,  Is.EqualTo(ItemType.LocalName));
		    Assert.That(cursor.Value,      Is.EqualTo("1"));
		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Test]
		public void Iterate_AllNodes_WhenNoMatchExists()
		{
		    var xml    = Xml("<X A='a'> foo <A/> bar </X>");
		    var cursor = Cursor(xml, "@Item|Item", ItemType, CursorFlags.None);

		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Test]
		public void Iterate_AllNodes_WhenOneMatchExists_AsAttribute()
		{
		    var xml    = Xml("<X A='a' Item='1' B='b'> <A/> </X>");
		    var cursor = Cursor(xml, "@Item|Item", ItemType, CursorFlags.None);

		    Assert.That(cursor.MoveNext(), Is.True);
		    Assert.That(cursor.LocalName,  Is.EqualTo(ItemType.LocalName));
		    Assert.That(cursor.Value,      Is.EqualTo("1"));
		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Test]
		public void Iterate_AllNodes_WhenOneMatchExists_AsElement()
		{
		    var xml    = Xml("<X A='a'> <A/> <Item>1</Item> <B/> </X>");
		    var cursor = Cursor(xml, "@Item|Item", ItemType, CursorFlags.None);

		    Assert.That(cursor.MoveNext(), Is.True);
		    Assert.That(cursor.LocalName,  Is.EqualTo(ItemType.LocalName));
		    Assert.That(cursor.Value,      Is.EqualTo("1"));
		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Test]
		public void Iterate_AllNodes_WhenMultipleMatchesExist_InSingleMode()
		{
		    var xml    = Xml("<X A='a' Item='2' B='b'> <A/> <Item>1</Item> <B/> </X>");
		    var cursor = Cursor(xml, "@Item|Item", ItemType, CursorFlags.None);

		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Test]
		public void Iterate_AllNodes_WhenMultipleMatchesExist_InMultipleMode()
		{
		    var xml    = Xml("<X A='a' Item='1' B='b'> <A/> <Item>2</Item> <B/> </X>");
		    var cursor = Cursor(xml, "@Item|Item", ItemType, CursorFlags.Multiple);

		    Assert.That(cursor.MoveNext(), Is.True);
		    Assert.That(cursor.LocalName,  Is.EqualTo(ItemType.LocalName));
		    Assert.That(cursor.Value,      Is.EqualTo("1"));
		    Assert.That(cursor.MoveNext(), Is.True);
		    Assert.That(cursor.LocalName,  Is.EqualTo(ItemType.LocalName));
		    Assert.That(cursor.Value,      Is.EqualTo("2"));
		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Test]
		public void Reset()
		{
		    var xml    = Xml("<X> <Item>1</Item> </X>");
		    var cursor = Cursor(xml, "Item", ItemType, CursorFlags.None);

		    Assert.That(cursor.MoveNext(), Is.True);
		    Assert.That(cursor.LocalName,  Is.EqualTo(ItemType.LocalName));
		    Assert.That(cursor.Value,      Is.EqualTo("1"));
		    Assert.That(cursor.MoveNext(), Is.False);

		    cursor.Reset();

		    Assert.That(cursor.MoveNext(), Is.True);
		    Assert.That(cursor.LocalName,  Is.EqualTo(ItemType.LocalName));
		    Assert.That(cursor.Value,      Is.EqualTo("1"));
		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Test]
		public void MoveToEnd()
		{
			var xml    = Xml("<X> <Item>1</Item> <Other>2</Other> </X>");
			var cursor = Cursor(xml, "Item|Other", ItemAndOtherType, CursorFlags.Multiple);

			cursor.MoveNext();
			cursor.MoveToEnd();

			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Test]
		public void MoveTo_NotXPathNode_Fails()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, "Item", ItemType, CursorFlags.Elements);

			Assert.Throws<InvalidOperationException>(() =>
				cursor.MoveTo(new DummyXmlNode()));
		}

		[Test]
		public void MoveTo_NotARecognizedNode_Fails()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, "Item", ItemType, CursorFlags.Elements);

			var wrongNode = new XPathNode(Xml("<Q/>"), typeof(object));

			Assert.Throws<InvalidOperationException>(() =>
				cursor.MoveTo(wrongNode));
		}

		[Test]
		public void MoveTo_RecognizedNode_Succeeds_ForElement()
		{
			var xml    = Xml("<X> <Item>1</Item> <Other>2</Other> </X>");
			var cursor = Cursor(xml, "Item|Other", ItemAndOtherType, CursorFlags.Elements | CursorFlags.Multiple);

			cursor.MoveNext();
			var node = cursor.Save();
			cursor.MoveToEnd();

			cursor.MoveTo(node);

			Assert.That(cursor.LocalName,  Is.EqualTo(ItemType.LocalName));
			Assert.That(cursor.Value,      Is.EqualTo("1"));
			Assert.That(cursor.MoveNext(), Is.True);
			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Test]
		public void MoveTo_RecognizedNode_Succeeds_ForAttribute()
		{
			var xml    = Xml("<X Item='1' Other='2'/>");
			var cursor = Cursor(xml, "@Item|@Other", ItemAndOtherType, CursorFlags.Attributes | CursorFlags.Multiple);

			cursor.MoveNext();
			cursor.MoveNext();
			var node = cursor.Save();
			cursor.MoveToEnd();

			cursor.MoveTo(node);

			Assert.That(cursor.LocalName,  Is.EqualTo(OtherType.LocalName));
			Assert.That(cursor.Value,      Is.EqualTo("2"));
			Assert.That(cursor.MoveNext(), Is.False);
		}

		protected static XPathNavigator Xml(params string[] xml)
		{
			var document = new XmlDocument();
			document.LoadXml(string.Concat(xml));
			return document.DocumentElement.CreateNavigator();
		}

		protected static XPathNode Node(params string[] xml)
		{
			return new XPathNode(Xml(xml), typeof(object));
		}

		protected static ICompiledPath Path(string path)
		{
			return new CompiledPath(path);
		}

		protected IXmlCursor Cursor(XPathNavigator parent, string pathText, IXmlTypeMap knownTypes, CursorFlags flags)
		{
			var lazyParent   = new DummyLazy<XPathNavigator> { Value = parent };
			var compiledPath = new CompiledPath(pathText);
			return Cursor(lazyParent, compiledPath, knownTypes, flags);
		}

		protected abstract IXmlCursor Cursor(ILazy<XPathNavigator> lazy, CompiledPath path, IXmlTypeMap knownTypes, CursorFlags flags);

		[TestFixtureSetUp]
		public void SetUp()
		{
			ItemType   = new XmlNamedType("Item",  null, typeof(_ItemClrType ));
			OtherType  = new XmlNamedType("Other", null, typeof(_OtherClrType));
			ItemAndOtherType = new XmlKnownTypeSet(typeof(object));
			ItemAndOtherType.Add(new XmlNamedType(ItemType .LocalName, null, typeof(_ItemClrType )));
			ItemAndOtherType.Add(new XmlNamedType(OtherType.LocalName, null, typeof(_OtherClrType)));
		}

		protected static XmlNamedType ItemType, OtherType;
		protected static XmlKnownTypeSet ItemAndOtherType;

		private class _ItemClrType  { }
		private class _OtherClrType { }
	}
}
