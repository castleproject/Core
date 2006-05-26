using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using NShop.Repositories;
using NShop.Services;
using NShop.Views;

namespace NShop.Controllers
{
	public class DefaultController : IController
	{
		private readonly IRepository<Customer> customerRepostiroy;
		private IDictionary<string, object> items = new Dictionary<string, object>();
	    private string viewUrl;

	    public IDictionary<string, object> Items
		{
			get { return items; }
		}

		public DefaultController(IRepository<Customer> customerRepostiroy)
		{
			this.customerRepostiroy = customerRepostiroy;
		}

		
		public void Process(HttpContext context)
		{
			Items["Customers"] = customerRepostiroy.Find();
		}

	    public string ViewUrl
	    {
	        get { return viewUrl; }
	        set { viewUrl = value; }
	    }

	    public void End()
		{
		}
	}
}
