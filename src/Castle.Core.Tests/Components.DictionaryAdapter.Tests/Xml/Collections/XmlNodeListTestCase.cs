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
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using NUnit.Framework;
	using Castle.Components.DictionaryAdapter.Tests;

	[TestFixture]
	public class XmlNodeListTestCase : XmlAdapterTestCase
	{
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

		public interface IFoo : IDictionaryAdapter
		{
			IList<string> SL { get; set; }
		}
	}
}
