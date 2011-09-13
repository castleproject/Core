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

    public class XmlAdapterTestCase
    {
        #region Default Behavior
        [TestFixture]
        public class DefaultBehavior : TestCase
        {
            public interface IFoo : IDictionaryAdapter
            {
                string A { get; set; }
                int    B { get; set; }
                Guid   C { get; set; }
                AnEnum D { get; set; }
                byte[] E { get; set; }
                int[]  F { get; set; }
                IBar   G { get; set; }

                IList      <string> SL { get; set; }
                ICollection<string> SC { get; set; }
                IEnumerable<string> SE { get; set; }
                ISet       <string> SS { get; set; }
                BindingList<string> SB { get; set; }

                FakeStandardXmlSerializable X { get; set; }
                FakeCustomXmlSerializable   Y { get; set; }
            }

            public interface IBar : IDictionaryAdapter
            {
                string X { get; set; }
            }

            public enum AnEnum { A, B }

            [Test]
            public void GetProperty_DefaultBehavior_Simple_Element()
            {
                var foo = Create<IFoo>("<Foo> <A>a</A> </Foo>");

                Assert.That(foo.A, Is.EqualTo("a"));
            }

            [Test]
            public void GetProperty_DefaultBehavior_Simple_Attribute()
            {
                var foo = Create<IFoo>("<Foo A='a'/>");

                Assert.That(foo.A, Is.EqualTo("a"));
            }

            [Test]
            public void SetProperty_DefaultBehavior_Simple()
            {
                var xml = Xml("<Foo/>");
                var foo = Create<IFoo>(xml);

                foo.A = "a";

                Assert.That(xml, XmlEquivalent.To("<Foo> <A>a</A> </Foo>"));
            }

            [Test]
            public void GetProperty_DefaultBehavior_Component_ReturnsSameElement()
            {
                var foo = Create<IFoo>("<Foo> <G> <X>x</X> </G> </Foo>");

				var instanceA = foo.G;
				var instanceB = foo.G;

                Assert.That(instanceA, Is.SameAs(instanceB),
					"Same component must be returned from successive calls.");
            }

            [Test]
            public void GetProperty_DefaultBehavior_Component_Element()
            {
                var foo = Create<IFoo>("<Foo> <G> <X>x</X> </G> </Foo>");

                Assert.That(foo.G.X, Is.EqualTo("x"));
            }

            [Test]
            public void GetProperty_DefaultBehavior_Component_Attribute()
            {
                var foo = Create<IFoo>("<Foo G='1'/>");

                Assert.That(foo.G.X, Is.Null);
            }

            [Test]
            public void SetProperty_DefaultBehavior_Component()
            {
                var xml = Xml("<Foo/>");
                var foo = Create<IFoo>(xml);

                foo.G.X = "x";

                Assert.That(xml, XmlEquivalent.To("<Foo> <G> <X>x</X> </G> </Foo>"));
            }

            [Test]
            public void GetProperty_DefaultBehavior_Array_Element()
            {
                var array = new[] { 1, 2 };
                var foo = Create<IFoo>("<Foo> <F> <int>1</int> <int>2</int> </F> </Foo>");

                Assert.That(foo.F, Is.EquivalentTo(array));
            }

            [Test]
            public void GetProperty_DefaultBehavior_Array_Attribute()
            {
                var array = new[] { 1, 2 };
                var foo = Create<IFoo>("<Foo F='1'/>");

                Assert.That(foo.F, Is.Not.Null & Is.Empty);
            }

            [Test]
            public void SetProperty_DefaultBehavior_Array()
            {
                var array = new[] { 1, 2 };
                var xml = Xml("<Foo/>");
                var foo = Create<IFoo>(xml);

                foo.F = array;

                Assert.That(xml, XmlEquivalent.To("<Foo> <F> <int>1</int> <int>2</int> </F> </Foo>"));
            }

            [Test]
            public void GetProperty_DefaultBehavior_List_Element()
            {
                var foo = Create<IFoo>("<Foo> <SL> <string>a</string> <string>b</string> </SL> </Foo>");

                var items = foo.SL.ToArray();

                Assert.That(items, Is.EquivalentTo(new[] { "a", "b" }));
            }

            [Test]
            public void GetProperty_DefaultBehavior_List_Attribute()
            {
                var array = new[] { 1, 2 };
                var foo = Create<IFoo>("<Foo F='1'/>");

                Assert.That(foo.SL, Is.Empty);
            }

            [Test]
            public void SetProperty_DefaultBehavior_List()
            {
                var xml = Xml("<Foo/>");
                var foo = Create<IFoo>(xml);

                foo.SL = new List<string> { "a", "b" };

                Assert.That(xml, XmlEquivalent.To("<Foo> <SL> <string>a</string> <string>b</string> </SL> </Foo>"));
            }

            [Test]
            public void GetProperty_DefaultBehavior_XmlSerializable_Element()
            {
                var foo = Create<IFoo>("<Foo> <X> <Text>hello</Text> </X> </Foo>");

                Assert.That(foo.X,      Is.Not.Null);
                Assert.That(foo.X.Text, Is.EqualTo("hello"));
            }

            [Test]
            public void GetProperty_DefaultBehavior_XmlSerializable_Attribute()
            {
                var foo = Create<IFoo>("<Foo X='hello'/>");

                Assert.That(foo.X, Is.Null);
            }

            [Test]
            public void SetProperty_DefaultBehavior_XmlSerializable()
            {
                var xml = Xml("<Foo/>");
                var foo = Create<IFoo>(xml);

                foo.X = new FakeStandardXmlSerializable { Text = "hello" };

                Assert.That(xml, XmlEquivalent.To("<Foo> <X> <Text>hello</Text> </X> </Foo>"));
            }
        }
        #endregion

        #region [XmlElement] Behavior
        [TestFixture]
        public class ElementBehavior : TestCase
        {

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

            public interface IFoo : IDictionaryAdapter
            {
                [XmlElement]
                string A { get; set; }

                [XmlElement]
                int B { get; set; }
            }
        }
        #endregion

        #region Multiple [XmlElement] Behavior
        [TestFixture]
        public class MultiElementBehavior : TestCase
        {
            [Test]
            public void GetProperty_ElementBehavior_Array_Element()
            {
                var foo = Create<IRoot>("<Root> <A X='1'/> <B X='1'/> </Root>");

				Assert.That(foo.Items,    Is.Not.Null & Has.Length.EqualTo(2));
				Assert.That(foo.Items[0], Is.InstanceOf<IDerived1>());
				Assert.That(foo.Items[1], Is.InstanceOf<IDerived2>());
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
        #endregion

        #region [XmlArray] Behavior
        [TestFixture]
        public class ArrayBehavior : TestCase
        {
            [Test]
            public void GetProperty_ArrayBehavior_Array_Element()
            {
                var foo = Create<IRoot>("<Root> <X> <A X='1'/> <B X='1'/> </X> </Root>");

                Assert.That(foo.Items,    Is.Not.Null & Has.Length.EqualTo(2));
                Assert.That(foo.Items[0], Is.InstanceOf<IDerived1>());
                Assert.That(foo.Items[1], Is.InstanceOf<IDerived2>());
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
                [XmlArray("X")]
                [XmlArrayItem("A", typeof(IDerived1))]
                [XmlArrayItem("B", typeof(IDerived2))]
                IBase[] Items { get; set; }
            }

            public interface IBase { }
            public interface IDerived1 : IBase { int    X { get; set; } }
            public interface IDerived2 : IBase { string X { get; set; } } 
        }
        #endregion

        #region [XmlRoot]
        public class XmlRootBehavior : TestCase
        {
            [XmlRoot("XX", Namespace = "urn:a")]
            public interface IA
            {
                string A { get; set; }
            }

            [Test]
            public void Foo()
            {
                var foo = Create<IA>();

                foo.A = "a";

                Assert.That(
                    XmlAdapter.For(foo).Node.Xml,
                    XmlEquivalent.To("<XX xmlns='urn:a'> <A>a</A> </XX>"));
            }
        }
        #endregion

        #region Base Test Case
        public abstract class TestCase
        {
            private DictionaryAdapterFactory factory;

            protected TestCase() { }

            [SetUp]
            public virtual void SetUp()
            {
                factory = new DictionaryAdapterFactory();
            }

            protected static XmlDocument Xml(params string[] xml)
            {
                var document = new XmlDocument();
                document.LoadXml(string.Concat(xml));
                return document;
            }

            protected T Create<T>()
            {
                return Create<T>(new XmlDocument());
            }

            protected T Create<T>(params string[] xml)
            {
                return Create<T>(Xml(xml));
            }

            protected T Create<T>(XmlNode storage)
            {
                var xmlAdapter = new XmlAdapter(storage);

                return (T) factory.GetAdapter(typeof(T),
                    new System.Collections.Hashtable(),
                    new DictionaryDescriptor()
                        .AddBehavior(XmlMetadataBehavior.Instance)
                        .AddBehavior(xmlAdapter));
            }

            public class FakeStandardXmlSerializable
            {
                public string Text { get; set; }
            }

            public class FakeCustomXmlSerializable : IXmlSerializable
            {
                public string Text { get; set; }

                System.Xml.Schema.XmlSchema IXmlSerializable.GetSchema() { return null; }
                void IXmlSerializable.ReadXml(XmlReader reader) { Text = reader.ReadString(); }
                void IXmlSerializable.WriteXml(XmlWriter writer) { writer.WriteString(Text); }
            }

            protected const string
                Base64String = "VGVzdA==",
                GuidString   = "c7da18ce-aa3f-452d-bf8f-8e3bb9cdec2b";

            protected readonly byte[] Base64Bytes = Convert.FromBase64String(Base64String);
            protected readonly Guid GuidValue = Guid.Parse(GuidString);
        }
        #endregion
    }
}
