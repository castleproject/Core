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

		public MappingPage() : base("Fields mapping")
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		public override void Activated(System.Collections.IDictionary context)
		{
			TableDefinition table = context["selectedtable"] as TableDefinition;
			
			if (table != _oldTable)
			{
				_oldTable = table;
				
				listView1.Items.Clear();

				INamingService naming = ServiceRegistry.Instance[ typeof(INamingService) ] as INamingService;
				ITypeInferenceService typeInference = ServiceRegistry.Instance[ typeof(ITypeInferenceService) ] as ITypeInferenceService;

				foreach(ColumnDefinition col in table.Columns)
				{
					ListViewItem item = listView1.Items.Add( naming.CreatePropertyName(col.Name) );
					item.SubItems.Add( typeInference.ConvertOleType(col.Type).ToString() );
					item.SubItems.Add( col.Name );
					item.SubItems.Add( col.Type.ToString() );
				}
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
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
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

