using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;

namespace NShop
{
	public class Container : WindsorContainer
	{
		static Container current;

		public static Container Current
		{
			get
			{
				if(current==null)
				{
					lock(typeof(Container))
					{
						current = new Container("Windsor.Config.xml");
					}
				}
				return current;
			}
		}

		private Container(string configFile)
		:base(new XmlInterpreter(configFile))
		{
		}
	}
}
