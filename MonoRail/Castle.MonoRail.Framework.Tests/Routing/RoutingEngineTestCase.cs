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
	public class RoutingEngineTestCase
	{
		private RoutingEngine engine;

		[SetUp]
		public void Init()
		{
			engine = new RoutingEngine();
		}

		[Test]
		public void ShouldNotMatchRulesIfItIsEmpty()
		{
			RouteMatch match = engine.FindMatch("localhost", "", "/product/1");
			Assert.IsNull(match);
		}

		[Test]
		public void ShouldMatchRulesWithCorrectUrls()
		{
			engine.Add( PatternRule.Build("ProductById", "product/<id:number>", typeof(ProductController), "View") );

			RouteMatch match = engine.FindMatch("localhost", "", "/product/1");

			Assert.IsNotNull(match);
			Assert.AreSame(typeof(ProductController), match.ControllerType);
			Assert.AreEqual("ProductById", match.RuleName);
			Assert.AreEqual("View", match.Action);
			Assert.AreEqual(1, match.Literals.Count);
			Assert.AreEqual(1, match.Parameters.Count);
			Assert.AreEqual("product", match.Literals[0]);
			Assert.AreEqual("1", match.Parameters["id"]);
		}

		[Test]
		public void ShouldMatchRulesWithCorrectUrls2()
		{
			engine.Add(PatternRule.Build("ProductByName", "product/<name>", typeof(ProductController), "View"));

			RouteMatch match = engine.FindMatch("localhost", "", "/product/iPod");

			Assert.IsNotNull(match);
			Assert.AreSame(typeof(ProductController), match.ControllerType);
			Assert.AreEqual("ProductByName", match.RuleName);
			Assert.AreEqual("View", match.Action);
			Assert.AreEqual(1, match.Literals.Count);
			Assert.AreEqual(1, match.Parameters.Count);
			Assert.AreEqual("product", match.Literals[0]);
			Assert.AreEqual("iPod", match.Parameters["name"]);
		}

		[Test]
		public void ShouldMatchRulesWithCorrectUrls3()
		{
			engine.Add(PatternRule.Build("Product", "product/<brand>/<type>/<name>", typeof(ProductController), "View"));
			engine.Add(PatternRule.Build("ProductByBrandType", "product/<brand>/<type>", typeof(ProductController), "View"));
			engine.Add(PatternRule.Build("ProductByBrand", "product/<brand>", typeof(ProductController), "View"));

			RouteMatch match = engine.FindMatch("localhost", "", "/product/apple/macbook/pro");

			Assert.IsNotNull(match);
			Assert.AreSame(typeof(ProductController), match.ControllerType);
			Assert.AreEqual("Product", match.RuleName);
			Assert.AreEqual("View", match.Action);
			Assert.AreEqual(1, match.Literals.Count);
			Assert.AreEqual(3, match.Parameters.Count);
			Assert.AreEqual("product", match.Literals[0]);
			Assert.AreEqual("apple", match.Parameters["brand"]);
			Assert.AreEqual("macbook", match.Parameters["type"]);
			Assert.AreEqual("pro", match.Parameters["name"]);
		}

		[Test]
		public void ShouldAcceptRoutesAsPath()
		{
			engine.Add(PatternRule.Build("ProductById", "product/", typeof(ProductController), "View"));
		}

		[Test]
		public void ShouldMatchAsPathAndAsFile()
		{
			engine.Add(PatternRule.Build("ProductById", "product", typeof(ProductController), "View"));

			RouteMatch match = engine.FindMatch("localhost", "", "/product");
			Assert.IsNotNull(match);

			match = engine.FindMatch("localhost", "", "/product/");
			Assert.IsNotNull(match);
		}

		public class ProductController : Controller {}
	}
}
