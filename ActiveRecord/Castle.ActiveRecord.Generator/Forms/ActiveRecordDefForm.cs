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

namespace Castle.ActiveRecord.Generator.Forms
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Castle.Model;

	using Castle.ActiveRecord.Generator.Model;


	public class ActiveRecordDefForm : Form
	{
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button saveButton;
		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.ColumnHeader propertyHeader;
		private System.Windows.Forms.ColumnHeader columnHeader;
		private System.Windows.Forms.ColumnHeader typeHeader;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private IApplicationModel _model;
		private ActiveRecordDescriptorBuilder _arDescBuilder;
		private ActiveRecordDescriptor _currentActiveRecord;


		public ActiveRecordDefForm(IApplicationModel model, ActiveRecordDescriptorBuilder arDescBuilder)
		{
			_model = model;
			_arDescBuilder = arDescBuilder;

			InitializeComponent();

			comboBox1.SelectedIndexChanged += new EventHandler(comboBox1_SelectedIndexChanged);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		public void Initialize()
		{
			comboBox1.Items.Clear();

			foreach(TableDefinition table in _model.CurrentProject.DatabaseDefinition.Tables)
			{
				comboBox1.Items.Add(table); 
			}
		}

		public ActiveRecordDescriptor CurrentActiveRecord
		{
			get { return _currentActiveRecord; }
			set
			{
				_currentActiveRecord = value;

				listView1.Items.Clear();

				if (_currentActiveRecord == null) return;

				foreach(ActiveRecordPropertyDescriptor property in _currentActiveRecord.Properties)
				{
					ListViewItem item = new ListViewItem(property.PropertyName);				
					item.Tag = property;
					item.Checked = property.Generate;
					item.SubItems.Add(property.ColumnName);
					item.SubItems.Add(property.ColumnTypeName);
					listView1.Items.Add(item);
				}
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.listView1 = new System.Windows.Forms.ListView();
			this.propertyHeader = new System.Windows.Forms.ColumnHeader();
			this.columnHeader = new System.Windows.Forms.ColumnHeader();
			this.typeHeader = new System.Windows.Forms.ColumnHeader();
			this.label1 = new System.Windows.Forms.Label();
			this.comboBox1 = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.saveButton = new System.Windows.Forms.Button();
			this.closeButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// listView1
			// 
			this.listView1.CheckBoxes = true;
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.propertyHeader,
																						this.columnHeader,
																						this.typeHeader});
			this.listView1.GridLines = true;
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView1.LabelEdit = true;
			this.listView1.Location = new System.Drawing.Point(12, 60);
			this.listView1.MultiSelect = false;
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(444, 168);
			this.listView1.TabIndex = 0;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listView1_AfterLabelEdit);
			this.listView1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listView1_ItemCheck);
			// 
			// propertyHeader
			// 
			this.propertyHeader.Text = "Property";
			this.propertyHeader.Width = 180;
			// 
			// columnHeader
			// 
			this.columnHeader.Text = "Column";
			this.columnHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.columnHeader.Width = 110;
			// 
			// typeHeader
			// 
			this.typeHeader.Text = "Type";
			this.typeHeader.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.typeHeader.Width = 110;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Table:";
			// 
			// comboBox1
			// 
			this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBox1.Location = new System.Drawing.Point(140, 12);
			this.comboBox1.Name = "comboBox1";
			this.comboBox1.Size = new System.Drawing.Size(212, 21);
			this.comboBox1.TabIndex = 2;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 44);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 3;
			this.label2.Text = "Mappings:";
			// 
			// saveButton
			// 
			this.saveButton.Location = new System.Drawing.Point(376, 244);
			this.saveButton.Name = "saveButton";
			this.saveButton.TabIndex = 4;
			this.saveButton.Text = "Save";
			this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
			// 
			// closeButton
			// 
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.closeButton.Location = new System.Drawing.Point(292, 244);
			this.closeButton.Name = "closeButton";
			this.closeButton.TabIndex = 5;
			this.closeButton.Text = "Close";
			this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
			// 
			// ActiveRecordDefForm
			// 
			this.AcceptButton = this.saveButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.CancelButton = this.closeButton;
			this.ClientSize = new System.Drawing.Size(466, 280);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.saveButton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboBox1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.listView1);
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.Name = "ActiveRecordDefForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "ActiveRecord Definition";
			this.ResumeLayout(false);

		}
		#endregion

		private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (comboBox1.SelectedItem != null)
			{
				TableDefinition table = comboBox1.SelectedItem as TableDefinition;

				if (table.RelatedDescriptor == null)
				{
					CurrentActiveRecord = _arDescBuilder.Build( table );
					table.RelatedDescriptor = CurrentActiveRecord;
				}
				else
				{
					CurrentActiveRecord = table.RelatedDescriptor;
				}
			}
		}

		private void listView1_ItemCheck(object sender, ItemCheckEventArgs e)
		{
			ListViewItem item = listView1.Items[ e.Index ];
			ActiveRecordPropertyDescriptor property = (ActiveRecordPropertyDescriptor) item.Tag;
			property.Generate = e.NewValue == CheckState.Checked;
		}

		private void saveButton_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
		}

		private void closeButton_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void listView1_AfterLabelEdit(object sender, LabelEditEventArgs e)
		{
			if (e.Label == null || e.Label.IndexOf(' ') != -1)
			{
				e.CancelEdit = true;
				return;
			}

			ListViewItem item = listView1.Items[ e.Item ];
			ActiveRecordPropertyDescriptor property = (ActiveRecordPropertyDescriptor) item.Tag;
			property.PropertyName = e.Label;
		}
	}
}
