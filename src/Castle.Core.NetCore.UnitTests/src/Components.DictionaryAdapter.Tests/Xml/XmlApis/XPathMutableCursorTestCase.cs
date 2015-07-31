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
	using System.Xml.XPath;
	using Castle.Components.DictionaryAdapter;
	using Castle.Components.DictionaryAdapter.Tests;
	using Castle.Components.DictionaryAdapter.Xml;
	using Xunit;

		public class XPathReadableCursorTestCase : XPathCursorTestCase
	{
		[Fact]
		public void AsVirtual_WhenParentIsRealNode()
		{
			IXmlNode   root, node;
			IXmlCursor cursor;

			root = Node("<X/>");
			cursor = root.Select(Path("Item"), IncludedTypes, NamespaceSource.Instance, CursorFlags.Mutable);
			cursor.MoveNext();
			node = cursor;

			node.Value = "1";

			Assert.That(root.Xml, XmlEquivalent.To("<X> <Item>1</Item> </X>"));
		}

		[Fact]
		public void AsVirtual_WhenParentIsVirtualNode()
		{
			IXmlNode   root, node;
			IXmlCursor cursor;

			root = Node("<X/>");
			cursor = root.Select(Path("Item"), IncludedTypes, NamespaceSource.Instance, CursorFlags.Mutable);
			cursor.MoveNext();
			cursor = cursor.Select(Path("Other"), IncludedTypes, NamespaceSource.Instance, CursorFlags.Mutable);
			cursor.MoveNext();
			node = cursor;

			node.Value = "1";

			Assert.That(root.Xml, XmlEquivalent.To("<X> <Item><Other>1</Other></Item> </X>"));
		}

		[Fact]
		public void Iterate_ComplexPath_WhenPartialMatchesExist()
		{
		    var xml    = Xml("<X> <A><B/></A> </X>");
		    var cursor = Cursor(xml, "A/B/@C", CursorFlags.Multiple);

		    Assert.That(cursor.MoveNext(), Is.True);
		    Assert.That(cursor.Value,      Is.Empty);
			Assert.That(cursor.IsNil,      Is.False);
		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Bug_SaveAfterCreate()
		{
		    var xml    = Xml("<X/>");
		    var cursor = Cursor(xml, "A/B/@C", CursorFlags.Multiple);

		    Assert.That(cursor.MoveNext(), Is.False);
			cursor.Create(TypeA.ClrType);
			cursor.Save();
		}

		[Fact]
		public void Bug_CoerceAttributeInAWayThatRequiresXsiType()
		{
		    var xml    = Xml("<X/>");
		    var cursor = Cursor(xml, "A/B/@C", CursorFlags.Multiple);

		    Assert.That(cursor.MoveNext(), Is.False);
			cursor.Create(TypeA.ClrType);

			Assert.Throws<InvalidOperationException>(() =>
				cursor.Coerce(TypeB.ClrType));
		}

		[Fact]
		public void Create()
		{
		    var xml    = Xml("<X/>");
		    var cursor = Cursor(xml, "A[B='b']/C[D[E][F='f'] and G]/@H", CursorFlags.Multiple);

		    Assert.That(cursor.MoveNext(), Is.False);

			cursor.Create(TypeA.ClrType);
			cursor.Value = "h";

			Assert.That(xml, XmlEquivalent.To
			(
				"<X>",
					"<A>",
						"<C H='h'>",
							"<D> <E/> <F>f</F> </D>",
							"<G/>",
						"</C>",
						"<B>b</B>",
					"</A>",
				"</X>"
			));
		}

		[Fact]
		public void Create_WithVariable()
		{
		    var xml    = Xml("<X/>");
		    var cursor = Cursor(xml, "A[B=$test:v]/C", CursorFlags.Multiple);

		    Assert.That(cursor.MoveNext(), Is.False);

			cursor.Create(TypeA.ClrType);
			cursor.Value = "c";

			Assert.That(xml, XmlEquivalent.To
			(
				"<X>",
					"<A>",
						"<C>c</C>",
						"<B>VariableValue</B>",
					"</A>",
				"</X>"
			));
		}

		protected override IXmlCursor Cursor(IXmlNode parent, CompiledXPath path, IXmlIncludedTypeMap includedTypes, CursorFlags flags)
		{
			return new XPathMutableCursor(parent, path, includedTypes, NamespaceSource.Instance, flags);
		}
	}
}
#endif
