using System;
using System.Reflection;
using Castle.MVC.Navigation;
using Castle.MVC.StatePersister;
using Castle.MVC.States;
using Castle.MVC.Views;
using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Windsor.Configuration.Sources;

namespace Castle.MVC.Test.Presentation
{
	/// <summary>
	/// Summary description for MyContainer.
	/// </summary>
	public class MyContainer : WindsorContainer
	{
		public MyContainer() : base( new XmlInterpreter( new AppDomainConfigSource("castle") ),new XmlInterpreter( "mvc.config" ))
		{
		}

	}
}
