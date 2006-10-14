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
	using System.Collections;
	using System.Windows.Forms;
	using Castle.VSNetIntegration.CastleWizards.Shared;

	/// <summary>
	/// Summary description for NHIntegrationPanel.
	/// </summary>
	[System.Runtime.InteropServices.ComVisible(false)]
	public class NHIntegrationPanel : WizardPanel
	{
		private GroupBox groupBox6;
		private Label label8;
		private GroupBox groupBox2;
		private ListBox hbmFiles;
		private Button button1;
		private TextBox hbmFileName;
		private GroupBox groupBox3;
		private ListBox assemblies;
		private Button button2;
		private TextBox assemblyName;
		
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public NHIntegrationPanel()
		{
			InitializeComponent();
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

		public override bool WantsToShow(ExtensionContext context)
		{
			return ((bool) context.Properties["enableWindsorIntegration"]) == true &&
				context.Properties.Contains("NHibernate Integration") &&
				((bool) context.Properties["NHibernate Integration"]) == true;
		}

		public ICollection HbmFiles
		{
			get { return hbmFiles.Items; }
		}

		public ICollection Assemblies
		{
			get { return assemblies.Items; }
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox6 = new GroupBox();
			this.label8 = new Label();
			this.groupBox2 = new GroupBox();
			this.hbmFiles = new ListBox();
			this.button1 = new Button();
			this.hbmFileName = new TextBox();
			this.groupBox3 = new GroupBox();
			this.assemblies = new ListBox();
			this.button2 = new Button();
			this.assemblyName = new TextBox();
			this.groupBox6.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.groupBox3);
			this.groupBox6.Controls.Add(this.groupBox2);
			this.groupBox6.Controls.Add(this.label8);
			this.groupBox6.Location = new System.Drawing.Point(-12, 4);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(480, 316);
			this.groupBox6.TabIndex = 47;
			this.groupBox6.TabStop = false;
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label8.Location = new System.Drawing.Point(40, 16);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(349, 16);
			this.label8.TabIndex = 44;
			this.label8.Text = "hbm.xml mappings:";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.hbmFiles);
			this.groupBox2.Controls.Add(this.button1);
			this.groupBox2.Controls.Add(this.hbmFileName);
			this.groupBox2.Location = new System.Drawing.Point(32, 40);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(416, 128);
			this.groupBox2.TabIndex = 48;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "External hbm files";
			// 
			// hbmFiles
			// 
			this.hbmFiles.Location = new System.Drawing.Point(16, 56);
			this.hbmFiles.Name = "hbmFiles";
			this.hbmFiles.Size = new System.Drawing.Size(384, 56);
			this.hbmFiles.TabIndex = 50;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(368, 24);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(32, 24);
			this.button1.TabIndex = 49;
			this.button1.Text = "+";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// hbmFileName
			// 
			this.hbmFileName.Location = new System.Drawing.Point(16, 24);
			this.hbmFileName.Name = "hbmFileName";
			this.hbmFileName.Size = new System.Drawing.Size(344, 20);
			this.hbmFileName.TabIndex = 48;
			this.hbmFileName.Text = "";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.assemblies);
			this.groupBox3.Controls.Add(this.button2);
			this.groupBox3.Controls.Add(this.assemblyName);
			this.groupBox3.Location = new System.Drawing.Point(32, 176);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(416, 128);
			this.groupBox3.TabIndex = 49;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Assemblies:";
			// 
			// assemblies
			// 
			this.assemblies.Location = new System.Drawing.Point(16, 56);
			this.assemblies.Name = "assemblies";
			this.assemblies.Size = new System.Drawing.Size(384, 56);
			this.assemblies.TabIndex = 53;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(368, 24);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(32, 24);
			this.button2.TabIndex = 52;
			this.button2.Text = "+";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// assemblyName
			// 
			this.assemblyName.Location = new System.Drawing.Point(16, 24);
			this.assemblyName.Name = "assemblyName";
			this.assemblyName.Size = new System.Drawing.Size(344, 20);
			this.assemblyName.TabIndex = 51;
			this.assemblyName.Text = "";
			// 
			// NHIntegrationPanel
			// 
			this.Controls.Add(this.groupBox6);
			this.Name = "NHIntegrationPanel";
			this.Size = new System.Drawing.Size(456, 328);
			this.groupBox6.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void button1_Click(object sender, System.EventArgs e)
		{
			if (hbmFileName.Text.Trim() != String.Empty)
			{
				hbmFiles.Items.Add(hbmFileName.Text);
				hbmFileName.Text = String.Empty;
			}
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			if (assemblyName.Text.Trim() != String.Empty)
			{
				assemblies.Items.Add(assemblyName.Text);
				assemblyName.Text = String.Empty;
			}
		}
	}
}
