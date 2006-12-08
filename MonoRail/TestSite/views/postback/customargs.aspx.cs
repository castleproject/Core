using System;
using System.Collections;

using Castle.MonoRail.Framework;

namespace AspnetSample.views.postback
{
	/// <summary>
	/// Summary description for index.
	/// </summary>
	public class CustomArgs : System.Web.UI.Page, IControllerAware
	{
		public string name;
		private Controller _controller;

		private void Page_Load(object sender, System.EventArgs e)
		{
			Hashtable arguments = new Hashtable();
			arguments.Add("i", null);
			arguments.Add("d", null);
			arguments.Add("s", null);
			
			_controller.Send("TestNullArgs", arguments);
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
