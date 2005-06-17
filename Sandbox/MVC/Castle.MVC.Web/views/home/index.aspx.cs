using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Castle.MVC.Controllers;
using Castle.MVC.Views;

using Castle.MVC.Web.Controllers;

namespace Castle.MVC.Web.Views.Home
{
	[Controller( typeof(HomeController) )]
	public class Index : WebFormView
	{
		protected System.Web.UI.WebControls.TextBox email;
		protected System.Web.UI.WebControls.TextBox passwd;
		protected System.Web.UI.WebControls.Button LoginIn;
		protected System.Web.UI.WebControls.Button Button1;
		protected System.Web.UI.WebControls.Button Login;
		protected System.Web.UI.WebControls.Button SignUp;

		/// <summary>
		/// Strong-typed controller property 
		/// that returns the controller so we get nice IntelliSense 
		/// and less casting errors.
		/// </summary>
		public HomeController Controller
		{
			get
			{
				return this.ControllerBase as HomeController;
			}
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			//Console.Write(e.ToString());
		}

		public void OnLogin(object sender, CommandEventArgs e)
		{
			this.State["toto"] = "fdfsdfdsf";
			this.Controller.MyState.SomeSessionString = "SomeSessionString";
			Controller.Login("ggg","jjj");
		}

		public void OnSignUp(object sender, EventArgs args)
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
			this.Button1.Click += new System.EventHandler(this.Button1_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void Button1_Click(object sender, System.EventArgs e)
		{
		
		}

	}
}
