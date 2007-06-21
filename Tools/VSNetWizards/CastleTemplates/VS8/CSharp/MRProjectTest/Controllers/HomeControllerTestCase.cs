namespace !NAMESPACE!.Tests
{
	using System;
	using !NAMESPACE!.Controllers;
	using Castle.MonoRail.TestSupport;
	using NUnit.Framework;

	/// <summary>
	/// More about testing controllers can be found at 
	/// http://using.castleproject.org/display/MR/TDDingControllers
	/// http://using.castleproject.org/display/MR/TDDing+WizardSteps
	/// If you want to test filters, see 
	/// http://using.castleproject.org/display/MR/Unit+Testing+Filters
	/// </summary>
	[TestFixture]
	public class HomeControllerTestCase : BaseControllerTest
	{
		private HomeController controller;

		[SetUp]
		public void Init()
		{
			controller = new HomeController();
			PrepareController(controller);
		}

		[Test]
		public void IndexActionShouldAddAccessDateToPropertyBag()
		{
			controller.Index();

			Assert.IsNotNull(controller.PropertyBag["AccessDate"]);
		}

		[Test]
		[ExpectedException(typeof(Exception), "Exception thrown from a MonoRail action")]
		public void BlowItAwayShouldThrowExceptionAsItsHardCodedToDoThat()
		{
			controller.BlowItAway();
		}
	}
}
