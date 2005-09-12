using System;
using System.Reflection;
using Castle.MVC.Navigation;
using Castle.MVC.StatePersister;
using Castle.MVC.States;
using Castle.MVC.Views;
using Castle.Windsor;

namespace Castle.MVC.Test.Presentation
{
	/// <summary>
	/// Summary description for MyContainer.
	/// </summary>
	public class MyContainer : WindsorContainer
	{
		public MyContainer(Assembly assembly):base()
		{
			this.AddFacility("MVCFacility", new MVCFacility(assembly) );
			this.AddComponent( "state", typeof(IState), typeof(MyApplicationState));
			this.AddComponent( "viewManager", typeof(IViewManager), typeof(XmlWebViewManager));

			AddControllers();
		}
 
		private void AddControllers()
		{
			this.AddComponent( "HomeController", typeof(HomeController) );
			this.AddComponent( "AccountController", typeof(AccountController) );
		}
	}
}
