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

#if !SILVERLIGHT && !MONO // Until support for other platforms is verified
namespace Castle.Components.DictionaryAdapter.Xml.Tests
{
	using System;
	using System.Collections.Generic;
	using Castle.Components.DictionaryAdapter.Tests;
	using NUnit.Framework;

	public class ReferenceBehaviorTestCase
	{
		[TestFixture]
		public class WithAttributeOnInterface : XmlAdapterTestCase
		{
			[Reference]
			public interface IFoo
			{				
				string      Value { get; set; }
				IFoo        One   { get; set; }
				IFoo        Two   { get; set; }
				IFoo[]      Array { get; set; }
				IList<IFoo> List  { get; set; }
			}

			[Test]
			public void InvalidXml()
			{
				var xml = Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<Two x:ref='2'/>",
					"</Foo>"
				);
				var foo = Create<IFoo>(xml);

				IFoo dummy;
				Assert.Throws<KeyNotFoundException>(() =>
					dummy = foo.Two);
			}

			[Test]
			public void PropertyReference_Get()
			{
				var xml = Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<Two x:ref='1'/>",
					"</Foo>"
				);
				var foo = Create<IFoo>(xml);

				Assert.That(foo.Two, Is.SameAs(foo.One));
			}

			[Test]
			public void PropertyReference_Set()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.One.Value = "One";
				foo.Two = foo.One;

				Assert.That(xml, XmlEquivalent.To(Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<Two x:ref='1'/>",
					"</Foo>"
				)));
			}

			[Test]
			public void PropertyReference_Set_ToVirtual()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.Two = foo.One;

				Assert.That(xml, XmlEquivalent.To(Xml(
					"<Foo $x>",
						"<Two x:ref='1'/>",
						"<One x:id='1'/>",
					"</Foo>"
				)));
			}

			[Test]
			public void PropertyReference_Set_ReplacingVirtual()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.One.Value = "One";
				var two = foo.Two;
				foo.Two = foo.One;

				Assert.That(xml, XmlEquivalent.To(Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<Two x:ref='1'/>",
					"</Foo>"
				)));
			}

			[Test]
			public void ArrayReference_Get()
			{
				var xml = Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<Array>",
							"<Foo x:ref='1'/>",
						"</Array>",
					"</Foo>"
				);
				var foo = Create<IFoo>(xml);

				Assert.That(foo.Array,    Is.Not.Null & Has.Length.EqualTo(1));
				Assert.That(foo.Array[0], Is.SameAs(foo.One));
			}

			[Test]
			public void ArrayReference_Set()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.One.Value = "One";
				foo.Array = new[] { foo.One };

				Assert.That(xml, XmlEquivalent.To(Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<Array>",
							"<Foo x:ref='1'/>",
						"</Array>",
					"</Foo>"
				)));
			}

			[Test]
			public void ListItemReference_Get()
			{
				var xml = Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<List>",
							"<Foo x:ref='1'/>",
						"</List>",
					"</Foo>"
				);
				var foo = Create<IFoo>(xml);

				Assert.That(foo.List,    Is.Not.Null & Has.Count.EqualTo(1));
				Assert.That(foo.List[0], Is.SameAs(foo.One));
			}

			[Test]
			public void ListItemReference_Add()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.One.Value = "One";
				foo.List.Add(foo.One);

				Assert.That(xml, XmlEquivalent.To(Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<List>",
							"<Foo x:ref='1'/>",
						"</List>",
					"</Foo>"
				)));
			}

			[Test]
			public void ListItemReference_Set()
			{
				var xml = Xml(
					"<Foo $x>",
						"<One> <Value>One</Value> </One>",
						"<List>",
							"<Foo> <Value>Two</Value> </Foo>",
						"</List>",
					"</Foo>"
				);
				var foo = Create<IFoo>(xml);

				foo.List[0] = foo.One;

				Assert.That(xml, XmlEquivalent.To(Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<List>",
							"<Foo x:ref='1'/>",
						"</List>",
					"</Foo>"
				)));
			}

			[Test]
			public void ListItemReference_Unset()
			{
				var xml = Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<List>",
							"<Foo x:ref='1'/>",
						"</List>",
					"</Foo>"
				);
				var foo = Create<IFoo>(xml);

				foo.List[0] = Create<IFoo>("<Foo> <Value>Two</Value> </Foo>");

				Assert.That(xml, XmlEquivalent.To(Xml(
					"<Foo $x>",
						"<One> <Value>One</Value> </One>",
						"<List>",
							"<Foo> <Value>Two</Value> </Foo>",
						"</List>",
					"</Foo>"
				)));

				Assert.That(foo.List,          Is.Not.Null & Has.Count.EqualTo(1));
				Assert.That(foo.List[0],       Is.Not.SameAs(foo.One));
				Assert.That(foo.List[0].Value, Is.EquivalentTo("Two"));
			}

			[Test]
			public void ListItemReference_Remove()
			{
				var xml = Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<List>",
							"<Foo x:ref='1'/>",
						"</List>",
					"</Foo>"
				);
				var foo = Create<IFoo>(xml);

				foo.List.RemoveAt(0);

				Assert.That(xml, XmlEquivalent.To(Xml(
					"<Foo $x>",
						"<One> <Value>One</Value> </One>",
						"<List/>",
					"</Foo>"
				)));
			}

			[Test]
			public void ListItemReference_Clear()
			{
				var xml = Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<List>",
							"<Foo x:ref='1'/>",
						"</List>",
					"</Foo>"
				);
				var foo = Create<IFoo>(xml);

				foo.List.Clear();

				Assert.That(xml, XmlEquivalent.To(Xml(
					"<Foo $x>",
						"<One> <Value>One</Value> </One>",
						"<List/>",
					"</Foo>"
				)));
			}

			[Test]
			public void NestedReference_NonNestedPrimary()
			{
				var xml = Xml("<Foo/>");
				var a = Create<IFoo>(xml);
				var b = Create<IFoo>();
				var c = Create<IFoo>();

				c.Value = "X";
				a.One = c;
				b.One = c;
				a.Two = b;

				Assert.That(xml, XmlEquivalent.To(Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>X</Value> </One>",
						"<Two>",
							"<One x:ref='1'/>",
						"</Two>",
					"</Foo>"
				)));
			}

			[Test]
			public void NestedReference_NestedPrimary()
			{
				var xml = Xml("<Foo/>");
				var a = Create<IFoo>(xml);
				var b = Create<IFoo>();
				var c = Create<IFoo>();

				c.Value = "X";
				b.One = c;
				b.Two = c;
				a.One = b;

				Assert.That(xml, XmlEquivalent.To(Xml(
					"<Foo $x>",
						"<One>",
							"<One x:id='1'> <Value>X</Value> </One>",
							"<Two x:ref='1'/>",
						"</One>",
					"</Foo>"
				)));
			}

			[Test]
			public void SelfReference_Root()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.One = foo;

				Assert.That(xml, XmlEquivalent.To(Xml(
					"<Foo x:id='1' $x>",
						"<One x:ref='1'/>",
					"</Foo>"
				)));
			}

			[Test]
			public void SelfReference_Child()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo = foo.One;
				foo.One = foo;

				Assert.That(xml, XmlEquivalent.To(Xml(
					"<Foo $x>",
						"<One x:id='1'>",
							"<One x:ref='1'/>",
						"</One>",
					"</Foo>"
				)));
			}
		}
	}
}
#endif
