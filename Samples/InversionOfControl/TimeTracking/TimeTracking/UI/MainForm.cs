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
	using System.Windows.Forms;
	using Castle.MicroKernel;
	using TimeTracking.Models;

	public class MainForm : Form
	{
		private readonly IKernel kernel;
		private readonly TimeRecordService timeRecordService;
		private readonly TaskService taskService;
		private MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.Button btManageTask;
		private System.Windows.Forms.ComboBox cmbTasks;
		private System.Windows.Forms.Button btStart;
		private System.Windows.Forms.Button btPause;
		private System.Windows.Forms.Button btStop;
		private System.Windows.Forms.PictureBox curStateBox;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ColumnHeader columnHeader3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MainForm()
		{
			InitializeComponent();
		}
		
		/// <summary>
		/// We have to keep the parameterless constructor
		/// or else VS.Net designer will complain
		/// </summary>
		/// <param name="kernel">Kernel instance</param>
		public MainForm(TimeRecordService timeRecordService, TaskService taskService, IKernel kernel) : this()
		{
			this.timeRecordService = timeRecordService;
			this.taskService = taskService;
			this.kernel = kernel;
			
			UpdateButtonsState();
		}

		private void UpdateButtonsState()
		{
			UpdateEventList();
			
			btStart.Enabled = timeRecordService.CanStartClock;
			btStop.Enabled = timeRecordService.CanStopClock;
			btPause.Enabled = timeRecordService.CanPauseClock || timeRecordService.CanResumeClock;
			
			btPause.Text = timeRecordService.CanResumeClock ? "Resume" : "Pause";
			
			switch(timeRecordService.CurrentClockState)
			{
				case EventType.Started:
					curStateBox.BackColor = Color.Green;
					break;
				case EventType.Stopped:
					curStateBox.BackColor = Color.Red;
					break;
				case EventType.Paused:
					curStateBox.BackColor = Color.Yellow;
					break;
				default:
					curStateBox.BackColor = Color.Gray;
					break;
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.btManageTask = new System.Windows.Forms.Button();
			this.cmbTasks = new System.Windows.Forms.ComboBox();
			this.btStart = new System.Windows.Forms.Button();
			this.btPause = new System.Windows.Forms.Button();
			this.btStop = new System.Windows.Forms.Button();
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.curStateBox = new System.Windows.Forms.PictureBox();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1,
																					  this.menuItem3,
																					  this.menuItem6});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem2});
			this.menuItem1.Text = "Manage";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.Text = "Add tasks";
			this.menuItem2.Click += new System.EventHandler(this.menuItem2_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 1;
			this.menuItem3.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem4,
																					  this.menuItem5});
			this.menuItem3.Text = "Time";
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 0;
			this.menuItem4.Text = "Start Clock";
			this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 1;
			this.menuItem5.Text = "Stop Clock";
			this.menuItem5.Click += new System.EventHandler(this.menuItem5_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 2;
			this.menuItem6.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem7});
			this.menuItem6.Text = "Help";
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 0;
			this.menuItem7.Text = "About";
			this.menuItem7.Click += new System.EventHandler(this.menuItem7_Click);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(16, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(240, 24);
			this.label1.TabIndex = 0;
			this.label1.Text = "1. Add the tasks";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(16, 104);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(240, 24);
			this.label2.TabIndex = 1;
			this.label2.Text = "2. Select the task";
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label3.Location = new System.Drawing.Point(16, 184);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(240, 24);
			this.label3.TabIndex = 2;
			this.label3.Text = "3. Start tracking";
			// 
			// btManageTask
			// 
			this.btManageTask.Location = new System.Drawing.Point(24, 48);
			this.btManageTask.Name = "btManageTask";
			this.btManageTask.Size = new System.Drawing.Size(216, 32);
			this.btManageTask.TabIndex = 3;
			this.btManageTask.Text = "Manage tasks...";
			this.btManageTask.Click += new System.EventHandler(this.btManageTask_Click);
			// 
			// cmbTasks
			// 
			this.cmbTasks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.cmbTasks.Location = new System.Drawing.Point(24, 136);
			this.cmbTasks.Name = "cmbTasks";
			this.cmbTasks.Size = new System.Drawing.Size(224, 21);
			this.cmbTasks.TabIndex = 4;
			// 
			// btStart
			// 
			this.btStart.Location = new System.Drawing.Point(152, 224);
			this.btStart.Name = "btStart";
			this.btStart.Size = new System.Drawing.Size(93, 32);
			this.btStart.TabIndex = 5;
			this.btStart.Text = "Start";
			this.btStart.Click += new System.EventHandler(this.btStart_Click);
			// 
			// btPause
			// 
			this.btPause.Location = new System.Drawing.Point(152, 272);
			this.btPause.Name = "btPause";
			this.btPause.Size = new System.Drawing.Size(93, 32);
			this.btPause.TabIndex = 6;
			this.btPause.Text = "Pause";
			this.btPause.Click += new System.EventHandler(this.btPause_Click);
			// 
			// btStop
			// 
			this.btStop.Location = new System.Drawing.Point(152, 320);
			this.btStop.Name = "btStop";
			this.btStop.Size = new System.Drawing.Size(93, 32);
			this.btStop.TabIndex = 7;
			this.btStop.Text = "Stop";
			this.btStop.Click += new System.EventHandler(this.btStop_Click);
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 473);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Size = new System.Drawing.Size(272, 22);
			this.statusBar1.TabIndex = 8;
			this.statusBar1.Text = "Ready";
			// 
			// curStateBox
			// 
			this.curStateBox.BackColor = System.Drawing.Color.Silver;
			this.curStateBox.Location = new System.Drawing.Point(24, 232);
			this.curStateBox.Name = "curStateBox";
			this.curStateBox.Size = new System.Drawing.Size(104, 104);
			this.curStateBox.TabIndex = 9;
			this.curStateBox.TabStop = false;
			// 
			// listView1
			// 
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader1,
																						this.columnHeader2,
																						this.columnHeader3});
			this.listView1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.listView1.GridLines = true;
			this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
			this.listView1.Location = new System.Drawing.Point(0, 369);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(272, 104);
			this.listView1.TabIndex = 10;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Task";
			this.columnHeader1.Width = 70;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Event";
			this.columnHeader2.Width = 70;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "At";
			this.columnHeader3.Width = 120;
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(272, 495);
			this.Controls.Add(this.listView1);
			this.Controls.Add(this.curStateBox);
			this.Controls.Add(this.statusBar1);
			this.Controls.Add(this.btStop);
			this.Controls.Add(this.btPause);
			this.Controls.Add(this.btStart);
			this.Controls.Add(this.cmbTasks);
			this.Controls.Add(this.btManageTask);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.Menu = this.mainMenu1;
			this.Name = "MainForm";
			this.Text = "Track!";
			this.ResumeLayout(false);

		}
		#endregion

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

		private void btManageTask_Click(object sender, System.EventArgs e)
		{
			TaskManagement form = kernel[typeof(TaskManagement)] as TaskManagement;
			
			form.ShowDialog(this);
			
			kernel.ReleaseComponent(form);
			
			UpdateTaskList();
		}

		private void UpdateTaskList()
		{
			cmbTasks.Items.Clear();
			
			Task[] tasks = taskService.FindAll();
			
			foreach(Task task in tasks)
			{
				cmbTasks.Items.Add(task);
			}
		}

		private void btStart_Click(object sender, System.EventArgs e)
		{
			try
			{
				Task task = (Task) cmbTasks.SelectedItem;
			
				timeRecordService.StartClock(task);
				
				UpdateButtonsState();
			}
			catch(Exception ex)
			{
				MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void btPause_Click(object sender, System.EventArgs e)
		{
			if (timeRecordService.CurrentClockState == EventType.Paused)
			{
				timeRecordService.ResumeClock();
			}
			else
			{
				timeRecordService.PauseClock();
			}
			
			UpdateButtonsState();
		}

		private void btStop_Click(object sender, System.EventArgs e)
		{
			timeRecordService.StopClock();
			
			UpdateButtonsState();
		}

		private void UpdateEventList()
		{
			listView1.Items.Clear();
			
			TimeRecord record = timeRecordService.CurrentRecord;
			
			if (record == null) return;
			
			foreach(TimeRecordEvent ev in record.TimeRecordEvents)
			{
				ListViewItem item = listView1.Items.Add(ev.ParentRecord.Task.Name);
				item.SubItems.Add(ev.EventType.ToString());
				item.SubItems.Add(ev.Time.ToShortTimeString());
			}
		}

		private void menuItem2_Click(object sender, System.EventArgs e)
		{
			btManageTask_Click(sender, e);
		}

		private void menuItem4_Click(object sender, System.EventArgs e)
		{
			btStart_Click(sender, e);
		}

		private void menuItem5_Click(object sender, System.EventArgs e)
		{
			btStop_Click(sender, e);
		}

		private void menuItem7_Click(object sender, System.EventArgs e)
		{
			MessageBox.Show(this, "Castle MicroKernel/Windsor Sample");
		}
	}
}
