using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using NShop.Impl;

namespace NShop
{
	public class Global : System.Web.HttpApplication
	{
		protected void Application_EndRequest(object sender, EventArgs e)
		{
			SessionProvider.Clear();
		}
	}
}