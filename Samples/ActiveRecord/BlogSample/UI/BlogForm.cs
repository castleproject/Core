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

	public class BlogForm : Form
	{
		private GroupBox groupBox1;
		private Label label2;
		private Label label1;
		private TextBox authorText;
		private TextBox nameText;
		private Button closeButton;
		private Button saveButton;
		private System.ComponentModel.Container components = null;

		private readonly Blog currentBlog;

		public BlogForm()
		{
			InitializeComponent();
			
			currentBlog = new Blog();
		}

		public BlogForm(Blog blog) : this()
		{
			currentBlog = blog;

			nameText.Text = currentBlog.Name;
			authorText.Text = currentBlog.Author;
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.authorText = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.nameText = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.closeButton = new System.Windows.Forms.Button();
			this.saveButton = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.authorText);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.nameText);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(456, 128);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Blog:";
			// 
			// authorText
			// 
			this.authorText.Location = new System.Drawing.Point(160, 72);
			this.authorText.Name = "authorText";
			this.authorText.Size = new System.Drawing.Size(232, 21);
			this.authorText.TabIndex = 3;
			this.authorText.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(40, 72);
			this.label2.Name = "label2";
			this.label2.TabIndex = 2;
			this.label2.Text = "Author:";
			// 
			// nameText
			// 
			this.nameText.Location = new System.Drawing.Point(160, 40);
			this.nameText.Name = "nameText";
			this.nameText.Size = new System.Drawing.Size(232, 21);
			this.nameText.TabIndex = 1;
			this.nameText.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(40, 40);
			this.label1.Name = "label1";
			this.label1.TabIndex = 0;
			this.label1.Text = "Blog Name:";
			// 
			// closeButton
			// 
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.closeButton.Location = new System.Drawing.Point(242, 152);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(104, 23);
			this.closeButton.TabIndex = 6;
			this.closeButton.Text = "Close";
			this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
			// 
			// saveButton
			// 
			this.saveButton.Location = new System.Drawing.Point(126, 152);
			this.saveButton.Name = "saveButton";
			this.saveButton.Size = new System.Drawing.Size(104, 23);
			this.saveButton.TabIndex = 7;
			this.saveButton.Text = "Save";
			this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
			// 
			// BlogForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.closeButton;
			this.ClientSize = new System.Drawing.Size(472, 190);
			this.Controls.Add(this.saveButton);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.groupBox1);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "BlogForm";
			this.Text = "BlogForm";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void closeButton_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			
			Hide();
		}

		private void saveButton_Click(object sender, System.EventArgs e)
		{
			currentBlog.Name = nameText.Text;
			currentBlog.Author = authorText.Text;
			currentBlog.Save();

			DialogResult = DialogResult.OK;
			
			Hide();
		}
	}
}
