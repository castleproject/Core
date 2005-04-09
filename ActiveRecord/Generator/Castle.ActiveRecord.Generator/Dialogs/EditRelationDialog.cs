namespace Castle.ActiveRecord.Generator.Dialogs
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using Castle.ActiveRecord.Generator.Components;
	using Castle.ActiveRecord.Generator.Components.Database;


	public class EditRelationDialog : AddRelationDialog
	{
		private System.ComponentModel.IContainer components = null;

		public EditRelationDialog() : this(null, null, null)
		{
		}

		public EditRelationDialog(ActiveRecordDescriptor descriptor, Project project, ActiveRecordPropertyRelationDescriptor prop) : base(descriptor, project)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			AssociationEnum assoc = AssociationEnum.Undefined;

			if (prop is ActiveRecordHasManyDescriptor)
			{
				hasManyButton.Checked = true;
				assoc = AssociationEnum.HasMany;
			}
			else if (prop is ActiveRecordBelongsToDescriptor)
			{
				belongsToButton.Checked = true;
				assoc = AssociationEnum.BelongsTo;
			}
			else if (prop is ActiveRecordHasAndBelongsToManyDescriptor)
			{
				hasAndBelongsToManyButton.Checked = true;
				assoc = AssociationEnum.HasAndBelongsToMany;
			}

			SelectedTarget = prop.TargetType;

			SwitchViewTo(assoc);

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
			// 
			// EditRelationDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(632, 330);
			this.Name = "EditRelationDialog";
			this.Text = "Edit Relationship";

		}
		#endregion
	}
}

