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
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using NUnit.Framework;
	using Castle.Components.DictionaryAdapter.Tests;

	[TestFixture]
	public class XmlNodeListTestCase : XmlAdapterTestCase
	{
		[Test]
		public void Get_Element()
		{
			var foo = Create<IFoo>(StringsXml);

			var items = foo.Strings.ToArray();

			CollectionAssert.AreEquivalent(Strings, items);
		}

		[Test]
		public void Get_Attribute()
		{
			var foo = Create<IFoo>("<Foo F='1'/>");

			CollectionAssert.IsEmpty(foo.Strings);
		}

		[Test]
		public void Set()
		{
			var xml = Xml("<Foo/>");
			var foo = Create<IFoo>(xml);

			foo.Strings = Strings;

			CustomAssert.AreXmlEquivalent(StringsXml, xml);
		}

		[Test]
		public void SetItemToNull()
		{
			var xml = Xml(StringsXml);
			var foo = Create<IFoo>(xml);

			foo.Strings[0] = null;
			var value = foo.Strings[0];

			// TODO: Why is xmlns:xsi not pushed to root?
			CustomAssert.AreXmlEquivalent(Xml("<Foo $xsi> <Strings> <string xsi:nil='true'/> <string>b</string> </Strings> </Foo>"), xml);
			Assert.IsNull(value);
		}

		[Test]
		public void Insert()
		{
			var xml = Xml(StringsXml);
			var foo = Create<IFoo>(xml);

			foo.Strings.Insert(1, "c");
			var value = foo.Strings[1];

			CustomAssert.AreXmlEquivalent(Xml("<Foo> <Strings> <string>a</string> <string>c</string> <string>b</string> </Strings> </Foo>"), xml);
			Assert.AreEqual("c", value);
		}

		private IList<string> GetListOfStrings()
		{
			return Create<IFoo>(StringsXml).Strings;
		}

		public interface IFoo : IDictionaryAdapter
		{
			IList<string> Strings { get; set; }
		}

		public interface IHasItems
		{
			IList<IItem> List { get; set; }
		}

		public interface IItem
		{
			string Value { get; set; }
		}

		private static readonly IList<string> Strings
			= Array.AsReadOnly(new[] { "a", "b" });

		private static readonly string StringsXml = string.Concat
		(
			"<Foo>",
				"<Strings>",
					"<string>a</string>",
					"<string>b</string>",
				"</Strings>",
			"</Foo>"
		);
	}
}
#endif
