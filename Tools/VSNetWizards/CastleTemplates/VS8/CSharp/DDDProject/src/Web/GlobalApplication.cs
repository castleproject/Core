namespace !NAMESPACE!.Web
{
	using System.Web;
	using Castle.Windsor;
	using Castle.Windsor.Configuration.Interpreters;

	public class GlobalApplication : HttpApplication, IContainerAccessor
	{
		private static IWindsorContainer container;

		#region IContainerAccessor

		public IWindsorContainer Container
		{
			get { return container; }
		}

		#endregion

		public void Application_OnStart()
		{
			container = new WindsorContainer(new XmlInterpreter(), new WebEnvInfo());
		}

		public void Application_OnEnd() 
		{
			container.Dispose();
		}
	}

	internal class WebEnvInfo : IEnvironmentInfo
	{
		public string GetEnvironmentName()
		{
			return HttpContext.Current.Request.Url.Port == 88 ? "test" : "production";
		}
	}
}
