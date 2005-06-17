using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Castle.MVC.Controllers;
using Castle.MVC.Views;

using Castle.MVC.Web.Controllers;

namespace Castle.MVC.Web.views.home
{
	/// <summary>
	/// Description résumée de Page2.
	/// </summary>
	[Controller( typeof(HomeController) )]
	public class Page2 : WebFormView
	{
		protected System.Web.UI.WebControls.Literal LiteralPreviousView;
		protected System.Web.UI.WebControls.Button GoPage1;
	
		private void Page_Load(object sender, System.EventArgs e)
		{
			string s= (this.State as MyApplicationState).SomeSessionString;
			if (s!="SomeSessionString")
			{
				int i =0;
				i = 1/i;
			}
			LiteralPreviousView.Text = this.State.PreviousView;
		}

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

		private void GoPage1_Click(object sender, System.EventArgs e)
		{
			this.Controller.Login2("ggg","");
		}

		#region Code généré par le Concepteur Web Form
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		/// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		/// le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent()
		{    
			this.GoPage1.Click += new System.EventHandler(this.GoPage1_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion



	}
}
