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
	using System.Xml.XPath;

	using Castle.Components.DictionaryAdapter.Tests;

	using NUnit.Framework;

	[TestFixture]
	public class XPathReadableCursorTestCase : XPathCursorTestCase
	{
		[Test]
		public void AsVirtual_WhenParentIsRealNode()
		{
			IXmlNode   root, node;
			IXmlCursor cursor;

			root = Node("<X/>");
			cursor = root.Select(Path("Item"), IncludedTypes, NamespaceSource.Instance, CursorFlags.Mutable);
			cursor.MoveNext();
			node = cursor;

			node.Value = "1";

			CustomAssert.AreXmlEquivalent("<X> <Item>1</Item> </X>", root.Xml);
		}

		[Test]
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

			CustomAssert.AreXmlEquivalent("<X> <Item><Other>1</Other></Item> </X>", root.Xml);
		}

		[Test]
		public void Iterate_ComplexPath_WhenPartialMatchesExist()
		{
		    var xml    = Xml("<X> <A><B/></A> </X>");
		    var cursor = Cursor(xml, "A/B/@C", CursorFlags.Multiple);

			Assert.True(cursor.MoveNext());
			Assert.IsEmpty(cursor.Value);
			Assert.False(cursor.IsNil);
			Assert.False(cursor.MoveNext());
		}

		[Test]
		public void Bug_SaveAfterCreate()
		{
		    var xml    = Xml("<X/>");
		    var cursor = Cursor(xml, "A/B/@C", CursorFlags.Multiple);

			Assert.False(cursor.MoveNext());
			cursor.Create(TypeA.ClrType);
			cursor.Save();
		}

		[Test]
		public void Bug_CoerceAttributeInAWayThatRequiresXsiType()
		{
		    var xml    = Xml("<X/>");
		    var cursor = Cursor(xml, "A/B/@C", CursorFlags.Multiple);

			Assert.False(cursor.MoveNext());
			cursor.Create(TypeA.ClrType);

			Assert.Throws<InvalidOperationException>(() =>
				cursor.Coerce(TypeB.ClrType));
		}

		[Test]
		public void Create()
		{
		    var xml    = Xml("<X/>");
		    var cursor = Cursor(xml, "A[B='b']/C[D[E][F='f'] and G]/@H", CursorFlags.Multiple);

			Assert.False(cursor.MoveNext());

			cursor.Create(TypeA.ClrType);
			cursor.Value = "h";

			CustomAssert.AreXmlEquivalent(string.Concat
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
			), xml);
		}

		[Test]
		public void Create_WithVariable()
		{
		    var xml    = Xml("<X/>");
		    var cursor = Cursor(xml, "A[B=$test:v]/C", CursorFlags.Multiple);

			Assert.False(cursor.MoveNext());

			cursor.Create(TypeA.ClrType);
			cursor.Value = "c";

			CustomAssert.AreXmlEquivalent(string.Concat
			(
				"<X>",
					"<A>",
						"<C>c</C>",
						"<B>VariableValue</B>",
					"</A>",
				"</X>"
			), xml);
		}

		protected override IXmlCursor Cursor(IXmlNode parent, CompiledXPath path, IXmlIncludedTypeMap includedTypes, CursorFlags flags)
		{
			return new XPathMutableCursor(parent, path, includedTypes, NamespaceSource.Instance, flags);
		}
	}
}
