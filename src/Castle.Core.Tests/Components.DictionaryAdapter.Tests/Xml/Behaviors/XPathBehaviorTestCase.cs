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
	using System.Xml.XPath;
	using System.Xml.Xsl;
	using Castle.Components.DictionaryAdapter.Tests;
    using NUnit.Framework;

	public class XPathBehaviorTestCase
	{
		[TestFixture]
		public class WithVariable_DefinedOnProperty : XmlAdapterTestCase
		{
			public interface IFoo
			{
				[XPath("A[@B=$p:v]"), WithMockVariable]
				string Item { get; set; }
			}

			[Test]
			public void Get()
			{
				var xml = Xml
				(
					"<Foo>",
						"<A B='other'>wrong A</A>",
						"<A B='value'>correct</A>",
						"<A B='other'>wrong B</A>",
					"</Foo>"
				);
				var obj = Create<IFoo>(xml);

				var value = obj.Item;

				Assert.That(value, Is.EqualTo("correct"));
			}

			[Test]
			public void Set()
			{
				var xml = Xml("<Foo/>");
				var obj = Create<IFoo>(xml);

				obj.Item = "correct";

				Assert.That(xml, XmlEquivalent.To
				(
					"<Foo>",
						"<A B='value'>correct</A>",
					"</Foo>"
				));
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
		}

		[TestFixture]
		public class WithFunction_DefinedOnProperty : XmlAdapterTestCase
		{
			public interface IFoo
			{
				[XPath("A[@B=p:f()]"), WithMockFunction]
				string Item { get; }
			}

			[Test]
			public void Get()
			{
				var xml = Xml
				(
					"<Foo>",
						"<A B='other'>wrong A</A>",
						"<A B='value'>correct</A>",
						"<A B='other'>wrong B</A>",
					"</Foo>"
				);
				var obj = Create<IFoo>(xml);

				var value = obj.Item;

				Assert.That(value, Is.EqualTo("correct"));
			}

			public class WithMockFunctionAttribute : XPathFunctionAttribute
			{
				public override XmlName Name { get { return new XmlName("f", "p"); } }
				public override XPathResultType ReturnType { get { return XPathResultType.String; } }

				public override object Invoke(XsltContext context, object[] args, XPathNavigator node)
				{
					return "value";
				}
			}
		}
	}
}
