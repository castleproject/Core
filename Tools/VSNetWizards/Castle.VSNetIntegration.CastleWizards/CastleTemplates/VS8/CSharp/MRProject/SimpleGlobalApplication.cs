namespace !NAMESPACE!
{
	using System;
	using System.Web;

	public class GlobalApplication : HttpApplication
	{
		public GlobalApplication()
		{
			this.BeginRequest += new EventHandler(OnBeginRequest);
			this.EndRequest += new EventHandler(OnEndRequest);
			this.AuthenticateRequest += new EventHandler(Application_AuthenticateRequest);
		}

		public void Application_OnStart()
		{
		}

		public void Application_OnEnd() 
		{
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
