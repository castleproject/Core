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
	using System.Linq;
	using System.Xml.XPath;
	using System.Xml.Xsl;
	using Castle.Components.DictionaryAdapter.Tests;
    using NUnit.Framework;

	public class XPathBehaviorTestCase
	{
		[TestFixture]
		public class CreatableNodeSetExpression : XmlAdapterTestCase
		{
			public interface IFoo : IDictionaryAdapter
			{
				[XPath("A[B='b']/C[D[@E][F='f'] and G]/@H")]
				string Value { get; set; }
			}

			[Test]
			public void Get_Existing()
			{
				var foo = Create<IFoo>
				(
					"<Foo>",
						"<A>",
							"<B>b</B>",
							"<C H='value'>",
								"<D E='?'>",
									"<F>f</F>",
								"</D>",
								"<G/>",
							"</C>",
						"</A>",
					"</Foo>"
				);

				Assert.AreEqual("value", foo.Value);
			}

			[Test]
			public void Get_Missing()
			{
				var foo = Create<IFoo>("<Foo/>");

				Assert.IsNull(foo.Value);
			}

			[Test]
			public void Set()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.Value = "value";

				CustomAssert.AreXmlEquivalent(string.Concat
				(
					"<Foo>",
						"<A>",
							"<C H='value'>",
								"<D E=''>",
									"<F>f</F>",
								"</D>",
								"<G/>",
							"</C>",
							"<B>b</B>",
						"</A>",
					"</Foo>"
				), xml);
				Assert.AreEqual("value", foo.Value);
			}
		}

		[TestFixture]
		public class CreatableNodeSetExpressionWithSelfReference : XmlAdapterTestCase
		{
			public interface IFoo : IDictionaryAdapter
			{
				[XPath("A[B/C[.='2']]/@D")]
				string Value { get; set; }
			}

			[Test]
			public void Get_Existing()
			{
				var foo = Create<IFoo>
				(
					"<Foo>",
						"<A D='value'>",
							"<B>",
								"<C>2</C>",
							"</B>",
						"</A>",
					"</Foo>"
				);

				Assert.AreEqual("value", foo.Value);
			}

			[Test]
			public void Get_Missing()
			{
				var foo = Create<IFoo>("<Foo/>");

				Assert.IsNull(foo.Value);
			}

			[Test]
			public void Set()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.Value = "value";

				CustomAssert.AreXmlEquivalent(string.Concat
				(
					"<Foo>",
						"<A D='value'>",
							"<B>",
								"<C>2</C>",
							"</B>",
						"</A>",
					"</Foo>"
				), xml);
				Assert.AreEqual("value", foo.Value);
			}
		}

		[TestFixture]
		public class NoncreatableNodeSetExpression : XmlAdapterTestCase
		{
			public interface IFoo : IDictionaryAdapter
			{
				[XPath("//*[local-name()='A']")]
				string Value { get; set; }
			}

			[Test]
			public void Get_Existing()
			{
				var obj = Create<IFoo>("<Foo xmlns:p='urn:a'> <X> <p:A>value</p:A> </X> </Foo>");

				Assert.AreEqual("value", obj.Value);
			}

			[Test]
			public void Get_Missing()
			{
				var obj = Create<IFoo>("<Foo/>");

				Assert.IsNull(obj.Value);
			}

			[Test]
			public void Set()
			{
				var obj = Create<IFoo>("<Foo/>");

				Assert.Throws<XPathException>(() => obj.Value = "value");
			}
		}

		[TestFixture]
		public class ValueExpression : XmlAdapterTestCase
		{
			public interface IFoo : IDictionaryAdapter
			{
				[XPath("string(A)")]
				string StringValue { get; set; }

				[XPath("count(A)")]
				int NumberValue { get; set; }

				[XPath("A='a'")]
				bool BooleanValue { get; set; }
			}

			[Test]
			public void Get_OnVirtual()
			{
				var foo = Create<IFoo>();

				Assert.IsNull(foo.StringValue);
				Assert.AreEqual(0, foo.NumberValue);
				Assert.False(foo.BooleanValue);
			}

			[Test]
			public void Get_OnActual()
			{
				var foo = Create<IFoo>("<Foo> <A>a</A> </Foo>");

				Assert.AreEqual("a", foo.StringValue);
				Assert.AreEqual(1, foo.NumberValue);
				Assert.True(foo.BooleanValue);
			}

			[Test]
			public void Set()
			{
				var foo = Create<IFoo>("<Foo/>");

				Assert.Throws<XPathException>(() => foo.StringValue  = "a");
				Assert.Throws<XPathException>(() => foo.NumberValue  = 1);
				Assert.Throws<XPathException>(() => foo.BooleanValue = true);
			}

			[Test]
			public void HasProperty()
			{
				var foo = Create<IFoo>("<Foo> <A>a</A> </Foo>");

				Assert.False(XmlAdapter.For(foo).HasProperty("StringValue",  foo));
				Assert.False(XmlAdapter.For(foo).HasProperty("NumberValue",  foo));
				Assert.False(XmlAdapter.For(foo).HasProperty("BooleanValue", foo));
			}
		}

		[TestFixture]
		public class VirtualComponent : XmlAdapterTestCase
		{
			public interface IFoo
			{
				[XPath("A/B")]
				IBar Bar { get; set; }
			}

			public interface IBar
			{
				[XPath("C")]
				string Value { get; set; }
			}

			[Test]
			public void Realize_Missing()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				var bar = foo.Bar;
				CustomAssert.AreXmlEquivalent("<Foo/>", xml);
				Assert.NotNull(bar);
				Assert.AreSame(foo.Bar, bar);
				Assert.IsNull(bar.Value);

				bar.Value = "value";
				CustomAssert.AreXmlEquivalent("<Foo> <A> <B> <C>value</C> </B> </A> </Foo>", xml);
				Assert.NotNull(bar);
				Assert.AreSame(foo.Bar, bar);
				Assert.AreEqual("value", bar.Value);
			}

			[Test]
			public void Realize_Partial()
			{
				var xml = Xml("<Foo> <A> <X/> </A> </Foo>");
				var foo = Create<IFoo>(xml);

				var bar = foo.Bar;
				CustomAssert.AreXmlEquivalent("<Foo> <A> <X/> </A> </Foo>", xml);
				Assert.NotNull(bar);
				Assert.AreSame(foo.Bar, bar);
				Assert.IsNull(bar.Value);

				bar.Value = "value";
				CustomAssert.AreXmlEquivalent("<Foo> <A> <X/> <B> <C>value</C> </B> </A> </Foo>", xml);
				Assert.NotNull(bar);
				Assert.AreSame(foo.Bar, bar);
				Assert.AreEqual("value", bar.Value);
			}

			[Test]
			public void SelectOnVirtual()
			{
				var xml = Xml("<Foo> <A/> </Foo>");
				var foo = Create<IFoo>(xml);

				var bar = foo.Bar;
				CustomAssert.AreXmlEquivalent("<Foo> <A/> </Foo>", xml);
				Assert.NotNull(bar);
				Assert.AreSame(foo.Bar, bar);

				var value = bar.Value;
				CustomAssert.AreXmlEquivalent("<Foo> <A/> </Foo>", xml);
				Assert.IsNull(value);

				bar.Value = "value";
				CustomAssert.AreXmlEquivalent("<Foo> <A> <B> <C>value</C> </B> </A> </Foo>", xml);
				Assert.NotNull(bar);
				Assert.AreSame(foo.Bar, bar);
				Assert.AreEqual("value", bar.Value);
			}
		}

		[TestFixture]
		public class PartialPaths : XmlAdapterTestCase
		{
			public interface IFoo
			{
				[XPath("A/B[@Id='2']/C")]
				string A { get; set; }
			}

			[Test]
			public void Delete_NotDoAnything()
			{
				var xml = Xml
				(
					"<Foo>",
						"<A>",
							"<B Id='1'> <C>value1</C> </B>",
						"</A>",
					"</Foo>"
				);

				Create<IFoo>(xml).A = null;

				CustomAssert.AreXmlEquivalent(string.Concat
				(
					"<Foo>",
						"<A>",
							"<B Id='1'> <C>value1</C> </B>",
						"</A>",
					"</Foo>"
				), xml);
			}

			[Test]
			public void Delete_Partial()
			{
				var xml = Xml
				(
					"<Foo>",
						"<A>",
							"<B Id='1'> <C>value1</C> </B>",
							"<B Id='2'> <C>value2</C> </B>",
						"</A>",
					"</Foo>"
				);

				Create<IFoo>(xml).A = null;

				CustomAssert.AreXmlEquivalent(string.Concat
				(
					"<Foo>",
						"<A>",
							"<B Id='1'> <C>value1</C> </B>",
						"</A>",
					"</Foo>"
				), xml);
			}

			[Test]
			public void Delete_Whole()
			{
				var xml = Xml
				(
					"<Foo>",
						"<A>",
							"<B Id='2'> <C>value2</C> </B>",
						"</A>",
					"</Foo>"
				);

				Create<IFoo>(xml).A = null;

				CustomAssert.AreXmlEquivalent("<Foo/>", xml);
			}
		}

		[TestFixture]
		public class SeparateGetterSetter_SimpleType : XmlAdapterTestCase
		{
			public interface IFoo
			{
				[XPath(get: "X", set: "Y")]
				string A { get; set; }
			}

			[Test]
			public void Get_NotPreviouslySet()
			{
				var foo = Create<IFoo>("<Foo> <X>x</X> </Foo>");

				var a = foo.A;

				Assert.AreEqual("x", a);
			}

			[Test]
			public void Get_PreviouslySet()
			{
				var foo = Create<IFoo>("<Foo> <X>x</X> <Y>y</Y> </Foo>");

				var a = foo.A;

				Assert.AreEqual("y", a);
			}

			[Test]
			public void Set()
			{
				var xml = Xml("<Foo> <X>x</X> <Y>y</Y> </Foo>");
				var foo = Create<IFoo>(xml);

				foo.A = "*";

				CustomAssert.AreXmlEquivalent("<Foo> <X>x</X> <Y>*</Y> </Foo>", xml);
			}
		}

		[TestFixture]
		public class SeparateGetterSetter_ComplexType : XmlAdapterTestCase
		{
			public interface IFoo
			{
				[XPath(get: "X", set: "Y")]
				IBar A { get; set; }
			}

			public interface IBar { }

			[Test]
			public void Get()
			{
				var foo = Create<IFoo>("<Foo> <X>x</X> <Y>y</Y> </Foo>");

				Assert.Throws<InvalidOperationException>(() =>
				{
					var a = foo.A;
				});
			}

			[Test]
			public void Set()
			{
				var xml = Xml("<Foo> <X>x</X> <Y>y</Y> </Foo>");
				var foo = Create<IFoo>(xml);

				Assert.Throws<InvalidOperationException>(() =>
				{
					foo.A = Create<IBar>();
				});
			}

		}

		[TestFixture]
		public class WithVariable_DefinedOnType : WithVariableOrFunction
		{
			[WithMockVariable]
			public interface IFoo
			{
				[XPath("A[@B=$p:v]")]
				string Item { get; set; }
			}

			[Test]
			public void Get()
			{
				TestGet<IFoo>(f => f.Item);
			}

			[Test]
			public void Set()
			{
				TestSet<IFoo>((f, v) => f.Item = v);
			}

			[Test]
			public void VariableMetadata()
			{
				var variable = typeof(IFoo)
					.GetCustomAttributes(false)
					.OfType<IXsltContextVariable>()
					.Single();

				Assert.False(variable.IsParam);
				Assert.False(variable.IsLocal);
			}
		}

		[TestFixture]
		public class WithVariable_DefinedOnProperty : WithVariableOrFunction
		{
			public interface IFoo
			{
				[XPath("A[@B=$p:v]"), WithMockVariable]
				string Item { get; set; }
			}

			[Test]
			public void Get()
			{
				TestGet<IFoo>(f => f.Item);
			}

			[Test]
			public void Set()
			{
				TestSet<IFoo>((f, v) => f.Item = v);
			}
		}

		[TestFixture]
		public class WithFunction_DefinedOnType : WithVariableOrFunction
		{
			[WithMockFunction]
			public interface IFoo
			{
				[XPath("A[@B=p:f('a')]")]
				string Item { get; }
			}

			[Test]
			public void Get()
			{
				TestGet<IFoo>(f => f.Item);
			}

			[Test]
			public void FunctionMetadata()
			{
				var function = typeof(IFoo)
					.GetCustomAttributes(false)
					.OfType<IXsltContextFunction>()
					.Single();

				Assert.NotNull(function.ArgTypes);
				Assert.AreEqual(1, function.ArgTypes.Length);
				Assert.AreEqual(XPathResultType.String, function.ArgTypes[0]);
				Assert.AreEqual(1, function.Minargs);
				Assert.AreEqual(1, function.Maxargs);
			}
		}

		[TestFixture]
		public class WithFunction_DefinedOnProperty : WithVariableOrFunction
		{
			public interface IFoo
			{
				[XPath("A[@B=p:f('a')]"), WithMockFunction]
				string Item { get; }
			}

			[Test]
			public void Get()
			{
				TestGet<IFoo>(f => f.Item);
			}
		}

		public class WithMockVariableAttribute : XPathVariableAttribute
		{
			public override XmlName Name { get { return new XmlName("v", "p"); } }
			public override XPathResultType VariableType { get { return XPathResultType.String; } }

			public override object Evaluate(XsltContext context)
			{
				return "value";
			}
		}

		public class WithMockFunctionAttribute : XPathFunctionAttribute
		{
			public override XmlName Name { get { return new XmlName("f", "p"); } }
			public override XPathResultType ReturnType { get { return XPathResultType.String; } }
			public override XPathResultType[] ArgTypes { get { return new[] { XPathResultType.String }; } }

			public override object Invoke(XsltContext context, object[] args, XPathNavigator node)
			{
				Assert.NotNull(args);
				Assert.AreEqual(1, args.Length);
				Assert.AreEqual("a", args[0]);
				return "value";
			}
		}

		public abstract class WithVariableOrFunction : XmlAdapterTestCase
		{
			protected void TestGet<T>(Func<T, string> getter)
			{
				var xml = Xml
				(
					"<Foo>",
						"<A B='other'>wrong A</A>",
						"<A B='value'>correct</A>",
						"<A B='other'>wrong B</A>",
					"</Foo>"
				);
				var obj = Create<T>(xml);

				var value = getter(obj);

				Assert.AreEqual("correct", value);
			}

			protected void TestSet<T>(Action<T, string> setter)
			{
				var xml = Xml("<Foo/>");
				var obj = Create<T>(xml);

				setter(obj, "correct");

				CustomAssert.AreXmlEquivalent(string.Concat
				(
					"<Foo>",
						"<A B='value'>correct</A>",
					"</Foo>"
				), xml);
			}
		}
	}
}
