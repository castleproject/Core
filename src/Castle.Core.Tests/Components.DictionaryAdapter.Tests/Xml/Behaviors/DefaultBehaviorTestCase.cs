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

#if !SILVERLIGHT // Until support for other platforms is verified
namespace Castle.Components.DictionaryAdapter.Xml.Tests
{
	using System;
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

				Assert.AreEqual("a", foo.A);
			}

			[Test]
			public void Get_Element_WithExpectedXsiType()
			{
				var foo = Create<IFoo>("<Foo $xsd $xsi> <A xsi:type='xsd:string'>a</A> </Foo>");

				Assert.AreEqual("a", foo.A);
			}

			[Test]
			public void Get_Element_WithUnexpectedXsiType()
			{
				var foo = Create<IFoo>("<Foo $xsi> <A xsi:type='unexpected'>a</A> </Foo>");

				Assert.IsNull(foo.A);
			}

			[Test]
			public void Get_Attribute()
			{
				var foo = Create<IFoo>("<Foo A='a'/>");

				Assert.AreEqual("a", foo.A);
			}

			[Test]
			public void Set()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.A = "a";

				CustomAssert.AreXmlEquivalent("<Foo> <A>a</A> </Foo>", xml);
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

				Assert.NotNull(foo.A);
				Assert.AreEqual("b", foo.A.B);
            }

            [Test]
            public void Get_Attribute()
            {
				var xml = Xml("<Foo A='a'/>");
                var foo = Create<IFoo>(xml);

				Assert.NotNull(foo.A);
				Assert.IsNull(foo.A.B);
				CustomAssert.AreXmlEquivalent("<Foo A='a'/>", xml);
            }

            [Test]
            public void Get_Missing()
            {
				var xml = Xml("<Foo/>");
                var foo = Create<IFoo>(xml);

				Assert.NotNull(foo.A);
				Assert.IsNull(foo.A.B);
				CustomAssert.AreXmlEquivalent("<Foo/>", xml);
            }

            [Test]
            public void Get_ReturnsSameInstance()
            {
                var foo = Create<IFoo>("<Foo> <A> <B>b</B> </A> </Foo>");

				var instanceA = foo.A;
				var instanceB = foo.A;

				Assert.AreSame(instanceB, instanceA, "Same component must be returned from successive calls.");
            }

            [Test]
            public void Set()
            {
                var xmlA = Xml(	"<Foo/>");
				var xmlB = Xml("<Foo> <A> <B>b</B> </A> </Foo>");
                var fooA = Create<IFoo>(xmlA);
				var fooB = Create<IFoo>(xmlB);

                fooA.A = fooB.A;
				var b = fooB.A.B;

				CustomAssert.AreXmlEquivalent(xmlB, xmlA);
            }

            [Test]
            public void SetPropertyOnVirtual()
            {
                var xml = Xml("<Foo/>");
                var foo = Create<IFoo>(xml);

                foo.A.B = "b";

				CustomAssert.AreXmlEquivalent("<Foo> <A> <B>b</B> </A> </Foo>", xml);
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

            [Test]
            public void Get_Element()
            {
                var foo = Create<IFoo>("<Foo> <A> <int>1</int> <int>2</int> </A> </Foo>");

				CollectionAssert.AreEquivalent(Items, foo.A);
            }

            [Test]
            public void Get_Attribute()
            {
				var xml = Xml("<Foo A='a'/>");
                var foo = Create<IFoo>(xml);

				Assert.NotNull(foo.A);
				Assert.IsEmpty(foo.A);
				CustomAssert.AreXmlEquivalent("<Foo A='a'/>", xml);
            }

            [Test]
            public void Get_Missing()
            {
				var xml = Xml("<Foo/>");
                var foo = Create<IFoo>("<Foo/>");

				Assert.NotNull(foo.A);
				Assert.IsEmpty(foo.A);
				CustomAssert.AreXmlEquivalent("<Foo/>", xml);
            }

            [Test]
            public void Set()
            {
                var xml = Xml("<Foo/>");
                var foo = Create<IFoo>(xml);

                foo.A = Items;

				CustomAssert.AreXmlEquivalent("<Foo> <A> <int>1</int> <int>2</int> </A> </Foo>", xml);
            }
		}

        [TestFixture]
        public class XmlSerializableProperty : XmlAdapterTestCase
        {
            public interface IFoo : IDictionaryAdapter
            {
                FakeStandardXmlSerializable X { get; set; }
            }

            [Test]
            public void GetProperty_DefaultBehavior_XmlSerializable_Element()
            {
                var foo = Create<IFoo>("<Foo> <X> <Text>hello</Text> </X> </Foo>");

				Assert.NotNull(foo.X);
				Assert.AreEqual("hello", foo.X.Text);
            }

            [Test]
            public void GetProperty_DefaultBehavior_XmlSerializable_Attribute()
            {
                var foo = Create<IFoo>("<Foo X='hello'/>");

				Assert.IsNull(foo.X);
            }

            [Test]
            public void SetProperty_DefaultBehavior_XmlSerializable()
            {
                var xml = Xml("<Foo/>");
                var foo = Create<IFoo>(xml);

                foo.X = new FakeStandardXmlSerializable { Text = "hello" };

				CustomAssert.AreXmlEquivalent("<Foo> <X> <Text>hello</Text> </X> </Foo>", xml);
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

			[Test]
			public void Set_ToNull_Element()
			{
				var xml = Xml("<Root> <Value>TestValue</Value> </Root>");
				var obj = Create<IRoot>(xml);

				obj.Value = null;

				CustomAssert.AreXmlEquivalent(Xml("<Root $xsi> <Value xsi:nil='true'/> </Root>"), xml);
			}

			[Test]
			public void Set_ToNull_Attribute()
			{
				var xml = Xml("<Root Value='TestValue'/>");
				var obj = Create<IRoot>(xml);

				obj.Value = null;

				CustomAssert.AreXmlEquivalent(Xml("<Root $xsi> <Value xsi:nil='true'/> </Root>"), xml);
			}

			[Test]
			public void Set_ToValue_Element()
			{
				var xml = Xml("<Root $xsi> <Value xsi:nil='true'/> </Root>");
				var obj = Create<IRoot>(xml);

				obj.Value = "TestValue";

				CustomAssert.AreXmlEquivalent(Xml("<Root $xsi> <Value>TestValue</Value> </Root>"), xml);
			}

			[Test]
			public void Set_ToArray_Element()
			{
				var xml = Xml("<Root $xsi> <Array xsi:nil='true'/> </Root>");
				var obj = Create<IRoot>(xml);

				obj.Array = new[] { "TestValue" };

				CustomAssert.AreXmlEquivalent(Xml("<Root $xsi> <Array> <string>TestValue</string> </Array> </Root>"), xml);
			}
		}

		[TestFixture]
		public class Coercion : XmlAdapterTestCase
		{
			public interface IFoo : IDictionaryAdapter { string A { get; set; } }
			public interface IBar : IDictionaryAdapter { string B { get; set; } }

			[Test]
			public void Coerce()
			{
				var xml = Xml("<Foo> <A>a</A> <B>b</B> </Foo>");
				var foo = Create<IFoo>(xml);
				var bar = foo.Coerce<IBar>();

				Assert.NotNull(bar);
				Assert.AreEqual("b", bar.B);
			}

			[Test]
			public void SharedXmlAdapters()
			{
				var xml = Xml("<Foo> <A>a</A> <B>b</B> </Foo>");
				var foo = Create<IFoo>(xml);
				var bar = foo.Coerce<IBar>();

				var fooAdapter = XmlAdapter.For(foo);
				var barAdapter = XmlAdapter.For(bar);

				Assert.AreNotSame(bar, foo);
				Assert.AreNotSame(bar.This, foo.This);
				Assert.AreSame(barAdapter, fooAdapter);
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

			[Test]
			public void NondestructiveRead()
			{
				var xml = Xml("<Obj/>");
				var obj = Create<IObj>(xml);

				Assert.True(obj.Foo.Bar.Id == Guid.Empty);

				CustomAssert.AreXmlEquivalent("<Obj/>", xml);
			}

			[Test]
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

				Assert.True(AsVirtual(obj).IsReal , "Obj exists");
				Assert.False(AsVirtual(foo).IsReal, "Foo exists");
				Assert.False(AsVirtual(bar).IsReal, "Bar exists");

				bar.Id = Guid.NewGuid();

				Assert.True(AsVirtual(obj).IsReal, "Obj exists");
				Assert.True(AsVirtual(foo).IsReal, "Foo exists");
				Assert.True(AsVirtual(bar).IsReal, "Bar exists");

				Assert.False(realizedObj, "Obj was realized");
				Assert.True(realizedFoo, "Foo was realized");
				Assert.True(realizedBar, "Bar was realized");
			}

			private static void HandleRealized(object sender, object expected, ref bool realized, string message)
			{
				Assert.AreSame(AsVirtual(expected), sender, message);
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
