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
	using NUnit.Framework;

	[TestFixture]
	public class XPathMutableCursorTestCase : XPathCursorTestCase
	{
		[Test]
		public void Iterate_AllNodes_WhenNoMatchExists()
		{
		    var xml    = Xml("<X Q='?'> foo <Q/> bar </X>");
		    var cursor = Cursor(xml, "@A|A", CursorFlags.None);

		    Assert.False(cursor.MoveNext());
		}

		[Test]
		public void Iterate_AllNodes_WhenOneMatchExists_AsAttribute()
		{
		    var xml    = Xml("<X Q='?' A='1' R='?'> <Q/> </X>");
		    var cursor = Cursor(xml, "@A|A", CursorFlags.None);

		    Assert.True(cursor.MoveNext());
		    Assert.AreEqual("A", cursor.Name.LocalName);
		    Assert.AreEqual("1", cursor.Value);
		    Assert.False(cursor.MoveNext());
		}

		[Test]
		public void Iterate_AllNodes_WhenOneMatchExists_AsElement()
		{
		    var xml    = Xml("<X Q='?'> <Q/> <A>1</A> <Q/> </X>");
		    var cursor = Cursor(xml, "@A|A", CursorFlags.None);

		    Assert.True(cursor.MoveNext());
		    Assert.AreEqual("A", cursor.Name.LocalName);
		    Assert.AreEqual("1", cursor.Value);
		    Assert.False(cursor.MoveNext());
		}

		[Test]
		public void Iterate_AllNodes_WhenMultipleMatchesExist_InSingleMode()
		{
		    var xml    = Xml("<X Q='?' A='1' R='?'> <Q/> <A>2</A> <Q/> </X>");
		    var cursor = Cursor(xml, "@A|A", CursorFlags.None);

		    Assert.False(cursor.MoveNext());
		}

		[Test]
		public void Iterate_AllNodes_WhenMultipleMatchesExist_InMultipleMode()
		{
		    var xml    = Xml("<X Q='?' A='1' R='?'> <Q/> <A>2</A> <Q/> </X>");
		    var cursor = Cursor(xml, "@A|A", CursorFlags.Multiple);

		    Assert.True(cursor.MoveNext());
		    Assert.AreEqual("A", cursor.Name.LocalName);
		    Assert.AreEqual("1", cursor.Value);
		    Assert.True(cursor.MoveNext());
		    Assert.AreEqual("A", cursor.Name.LocalName);
		    Assert.AreEqual("2", cursor.Value);
		    Assert.False(cursor.MoveNext());
		}

		protected override IXmlCursor Cursor(IXmlNode parent, CompiledXPath path, IXmlIncludedTypeMap includedTypes, CursorFlags flags)
		{
			return new XPathReadOnlyCursor(parent, path, includedTypes, NamespaceSource.Instance, flags);
		}
	}
}
