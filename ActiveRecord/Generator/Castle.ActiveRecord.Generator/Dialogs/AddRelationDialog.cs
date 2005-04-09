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
	/// Summary description for AddRelationDialog.
	/// </summary>
	public class AddRelationDialog : System.Windows.Forms.Form
	{
		protected System.Windows.Forms.Button cancelButton;
		protected System.Windows.Forms.Button okButton;
		protected System.Windows.Forms.TabControl tabControl1;
		protected System.Windows.Forms.TabPage tabPage1;
		protected System.Windows.Forms.TextBox className;
		protected System.Windows.Forms.TabPage tabPage2;
		protected System.Windows.Forms.TabPage tabPage3;
		protected System.Windows.Forms.ListBox parentCols;
		protected System.Windows.Forms.ListBox relatedCols;
		protected System.Windows.Forms.ComboBox associationTable;
		protected System.Windows.Forms.GroupBox groupBox1;
		protected System.Windows.Forms.GroupBox groupBox2;
		protected System.Windows.Forms.Label label4;
		protected System.Windows.Forms.Label label5;
		protected System.Windows.Forms.ListBox targetTableList;
		protected System.Windows.Forms.RadioButton hasAndBelongsToManyButton;
		protected System.Windows.Forms.RadioButton belongsToButton;
		protected System.Windows.Forms.RadioButton hasManyButton;
		protected System.Windows.Forms.TextBox where;
		protected System.Windows.Forms.TextBox order;
		protected System.Windows.Forms.CheckBox lazyButton;
		protected System.Windows.Forms.CheckBox inverseButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		protected System.ComponentModel.Container components = null;
		protected System.Windows.Forms.Label label10;
		protected System.Windows.Forms.Label label11;

		protected ActiveRecordDescriptor _descriptor;
		protected Project _project;
		protected AssociationEnum _association = AssociationEnum.Undefined;
		protected System.Windows.Forms.Label associationTableLabel;
		protected System.Windows.Forms.Label relatedColsLabel;
		protected System.Windows.Forms.Label parentColsLabel;
		protected System.Windows.Forms.ComboBox outerJoin;
		protected System.Windows.Forms.Label label9;
		protected System.Windows.Forms.ComboBox cascade;
		protected System.Windows.Forms.Label label1;
		protected System.Windows.Forms.CheckBox updateButton;
		protected System.Windows.Forms.CheckBox insertButton;
		protected ActiveRecordDescriptor _oldDescSelection;
		protected IRelationshipBuilder _relationBuilder;


		public AddRelationDialog(ActiveRecordDescriptor descriptor, Project project) : this()
		{
			_descriptor = descriptor;
			_project = project;

			_relationBuilder = (IRelationshipBuilder) ServiceRegistry.Instance[ typeof(IRelationshipBuilder) ];

			className.Text = _descriptor.ClassName;

			targetTableList.ValueMember = "ClassName";

			foreach(IActiveRecordDescriptor desc in _project.Descriptors)
			{
				if (desc is ActiveRecordBaseDescriptor) continue;
				if (desc.ClassName == null) continue;

				targetTableList.Items.Add( desc );
			}
		}

		public AddRelationDialog()
		{
			InitializeComponent();

			outerJoin.SelectedIndex = 0;
			cascade.SelectedIndex = 0;
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
			this.cancelButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.targetTableList = new System.Windows.Forms.ListBox();
			this.className = new System.Windows.Forms.TextBox();
			this.hasAndBelongsToManyButton = new System.Windows.Forms.RadioButton();
			this.belongsToButton = new System.Windows.Forms.RadioButton();
			this.hasManyButton = new System.Windows.Forms.RadioButton();
			this.tabPage3 = new System.Windows.Forms.TabPage();
			this.associationTableLabel = new System.Windows.Forms.Label();
			this.associationTable = new System.Windows.Forms.ComboBox();
			this.relatedCols = new System.Windows.Forms.ListBox();
			this.parentCols = new System.Windows.Forms.ListBox();
			this.relatedColsLabel = new System.Windows.Forms.Label();
			this.parentColsLabel = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.cascade = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.updateButton = new System.Windows.Forms.CheckBox();
			this.insertButton = new System.Windows.Forms.CheckBox();
			this.inverseButton = new System.Windows.Forms.CheckBox();
			this.lazyButton = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.outerJoin = new System.Windows.Forms.ComboBox();
			this.label9 = new System.Windows.Forms.Label();
			this.order = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.where = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(448, 296);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.TabIndex = 6;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// okButton
			// 
			this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.okButton.Location = new System.Drawing.Point(536, 296);
			this.okButton.Name = "okButton";
			this.okButton.TabIndex = 5;
			this.okButton.Text = "OK";
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage3);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(16, 16);
			this.tabControl1.Multiline = true;
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(600, 272);
			this.tabControl1.TabIndex = 9;
			// 
			// tabPage1
			// 
			this.tabPage1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tabPage1.Controls.Add(this.label11);
			this.tabPage1.Controls.Add(this.label10);
			this.tabPage1.Controls.Add(this.targetTableList);
			this.tabPage1.Controls.Add(this.className);
			this.tabPage1.Controls.Add(this.hasAndBelongsToManyButton);
			this.tabPage1.Controls.Add(this.belongsToButton);
			this.tabPage1.Controls.Add(this.hasManyButton);
			this.tabPage1.Location = new System.Drawing.Point(4, 25);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(592, 243);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Association";
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(24, 72);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(120, 16);
			this.label11.TabIndex = 11;
			this.label11.Text = "ActiveRecord class:";
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(400, 24);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(120, 16);
			this.label10.TabIndex = 10;
			this.label10.Text = "ActiveRecord classes:";
			// 
			// targetTableList
			// 
			this.targetTableList.Location = new System.Drawing.Point(400, 40);
			this.targetTableList.Name = "targetTableList";
			this.targetTableList.Size = new System.Drawing.Size(168, 160);
			this.targetTableList.TabIndex = 9;
			this.targetTableList.SelectedIndexChanged += new System.EventHandler(this.CheckRelationDeclaration);
			// 
			// className
			// 
			this.className.Location = new System.Drawing.Point(24, 88);
			this.className.Name = "className";
			this.className.ReadOnly = true;
			this.className.Size = new System.Drawing.Size(168, 21);
			this.className.TabIndex = 8;
			this.className.Text = "Firm";
			// 
			// hasAndBelongsToManyButton
			// 
			this.hasAndBelongsToManyButton.Location = new System.Drawing.Point(224, 112);
			this.hasAndBelongsToManyButton.Name = "hasAndBelongsToManyButton";
			this.hasAndBelongsToManyButton.Size = new System.Drawing.Size(152, 24);
			this.hasAndBelongsToManyButton.TabIndex = 7;
			this.hasAndBelongsToManyButton.Text = "Has and Belongs to Many";
			this.hasAndBelongsToManyButton.CheckedChanged += new System.EventHandler(this.CheckRelationDeclaration);
			// 
			// belongsToButton
			// 
			this.belongsToButton.Location = new System.Drawing.Point(224, 88);
			this.belongsToButton.Name = "belongsToButton";
			this.belongsToButton.TabIndex = 6;
			this.belongsToButton.Text = "Belongs to";
			this.belongsToButton.CheckedChanged += new System.EventHandler(this.CheckRelationDeclaration);
			// 
			// hasManyButton
			// 
			this.hasManyButton.Location = new System.Drawing.Point(224, 64);
			this.hasManyButton.Name = "hasManyButton";
			this.hasManyButton.TabIndex = 5;
			this.hasManyButton.Text = "Has Many";
			this.hasManyButton.CheckedChanged += new System.EventHandler(this.CheckRelationDeclaration);
			// 
			// tabPage3
			// 
			this.tabPage3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tabPage3.Controls.Add(this.associationTableLabel);
			this.tabPage3.Controls.Add(this.associationTable);
			this.tabPage3.Controls.Add(this.relatedCols);
			this.tabPage3.Controls.Add(this.parentCols);
			this.tabPage3.Controls.Add(this.relatedColsLabel);
			this.tabPage3.Controls.Add(this.parentColsLabel);
			this.tabPage3.Location = new System.Drawing.Point(4, 25);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(592, 243);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Columns";
			// 
			// associationTableLabel
			// 
			this.associationTableLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.associationTableLabel.Location = new System.Drawing.Point(208, 8);
			this.associationTableLabel.Name = "associationTableLabel";
			this.associationTableLabel.Size = new System.Drawing.Size(112, 16);
			this.associationTableLabel.TabIndex = 5;
			this.associationTableLabel.Text = "Association table:";
			// 
			// associationTable
			// 
			this.associationTable.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.associationTable.Location = new System.Drawing.Point(208, 24);
			this.associationTable.Name = "associationTable";
			this.associationTable.Size = new System.Drawing.Size(160, 21);
			this.associationTable.TabIndex = 4;
			this.associationTable.SelectedIndexChanged += new System.EventHandler(this.associationTable_SelectedIndexChanged);
			// 
			// relatedCols
			// 
			this.relatedCols.Location = new System.Drawing.Point(360, 88);
			this.relatedCols.Name = "relatedCols";
			this.relatedCols.Size = new System.Drawing.Size(192, 121);
			this.relatedCols.TabIndex = 3;
			// 
			// parentCols
			// 
			this.parentCols.Location = new System.Drawing.Point(24, 88);
			this.parentCols.Name = "parentCols";
			this.parentCols.Size = new System.Drawing.Size(192, 121);
			this.parentCols.TabIndex = 2;
			// 
			// relatedColsLabel
			// 
			this.relatedColsLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.relatedColsLabel.Location = new System.Drawing.Point(360, 72);
			this.relatedColsLabel.Name = "relatedColsLabel";
			this.relatedColsLabel.Size = new System.Drawing.Size(100, 16);
			this.relatedColsLabel.TabIndex = 1;
			this.relatedColsLabel.Text = "Foreign Key:";
			// 
			// parentColsLabel
			// 
			this.parentColsLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.parentColsLabel.Location = new System.Drawing.Point(24, 72);
			this.parentColsLabel.Name = "parentColsLabel";
			this.parentColsLabel.Size = new System.Drawing.Size(100, 16);
			this.parentColsLabel.TabIndex = 0;
			this.parentColsLabel.Text = "Parent column:";
			// 
			// tabPage2
			// 
			this.tabPage2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.tabPage2.Controls.Add(this.groupBox2);
			this.tabPage2.Controls.Add(this.groupBox1);
			this.tabPage2.Location = new System.Drawing.Point(4, 25);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(592, 243);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Customizations";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.cascade);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.updateButton);
			this.groupBox2.Controls.Add(this.insertButton);
			this.groupBox2.Controls.Add(this.inverseButton);
			this.groupBox2.Controls.Add(this.lazyButton);
			this.groupBox2.Location = new System.Drawing.Point(24, 136);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(544, 88);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Behavior and tuning";
			// 
			// cascade
			// 
			this.cascade.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cascade.Items.AddRange(new object[] {
														 "none",
														 "all",
														 "save-update",
														 "delete"});
			this.cascade.Location = new System.Drawing.Point(320, 42);
			this.cascade.Name = "cascade";
			this.cascade.Size = new System.Drawing.Size(184, 21);
			this.cascade.TabIndex = 19;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(320, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 18;
			this.label1.Text = "Cascade:";
			// 
			// updateButton
			// 
			this.updateButton.Checked = true;
			this.updateButton.CheckState = System.Windows.Forms.CheckState.Checked;
			this.updateButton.Location = new System.Drawing.Point(48, 56);
			this.updateButton.Name = "updateButton";
			this.updateButton.Size = new System.Drawing.Size(104, 16);
			this.updateButton.TabIndex = 17;
			this.updateButton.Text = "Update";
			// 
			// insertButton
			// 
			this.insertButton.Checked = true;
			this.insertButton.CheckState = System.Windows.Forms.CheckState.Checked;
			this.insertButton.Location = new System.Drawing.Point(48, 24);
			this.insertButton.Name = "insertButton";
			this.insertButton.Size = new System.Drawing.Size(104, 16);
			this.insertButton.TabIndex = 16;
			this.insertButton.Text = "Insert";
			// 
			// inverseButton
			// 
			this.inverseButton.Location = new System.Drawing.Point(208, 24);
			this.inverseButton.Name = "inverseButton";
			this.inverseButton.Size = new System.Drawing.Size(64, 16);
			this.inverseButton.TabIndex = 1;
			this.inverseButton.Text = "Inverse";
			// 
			// lazyButton
			// 
			this.lazyButton.Location = new System.Drawing.Point(208, 56);
			this.lazyButton.Name = "lazyButton";
			this.lazyButton.Size = new System.Drawing.Size(96, 16);
			this.lazyButton.TabIndex = 0;
			this.lazyButton.Text = "Lazy";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.outerJoin);
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Controls.Add(this.order);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.where);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Location = new System.Drawing.Point(24, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(544, 112);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Queries";
			// 
			// outerJoin
			// 
			this.outerJoin.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.outerJoin.Items.AddRange(new object[] {
														   "auto",
														   "true",
														   "false"});
			this.outerJoin.Location = new System.Drawing.Point(16, 80);
			this.outerJoin.Name = "outerJoin";
			this.outerJoin.Size = new System.Drawing.Size(224, 21);
			this.outerJoin.TabIndex = 17;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(16, 64);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(100, 16);
			this.label9.TabIndex = 16;
			this.label9.Text = "Outer join:";
			// 
			// order
			// 
			this.order.Location = new System.Drawing.Point(288, 40);
			this.order.Name = "order";
			this.order.Size = new System.Drawing.Size(224, 21);
			this.order.TabIndex = 3;
			this.order.Text = "";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(288, 24);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 16);
			this.label5.TabIndex = 2;
			this.label5.Text = "Order:";
			// 
			// where
			// 
			this.where.Location = new System.Drawing.Point(16, 40);
			this.where.Name = "where";
			this.where.Size = new System.Drawing.Size(224, 21);
			this.where.TabIndex = 1;
			this.where.Text = "";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(16, 24);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 16);
			this.label4.TabIndex = 0;
			this.label4.Text = "Where:";
			// 
			// AddRelationDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(632, 330);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "AddRelationDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Add New Relationship";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void CheckRelationDeclaration(object sender, System.EventArgs e)
		{
			CheckRelationDeclaration();
		}

		private void CheckRelationDeclaration()
		{
			tabPage2.Enabled = tabPage3.Enabled = false;

			if (hasManyButton.Checked || belongsToButton.Checked || hasAndBelongsToManyButton.Checked)
			{
				if (SelectedTarget != null)
				{
					tabPage2.Enabled = tabPage3.Enabled = true;
				}
				else
				{
					return;
				}
			}

			if (hasManyButton.Checked)
			{
				SwitchViewTo(AssociationEnum.HasMany);
			}
			else if (hasAndBelongsToManyButton.Checked)
			{
				SwitchViewTo(AssociationEnum.HasAndBelongsToMany);
			}
			else if (belongsToButton.Checked)
			{
				SwitchViewTo(AssociationEnum.BelongsTo);
			}
		}

		protected ActiveRecordDescriptor SelectedTarget
		{
			get
			{
				if (targetTableList.SelectedIndex != -1)
				{
					return targetTableList.SelectedItem as ActiveRecordDescriptor;
				}
				return null;
			}
			set
			{
				targetTableList.SelectedItem = value;
			}
		}

		protected ColumnDefinition SelectedRelatedCol
		{
			get
			{
				if (relatedCols.SelectedIndex != -1)
				{
					return relatedCols.SelectedItem as ColumnDefinition;
				}
				return null;
			}
		}

		protected ColumnDefinition SelectedParentCol
		{
			get
			{
				if (parentCols.SelectedIndex != -1)
				{
					return parentCols.SelectedItem as ColumnDefinition;
				}
				return null;
			}
		}

		protected TableDefinition SelectedAssociationTable
		{
			get
			{
				if (associationTable.SelectedIndex != -1)
				{
					return associationTable.SelectedItem as TableDefinition;
				}
				return null;
			}
		}

		protected void SwitchViewTo(AssociationEnum association)
		{
			// Nothing's changed?
			if (_oldDescSelection == SelectedTarget && association == _association)
			{
				// Aparently not
				return;
			}

			// Saves 
			_oldDescSelection = SelectedTarget;
			_association = association;

			// Disabling
			associationTable.Enabled = associationTableLabel.Enabled = false;
			parentCols.Enabled = parentColsLabel.Enabled = false;
			relatedCols.Enabled = relatedColsLabel.Enabled = false;

			if (association == AssociationEnum.BelongsTo)
			{
				parentCols.Enabled = parentColsLabel.Enabled = true;

				PopulateColumnsInListBox( parentCols, _descriptor.Table);
			}
			else if (association == AssociationEnum.HasAndBelongsToMany)
			{
				associationTable.Enabled = associationTableLabel.Enabled = true;
				parentCols.Enabled = parentColsLabel.Enabled = true;
				relatedCols.Enabled = relatedColsLabel.Enabled = true;

				associationTable.SelectedIndex = -1;

				parentCols.Items.Clear();
				relatedCols.Items.Clear();

				PopulateTablesInComboBox(associationTable, _descriptor.Table.DatabaseDefinition);
			}
			else if (association == AssociationEnum.HasMany)
			{
				relatedCols.Enabled = relatedColsLabel.Enabled = true;

				PopulateColumnsInListBox( relatedCols, SelectedTarget.Table);
			}
		}

		private void PopulateColumnsInListBox(ListBox cols, TableDefinition table)
		{
			cols.Items.Clear();
			cols.ValueMember = "Name";

			foreach(ColumnDefinition col in table.Columns)
			{
				cols.Items.Add(col);
			}
		}

		private void PopulateTablesInComboBox(ComboBox combo, DatabaseDefinition dbDef)
		{
			combo.Items.Clear();
			combo.ValueMember = "Name";

			foreach(TableDefinition table in dbDef.Tables)
			{
				combo.Items.Add(table);
			}
		}

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			if (_association == AssociationEnum.Undefined)
			{
				cancelButton_Click(sender, e);
				return;
			}

			try
			{
				RelationshipInfo info = new RelationshipInfo(_association, _descriptor, SelectedTarget);
				
				info.ParentCol = SelectedParentCol;
				info.ChildCol = SelectedRelatedCol;
				info.AssociationTable = SelectedAssociationTable;

				info.Where = where.Text;
				info.OrderBy = order.Text;
				info.OuterJoin = outerJoin.Text;

				// TODO: Add checkbox for proxy
				info.UseProxy = false;

				info.Insert = insertButton.Checked;
				info.Update = updateButton.Checked;
				info.Lazy = lazyButton.Checked;
				info.Cascade = cascade.SelectedText;

				ActiveRecordPropertyRelationDescriptor prop = _relationBuilder.Build( info );

				_descriptor.PropertiesRelations.Add(prop);
			}
			catch(Exception ex)
			{
				String message = String.Format("Something is missing... \r\n\r\n{0}", ex.Message);
				MessageBox.Show(this, message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			DialogResult = DialogResult.OK;
		}

		private void associationTable_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (SelectedAssociationTable != null)
			{
				PopulateColumnsInListBox(parentCols, SelectedAssociationTable);
				PopulateColumnsInListBox(relatedCols, SelectedAssociationTable);
			}
		}
	}
}
