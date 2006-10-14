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

namespace Castle.VSNetIntegration.CastleWizards.Dialogs.Panels
{
	using System;
	using Castle.VSNetIntegration.CastleWizards.Shared;

	/// <summary>
	/// Summary description for ConnStringPanel.
	/// </summary>
	[System.Runtime.InteropServices.ComVisible(false)]
	public class ConnStringPanel : WizardPanel
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox connectionString;
		private System.Windows.Forms.ComboBox database;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private String dependencyKey;

		public ConnStringPanel()
		{
			InitializeComponent();

			database.SelectedIndex = 0;
		}

		public String Database
		{
			get { return database.SelectedItem.ToString(); }
		}

		public String ConnectionString
		{
			get { return connectionString.Text; }
			set { connectionString.Text = value; }
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.connectionString = new System.Windows.Forms.TextBox();
			this.database = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.connectionString);
			this.groupBox1.Controls.Add(this.database);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(-8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(480, 320);
			this.groupBox1.TabIndex = 49;
			this.groupBox1.TabStop = false;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(56, 64);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(200, 16);
			this.label3.TabIndex = 48;
			this.label3.Text = "Database:";
			// 
			// connectionString
			// 
			this.connectionString.Location = new System.Drawing.Point(56, 136);
			this.connectionString.Multiline = true;
			this.connectionString.Name = "connectionString";
			this.connectionString.Size = new System.Drawing.Size(360, 64);
			this.connectionString.TabIndex = 47;
			this.connectionString.Text = "";
			// 
			// database
			// 
			this.database.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.database.Items.AddRange(new object[] {
														  "MS SQLServer",
														  "Oracle",
														  "MySql",
														  "Firebird",
														  "PostgreSQL",
														  "SQLite"});
			this.database.Location = new System.Drawing.Point(56, 80);
			this.database.Name = "database";
			this.database.Size = new System.Drawing.Size(136, 21);
			this.database.TabIndex = 45;
			this.database.SelectedIndexChanged += new System.EventHandler(this.database_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(40, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(408, 16);
			this.label1.TabIndex = 44;
			this.label1.Text = "Connection properties:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(56, 120);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(200, 16);
			this.label2.TabIndex = 46;
			this.label2.Text = "Connection string:";
			// 
			// ConnStringPanel
			// 
			this.Controls.Add(this.groupBox1);
			this.Name = "ConnStringPanel";
			this.Size = new System.Drawing.Size(464, 336);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		public override bool WantsToShow(ExtensionContext context)
		{
			return ((bool) context.Properties["enableWindsorIntegration"]) == true &&
				context.Properties.Contains(DependencyKey) &&
				((bool) context.Properties[DependencyKey]) == true;
		}

		private void database_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			int index = database.SelectedIndex;
			
			if (index < 0) return;
			
			// Show connection string example
			
			String selectedDb = Database;
			
			foreach(Pair pair in NHUtil.GetSampleConnectionStrings())
			{
				if (pair.First.ToString() == selectedDb)
				{
					connectionString.Text = pair.Second.ToString();
					break;
				}
			}
		}

		public String DependencyKey
		{
			get { return dependencyKey; }
			set { dependencyKey = value; }
		}

		public String Title
		{
			get { return label1.Text; }
			set { label1.Text = String.Format("{0} connection properties:", value); }
		}
	}
}