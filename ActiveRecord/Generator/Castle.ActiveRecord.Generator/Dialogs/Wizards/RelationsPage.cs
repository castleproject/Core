// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

namespace Castle.ActiveRecord.Generator.Dialogs.Wizards
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;

	using Castle.ActiveRecord.Generator.Components;
	using Castle.ActiveRecord.Generator.Components.Database;


	public class RelationsPage : AbstractControlPage
	{
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.Label label2;
		private TableDefinition _oldTable;

		public RelationsPage() : base("Relationships mapping")
		{
			InitializeComponent();
		}

		public override void Activated(IDictionary context)
		{
			TableDefinition table = context["selectedtable"] as TableDefinition;
			
			if (table != _oldTable)
			{
				_oldTable = table;

				ActiveRecordDescriptor ar = context["ardesc"] as ActiveRecordDescriptor;

				listView1.Items.Clear();

				BuildContext buildCtx = context["buildcontext"] as BuildContext;

				IRelationshipInferenceService relationInference = 
					ServiceRegistry.Instance[ typeof(IRelationshipInferenceService) ] as IRelationshipInferenceService;

				ActiveRecordPropertyRelationDescriptor[] properties = 
					relationInference.InferRelations( ar, table, buildCtx );

				ar.PropertiesRelations.Clear();
				ar.PropertiesRelations.AddRange(properties);

				foreach(ActiveRecordPropertyRelationDescriptor property in properties)
				{
					ListViewItem item = listView1.Items.Add( property.PropertyName );
					item.Tag = property;
					item.Checked = property.Generate;
					
					if (property.TargetType != null)
					{
						item.SubItems.Add( 
							property.TargetType.ClassName != null ? property.TargetType.ClassName : "<Pendent>" );
					}
					else
					{
						throw new ApplicationException("Information missing");
					}

					item.SubItems.Add( property.RelationType );
					item.SubItems.Add( property.ColumnName );
				}
			}
		}

		public override void Deactivated(IDictionary context)
		{
			base.Deactivated(context);

			BuildContext buildCtx = context["buildcontext"] as BuildContext;
			ActiveRecordDescriptor desc = context["ardesc"] as ActiveRecordDescriptor;

			desc.PropertiesRelations.Clear();

			foreach(ListViewItem item in listView1.Items)
			{
				ActiveRecordPropertyRelationDescriptor property = item.Tag as ActiveRecordPropertyRelationDescriptor;

				if (!item.Checked)
				{
					desc.PropertiesRelations.Remove(property);
					buildCtx.IgnorePendent(property);
				}
				else
				{
					desc.PropertiesRelations.Add(property);
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
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// listView1
			// 
			this.listView1.CheckBoxes = true;
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader1,
																						this.columnHeader2,
																						this.columnHeader5,
																						this.columnHeader3});
			this.listView1.FullRowSelect = true;
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView1.LabelEdit = true;
			this.listView1.Location = new System.Drawing.Point(16, 96);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(576, 152);
			this.listView1.TabIndex = 3;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listView1_AfterLabelEdit);
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
			// columnHeader5
			// 
			this.columnHeader5.Text = "Association";
			this.columnHeader5.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.columnHeader5.Width = 120;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Column";
			this.columnHeader3.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.columnHeader3.Width = 160;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 56);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(560, 32);
			this.label1.TabIndex = 2;
			this.label1.Text = "Map the relation this class might have with others. Here is our suggestion, just " +
				"uncheck the ones that don\'t make sense";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(24, 264);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(560, 48);
			this.label2.TabIndex = 5;
			this.label2.Text = "Please note that sometimes the relationship points to an ActiveRecord class that " +
				"does not exist yet. In this case, the generator will create them for you. You ca" +
				"n always customize them later.";
			// 
			// RelationsPage
			// 
			this.Controls.Add(this.label2);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.label1);
			this.Name = "RelationsPage";
			this.Size = new System.Drawing.Size(611, 344);
			this.ResumeLayout(false);

		}
		#endregion

		private void listView1_AfterLabelEdit(object sender, System.Windows.Forms.LabelEditEventArgs e)
		{
			if (e.Label == null || e.Label.Length == 0)
			{
				e.CancelEdit = true;
			}
			else
			{
				ActiveRecordPropertyDescriptor desc = listView1.Items[e.Item].Tag as ActiveRecordPropertyDescriptor;			
				desc.PropertyName = e.Label;
			}
		}
	}
}

