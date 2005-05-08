// ${Copyrigth}

namespace Castle.MonoRail.Framework.Tests.Controllers
{
	using System;

	
	[Helper(typeof(BarHelper))]
	public class HelperController : Controller
	{
		public HelperController()
		{
		}
	}

	public class BarHelper
	{
		public BarHelper()
		{
		}

		public int Pong()
		{
			return 1;
		}
	}
}
