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

using Castle.ActiveRecord.Generator.Dialogs;

namespace Castle.ActiveRecord.Generator.Parts
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;
	using WeifenLuo.WinFormsUI;

	using Castle.ActiveRecord.Generator.Actions;
	using Castle.ActiveRecord.Generator.Components;
	using Castle.ActiveRecord.Generator.Components.Database;


	/// <summary>
	/// Summary description for ProjectExplorer.
	/// </summary>
	public class ProjectExplorer : DockContent, ISubWorkspace
	{
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.TreeView treeView1;
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.ContextMenu contextMenu1;
		private System.Windows.Forms.MenuItem propertiesMenu;
		private Model _model;

		public ProjectExplorer()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			treeView1.ContextMenu = contextMenu1;
		}

		public ProjectExplorer(Model model) : this()
		{
			_model = model;

			_model.OnProjectReplaced += new ProjectReplaceDelegate(OnProjectReplaced);
			_model.OnProjectChanged += new ProjectDelegate(OnProjectChange);
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ProjectExplorer));
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.toolBar1 = new System.Windows.Forms.ToolBar();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.contextMenu1 = new System.Windows.Forms.ContextMenu();
			this.propertiesMenu = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// imageList1
			// 
			this.imageList1.ColorDepth = System.Windows.Forms.ColorDepth.Depth24Bit;
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
			this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
			// 
			// toolBar1
			// 
			this.toolBar1.Appearance = System.Windows.Forms.ToolBarAppearance.Flat;
			this.toolBar1.DropDownArrows = true;
			this.toolBar1.ImageList = this.imageList1;
			this.toolBar1.Location = new System.Drawing.Point(0, 0);
			this.toolBar1.Name = "toolBar1";
			this.toolBar1.ShowToolTips = true;
			this.toolBar1.Size = new System.Drawing.Size(246, 42);
			this.toolBar1.TabIndex = 2;
			// 
			// pictureBox1
			// 
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Location = new System.Drawing.Point(0, 42);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(246, 279);
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			// 
			// treeView1
			// 
			this.treeView1.ContextMenu = this.contextMenu1;
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.ImageList = this.imageList1;
			this.treeView1.Indent = 19;
			this.treeView1.Location = new System.Drawing.Point(0, 42);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(246, 279);
			this.treeView1.TabIndex = 4;
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			// 
			// contextMenu1
			// 
			this.contextMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.propertiesMenu});
			// 
			// propertiesMenu
			// 
			this.propertiesMenu.Index = 0;
			this.propertiesMenu.Text = "Properties";
			this.propertiesMenu.Click += new System.EventHandler(this.propertiesMenu_Click);
			// 
			// ProjectExplorer
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(246, 323);
			this.Controls.Add(this.treeView1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.toolBar1);
			this.DockableAreas = ((WeifenLuo.WinFormsUI.DockAreas)(((((WeifenLuo.WinFormsUI.DockAreas.Float | WeifenLuo.WinFormsUI.DockAreas.DockLeft) 
				| WeifenLuo.WinFormsUI.DockAreas.DockRight) 
				| WeifenLuo.WinFormsUI.DockAreas.DockTop) 
				| WeifenLuo.WinFormsUI.DockAreas.DockBottom)));
			this.DockPadding.Bottom = 2;
			this.HideOnClose = true;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "ProjectExplorer";
			this.ShowHint = WeifenLuo.WinFormsUI.DockState.DockRight;
			this.Text = "Project Explorer";
			this.Load += new System.EventHandler(this.ProjectExplorer_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private void ProjectExplorer_Load(object sender, System.EventArgs e)
		{
			ProjectExplorerActionSet aset = new ProjectExplorerActionSet();
			aset.Init(_model);
			aset.Install(this);
		}

		#region IWorkspace Members

		public IWin32Window ActiveWindow
		{
			get { return this; }
		}

		public MainMenu MainMenu
		{
			get { return null; }
		}

		public ToolBar MainToolBar
		{
			get { return toolBar1; }
		}

		public StatusBar MainStatusBar
		{
			get { return null; }
		}

		public DockPanel MainDockManager
		{
			get { return null; }
		}

		public IWorkspace ParentWorkspace
		{
			get { return null; }
			set {  }
		}

		#endregion

		private void OnProjectReplaced(object sender, Project oldProject, Project newProject)
		{
			UpdateTree(newProject);
		}

		private void OnProjectChange(object sender, Project project)
		{
			UpdateTree(project);
		}

		private void UpdateTree(Project project)
		{
			treeView1.Nodes.Clear();

			TreeNode projectNode = new TreeNode(project.Name, ImageConstants.Database_Views, ImageConstants.Database_Views);

			treeView1.Nodes.Add(projectNode);

			foreach(DatabaseDefinition def in project.Databases)
			{
				TreeNode dbNode = new TreeNode(def.Alias, ImageConstants.Database_Catalog, ImageConstants.Database_Catalog);
				projectNode.Nodes.Add(dbNode);
				dbNode.EnsureVisible();

				foreach(TableDefinition table in def.Tables)
				{
					TreeNode tableNode = new TreeNode(table.Name, ImageConstants.Database_Table, ImageConstants.Database_Table);
					dbNode.Nodes.Add(tableNode);

					foreach(ColumnDefinition colDef in table.Columns)
					{
						TreeNode colNode = new TreeNode(colDef.Name, ImageConstants.Database_Field, ImageConstants.Database_Field);
						tableNode.Nodes.Add(colNode);
					}
				}
			}

			Hashtable db2Node = new Hashtable();

			foreach(DictionaryEntry entry in project.BaseClasses)
			{
				IActiveRecordDescriptor baseDesc = entry.Value as IActiveRecordDescriptor;

				TreeNode arBaseNode = new TreeNode(baseDesc.ClassName, ImageConstants.Classes_Entity, ImageConstants.Classes_Entity);
				arBaseNode.Tag = baseDesc;

				projectNode.Nodes.Add(arBaseNode);

				db2Node[entry.Key] = arBaseNode;
			}

			foreach(IActiveRecordDescriptor desc in project.Descriptors)
			{
				ActiveRecordDescriptor arDesc = desc as ActiveRecordDescriptor;

				if (arDesc == null) continue;

				TreeNode arNode = new TreeNode(desc.ClassName, ImageConstants.Classes_Entity, ImageConstants.Classes_Entity);
				arNode.Tag = arDesc;

				TreeNode parent = db2Node[arDesc.Table.DatabaseDefinition] as TreeNode;

				parent.Nodes.Add(arNode);
			}
		}

		private void propertiesMenu_Click(object sender, System.EventArgs e)
		{
			if (treeView1.SelectedNode.Tag is ActiveRecordDescriptor) 
			{
				using(ActiveRecordPropertiesDialog d = 
						  new ActiveRecordPropertiesDialog(
							treeView1.SelectedNode.Tag as ActiveRecordDescriptor, 
							_model.CurrentProject))
				{
					d.ShowDialog();
				}
			}
		}

		private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
		{
			propertiesMenu.Enabled = (e.Node.Tag is IActiveRecordDescriptor);
		}
	}
}