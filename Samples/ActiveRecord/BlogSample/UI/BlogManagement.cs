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

namespace BlogSample.UI
{
	using System;
	using System.Windows.Forms;

	public class BlogManagement : Form
	{
		private GroupBox groupBox2;
		private ColumnHeader columnHeader1;
		private ColumnHeader columnHeader2;
		private ColumnHeader columnHeader3;
		private System.Windows.Forms.ListView blogsList;
		private System.Windows.Forms.Button viewPostsButton;
		private System.Windows.Forms.Button deleteButton;
		private System.Windows.Forms.Button closeButton;
		private System.Windows.Forms.Button addButton;
		private System.ComponentModel.Container components = null;

		public BlogManagement()
		{
			InitializeComponent();
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			PopulateBlogList();
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
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.addButton = new System.Windows.Forms.Button();
			this.viewPostsButton = new System.Windows.Forms.Button();
			this.deleteButton = new System.Windows.Forms.Button();
			this.blogsList = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.closeButton = new System.Windows.Forms.Button();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.addButton);
			this.groupBox2.Controls.Add(this.viewPostsButton);
			this.groupBox2.Controls.Add(this.deleteButton);
			this.groupBox2.Controls.Add(this.blogsList);
			this.groupBox2.Location = new System.Drawing.Point(8, 8);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(568, 216);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Blog management";
			// 
			// addButton
			// 
			this.addButton.Location = new System.Drawing.Point(440, 32);
			this.addButton.Name = "addButton";
			this.addButton.Size = new System.Drawing.Size(112, 23);
			this.addButton.TabIndex = 3;
			this.addButton.Text = "Add...";
			this.addButton.Click += new System.EventHandler(this.addButton_Click);
			// 
			// viewPostsButton
			// 
			this.viewPostsButton.Location = new System.Drawing.Point(440, 96);
			this.viewPostsButton.Name = "viewPostsButton";
			this.viewPostsButton.Size = new System.Drawing.Size(112, 23);
			this.viewPostsButton.TabIndex = 2;
			this.viewPostsButton.Text = "View Posts";
			this.viewPostsButton.Click += new System.EventHandler(this.viewPostsButton_Click);
			// 
			// deleteButton
			// 
			this.deleteButton.Location = new System.Drawing.Point(440, 64);
			this.deleteButton.Name = "deleteButton";
			this.deleteButton.Size = new System.Drawing.Size(112, 23);
			this.deleteButton.TabIndex = 1;
			this.deleteButton.Text = "Delete";
			this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
			// 
			// blogsList
			// 
			this.blogsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader1,
																						this.columnHeader2,
																						this.columnHeader3});
			this.blogsList.FullRowSelect = true;
			this.blogsList.GridLines = true;
			this.blogsList.Location = new System.Drawing.Point(16, 32);
			this.blogsList.MultiSelect = false;
			this.blogsList.Name = "blogsList";
			this.blogsList.Size = new System.Drawing.Size(408, 168);
			this.blogsList.TabIndex = 0;
			this.blogsList.View = System.Windows.Forms.View.Details;
			this.blogsList.SelectedIndexChanged += new System.EventHandler(this.blogsList_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Id";
			this.columnHeader1.Width = 50;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Name";
			this.columnHeader2.Width = 170;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Author";
			this.columnHeader3.Width = 170;
			// 
			// closeButton
			// 
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.closeButton.Location = new System.Drawing.Point(464, 240);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(104, 23);
			this.closeButton.TabIndex = 5;
			this.closeButton.Text = "Close";
			this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
			// 
			// BlogManagement
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.closeButton;
			this.ClientSize = new System.Drawing.Size(584, 280);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.groupBox2);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BlogManagement";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "BlogManagement";
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private void PopulateBlogList()
		{
			blogsList.Items.Clear();

			foreach(Blog blog in Blog.FindAll())
			{
				ListViewItem item = blogsList.Items.Add(blog.Id.ToString());

				item.Tag = blog;

				item.SubItems.Add(blog.Name);
				item.SubItems.Add(blog.Author);
			}
		}

		private void closeButton_Click(object sender, System.EventArgs e)
		{
			Hide();
		}

		private void addButton_Click(object sender, System.EventArgs e)
		{
			using(BlogForm newBlog = new BlogForm())
			{
				if (newBlog.ShowDialog(this) == DialogResult.OK)
				{
					PopulateBlogList();
				}
			}
		}

		private void deleteButton_Click(object sender, System.EventArgs e)
		{
			if (blogsList.SelectedItems.Count == 0)
			{
				MessageBox.Show(this, "Select a blog to delete first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			
			foreach(ListViewItem item in blogsList.SelectedItems)
			{
				Blog blog = (Blog) item.Tag;
				
				blog.Delete();
			}
			
			PopulateBlogList();
		}

		private void viewPostsButton_Click(object sender, System.EventArgs e)
		{
			if (blogsList.SelectedItems.Count == 0)
			{
				MessageBox.Show(this, "Select a blog first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			
			Blog selected = (Blog) blogsList.SelectedItems[0].Tag;

			using(PostManagement form = new PostManagement(selected))
			{
				form.ShowDialog(this);
			}
		}

		private void blogsList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (blogsList.SelectedItems.Count == 1)
			{
				Blog blog = (Blog) blogsList.SelectedItems[0].Tag;
				
				using(BlogForm newBlog = new BlogForm(blog))
				{
					if (newBlog.ShowDialog(this) == DialogResult.OK)
					{
						PopulateBlogList();
					}
				}
			}
		}
	}
}