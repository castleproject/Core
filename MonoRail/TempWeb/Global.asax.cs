namespace TempWeb
{
	using System;
	using System.IO;
	using Castle.Core.Configuration;
	using Castle.MicroKernel.Registration;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Configuration;
	using Castle.MonoRail.Framework.Extensions.ExceptionChaining;
	using Castle.MonoRail.Framework.Internal;
	using Castle.MonoRail.Framework.Views.Aspx;
	using Castle.MonoRail.Framework.Views.NVelocity;
	using Castle.MonoRail.WindsorExtension;
	using Castle.Windsor;
	using Controllers;

	public class Global : System.Web.HttpApplication, IContainerAccessor, IMonoRailConfigurationEvents
	{
		private static WindsorContainer container;

		public void Configure(IMonoRailConfiguration config)
		{
			config.ControllersConfig.AddAssembly(typeof(Global).Assembly);
			config.ViewEngineConfig.ViewPathRoot = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Views");
			config.ViewEngineConfig.ViewEngines.Add(new ViewEngineInfo(typeof(NVelocityViewEngine), false));
			config.ViewEngineConfig.ViewEngines.Add(new ViewEngineInfo(typeof(WebFormsViewEngine), false));

			config.ExtensionEntries.Add(new ExtensionEntry(typeof(ExceptionChainingExtension), null));

			MutableConfiguration configSection = new MutableConfiguration("monorail");
			IConfiguration exceptionNode = configSection.Children.Add(new MutableConfiguration("exception"));

			exceptionNode.Children.Add(new MutableConfiguration("handler")).Attributes["type"] = typeof(LocalExceptionFilterHandler).FullName;

			config.ConfigurationSection = configSection;
		}

		public void Application_OnStart()
		{
			container = new WindsorContainer();
			container.AddFacility("rails", new MonoRailFacility());

			container.Register(Component.For<HomeController>().Named("home.controller"));
			container.Register(Component.For<AccCreationWizard>().Named("acc.creation.wizard"));
			container.Register(Component.For<Home2Controller>().Named("home2.controller"));
		}

		public void Application_OnEnd()
		{
			container.Dispose();
		}

		#region IContainerAccessor implementation

		public IWindsorContainer Container
		{
			get { return container; }
		}

		#endregion
	}
}