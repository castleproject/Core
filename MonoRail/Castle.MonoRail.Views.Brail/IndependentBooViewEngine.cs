namespace Castle.MonoRail.Views.Brail
{
	using System.IO;
	using Framework;
	using System.Collections;

	public class IndependentBooViewEngine
	{
		readonly BooViewEngine bve = new BooViewEngine();

		public BooViewEngineOptions Options
		{
			get { return bve.Options; }
		}

		public IndependentBooViewEngine(IViewSourceLoader viewSourceLoader, BooViewEngineOptions options)
		{
			bve.Options = options;
			bve.SetViewSourceLoader(viewSourceLoader);
			bve.Initialize();
		}

		public void Process(string templateName, TextWriter output, IDictionary parameters)
		{
			DummyController controller = new DummyController();
			controller.PropertyBag = parameters;
			bve.Process(output, null, controller, templateName);
		}

		private class DummyController : Controller
		{

		}
	}
}