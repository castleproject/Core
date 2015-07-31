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
	using System.Linq;
	using System.Xml.XPath;
	using System.Xml.Xsl;
	using Castle.Components.DictionaryAdapter.Tests;
    using Xunit;

	public class XPathBehaviorTestCase
	{
				public class CreatableNodeSetExpression : XmlAdapterTestCase
		{
			public interface IFoo : IDictionaryAdapter
			{
				[XPath("A[B='b']/C[D[@E][F='f'] and G]/@H")]
				string Value { get; set; }
			}

			[Fact]
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

				Assert.That(foo.Value, Is.EqualTo("value"));
			}

			[Fact]
			public void Get_Missing()
			{
				var foo = Create<IFoo>("<Foo/>");

				Assert.That(foo.Value, Is.Null);
			}

			[Fact]
			public void Set()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.Value = "value";

				Assert.That(xml, XmlEquivalent.To
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
				));
				Assert.That(foo.Value, Is.EqualTo("value"));
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

			[Fact]
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

				Assert.That(foo.Value, Is.EqualTo("value"));
			}

			[Fact]
			public void Get_Missing()
			{
				var foo = Create<IFoo>("<Foo/>");

				Assert.That(foo.Value, Is.Null);
			}

			[Fact]
			public void Set()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				foo.Value = "value";

				Assert.That(xml, XmlEquivalent.To
				(
					"<Foo>",
						"<A D='value'>",
							"<B>",
								"<C>2</C>",
							"</B>",
						"</A>",
					"</Foo>"
				));
				Assert.That(foo.Value, Is.EqualTo("value"));
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

			[Fact]
			public void Get_Existing()
			{
				var obj = Create<IFoo>("<Foo xmlns:p='urn:a'> <X> <p:A>value</p:A> </X> </Foo>");

				Assert.That(obj.Value, Is.EqualTo("value"));
			}

			[Fact]
			public void Get_Missing()
			{
				var obj = Create<IFoo>("<Foo/>");

				Assert.That(obj.Value, Is.Null);
			}

			[Fact]
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

			[Fact]
			public void Get_OnVirtual()
			{
				var foo = Create<IFoo>();

				Assert.That(foo.StringValue,  Is.Null);
				Assert.That(foo.NumberValue,  Is.EqualTo(0));
				Assert.That(foo.BooleanValue, Is.False);
			}

			[Fact]
			public void Get_OnActual()
			{
				var foo = Create<IFoo>("<Foo> <A>a</A> </Foo>");

				Assert.That(foo.StringValue,  Is.EqualTo("a"));
				Assert.That(foo.NumberValue,  Is.EqualTo(1));
				Assert.That(foo.BooleanValue, Is.EqualTo(true));
			}

			[Fact]
			public void Set()
			{
				var foo = Create<IFoo>("<Foo/>");

				Assert.Throws<XPathException>(() => foo.StringValue  = "a");
				Assert.Throws<XPathException>(() => foo.NumberValue  = 1);
				Assert.Throws<XPathException>(() => foo.BooleanValue = true);
			}

			[Fact]
			public void HasProperty()
			{
				var foo = Create<IFoo>("<Foo> <A>a</A> </Foo>");

				Assert.That(XmlAdapter.For(foo).HasProperty("StringValue",  foo), Is.False);
				Assert.That(XmlAdapter.For(foo).HasProperty("NumberValue",  foo), Is.False);
				Assert.That(XmlAdapter.For(foo).HasProperty("BooleanValue", foo), Is.False);
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

			[Fact]
			public void Realize_Missing()
			{
				var xml = Xml("<Foo/>");
				var foo = Create<IFoo>(xml);

				var bar = foo.Bar;
				Assert.That(xml,       XmlEquivalent.To("<Foo/>"));
				Assert.That(bar,       Is.Not.Null & Is.SameAs(foo.Bar));
				Assert.That(bar.Value, Is.Null);

				bar.Value = "value";
				Assert.That(xml,       XmlEquivalent.To("<Foo> <A> <B> <C>value</C> </B> </A> </Foo>"));
				Assert.That(bar,       Is.Not.Null & Is.SameAs(foo.Bar));
				Assert.That(bar.Value, Is.EqualTo("value"));
			}

			[Fact]
			public void Realize_Partial()
			{
				var xml = Xml("<Foo> <A> <X/> </A> </Foo>");
				var foo = Create<IFoo>(xml);

				var bar = foo.Bar;
				Assert.That(xml,       XmlEquivalent.To("<Foo> <A> <X/> </A> </Foo>"));
				Assert.That(bar,       Is.Not.Null & Is.SameAs(foo.Bar));
				Assert.That(bar.Value, Is.Null);

				bar.Value = "value";
				Assert.That(xml,       XmlEquivalent.To("<Foo> <A> <X/> <B> <C>value</C> </B> </A> </Foo>"));
				Assert.That(bar,       Is.Not.Null & Is.SameAs(foo.Bar));
				Assert.That(bar.Value, Is.EqualTo("value"));
			}

			[Fact]
			public void SelectOnVirtual()
			{
				var xml = Xml("<Foo> <A/> </Foo>");
				var foo = Create<IFoo>(xml);

				var bar = foo.Bar;
				Assert.That(xml,       XmlEquivalent.To("<Foo> <A/> </Foo>"));
				Assert.That(bar,       Is.Not.Null & Is.SameAs(foo.Bar));

				var value = bar.Value;
				Assert.That(xml,       XmlEquivalent.To("<Foo> <A/> </Foo>"));
				Assert.That(value,     Is.Null);

				bar.Value = "value";
				Assert.That(xml,       XmlEquivalent.To("<Foo> <A> <B> <C>value</C> </B> </A> </Foo>"));
				Assert.That(bar,       Is.Not.Null & Is.SameAs(foo.Bar));
				Assert.That(bar.Value, Is.EqualTo("value"));
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

			[Fact]
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

				Assert.That(xml, XmlEquivalent.To
				(
					"<Foo>",
						"<A>",
							"<B Id='1'> <C>value1</C> </B>",
						"</A>",
					"</Foo>"
				));
			}

			[Fact]
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

				Assert.That(xml, XmlEquivalent.To
				(
					"<Foo>",
						"<A>",
							"<B Id='1'> <C>value1</C> </B>",
						"</A>",
					"</Foo>"
				));
			}

			[Fact]
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

				Assert.That(xml, XmlEquivalent.To("<Foo/>"));
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

			[Fact]
			public void Get_NotPreviouslySet()
			{
				var foo = Create<IFoo>("<Foo> <X>x</X> </Foo>");

				var a = foo.A;

				Assert.That(a, Is.EqualTo("x"));
			}

			[Fact]
			public void Get_PreviouslySet()
			{
				var foo = Create<IFoo>("<Foo> <X>x</X> <Y>y</Y> </Foo>");

				var a = foo.A;

				Assert.That(a, Is.EqualTo("y"));
			}

			[Fact]
			public void Set()
			{
				var xml = Xml("<Foo> <X>x</X> <Y>y</Y> </Foo>");
				var foo = Create<IFoo>(xml);

				foo.A = "*";

				Assert.That(xml, XmlEquivalent.To("<Foo> <X>x</X> <Y>*</Y> </Foo>"));
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

			[Fact]
			public void Get()
			{
				var foo = Create<IFoo>("<Foo> <X>x</X> <Y>y</Y> </Foo>");

				Assert.Throws<InvalidOperationException>(() =>
				{
					var a = foo.A;
				});
			}

			[Fact]
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

			[Fact]
			public void Get()
			{
				TestGet<IFoo>(f => f.Item);
			}

			[Fact]
			public void Set()
			{
				TestSet<IFoo>((f, v) => f.Item = v);
			}

			[Fact]
			public void VariableMetadata()
			{
				var variable = typeof(IFoo)
					.GetCustomAttributes(false)
					.OfType<IXsltContextVariable>()
					.Single();

				Assert.That(variable.IsParam, Is.False);
				Assert.That(variable.IsLocal, Is.False);
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

			[Fact]
			public void Get()
			{
				TestGet<IFoo>(f => f.Item);
			}

			[Fact]
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

			[Fact]
			public void Get()
			{
				TestGet<IFoo>(f => f.Item);
			}

			[Fact]
			public void FunctionMetadata()
			{
				var function = typeof(IFoo)
					.GetCustomAttributes(false)
					.OfType<IXsltContextFunction>()
					.Single();

				Assert.That(function.ArgTypes,    Is.Not.Null & Has.Length.EqualTo(1));
				Assert.That(function.ArgTypes[0], Is.EqualTo(XPathResultType.String));
				Assert.That(function.Minargs,     Is.EqualTo(1));
				Assert.That(function.Maxargs,     Is.EqualTo(1));
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

			[Fact]
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
				Assert.That(args,    Is.Not.Null & Has.Length.EqualTo(1));
				Assert.That(args[0], Is.EqualTo("a"));
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

				Assert.That(value, Is.EqualTo("correct"));
			}

			protected void TestSet<T>(Action<T, string> setter)
			{
				var xml = Xml("<Foo/>");
				var obj = Create<T>(xml);

				setter(obj, "correct");

				Assert.That(xml, XmlEquivalent.To
				(
					"<Foo>",
						"<A B='value'>correct</A>",
					"</Foo>"
				));
			}
		}
	}
}
#endif
