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
	using System.Data;
	using System.Windows.Forms;

	using Castle.Facilities.ActiveRecordGenerator.CodeGenerator;
	using Castle.Facilities.ActiveRecordGenerator.Model;
	using Castle.Facilities.ActiveRecordGenerator.Utils;


	public class NewProjectForm : Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button createButton;
		private System.Windows.Forms.TextBox name;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.TextBox connectionString;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.ComboBox database;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.TextBox ns;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.FolderBrowserDialog browserDialog1;
		private System.Windows.Forms.TextBox location;
		private System.Windows.Forms.ComboBox languages;
		private System.ComponentModel.Container components = null;
		private Project _project;
		private System.Windows.Forms.Button buildConnStringButton;
		private IProjectFactory _projectFactory;

		public NewProjectForm(ICodeProviderFactory factory, IProjectFactory projectFactory)
		{
			InitializeComponent();
			InitializeDatabaseShortcuts();

			_projectFactory = projectFactory;

			CodeProviderInfo[] providers = factory.GetAvailableProviders();

			foreach (CodeProviderInfo provider in providers)
			{
				languages.Items.Add(provider);
			}

			languages.SelectedIndex = 0;

			location.Text = AppDomain.CurrentDomain.BaseDirectory;
		}

		private void InitializeDatabaseShortcuts()
		{
			database.Items.Add(new Pair("MS SQL Server", "System.Data.SqlClient.SqlConnection, System.Data, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"));
			database.Items.Add(new Pair("Oracle", "Oracle.DataAccess.Client.OracleConnection, Oracle.DataAccess"));
			database.Items.Add(new Pair("MySQL", "MySql.Data.MySqlClient.MySqlConnection, MySql.Data"));
			database.Items.Add(new Pair("Odbc", "System.Data.Odbc.OdbcConnection, System.Data, Version=1.0.5000.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"));
		}

		public Project Project
		{
			get { return _project; }
		}

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

		private void button1_Click(object sender, System.EventArgs e)
		{
			browserDialog1.RootFolder = Environment.SpecialFolder.Personal;
			browserDialog1.SelectedPath = location.Text;

			if (browserDialog1.ShowDialog(this) == DialogResult.OK)
			{
				location.Text = browserDialog1.SelectedPath;
			}
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.button1 = new System.Windows.Forms.Button();
			this.location = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.name = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.cancelButton = new System.Windows.Forms.Button();
			this.createButton = new System.Windows.Forms.Button();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.connectionString = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.database = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.languages = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.ns = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.browserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.buildConnStringButton = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.button1);
			this.groupBox1.Controls.Add(this.location);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.name);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(504, 100);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Project information:";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(384, 64);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(100, 20);
			this.button1.TabIndex = 4;
			this.button1.Text = "&Browse...";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// location
			// 
			this.location.Location = new System.Drawing.Point(136, 64);
			this.location.Name = "location";
			this.location.Size = new System.Drawing.Size(236, 21);
			this.location.TabIndex = 3;
			this.location.Text = "";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(16, 64);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(116, 16);
			this.label6.TabIndex = 2;
			this.label6.Text = "Location";
			// 
			// name
			// 
			this.name.Location = new System.Drawing.Point(136, 32);
			this.name.Name = "name";
			this.name.Size = new System.Drawing.Size(160, 21);
			this.name.TabIndex = 1;
			this.name.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(116, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Name";
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(344, 364);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.TabIndex = 4;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// createButton
			// 
			this.createButton.Location = new System.Drawing.Point(428, 364);
			this.createButton.Name = "createButton";
			this.createButton.TabIndex = 3;
			this.createButton.Text = "Create";
			this.createButton.Click += new System.EventHandler(this.createButton_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.buildConnStringButton);
			this.groupBox2.Controls.Add(this.connectionString);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.database);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Location = new System.Drawing.Point(8, 116);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(504, 132);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Database:";
			// 
			// connectionString
			// 
			this.connectionString.Location = new System.Drawing.Point(138, 60);
			this.connectionString.Multiline = true;
			this.connectionString.Name = "connectionString";
			this.connectionString.Size = new System.Drawing.Size(310, 60);
			this.connectionString.TabIndex = 11;
			this.connectionString.Text = "";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(18, 68);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(116, 16);
			this.label5.TabIndex = 10;
			this.label5.Text = "Conn. string";
			// 
			// database
			// 
			this.database.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.database.Location = new System.Drawing.Point(138, 31);
			this.database.Name = "database";
			this.database.Size = new System.Drawing.Size(164, 21);
			this.database.TabIndex = 7;
			this.database.SelectionChangeCommitted += new System.EventHandler(this.database_SelectionChangeCommitted);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(18, 35);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(116, 16);
			this.label3.TabIndex = 6;
			this.label3.Text = "Database";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.languages);
			this.groupBox3.Controls.Add(this.label7);
			this.groupBox3.Controls.Add(this.ns);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Location = new System.Drawing.Point(8, 256);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(504, 96);
			this.groupBox3.TabIndex = 2;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Code generation:";
			// 
			// languages
			// 
			this.languages.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.languages.Location = new System.Drawing.Point(140, 60);
			this.languages.Name = "languages";
			this.languages.Size = new System.Drawing.Size(164, 21);
			this.languages.TabIndex = 17;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(20, 64);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(116, 16);
			this.label7.TabIndex = 16;
			this.label7.Text = "Language";
			// 
			// ns
			// 
			this.ns.Location = new System.Drawing.Point(140, 27);
			this.ns.Name = "ns";
			this.ns.Size = new System.Drawing.Size(344, 21);
			this.ns.TabIndex = 15;
			this.ns.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(20, 32);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(116, 16);
			this.label2.TabIndex = 14;
			this.label2.Text = "Namespace";
			// 
			// buildConnStringButton
			// 
			this.buildConnStringButton.Location = new System.Drawing.Point(456, 60);
			this.buildConnStringButton.Name = "buildConnStringButton";
			this.buildConnStringButton.Size = new System.Drawing.Size(28, 20);
			this.buildConnStringButton.TabIndex = 12;
			this.buildConnStringButton.Text = "...";
			this.buildConnStringButton.Click += new System.EventHandler(this.buildConnStringButton_Click);
			// 
			// NewProjectForm
			// 
			this.AcceptButton = this.createButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(520, 400);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.createButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.groupBox1);
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte) (0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.Name = "NewProjectForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New Project";
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private void database_SelectionChangeCommitted(object sender, System.EventArgs e)
		{
//			if (database.SelectedItem != null)
//			{
//				driver.Text = (database.SelectedItem as Pair).Second;
//			}
		}

		private void createButton_Click(object sender, System.EventArgs e)
		{
			if (!ValidateConnectionInformation())
			{
				return;
			}

			_project = _projectFactory.Create();

			_project.Name = name.Text;
			_project.Location = location.Text;
//			_project.Driver = driver.Text;
			_project.ConnectionString = connectionString.Text;
			_project.CodeNamespace = ns.Text;
			_project.CodeProvider = languages.SelectedItem as CodeProviderInfo;

			if (_project.IsValid())
			{
				DialogResult = DialogResult.OK;
			}
			else
			{
				// TODO: Message box
			}
		}

		private bool ValidateConnectionInformation()
		{
//			Type driverType = Type.GetType(driver.Text, false, false);
//
//			if (driverType == null) return false;
//
//			try
//			{
//				IDbConnection connection = (IDbConnection)
//					Activator.CreateInstance(driverType);
//
//				connection.ConnectionString = connectionString.Text;
//				connection.Open();
//				connection.Close();
//			}
//			catch (Exception ex)
//			{
//				String message = String.Format("Could not create connection\r\n{0}", ex.Message);
//
//				MessageBox.Show(this, message,
//				                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
//
//				return false;
//			}

			return true;
		}

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}

		private void buildConnStringButton_Click(object sender, System.EventArgs e)
		{
			MSDASC.DataLinks dataLinks = new MSDASC.DataLinksClass();
			ADODB._Connection connection;

			if (connectionString.Text == String.Empty)
			{
				try
				{
					connection = (ADODB._Connection) dataLinks.PromptNew();

					connectionString.Text = connection.ConnectionString.ToString();
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString());
				}
			}
			else
			{
				connection = new ADODB.ConnectionClass();
				connection.ConnectionString = connectionString.Text;

				object oConnection = connection;
				
				try
				{
					if ((bool) dataLinks.PromptEdit(ref oConnection))
					{
						connectionString.Text = connection.ConnectionString;
					}
				}
				catch (Exception ex)
				{
					MessageBox.Show(ex.ToString());
				}
			}
		}
	}
}
