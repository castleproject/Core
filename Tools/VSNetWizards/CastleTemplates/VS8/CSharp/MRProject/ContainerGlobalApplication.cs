namespace !NAMESPACE!
{
	using System;
	using System.Web;

	using Castle.Windsor;
	using Castle.Windsor.Configuration.Interpreters;

	public class GlobalApplication : HttpApplication, IContainerAccessor
	{
		private static IWindsorContainer container;

		public GlobalApplication()
		{
		}

		#region IContainerAccessor

		public IWindsorContainer Container
		{
			get { return container; }
		}

		#endregion

		public void Application_OnStart()
		{
			container = new WindsorContainer(new XmlInterpreter());
		}

		public void Application_OnEnd() 
		{
			container.Dispose();
		}
	}
}
