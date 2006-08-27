namespace !NAMESPACE!
{
	using System;
	using System.Web;

	using Castle.Core;
	using Castle.Windsor;


	public class GlobalApplication : HttpApplication, IContainerAccessor
	{
		private static WebContainer container;

		public GlobalApplication()
		{
			this.BeginRequest += new EventHandler(OnBeginRequest);
			this.EndRequest += new EventHandler(OnEndRequest);
			this.AuthenticateRequest += new EventHandler(Application_AuthenticateRequest);
		}

		#region IContainerAccessor

		public IWindsorContainer Container
		{
			get { return container; }
		}

		#endregion

		public void Application_OnStart()
		{
			container = new WebContainer();
		}

		public void Application_OnEnd() 
		{
			container.Dispose();
		}

		public void Application_AuthenticateRequest(object sender, EventArgs e)
		{
			
		}

		public void OnBeginRequest(object sender, EventArgs e)
		{
			// If you intend to use AR with lazy loading enabled
			// HttpContext.Current.Items.Add( "nh.sessionscope", new SessionScope() );
		}

		public void OnEndRequest(object sender, EventArgs e)
		{
			// Same thing
			// SessionScope scope = (SessionScope) HttpContext.Current.Items["nh.sessionscope"];
			// try
			// {
			// 	if (scope != null) scope.Dispose();
			// }
			// catch(Exception ex)
			// {
			// 	HttpContext.Current.Trace.Warn( "Error", "Problems with the session:" + ex.Message, ex );
			// }
		}
	}
}
