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
	using Castle.VSNetIntegration.CastleWizards.Shared;

	[System.Runtime.InteropServices.ComVisible(false)]
	public class MRTestPanel : WizardPanel
	{
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox checkBoxTest;
		private System.ComponentModel.IContainer components = null;

		public MRTestPanel()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}
		
		public bool WantsTestProject
		{
			get { return checkBoxTest.Checked; }
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.checkBoxTest = new System.Windows.Forms.CheckBox();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.checkBoxTest);
			this.groupBox3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox3.Location = new System.Drawing.Point(-8, 10);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(480, 104);
			this.groupBox3.TabIndex = 48;
			this.groupBox3.TabStop = false;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label3.Location = new System.Drawing.Point(48, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(416, 16);
			this.label3.TabIndex = 16;
			this.label3.Text = "Would you like to create a test project?";
			// 
			// checkBoxTest
			// 
			this.checkBoxTest.Checked = true;
			this.checkBoxTest.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxTest.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.checkBoxTest.Location = new System.Drawing.Point(64, 56);
			this.checkBoxTest.Name = "checkBoxTest";
			this.checkBoxTest.Size = new System.Drawing.Size(232, 24);
			this.checkBoxTest.TabIndex = 6;
			this.checkBoxTest.Text = "Yes, I\'m a &TDD enthusiast!";
			// 
			// MRTestPanel
			// 
			this.Controls.Add(this.groupBox3);
			this.Name = "MRTestPanel";
			this.Size = new System.Drawing.Size(464, 336);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}

