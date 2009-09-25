namespace Castle.MonoRail.Framework.Tests.Bugs
{
	using NUnit.Framework;
	using Test;

	[TestFixture]
	public class MR_ISSUE_472
	{
		StubRequest request;
		StubResponse response;
		StubMonoRailServices services;
		StubEngineContext engineContext;

		[SetUp]
		public void Init()
		{
			request = new StubRequest();
			response = new StubResponse();
			services = new StubMonoRailServices();
			services = new StubMonoRailServices { ViewEngineManager = new StubViewEngineManager() };
			engineContext = new StubEngineContext(request, response, services, null);
		}

		[Test]
		public void Controller_Redirect_Honors_Extensionless_Urls()
		{
			services.UrlBuilder.UseExtensions = false;

			var controller = new ControllerWithRedirect();

			var context = services.ControllerContextFactory.
				Create("", "home", "Redirect", services.ControllerDescriptorProvider.BuildDescriptor(controller));

			engineContext.CurrentController = controller;
			engineContext.CurrentControllerContext = context;

			controller.Process(engineContext, context);

			Assert.IsTrue(response.WasRedirected);
			Assert.That(response.RedirectedTo, Is.EqualTo("/home/action"));
		}

		[Test]
		public void Controller_RedirectToAction_Honors_Extensionless_Urls()
		{
			services.UrlBuilder.UseExtensions = false;

			var controller = new ControllerWithRedirect();

			var context = services.ControllerContextFactory.
				Create("", "home", "RedirectToAction", services.ControllerDescriptorProvider.BuildDescriptor(controller));

			engineContext.CurrentController = controller;
			engineContext.CurrentControllerContext = context;

			controller.Process(engineContext, context);

			Assert.IsTrue(response.WasRedirected);
			Assert.That(response.RedirectedTo, Is.EqualTo("/home/action"));
		}

		class ControllerWithRedirect : Controller
		{
			public void Redirect()
			{
				Redirect("home", "action");
			}

			public void RedirectToAction()
			{
				RedirectToAction("action");
			}
		}
	}
}