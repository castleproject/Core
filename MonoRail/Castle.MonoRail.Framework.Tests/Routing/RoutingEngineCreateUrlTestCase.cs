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

namespace Castle.MonoRail.Framework.Tests.Routing
{
	using Castle.MonoRail.Framework.Helpers;
	using Framework.Routing;
	using NUnit.Framework;

	[TestFixture]
	public class RoutingEngineCreateUrlTestCase : BaseRuleTestFixture
	{
		private RoutingEngine engine;

		[SetUp]
		public void Init()
		{
			engine = new RoutingEngine();

			engine.Add(new PatternRoute("<area>/<controller>/[action]/[key]").
				DefaultForAction().Is("index"));
			engine.Add(new PatternRoute("<controller>/[action]").
				DefaultForAction().Is("index"));
			engine.Add(new PatternRoute("admin/<customer>/<controller>/[action]").
				DefaultForAction().Is("index"));
			engine.Add(
				new PatternRoute("/projects/<project>/<controller>/<key>").
					DefaultFor("action").Is("view").
					DefaultForArea().Is("projects"));
			engine.Add(
				new PatternRoute("/projects/<project>/<controller>/[action]/[key]").
					DefaultForArea().Is("projects").
					DefaultForAction().Is("list"));
		}

		[Test]
		public void CreateUrl_ForProject_WithoutActionButWithKey()
		{
			string url = engine.CreateUrl("host", "vpath", DictHelper.Create("project=some", "controller=products", "key=123"));
			Assert.IsNotNull(url);
			Assert.AreEqual("/projects/some/products/123", url);
		}

		[Test]
		public void CreateUrl_ForProject_WithActionAndKey()
		{
			string url = engine.CreateUrl("host", "vpath", DictHelper.Create("project=some", "controller=products", "action=show", "key=123"));
			Assert.IsNotNull(url);
			Assert.AreEqual("/projects/some/products/show/123", url);
		}

		[Test]
		public void CreateUrl_ShouldWorkForAreaControllerAndAction()
		{
			string url = engine.CreateUrl("host", "vpath", DictHelper.Create("area=shopping", "controller=products", "action=list"));
			Assert.IsNotNull(url);
			Assert.AreEqual("/shopping/products/list", url);
		}

		[Test]
		public void CreateUrl_ShouldWorkForAreaControllerAndAction_WithKey()
		{
			string url = engine.CreateUrl("host", "vpath", DictHelper.Create("area=shopping", "controller=products", "action=show", "key=1"));
			Assert.IsNotNull(url);
			Assert.AreEqual("/shopping/products/show/1", url);
		}

		[Test]
		public void CreateUrl_ShouldWorkForAreaControllerAndNoAction()
		{
			string url = engine.CreateUrl("host", "vpath", DictHelper.Create("area=shopping", "controller=products"));
			Assert.IsNotNull(url);
			Assert.AreEqual("/shopping/products", url);
		}

		[Test]
		public void CreateUrl_ShouldWorkForControllerAndAction()
		{
			string url = engine.CreateUrl("host", "vpath", DictHelper.Create("controller=products", "action=list"));
			Assert.IsNotNull(url);
			Assert.AreEqual("/products/list", url);
		}

		[Test]
		public void CreateUrl_EmptyAreaShouldBeIgnored()
		{
			string url = engine.CreateUrl("host", "vpath", DictHelper.Create("area=", "controller=products", "action=list"));
			Assert.IsNotNull(url);
			Assert.AreEqual("/products/list", url);
		}

		[Test]
		public void CreateUrl_CustomerParameterShouldRestrictSetOfRoutesThatCanBeUsedToBuildURL()
		{
			string url = engine.CreateUrl("host", "vpath", DictHelper.Create("customer=apple", "controller=workflow", "action=list"));
			Assert.IsNotNull(url);
			Assert.AreEqual("/admin/apple/workflow/list", url);
		}
	}
}
