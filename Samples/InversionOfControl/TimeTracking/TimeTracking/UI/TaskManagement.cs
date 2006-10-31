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

namespace TimeTracking.UI
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;
	using TimeTracking.Models;

	/// <summary>
	/// Summary description for TaskManagement.
	/// </summary>
	public class TaskManagement : System.Windows.Forms.Form
	{
		private readonly TaskService taskService;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		private System.Windows.Forms.Button btDelete;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button btClose;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btInsert;
		private System.Windows.Forms.ListView taskList;
		private System.Windows.Forms.TextBox commentBox;
		private System.Windows.Forms.TextBox nameBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public TaskManagement()
		{
			InitializeComponent();
		}

		public TaskManagement(TaskService taskService) : this()
		{
			this.taskService = taskService;
			
			UpdateTaskList();
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
			this.taskList = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.label1 = new System.Windows.Forms.Label();
			this.btDelete = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btInsert = new System.Windows.Forms.Button();
			this.commentBox = new System.Windows.Forms.TextBox();
			this.nameBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.btClose = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// taskList
			// 
			this.taskList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																					   this.columnHeader1,
																					   this.columnHeader2,
																					   this.columnHeader3});
			this.taskList.FullRowSelect = true;
			this.taskList.GridLines = true;
			this.taskList.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.taskList.HideSelection = false;
			this.taskList.Location = new System.Drawing.Point(16, 184);
			this.taskList.Name = "taskList";
			this.taskList.Size = new System.Drawing.Size(528, 136);
			this.taskList.TabIndex = 0;
			this.taskList.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Id";
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Name";
			this.columnHeader2.Width = 180;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Comment";
			this.columnHeader3.Width = 280;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 168);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(120, 16);
			this.label1.TabIndex = 1;
			this.label1.Text = "Task list:";
			// 
			// btDelete
			// 
			this.btDelete.Enabled = false;
			this.btDelete.Location = new System.Drawing.Point(16, 328);
			this.btDelete.Name = "btDelete";
			this.btDelete.Size = new System.Drawing.Size(144, 24);
			this.btDelete.TabIndex = 2;
			this.btDelete.Text = "Delete selected task";
			this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btInsert);
			this.groupBox1.Controls.Add(this.commentBox);
			this.groupBox1.Controls.Add(this.nameBox);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(16, 16);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(528, 136);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "New Task Info:";
			// 
			// btInsert
			// 
			this.btInsert.Location = new System.Drawing.Point(424, 32);
			this.btInsert.Name = "btInsert";
			this.btInsert.Size = new System.Drawing.Size(88, 24);
			this.btInsert.TabIndex = 4;
			this.btInsert.Text = "Insert";
			this.btInsert.Click += new System.EventHandler(this.btInsert_Click);
			// 
			// commentBox
			// 
			this.commentBox.Location = new System.Drawing.Point(128, 72);
			this.commentBox.Multiline = true;
			this.commentBox.Name = "commentBox";
			this.commentBox.Size = new System.Drawing.Size(280, 48);
			this.commentBox.TabIndex = 3;
			this.commentBox.Text = "";
			// 
			// nameBox
			// 
			this.nameBox.Location = new System.Drawing.Point(128, 32);
			this.nameBox.Name = "nameBox";
			this.nameBox.Size = new System.Drawing.Size(280, 21);
			this.nameBox.TabIndex = 2;
			this.nameBox.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(24, 72);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(88, 24);
			this.label3.TabIndex = 1;
			this.label3.Text = "Comment:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(88, 24);
			this.label2.TabIndex = 0;
			this.label2.Text = "Name:";
			// 
			// panel1
			// 
			this.panel1.Location = new System.Drawing.Point(-32, 368);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(608, 2);
			this.panel1.TabIndex = 4;
			// 
			// btClose
			// 
			this.btClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btClose.Location = new System.Drawing.Point(424, 384);
			this.btClose.Name = "btClose";
			this.btClose.Size = new System.Drawing.Size(112, 24);
			this.btClose.TabIndex = 5;
			this.btClose.Text = "Close";
			this.btClose.Click += new System.EventHandler(this.btClose_Click);
			// 
			// TaskManagement
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.btClose;
			this.ClientSize = new System.Drawing.Size(560, 422);
			this.Controls.Add(this.btClose);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btDelete);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.taskList);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "TaskManagement";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "TaskManagement";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void btClose_Click(object sender, System.EventArgs e)
		{
			Hide();
		}

		private void btInsert_Click(object sender, System.EventArgs e)
		{
			try
			{
				taskService.Add(new Task(nameBox.Text, commentBox.Text));
				
				UpdateTaskList();
				
				nameBox.Text = commentBox.Text = String.Empty;
			}
			catch(Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void UpdateTaskList()
		{
			taskList.Items.Clear();
			
			foreach(Task task in taskService.FindAll())
			{
 				ListViewItem item = taskList.Items.Add(task.Id.ToString());
				item.SubItems.Add(task.Name);
				item.SubItems.Add(task.Comment);
			}
		}

		private void btDelete_Click(object sender, System.EventArgs e)
		{
			// Not implemented
		}
	}
}
