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

namespace Castle.ActiveRecord.Generator
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Data;
	using WeifenLuo.WinFormsUI;

	using Castle.ActiveRecord.Generator.Components;


	public class MainForm : Form, IWorkspace
	{
		private DockPanel dockManager;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.ImageList imageList2;
		private System.ComponentModel.IContainer components;

		public MainForm()
		{
			InitializeComponent();
		}

		public MainForm(Model model) : this()
		{
			model.OnProjectChanged += new ProjectDelegate(OnChanged);
			model.OnProjectReplaced += new ProjectReplaceDelegate(OnProjectReplaced);
		}

		#region IWorkspace Members

		public MainMenu MainMenu
		{
			get { return mainMenu1; }
		}

		public ToolBar MainToolBar
		{
			get { return toolBar1; }
		}

		public StatusBar MainStatusBar
		{
			get { return statusBar1; }
		}

		public DockPanel MainDockManager
		{
			get { return dockManager; }
		}

		public IWin32Window ActiveWindow
		{
			get { return this; }
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this.dockManager = new WeifenLuo.WinFormsUI.DockPanel();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.toolBar1 = new System.Windows.Forms.ToolBar();
			this.imageList2 = new System.Windows.Forms.ImageList(this.components);
			this.SuspendLayout();
			// 
			// dockManager
			// 
			this.dockManager.ActiveAutoHideContent = null;
			this.dockManager.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.dockManager.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dockManager.Font = new System.Drawing.Font("Tahoma", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.World, ((System.Byte)(0)));
			this.dockManager.Location = new System.Drawing.Point(0, 42);
			this.dockManager.MdiIntegration = false;
			this.dockManager.Name = "dockManager";
			this.dockManager.Size = new System.Drawing.Size(855, 609);
			this.dockManager.TabIndex = 0;
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 651);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Size = new System.Drawing.Size(855, 21);
			this.statusBar1.TabIndex = 0;
			this.statusBar1.Text = "Ready.";
			// 
			// toolBar1
			// 
			this.toolBar1.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this.toolBar1.BackColor = System.Drawing.SystemColors.Control;
			this.toolBar1.DropDownArrows = true;
			this.toolBar1.ImageList = this.imageList2;
			this.toolBar1.Location = new System.Drawing.Point(0, 0);
			this.toolBar1.Name = "toolBar1";
			this.toolBar1.ShowToolTips = true;
			this.toolBar1.Size = new System.Drawing.Size(855, 42);
			this.toolBar1.TabIndex = 1;
			// 
			// imageList2
			// 
			this.imageList2.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList2.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList2.ImageStream")));
			this.imageList2.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(855, 672);
			this.Controls.Add(this.dockManager);
			this.Controls.Add(this.toolBar1);
			this.Controls.Add(this.statusBar1);
			this.IsMdiContainer = true;
			this.Menu = this.mainMenu1;
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.ResumeLayout(false);

		}
		#endregion

		private void OnProjectReplaced(object sender, Project oldProject, Project newProject)
		{
			OnChanged(sender, newProject);
		}

		private void OnChanged(object sender, Project project)
		{
			this.Text = String.Format("ActiveRecord Generator - [{0}]", project.Name);
		}
	}
}
