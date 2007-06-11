using System;
using System.ServiceModel;
using System.Web.UI;
using Castle.Facilities.WcfIntegration.Demo.UsingWindsorSvc;

namespace Castle.Facilities.WcfIntegration.Demo
{
	public partial class _Default : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			AmUsingWindsorClient usingWindsorClient = new UsingWindsorSvc.AmUsingWindsorClient();
			ValueFromService.Text = usingWindsorClient.GetValueFromWindsorConfig().ToString();
		}
	}
}
