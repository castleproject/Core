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
	using System.Collections.Generic;
	using System.Linq;
	using Castle.Components.DictionaryAdapter.Tests;
	using NUnit.Framework;

	public class ReferenceBehaviorTestCase
	{
		[TestFixture]
		public class WithAttributeOnInterface : XmlAdapterTestCase
		{
			[Reference]
			public interface IFoo : IDictionaryAdapter
			{
				string      Value { get; set; }
				IFoo        One   { get; set; }
				IFoo        Two   { get; set; }
				IFoo[]      Array { get; set; }
				IList<IFoo> List  { get; set; }
#if DOTNET40
				ISet <IFoo> Set   { get; set; }
#endif
			}

			public interface IBar : IDictionaryAdapter
			{
				string      Text  { get; set; }
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

				Assert.AreSame(foo.One, foo.Two);
			}

			[Test]
			public void PropertyReference_Set()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.One.Value = "One";
				foo.Two = foo.One;

				CustomAssert.AreXmlEquivalent(Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<Two x:ref='1'/>",
					"</Foo>"
				), xml);
			}

			[Test]
			public void PropertyReference_Set_ToVirtual()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.Two = foo.One;

				CustomAssert.AreXmlEquivalent(Xml(
					"<Foo $x>",
						"<Two x:ref='1'/>",
						"<One x:id='1'/>",
					"</Foo>"
				), xml);
			}

			[Test]
			public void PropertyReference_Set_ReplacingVirtual()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.One.Value = "One";
				var two = foo.Two;
				foo.Two = foo.One;

				CustomAssert.AreXmlEquivalent(Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<Two x:ref='1'/>",
					"</Foo>"
				), xml);
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

				Assert.NotNull(foo.Array);
				Assert.AreEqual(1, foo.Array.Length);
				Assert.AreSame(foo.One, foo.Array[0]);
			}

			[Test]
			public void ArrayReference_Set()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.One.Value = "One";
				foo.Array = new[] { foo.One };

				CustomAssert.AreXmlEquivalent(Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<Array>",
							"<Foo x:ref='1'/>",
						"</Array>",
					"</Foo>"
				), xml);
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

				Assert.AreEqual(1, foo.List.Count);
				Assert.AreSame(foo.One, foo.List[0]);
			}

			[Test]
			public void ListItemReference_Add()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.One.Value = "One";
				foo.List.Add(foo.One);

				CustomAssert.AreXmlEquivalent(Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<List>",
							"<Foo x:ref='1'/>",
						"</List>",
					"</Foo>"
				), xml);
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

				CustomAssert.AreXmlEquivalent(Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<List>",
							"<Foo x:ref='1'/>",
						"</List>",
					"</Foo>"
				), xml);
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

				CustomAssert.AreXmlEquivalent(Xml(
					"<Foo $x>",
						"<One> <Value>One</Value> </One>",
						"<List>",
							"<Foo> <Value>Two</Value> </Foo>",
						"</List>",
					"</Foo>"
				), xml);

				Assert.AreEqual(1, foo.List.Count);
				Assert.AreNotSame(foo.One, foo.List[0]);
				Assert.AreEqual("Two", foo.List[0].Value);
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

				CustomAssert.AreXmlEquivalent(Xml(
					"<Foo $x>",
						"<One> <Value>One</Value> </One>",
						"<List/>",
					"</Foo>"
				), xml);
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

				CustomAssert.AreXmlEquivalent(Xml(
					"<Foo $x>",
						"<One> <Value>One</Value> </One>",
						"<List/>",
					"</Foo>"
				), xml);
			}

			#region SetItemReference
#if DOTNET40
			[Test]
			public void SetItemReference_Get()
			{
				var xml = Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<Set>",
							"<Foo x:ref='1'/>",
						"</Set>",
					"</Foo>"
				);
				var foo = Create<IFoo>(xml);

				Assert.AreEqual(1, foo.Set.Count);
				CollectionAssert.Contains(foo.Set, foo.One);
			}

			[Test]
			public void Set_Assign_ExternalReference()
			{
				var xml = Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
					"</Foo>"
				);
				var foo = Create<IFoo>(xml);

				foo.Set = new HashSet<IFoo> { foo.One };

				CustomAssert.AreXmlEquivalent(Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<Set>",
							"<Foo x:ref='1'/>",
						"</Set>",
					"</Foo>"
				), xml);

				Assert.AreEqual(1, foo.Set.Count);
				CollectionAssert.Contains(foo.Set, foo.One);
			}

			[Test]
			public void Set_Assign_CrossReference()
			{
				var xmlA = Xml("<Foo $x/>");
				var xmlB = Xml("<Foo $x/>");
				var fooA = Create<IFoo>(xmlA);
				var fooB = Create<IFoo>(xmlB);

				fooA.Value = "a";
				fooB.Value = "b";
				fooA.Set = new HashSet<IFoo> { fooB };
				fooB.Set = new HashSet<IFoo> { fooA };

				CustomAssert.AreXmlEquivalent(Xml(
					"<Foo $x>",
						"<Value>a</Value>",
						"<Set>",
							"<Foo> <Value>b</Value> </Foo>",
						"</Set>",
					"</Foo>"
				), xmlA);

				CustomAssert.AreXmlEquivalent(Xml(
					"<Foo $x x:id='1'>",
						"<Value>b</Value>",
						"<Set>",
							"<Foo>",
								"<Value>a</Value>",
								"<Set>",
									"<Foo x:ref='1'/>",
								"</Set>",
							"</Foo>",
						"</Set>",
					"</Foo>"
				), xmlB);

				Assert.AreEqual(1, fooA.Set.Count);
				CollectionAssert.DoesNotContain(fooA.Set, fooB);
				Assert.AreEqual(fooB.Value, fooA.Set.First().Value);
			}

			[Test]
			public void SetItemReference_Add()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.One.Value = "One";
				foo.Set.Add(foo.One);

				CustomAssert.AreXmlEquivalent(Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<Set>",
							"<Foo x:ref='1'/>",
						"</Set>",
					"</Foo>"
				), xml);
			}

			[Test]
			public void SetItemReference_Remove()
			{
				var xml = Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<Set>",
							"<Foo x:ref='1'/>",
						"</Set>",
					"</Foo>"
				);
				var foo = Create<IFoo>(xml);

				foo.Set.Remove(foo.One);

				CustomAssert.AreXmlEquivalent(Xml(
					"<Foo $x>",
						"<One> <Value>One</Value> </One>",
						"<Set/>",
					"</Foo>"
				), xml);
			}

			[Test]
			public void SetItemReference_Clear()
			{
				var xml = Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>One</Value> </One>",
						"<Set>",
							"<Foo x:ref='1'/>",
						"</Set>",
					"</Foo>"
				);
				var foo = Create<IFoo>(xml);

				foo.Set.Clear();

				CustomAssert.AreXmlEquivalent(Xml(
					"<Foo $x>",
						"<One> <Value>One</Value> </One>",
						"<Set/>",
					"</Foo>"
				), xml);
			}
#endif
			#endregion

#if DOTNET40
			[Test]
			public void Complex_Collection_Assign_InternalReferences()
			{
			    var xml = Xml("<Foo $x/>");
			    var foo = Create<IFoo>(xml);

			    var a = Create<IFoo>(); a.Value = "a";
			    var b = Create<IFoo>();	b.Value = "b";
			    var c = Create<IFoo>();	c.Value = "c";

				var list = new List<IFoo> { a, b, c };

				foreach (var x in list)
				{
					x.Set = new HashSet<IFoo>(list.Except(Enumerable.Repeat(x, 1)));
				}

				foo.List = list;

				//a  .Set  = new HashSet<IFoo> {    b, c };
				//b  .Set  = new HashSet<IFoo> { a,    c }; // no effect, due to order of operations
				//c  .Set  = new HashSet<IFoo> { a, b    }; // no effect, due to order of operations
				//foo.List = new List   <IFoo> { a, b, c };

			    CustomAssert.AreXmlEquivalent(Xml(
			        "<Foo $x>",
			            "<List>",
			                "<Foo>",
			                    "<Value>a</Value>",
			                    "<Set>",
			                        "<Foo x:id='1'> <Value>b</Value> </Foo>",
									"<Foo x:id='2'> <Value>c</Value> </Foo>",
			                    "</Set>",
			                "</Foo>",
			                "<Foo x:ref='1'/>",
			                "<Foo x:ref='2'/>",
			            "</List>",
			        "</Foo>"
			    ), xml);

				Assert.AreEqual(3, foo.List.Count);
				Assert.AreEqual(2, foo.List[0].Set.Count);
				CollectionAssert.Contains(foo.List[0].Set, foo.List[1]);
				CollectionAssert.Contains(foo.List[0].Set, foo.List[2]);
				CollectionAssert.DoesNotContain(foo.List[0].Set, foo);
			}
#endif

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

				CustomAssert.AreXmlEquivalent(Xml(
					"<Foo $x>",
						"<One x:id='1'> <Value>X</Value> </One>",
						"<Two>",
							"<One x:ref='1'/>",
						"</Two>",
					"</Foo>"
				), xml);
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

				CustomAssert.AreXmlEquivalent(Xml(
					"<Foo $x>",
						"<One>",
							"<One x:id='1'> <Value>X</Value> </One>",
							"<Two x:ref='1'/>",
						"</One>",
					"</Foo>"
				), xml);
			}

			[Test]
			public void SelfReference_Root()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.One = foo;

				CustomAssert.AreXmlEquivalent(Xml(
					"<Foo x:id='1' $x>",
						"<One x:ref='1'/>",
					"</Foo>"
				), xml);
			}

			[Test]
			public void SelfReference_Child()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo = foo.One;
				foo.One = foo;

				CustomAssert.AreXmlEquivalent(Xml(
					"<Foo $x>",
						"<One x:id='1'>",
							"<One x:ref='1'/>",
						"</One>",
					"</Foo>"
				), xml);
			}

			[Test]
			public void IsReference_True()
			{
				var adapter  = (IDictionaryAdapter) Create<IFoo>();
				var manager  = GetReferenceManager(adapter);

				var isReference = manager.IsReferenceProperty(adapter, "Value");

				Assert.True(isReference);
			}

			[Test]
			public void IsReference_False()
			{
				var adapter  = (IDictionaryAdapter) Create<IBar>();
				var manager  = GetReferenceManager(adapter);

				var isReference = manager.IsReferenceProperty(adapter, "Value");

				Assert.False(isReference);
			}

			private static IDictionaryReferenceManager GetReferenceManager(IDictionaryAdapter dictionaryAdapter)
			{
				return dictionaryAdapter.This.Initializers
					.OfType<IDictionaryReferenceManager>().Single();
			}
		}
	}
}
#endif
