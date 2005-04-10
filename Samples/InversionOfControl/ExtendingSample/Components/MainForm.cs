
namespace ExtendingSample.Components
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;

	using ExtendingSample.Dao;

	/// <summary>
	/// Summary description for MainForm.
	/// </summary>
	public class MainForm : Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private INewsletterService _newsletterService;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ListView listView1;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.ColumnHeader columnHeader2;
		private System.Windows.Forms.ComboBox templateName;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private FriendsDao _friendsDao;

		public MainForm(INewsletterService newsletterService, FriendsDao friendsDao)
		{
			InitializeComponent();

			_newsletterService = newsletterService;
			_friendsDao = friendsDao;
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
			this.panel1 = new System.Windows.Forms.Panel();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.listView1 = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
			this.label1 = new System.Windows.Forms.Label();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.templateName = new System.Windows.Forms.ComboBox();
			this.label2 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Location = new System.Drawing.Point(16, 236);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(528, 2);
			this.panel1.TabIndex = 2;
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(12, 8);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(544, 220);
			this.tabControl1.TabIndex = 5;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.listView1);
			this.tabPage1.Controls.Add(this.label1);
			this.tabPage1.Location = new System.Drawing.Point(4, 23);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Size = new System.Drawing.Size(536, 193);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Targets";
			// 
			// listView1
			// 
			this.listView1.CheckBoxes = true;
			this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																						this.columnHeader1,
																						this.columnHeader2});
			this.listView1.FullRowSelect = true;
			this.listView1.GridLines = true;
			this.listView1.Location = new System.Drawing.Point(8, 24);
			this.listView1.Name = "listView1";
			this.listView1.Size = new System.Drawing.Size(520, 152);
			this.listView1.TabIndex = 3;
			this.listView1.View = System.Windows.Forms.View.Details;
			// 
			// columnHeader1
			// 
			this.columnHeader1.Text = "Name";
			this.columnHeader1.Width = 190;
			// 
			// columnHeader2
			// 
			this.columnHeader2.Text = "E-Mail";
			this.columnHeader2.Width = 240;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(184, 16);
			this.label1.TabIndex = 2;
			this.label1.Text = "List of friends:";
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.templateName);
			this.tabPage2.Controls.Add(this.label2);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Size = new System.Drawing.Size(536, 194);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Template";
			// 
			// templateName
			// 
			this.templateName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.templateName.Location = new System.Drawing.Point(166, 85);
			this.templateName.Name = "templateName";
			this.templateName.Size = new System.Drawing.Size(204, 22);
			this.templateName.TabIndex = 6;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(166, 65);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(200, 23);
			this.label2.TabIndex = 5;
			this.label2.Text = "Message template:";
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button1.Location = new System.Drawing.Point(384, 248);
			this.button1.Name = "button1";
			this.button1.TabIndex = 6;
			this.button1.Text = "Close";
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(468, 248);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(84, 23);
			this.button2.TabIndex = 7;
			this.button2.Text = "Dispatch";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// MainForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(7, 15);
			this.CancelButton = this.button1;
			this.ClientSize = new System.Drawing.Size(568, 283);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.panel1);
			this.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.Text = "MainForm";
			this.Load += new System.EventHandler(this.OnLoad);
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void OnCancel(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void OnLoad(object sender, System.EventArgs e)
		{
			Friend[] friends = _friendsDao.Find();

			foreach(Friend friend in friends)
			{
				listView1.Items.Add( friend.Name ).SubItems.Add(friend.Email);
			}

			templateName.Items.Add( "Independency day" );
			templateName.Items.Add( "Happy b-day" );
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			ArrayList friends = new ArrayList();

			foreach(ListViewItem item in listView1.Items)
			{
				if (!item.Checked) continue;
				friends.Add(item.Text);
			}

			_newsletterService.Dispatch( 
				"me@myhost.com", 
				(String[]) friends.ToArray( typeof(String) ), 
				templateName.SelectedText );

			MessageBox.Show("Message(s) sent!", "Done", MessageBoxButtons.OK, MessageBoxIcon.Information);
		}
	}
}
