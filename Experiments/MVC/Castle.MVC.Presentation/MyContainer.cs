using System;
using System.Reflection;
using Castle.MVC.Navigation;
using Castle.MVC.StatePersister;
using Castle.MVC.States;
using Castle.MVC.Views;
using Castle.Windsor;
using Castle.Windsor.Configuration;
using Castle.Windsor.Configuration.Interpreters;

namespace Castle.MVC.Test.Presentation
{
	/// <summary>
	/// Summary description for MyContainer.
	/// </summary>
	public class MyContainer : WindsorContainer
	{
		public MyContainer( IConfigurationInterpreter interpreter ) : base(interpreter)
		{
		}

	}
}
