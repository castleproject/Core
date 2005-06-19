using System;
using System.Web.UI.WebControls;
using Castle.MVC.Test.Presentation;
using Castle.MVC.Views;

namespace Castle.MVC.Test.Web.Views
{
	/// <summary>
	/// Description résumée de MyUserControl.
	/// </summary>
	public class MyUserControl : WebUserControlView
	{
		protected Button GoToPage2;
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
			// Put user code to initialize the page here
		}

		private void GoToPage2_Click(object sender, EventArgs e)
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
		///		Méthode requise pour la prise en charge du concepteur - ne modifiez pas
		///		le contenu de cette méthode avec l'éditeur de code.
		/// </summary>
		private void InitializeComponent()
		{
			this.GoToPage2.Click += new EventHandler(this.GoToPage2_Click);
			this.Load += new EventHandler(this.Page_Load);

		}
		#endregion

	}
}
