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
using Castle.CastleOnRails.Framework;

namespace TestSite.views.ajax
{
	/// <summary>
	/// Summary description for index.
	/// </summary>
	public class index : System.Web.UI.Page, IControllerAware
	{
		protected Castle.CastleOnRails.Framework.Views.Aspx.InvokeHelper GetJavascriptFunctions;
		protected Castle.CastleOnRails.Framework.Views.Aspx.InvokeHelper ObserveField_Zip;
		protected System.Web.UI.WebControls.DataGrid DataGrid1;
		protected System.Web.UI.WebControls.TextBox name;
		protected System.Web.UI.WebControls.TextBox addressf;
		protected Castle.CastleOnRails.Framework.Views.Aspx.InvokeHelper ObserveForm;

		private Controller _controller;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (!IsPostBack)
			{
				DataGrid1.DataSource = _controller.PropertyBag["users"];
				DataGrid1.DataBind();
			}
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
