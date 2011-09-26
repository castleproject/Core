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
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.Components.DictionaryAdapter.Xml.Tests
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Linq;
	using System.Xml;
	using System.Xml.Serialization;
	using System.Xml.XPath;
	using Castle.Components.DictionaryAdapter.Tests;
	using NUnit.Framework;

	public class DefaultBehaviorTestCase
	{
		[TestFixture]
		public class SimpleProperty : XmlAdapterTestCase
		{
			public interface IFoo : IDictionaryAdapter
			{
				string A { get; set; }
			}

			[Test]
			public void Get_Element()
			{
				var foo = Create<IFoo>("<Foo> <A>a</A> </Foo>");

				Assert.That(foo.A, Is.EqualTo("a"));
			}

			[Test]
			public void Get_Element_WithExpectedXsiType()
			{
				var foo = Create<IFoo>("<Foo $xsd $xsi> <A xsi:type='xsd:string'>a</A> </Foo>");

				Assert.That(foo.A, Is.EqualTo("a"));
			}

			[Test]
			public void Get_Element_WithUnexpectedXsiType()
			{
				var foo = Create<IFoo>("<Foo $xsi> <A xsi:type='unexpected'>a</A> </Foo>");

				Assert.That(foo.A, Is.Null);
			}

			[Test]
			public void Get_Attribute()
			{
				var foo = Create<IFoo>("<Foo A='a'/>");

				Assert.That(foo.A, Is.EqualTo("a"));
			}

			[Test]
			public void Set()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.A = "a";

				Assert.That(xml, XmlEquivalent.To("<Foo> <A>a</A> </Foo>"));
			}
		}

		[TestFixture]
		public class ComplexProperty : XmlAdapterTestCase
		{
			public interface IFoo : IDictionaryAdapter
			{
				IBar A { get; set; }
			}

			public interface IBar : IDictionaryAdapter
			{
				string B { get; set; }
			}

            [Test]
            public void Get_Element()
            {
                var foo = Create<IFoo>("<Foo> <A> <B>b</B> </A> </Foo>");

				Assert.That(foo.A,   Is.Not.Null);
                Assert.That(foo.A.B, Is.EqualTo("b"));
            }

            [Test]
            public void Get_Attribute()
            {
				var xml = Xml("<Foo A='a'/>");
                var foo = Create<IFoo>(xml);

				Assert.That(foo.A,   Is.Not.Null);
                Assert.That(foo.A.B, Is.Null);
				Assert.That(xml,     XmlEquivalent.To("<Foo A='a'/>"));
            }

            [Test]
            public void Get_Missing()
            {
				var xml = Xml("<Foo/>");
                var foo = Create<IFoo>(xml);

				Assert.That(foo.A,   Is.Not.Null);
                Assert.That(foo.A.B, Is.Null);
				Assert.That(xml,     XmlEquivalent.To("<Foo/>"));
            }

            [Test]
            public void Get_ReturnsSameInstance()
            {
                var foo = Create<IFoo>("<Foo> <A> <B>b</B> </A> </Foo>");

				var instanceA = foo.A;
				var instanceB = foo.A;

                Assert.That(instanceA, Is.SameAs(instanceB),
					"Same component must be returned from successive calls.");
            }

            [Test]
            public void Set()
            {
                var xmlA = Xml("<Foo/>");
				var xmlB = Xml("<Foo> <A> <B>b</B> </A> </Foo>");
                var fooA = Create<IFoo>(xmlA);
				var fooB = Create<IFoo>(xmlB);

                fooA.A = fooB.A;

                Assert.That(xmlA, XmlEquivalent.To(xmlB));
            }

            [Test]
            public void SetPropertyOnVirtual()
            {
                var xml = Xml("<Foo/>");
                var foo = Create<IFoo>(xml);

                foo.A.B = "b";

                Assert.That(xml, XmlEquivalent.To("<Foo> <A> <B>b</B> </A> </Foo>"));
            }
		}

		[TestFixture]
		public class Collection : XmlAdapterTestCase
		{
			public interface IFoo : IDictionaryAdapter
			{
				int[] A { get; set; }
			}

			public int[] Items = { 1, 2 };

            [Test]
            public void Get_Element()
            {
                var foo = Create<IFoo>("<Foo> <A> <int>1</int> <int>2</int> </A> </Foo>");

                Assert.That(foo.A, Is.EquivalentTo(Items));
            }

            [Test]
            public void Get_Attribute()
            {
				var xml = Xml("<Foo A='a'/>");
                var foo = Create<IFoo>(xml);

                Assert.That(foo.A, Is.Not.Null & Is.Empty);
				Assert.That(xml,   XmlEquivalent.To("<Foo A='a'/>"));
            }

            [Test]
            public void Get_Missing()
            {
				var xml = Xml("<Foo/>");
                var foo = Create<IFoo>("<Foo/>");

                Assert.That(foo.A, Is.Not.Null & Is.Empty);
				Assert.That(xml,   XmlEquivalent.To("<Foo/>"));
            }

            [Test]
            public void Set()
            {
                var xml = Xml("<Foo/>");
                var foo = Create<IFoo>(xml);

                foo.A = Items;

                Assert.That(xml, XmlEquivalent.To("<Foo> <A> <int>1</int> <int>2</int> </A> </Foo>"));
            }
		}
	}
}
