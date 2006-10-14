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
	/// Summary description for ARIntegrationPanel.
	/// </summary>
	[System.Runtime.InteropServices.ComVisible(false)]
	public class ARIntegrationPanel : WizardPanel
	{
		private GroupBox groupBox6;
		private Label label8;
		private TextBox assemblyName;
		private Button button1;
		private ListBox arAssemblies;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ARIntegrationPanel()
		{
			InitializeComponent();
		}

		public ICollection ARAssemblies
		{
			get { return arAssemblies.Items; }
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
			this.groupBox6 = new GroupBox();
			this.arAssemblies = new ListBox();
			this.button1 = new Button();
			this.assemblyName = new TextBox();
			this.label8 = new Label();
			this.groupBox6.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.arAssemblies);
			this.groupBox6.Controls.Add(this.button1);
			this.groupBox6.Controls.Add(this.assemblyName);
			this.groupBox6.Controls.Add(this.label8);
			this.groupBox6.Location = new System.Drawing.Point(-8, 8);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(480, 320);
			this.groupBox6.TabIndex = 45;
			this.groupBox6.TabStop = false;
			// 
			// arAssemblies
			// 
			this.arAssemblies.Location = new System.Drawing.Point(88, 104);
			this.arAssemblies.Name = "arAssemblies";
			this.arAssemblies.Size = new System.Drawing.Size(304, 134);
			this.arAssemblies.TabIndex = 47;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(360, 72);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(32, 24);
			this.button1.TabIndex = 46;
			this.button1.Text = "+";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// assemblyName
			// 
			this.assemblyName.Location = new System.Drawing.Point(88, 72);
			this.assemblyName.Name = "assemblyName";
			this.assemblyName.Size = new System.Drawing.Size(264, 20);
			this.assemblyName.TabIndex = 45;
			this.assemblyName.Text = "Name.Of.Your.Assembly";
			this.assemblyName.Enter += new System.EventHandler(this.assemblyName_Enter);
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte) (0)));
			this.label8.Location = new System.Drawing.Point(40, 16);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(349, 16);
			this.label8.TabIndex = 44;
			this.label8.Text = "ActiveRecord assemblies:";
			// 
			// ARIntegrationPanel
			// 
			this.Controls.Add(this.groupBox6);
			this.Name = "ARIntegrationPanel";
			this.Size = new System.Drawing.Size(464, 336);
			this.groupBox6.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		public override bool WantsToShow(ExtensionContext context)
		{
			return ((bool) context.Properties["enableWindsorIntegration"]) &&
				context.Properties.Contains("ActiveRecord Integration") &&
				((bool) context.Properties["ActiveRecord Integration"]);
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			if (assemblyName.Text.Trim() != String.Empty)
			{
				arAssemblies.Items.Add(assemblyName.Text);

				assemblyName.Text = String.Empty;
			}
		}

		private void assemblyName_Enter(object sender, System.EventArgs e)
		{
			if (assemblyName.Text == "Name.Of.Your.Assembly")
			{
				assemblyName.Text = "";
			}
		}
	}
}