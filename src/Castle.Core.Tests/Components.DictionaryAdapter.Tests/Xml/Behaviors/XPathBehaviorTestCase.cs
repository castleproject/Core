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

				Assert.That(foo.Value, Is.EqualTo("value"));
			}

			[Test]
			public void Get_Missing()
			{
				var foo = Create<IFoo>("<Foo/>");

				Assert.That(foo.Value, Is.Null);
			}

			[Test]
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

				Assert.That(obj.Value, Is.EqualTo("value"));
			}

			[Test]
			public void Get_Missing()
			{
				var obj = Create<IFoo>("<Foo/>");

				Assert.That(obj.Value, Is.Null);
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
			public void Get_String()
			{
				var foo = Create<IFoo>("<Foo> <A>a</A> </Foo>");

				Assert.That(foo.StringValue, Is.EqualTo("a"));
			}

			[Test]
			public void Get_Number()
			{
				var foo = Create<IFoo>("<Foo> <A/> <A/> <A/> </Foo>");

				Assert.That(foo.NumberValue, Is.EqualTo(3));
			}

			[Test]
			public void Get_Boolean()
			{
				var foo = Create<IFoo>("<Foo> <A>a</A> </Foo>");

				Assert.That(foo.BooleanValue, Is.EqualTo(true));
			}

			[Test]
			public void SetProperty()
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

			[Test]
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

			[Test]
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

				Assert.That(a, Is.EqualTo("x"));
			}

			[Test]
			public void Get_PreviouslySet()
			{
				var foo = Create<IFoo>("<Foo> <X>x</X> <Y>y</Y> </Foo>");

				var a = foo.A;

				Assert.That(a, Is.EqualTo("y"));
			}

			[Test]
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
