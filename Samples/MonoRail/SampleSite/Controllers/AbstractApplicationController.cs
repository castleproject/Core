namespace SampleSite.Controllers
{
	using System;

	using Castle.MonoRail.Framework;

	[Filter(ExecuteEnum.AfterAction, typeof(FooterLinkFilter))]
	[Layout("default")]
	public class AbstractApplicationController : Controller
	{
	}

	[Filter(ExecuteEnum.AfterAction, typeof(FooterLinkFilter))]
	[Layout("defaultajax")]
	public class AbstractAjaxApplicationController : SmartDispatcherController
	{
	}
}
