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
		[Test, Ignore("Need to re-think this one")]
		public void ShouldNotMatchStaticRule()
		{
			PatternRoute route = new PatternRoute("/some/path");
			Assert.IsNull(route.CreateUrl("localhost", "", DictHelper.Create("")));
		}

		[Test]
		public void ShouldNotMatchIfParameterIsNotPresent()
		{
			PatternRoute route = new PatternRoute("/some/<controller>");
			Assert.IsNull(route.CreateUrl("localhost", "", DictHelper.Create("")));
		}

		[Test]
		public void ShouldMatchNamedRequiredParameter()
		{
			PatternRoute route = new PatternRoute("/some/<controller>");
			Assert.AreEqual("/some/home", route.CreateUrl("localhost", "",
				DictHelper.Create("controller=home")));
		}

		[Test]
		public void ShouldMatchNamedRequiredParameters()
		{
			PatternRoute route = new PatternRoute("/some/<controller>/<action>");
			Assert.AreEqual("/some/home/index", route.CreateUrl("localhost", "",
				DictHelper.Create("controller=home", "action=index")));
		}

		[Test]
		public void ShouldMatchNamedRequiredParametersWithExtension()
		{
			PatternRoute route = new PatternRoute("/some/<controller>.castle/<action>");

			Assert.AreEqual("/some/home.castle/index", route.CreateUrl("localhost", "",
				DictHelper.Create("controller=home", "action=index")));
		}

		[Test]
		public void ShouldIgnoreOptionalParameterButMatchOthers()
		{
			PatternRoute route = new PatternRoute("/<controller>/[action]");

			Assert.AreEqual("/home", route.CreateUrl("localhost", "",
				DictHelper.Create("controller=home")));
		}

		[Test]
		public void ShouldApplyRestrictionsToParameters()
		{
			PatternRoute route = new PatternRoute("/projects/<project>/<controller>/[action]/[id]").
					DefaultFor("action").Is("index").
					Restrict("controller").AnyOf("stories", "bugs", "tasks").
					Restrict("id").ValidInteger;

			Assert.IsNull(route.CreateUrl("localhost", "",
				DictHelper.Create("project=MonoRail", "controller=home")));

			Assert.AreEqual("/projects/MonoRail/Stories", route.CreateUrl("localhost", "",
				DictHelper.Create("project=MonoRail", "controller=Stories")));
			
			Assert.AreEqual("/projects/MonoRail/bugs", route.CreateUrl("localhost", "",
				DictHelper.Create("project=MonoRail", "controller=bugs", "action=index")));
		}

		[Test]
		public void ShouldOmitOptionalParameterIfMatchesWithDefault()
		{
			PatternRoute route = new PatternRoute("/projects/<project>/<controller>/[action]/[id]").
					DefaultFor("action").Is("index").
					Restrict("controller").AnyOf("stories", "bugs", "tasks").
					Restrict("id").ValidInteger;

			Assert.AreEqual("/projects/MonoRail/bugs", route.CreateUrl("localhost", "",
				DictHelper.Create("project=MonoRail", "controller=bugs", "action=index")));
		}

		[Test]
		public void ShouldNotCreateRouteUrlIfDefaultsDoNotMatchAndDefaultDoesNotHaveARestriction()
		{
			PatternRoute route = new PatternRoute("/people/<id>/edit").
				DefaultForAction().Is("edit").
				DefaultForController().Is("companies").
				Restrict("id").ValidInteger;

			Assert.IsNull(route.CreateUrl("localhost", "",
				DictHelper.Create("controller=people", "action=edit", "id=1")));
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

			Assert.AreEqual("/people/1/edit.json", route.CreateUrl("localhost", "",
				DictHelper.Create("id=1", "format=json")));
		}

		[Test]
		public void ShouldCreateRouteUrlIfDefaultsAreNotSupplied()
		{
			PatternRoute route = new PatternRoute("/people/<id>/edit").
				DefaultForAction().Is("edit").
				DefaultForController().Is("people").
				Restrict("id").ValidInteger;

			Assert.AreEqual("/people/1/edit", route.CreateUrl("localhost", "",
				DictHelper.Create("id=1")));
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

			Assert.AreEqual("/people/1/edit", route.CreateUrl("localhost", "",
				DictHelper.Create("id=1")));
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

			Assert.AreEqual("/people/1/edit", route.CreateUrl("localhost", "",
				DictHelper.Create("id=1")));
		}
	}
}
