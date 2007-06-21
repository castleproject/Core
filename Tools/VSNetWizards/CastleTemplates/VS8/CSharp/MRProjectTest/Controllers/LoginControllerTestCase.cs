namespace !NAMESPACE!.Tests
{
	using NUnit.Framework;
	using !NAMESPACE!.Controllers;
	using Castle.MonoRail.TestSupport;

	/// <summary>
	/// More about testing controllers can be found at 
	/// http://using.castleproject.org/display/MR/TDDingControllers
	/// http://using.castleproject.org/display/MR/TDDing+WizardSteps
	/// If you want to test filters, see 
	/// http://using.castleproject.org/display/MR/Unit+Testing+Filters
	/// </summary>
	[TestFixture]
	public class LoginControllerTestCase : BaseControllerTest
	{
		private LoginController controller;

		[SetUp]
		public void Init()
		{
			controller = new LoginController();
			PrepareController(controller);
		}

		/// <summary>
		/// Nothing to assert right?,
		/// Just adding this test as a placeholder
		/// </summary>
		[Test]
		public void IndexActionShouldYadaYadaYada()
		{
			controller.Index();
			Assert.AreEqual(0, controller.PropertyBag.Count);
		}

		[Test]
		public void AuthenticateShouldUseTheAuthenticationService()
		{
			// This is a great place to add an interaction place (Rhino.Mocks comes to mind)
			// as you want to test how the controller should interact with a service

			controller.Authenticate("username", "my password", false);

			Assert.AreEqual(3, controller.PropertyBag.Count);
			Assert.AreEqual("username", controller.PropertyBag["username"]);
			Assert.AreEqual("my password", controller.PropertyBag["password"]);
			Assert.AreEqual(false, controller.PropertyBag["autoLogin"]);
		}
	}
}
