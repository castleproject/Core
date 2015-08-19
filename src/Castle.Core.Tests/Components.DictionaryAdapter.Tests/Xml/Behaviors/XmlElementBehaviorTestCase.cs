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

				Assert.AreEqual("a", foo.A);
            }

            [Test]
            public void GetProperty_ElementBehavior_String_Attribute()
            {
                var foo = Create<IFoo>("<Foo A='a'/>");

				Assert.AreEqual(default(string), foo.A);
            }

            [Test]
            public void SetProperty_ElementBehavior_String()
            {
                var xml = Xml("<Foo/>");
                var foo = Create<IFoo>(xml);

                foo.A = "a";

				CustomAssert.AreXmlEquivalent("<Foo> <A>a</A> </Foo>", xml);
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

				Assert.AreEqual(2, array.Length);
				Assert.AreEqual("0", array[0]);
				Assert.AreEqual("1", array[1]);
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

				Assert.AreEqual(2, array.Length);
				Assert.IsInstanceOf<IDerived1>(array[0]);
				Assert.IsInstanceOf<IDerived2>(array[1]);
            }

            //[Test]
            //public void SetProperty_ArrayBehavior_Array()
            //{
            //    var array = new[] { 1, 2 };
            //    var xml = Xml("<Foo/>");
            //    var foo = Create<IRoot>(xml);

            //    foo.F = array;

            //    CustomAssert.AreXmlEquivalent("<Foo> <F> <int>1</int> <int>2</int> </F> </Foo>", xml);
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
#endif
