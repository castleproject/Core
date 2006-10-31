namespace GettingStartedSample.Controllers
{
	using System;

	using Castle.MonoRail.Framework;

	[Layout("default"), Rescue("generalerror")]
	public class HomeController : SmartDispatcherController
	{
		public void Index()
		{
		}
		
		public void ErroneousAction()
		{
			throw new Exception("Forced exception to test Rescue");
		}
		
		public void DataToTheView()
		{
			PropertyBag["name"] = "John Doe";
			PropertyBag["today"] = DateTime.Now;
			
			RenderView("data");
		}
	}
}
