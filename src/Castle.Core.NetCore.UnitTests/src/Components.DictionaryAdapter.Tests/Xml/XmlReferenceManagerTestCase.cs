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
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
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

    public static class ReferenceManagerTestCase
    {
	    		public class WithValidXml : TestScenario
		{
			protected override string GetXmlText()
			{
				return string.Concat
				(
					"<Root xmlns:x='urn:schemas-castle-org:xml-reference'>",
						"<A x:id='1'>",
							"<Value>a</Value>",
						"</A>",
						"<D>",  // Put this out of order to exercise deferred reference loading; // TODO: Rename stuff so it's in order
							"<Y x:ref='1'/>",
							"<Z x:ref='2'/>",
						"</D>",
						"<B>",
							"<X>",
								"<Value>b1</Value>",
							"</X>",
							"<X x:id='2'>",
								"<Value>b2</Value>",
							"</X>",
							"<X>",
								"<Value>b3</Value>",
							"</X>",
						"</B>",
						"<C>",
							"<Value>c</Value>",
						"</C>",
						"<E/>",
					"</Root>"
				);
			}

			[Fact]
			public void Get_IrrelevantNode()
			{
				var cursorC = SelectChild(Node, "C");
				var node    = cursorC as IXmlNode;
				var value   = null    as object;
				object token;

				var proceed = Manager.OnGetStarting(ref node, ref value, out token);

				Assert.That(proceed, Is.True);
				Assert.That(node,    Is.SameAs(cursorC));
				Assert.That(value,   Is.Null);
				Assert.That(token,   Is.Not.Null);

				Manager.OnGetCompleted(node, ValueA, token);
			}

			[Fact]
			public void Get_ValueType()
			{
				var cursorA = SelectChild(Node, "A", typeof(int));
				var node    = cursorA as IXmlNode;
				var value   = null    as object;
				object token;

				var proceed = Manager.OnGetStarting(ref node, ref value, out token);

				Assert.That(proceed, Is.True);
				Assert.That(node,    Is.SameAs(cursorA));
				Assert.That(value,   Is.Null);
				Assert.That(token,   Is.Null);
			}

			[Fact]
			public void Get_IdentityNode()
			{
				var cursorA = SelectChild(Node, "A");
				var node    = cursorA as IXmlNode;
				var value   = null    as object;
				object token;

				var proceed = Manager.OnGetStarting(ref node, ref value, out token);

				Assert.That(proceed, Is.True);
				Assert.That(node,    Is.SameAs(cursorA));
				Assert.That(value,   Is.Null);
				Assert.That(token,   Is.Not.Null);

				Manager.OnGetCompleted(node, ValueA, token);
			}

			[Fact]
			public void Get_IdentityNode_Again()
			{
				Get_IdentityNode();

				var cursorA = SelectChild(Node, "A");
				var node    = cursorA as IXmlNode;
				var value   = null    as object;
				object token;

				var proceed = Manager.OnGetStarting(ref node, ref value, out token);

				Assert.That(proceed, Is.False);
				Assert.That(node,    Is.SameAs(cursorA));
				Assert.That(value,   Is.EqualTo(ValueA));
				Assert.That(token,   Is.Null);

				// Shouldn't call this!  Here only to exercise the code path.
				Manager.OnGetCompleted(node, value, token);
		}

			[Fact]
			public void Get_IdentityNode_ThenReferencingNodeOfSameType()
			{
				Get_IdentityNode();

				var cursorD = SelectChild(Node,    "D");
				var cursorY = SelectChild(cursorD, "Y");
				var node    = cursorY as IXmlNode;
				var value   = null    as object;
				object token;

				var proceed = Manager.OnGetStarting(ref node, ref value, out token);

				Assert.That(proceed, Is.False);
				Assert.That(node,    Is.Not.SameAs(cursorY));
				Assert.That(value,   Is.EqualTo(ValueA));
				Assert.That(token,   Is.Null);
			}

			[Fact]
			public void Get_IdentityNode_ThenReferencingNodeOfDifferentType()
			{
				Get_IdentityNode();

				var cursorD = SelectChild(Node,    "D");
				var cursorY = SelectChild(cursorD, "Y", typeof(TY));
				var node    = cursorY as IXmlNode;
				var value   = null    as object;
				object token;

				var proceed = Manager.OnGetStarting(ref node, ref value, out token);

				Assert.That(proceed, Is.True);
				Assert.That(node,    Is.Not.SameAs(cursorY));
				Assert.That(value,   Is.Null);
				Assert.That(token,   Is.Not.Null);

				Manager.OnGetCompleted(node, OtherA, token);
			}

			[Fact]
			public void Get_IdentityNode_ThenReferencingNodeOfDifferentType_Again()
			{
				Get_IdentityNode_ThenReferencingNodeOfDifferentType();

				var cursorD = SelectChild(Node,    "D");
				var cursorY = SelectChild(cursorD, "Y", typeof(TY));
				var node    = cursorY as IXmlNode;
				var value   = null    as object;
				object token;

				var proceed = Manager.OnGetStarting(ref node, ref value, out token);

				Assert.That(proceed, Is.False);
				Assert.That(node,    Is.Not.SameAs(cursorY));
				Assert.That(value,   Is.SameAs(OtherA));
				Assert.That(token,   Is.Null);
			}

			[Fact]
			public void Set_ValueType()
			{
				var cursorC = SelectChild(Node, "C", typeof(int));
				var node    = cursorC as IXmlNode;
				var value   = 42 as object;
				object token;

				var proceed = Manager.OnAssigningValue(node, null, ref value, out token);

				Assert.That(proceed, Is.True);
				Assert.That(value,   Is.EqualTo(42));
				Assert.That(token,   Is.Null);
			}
		}

		[TestFixture]
		public class SetValueScenario : TestScenario
		{
			protected override string GetXmlText()
			{
				return "<Root $x> <A/> <B/> <C/> </Root>";
			}

			[Fact]
			public void Set_Primary()
			{
				SetValue(Node, "A", null, ValueA, SetResult.ProceedTracked);

				Assert.That(Document, XmlEquivalent.To(OriginalXml));
			}

			[Fact]
			public void Set_Primary_Again_SameValue()
			{
				SetValue(Node, "A", null,   ValueA, SetResult.ProceedTracked);
				SetValue(Node, "A", ValueA, ValueA, SetResult.Return);

				Assert.That(Document, XmlEquivalent.To(OriginalXml));
			}

			[Fact]
			public void Set_Primary_Again_DifferentValue()
			{
				SetValue(Node, "A", null,   ValueA, SetResult.ProceedTracked);
				SetValue(Node, "A", ValueA, OtherA, SetResult.ProceedTracked);

				Assert.That(Document, XmlEquivalent.To(OriginalXml));
			}

			[Fact]
			public void Set_Reference()
			{
				SetValue(Node, "A", null, ValueA, SetResult.ProceedTracked);
				SetValue(Node, "B", null, ValueA, SetResult.Return);

				Assert.That(Document, XmlEquivalent.To(
					Xml("<Root $x> <A x:id='1'/> <B x:ref='1'/> <C/> </Root>")
				));
			}

			[Fact]
			public void Set_Reference_Again_SameNode()
			{
				SetValue(Node, "A", null,   ValueA, SetResult.ProceedTracked);
				SetValue(Node, "B", null,   ValueA, SetResult.Return);
				SetValue(Node, "B", ValueA, ValueA, SetResult.Return);

				Assert.That(Document, XmlEquivalent.To(
					Xml("<Root $x> <A x:id='1'/> <B x:ref='1'/> <C/> </Root>")
				));
			}

			[Fact]
			public void Set_Reference_Again_DifferentNode()
			{
				SetValue(Node, "A", null, ValueA, SetResult.ProceedTracked);
				SetValue(Node, "B", null, ValueA, SetResult.Return);
				SetValue(Node, "C", null, ValueA, SetResult.Return);

				Assert.That(Document, XmlEquivalent.To(
					Xml("<Root $x> <A x:id='1'/> <B x:ref='1'/> <C x:ref='1'/> </Root>")
				));
			}

			[Fact]
			public void Reset_Primary()
			{
				SetValue(Node, "A", null,   ValueA, SetResult.ProceedTracked);
				SetValue(Node, "B", null,   ValueA, SetResult.Return);
				SetValue(Node, "C", null,   ValueA, SetResult.Return);
				SetValue(Node, "A", ValueA, OtherA, SetResult.ProceedTracked);

				Assert.That(Document, XmlEquivalent.To(
					Xml("<Root $x> <A/> <B x:id='1'/> <C x:ref='1'/> </Root>")
				));
			}

			[Fact]
			public void Reset_Reference()
			{
				SetValue(Node, "A", null,   ValueA, SetResult.ProceedTracked);
				SetValue(Node, "B", null,   ValueA, SetResult.Return);
				SetValue(Node, "C", null,   ValueA, SetResult.Return);
				SetValue(Node, "B", ValueA, OtherA, SetResult.ProceedTracked);

				Assert.That(Document, XmlEquivalent.To(
					Xml("<Root $x> <A x:id='1'/> <B/> <C x:ref='1'/> </Root>")
				));
			}

			[Fact]
			public void Reset_AllButOne()
			{
				SetValue(Node, "A", null,   ValueA, SetResult.ProceedTracked);
				SetValue(Node, "B", null,   ValueA, SetResult.Return);
				SetValue(Node, "C", null,   ValueA, SetResult.Return);
				SetValue(Node, "A", ValueA, OtherA, SetResult.ProceedTracked);
				SetValue(Node, "B", ValueA, OtherB, SetResult.ProceedTracked);

				Assert.That(Document, XmlEquivalent.To(OriginalXml));
			}

			[Fact]
			public void Reset_All()
			{
				SetValue(Node, "A", null,   ValueA, SetResult.ProceedTracked);
				SetValue(Node, "B", null,   ValueA, SetResult.Return);
				SetValue(Node, "C", null,   ValueA, SetResult.Return);
				SetValue(Node, "A", ValueA, OtherA, SetResult.ProceedTracked);
				SetValue(Node, "B", ValueA, OtherB, SetResult.ProceedTracked);
				SetValue(Node, "C", ValueA, OtherC, SetResult.ProceedTracked);

				Assert.That(Document, XmlEquivalent.To(OriginalXml));
			}

			[Fact]
			public void TryGet_TrackedObject()
			{
				Set_Primary();

				object value;
				Assert.That(Manager.TryGet(ValueA, out value), Is.True);
				Assert.That(value, Is.SameAs(ValueA));
			}

			[Fact]
			public void TryGet_UntrackedObject()
			{
				Set_Primary();

				object value;
				Assert.That(Manager.TryGet(OtherA, out value), Is.False);
				Assert.That(value, Is.Null);
			}
		}

		[TestFixture]
		public class NodeCopyScenario1 : TestScenario
		{
			protected override string GetXmlText()
			{
				return string.Concat
				(
					"<Root $x>",
						"<A C='c'> d <E>f</E> g </A>",
						"<B/>",
					"</Root>"
				);
			}

			[Fact]
			public void Reset_Primary()
			{
				SetValue(Node, "A", null,   ValueA, SetResult.ProceedTracked);
				SetValue(Node, "B", null,   ValueA, SetResult.Return);
				SetValue(Node, "A", ValueA, OtherA, SetResult.ProceedTracked);

				Assert.That(Document, XmlEquivalent.To(
					Xml("<Root $x> <A/> <B C='c'> d <E>f</E> g </B> </Root>")
				));
			}
		}

		[TestFixture]
		public class NodeCopyScenario2 : TestScenario
		{
			protected override string GetXmlText()
			{
				return string.Concat
				(
					"<Root $x>",
						"<A C='c'> d <E>f</E> g </A>",
						"<B/>",
						"<C/>",
					"</Root>"
				);
			}

			[Fact]
			public void Reset_Primary()
			{
				SetValue(Node, "A", null,   ValueA, SetResult.ProceedTracked);
				SetValue(Node, "B", null,   ValueA, SetResult.Return);
				SetValue(Node, "C", null,   ValueA, SetResult.Return);
				SetValue(Node, "A", ValueA, OtherA, SetResult.ProceedTracked);

				Assert.That(Document, XmlEquivalent.To(
					Xml("<Root $x> <A/> <B C='c' x:id='1'> d <E>f</E> g </B> <C x:ref='1'/> </Root>")
				));
			}
		}

		public abstract class TestScenario
		{
			protected abstract string     GetXmlText  ();
			protected string              OriginalXml { get; private set; }
			protected XmlDocument         Document    { get; private set; }
			protected IXmlNamespaceSource Namespaces  { get; private set; }
			protected IXmlNode            Node        { get; private set; }
			protected XmlReferenceManager Manager     { get; private set; }

			protected static readonly object
				ValueA = new TX(),
				OtherA = new TX(),
				OtherB = new TX(),
				OtherC = new TX();

			[SetUp]
			public void SetUp()
			{
				OriginalXml = GetXmlText();
				Document    = Xml(OriginalXml);
				OriginalXml = Document.OuterXml;
				Namespaces  = new XmlContextBase();
				Node        = new SysXmlNode(Document.DocumentElement, typeof(object), Namespaces);
				Manager     = new XmlReferenceManager(Node, DefaultXmlReferenceFormat.Instance);
			}

			protected void SetValue(IXmlNode parentNode, string childName, object oldValue, object newValue, SetResult expectedResult)
			{
				var cursor = SelectChild(parentNode, childName);
				var node   = cursor as IXmlNode;
				var value  = newValue as object;
				object token;

				var proceed = Manager.OnAssigningValue(node, oldValue, ref value, out token);

				switch (expectedResult)
				{
					case SetResult.ProceedUntracked:
						Assert.That(proceed, Is.True);
						Assert.That(value,   Is.EqualTo(newValue));
						Assert.That(token,   Is.Null);
						break;

					case SetResult.ProceedTracked:
						Assert.That(proceed, Is.True);
						Assert.That(value,   Is.EqualTo(newValue));
						Assert.That(token,   Is.Not.Null);
						Manager.OnAssignedValue(node, value, value, token);
						break;

					case SetResult.Return:
						Assert.That(proceed, Is.False);
						Assert.That(value,   Is.EqualTo(newValue));
						Assert.That(token,   Is.Null);
						break;
				}
			}

			protected enum SetResult
			{
				ProceedUntracked,
				ProceedTracked,
				Return
			}

			protected IXmlCursor SelectChild(IXmlNode node, string name)
			{
				return SelectChild(node, name, typeof(TX));
			}

			protected IXmlCursor SelectChild(IXmlNode node, string name, Type type)
			{
				var knownTypes = new XmlKnownTypeSet(type);
				var knownType  = new XmlKnownType(name, null, null, null, type);
				knownTypes.Add(knownType, true);

				var cursor = node.SelectChildren(knownTypes, Namespaces, CursorFlags.Elements);
				Assert.That(cursor.MoveNext(), Is.True);

				return cursor;
			}

			protected static XmlDocument Xml(params string[] xml)
			{
				var document = new XmlDocument();

				var text = string.Concat(xml)
					.Replace("$x", "xmlns:x='urn:schemas-castle-org:xml-reference'");

				document.LoadXml(text);
				return document;
			}
		}

		public sealed class TX { }
		public sealed class TY { }
    }
}
#endif
