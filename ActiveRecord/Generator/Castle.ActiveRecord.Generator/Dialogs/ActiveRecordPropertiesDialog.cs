// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Generator.Dialogs
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Castle.ActiveRecord.Generator.Components;
	using Castle.ActiveRecord.Generator.Components.Database;

	/// <summary>
	/// Summary description for ActiveRecordPropertiesDialog.
	/// </summary>
	public class ActiveRecordPropertiesDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TabPage tabPage3;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.ColumnHeader columnHeader4;
		private System.Windows.Forms.Label title;
		private System.Windows.Forms.ColumnHeader columnHeader5;
		private System.Windows.Forms.ColumnHeader columnHeader6;
		private System.Windows.Forms.ColumnHeader columnHeader7;
		private System.Windows.Forms.ColumnHeader columnHeader8;
		private System.Windows.Forms.ListView listView2;
		private System.Windows.Forms.ColumnHeader columnHeader9;
		private System.Windows.Forms.ColumnHeader columnHeader10;
		private System.Windows.Forms.ColumnHeader columnHeader11;
		private System.Windows.Forms.ColumnHeader columnHeader12;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button AddRelation;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.CheckBox useDiscriminator;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox discValue;
		private System.Windows.Forms.ComboBox discColumn;
		private System.Windows.Forms.CheckBox isJoinedSubClass;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox joinedSubKeyColumn;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox parentClass;
		private System.Windows.Forms.TextBox className;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.ColumnHeader columnHeader13;
		private System.Windows.Forms.Button cancelButton;

		private ActiveRecordDescriptor _descriptor;
		private ActiveRecordDescriptor _parent;
		private System.Windows.Forms.Button EditRelation;
		private Project _project;


		public ActiveRecordPropertiesDialog(ActiveRecordDescriptor descriptor, Project project)
		{
			_project = project;
			_descriptor = descriptor;

			if (descriptor is ActiveRecordDescriptorSubClass)
			{
				_parent = ((ActiveRecordDescriptorSubClass) descriptor).BaseClass;
			}

			InitializeComponent();

			Title = descriptor.ClassName + " Properties";

			FillClassDetails(descriptor);
			FillClassProperties(descriptor);
			FillClassRelationships(descriptor);
		}

		private void FillClassDetails(ActiveRecordDescriptor descriptor)
		{
			className.Text = descriptor.ClassName;
	
			parentClass.Items.Add( "ActiveRecordBase" );
	
			foreach(TableDefinition table in descriptor.Table.DatabaseDefinition.Tables)
			{
				if (table.RelatedDescriptor == null) continue;
				if (table.RelatedDescriptor.ClassName == null) continue;
				if (table.RelatedDescriptor == descriptor) continue;

				parentClass.Items.Add( table.RelatedDescriptor.ClassName );
			}
	
			parentClass.Enabled = false;
			groupBox2.Enabled = false;
	
			discColumn.ValueMember = "Name";
	
			foreach(ColumnDefinition col in descriptor.Table.Columns)
			{
				discColumn.Items.Add( col );
				
				if (col.Name.Equals(descriptor.DiscriminatorField))
				{
					discColumn.SelectedItem = col;
				}
			}

			useDiscriminator.Checked = descriptor.DiscriminatorField != null;

			if (descriptor.DiscriminatorValue != null)
			{
				discValue.Text = descriptor.DiscriminatorValue;
			}
	
			if (descriptor is ActiveRecordDescriptorSubClass ||
				descriptor is ActiveRecordDescriptorJoinedSubClass )
			{
				useDiscriminator.Enabled = false;
				isJoinedSubClass.Enabled = false;
			}
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.title = new System.Windows.Forms.Label();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.parentClass = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.joinedSubKeyColumn = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.isJoinedSubClass = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.discValue = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.discColumn = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.useDiscriminator = new System.Windows.Forms.CheckBox();
			this.className = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader13 = new System.Windows.Forms.ColumnHeader();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.EditRelation = new System.Windows.Forms.Button();
			this.AddRelation = new System.Windows.Forms.Button();
			this.listView2 = new System.Windows.Forms.ListView();
			this.columnHeader9 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader10 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader11 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader12 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader5 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader6 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader7 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader8 = new System.Windows.Forms.ColumnHeader();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.tabControl1.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// title
			// 
			this.title.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.title.Location = new System.Drawing.Point(16, 12);
			this.title.Name = "title";
			this.title.Size = new System.Drawing.Size(648, 23);
			this.title.TabIndex = 1;
			this.title.Text = "Class properties";
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(16, 40);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(656, 320);
			this.tabControl1.TabIndex = 2;
			// 
			// tabPage3
			// 
			this.tabPage3.Controls.Add(this.parentClass);
			this.tabPage3.Controls.Add(this.label3);
			this.tabPage3.Controls.Add(this.groupBox2);
			this.tabPage3.Controls.Add(this.groupBox1);
			this.tabPage3.Controls.Add(this.className);
			this.tabPage3.Controls.Add(this.label1);
			this.tabPage3.Location = new System.Drawing.Point(4, 22);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(648, 294);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Class";
			// 
			// parentClass
			// 
			this.parentClass.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.parentClass.Location = new System.Drawing.Point(272, 48);
			this.parentClass.Name = "parentClass";
			this.parentClass.Size = new System.Drawing.Size(192, 21);
			this.parentClass.TabIndex = 9;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(176, 48);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 24);
			this.label3.TabIndex = 8;
			this.label3.Text = "Parent:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.joinedSubKeyColumn);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.isJoinedSubClass);
			this.groupBox2.Location = new System.Drawing.Point(336, 104);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(288, 168);
			this.groupBox2.TabIndex = 4;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Joined-subclass";
			// 
			// joinedSubKeyColumn
			// 
			this.joinedSubKeyColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.joinedSubKeyColumn.Enabled = false;
			this.joinedSubKeyColumn.Location = new System.Drawing.Point(128, 64);
			this.joinedSubKeyColumn.Name = "joinedSubKeyColumn";
			this.joinedSubKeyColumn.Size = new System.Drawing.Size(136, 21);
			this.joinedSubKeyColumn.TabIndex = 7;
			// 
			// label5
			// 
			this.label5.Enabled = false;
			this.label5.Location = new System.Drawing.Point(32, 64);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 24);
			this.label5.TabIndex = 6;
			this.label5.Text = "Key column:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// isJoinedSubClass
			// 
			this.isJoinedSubClass.Location = new System.Drawing.Point(72, 24);
			this.isJoinedSubClass.Name = "isJoinedSubClass";
			this.isJoinedSubClass.Size = new System.Drawing.Size(152, 24);
			this.isJoinedSubClass.TabIndex = 4;
			this.isJoinedSubClass.Text = "Is Joined subclass";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.discValue);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.discColumn);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.useDiscriminator);
			this.groupBox1.Location = new System.Drawing.Point(24, 104);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(288, 168);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Table hierarchy";
			// 
			// discValue
			// 
			this.discValue.Location = new System.Drawing.Point(120, 96);
			this.discValue.Name = "discValue";
			this.discValue.Size = new System.Drawing.Size(136, 21);
			this.discValue.TabIndex = 9;
			this.discValue.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(24, 96);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 24);
			this.label4.TabIndex = 8;
			this.label4.Text = "Disc. Value:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// discColumn
			// 
			this.discColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.discColumn.Enabled = false;
			this.discColumn.Location = new System.Drawing.Point(120, 64);
			this.discColumn.Name = "discColumn";
			this.discColumn.Size = new System.Drawing.Size(136, 21);
			this.discColumn.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.Enabled = false;
			this.label2.Location = new System.Drawing.Point(24, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 24);
			this.label2.TabIndex = 4;
			this.label2.Text = "Discriminator:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// useDiscriminator
			// 
			this.useDiscriminator.Location = new System.Drawing.Point(56, 24);
			this.useDiscriminator.Name = "useDiscriminator";
			this.useDiscriminator.Size = new System.Drawing.Size(152, 24);
			this.useDiscriminator.TabIndex = 3;
			this.useDiscriminator.Text = "Use discriminator column";
			this.useDiscriminator.CheckedChanged += new System.EventHandler(this.useDiscriminator_CheckedChanged);
			// 
			// className
			// 
			this.className.Location = new System.Drawing.Point(272, 16);
			this.className.Name = "className";
			this.className.Size = new System.Drawing.Size(192, 21);
			this.className.TabIndex = 1;
			this.className.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(176, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 24);
			this.label1.TabIndex = 0;
			this.label1.Text = "Class name:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.listView1);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(648, 294);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Columns";
			// 
			// listView1
			// 
			this.listView1.CheckBoxes = true;
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader1,
																						this.columnHeader2,
																						this.columnHeader3,
																						this.columnHeader4,
																						this.columnHeader13});
			this.listView1.FullRowSelect = true;
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView1.LabelEdit = true;
			this.listView1.Location = new System.Drawing.Point(36, 27);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(576, 213);
			this.listView1.TabIndex = 2;
			this.listView1.View = System.Windows.Forms.View.Details;
			this.listView1.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listView1_AfterLabelEdit);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Property";
			this.columnHeader1.Width = 170;
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
			// columnHeader13
			// 
			this.columnHeader13.Text = "PK";
			this.columnHeader13.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.EditRelation);
			this.tabPage2.Controls.Add(this.AddRelation);
			this.tabPage2.Controls.Add(this.listView2);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(648, 294);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Relationships";
			// 
			// EditRelation
			// 
			this.EditRelation.Enabled = false;
			this.EditRelation.Location = new System.Drawing.Point(392, 256);
			this.EditRelation.Name = "EditRelation";
			this.EditRelation.Size = new System.Drawing.Size(104, 23);
			this.EditRelation.TabIndex = 6;
			this.EditRelation.Text = "Edit Relation";
			this.EditRelation.Click += new System.EventHandler(this.EditRelation_Click);
			// 
			// AddRelation
			// 
			this.AddRelation.Location = new System.Drawing.Point(504, 256);
			this.AddRelation.Name = "AddRelation";
			this.AddRelation.Size = new System.Drawing.Size(104, 23);
			this.AddRelation.TabIndex = 5;
			this.AddRelation.Text = "Add Relation";
			this.AddRelation.Click += new System.EventHandler(this.AddRelation_Click);
			// 
			// listView2
			// 
			this.listView2.CheckBoxes = true;
			this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader9,
																						this.columnHeader10,
																						this.columnHeader11,
																						this.columnHeader12});
			this.listView2.FullRowSelect = true;
			this.listView2.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView2.LabelEdit = true;
			this.listView2.Location = new System.Drawing.Point(36, 27);
			this.listView2.Name = "listView2";
			this.listView2.Size = new System.Drawing.Size(576, 213);
			this.listView2.TabIndex = 4;
			this.listView2.View = System.Windows.Forms.View.Details;
			this.listView2.AfterLabelEdit += new System.Windows.Forms.LabelEditEventHandler(this.listView2_AfterLabelEdit);
			// 
			// columnHeader9
			// 
			this.columnHeader9.Text = "Property";
			this.columnHeader9.Width = 190;
			// 
			// columnHeader10
			// 
			this.columnHeader10.Text = "Type";
			this.columnHeader10.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.columnHeader10.Width = 90;
			// 
			// columnHeader11
			// 
			this.columnHeader11.Text = "Association";
			this.columnHeader11.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.columnHeader11.Width = 120;
			// 
			// columnHeader12
			// 
			this.columnHeader12.Text = "Column";
			this.columnHeader12.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.columnHeader12.Width = 160;
			// 
			// columnHeader5
			// 
			this.columnHeader5.Text = "Property";
			this.columnHeader5.Width = 190;
			// 
			// columnHeader6
			// 
			this.columnHeader6.Text = "Type";
			this.columnHeader6.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.columnHeader6.Width = 90;
			// 
			// columnHeader7
			// 
			this.columnHeader7.Text = "Association";
			this.columnHeader7.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.columnHeader7.Width = 120;
			// 
			// columnHeader8
			// 
			this.columnHeader8.Text = "Column";
			this.columnHeader8.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.columnHeader8.Width = 160;
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(592, 376);
			this.okButton.Name = "okButton";
			this.okButton.TabIndex = 3;
			this.okButton.Text = "OK";
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(504, 376);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.TabIndex = 4;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// ActiveRecordPropertiesDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(686, 408);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.title);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ActiveRecordPropertiesDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Properties";
			this.tabControl1.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void useDiscriminator_CheckedChanged(object sender, System.EventArgs e)
		{
			label2.Enabled = discColumn.Enabled = useDiscriminator.Checked;
		}

		public String Title
		{
			get { return title.Text; }
			set { title.Text = value; }
		}

		private void FillClassProperties(ActiveRecordDescriptor descriptor)
		{
			listView1.Items.Clear();

			IList added = new ArrayList();

			foreach(ActiveRecordPropertyDescriptor prop in descriptor.Properties)
			{
				ListViewItem item = listView1.Items.Add(prop.PropertyName);
				item.Tag = prop;
				item.Checked = prop.Generate;
				item.SubItems.Add( prop.PropertyType.ToString() );
				item.SubItems.Add( prop.ColumnName );
				item.SubItems.Add( prop.ColumnTypeName );
				item.SubItems.Add( (prop is ActiveRecordPrimaryKeyDescriptor) ? "Yes" : "" );

				added.Add(prop);
			}

			if (descriptor is ActiveRecordDescriptorSubClass)
			{
				// This code might look strange, but we're
				// only showing the fields on the parent class that haven't been
				// marked for generation, so the user might have the
				// oportunity to add them

				foreach(ActiveRecordPropertyDescriptor prop in _parent.Properties)
				{
					if (prop.Generate || added.Contains(prop)) continue;

					ListViewItem item = listView1.Items.Add(prop.PropertyName);
					item.Tag = prop;
					item.Checked = prop.Generate;
					item.SubItems.Add( prop.PropertyType.ToString() );
					item.SubItems.Add( prop.ColumnName );
					item.SubItems.Add( prop.ColumnTypeName );
					item.SubItems.Add( (prop is ActiveRecordPrimaryKeyDescriptor) ? "Yes" : "" );
				}
			}
		}

		private void FillClassRelationships(ActiveRecordDescriptor descriptor)
		{
			listView2.Items.Clear();

			IList added = new ArrayList();

			foreach(ActiveRecordPropertyRelationDescriptor prop in descriptor.PropertiesRelations)
			{
				ListViewItem item = listView2.Items.Add(prop.PropertyName);
				item.Tag = prop;
				item.Checked = prop.Generate;

				if (prop.TargetType == null)
				{
					throw new ApplicationException("Information missing");
				}

				item.SubItems.Add( prop.TargetType.ClassName );
				item.SubItems.Add( prop.RelationType );
				item.SubItems.Add( prop.ColumnName );

				added.Add(prop);
			}

			if (descriptor is ActiveRecordDescriptorSubClass)
			{
				// This code might look strange, but we're
				// only showing the fields on the parent class that haven't been
				// marked for generation, so the user might have the
				// oportunity to add them

				foreach(ActiveRecordPropertyRelationDescriptor prop in _parent.PropertiesRelations)
				{
					if (prop.Generate || added.Contains(prop)) continue;

					ListViewItem item = listView2.Items.Add(prop.PropertyName);
					item.Tag = prop;
					item.Checked = prop.Generate;

					if (prop.TargetType == null)
					{
						throw new ApplicationException("Information missing");
					}

					item.SubItems.Add( prop.TargetType.ClassName );
					item.SubItems.Add( prop.RelationType );
					item.SubItems.Add( prop.ColumnName );
				}
			}
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			if (className.Text.Length == 0 || className.Text.IndexOf(' ') != -1)
			{
				MessageBox.Show("Invalid class name.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			// What's common can be updated without any problems:

			SaveCommonFields();

			// Updating properties

			SaveClassProperties();

			// Updating relations

			SaveClassRelationships();

			// Now here, semantic matters

			DialogResult = DialogResult.OK;

			Close();
		}

		private void AddRelation_Click(object sender, System.EventArgs e)
		{
			using(AddRelationDialog addRelationDlg = new AddRelationDialog(_descriptor, _project))
			{
				if (addRelationDlg.ShowDialog( this ) == DialogResult.OK)
				{
					FillClassRelationships( _descriptor );
				}
			}
		}

		private void SaveClassRelationships()
		{
			_descriptor.PropertiesRelations.Clear();

			foreach(ListViewItem item in listView2.Items)
			{
				ActiveRecordPropertyDescriptor prop = item.Tag as ActiveRecordPropertyDescriptor;

				if (_parent != null && _parent.PropertiesRelations.Contains(prop))
				{
					// This is important as modification here
					// wont affect the property descriptor on the parent
					prop = (ActiveRecordPropertyDescriptor) prop.Clone();
				}

				_descriptor.PropertiesRelations.Add( prop );

				prop.Generate = item.Checked;
			}
		}

		private void SaveClassProperties()
		{
			_descriptor.Properties.Clear();

			foreach(ListViewItem item in listView1.Items)
			{
				ActiveRecordPropertyDescriptor prop = item.Tag as ActiveRecordPropertyDescriptor;

				if (_parent != null && _parent.Properties.Contains(prop))
				{
					// This is important as modification here
					// wont affect the property descriptor on the parent
					prop = (ActiveRecordPropertyDescriptor) prop.Clone();
				}

				_descriptor.Properties.Add( prop );

				prop.Generate = item.Checked;
			}
		}

		private void SaveCommonFields()
		{
			if (className.Text.Length != 0)
			{
				_descriptor.ClassName = className.Text;
			}
	
			if (discColumn.SelectedIndex == -1)
			{
				_descriptor.DiscriminatorField = null;
			}
			else
			{
				_descriptor.DiscriminatorField = (discColumn.SelectedItem as ColumnDefinition).Name;
			}
	
			if (discValue.Text.Length != 0)
			{
				_descriptor.DiscriminatorValue = discValue.Text;
			}
			else
			{
				_descriptor.DiscriminatorValue = null;
			}
		}

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

		private void listView2_AfterLabelEdit(object sender, System.Windows.Forms.LabelEditEventArgs e)
		{
			if (e.Label == null || e.Label.Length == 0)
			{
				e.CancelEdit = true;
			}
			else
			{
				ActiveRecordPropertyDescriptor desc = listView2.Items[e.Item].Tag as ActiveRecordPropertyDescriptor;			
				desc.PropertyName = e.Label;
			}
		}

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void EditRelation_Click(object sender, System.EventArgs e)
		{
			if (listView2.SelectedItems.Count != 0)
			{
				ActiveRecordPropertyRelationDescriptor prop = listView2.SelectedItems[0].Tag as ActiveRecordPropertyRelationDescriptor;

				using(EditRelationDialog editDlg = new EditRelationDialog(_descriptor, _project, prop))
				{
					if (editDlg.ShowDialog( this ) == DialogResult.OK)
					{
						_descriptor.PropertiesRelations.Remove(prop);

						FillClassRelationships( _descriptor );
					}
				}
			}
			else
			{
				MessageBox.Show(this.Owner, "Select an existing relationship first.");
			}
		}
	}
}
