namespace Castle.MonoRail.Framework.Tests.Routing
{
	using System;
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
			RouteMatch match = engine.FindMatch("/product/1");
			Assert.IsNull(match);
		}

		[Test]
		public void ShouldMatchRulesWithCorrectUrls()
		{
			engine.Add( PatternRule.Build("ProductById", "product/<id:number>", typeof(ProductController), "View") );

			RouteMatch match = engine.FindMatch("/product/1");

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

			RouteMatch match = engine.FindMatch("/product/iPod");

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
			engine.Add(PatternRule.Build("ProductByBrand", "product/<brand>", typeof(ProductController), "View"));
			engine.Add(PatternRule.Build("ProductByBrandType", "product/<brand>/<type>", typeof(ProductController), "View"));
			engine.Add(PatternRule.Build("Product", "product/<brand>/<type>/<name>", typeof(ProductController), "View"));

			RouteMatch match = engine.FindMatch("/product/apple/macbook/pro");

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

		public class ProductController : Controller
		{}

		public class LoginController : Controller
		{}
	}
}
