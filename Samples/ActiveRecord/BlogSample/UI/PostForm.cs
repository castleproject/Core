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
	using System.Windows.Forms;

	public class PostForm : Form
	{
		private Button saveButton;
		private Button closeButton;
		private GroupBox groupBox1;
		private Label label2;
		private Label label1;
		private Label label3;
		private Label label4;
		private TextBox contentsText;
		private TextBox titleText;
		private TextBox categoryText;
		private DateTimePicker createdDtTime;
		private CheckBox publishedCheck;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private readonly Post currentPost;
		private readonly Blog parentBlog;

		public PostForm()
		{
			InitializeComponent();
		}

		public PostForm(Blog parentBlog) : this()
		{
			this.parentBlog = parentBlog;
			
			currentPost = new Post();
		}

		public PostForm(Blog parentBlog, Post post) : this(parentBlog)
		{
			currentPost = post;
			
			titleText.Text = currentPost.Title;
			contentsText.Text = currentPost.Contents;
			categoryText.Text = currentPost.Category;
			createdDtTime.Value = currentPost.Created;
			publishedCheck.Checked = currentPost.Published;
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
			this.saveButton = new System.Windows.Forms.Button();
			this.closeButton = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.publishedCheck = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.createdDtTime = new System.Windows.Forms.DateTimePicker();
			this.categoryText = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.contentsText = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.titleText = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// saveButton
			// 
			this.saveButton.Location = new System.Drawing.Point(124, 296);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(104, 23);
			this.saveButton.TabIndex = 10;
			this.saveButton.Text = "Save";
			this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
			// 
			// closeButton
			// 
			this.closeButton.Location = new System.Drawing.Point(244, 296);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(104, 23);
			this.closeButton.TabIndex = 9;
			this.closeButton.Text = "Close";
			this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.publishedCheck);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.createdDtTime);
			this.groupBox1.Controls.Add(this.categoryText);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.contentsText);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.titleText);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(456, 272);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Blog:";
			// 
			// publishedCheck
			// 
			this.publishedCheck.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.publishedCheck.Location = new System.Drawing.Point(40, 224);
			this.publishedCheck.Name = "publishedCheck";
			this.publishedCheck.Size = new System.Drawing.Size(136, 24);
			this.publishedCheck.TabIndex = 8;
			this.publishedCheck.Text = "Published:";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(40, 192);
			this.label4.Name = "label4";
			this.label4.TabIndex = 7;
			this.label4.Text = "Created at:";
			// 
			// createdDtTime
			// 
			this.createdDtTime.Location = new System.Drawing.Point(160, 192);
			this.createdDtTime.Name = "createdDtTime";
			this.createdDtTime.Size = new System.Drawing.Size(232, 21);
			this.createdDtTime.TabIndex = 6;
			// 
			// categoryText
			// 
			this.categoryText.Location = new System.Drawing.Point(160, 160);
			this.categoryText.Name = "categoryText";
			this.categoryText.Size = new System.Drawing.Size(232, 21);
			this.categoryText.TabIndex = 5;
			this.categoryText.Text = "";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(40, 160);
			this.label3.Name = "label3";
			this.label3.TabIndex = 4;
			this.label3.Text = "Category:";
			// 
			// contentsText
			// 
			this.contentsText.Location = new System.Drawing.Point(160, 72);
			this.contentsText.Multiline = true;
			this.contentsText.Name = "contentsText";
			this.contentsText.Size = new System.Drawing.Size(232, 80);
			this.contentsText.TabIndex = 3;
			this.contentsText.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(40, 72);
			this.label2.Name = "label2";
			this.label2.TabIndex = 2;
			this.label2.Text = "Contents:";
			// 
			// titleText
			// 
			this.titleText.Location = new System.Drawing.Point(160, 40);
			this.titleText.Name = "titleText";
			this.titleText.Size = new System.Drawing.Size(232, 21);
			this.titleText.TabIndex = 1;
			this.titleText.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(40, 40);
			this.label1.Name = "label1";
			this.label1.TabIndex = 0;
			this.label1.Text = "Title:";
			// 
			// PostForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.ClientSize = new System.Drawing.Size(472, 334);
			this.Controls.Add(this.saveButton);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.groupBox1);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "PostForm";
			this.Text = "PostForm";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private void saveButton_Click(object sender, System.EventArgs e)
		{
			currentPost.Blog = parentBlog;
			
			currentPost.Title = titleText.Text;
			currentPost.Contents = contentsText.Text;
			currentPost.Category = categoryText.Text;
			currentPost.Created = createdDtTime.Value;
			currentPost.Published = publishedCheck.Checked;
			
			currentPost.Save();
			
			DialogResult = DialogResult.OK;
			
			Hide();
		}

		private void closeButton_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			
			Hide();
		}
	}
}