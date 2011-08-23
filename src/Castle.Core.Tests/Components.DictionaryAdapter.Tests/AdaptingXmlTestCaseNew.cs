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
	using System.Xml;
	using System.Xml.Serialization;
	using System.Xml.XPath;
	using Castle.Components.DictionaryAdapter.Tests;
	using NUnit.Framework;

	public class AdaptingXmlTestCaseNew
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
			}

			public interface IBar : IDictionaryAdapter
			{
				string X { get; set; }
			}

			public enum AnEnum { A, B }

			[Test]
			public void GetProperty_DefaultBehavior_String_Element()
			{
				var foo = Create<IFoo>("<Foo> <A>a</A> </Foo>");

				Assert.That(foo.A, Is.EqualTo("a"));
			}

			[Test]
			public void GetProperty_DefaultBehavior_String_Attribute()
			{
				var foo = Create<IFoo>("<Foo A='a'/>");

				Assert.That(foo.A, Is.EqualTo("a"));
			}

			[Test]
			public void SetProperty_DefaultBehavior_String()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.A = "a";

				Assert.That(xml, XmlEquivalent.To("<Foo> <A>a</A> </Foo>"));
			}

			[Test]
			public void GetProperty_DefaultBehavior_Number_Element()
			{
				var foo = Create<IFoo>("<Foo> <B>1</B> </Foo>");

				Assert.That(foo.B, Is.EqualTo(1));
			}

			[Test]
			public void GetProperty_DefaultBehavior_Number_Attribute()
			{
				var foo = Create<IFoo>("<Foo B='1'/>");

				Assert.That(foo.B, Is.EqualTo(1));
			}

			[Test]
			public void SetProperty_DefaultBehavior_Number()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.B = 1;

				Assert.That(xml, XmlEquivalent.To("<Foo> <B>1</B> </Foo>"));
			}

			[Test]
			public void GetProperty_DefaultBehavior_Guid_Element()
			{
				var foo = Create<IFoo>("<Foo> <C>", GuidString, "</C> </Foo>");

				Assert.That(foo.C, Is.EqualTo(GuidValue));
			}

			[Test]
			public void GetProperty_DefaultBehavior_Guid_Attribute()
			{
				var foo = Create<IFoo>("<Foo C='", GuidString, "'/>");

				Assert.That(foo.C, Is.EqualTo(GuidValue));
			}

			[Test]
			public void SetProperty_DefaultBehavior_Guid()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.C = GuidValue;

				Assert.That(xml, XmlEquivalent.To("<Foo> <C>", GuidString, "</C> </Foo>"));
			}

			[Test]
			public void GetProperty_DefaultBehavior_Enum_Element()
			{
				var foo = Create<IFoo>("<Foo> <D>A</D> </Foo>");

				Assert.That(foo.D, Is.EqualTo(AnEnum.A));
			}

			[Test]
			public void GetProperty_DefaultBehavior_Enum_Attribute()
			{
				var foo = Create<IFoo>("<Foo D='A'/>");

				Assert.That(foo.D, Is.EqualTo(AnEnum.A));
			}

			[Test]
			public void SetProperty_DefaultBehavior_Enum()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.D = AnEnum.A;

				Assert.That(xml, XmlEquivalent.To("<Foo> <D>A</D> </Foo>"));
			}

			[Test]
			public void GetProperty_DefaultBehavior_ByteArray_Element()
			{
				var foo = Create<IFoo>("<Foo> <E>", Base64String, "</E> </Foo>");

				Assert.That(foo.E, Is.EqualTo(Base64Bytes));
			}

			[Test]
			public void GetProperty_DefaultBehavior_ByteArray_Attribute()
			{
				var foo = Create<IFoo>("<Foo E='", Base64String, "'/>");

				Assert.That(foo.E, Is.EqualTo(Base64Bytes));
			}

			[Test]
			public void SetProperty_DefaultBehavior_ByteArray()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.E = Base64Bytes;

				Assert.That(xml, XmlEquivalent.To("<Foo> <E>", Base64String, "</E> </Foo>"));
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

				Assert.That(foo.F, Is.Null | Is.Empty);
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
		}
		#endregion

		#region [XmlElement] Behavior
		[TestFixture]
		public class ElementBehavior : TestCase
		{

			[Test]
			public void GetProperty_ElementBehavior_String_Element()
			{
				var foo = Create<IFooElement>("<Foo> <A>a</A> </Foo>");

				Assert.That(foo.A, Is.EqualTo("a"));
			}

			[Test]
			public void GetProperty_ElementBehavior_String_Attribute()
			{
				var foo = Create<IFooElement>("<Foo A='a'/>");

				Assert.That(foo.A, Is.EqualTo(default(string)));
			}

			[Test]
			public void SetProperty_ElementBehavior_String()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFooElement>(xml);

				foo.A = "a";

				Assert.That(xml.OuterXml, Contains.Substring("<A>a</A>"));
			}

			[Test]
			public void GetProperty_ElementBehavior_Number_Element()
			{
				var foo = Create<IFooElement>("<Foo> <B>1</B> </Foo>");

				Assert.That(foo.B, Is.EqualTo(1));
			}

			[Test]
			public void GetProperty_ElementBehavior_Number_Attribute()
			{
				var foo = Create<IFooElement>("<Foo B='1'/>");

				Assert.That(foo.B, Is.EqualTo(default(int)));
			}

			[Test]
			public void SetProperty_ElementBehavior_Number()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFooElement>(xml);

				foo.B = 1;

				Assert.That(xml.OuterXml, Contains.Substring("<B>1</B>"));
			}

			public interface IFooElement : IDictionaryAdapter
			{
				[XmlElement]
				string A { get; set; }

				[XmlElement]
				int B { get; set; }
			}
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
					XmlAdapter.For(foo),
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

			protected T Create<T>(IXPathNavigable storage)
			{
				var xmlAdapter = new XmlAdapter(storage);

				return (T) factory.GetAdapter(typeof(T),
					new System.Collections.Hashtable(),
					new DictionaryDescriptor()
						.AddBehavior(XmlMetadataBehavior.Instance)
						.AddBehavior(xmlAdapter));
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
