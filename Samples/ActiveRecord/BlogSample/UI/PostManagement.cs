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

	public class PostManagement : Form
	{
		private readonly Blog parentBlog;
		private Button closeButton;
		private GroupBox groupBox2;
		private Button addButton;
		private Button deleteButton;
		private ColumnHeader columnHeader1;
		private ColumnHeader columnHeader2;
		private ColumnHeader columnHeader3;
		private ColumnHeader columnHeader4;
		private ListView postsList;
		private System.ComponentModel.Container components = null;

		public PostManagement()
		{
			InitializeComponent();
		}

		public PostManagement(Blog parentBlog) : this()
		{
			this.parentBlog = parentBlog;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			
			PopulatePostList();
		}

		private void PopulatePostList()
		{
			parentBlog.Refresh();
			
			postsList.Items.Clear();
			
			foreach(Post post in parentBlog.Posts)
			{
				ListViewItem item = postsList.Items.Add(post.Id.ToString());
				
				item.Tag = post;
				
				item.SubItems.Add(post.Title);
				item.SubItems.Add(post.Category);
				item.SubItems.Add(post.Created.ToShortDateString());
			}
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
			this.closeButton = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.addButton = new System.Windows.Forms.Button();
			this.deleteButton = new System.Windows.Forms.Button();
			this.postsList = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader4 = new System.Windows.Forms.ColumnHeader();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// closeButton
			// 
			this.closeButton.Location = new System.Drawing.Point(464, 240);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(104, 23);
			this.closeButton.TabIndex = 7;
			this.closeButton.Text = "Close";
			this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.addButton);
			this.groupBox2.Controls.Add(this.deleteButton);
			this.groupBox2.Controls.Add(this.postsList);
			this.groupBox2.Location = new System.Drawing.Point(8, 8);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(568, 216);
			this.groupBox2.TabIndex = 6;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Post management";
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
			// deleteButton
			// 
			this.deleteButton.Location = new System.Drawing.Point(440, 64);
			this.deleteButton.Name = "deleteButton";
			this.deleteButton.Size = new System.Drawing.Size(112, 23);
			this.deleteButton.TabIndex = 1;
			this.deleteButton.Text = "Delete";
			this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
			// 
			// postsList
			// 
			this.postsList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader1,
																						this.columnHeader2,
																						this.columnHeader3,
																						this.columnHeader4});
			this.postsList.FullRowSelect = true;
			this.postsList.GridLines = true;
			this.postsList.Location = new System.Drawing.Point(16, 32);
			this.postsList.MultiSelect = false;
			this.postsList.Name = "postsList";
			this.postsList.Size = new System.Drawing.Size(408, 168);
			this.postsList.TabIndex = 0;
			this.postsList.View = System.Windows.Forms.View.Details;
			this.postsList.SelectedIndexChanged += new System.EventHandler(this.postsList_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Id";
			this.columnHeader1.Width = 50;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "Title";
			this.columnHeader2.Width = 170;
			// 
			// columnHeader3
			// 
			this.columnHeader3.Text = "Category";
			this.columnHeader3.Width = 90;
			// 
			// columnHeader4
			// 
			this.columnHeader4.Text = "Created";
			this.columnHeader4.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.columnHeader4.Width = 85;
			// 
			// PostManagement
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(586, 280);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.groupBox2);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PostManagement";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "PostManagement";
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void deleteButton_Click(object sender, System.EventArgs e)
		{
			if (postsList.SelectedItems.Count == 0)
			{
				MessageBox.Show(this, "Select a post to delete first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}
			
			foreach(ListViewItem item in postsList.SelectedItems)
			{
				Post post = (Post) item.Tag;
				
				post.Delete();
			}
			
			PopulatePostList();
		}

		private void addButton_Click(object sender, System.EventArgs e)
		{
			using(PostForm form = new PostForm(parentBlog))
			{
				if (form.ShowDialog(this) == DialogResult.OK)
				{
					PopulatePostList();
				}
			}
		}

		private void postsList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (postsList.SelectedItems.Count == 1)
			{
				Post post = (Post) postsList.SelectedItems[0].Tag;
				
				using(PostForm form = new PostForm(parentBlog, post))
				{
					if (form.ShowDialog(this) == DialogResult.OK)
					{
						PopulatePostList();
					}
				}
			}
		}

		private void closeButton_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			
			Hide();
		}
	}
}
