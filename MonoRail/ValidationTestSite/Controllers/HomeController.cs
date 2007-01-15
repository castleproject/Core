namespace ValidationTestSite.Controllers
{
	using Castle.MonoRail.Framework;
	using ValidationTestSite.Models;

	[Layout("default"), Rescue("generalerror")]
	public class HomeController : SmartDispatcherController
	{
		public void Index()
		{
			// PropertyBag["client"] = new Client();
			PropertyBag["clienttype"] = typeof(Client);
		}

		public void Index2()
		{
			PropertyBag["clienttype"] = typeof(Client);
		}

		public void Save([DataBind("client", Validate=true)] Client client)
		{
			if (HasValidationError(client))
			{
				Flash["errors"] = GetErrorSummary(client);
				Flash["client"] = client;
				RedirectToReferer();
			}
		}

		public void Supplier()
		{
			PropertyBag["supplier"] = new Supplier();
		}
	}
}
