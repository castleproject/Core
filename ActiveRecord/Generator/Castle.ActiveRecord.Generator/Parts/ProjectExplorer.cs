namespace Castle.ActiveRecord.Generator.Parts
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;
	using WeifenLuo.WinFormsUI;

	using Castle.ActiveRecord.Generator.Actions;


	/// <summary>
	/// Summary description for ProjectExplorer.
	/// </summary>
	public class ProjectExplorer : Content, ISubWorkspace
	{
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.ToolBar toolBar1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.TreeView treeView1;
		private System.ComponentModel.IContainer components;

		public ProjectExplorer()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
//			treeView1.Nodes[1].Expand();
//			treeView1.Nodes[2].Expand();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof (ProjectExplorer));
			this.imageList1 = new System.Windows.Forms.ImageList(this.components);
			this.toolBar1 = new System.Windows.Forms.ToolBar();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// imageList1
			// 
			this.imageList1.ImageSize = new System.Drawing.Size(16, 16);
			this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer) (resources.GetObject("imageList1.ImageStream")));
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
			this.toolBar1.Size = new System.Drawing.Size(246, 28);
			this.toolBar1.TabIndex = 2;
//			this.toolBar1.ButtonClick += new System.Windows.Forms.ToolBarButtonClickEventHandler(this.toolBar1_ButtonClick);
			// 
			// pictureBox1
			// 
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Location = new System.Drawing.Point(0, 28);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(246, 293);
			this.pictureBox1.TabIndex = 3;
			this.pictureBox1.TabStop = false;
			// 
			// treeView1
			// 
			this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeView1.ImageList = this.imageList1;
			this.treeView1.Indent = 19;
			this.treeView1.Location = new System.Drawing.Point(0, 28);
			this.treeView1.Name = "treeView1";
			this.treeView1.Size = new System.Drawing.Size(246, 293);
			this.treeView1.TabIndex = 4;
			// 
			// ProjectExplorer
			// 
			this.AllowedStates = ((WeifenLuo.WinFormsUI.ContentStates) (((((WeifenLuo.WinFormsUI.ContentStates.Float | WeifenLuo.WinFormsUI.ContentStates.DockLeft)
				| WeifenLuo.WinFormsUI.ContentStates.DockRight)
				| WeifenLuo.WinFormsUI.ContentStates.DockTop)
				| WeifenLuo.WinFormsUI.ContentStates.DockBottom)));
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(246, 323);
			this.Controls.Add(this.treeView1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.toolBar1);
			this.DockPadding.Bottom = 2;
			this.HideOnClose = true;
			this.Icon = ((System.Drawing.Icon) (resources.GetObject("$this.Icon")));
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
			aset.Install(this);

//			treeView1.Nodes.Add(new System.Windows.Forms.TreeNode("DockSample", 3, 3));
		}

		private void toolBar1_ButtonClick(object sender, System.Windows.Forms.ToolBarButtonClickEventArgs e)
		{
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

		public DockManager MainDockManager
		{
			get { return null; }
		}

		#endregion
	}
}