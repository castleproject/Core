namespace !NAMESPACE!.Tests
{
	using NUnit.Framework;
	using !NAMESPACE!.Controllers;
	using Castle.MonoRail.TestSupport;

	[TestFixture]
	public class LoginControllerTestCase : AbstractMRTestCase
	{
		[Test]
		public void IndexAction()
		{
			LoginController controller = new LoginController();
			controller.Index();

			Assert.AreEqual(0, controller.PropertyBag.Count);
		}

		[Test]
		public void ValidateLoginAction()
		{
			LoginController controller = new LoginController();
			controller.ValidateLogin("a username", "my password", false);

			Assert.AreEqual(3, controller.PropertyBag.Count);
			Assert.AreEqual("a username", controller.PropertyBag["username"]);
			Assert.AreEqual("my password", controller.PropertyBag["password"]);
			Assert.AreEqual(false, controller.PropertyBag["autoLogin"]);
		}
	}
}
