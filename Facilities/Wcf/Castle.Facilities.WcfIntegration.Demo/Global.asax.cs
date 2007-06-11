using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Castle.Windsor;

namespace Castle.Facilities.WcfIntegration.Demo
{
	public class Global : System.Web.HttpApplication
	{
		protected void Application_Start(object sender, EventArgs e)
		{
			WindsorContainer container = new WindsorContainer("windsor.xml");
			WindsorServiceHostFactory.RegisterContainer(container);

		}

		protected void Application_End(object sender, EventArgs e)
		{

		}
	}
}