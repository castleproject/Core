namespace ActiveRecordIntegrationSample.Controllers
{
	using System;

	using Castle.MonoRail.Framework;
	using Common.Models;

	[Layout("default"), Rescue("generalerror")]
	public class HomeController : SmartDispatcherController
	{
		public void Index()
		{
			PropertyBag["categories"] = Category.FindAll();
			PropertyBag["prodlicenses"] = ProductLicense.FindAll();
			PropertyBag["accounts"] = Account.FindAll();
			PropertyBag["accountpermissions"] = AccountPermission.FindAll();
		}
	}
}
