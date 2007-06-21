namespace !NAMESPACE!.Tests
{
	using System;
	using !NAMESPACE!.Controllers;

	using Castle.MonoRail.TestSupport;
	
	using NUnit.Framework;

	[TestFixture]
	public class HomeControllerTestCase : AbstractMRTestCase
	{
		[Test]
		public void IndexAction()
		{
			HomeController controller = new HomeController();
			controller.Index();

			Assert.IsNotNull(controller.PropertyBag["accessDate"]);
		}

		[Test]
		[ExpectedException(typeof(Exception), "Exception thrown from a MonoRail action")]
		public void BlowItAwayAction()
		{
			HomeController controller = new HomeController();
			controller.BlowItAway();
		}
	}
}
