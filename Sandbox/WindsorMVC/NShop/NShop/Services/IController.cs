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

namespace NShop.Services
{
	public interface IController
	{
		void Process(HttpContext context);
		string View { get; }
		void End();

		IDictionary<string, object> Items { get; }
	}
}
