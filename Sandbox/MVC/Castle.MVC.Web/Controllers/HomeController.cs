using System;
using Castle.MVC.Controllers;

using Castle.MVC.Web.Views;
using Castle.MVC.Web;

namespace Castle.MVC.Web.Controllers
{
	public class HomeController : Controller
	{

		public void Login(string email, string passwd)
		{
			//this.NextView = WebViews.PAGE2;
			this.Navigate();
		}

		public void Login2(string email, string passwd)
		{
			//this.NextView = WebViews.PAGE2;
			this.Navigate();
		}

		public MyApplicationState MyState
		{
			get
			{
				return this.State as MyApplicationState;
			}
		}
	}
}
