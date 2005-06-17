namespace Extending2.Components
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;

	using Extending2.Dao;

	/// <summary>
	/// Summary description for NewBlog.
	/// </summary>
	public class NewBlog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox nameBox;
		private System.Windows.Forms.TextBox descBox;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private BlogDao _blogDao;

		public NewBlog(BlogDao blogDao)
		{
			InitializeComponent();

			_blogDao = blogDao;
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.nameBox = new System.Windows.Forms.TextBox();
			this.descBox = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(24, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(96, 23);
			this.label2.TabIndex = 1;
			this.label2.Text = "Description:";
			// 
			// nameBox
			// 
			this.nameBox.Location = new System.Drawing.Point(120, 31);
			this.nameBox.Name = "nameBox";
			this.nameBox.Size = new System.Drawing.Size(128, 22);
			this.nameBox.TabIndex = 2;
			this.nameBox.Text = "";
			// 
			// descBox
			// 
			this.descBox.Location = new System.Drawing.Point(120, 63);
			this.descBox.Name = "descBox";
			this.descBox.Size = new System.Drawing.Size(224, 22);
			this.descBox.TabIndex = 3;
			this.descBox.Text = "";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(108, 120);
			this.button1.Name = "button1";
			this.button1.TabIndex = 4;
			this.button1.Text = "Create";
			this.button1.Click += new System.EventHandler(OnSave);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(201, 120);
			this.button2.Name = "button2";
			this.button2.TabIndex = 5;
			this.button2.Text = "Close";
			this.button2.Click += new System.EventHandler(OnClose);
			// 
			// NewBlog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(7, 15);
			this.ClientSize = new System.Drawing.Size(384, 157);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.descBox);
			this.Controls.Add(this.nameBox);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
			this.Name = "NewBlog";
			this.Text = "NewBlog";
			this.ResumeLayout(false);

		}
		#endregion

		private void OnSave(object sender, System.EventArgs e)
		{
			_blogDao.Create( new Blog(nameBox.Text, descBox.Text) );

			Close();
		}

		private void OnClose(object sender, System.EventArgs e)
		{
			Close();
		}
	}
}
