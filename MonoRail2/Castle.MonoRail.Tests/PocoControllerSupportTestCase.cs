namespace Castle.MonoRail.Tests
{
	using Castle.MonoRail.Test;
	using NUnit.Framework;

	/// <summary>
	/// This fixture asserts that Poco are accept as controllers, although
	/// this use presents some design differences
	/// </summary>
	[TestFixture, Category(TestCategories.Core)]
	public class PocoControllerSupportTestCase
	{
		private PocoController controller;
		private IExecutionContext testContext;

		[SetUp]
		public void CreateObjects()
		{
			controller = new PocoController();
			testContext = new TestContext(new UrlInfo("area", "controller", "index"));
		}

		[Test]
		public void ExecutorDoesNotFailToAcceptPoco()
		{
			new ControllerExecutor(controller, testContext);

			// How Can I write an assert for that?
		}

		[Test]
		public void ActionIsSelectedAndRun()
		{
			ControllerExecutor executor = new ControllerExecutor(controller, testContext);

			ActionExecutor actionExec = executor.SelectAction();

			Assert.IsNotNull(actionExec);
			Assert.AreEqual(ActionType.Method, actionExec.ActionType);
			Assert.AreEqual("Index", actionExec.Name);

			executor.Execute(actionExec);

			Assert.IsTrue(controller.wasRun);
		}

		public class PocoController
		{
			public bool wasRun;

			public void Index()
			{
				wasRun = true;
			}
		}
	}
}
