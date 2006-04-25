using System;

namespace STMono.Controllers
{
	using Castle.MonoRail.Framework;
	using log4net;

	public class HomeController : Controller
	{
		private static readonly ILog log = LogManager.GetLogger(typeof (HomeController));

		public HomeController()
		{
		}

		public void Home()
		{
			log.DebugFormat("Home!");
			PropertyBag["name"] = "Sean St. Quentin";
			PropertyBag["locations"] = new String[] { "Loc1", "Another Loc", "Loc3", "Nowhere!" };
		}
	}
}
