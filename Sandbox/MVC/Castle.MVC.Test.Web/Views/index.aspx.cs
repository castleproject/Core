using System;
using System.Web.UI.WebControls;
using Castle.MVC.Test.Presentation;
using Castle.MVC.Views;

namespace Castle.MVC.Test.Web.Views
{
	/// <summary>
	/// Description résumée de index.
	/// </summary>
	public class index : WebFormView
	{
		protected TextBox email;
		protected TextBox passwd;
		protected Button ButtonLogin;
		protected LinkButton LinkToPage2;
		private HomeController _homeController = null;

		public HomeController HomeController
		{
			set
			{
				_homeController = value;
			}
		}

		private void Page_Load(object sender, EventArgs e)
		{
		}


		private void ButtonLogin_Click(object sender, System.EventArgs e)
		{
			this.State["toto"] = "fdfsdfdsf";
			(this.State as MyApplicationState).SomeSessionString = "SomeSessionString";
			_homeController.Login("ggg","jjj");		
		}

		private void LinkToPage2_Click(object sender, EventArgs e)
		{
			this.State["toto"] = "fdfsdfdsf";
			(this.State as MyApplicationState).SomeSessionString = "SomeSessionString";
			_homeController.Login("ggg","jjj");
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
			this.ButtonLogin.Click += new System.EventHandler(this.ButtonLogin_Click);
			this.LinkToPage2.Click += new System.EventHandler(this.LinkToPage2_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

	}
}
