using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;

using NVelocity.App;
using NVelocity.Runtime;
using NVelocity.Runtime.Log;

namespace ViewHandler {

    /// <summary>
    /// Summary description for Global.
    /// </summary>
    public class Global : System.Web.HttpApplication {

	private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

	public Global() {
	    InitializeComponent();
	}

	protected void Application_Start(Object sender, EventArgs e) {
	    log.Info("Web application starting");
	}

	protected void Session_Start(Object sender, EventArgs e) {}

	protected void Application_BeginRequest(Object sender, EventArgs e) {}

	protected void Application_EndRequest(Object sender, EventArgs e) {}

	protected void Application_AuthenticateRequest(Object sender, EventArgs e) {}

	protected void Application_Error(Object sender, EventArgs e) {
	    log.Error(Server.GetLastError());
	}

	protected void Session_End(Object sender, EventArgs e) {}

	protected void Application_End(Object sender, EventArgs e) {
	    log.Info("Web application stopping");
	}

	#region Web Form Designer generated code
	/// <summary>
	/// Required method for Designer support - do not modify
	/// the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent() {}
	#endregion
    }
}

