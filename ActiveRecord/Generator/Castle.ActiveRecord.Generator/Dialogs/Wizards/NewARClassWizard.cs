using Castle.ActiveRecord.Generator.Components;

namespace Castle.ActiveRecord.Generator.Dialogs.Wizards
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;
	using Netron.GraphLib;

	using Castle.ActiveRecord.Generator.Components.Database;
	using Castle.ActiveRecord.Generator.Parts.Shapes;


	public class NewARClassWizard : WizardBaseDialog
	{
		private System.ComponentModel.IContainer components = null;
		private ActiveRecordShape _shape;
		private ActiveRecordDescriptor[] _dependents;

		public NewARClassWizard(Model model, ActiveRecordShape shape) : base(model)
		{
//			if (shape.ActiveRecordDescriptor == null)
//			{
//				throw new ArgumentException("No AR instance in shape");
//			}

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

			ar.Table = (Context["selectedtable"] as TableDefinition);
			ar.Table.RelatedDescriptor = ar;
			_shape.ActiveRecordDescriptor = ar;

			// Build dependencies

			IActiveRecordDescriptorBuilder builder = 
				ServiceRegistry.Instance[ typeof(IActiveRecordDescriptorBuilder) ] 
				as IActiveRecordDescriptorBuilder;

			BuildContext buildContext = Context["buildcontext"] as BuildContext;
			Dependents = builder.Build( buildContext );
		}

		public ActiveRecordDescriptor[] Dependents
		{
			set { _dependents = value; }
			get { return _dependents;  }
		}
	}
}

