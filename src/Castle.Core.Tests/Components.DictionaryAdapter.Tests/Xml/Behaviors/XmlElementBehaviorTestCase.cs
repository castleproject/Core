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
	using System.Xml.Serialization;
	using Castle.Components.DictionaryAdapter.Tests;
    using NUnit.Framework;

	public class XmlElementBehaviorTestCase
	{
        [TestFixture]
        public class SimpleProperty : XmlAdapterTestCase
        {
            public interface IFoo : IDictionaryAdapter
            {
                [XmlElement]
                string A { get; set; }
            }

            [Test]
            public void GetProperty_ElementBehavior_String_Element()
            {
                var foo = Create<IFoo>("<Foo> <A>a</A> </Foo>");

                Assert.That(foo.A, Is.EqualTo("a"));
            }

            [Test]
            public void GetProperty_ElementBehavior_String_Attribute()
            {
                var foo = Create<IFoo>("<Foo A='a'/>");

                Assert.That(foo.A, Is.EqualTo(default(string)));
            }

            [Test]
            public void SetProperty_ElementBehavior_String()
            {
                var xml = Xml("<Foo/>");
                var foo = Create<IFoo>(xml);

                foo.A = "a";

                Assert.That(xml, XmlEquivalent.To("<Foo> <A>a</A> </Foo>"));
            }
        }

		[TestFixture]
		public class ElementBehavior_CollectionProperty : XmlAdapterTestCase
		{
			public interface IFoo : IDictionaryAdapter
			{
				[XmlElement("A")]
				string[] Items { get; set; }
			}

			[Test]
			public void GetProperty()
			{
				var xml = Xml("<Foo> <A>0</A> <A>1</A> </Foo>");
				var foo = Create<IFoo>(xml);

				var array = foo.Items;

				Assert.That(array,    Is.Not.Null & Has.Length.EqualTo(2));
				Assert.That(array[0], Is.EqualTo("0"));
				Assert.That(array[1], Is.EqualTo("1"));
			}
		}

        [TestFixture]
        public class MultiElementBehavior : XmlAdapterTestCase
        {
            [Test]
            public void GetProperty_ElementBehavior_Array_Element()
            {
                var foo = Create<IRoot>("<Root> <A X='1'/> <B X='1'/> </Root>");

				var array = foo.Items;

				Assert.That(array,    Is.Not.Null & Has.Length.EqualTo(2));
				Assert.That(array[0], Is.InstanceOf<IDerived1>());
				Assert.That(array[1], Is.InstanceOf<IDerived2>());
            }

            //[Test]
            //public void SetProperty_ArrayBehavior_Array()
            //{
            //    var array = new[] { 1, 2 };
            //    var xml = Xml("<Foo/>");
            //    var foo = Create<IRoot>(xml);

            //    foo.F = array;

            //    Assert.That(xml, XmlEquivalent.To("<Foo> <F> <int>1</int> <int>2</int> </F> </Foo>"));
            //}

            public interface IRoot
            {
                [XmlElement("A", typeof(IDerived1))]
                [XmlElement("B", typeof(IDerived2))]
                IBase[] Items { get; set; }
            }

            public interface IBase { }
            public interface IDerived1 : IBase { int    X { get; set; } }
            public interface IDerived2 : IBase { string X { get; set; } } 
        }
	}
}
