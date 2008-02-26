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
	using Castle.MonoRail.Framework.Routing;
	using NUnit.Framework;

	[TestFixture]
	public class PatternRouteCreateUrlTestCase : BaseRuleTestFixture
	{
		[Test]
		public void ShouldNotMatchStaticRule()
		{
			PatternRoute route = new PatternRoute("/some/path");
			int points;
			route.CreateUrl("localhost", "", DictHelper.Create(""), out points);
			Assert.AreEqual(0, points);
		}

		[Test]
		public void ShouldNotMatchIfParameterIsNotPresent()
		{
			PatternRoute route = new PatternRoute("/some/<controller>");
			int points;
			Assert.IsNull(route.CreateUrl("localhost", "", DictHelper.Create(""), out points));
			Assert.AreEqual(0, points);
		}

		[Test]
		public void ShouldMatchNamedRequiredParameter()
		{
			PatternRoute route = new PatternRoute("/some/<controller>");
			int points;
			Assert.AreEqual("/some/home", route.CreateUrl("localhost", "",
				DictHelper.Create("controller=home"), out points));
			Assert.AreEqual(1, points);
		}

		[Test]
		public void ShouldMatchNamedRequiredParameters()
		{
			PatternRoute route = new PatternRoute("/some/<controller>/<action>");
			int points;
			Assert.AreEqual("/some/home/index", route.CreateUrl("localhost", "",
				DictHelper.Create("controller=home", "action=index"), out points));
			Assert.AreEqual(2, points);
		}

		[Test]
		public void ShouldMatchNamedRequiredParametersWithExtension()
		{
			PatternRoute route = new PatternRoute("/some/<controller>.castle/<action>");
			int points;
			Assert.AreEqual("/some/home.castle/index", route.CreateUrl("localhost", "",
				DictHelper.Create("controller=home", "action=index"), out points));
			Assert.AreEqual(2, points);
		}

		[Test]
		public void ShouldIgnoreOptionalParameterButMatchOthers()
		{
			PatternRoute route = new PatternRoute("/<controller>/[action]");
			int points;
			Assert.AreEqual("/home", route.CreateUrl("localhost", "",
				DictHelper.Create("controller=home"), out points));
			Assert.AreEqual(1, points);
		}

		[Test]
		public void ShouldApplyRestrictionsToParameters()
		{
			PatternRoute route = new PatternRoute("/projects/<project>/<controller>/[action]/[id]").
					DefaultFor("action").Is("index").
					Restrict("controller").AnyOf("stories", "bugs", "tasks").
					Restrict("id").ValidInteger;

			int points;
			Assert.IsNull(route.CreateUrl("localhost", "",
				DictHelper.Create("project=MonoRail", "controller=home"), out points));
			Assert.AreEqual(1, points);

			Assert.AreEqual("/projects/MonoRail/Stories", route.CreateUrl("localhost", "",
				DictHelper.Create("project=MonoRail", "controller=Stories"), out points));
			Assert.AreEqual(2, points);
			
			Assert.AreEqual("/projects/MonoRail/bugs", route.CreateUrl("localhost", "",
				DictHelper.Create("project=MonoRail", "controller=bugs", "action=index"), out points));
			Assert.AreEqual(3, points);
		}

		[Test]
		public void ShouldOmitOptionalParameterIfMatchesWithDefault()
		{
			PatternRoute route = new PatternRoute("/projects/<project>/<controller>/[action]/[id]").
					DefaultFor("action").Is("index").
					Restrict("controller").AnyOf("stories", "bugs", "tasks").
					Restrict("id").ValidInteger;

			int points;
			Assert.AreEqual("/projects/MonoRail/bugs", route.CreateUrl("localhost", "",
				DictHelper.Create("project=MonoRail", "controller=bugs", "action=index"), out points));
			Assert.AreEqual(3, points);
		}

		[Test]
		public void ShouldNotCreateRouteUrlIfDefaultsDoNotMatchAndDefaultDoesNotHaveARestriction()
		{
			PatternRoute route = new PatternRoute("/people/<id>/edit").
				DefaultForAction().Is("edit").
				DefaultForController().Is("companies").
				Restrict("id").ValidInteger;

			int points;
			Assert.IsNull(route.CreateUrl("localhost", "",
				DictHelper.Create("controller=people", "action=edit", "id=1"), out points));
			Assert.AreEqual(1, points);
		}

		[Test]
		public void ShouldCreateRouteUrlIfDefaultsDoNotMatchAndDefaultsHaveRestrictions()
		{
			PatternRoute route = new PatternRoute("/people/<id>/edit.[format]").
				DefaultForAction().Is("edit").
				DefaultForController().Is("people").
				Restrict("id").ValidInteger.
				Restrict("format").AnyOf(new string[]{"html", "json", "xml"}).
				DefaultFor("format").Is("html");

			int points;
			Assert.AreEqual("/people/1/edit.json", route.CreateUrl("localhost", "",
				DictHelper.Create("id=1", "format=json"), out points));
			Assert.AreEqual(2, points);
		}

		[Test]
		public void ShouldCreateRouteUrlIfDefaultsAreNotSupplied()
		{
			PatternRoute route = new PatternRoute("/people/<id>/edit").
				DefaultForAction().Is("edit").
				DefaultForController().Is("people").
				Restrict("id").ValidInteger;

			int points;
			Assert.AreEqual("/people/1/edit", route.CreateUrl("localhost", "",
				DictHelper.Create("id=1"), out points));
			Assert.AreEqual(1, points);
		}

		[Test]
		public void ShouldNotLeaveATrailingDot()
		{
			PatternRoute route = new PatternRoute("/people/<id>/edit.[format]").
				DefaultForAction().Is("edit").
				DefaultForController().Is("people").
				Restrict("id").ValidInteger.
				Restrict("format").AnyOf(new string[] { "html", "json", "xml" }).
				DefaultFor("format").Is("html");

			int points;
			Assert.AreEqual("/people/1/edit", route.CreateUrl("localhost", "",
				DictHelper.Create("id=1"), out points));
			Assert.AreEqual(1, points);
		}

		[Test]
		public void ShouldNotLeaveATrailingSlash()
		{
			PatternRoute route = new PatternRoute("/people/<id>/edit.[format]/").
				DefaultForAction().Is("edit").
				DefaultForController().Is("people").
				Restrict("id").ValidInteger.
				Restrict("format").AnyOf(new string[] { "html", "json", "xml" }).
				DefaultFor("format").Is("html");

			int points;
			Assert.AreEqual("/people/1/edit", route.CreateUrl("localhost", "",
				DictHelper.Create("id=1"), out points));
			Assert.AreEqual(1, points);
		}
	}
}
