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

	public class MainForm : Form, IWorkspace
	{
		private DockManager dockManager;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.ComponentModel.IContainer components;

		public MainForm()
		{
			InitializeComponent();
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

		public DockManager MainDockManager
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
			this.Visible = false;
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MainForm));
			this.dockManager = new WeifenLuo.WinFormsUI.DockManager();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.toolBar1 = new System.Windows.Forms.ToolBar();
			this.SuspendLayout();
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 500);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Size = new System.Drawing.Size(832, 22);
			this.statusBar1.TabIndex = 0;
			this.statusBar1.Text = "statusBar1";
			// 
			// toolBar1
			// 
			this.toolBar1.DropDownArrows = true;
			this.toolBar1.ImageList = this.imageList1;
			this.toolBar1.Location = new System.Drawing.Point(0, 0);
			this.toolBar1.Name = "toolBar1";
			this.toolBar1.ShowToolTips = true;
			this.toolBar1.Size = new System.Drawing.Size(832, 18);
			this.toolBar1.TabIndex = 1;
			this.toolBar1.BackColor = System.Drawing.SystemColors.Control;
			this.toolBar1.Appearance = ToolBarAppearance.Flat;
			// 
			// dockManager
			// 
			// 
			// dockManager
			// 
			this.dockManager.ActiveAutoHideContent = null;
			this.dockManager.Dock = System.Windows.Forms.DockStyle.Fill;
			this.dockManager.Location = new System.Drawing.Point(0, 28);
			this.dockManager.Name = "dockManager";
			this.dockManager.Size = new System.Drawing.Size(1027, 672);
			this.dockManager.TabIndex = 1;
			//			this.dockManager.ContentRemoved += new WeifenLuo.WinFormsUI.DockManager.ContentEventHandler(this.dockManager_ContentRemoved);
			//			this.dockManager.ActiveDocumentChanged += new System.EventHandler(this.dockManager_ActiveDocumentChanged);
			//			this.dockManager.ContentAdded += new WeifenLuo.WinFormsUI.DockManager.ContentEventHandler(this.dockManager_ContentAdded);
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(1027, 724);
			this.Controls.Add(this.dockManager);
			this.Controls.Add(this.toolBar1);
			this.Controls.Add(this.statusBar1);
			this.IsMdiContainer = true;
			this.Menu = this.mainMenu1;
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			//			this.Closing += new System.ComponentModel.CancelEventHandler(this.MainForm_Closing);
			this.ResumeLayout(false);
		}
		#endregion
	}
}
