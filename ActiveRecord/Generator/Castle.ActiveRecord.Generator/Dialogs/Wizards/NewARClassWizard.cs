using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace Castle.ActiveRecord.Generator.Dialogs.Wizards
{
	public class NewARClassWizard : Castle.ActiveRecord.Generator.Dialogs.Wizards.WizardBaseDialog
	{
		private System.ComponentModel.IContainer components = null;

		public NewARClassWizard(Model model) : base(model)
		{
			InitializeComponent();

			Title = "New ActiveRecord class";

			AddPage(new WelcomePage());
			AddPage(new TableSelectionPage());
			AddPage(new MappingPage());
			AddPage(new RelationsPage());
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			components = new System.ComponentModel.Container();
		}
		#endregion
	}
}

