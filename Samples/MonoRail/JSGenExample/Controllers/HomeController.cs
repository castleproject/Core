namespace JSGenExample.Controllers
{
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Helpers;
	using JSGenExample.Models;

	[Layout("default"), Rescue("generalerror")]
	public class HomeController : SmartDispatcherController
	{
		private const int PageSize = 5;

		public void Index()
		{
			PropertyBag["customers"] = PaginationHelper.CreatePagination<Customer>(Context, Customers, PageSize); 
		}

		/// <summary>
		/// Invoked by Ajax
		/// </summary>
		public void NewCustomer()
		{
			// Here, the NewCustomer.njs will be "rendered"
		}

		/// <summary>
		/// Invoked by Ajax
		/// </summary>
		public void AddCustomer([DataBind("customer")] Customer customer)
		{
			// Pretend to save it
			// Here, the AddCustomer.njs will be "rendered"

			PropertyBag["customer"] = customer;
		}

		/// <summary>
		/// Just to mimic a persistent storage
		/// </summary>
		protected Customer[] Customers
		{
			get
			{
				if (!Session.Contains("items"))
				{
					Session["items"] = new Customer[]
					{
						new Customer(1, "Apple", "contact@apple.com"),
						new Customer(2, "Google", "contact@google.com"),
						new Customer(3, "Microsoft", "contact@ms.com"),
						new Customer(4, "ChangeCorporation", "contact@changecorporation.com.au"),
						new Customer(5, "Castle Stronghold", "contact@castlestronghold.com"),
						new Customer(6, "Fox Networks", "contact@fox-intl.com"),
						new Customer(7, "Cisco", "contact@cisco.com"),
						new Customer(8, "Linksys", "contact@linksys.com"),
						new Customer(9, "Siemens", "contact@siemens.com"),
						new Customer(10, "Toshiba", "contact@toshiba.com"),
						new Customer(11, "Dell", "contact@dell.com"),
					};
				}

				return (Customer[]) Session["items"];
			}
		}
	}
}
