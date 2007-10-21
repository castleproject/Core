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
	using System;
	using Castle.MonoRail.Framework.Routing;
	using NUnit.Framework;

	[TestFixture]
	public class PatternRuleTestCase
	{
		[Test]
		public void ShouldMatchAsPath()
		{
			PatternRule rule = PatternRule.Build("ProductById", "product", typeof(ProductController), "View");

			RouteMatch match = new RouteMatch(typeof(ProductController), "foo", "bar");

			Assert.IsTrue(rule.Matches("localhost", "", "product", match));
		}

		[Test]
		public void StartShouldAllowAMatchAll()
		{
			PatternRule rule = PatternRule.Build("Product", "product/*", typeof(ProductController), "View");

			RouteMatch match = new RouteMatch(typeof(ProductController), "foo", "bar");

			Assert.IsTrue(rule.Matches("localhost", "", "product/", match));
			Assert.IsTrue(rule.Matches("localhost", "", "product", match));
			Assert.IsTrue(rule.Matches("localhost", "", "product/iPod", match));
			Assert.IsTrue(rule.Matches("localhost", "", "product/iPod/Nano", match));
		}

		[Test]
		public void ShouldMatchRulesWithCorrectUrls()
		{
			PatternRule rule = PatternRule.Build("ProductById", "product/<id:number>", typeof(ProductController), "View");

			RouteMatch match = new RouteMatch(typeof(ProductController), "foo", "bar");

			Assert.IsTrue(rule.Matches("localhost", "", "product/1", match));

			Assert.IsNotNull(match);
			Assert.AreEqual(1, match.Literals.Count);
			Assert.AreEqual(1, match.Parameters.Count);
			Assert.AreEqual("product", match.Literals[0]);
			Assert.AreEqual("1", match.Parameters["id"]);
		}

		[Test]
		public void ShouldMatchRulesWithCorrectUrls2()
		{
			PatternRule rule = PatternRule.Build("ProductById", "product/<name>", typeof(ProductController), "View");

			RouteMatch match = new RouteMatch(typeof(ProductController), "foo", "bar");

			Assert.IsTrue(rule.Matches("localhost", "", "product/iPod", match));

			Assert.IsNotNull(match);
			Assert.AreEqual(1, match.Literals.Count);
			Assert.AreEqual(1, match.Parameters.Count);
			Assert.AreEqual("product", match.Literals[0]);
			Assert.AreEqual("iPod", match.Parameters["name"]);
		}

		[Test]
		public void ShouldMatchChoice()
		{
			PatternRule rule = PatternRule.Build("ProductById", "product/<brand:apple|ms|cisco>", typeof(ProductController), "View");

			RouteMatch match = new RouteMatch(typeof(ProductController), "foo", "bar");

			Assert.IsTrue(rule.Matches("localhost", "", "product/apple", match));
			Assert.IsTrue(rule.Matches("localhost", "", "product/ms", match));
			Assert.IsTrue(rule.Matches("localhost", "", "product/cisco", match));
			
			Assert.IsFalse(rule.Matches("localhost", "", "product/novell", match));
			Assert.IsFalse(rule.Matches("localhost", "", "product/appletalk", match));
			Assert.IsFalse(rule.Matches("localhost", "", "product/mapple", match));
			Assert.IsFalse(rule.Matches("localhost", "", "product/mcs", match));
			Assert.IsFalse(rule.Matches("localhost", "", "product/mss", match));
			Assert.IsFalse(rule.Matches("localhost", "", "product/sms", match));
		}

		[Test]
		public void ShouldNotMatchGreedly()
		{
			PatternRule rule = PatternRule.Build("ProductById", "product/<brand:apple|ms|cisco>", typeof(ProductController), "View");

			RouteMatch match = new RouteMatch(typeof(ProductController), "foo", "bar");

			Assert.IsFalse(rule.Matches("localhost", "", "product/apple/ipod", match));
			Assert.IsFalse(rule.Matches("localhost", "", "product/ms/zune", match));
			Assert.IsFalse(rule.Matches("localhost", "", "product/cisco/some", match));
		}

		[Test]
		public void ChoiceThatMatchedShouldBeAvailableOnRouteMatch()
		{
			PatternRule rule = PatternRule.Build("ProductById", "product/<brand:apple|ms|cisco>", typeof(ProductController), "View");

			RouteMatch match = new RouteMatch(typeof(ProductController), "foo", "bar");
			Assert.IsTrue(rule.Matches("localhost", "", "product/apple", match));

			Assert.IsTrue(match.Parameters.ContainsKey("brand"));
			Assert.AreEqual("apple", match.Parameters["brand"]);

			match = new RouteMatch(typeof(ProductController), "foo", "bar");
			Assert.IsTrue(rule.Matches("localhost", "", "product/ms", match));

			Assert.IsTrue(match.Parameters.ContainsKey("brand"));
			Assert.AreEqual("ms", match.Parameters["brand"]);
		}

		[Test]
		public void NumberPatternShouldMatchOnlyNumbers()
		{
			PatternRule rule = PatternRule.Build("ProductById", "product/<id:number>", typeof(ProductController), "View");

			RouteMatch match = new RouteMatch(typeof(ProductController), "foo", "bar");

			Assert.IsFalse(rule.Matches("localhost", "", "product/iPod", match));
		}

		[Test, ExpectedException(typeof(ArgumentException), "token has invalid value 'int'. Expected 'int' or 'string'")]
		public void InvalidPatternType()
		{
			PatternRule.Build("ProductById", "product/<id:int>", typeof(ProductController), "View");
		}

		[Test, ExpectedException(typeof(ArgumentException), "token has invalid value 'num'. Expected 'int' or 'string'")]
		public void InvalidPatternType2()
		{
			PatternRule.Build("ProductById", "product/<id:num>", typeof(ProductController), "View");
		}

		[Test, ExpectedException(typeof(ArgumentException), "Spaces are not allowed on a pattern token. Please check the pattern '<id : number>'")]
		public void SpacesAreNotAllowedOnPattern()
		{
			PatternRule.Build("ProductById", "product/<id : number>", typeof(ProductController), "View");
		}

		[Test, ExpectedException(typeof(ArgumentException), "Token is not wellformed. It should end with '>' or ']'")]
		public void PatternMustBeWellFormed()
		{
			PatternRule.Build("ProductById", "product/<id:number", typeof(ProductController), "View");
		}

		[Test, ExpectedException(typeof(ArgumentException), "Token is not wellformed. It should end with '>' or ']'")]
		public void PatternMustBeWellFormed2()
		{
			PatternRule.Build("ProductById", "product/[id:number", typeof(ProductController), "View");
		}

		public class ProductController : Controller { }
	}
}
