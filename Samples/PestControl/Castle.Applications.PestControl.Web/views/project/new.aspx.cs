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
using Castle.Applications.PestControl.Model;
using Castle.Applications.PestControl.Services.BuildSystems;
using Castle.Applications.PestControl.Services.SourceControl;
using Castle.MonoRail.Framework;

namespace Castle.Applications.PestControl.Web.views.project
{
	/// <summary>
	/// Summary description for _new.
	/// </summary>
	public class New : System.Web.UI.Page, IControllerAware
	{
		protected System.Web.UI.WebControls.ValidationSummary ValidationSummary1;
		protected System.Web.UI.WebControls.TextBox name;
		protected System.Web.UI.WebControls.Button Save;
		protected System.Web.UI.WebControls.DropDownList sc;
		protected System.Web.UI.WebControls.DropDownList bs;
		protected System.Web.UI.WebControls.CheckBox isPublic;
		
		private SourceControlManager _sourceControlManager;
		protected System.Web.UI.WebControls.Repeater dynRep;
		private BuildSystemManager _buildSystemManager;
		private Controller _controller;

		private void Page_Load(object sender, System.EventArgs e)
		{
			Save.Click += new EventHandler(OnSave);

			ISourceControl[] sourceControls = SourceControlManager.AvailableSourceControl();

			if (!IsPostBack)
			{
				SelectedSourceControl = -1;

				sc.AutoPostBack = true;
				sc.DataSource = sourceControls;
				sc.DataTextField = "Name";
				sc.DataValueField = "Key";
				sc.DataBind();

				bs.DataSource = BuildSystemManager.AvailableBuildSystems();
				bs.DataTextField = "Name";
				bs.DataValueField = "Key";
				bs.DataBind();
			}

			if (sc.SelectedIndex != SelectedSourceControl )
			{
				// If the user changed the combo, we rebind the
				// properties for the selected source control

				ISourceControl control = sourceControls[ sc.SelectedIndex ];

				SelectedSourceControl = sc.SelectedIndex;

				dynRep.DataSource = control.ParametersDefinition;
				dynRep.DataBind();
			}
		}

		public int SelectedSourceControl
		{
			get { return (int) this.ViewState["SelectedSourceControl"]; }
			set { this.ViewState["SelectedSourceControl"] = value; }
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
			this.dynRep.ItemCommand += new System.Web.UI.WebControls.RepeaterCommandEventHandler(this.dynRep_ItemCommand);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void dynRep_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
		{
		
		}

		private void OnSave(object sender, EventArgs e)
		{
			Hashtable properties = new Hashtable();

			foreach(RepeaterItem item in dynRep.Items)
			{
				HtmlInputControl pKey = (HtmlInputControl) item.FindControl("propKey");
				TextBox pValue = (TextBox) item.FindControl("propValue");
			
				properties.Add(pKey.Value, pValue.Text);
			}

			_controller.PropertyBag.Add("properties", properties);
			_controller.PropertyBag.Add("isPublic", isPublic.Checked);
			_controller.PropertyBag.Add("ownerEmail", ((User) Context.User).Email );

			_controller.Send("insert");
		}

		#region IControllerAware Members

		public void SetController(Controller controller)
		{
			_controller = controller;
		}

		#endregion
	}
}
