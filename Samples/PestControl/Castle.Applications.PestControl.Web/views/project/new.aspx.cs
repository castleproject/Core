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
using Castle.Applications.PestControl.Services.BuildSystems;
using Castle.Applications.PestControl.Services.SourceControl;

namespace Castle.Applications.PestControl.Web.views.project
{
	/// <summary>
	/// Summary description for _new.
	/// </summary>
	public class New : System.Web.UI.Page
	{
		protected System.Web.UI.WebControls.ValidationSummary ValidationSummary1;
		protected System.Web.UI.WebControls.TextBox name;
		protected System.Web.UI.WebControls.Button Save;
		protected System.Web.UI.WebControls.DropDownList sc;
		protected System.Web.UI.WebControls.DropDownList bs;
		
		private SourceControlManager _sourceControlManager;
		private BuildSystemManager _buildSystemManager;

		private void Page_Load(object sender, System.EventArgs e)
		{
			sc.DataSource = this.SourceControlManager.AvailableSourceControl();
			sc.DataTextField = "Name";
			sc.DataValueField = "Key";
			sc.DataBind();

			bs.DataSource = this.BuildSystemManager.AvailableBuildSystems();
			bs.DataTextField = "Name";
			bs.DataValueField = "Key";
			bs.DataBind();
		}

		public SourceControlManager SourceControlManager
		{
			get { return _sourceControlManager; }
			set { _sourceControlManager = value; }
		}

		public BuildSystemManager BuildSystemManager
		{
			get { return _buildSystemManager; }
			set { _buildSystemManager = value; }
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
	}
}
