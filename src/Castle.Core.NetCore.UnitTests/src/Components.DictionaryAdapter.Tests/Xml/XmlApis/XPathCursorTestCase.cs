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
	using System.Xml.XPath;
	using Castle.Components.DictionaryAdapter.Xml;
	using Xunit;

		public abstract class XPathCursorTestCase
	{
		[Fact]
		public void Iterate_WhenEmpty()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, "A", CursorFlags.None);

			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_WhenAtEnd()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, "A", CursorFlags.None);
			cursor.MoveNext();

			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_Element_WhenNoMatchExists()
		{
		    var xml    = Xml("<X A='?'> foo <Q/> bar </X>");
		    var cursor = Cursor(xml, "A", CursorFlags.None);

		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_Element_WhenOneMatchExists()
		{
		    var xml    = Xml("<X A='?'> <Q/> <A>1</A> <Q/> </X>");
		    var cursor = Cursor(xml, "A", CursorFlags.None);

		    Assert.That(cursor.MoveNext(), Is.True);
		    Assert.That(cursor.Name.LocalName,  Is.EqualTo("A"));
		    Assert.That(cursor.Value,      Is.EqualTo("1"));
		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_Element_WhenMultipleMatchesExist_InSingleMode()
		{
		    var xml    = Xml("<X A='?'> <Q/> <A>1</A> <Q/> <A>2</A> <Q/> </X>");
		    var cursor = Cursor(xml, "A", CursorFlags.None);

		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_Element_WhenMultipleMatchesExist_InMultipleMode()
		{
		    var xml    = Xml("<X A='?'> <Q/> <A>1</A> <Q/> <A>2</A> <Q/> </X>");
		    var cursor = Cursor(xml, "A", CursorFlags.Multiple);

		    Assert.That(cursor.MoveNext(), Is.True);
		    Assert.That(cursor.Name.LocalName,  Is.EqualTo("A"));
		    Assert.That(cursor.Value,      Is.EqualTo("1"));
		    Assert.That(cursor.MoveNext(), Is.True);
		    Assert.That(cursor.Name.LocalName,  Is.EqualTo("A"));
		    Assert.That(cursor.Value,      Is.EqualTo("2"));
		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_Attribute_WhenNoMatchExists()
		{
		    var xml    = Xml("<X Q='?'> <A>?</A> </X>");
		    var cursor = Cursor(xml, "@A", CursorFlags.None);

		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_Attribute_WhenOneMatchExists()
		{
		    var xml    = Xml("<X Q='?' A='1' R='?'> <A>?</A> </X>");
		    var cursor = Cursor(xml, "@A", CursorFlags.None);

		    Assert.That(cursor.MoveNext(), Is.True);
		    Assert.That(cursor.Name.LocalName,  Is.EqualTo("A"));
		    Assert.That(cursor.Value,      Is.EqualTo("1"));
		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_ComplexPath_WhenNoMatchExists()
		{
		    var xml    = Xml("<X A='?'> foo <Q/> bar </X>");
		    var cursor = Cursor(xml, "A/B/@C", CursorFlags.None);

		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_ComplexPath_WhenOneMatchExists()
		{
		    var xml    = Xml("<X A='?'> <Q/> <A><B C='1'/></A> <Q/> </X>");
		    var cursor = Cursor(xml, "A/B/@C", CursorFlags.None);

		    Assert.That(cursor.MoveNext(), Is.True);
		    Assert.That(cursor.Name.LocalName,  Is.EqualTo("C"));
		    Assert.That(cursor.Value,      Is.EqualTo("1"));
		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_ComplexPath_WhenMultipleMatchesExist_InSingleMode()
		{
		    var xml    = Xml("<X A='?'> <Q/> <A><B C='1'/></A> <Q/> <A><B C='2'/></A> <Q/> </X>");
		    var cursor = Cursor(xml, "A/B/@C", CursorFlags.None);

		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Iterate_ComplexPath_WhenMultipleMatchesExist_InMultipleMode()
		{
		    var xml    = Xml("<X A='?'> <Q/> <A><B C='1'/></A> <Q/> <A><B C='2'/></A> <Q/> </X>");
		    var cursor = Cursor(xml, "A/B/@C", CursorFlags.Multiple);

		    Assert.That(cursor.MoveNext(), Is.True);
		    Assert.That(cursor.Name.LocalName,  Is.EqualTo("C"));
		    Assert.That(cursor.Value,      Is.EqualTo("1"));
		    Assert.That(cursor.MoveNext(), Is.True);
		    Assert.That(cursor.Name.LocalName,  Is.EqualTo("C"));
		    Assert.That(cursor.Value,      Is.EqualTo("2"));
		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void Reset()
		{
		    var xml    = Xml("<X> <A>1</A> </X>");
		    var cursor = Cursor(xml, "A", CursorFlags.Multiple);

		    Assert.That(cursor.MoveNext(), Is.True);
		    Assert.That(cursor.Name.LocalName,  Is.EqualTo("A"));
		    Assert.That(cursor.Value,      Is.EqualTo("1"));
		    Assert.That(cursor.MoveNext(), Is.False);

		    cursor.Reset();

		    Assert.That(cursor.MoveNext(), Is.True);
		    Assert.That(cursor.Name.LocalName,  Is.EqualTo("A"));
		    Assert.That(cursor.Value,      Is.EqualTo("1"));
		    Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void MoveToEnd()
		{
			var xml    = Xml("<X> <A>1</A> <A>2</A> </X>");
			var cursor = Cursor(xml, "A", CursorFlags.Multiple);

			cursor.MoveNext();
			cursor.MoveToEnd();

			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void MoveTo_NotXPathNode_Fails()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, "A", CursorFlags.Multiple);

			Assert.Throws<InvalidOperationException>(() =>
				cursor.MoveTo(new DummyXmlNode()));
		}

		[Fact]
		public void MoveTo_NotARecognizedNode_Fails()
		{
			var xml    = Xml("<X/>");
			var cursor = Cursor(xml, "A", CursorFlags.Multiple);

			var wrongNode = new XPathNode(Xml("<Q/>"), typeof(object), NamespaceSource.Instance);

			Assert.Throws<InvalidOperationException>(() =>
				cursor.MoveTo(wrongNode));
		}

		[Fact]
		public void MoveTo_RecognizedNode_Succeeds_ForElement()
		{
			var xml    = Xml("<X> <A>1</A> <A>2</A> </X>");
			var cursor = Cursor(xml, "A", CursorFlags.Multiple);

			cursor.MoveNext();
			cursor.MoveNext();
			var node = cursor.Save();
			cursor.MoveToEnd();

			cursor.MoveTo(node);

			Assert.That(cursor.Name.LocalName,  Is.EqualTo("A"));
			Assert.That(cursor.Value,      Is.EqualTo("2"));
			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void MoveTo_RecognizedNode_Succeeds_ForAttribute()
		{
			var xml    = Xml("<X A='1'/>");
			var cursor = Cursor(xml, "@A", CursorFlags.None);

			cursor.MoveNext();
			var node = cursor.Save();
			cursor.MoveToEnd();

			cursor.MoveTo(node);

			Assert.That(cursor.Name.LocalName,  Is.EqualTo("A"));
			Assert.That(cursor.Value,      Is.EqualTo("1"));
			Assert.That(cursor.MoveNext(), Is.False);
		}

		[Fact]
		public void LocalNameAndNamespace_EmptyNamespace()
		{
			var xml    = Xml("<X> <A>1</A> </X>");
			var cursor = Cursor(xml, "A", CursorFlags.None);

			Assert.That(cursor.MoveNext(),   Is.True);
			Assert.That(cursor.Name.LocalName,    Is.EqualTo("A"));
			Assert.That(cursor.Name.NamespaceUri, Is.EqualTo(string.Empty));
		}

		[Fact]
		public void LocalNameAndNamespace_DefaultNamespace()
		{
			var xml    = Xml("<X xmlns='ns'> <A>1</A> </X>");
			var cursor = Cursor(xml, "p:A", CursorFlags.None);

			Assert.That(cursor.MoveNext(),   Is.True);
			Assert.That(cursor.Name.LocalName,    Is.EqualTo("A"));
			Assert.That(cursor.Name.NamespaceUri, Is.EqualTo("ns"));
		}

		[Fact]
		public void LocalNameAndNamespace_PrefixedNamespace()
		{
			var xml    = Xml("<X xmlns:n='ns'> <n:A>1</n:A> </X>");
			var cursor = Cursor(xml, "p:A", CursorFlags.None);

			Assert.That(cursor.MoveNext(),   Is.True);
			Assert.That(cursor.Name.LocalName,    Is.EqualTo("A"));
			Assert.That(cursor.Name.NamespaceUri, Is.EqualTo("ns"));
		}

		protected static XPathNavigator Xml(params string[] xml)
		{
			var document = new XmlDocument();
			document.LoadXml(string.Concat(xml));
			return document.DocumentElement.CreateNavigator();
		}

		protected static XPathNode Node(params string[] xml)
		{
			return new XPathNode(Xml(xml), typeof(object), NamespaceSource.Instance);
		}

		protected static CompiledXPath Path(string path)
		{
			return XPathCompiler.Compile(path);
		}

		protected IXmlCursor Cursor(XPathNavigator parent, string pathText, CursorFlags flags)
		{
			var parentNode = new XPathNode(parent, typeof(object), NamespaceSource.Instance);
			var compiledPath = XPathCompiler.Compile(pathText);
			compiledPath.SetContext(Context);
			return Cursor(parentNode, compiledPath, IncludedTypes, flags);
		}

		protected abstract IXmlCursor Cursor(IXmlNode parent, CompiledXPath path, IXmlIncludedTypeMap includedTypes, CursorFlags flags);

		[TestFixtureSetUp]
		public virtual void OneTimeSetUp()
		{
			if (IncludedTypes == null)
			{
				IncludedTypes = new MockXmlIncludedTypeMap();
				IncludedTypes.DefaultClrType = TypeA.ClrType;
				IncludedTypes.InnerSet.Add(TypeA);
				IncludedTypes.InnerSet.Add(TypeB);
			}

			if (Context == null)
			{
				Context = new XmlContextBase();
				Context.AddNamespace("p", "ns");
				Context.AddVariable("test", "v", new MockXPathVariable("VariableValue"));
				Context.AddFunction("test", "f", new MockXPathFunction("FunctionValue"));
			}
		}

		protected static MockXmlIncludedTypeMap IncludedTypes;
		protected static readonly XmlIncludedType
			TypeA = new XmlIncludedType("a", null, typeof(_TypeA)),
			TypeB = new XmlIncludedType("b", null, typeof(_TypeB));

		protected static XmlContextBase Context;

		private class _TypeA          { }
		private class _TypeB : _TypeA { }
	}
}
#endif
