namespace !NAMESPACE!.Controllers
{
	using System;

	public class HomeController : BaseController
	{
		public void Index()
		{
			PropertyBag["AccessDate"] = DateTime.Now;
		}

		public void BlowItAway()
		{
			throw new Exception("Exception thrown from a MonoRail action");
		}
	}
}
