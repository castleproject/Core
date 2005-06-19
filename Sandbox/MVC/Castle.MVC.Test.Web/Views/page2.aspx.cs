using System;
using System.Web.UI.WebControls;
using Castle.MVC.Test.Presentation;
using Castle.MVC.Views;

namespace Castle.MVC.Test.Web.Views
{
	/// <summary>
	/// Description résumée de page2.
	/// </summary>
	public class page2 : WebFormView
	{
		protected Button Button;
		protected System.Web.UI.WebControls.Label LabelPreviousView;
		private HomeController _homeController;
	
		private void Page_Load(object sender, EventArgs e)
		{
			string s= (this.State as MyApplicationState).SomeSessionString;
			if (s!="SomeSessionString")
			{
				int i =0;
				i = 1/i;
			}
			LabelPreviousView.Text = this.State.PreviousView;
		}

		public HomeController HomeController
		{
			set
			{
				_homeController = value;
			}
		}

		private void Button_Click(object sender, EventArgs e)
		{
			this.State["toto"] = "fdfsdfdsf";
			(this.State as MyApplicationState).SomeSessionString = "SomeSessionString";
			_homeController.Login2("ggg","");
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
			this.Button.Click += new System.EventHandler(this.Button_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

	}
}
