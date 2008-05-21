// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Tests.Services
{
	using System.Collections.Specialized;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Routing;
	using Castle.MonoRail.Framework.Services;
	using NUnit.Framework;
	using Test;

	[TestFixture]
	public class DefaultUrlBuilderRouteIntegrationTestCase
	{
		private DefaultUrlBuilder urlBuilder;
		private RoutingEngine engine;

		[SetUp]
		public void Init()
		{
			engine = new RoutingEngine();
			urlBuilder = new DefaultUrlBuilder();
			urlBuilder.ServerUtil = new MockServerUtility();
			urlBuilder.RoutingEngine = engine;
		}

		[Test]
		public void ShouldTryToBuildUrlUsingMatchingRoutingRule()
		{
			engine.Add(new PatternRoute("/<area>/<controller>/something/<action>/[id]"));
			engine.Add(new PatternRoute("/<controller>/something/<action>/[id]"));

			UrlInfo url = new UrlInfo("", "controller", "action", "", ".castle");

			HybridDictionary dict = new HybridDictionary(true);
			dict["controller"] = "cart";
			dict["action"] = "new";
			dict["params"] = DictHelper.Create("id=10");

			Assert.AreEqual("/cart/something/new/10", urlBuilder.BuildUrl(url, dict));
		}

		[Test]
		public void ShouldBeAbleToConstructAnAbsoluteURL()
		{
			engine.Add(new PatternRoute("/<area>/<controller>/something/<action>/[id]"));
			engine.Add(new PatternRoute("/<controller>/something/<action>/[id]"));

			UrlInfo url = new UrlInfo("domain.com", null, "", "http", 80, "", "", "controller", "action", ".castle", null);

			HybridDictionary dict = new HybridDictionary(true);
			dict["absolute"] = "true";
			dict["controller"] = "cart";
			dict["action"] = "new";
			dict["params"] = DictHelper.Create("id=10");

			Assert.AreEqual("http://domain.com/cart/something/new/10", urlBuilder.BuildUrl(url, dict));
		}

		[Test]
		public void ShouldBeAbleToConstructAnAbsoluteURLWithAppVirtualDir()
		{
			engine.Add(new PatternRoute("/<area>/<controller>/something/<action>/[id]"));
			engine.Add(new PatternRoute("/<controller>/something/<action>/[id]"));

			UrlInfo url = new UrlInfo("domain.com", null, "someproject", "http", 80, "", "", "controller", "action", ".castle", null);

			HybridDictionary dict = new HybridDictionary(true);
			dict["absolute"] = "true";
			dict["controller"] = "cart";
			dict["action"] = "new";
			dict["params"] = DictHelper.Create("id=10");

			Assert.AreEqual("http://domain.com/someproject/cart/something/new/10", urlBuilder.BuildUrl(url, dict));
		}

		[Test]
		public void OptionalPartsShouldMatchOnlyIfExplictlyPresent()
		{
			engine.Add(new PatternRoute("/<controller>/something/[action]/[id]"));

			UrlInfo url = new UrlInfo("", "controller", "action", "", ".castle");

			HybridDictionary dict = new HybridDictionary(true);
			dict["controller"] = "home";

			Assert.AreEqual("/home/something",
							urlBuilder.BuildUrl(url, dict));
		}

		[Test]
		public void ParametersJustToResolveAmbuiguity()
		{
			engine.Add(new PatternRoute("/something/<param1>/admin/[controller]/[action]/[id]"));

			UrlInfo url = new UrlInfo("", "controller", "action", "", ".castle");

			HybridDictionary dict = new HybridDictionary(true);
			dict["controller"] = "Cart";
			dict["action"] = "new";
			dict["params"] = DictHelper.Create("param1=Homer");

			Assert.AreEqual("/something/Homer/admin/Cart/new",
							urlBuilder.BuildUrl(url, dict));
		}

		[Test]
		public void ShouldAppendQueryString()
		{
			engine.Add(new PatternRoute("/<controller>/something/<action>/[id]"));

			UrlInfo url = new UrlInfo("", "controller", "action", "", ".castle");

			HybridDictionary dict = new HybridDictionary(true);
			dict["controller"] = "cart";
			dict["action"] = "new";
			dict["querystring"] = DictHelper.Create("name=john");

			Assert.AreEqual("/cart/something/new?name=john", urlBuilder.BuildUrl(url, dict));

			dict = new HybridDictionary(true);
			dict["controller"] = "cart";
			dict["action"] = "new";
			dict["params"] = DictHelper.Create("id=10");
			dict["querystring"] = DictHelper.Create("name=john");

			Assert.AreEqual("/cart/something/new/10?name=john", urlBuilder.BuildUrl(url, dict));
		}
	}
}
