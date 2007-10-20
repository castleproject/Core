// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.Routing
{
	using Castle.MonoRail.Framework.Routing;
	using NUnit.Framework;

	[TestFixture]
	public class RegexRuleTestCase
	{
		[Test]
		public void ShouldMatchAndGenerateAnInformativeResult()
		{
			RegexRule rule = RegexRule.Build("ProductById", "product/(?<id>\\d+)", typeof(ProductController), "View");

			RouteMatch match = new RouteMatch(typeof(ProductController), "name", "view");

			Assert.IsFalse(rule.Matches("localhost", "", "product/", new RouteMatch(typeof(ProductController), "name", "view")));
			Assert.IsFalse(rule.Matches("localhost", "", "product/iPod", new RouteMatch(typeof(ProductController), "name", "view")));
			Assert.IsTrue(rule.Matches("localhost", "", "product/1", match));

			Assert.AreEqual(0, match.Literals.Count);
			Assert.AreEqual(2, match.Parameters.Count);
			Assert.AreEqual("1", match.Parameters["id"]);
		}

		[Test]
		public void ShouldMatchNamedGroups()
		{
			RegexRule rule = RegexRule.Build("ProductById", "product/(?<id>\\d+)/(?<page>\\d+)", typeof(ProductController), "View");

			RouteMatch match = new RouteMatch(typeof(ProductController), "name", "view");

			Assert.IsFalse(rule.Matches("localhost", "", "product/", new RouteMatch(typeof(ProductController), "name", "view")));
			Assert.IsFalse(rule.Matches("localhost", "", "product/iPod", new RouteMatch(typeof(ProductController), "name", "view")));
			Assert.IsFalse(rule.Matches("localhost", "", "product/1", new RouteMatch(typeof(ProductController), "name", "view")));
			Assert.IsTrue(rule.Matches("localhost", "", "product/12/10", match));

			Assert.AreEqual(0, match.Literals.Count);
			Assert.AreEqual(3, match.Parameters.Count);
			Assert.AreEqual("12", match.Parameters["id"]);
			Assert.AreEqual("10", match.Parameters["page"]);
		}

		public class ProductController : Controller { }
	}
}
