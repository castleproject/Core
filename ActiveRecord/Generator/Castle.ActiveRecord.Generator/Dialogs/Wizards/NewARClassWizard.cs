using Castle.ActiveRecord.Generator.Components.Database;
using Castle.ActiveRecord.Generator.Parts.Shapes;

namespace Castle.ActiveRecord.Generator.Dialogs.Wizards
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;
	using Netron.GraphLib;

	public class NewARClassWizard : WizardBaseDialog
	{
		private System.ComponentModel.IContainer components = null;
		private Shape _shape;

		public NewARClassWizard(Model model, Shape shape) : base(model)
		{
			_shape = shape;
			Context["ardesc"] = new ActiveRecordDescriptor();

			InitializeComponent();

			Title = "New ActiveRecord class";

			AddPage(new WelcomePage());
			AddPage(new TableSelectionPage());
			AddPage(new MappingPage());
			AddPage(new RelationsPage());
			AddPage(new ClassNamePage());
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

		protected override void DoProcess()
		{
			ActiveRecordDescriptor ar = Context["ardesc"] as ActiveRecordDescriptor;

			ar.TableName = (Context["selectedtable"] as TableDefinition).Name;
			ar.DbAlias = (Context["selecteddb"] as DatabaseDefinition).Alias;

			(_shape as ActiveRecordShape).ActiveRecordDescriptor = ar;
		}
	}
}

