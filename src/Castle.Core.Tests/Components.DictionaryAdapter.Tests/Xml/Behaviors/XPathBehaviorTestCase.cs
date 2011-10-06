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
	using System.Xml.Serialization;
	using System.Xml.XPath;
	using System.Xml.Xsl;
	using Castle.Components.DictionaryAdapter.Tests;
    using NUnit.Framework;

	public class XPathBehaviorTestCase
	{
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
	}
}
