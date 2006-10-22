namespace !NAMESPACE!.Controllers
{
	using System;

	using Castle.MonoRail.Framework;

	[Layout("default"), Rescue("generalerror")]
	public class HomeController : SmartDispatcherController
	{
		public void Index()
		{
			
		}
	}
}
