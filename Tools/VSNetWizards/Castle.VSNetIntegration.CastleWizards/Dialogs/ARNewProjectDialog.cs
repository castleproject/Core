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

namespace Castle.VSNetIntegration.CastleWizards.Dialogs
{
	using System.Runtime.InteropServices;
	using EnvDTE;

	/// <summary>
	/// Summary description for ARNewProjectDialog.
	/// </summary>
	[ComVisible(false)]
	public class ARNewProjectDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Button buttonHelp;
		private System.Windows.Forms.Button buttonBack;
		private System.Windows.Forms.Button buttonFinish;
		private System.Windows.Forms.Button buttonCancel;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox checkBoxUI;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RadioButton radioButtonVB;
		private System.Windows.Forms.RadioButton radioButtonCS;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;

		private wizardResult result = wizardResult.wizardResultCancel;

		public ARNewProjectDialog()
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ARNewProjectDialog));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.buttonHelp = new System.Windows.Forms.Button();
			this.buttonBack = new System.Windows.Forms.Button();
			this.buttonFinish = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.checkBoxUI = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.radioButtonVB = new System.Windows.Forms.RadioButton();
			this.radioButtonCS = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.panel1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(144, 448);
			this.pictureBox1.TabIndex = 35;
			this.pictureBox1.TabStop = false;
			// 
			// buttonHelp
			// 
			this.buttonHelp.Enabled = false;
			this.buttonHelp.Location = new System.Drawing.Point(168, 384);
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.Size = new System.Drawing.Size(72, 24);
			this.buttonHelp.TabIndex = 30;
			this.buttonHelp.Text = "&Help";
			// 
			// buttonBack
			// 
			this.buttonBack.Enabled = false;
			this.buttonBack.Location = new System.Drawing.Point(344, 384);
			this.buttonBack.Name = "buttonBack";
			this.buttonBack.Size = new System.Drawing.Size(68, 24);
			this.buttonBack.TabIndex = 26;
			this.buttonBack.Text = "<&Back";
			// 
			// buttonFinish
			// 
			this.buttonFinish.Location = new System.Drawing.Point(416, 384);
			this.buttonFinish.Name = "buttonFinish";
			this.buttonFinish.Size = new System.Drawing.Size(68, 24);
			this.buttonFinish.TabIndex = 27;
			this.buttonFinish.Text = "&Finish";
			this.buttonFinish.Click += new System.EventHandler(this.buttonFinish_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(528, 384);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(68, 24);
			this.buttonCancel.TabIndex = 28;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.SystemColors.Control;
			this.panel1.Controls.Add(this.groupBox3);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.radioButtonVB);
			this.panel1.Controls.Add(this.radioButtonCS);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Location = new System.Drawing.Point(144, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(472, 368);
			this.panel1.TabIndex = 37;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.checkBoxUI);
			this.groupBox3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox3.Location = new System.Drawing.Point(0, 152);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(467, 78);
			this.groupBox3.TabIndex = 42;
			this.groupBox3.TabStop = false;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label3.Location = new System.Drawing.Point(40, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(416, 16);
			this.label3.TabIndex = 16;
			this.label3.Text = "Would you like to create a test project as well?";
			// 
			// checkBoxUI
			// 
			this.checkBoxUI.Checked = true;
			this.checkBoxUI.CheckState = System.Windows.Forms.CheckState.Indeterminate;
			this.checkBoxUI.Enabled = false;
			this.checkBoxUI.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.checkBoxUI.Location = new System.Drawing.Point(64, 41);
			this.checkBoxUI.Name = "checkBoxUI";
			this.checkBoxUI.Size = new System.Drawing.Size(232, 24);
			this.checkBoxUI.TabIndex = 6;
			this.checkBoxUI.Text = "Yes, I\'m a &TDD enthusiast!";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(40, 56);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(349, 23);
			this.label2.TabIndex = 40;
			this.label2.Text = "Select a Programming Language";
			// 
			// radioButtonVB
			// 
			this.radioButtonVB.Enabled = false;
			this.radioButtonVB.Location = new System.Drawing.Point(56, 112);
			this.radioButtonVB.Name = "radioButtonVB";
			this.radioButtonVB.Size = new System.Drawing.Size(240, 24);
			this.radioButtonVB.TabIndex = 39;
			this.radioButtonVB.Text = "&Visual Basic";
			// 
			// radioButtonCS
			// 
			this.radioButtonCS.Checked = true;
			this.radioButtonCS.Location = new System.Drawing.Point(56, 80);
			this.radioButtonCS.Name = "radioButtonCS";
			this.radioButtonCS.Size = new System.Drawing.Size(240, 24);
			this.radioButtonCS.TabIndex = 38;
			this.radioButtonCS.TabStop = true;
			this.radioButtonCS.Text = "Visual &C#";
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.label1.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(222)));
			this.label1.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(0)), ((System.Byte)(192)));
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(472, 40);
			this.label1.TabIndex = 37;
			this.label1.Text = "New ActiveRecord project";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBox1
			// 
			this.groupBox1.Location = new System.Drawing.Point(0, 40);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(467, 112);
			this.groupBox1.TabIndex = 41;
			this.groupBox1.TabStop = false;
			// 
			// ARNewProjectDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(614, 420);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.buttonHelp);
			this.Controls.Add(this.buttonBack);
			this.Controls.Add(this.buttonFinish);
			this.Controls.Add(this.buttonCancel);
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ARNewProjectDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New ActiveRecord project wizard";
			this.panel1.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonFinish_Click(object sender, System.EventArgs e)
		{
			result = wizardResult.wizardResultSuccess;
			Hide();
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			Hide();
		}

		public wizardResult WizardResult
		{
			get { return result; }
		}
	}
}
