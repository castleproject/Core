namespace LocalizationSample.Controllers
{
	using System;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Filters;

	[Layout("default")]
	[Resource("text", "LocalizationSample.Resources.Home")]
	[LocalizationFilter(RequestStore.Cookie, "locale")]
	public class HomeController : SmartDispatcherController
	{
		public void Index()
		{
		}
		
		public void SetLanguage(String langCode)
		{
			Response.CreateCookie("locale", langCode);
			
			RedirectToAction("index");
		}
	}
}
