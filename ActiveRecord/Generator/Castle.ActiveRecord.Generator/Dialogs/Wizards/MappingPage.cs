namespace Castle.ActiveRecord.Generator.Dialogs.Wizards
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;
	using Castle.ActiveRecord.Generator.Components;
	using Castle.ActiveRecord.Generator.Components.Database;

	public class MappingPage : Castle.ActiveRecord.Generator.Dialogs.Wizards.AbstractControlPage
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.ComponentModel.IContainer components = null;
		private TableDefinition _oldTable;
		private INamingService naming;
		private ITypeInferenceService typeInference;

		public MappingPage() : base("Fields mapping")
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			naming = ServiceRegistry.Instance[ typeof(INamingService) ] as INamingService;
			typeInference = ServiceRegistry.Instance[ typeof(ITypeInferenceService) ] as ITypeInferenceService;
		}

		public override void Activated(IDictionary context)
		{
			TableDefinition table = context["selectedtable"] as TableDefinition;
			
			if (table != _oldTable)
			{
				_oldTable = table;

				IPlainFieldInferenceService fieldInference = ServiceRegistry.Instance[ typeof(IPlainFieldInferenceService) ] as IPlainFieldInferenceService;

				ActiveRecordPropertyDescriptor[] properties = 
					fieldInference.InferProperties( table );

				ActiveRecordDescriptor ar = context["ardesc"] as ActiveRecordDescriptor;
				ar.Properties.Clear();
				ar.Properties.AddRange(properties);
				

				listView1.Items.Clear();

				foreach(ActiveRecordPropertyDescriptor desc in properties)
				{
					ListViewItem item = listView1.Items.Add( desc.PropertyName );
					item.Tag = desc;
					item.Checked = desc.Generate;
					item.SubItems.Add( desc.PropertyType.Name );
					item.SubItems.Add( desc.ColumnName );
					item.SubItems.Add( desc.ColumnTypeName );
				}
			}
		}

		public override void Deactivated(IDictionary context)
		{
			base.Deactivated(context);

			ActiveRecordDescriptor desc = context["ardesc"] as ActiveRecordDescriptor;

			desc.Properties.Clear();

			foreach(ListViewItem item in listView1.Items)
			{
				ActiveRecordPropertyDescriptor property = item.Tag as ActiveRecordPropertyDescriptor;
				property.Generate = item.Checked;
			}
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
			this.label1 = new System.Windows.Forms.Label();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 56);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(568, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Check the fields you\'d like to map. You can also change the corresponding propert" +
				"y names.";
			// 
			// listView1
			// 
			this.listView1.CheckBoxes = true;
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader1,
																						this.columnHeader2,
																						this.columnHeader3,
																						this.columnHeader4});
			this.listView1.FullRowSelect = true;
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView1.LabelEdit = true;
			this.listView1.Location = new System.Drawing.Point(16, 88);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(576, 232);
			this.listView1.TabIndex = 1;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Property";
			this.columnHeader1.Width = 190;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Type";
			this.columnHeader2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.columnHeader2.Width = 90;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Column";
			this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.columnHeader3.Width = 160;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Type";
			this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.columnHeader4.Width = 90;
			// 
			// MappingPage
			// 
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.label1);
			this.Name = "MappingPage";
			this.Size = new System.Drawing.Size(616, 344);
			this.ResumeLayout(false);

		}
		#endregion
	}
}

