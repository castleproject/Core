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

using Castle.ActiveRecord.Generator.Components.Database;

namespace Castle.ActiveRecord.Generator.Dialogs.Wizards
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using System.Data;
	using System.Windows.Forms;

	/// <summary>
	/// Summary description for WelcomePage.
	/// </summary>
	public class WelcomePage : AbstractControlPage
	{
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.ComboBox dbConnections;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public WelcomePage() : base("Welcome!")
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.dbConnections = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBox1.Location = new System.Drawing.Point(16, 56);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(584, 72);
			this.textBox1.TabIndex = 1;
			this.textBox1.TabStop = false;
			this.textBox1.Text = @"You need to associate this new ActiveRecord class to a database connection, in order to allow this generator to extract information and infer some relations. 

If you haven't created a database connection yet, please Cancel this Wizard, go to the Project Explorer window and create one or more database connections.";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(192, 192);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(140, 17);
			this.label2.TabIndex = 2;
			this.label2.Text = "Database Connection Alias:";
			// 
			// dbConnections
			// 
			this.dbConnections.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.dbConnections.Location = new System.Drawing.Point(192, 208);
			this.dbConnections.Name = "dbConnections";
			this.dbConnections.Size = new System.Drawing.Size(232, 21);
			this.dbConnections.TabIndex = 3;
			this.dbConnections.SelectedIndexChanged += new System.EventHandler(this.dbConnections_SelectedIndexChanged);
			// 
			// WelcomePage
			// 
			this.Controls.Add(this.dbConnections);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textBox1);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Name = "WelcomePage";
			this.Size = new System.Drawing.Size(616, 344);
			this.ResumeLayout(false);

		}
		#endregion

		public override void Initialize(Model model, IDictionary context)
		{
			base.Initialize(model, context);

			dbConnections.Items.Clear();
			dbConnections.ValueMember = "Alias";

			foreach(DatabaseDefinition def in model.CurrentProject.Databases)
			{
				dbConnections.Items.Add(def);
			}
		}

		public override void Deactivated(IDictionary context)
		{
			context["selecteddb"] = dbConnections.SelectedItem;
		}

		public override bool IsValid()
		{
			return dbConnections.SelectedIndex != -1;
		}

		private void dbConnections_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			RaiseChange(this, e);
		}

		public override String ErrorMessage
		{
			get
			{
				return "Please select a database connection";
			}
		}
	}
}
