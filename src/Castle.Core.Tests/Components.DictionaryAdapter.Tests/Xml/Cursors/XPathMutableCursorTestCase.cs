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
	using System.Xml.XPath;
	using Castle.Components.DictionaryAdapter;
	using Castle.Components.DictionaryAdapter.Tests;
	using Castle.Components.DictionaryAdapter.Xml;
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
			cursor = root.Select(Path("Item"), KnownTypes, CursorFlags.Mutable);
			cursor.MoveNext();
			node = cursor;

			node.Value = "1";

			Assert.That(root.Xml, XmlEquivalent.To("<X> <Item>1</Item> </X>"));
		}

		[Test]
		public void AsVirtual_WhenParentIsVirtualNode()
		{
			IXmlNode   root, node;
			IXmlCursor cursor;

			root = Node("<X/>");
			cursor = root.Select(Path("Item"), KnownTypes, CursorFlags.Mutable);
			cursor.MoveNext();
			cursor = cursor.Select(Path("Other"), KnownTypes, CursorFlags.Mutable);
			cursor.MoveNext();
			node = cursor;

			node.Value = "1";

			Assert.That(root.Xml, XmlEquivalent.To("<X> <Item><Other>1</Other></Item> </X>"));
		}

		protected override IXmlCursor Cursor(ILazy<XPathNavigator> parent, CompiledPath path, IXmlTypeMap knownTypes, CursorFlags flags)
		{
			return new XPathMutableCursor(parent, path, knownTypes, flags);
		}
	}
}
