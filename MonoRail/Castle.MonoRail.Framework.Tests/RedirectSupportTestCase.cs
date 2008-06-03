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

namespace Castle.MonoRail.Framework.Tests
{
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Routing;
	using Castle.MonoRail.Framework.Services;
	using NUnit.Framework;
	using Test;

	[TestFixture]
	public class RedirectSupportTestCase
	{
		private DefaultUrlBuilder urlBuilder;
		private RoutingEngine engine;

		[SetUp]
		public void Init()
		{
			engine = new RoutingEngine();
			urlBuilder = new DefaultUrlBuilder();
			urlBuilder.ServerUtil = new StubServerUtility();
			urlBuilder.RoutingEngine = engine;
		}

		[Test]
		public void RedirectToSiteRootUsesAppVirtualDir()
		{
			UrlInfo url = new UrlInfo("area", "home", "index", "", ".castle");
			StubResponse response = new StubResponse(url, urlBuilder, urlBuilder.ServerUtil, new RouteMatch());
			response.RedirectToSiteRoot();
			Assert.AreEqual("/", response.RedirectedTo);

			url = new UrlInfo("area", "home", "index", "/app", ".castle");
			response = new StubResponse(url, urlBuilder, urlBuilder.ServerUtil, new RouteMatch());
			response.RedirectToSiteRoot();
			Assert.AreEqual("/app/", response.RedirectedTo);
		}

		[Test]
		public void RedirectToUrlDoesNotTouchUrl()
		{
			UrlInfo url = new UrlInfo("area", "home", "index", "", ".castle");
			StubResponse response = new StubResponse(url, urlBuilder, urlBuilder.ServerUtil, new RouteMatch());
			response.RedirectToUrl("/uol/com/folha");
			Assert.AreEqual("/uol/com/folha", response.RedirectedTo);
		}

		[Test]
		public void RedirectToUrlWithQueryStringAsDict()
		{
			UrlInfo url = new UrlInfo("area", "home", "index", "", ".castle");
			StubResponse response = new StubResponse(url, urlBuilder, urlBuilder.ServerUtil, new RouteMatch());
			response.RedirectToUrl("/uol/com/folha", DictHelper.Create("id=1", "name=john doe"));
			Assert.AreEqual("/uol/com/folha?id=1&name=john+doe", response.RedirectedTo);

			response.RedirectToUrl("/uol/com/folha?something=1", DictHelper.Create("id=1", "name=john doe"));
			Assert.AreEqual("/uol/com/folha?something=1&id=1&name=john+doe", response.RedirectedTo);
		}

		[Test]
		public void Redirect_ToControllerAction()
		{
			UrlInfo url = new UrlInfo("area", "home", "index", "", ".castle");
			StubResponse response = new StubResponse(url, urlBuilder, urlBuilder.ServerUtil, new RouteMatch());
			response.Redirect("cart", "view");
			Assert.AreEqual("/area/cart/view.castle", response.RedirectedTo);

			url = new UrlInfo("", "home", "index", "", ".castle");
			response = new StubResponse(url, urlBuilder, urlBuilder.ServerUtil, new RouteMatch());
			response.Redirect("cart", "view");
			Assert.AreEqual("/cart/view.castle", response.RedirectedTo);
		}

		[Test]
		public void Redirect_ToAreaControllerAction()
		{
			UrlInfo url = new UrlInfo("area", "home", "index", "", ".castle");
			StubResponse response = new StubResponse(url, urlBuilder, urlBuilder.ServerUtil, new RouteMatch());
			response.Redirect("admin", "cart", "view");
			Assert.AreEqual("/admin/cart/view.castle", response.RedirectedTo);

			url = new UrlInfo("", "home", "index", "", ".castle");
			response = new StubResponse(url, urlBuilder, urlBuilder.ServerUtil, new RouteMatch());
			response.Redirect("admin", "cart", "view");
			Assert.AreEqual("/admin/cart/view.castle", response.RedirectedTo);
		}

		[Test]
		public void RedirectUsingRoute_InheritingParameters()
		{
			engine.Add(new PatternRoute("/something/<param1>/admin/[controller]/[action]/[id]"));

			RouteMatch match = new RouteMatch();
			match.AddNamed("param1", "Homer");

			UrlInfo url = new UrlInfo("area", "home", "index", "", ".castle");
			StubResponse response = new StubResponse(url, urlBuilder, urlBuilder.ServerUtil, match);
			response.RedirectUsingRoute("cart", "checkout", true);
			Assert.AreEqual("/something/Homer/admin/cart/checkout", response.RedirectedTo);
		}

		[Test]
		public void RedirectUsingRoute_SpecifyingParameters()
		{
			engine.Add(new PatternRoute("/something/<param1>/admin/[controller]/[action]/[id]"));

			RouteMatch match = new RouteMatch();

			UrlInfo url = new UrlInfo("area", "home", "index", "", ".castle");
			StubResponse response = new StubResponse(url, urlBuilder, urlBuilder.ServerUtil, match);
			response.RedirectUsingRoute("cart", "checkout", DictHelper.Create("param1=Marge"));
			Assert.AreEqual("/something/Marge/admin/cart/checkout", response.RedirectedTo);
		}
	}
}
