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
	using Xunit;

	public class DefaultBehaviorTestCase
	{
				public class SimpleProperty : XmlAdapterTestCase
		{
			public interface IFoo : IDictionaryAdapter
			{
				string A { get; set; }
			}

			[Fact]
			public void Get_Element()
			{
				var foo = Create<IFoo>("<Foo> <A>a</A> </Foo>");

				Assert.That(foo.A, Is.EqualTo("a"));
			}

			[Fact]
			public void Get_Element_WithExpectedXsiType()
			{
				var foo = Create<IFoo>("<Foo $xsd $xsi> <A xsi:type='xsd:string'>a</A> </Foo>");

				Assert.That(foo.A, Is.EqualTo("a"));
			}

			[Fact]
			public void Get_Element_WithUnexpectedXsiType()
			{
				var foo = Create<IFoo>("<Foo $xsi> <A xsi:type='unexpected'>a</A> </Foo>");

				Assert.That(foo.A, Is.Null);
			}

			[Fact]
			public void Get_Attribute()
			{
				var foo = Create<IFoo>("<Foo A='a'/>");

				Assert.That(foo.A, Is.EqualTo("a"));
			}

			[Fact]
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

            [Fact]
            public void Get_Element()
            {
                var foo = Create<IFoo>("<Foo> <A> <B>b</B> </A> </Foo>");

				Assert.That(foo.A,   Is.Not.Null);
                Assert.That(foo.A.B, Is.EqualTo("b"));
            }

            [Fact]
            public void Get_Attribute()
            {
				var xml = Xml("<Foo A='a'/>");
                var foo = Create<IFoo>(xml);

				Assert.That(foo.A,   Is.Not.Null);
                Assert.That(foo.A.B, Is.Null);
				Assert.That(xml,     XmlEquivalent.To("<Foo A='a'/>"));
            }

            [Fact]
            public void Get_Missing()
            {
				var xml = Xml("<Foo/>");
                var foo = Create<IFoo>(xml);

				Assert.That(foo.A,   Is.Not.Null);
                Assert.That(foo.A.B, Is.Null);
				Assert.That(xml,     XmlEquivalent.To("<Foo/>"));
            }

            [Fact]
            public void Get_ReturnsSameInstance()
            {
                var foo = Create<IFoo>("<Foo> <A> <B>b</B> </A> </Foo>");

				var instanceA = foo.A;
				var instanceB = foo.A;

                Assert.That(instanceA, Is.SameAs(instanceB),
					"Same component must be returned from successive calls.");
            }

            [Fact]
            public void Set()
            {
                var xmlA = Xml(	"<Foo/>");
				var xmlB = Xml("<Foo> <A> <B>b</B> </A> </Foo>");
                var fooA = Create<IFoo>(xmlA);
				var fooB = Create<IFoo>(xmlB);

                fooA.A = fooB.A;
				var b = fooB.A.B;

                Assert.That(xmlA, XmlEquivalent.To(xmlB));
            }

            [Fact]
            public void SetPropertyOnVirtual()
            {
                var xml = Xml("<Foo/>");
                var foo = Create<IFoo>(xml);

                foo.A.B = "b";

                Assert.That(xml, XmlEquivalent.To("<Foo> <A> <B>b</B> </A> </Foo>"));
            }
		}

		[TestFixture]
		public class CollectionProperty : XmlAdapterTestCase
		{
			public interface IFoo : IDictionaryAdapter
			{
				int[] A { get; set; }
			}

			public int[] Items = { 1, 2 };

            [Fact]
            public void Get_Element()
            {
                var foo = Create<IFoo>("<Foo> <A> <int>1</int> <int>2</int> </A> </Foo>");

                Assert.That(foo.A, Is.EquivalentTo(Items));
            }

            [Fact]
            public void Get_Attribute()
            {
				var xml = Xml("<Foo A='a'/>");
                var foo = Create<IFoo>(xml);

                Assert.That(foo.A, Is.Not.Null & Is.Empty);
				Assert.That(xml,   XmlEquivalent.To("<Foo A='a'/>"));
            }

            [Fact]
            public void Get_Missing()
            {
				var xml = Xml("<Foo/>");
                var foo = Create<IFoo>("<Foo/>");

                Assert.That(foo.A, Is.Not.Null & Is.Empty);
				Assert.That(xml,   XmlEquivalent.To("<Foo/>"));
            }

            [Fact]
            public void Set()
            {
                var xml = Xml("<Foo/>");
                var foo = Create<IFoo>(xml);

                foo.A = Items;

                Assert.That(xml, XmlEquivalent.To("<Foo> <A> <int>1</int> <int>2</int> </A> </Foo>"));
            }
		}

        [TestFixture]
        public class XmlSerializableProperty : XmlAdapterTestCase
        {
            public interface IFoo : IDictionaryAdapter
            {
                FakeStandardXmlSerializable X { get; set; }
            }

            [Fact]
            public void GetProperty_DefaultBehavior_XmlSerializable_Element()
            {
                var foo = Create<IFoo>("<Foo> <X> <Text>hello</Text> </X> </Foo>");

                Assert.That(foo.X,      Is.Not.Null);
                Assert.That(foo.X.Text, Is.EqualTo("hello"));
            }

            [Fact]
            public void GetProperty_DefaultBehavior_XmlSerializable_Attribute()
            {
                var foo = Create<IFoo>("<Foo X='hello'/>");

                Assert.That(foo.X, Is.Null);
            }

            [Fact]
            public void SetProperty_DefaultBehavior_XmlSerializable()
            {
                var xml = Xml("<Foo/>");
                var foo = Create<IFoo>(xml);

                foo.X = new FakeStandardXmlSerializable { Text = "hello" };

                Assert.That(xml, XmlEquivalent.To("<Foo> <X> <Text>hello</Text> </X> </Foo>"));
            }
        }

		[TestFixture]
		public class Nullable : XmlAdapterTestCase
		{
			[XmlDefaults(IsNullable = true)]
			public interface IRoot
			{
				string   Value { get; set; }
				string[] Array { get; set; }
			}

			[Fact]
			public void Set_ToNull_Element()
			{
				var xml = Xml("<Root> <Value>TestValue</Value> </Root>");
				var obj = Create<IRoot>(xml);

				obj.Value = null;

				Assert.That(xml, XmlEquivalent.To(Xml("<Root $xsi> <Value xsi:nil='true'/> </Root>")));
			}

			[Fact]
			public void Set_ToNull_Attribute()
			{
				var xml = Xml("<Root Value='TestValue'/>");
				var obj = Create<IRoot>(xml);

				obj.Value = null;

				Assert.That(xml, XmlEquivalent.To(Xml("<Root $xsi> <Value xsi:nil='true'/> </Root>")));
			}

			[Fact]
			public void Set_ToValue_Element()
			{
				var xml = Xml("<Root $xsi> <Value xsi:nil='true'/> </Root>");
				var obj = Create<IRoot>(xml);

				obj.Value = "TestValue";

				Assert.That(xml, XmlEquivalent.To(Xml("<Root $xsi> <Value>TestValue</Value> </Root>")));
			}

			[Fact]
			public void Set_ToArray_Element()
			{
				var xml = Xml("<Root $xsi> <Array xsi:nil='true'/> </Root>");
				var obj = Create<IRoot>(xml);

				obj.Array = new[] { "TestValue" };

				Assert.That(xml, XmlEquivalent.To(Xml("<Root $xsi> <Array> <string>TestValue</string> </Array> </Root>")));
			}
		}

		[TestFixture]
		public class Coercion : XmlAdapterTestCase
		{
			public interface IFoo : IDictionaryAdapter { string A { get; set; } }
			public interface IBar : IDictionaryAdapter { string B { get; set; } }

			[Fact]
			public void Coerce()
			{
				var xml = Xml("<Foo> <A>a</A> <B>b</B> </Foo>");
				var foo = Create<IFoo>(xml);
				var bar = foo.Coerce<IBar>();

				Assert.That(bar,   Is.Not.Null);
				Assert.That(bar.B, Is.EqualTo("b"));
			}

			[Fact]
			public void SharedXmlAdapters()
			{
				var xml = Xml("<Foo> <A>a</A> <B>b</B> </Foo>");
				var foo = Create<IFoo>(xml);
				var bar = foo.Coerce<IBar>();

				var fooAdapter = XmlAdapter.For(foo);
				var barAdapter = XmlAdapter.For(bar);

				Assert.That(foo,        Is.Not.SameAs(bar));
				Assert.That(foo.This,   Is.Not.SameAs(bar.This));
				Assert.That(fooAdapter, Is    .SameAs(barAdapter));
			}
		}

		[TestFixture]
		public class VirtualObjects : XmlAdapterTestCase
		{
			public interface IObj
			{
				IFoo Foo { get; set; }
			}

			public interface IFoo
			{
				IBar Bar { get; set; }
			}

			public interface IBar
			{
				Guid Id { get; set; }
			}

			[Fact]
			public void NondestructiveRead()
			{
				var xml = Xml("<Obj/>");
				var obj = Create<IObj>(xml);

				Assert.That(obj.Foo.Bar.Id == Guid.Empty, Is.True);

				Assert.That(xml, XmlEquivalent.To(
					"<Obj/>"
				));
			}

			[Fact]
			public void ObservingRealization()
			{
				var realizedObj = false;
				var realizedFoo = false;
				var realizedBar = false;

				var obj = Create<IObj>("<Obj/>");
				var foo = obj.Foo;
				var bar = foo.Bar;

				AsVirtual(obj).Realized += (s, e) => HandleRealized(s, obj, ref realizedObj, "(This should never happen!)");
				AsVirtual(foo).Realized += (s, e) => HandleRealized(s, foo, ref realizedFoo, "Sender was Foo's virtual");
				AsVirtual(bar).Realized += (s, e) => HandleRealized(s, bar, ref realizedBar, "Sender was Bar's virtual");

				Assert.That(AsVirtual(obj).IsReal, Is.True , "Obj exists");
				Assert.That(AsVirtual(foo).IsReal, Is.False, "Foo exists");
				Assert.That(AsVirtual(bar).IsReal, Is.False, "Bar exists");

				bar.Id = Guid.NewGuid();

				Assert.That(AsVirtual(obj).IsReal, Is.True, "Obj exists");
				Assert.That(AsVirtual(foo).IsReal, Is.True, "Foo exists");
				Assert.That(AsVirtual(bar).IsReal, Is.True, "Bar exists");

				Assert.That(realizedObj, Is.False, "Obj was realized");
				Assert.That(realizedFoo, Is.True , "Foo was realized");
				Assert.That(realizedBar, Is.True , "Bar was realized");
			}

			private static void HandleRealized(object sender, object expected, ref bool realized, string message)
			{
				Assert.That(sender, Is.SameAs(AsVirtual(expected)), message);
				realized = true;
			}

			private static IVirtual AsVirtual(object source)
			{
				return ((IDictionaryAdapter) source).AsVirtual();
			}
		}
	}
}
#endif
