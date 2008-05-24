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
	using Framework.Routing;
	using NUnit.Framework;

	[TestFixture]
	public class RoutingEngineTestCase : BaseRuleTestFixture
	{
		private RoutingEngine engine;

		[SetUp]
		public void Init()
		{
			engine = new RoutingEngine();
		}

		[Test]
		public void FindMatch_SelectsTheMatchWithMostPoints()
		{
			engine.Add(new PatternRoute("/<controller>/<action>/[id]").
				Restrict("id").ValidInteger);
			engine.Add(new PatternRoute("/<controller>/shop/<category>"));

			RouteMatch match = engine.FindMatch("/home/shop/movies", CreateGetContext());
			Assert.IsNotNull(match);
			Assert.AreEqual("home", match.Parameters["controller"]);
			Assert.AreEqual("movies", match.Parameters["category"]);
		}

		[Test]
		public void FindMatch_IgnoresMonoRailResourceUrls()
		{
			engine.Add(new PatternRoute("/<area>/<controller>/<action>"));

			RouteMatch match = engine.FindMatch("/admin/users/edit", CreateGetContext());
			Assert.IsNotNull(match);
			Assert.AreEqual("admin", match.Parameters["area"]);
			Assert.AreEqual("users", match.Parameters["controller"]);
			Assert.AreEqual("edit", match.Parameters["action"]);

			Assert.IsNull(engine.FindMatch("/MonoRail/Files/BehaviourScripts", CreateGetContext()));
			Assert.IsNull(engine.FindMatch("/MonoRail/Files/AjaxScripts", CreateGetContext()));
			Assert.IsNull(engine.FindMatch("/MonoRail/Files/FormHelperScript", CreateGetContext()));
			Assert.IsNull(engine.FindMatch("/MonoRail/Files/ZebdaScripts", CreateGetContext()));
			Assert.IsNull(engine.FindMatch("/MonoRail/Files/NonExistent", CreateGetContext()));
		}
	}
}
