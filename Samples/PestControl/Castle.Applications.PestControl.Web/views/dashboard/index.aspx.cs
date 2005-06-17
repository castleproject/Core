using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Castle.Applications.PestControl.Model;
using Castle.MonoRail.Framework;

namespace Castle.Applications.PestControl.Web.Views.Dashboard
{
	public class Index : System.Web.UI.Page, IControllerAware
	{
		private Controller _controller;
		public Repeater projectsRepeater;
		protected ProjectCollection _projects;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				projectsRepeater.DataSource = _projects;
				projectsRepeater.DataBind();
			}
		}

		public ProjectCollection Projects
		{
			get { return _projects; }
			set { _projects = value; }
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

		public void SetController(Controller controller)
		{
			_controller = controller;
		}
	}
}
