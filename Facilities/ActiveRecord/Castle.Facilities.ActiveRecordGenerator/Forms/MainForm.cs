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

namespace Castle.Facilities.ActiveRecordGenerator.Forms
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;
	using System.Data;

	using Castle.Facilities.ActiveRecordGenerator.Model;
	using Castle.Facilities.ActiveRecordGenerator.Forms.Views;

	public delegate void ActionDelegate(String actionName);


	public class MainForm : Form
	{
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.StatusBar statusBar1;
		private System.Windows.Forms.StatusBarPanel statusBarPanel1;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.Splitter splitter1;
		private System.Windows.Forms.MenuItem menuItem13;
		private System.Windows.Forms.MenuItem fileMenu;
		private System.Windows.Forms.MenuItem newMenu;
		private System.Windows.Forms.MenuItem openMenu;
		private System.Windows.Forms.MenuItem saveMenu;
		private System.Windows.Forms.MenuItem saveAsMenu;
		private System.Windows.Forms.MenuItem exitMenu;
		private System.Windows.Forms.MenuItem helpMenu;
		private System.Windows.Forms.MenuItem aboutMenu;
		private System.ComponentModel.IContainer components;

		private Hashtable _translator = new Hashtable();

		private Project _currentProject;
		private System.Windows.Forms.ContextMenu arContextMenu;
		private System.Windows.Forms.MenuItem newActiveRecord;
		private ProjectExplorerView _explorer;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.RichTextBox codeBox;
		private System.Windows.Forms.SaveFileDialog saveFileDialog1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private CodeView _codeView;


		public MainForm(IApplicationModel model)
		{
			InitializeComponent();
			InitTranslator();

			treeView1.ContextMenu = arContextMenu;

			_explorer = new ProjectExplorerView(treeView1, model);
			_codeView = new CodeView(codeBox, model);
		}

		public event ActionDelegate OnAction;

		private void InitTranslator()
		{
			_translator.Add(newMenu, ActionConstants.New_Project);
			_translator.Add(openMenu, ActionConstants.Open_Project);
			_translator.Add(saveMenu, ActionConstants.Save_Project);
			_translator.Add(saveAsMenu, ActionConstants.SaveAs_Project);
			_translator.Add(exitMenu, ActionConstants.Exit);
			_translator.Add(newActiveRecord, ActionConstants.New_Active_Record);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose(disposing);
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
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.fileMenu = new System.Windows.Forms.MenuItem();
			this.newMenu = new System.Windows.Forms.MenuItem();
			this.openMenu = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.saveMenu = new System.Windows.Forms.MenuItem();
			this.saveAsMenu = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.menuItem13 = new System.Windows.Forms.MenuItem();
			this.menuItem9 = new System.Windows.Forms.MenuItem();
			this.exitMenu = new System.Windows.Forms.MenuItem();
			this.helpMenu = new System.Windows.Forms.MenuItem();
			this.aboutMenu = new System.Windows.Forms.MenuItem();
			this.statusBar1 = new System.Windows.Forms.StatusBar();
			this.statusBarPanel1 = new System.Windows.Forms.StatusBarPanel();
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.splitter1 = new System.Windows.Forms.Splitter();
			this.arContextMenu = new System.Windows.Forms.ContextMenu();
			this.newActiveRecord = new System.Windows.Forms.MenuItem();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.codeBox = new System.Windows.Forms.RichTextBox();
			this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).BeginInit();
			this.SuspendLayout();
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.fileMenu,
																					  this.menuItem1,
																					  this.helpMenu});
			// 
			// fileMenu
			// 
			this.fileMenu.Index = 0;
			this.fileMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.newMenu,
																					 this.openMenu,
																					 this.menuItem4,
																					 this.saveMenu,
																					 this.saveAsMenu,
																					 this.menuItem7,
																					 this.menuItem8,
																					 this.menuItem9,
																					 this.exitMenu});
			this.fileMenu.Text = "&File";
			// 
			// newMenu
			// 
			this.newMenu.Index = 0;
			this.newMenu.Text = "&New...";
			this.newMenu.Click += new System.EventHandler(this.Menu_Click);
			// 
			// openMenu
			// 
			this.openMenu.Index = 1;
			this.openMenu.Text = "Open...";
			this.openMenu.Click += new System.EventHandler(this.Menu_Click);
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 2;
			this.menuItem4.Text = "-";
			// 
			// saveMenu
			// 
			this.saveMenu.Index = 3;
			this.saveMenu.Text = "&Save";
			this.saveMenu.Click += new System.EventHandler(this.Menu_Click);
			// 
			// saveAsMenu
			// 
			this.saveAsMenu.Index = 4;
			this.saveAsMenu.Text = "S&ave As...";
			this.saveAsMenu.Click += new System.EventHandler(this.Menu_Click);
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 5;
			this.menuItem7.Text = "-";
			// 
			// menuItem8
			// 
			this.menuItem8.Index = 6;
			this.menuItem8.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem13});
			this.menuItem8.Text = "Recent Projecs";
			// 
			// menuItem13
			// 
			this.menuItem13.Index = 0;
			this.menuItem13.Text = "Sample";
			// 
			// menuItem9
			// 
			this.menuItem9.Index = 7;
			this.menuItem9.Text = "-";
			// 
			// exitMenu
			// 
			this.exitMenu.Index = 8;
			this.exitMenu.Text = "E&xit";
			this.exitMenu.Click += new System.EventHandler(this.Menu_Click);
			// 
			// helpMenu
			// 
			this.helpMenu.Index = 2;
			this.helpMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.aboutMenu});
			this.helpMenu.Text = "&Help";
			// 
			// aboutMenu
			// 
			this.aboutMenu.Index = 0;
			this.aboutMenu.Text = "About...";
			this.aboutMenu.Click += new System.EventHandler(this.Menu_Click);
			// 
			// statusBar1
			// 
			this.statusBar1.Location = new System.Drawing.Point(0, 440);
			this.statusBar1.Name = "statusBar1";
			this.statusBar1.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																						  this.statusBarPanel1});
			this.statusBar1.ShowPanels = true;
			this.statusBar1.Size = new System.Drawing.Size(752, 22);
			this.statusBar1.TabIndex = 3;
			this.statusBar1.Text = "statusBar1";
			// 
			// statusBarPanel1
			// 
			this.statusBarPanel1.AutoSize = System.Windows.Forms.StatusBarPanelAutoSize.Spring;
			this.statusBarPanel1.Text = "Ready.";
			this.statusBarPanel1.Width = 736;
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth32Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// treeView1
			// 
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Left;
			this.treeView1.ImageList = this.imageList1;
			this.treeView1.Location = new System.Drawing.Point(0, 0);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(178, 440);
			this.treeView1.TabIndex = 8;
			this.treeView1.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeView1_BeforeLabelEdit);
			// 
			// splitter1
			// 
			this.splitter1.Location = new System.Drawing.Point(178, 0);
			this.splitter1.Name = "splitter1";
			this.splitter1.Size = new System.Drawing.Size(4, 440);
			this.splitter1.TabIndex = 9;
			this.splitter1.TabStop = false;
			// 
			// arContextMenu
			// 
			this.arContextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						  this.newActiveRecord});
			// 
			// newActiveRecord
			// 
			this.newActiveRecord.Index = 0;
			this.newActiveRecord.Text = "Create ActiveRecord Representation...";
			this.newActiveRecord.Click += new System.EventHandler(this.Menu_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Location = new System.Drawing.Point(182, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(570, 440);
			this.pictureBox1.TabIndex = 10;
			this.pictureBox1.TabStop = false;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.Transparent;
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label1.Dock = System.Windows.Forms.DockStyle.Top;
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(182, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(570, 24);
			this.label1.TabIndex = 11;
			this.label1.Text = "Code Preview:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// codeBox
			// 
			this.codeBox.BackColor = System.Drawing.SystemColors.ControlLight;
			this.codeBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.codeBox.Font = new System.Drawing.Font("Lucida Console", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.codeBox.Location = new System.Drawing.Point(182, 24);
			this.codeBox.Name = "codeBox";
			this.codeBox.ReadOnly = true;
			this.codeBox.Size = new System.Drawing.Size(570, 416);
			this.codeBox.TabIndex = 12;
			this.codeBox.Text = "";
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 1;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem2});
			this.menuItem1.Text = "Project";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.Text = "Generate...";
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(752, 462);
			this.Controls.Add(this.codeBox);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.splitter1);
			this.Controls.Add(this.treeView1);
			this.Controls.Add(this.statusBar1);
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Menu = this.mainMenu1;
			this.Name = "MainForm";
			this.Text = "Castle Project\'s ActiveRecord Generator";
			((System.ComponentModel.ISupportInitialize)(this.statusBarPanel1)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private void Menu_Click(object sender, System.EventArgs e)
		{
			String actionName = (String) _translator[sender];

			if (OnAction != null && actionName != null)
			{
				OnAction(actionName);
			}
		}

		private void treeView1_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
		{
			e.CancelEdit = true;
		}
	}
}