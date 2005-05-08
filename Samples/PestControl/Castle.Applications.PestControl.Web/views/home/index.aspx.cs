using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Castle.MonoRail.Framework;

namespace Castle.Applications.PestControl.Web.Views.Home
{
	public class Index : System.Web.UI.Page, IControllerAware
	{
		protected System.Web.UI.WebControls.TextBox email;
		protected System.Web.UI.WebControls.TextBox passwd;
		protected System.Web.UI.WebControls.Button LoginIn;
		protected System.Web.UI.WebControls.Button SignUp;
		private Controller _controller;

		private void Page_Load(object sender, System.EventArgs e)
		{
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		public String ErrorMessage
		{
			get { return (String) _controller.Context.Flash["ErrorMessage"]; }
		}

		public void SetController(Controller controller)
		{
			_controller = controller;
		}

		public void OnLogin(object sender, EventArgs args)
		{
			// Any validation to perform?

			// Perform login

			_controller.Send("Login");

			// Response.End();
		}

		public void OnSignUp(object sender, EventArgs args)
		{
			_controller.Send( "SignUp" );

			// Response.End();
		}
	}
}
