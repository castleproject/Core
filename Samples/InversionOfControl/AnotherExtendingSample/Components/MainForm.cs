
namespace Extending2.Components
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Extending2.Dao;

	/// <summary>
	/// Summary description for MainForm.
	/// </summary>
	public class MainForm : Form
	{
		private System.Windows.Forms.TreeView treeView1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox titleBox;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox contentsBox;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private System.Windows.Forms.Button saveButton;
		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.Button newPostButton;

		private Blog _selectedBlog;
		private Post _selectedPost;
		private NewBlog _newBlog;
		private BlogDao _blogDao;
		private PostDao _postDao;


		public MainForm(NewBlog newBlog, BlogDao blogDao, PostDao postDao)
		{
			InitializeComponent();

			_newBlog = newBlog;
			_blogDao = blogDao;
			_postDao = postDao;
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

		protected override void OnClosed(EventArgs e)
		{
			base.OnClosed(e);

			Application.Exit();
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.treeView1 = new System.Windows.Forms.TreeView();
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.saveButton = new System.Windows.Forms.Button();
			this.closeButton = new System.Windows.Forms.Button();
			this.titleBox = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.contentsBox = new System.Windows.Forms.TextBox();
			this.newPostButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// treeView1
			// 
			this.treeView1.ImageIndex = -1;
			this.treeView1.Location = new System.Drawing.Point(8, 12);
			this.treeView1.Name = "treeView1";
			this.treeView1.SelectedImageIndex = -1;
			this.treeView1.Size = new System.Drawing.Size(124, 252);
			this.treeView1.TabIndex = 0;
			this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Verdana", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(140, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(412, 23);
			this.label1.TabIndex = 1;
			this.label1.Text = "Post";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(12, 276);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(120, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "Add new Blog";
			this.button1.Click += new System.EventHandler(this.OnAddNewBlog);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.ControlDark;
			this.panel1.Location = new System.Drawing.Point(144, 36);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(404, 4);
			this.panel1.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(144, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 4;
			this.label2.Text = "Title:";
			// 
			// saveButton
			// 
			this.saveButton.Location = new System.Drawing.Point(388, 276);
			this.saveButton.Name = "saveButton";
			this.saveButton.TabIndex = 5;
			this.saveButton.Text = "Save";
			this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
			// 
			// closeButton
			// 
			this.closeButton.Location = new System.Drawing.Point(472, 276);
			this.closeButton.Name = "closeButton";
			this.closeButton.TabIndex = 6;
			this.closeButton.Text = "Close";
			// 
			// titleBox
			// 
			this.titleBox.Location = new System.Drawing.Point(252, 52);
			this.titleBox.Name = "titleBox";
			this.titleBox.Size = new System.Drawing.Size(292, 22);
			this.titleBox.TabIndex = 7;
			this.titleBox.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(144, 84);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 16);
			this.label3.TabIndex = 8;
			this.label3.Text = "Contents:";
			// 
			// contentsBox
			// 
			this.contentsBox.Location = new System.Drawing.Point(144, 104);
			this.contentsBox.Multiline = true;
			this.contentsBox.Name = "contentsBox";
			this.contentsBox.Size = new System.Drawing.Size(404, 160);
			this.contentsBox.TabIndex = 9;
			this.contentsBox.Text = "Contents here...";
			// 
			// newPostButton
			// 
			this.newPostButton.Location = new System.Drawing.Point(144, 276);
			this.newPostButton.Name = "newPostButton";
			this.newPostButton.Size = new System.Drawing.Size(120, 23);
			this.newPostButton.TabIndex = 10;
			this.newPostButton.Text = "New Post";
			this.newPostButton.Click += new System.EventHandler(this.newPostButton_Click);
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(7, 15);
			this.ClientSize = new System.Drawing.Size(562, 323);
			this.Controls.Add(this.newPostButton);
			this.Controls.Add(this.contentsBox);
			this.Controls.Add(this.titleBox);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.saveButton);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.treeView1);
			this.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.Load += new System.EventHandler(this.OnLoad);
			this.ResumeLayout(false);

		}
		#endregion

		private void OnCancel(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void OnLoad(object sender, System.EventArgs e)
		{
			RefreshView();
		}

		private void OnAddNewBlog(object sender, System.EventArgs e)
		{
			_newBlog.ShowDialog(this);

			RefreshView();
		}

		private void RefreshView()
		{
			ClearFields();
			LockFields();

			treeView1.Nodes.Clear();

			IList blogs = _blogDao.Find();

			TreeNode rootNode = treeView1.Nodes.Add("Blogs");

			foreach(Blog blog in blogs)
			{
				TreeNode blogNode = rootNode.Nodes.Add( blog.Name );
				blogNode.Tag = blog;

				IList posts =  _postDao.Find(blog);

				foreach(Post post in posts)
				{
					blogNode.Nodes.Add( post.Title ).Tag = post;
				}
			}
		}

		private void newPostButton_Click(object sender, System.EventArgs e)
		{
			TreeNode selectedNode = treeView1.SelectedNode;

			if (selectedNode.Tag is Blog)
			{
				ClearFields();
				SelectedBlog = selectedNode.Tag as Blog;
			}
			else
			{
				MessageBox.Show("You must select a blog in the tree first.", 
					"Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ClearFields()
		{
			UnlockFields();

			titleBox.Text = "";
			contentsBox.Text = "";
		}

		private void UnlockFields()
		{
			titleBox.ReadOnly = contentsBox.ReadOnly = false;
		}

		private void LockFields()
		{
			titleBox.ReadOnly = contentsBox.ReadOnly = true;
		}

		private void saveButton_Click(object sender, System.EventArgs e)
		{
			if (SelectedBlog != null)
			{
				if (titleBox.Text.Trim().Length == 0)
				{
					MessageBox.Show("At least you must fill the title for your post.", 
						"Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
					return;
				}

				if (SelectedPost == null)
				{
					_postDao.Create( new Post(SelectedBlog, titleBox.Text, contentsBox.Text) );
				}
				else
				{
					SelectedPost.Title = titleBox.Text;
					SelectedPost.Contents = contentsBox.Text;
					
					_postDao.Update( SelectedPost );
				}

				RefreshView();
			}
		}

		private void treeView1_AfterSelect(object sender, System.Windows.Forms.TreeViewEventArgs e)
		{
			if (e.Node.Tag is Post)
			{
				SelectedPost = e.Node.Tag as Post;
				SelectedBlog = SelectedPost.Blog;

				UnlockFields();
			}
			else
			{
				LockFields();
			}
		}

		public Blog SelectedBlog
		{
			get { return _selectedBlog; }
			set { _selectedBlog = value; }
		}

		public Post SelectedPost
		{
			get { return _selectedPost; }
			set { 
				_selectedPost = value;
				titleBox.Text = _selectedPost.Title;
				contentsBox.Text = _selectedPost.Contents;
			}
		}
	}
}
