namespace AuthenticationUsingForms.Controllers
{
	using System;
	using AuthenticationUsingFilters.Filters;
	using Castle.MonoRail.Framework;

	[Layout("default")]
	[Filter(ExecuteEnum.BeforeAction, typeof(AuthenticationFilter))]
	public class HomeController : SmartDispatcherController
	{
		public void Index()
		{
		}
	}
}
