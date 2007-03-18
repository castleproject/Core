namespace Castle.MonoRail.Tests
{
	using Castle.MonoRail.Test;
	using NUnit.Framework;

	[TestFixture]
	public class ControllerExecutorTestCase
	{
		[Test]
		public void ControllerStateIsProperlyInitialized()
		{
			SimpleController controller = new SimpleController();

			IExecutionContext testContext = new TestContext(new UrlInfo("area", "controller", "action"));

			Assert.IsNull(controller.AreaName);
			Assert.IsNull(controller.Name);
			Assert.IsNull(controller.ActionName);

			new ControllerExecutor(controller, testContext);

			Assert.AreEqual("area", controller.AreaName);
			Assert.AreEqual("controller", controller.Name);
			Assert.AreEqual("action", controller.ActionName);
		}

		public class SimpleController : Controller
		{
			
		}
	}
}
