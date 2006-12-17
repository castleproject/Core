using System;
using System.Web.UI.WebControls;
using Igloo.Clinic.Application;
using Castle.Igloo.UI.Web;

namespace Igloo.Clinic.Web.Views
{
	/// <summary>
	/// Description résumée de MyUserControl.
	/// </summary>
	public class MyUserControl : UserControl
	{
		protected Button GoToPage2;
		private LoginController _homeController = null;

		public LoginController AccountController
		{
			set
			{
				_homeController = value;
			}
		}

		private void Page_Load(object sender, EventArgs e)
		{
		}

		private void GoToPage2_Click(object sender, EventArgs e)
		{
            //this.State["toto"] = "fdfsdfdsf";
            //(this.State as MyApplicationState).SomeSessionString = "SomeSessionString";
			_homeController.Validate("ggg","jjj");
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
