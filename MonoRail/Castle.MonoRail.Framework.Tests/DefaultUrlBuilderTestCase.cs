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

namespace Castle.MonoRail.Framework.Tests
{
	using System;
	using System.Collections.Specialized;
	using Castle.MonoRail.Framework.Helpers;
	using Castle.MonoRail.Framework.Routing;
	using Castle.MonoRail.Framework.Services;
	using Castle.MonoRail.Framework.Test;
	using NUnit.Framework;

	[TestFixture]
	public class DefaultUrlBuilderTestCase
	{
		private readonly UrlInfo noAreaUrl, areaUrl, withSubDomain, diffPort;
		private DefaultUrlBuilder urlBuilder;

		public DefaultUrlBuilderTestCase()
		{
			DefaultUrlTokenizer tokenizer = new DefaultUrlTokenizer();

			noAreaUrl = tokenizer.TokenizeUrl("/home/index.rails", new Uri("http://localhost/home/index.rails"), true, "/");
			areaUrl = tokenizer.TokenizeUrl("/area/home/index.rails", new Uri("http://localhost/area/home/index.rails"), true, "/");
			withSubDomain = tokenizer.TokenizeUrl("/app/home/index.rails", new Uri("http://sub.domain.com/app/home/index.rails"), false, "/app");
			diffPort = tokenizer.TokenizeUrl("/app/home/index.rails", new Uri("http://localhost:81/app/home/index.rails"), false, "/app");
		}

		[SetUp]
		public void Init()
		{
			urlBuilder = new DefaultUrlBuilder();
			urlBuilder.ServerUtil = new MockServerUtility();
			urlBuilder.RoutingEngine = new RoutingEngine();
		}

		[Test]
		public void ShouldUseRoutingEngineForNamedRoutes()
		{
			urlBuilder.RoutingEngine.Add(PatternRule.Build("link", "products", typeof(HomeController), "View"));

			HybridDictionary dict = new HybridDictionary(true);
			dict["named"] = "link";

			Assert.AreEqual("/products", urlBuilder.BuildUrl(noAreaUrl, dict));
		}

		[Test]
		public void ShouldUseTheParamsEntryForRoutesWithParams()
		{
			urlBuilder.RoutingEngine.Add(PatternRule.Build("link", "products/<id:number>", typeof(HomeController), "View"));

			HybridDictionary dict = new HybridDictionary(true);
			dict["named"] = "link";
			dict["params"] = DictHelper.Create("id=1");

			Assert.AreEqual("/products/1", urlBuilder.BuildUrl(noAreaUrl, dict));
		}

		[Test]
		public void SimpleOperations()
		{
			Assert.AreEqual("/product/list.rails", urlBuilder.BuildUrl(noAreaUrl, "product", "list"));
			Assert.AreEqual("/home/list.rails", urlBuilder.BuildUrl(noAreaUrl, DictHelper.Create("action=list")));
			Assert.AreEqual("/product/list.rails?id=1&name=hammett&", urlBuilder.BuildUrl(noAreaUrl, "product", "list", DictHelper.Create("id=1", "name=hammett")));
		}

		[Test]
		public void OperationsWithArea()
		{
			Assert.AreEqual("/product/list.rails", urlBuilder.BuildUrl(areaUrl, null, "product", "list"));
			Assert.AreEqual("/product/list.rails", urlBuilder.BuildUrl(areaUrl, "", "product", "list"));
			Assert.AreEqual("/test/product/list.rails", urlBuilder.BuildUrl(areaUrl, "test", "product", "list"));
			Assert.AreEqual("/area/product/list.rails", urlBuilder.BuildUrl(areaUrl, "product", "list"));
		}

		[Test]
		public void NoExtensions()
		{
			urlBuilder.UseExtensions = false;

			Assert.AreEqual("/product/list", urlBuilder.BuildUrl(noAreaUrl, "product", "list"));
			Assert.AreEqual("/home/list", urlBuilder.BuildUrl(noAreaUrl, DictHelper.Create("action=list")));
			Assert.AreEqual("/product/list?id=1&name=hammett&", urlBuilder.BuildUrl(noAreaUrl, "product", "list", DictHelper.Create("id=1", "name=hammett")));
			Assert.AreEqual("/area/home/list", urlBuilder.BuildUrl(areaUrl, DictHelper.Create("action=list")));
			Assert.AreEqual("/app/home/list", urlBuilder.BuildUrl(withSubDomain, DictHelper.Create("action=list")));
		}

		[Test]
		public void AbsoluteUrls()
		{
			Assert.AreEqual("http://localhost:81/app/home/list.rails", urlBuilder.BuildUrl(diffPort, DictHelper.Create("absolute=true", "action=list")));
		}

		[Test]
		public void AbsoluteUrlWithDifferentPort()
		{
			Assert.AreEqual("http://localhost:81/app/home/list.rails", urlBuilder.BuildUrl(diffPort, DictHelper.Create("absolute=true", "action=list")));
		}

		[Test]
		public void WithDomains()
		{
			Assert.AreEqual("http://sub.domain.com/app/home/list.rails", urlBuilder.BuildUrl(withSubDomain, DictHelper.Create("absolute=true", "action=list")));
		}

		[Test]
		public void SwitchingDomains()
		{
			Assert.AreEqual("http://testsub.domain.com/app/home/list.rails", urlBuilder.BuildUrl(withSubDomain, DictHelper.Create("absolute=true", "action=list", "subdomain=test")));
			Assert.AreEqual("http://something.else/app/home/list.rails", urlBuilder.BuildUrl(withSubDomain, DictHelper.Create("absolute=true", "action=list", "domain=something.else")));
		}

		[Test]
		public void UseBasePathMustDiscardTheAppVirtualDirInfo()
		{
			Assert.AreEqual("http://localhost/theArea/home/index.rails", urlBuilder.BuildUrl(areaUrl,
				DictHelper.Create("basepath=http://localhost/", "area=theArea", "controller=home", "action=index")));

			Assert.AreEqual("http://localhost/theArea/home/index.rails", urlBuilder.BuildUrl(areaUrl,
				DictHelper.Create("basepath=http://localhost", "area=theArea", "controller=home", "action=index")));
		}

		[Test]
		public void UseBasePathMustDiscardTheAreaIfTheValueIsDuplicated()
		{
			Assert.AreEqual("http://localhost/theArea/home/index.rails", urlBuilder.BuildUrl(areaUrl,
				DictHelper.Create("basepath=http://localhost/theArea", "area=theArea", "controller=home", "action=index")));

			Assert.AreEqual("http://localhost/theArea/home/index.rails", urlBuilder.BuildUrl(areaUrl,
				DictHelper.Create("basepath=http://localhost/theArea/", "area=theArea", "controller=home", "action=index")));
		}

		[Test]
		public void UseBasePathWithQuerystring()
		{
			Assert.AreEqual("http://localhost/theArea/home/index.rails?key=value", urlBuilder.BuildUrl(areaUrl,
				DictHelper.Create("basepath=http://localhost/theArea", "area=theArea", "controller=home", "action=index", "querystring=key=value")));
		}

		public class HomeController : Controller
		{
		}
	}
}
