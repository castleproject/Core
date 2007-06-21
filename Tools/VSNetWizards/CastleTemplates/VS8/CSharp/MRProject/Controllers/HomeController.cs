namespace !NAMESPACE!.Controllers
{
	using System;

	using Castle.MonoRail.Framework;

	[Layout("default"), Rescue("generalerror")]
	public class HomeController : SmartDispatcherController
	{
		public void Index()
		{
			PropertyBag["accessDate"] = DateTime.Now;
		}

		public void BlowItAway()
		{
			throw new Exception("Exception thrown from a MonoRail action");
		}
	}
}
