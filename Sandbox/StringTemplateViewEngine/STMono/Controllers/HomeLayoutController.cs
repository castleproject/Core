using System;

namespace STMono.Controllers
{
	using Castle.MonoRail.Framework;

	[Layout("Default")]
	public class HomeLayoutController : Controller
	{
		public HomeLayoutController()
		{
		}

		public void Home()
		{
			PropertyBag["name"] = "<script>Sean St. Quentin</script>";
			PropertyBag["locations"] = new String[] { "Loc1", "Another Loc", "Loc3", "Nowhere!" };
		}

		public void DirectRender()
		{
			PropertyBag["name"] = "<script>Sean St. Quentin</script>";
			this.DirectRender("User: $name$");
		}
	}
}
