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

namespace Castle.VSNetIntegration.CastleWizards
{
	using System;
	using System.Runtime.InteropServices;

	using EnvDTE;

	/// <summary>
	/// Summary description for MRNewProjectDialog.
	/// </summary>
	[ComVisible(false)]
	public class MRNewProjectDialog : System.Windows.Forms.Form
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
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RadioButton radioButtonVB;
		private System.Windows.Forms.RadioButton radioButtonCS;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.RadioButton radioButton1;
		private System.Windows.Forms.RadioButton radioButton2;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.RadioButton radioButton3;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckedListBox checkedListBox1;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Label facilitylabel;

		private wizardResult result = wizardResult.wizardResultCancel;
		private System.Windows.Forms.CheckBox enableWindsorIntegration;
		private System.Windows.Forms.CheckBox createTestProject;
		private System.Windows.Forms.RadioButton veWebForms;
		private System.Windows.Forms.RadioButton veBrail;
		private System.Windows.Forms.RadioButton veNVelocity;
		private bool isStep1;

		public System.Windows.Forms.CheckedListBox facilities;

		public MRNewProjectDialog()
		{
			InitializeComponent();

			SetStep1();
		}

		private void SetStep1()
		{
			isStep1 = true;
			panel1.Visible = true;
			panel2.Visible = false;
			buttonFinish.Text = "&Next >";
			buttonBack.Enabled = false;
		}

		private void SetStep2()
		{
			enableWindsorIntegration_CheckedChanged(this, EventArgs.Empty);

			isStep1 = false;
			panel1.Visible = false;
			panel2.Visible = true;
			buttonFinish.Text = "&Finish";
			buttonBack.Enabled = true;
		}

		public bool CreateTestProject
		{
			get { return createTestProject.Checked; }
		}

		public bool EnableWindsorIntegration
		{
			get { return enableWindsorIntegration.Checked; }
		}

		public bool UseNVelocity
		{
			get { return veNVelocity.Checked; }
		}

		public bool UseBrail
		{
			get { return veBrail.Checked; }
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(MRNewProjectDialog));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.buttonHelp = new System.Windows.Forms.Button();
			this.buttonBack = new System.Windows.Forms.Button();
			this.buttonFinish = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.createTestProject = new System.Windows.Forms.CheckBox();
			this.label2 = new System.Windows.Forms.Label();
			this.radioButtonVB = new System.Windows.Forms.RadioButton();
			this.radioButtonCS = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.label5 = new System.Windows.Forms.Label();
			this.radioButton1 = new System.Windows.Forms.RadioButton();
			this.radioButton2 = new System.Windows.Forms.RadioButton();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.radioButton3 = new System.Windows.Forms.RadioButton();
			this.label6 = new System.Windows.Forms.Label();
			this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
			this.panel2 = new System.Windows.Forms.Panel();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.facilities = new System.Windows.Forms.CheckedListBox();
			this.facilitylabel = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.enableWindsorIntegration = new System.Windows.Forms.CheckBox();
			this.label8 = new System.Windows.Forms.Label();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.veWebForms = new System.Windows.Forms.RadioButton();
			this.veBrail = new System.Windows.Forms.RadioButton();
			this.veNVelocity = new System.Windows.Forms.RadioButton();
			this.panel1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.panel2.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox6.SuspendLayout();
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
			this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
			// 
			// buttonFinish
			// 
			this.buttonFinish.Location = new System.Drawing.Point(416, 384);
			this.buttonFinish.Name = "buttonFinish";
			this.buttonFinish.Size = new System.Drawing.Size(68, 24);
			this.buttonFinish.TabIndex = 27;
			this.buttonFinish.Text = "&Next >";
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
			this.panel1.Controls.Add(this.groupBox1);
			this.panel1.Location = new System.Drawing.Point(144, 40);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(472, 320);
			this.panel1.TabIndex = 37;
			this.panel1.Visible = false;
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.createTestProject);
			this.groupBox3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox3.Location = new System.Drawing.Point(0, 112);
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
			// createTestProject
			// 
			this.createTestProject.Location = new System.Drawing.Point(64, 41);
			this.createTestProject.Name = "createTestProject";
			this.createTestProject.Size = new System.Drawing.Size(320, 24);
			this.createTestProject.TabIndex = 6;
			this.createTestProject.Text = "Yes, I\'m a &TDD enthusiast!";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(40, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(349, 23);
			this.label2.TabIndex = 40;
			this.label2.Text = "Select a Programming Language";
			// 
			// radioButtonVB
			// 
			this.radioButtonVB.Enabled = false;
			this.radioButtonVB.Location = new System.Drawing.Point(56, 72);
			this.radioButtonVB.Name = "radioButtonVB";
			this.radioButtonVB.Size = new System.Drawing.Size(240, 24);
			this.radioButtonVB.TabIndex = 39;
			this.radioButtonVB.Text = "&Visual Basic";
			// 
			// radioButtonCS
			// 
			this.radioButtonCS.Checked = true;
			this.radioButtonCS.Location = new System.Drawing.Point(56, 40);
			this.radioButtonCS.Name = "radioButtonCS";
			this.radioButtonCS.Size = new System.Drawing.Size(240, 24);
			this.radioButtonCS.TabIndex = 38;
			this.radioButtonCS.TabStop = true;
			this.radioButtonCS.Text = "Visual &C#";
			// 
			// groupBox1
			// 
			this.groupBox1.Location = new System.Drawing.Point(0, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(467, 112);
			this.groupBox1.TabIndex = 41;
			this.groupBox1.TabStop = false;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.label1.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(222)));
			this.label1.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(0)), ((System.Byte)(192)));
			this.label1.Location = new System.Drawing.Point(144, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(472, 40);
			this.label1.TabIndex = 38;
			this.label1.Text = "New MonoRail project";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// groupBox2
			// 
			this.groupBox2.Location = new System.Drawing.Point(0, 0);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabIndex = 0;
			this.groupBox2.TabStop = false;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(0, 0);
			this.label4.Name = "label4";
			this.label4.TabIndex = 0;
			// 
			// checkBox1
			// 
			this.checkBox1.Location = new System.Drawing.Point(0, 0);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.TabIndex = 0;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(0, 0);
			this.label5.Name = "label5";
			this.label5.TabIndex = 0;
			// 
			// radioButton1
			// 
			this.radioButton1.Location = new System.Drawing.Point(0, 0);
			this.radioButton1.Name = "radioButton1";
			this.radioButton1.TabIndex = 0;
			// 
			// radioButton2
			// 
			this.radioButton2.Location = new System.Drawing.Point(0, 0);
			this.radioButton2.Name = "radioButton2";
			this.radioButton2.TabIndex = 0;
			// 
			// groupBox4
			// 
			this.groupBox4.Location = new System.Drawing.Point(0, 0);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.TabIndex = 0;
			this.groupBox4.TabStop = false;
			// 
			// radioButton3
			// 
			this.radioButton3.Location = new System.Drawing.Point(0, 0);
			this.radioButton3.Name = "radioButton3";
			this.radioButton3.TabIndex = 0;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(0, 0);
			this.label6.Name = "label6";
			this.label6.TabIndex = 0;
			// 
			// checkedListBox1
			// 
			this.checkedListBox1.Location = new System.Drawing.Point(0, 0);
			this.checkedListBox1.Name = "checkedListBox1";
			this.checkedListBox1.TabIndex = 0;
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.SystemColors.Control;
			this.panel2.Controls.Add(this.groupBox5);
			this.panel2.Controls.Add(this.label8);
			this.panel2.Controls.Add(this.groupBox6);
			this.panel2.Location = new System.Drawing.Point(144, 40);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(472, 320);
			this.panel2.TabIndex = 39;
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.facilities);
			this.groupBox5.Controls.Add(this.facilitylabel);
			this.groupBox5.Controls.Add(this.label7);
			this.groupBox5.Controls.Add(this.enableWindsorIntegration);
			this.groupBox5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox5.Location = new System.Drawing.Point(0, 120);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(467, 200);
			this.groupBox5.TabIndex = 42;
			this.groupBox5.TabStop = false;
			// 
			// facilities
			// 
			this.facilities.Items.AddRange(new object[] {
															"ActiveRecord Integration",
															"NHibernate Integration",
															"Automatic Transaction Management",
															"Logging"});
			this.facilities.Location = new System.Drawing.Point(64, 88);
			this.facilities.Name = "facilities";
			this.facilities.Size = new System.Drawing.Size(328, 100);
			this.facilities.TabIndex = 19;
			// 
			// facilitylabel
			// 
			this.facilitylabel.Location = new System.Drawing.Point(64, 72);
			this.facilitylabel.Name = "facilitylabel";
			this.facilitylabel.Size = new System.Drawing.Size(352, 16);
			this.facilitylabel.TabIndex = 18;
			this.facilitylabel.Text = "..and enable the following facilities/integrations:";
			// 
			// label7
			// 
			this.label7.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label7.Location = new System.Drawing.Point(40, 16);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(416, 16);
			this.label7.TabIndex = 16;
			this.label7.Text = "Enable Windsor (inversion of control) integration?";
			// 
			// enableWindsorIntegration
			// 
			this.enableWindsorIntegration.Location = new System.Drawing.Point(64, 41);
			this.enableWindsorIntegration.Name = "enableWindsorIntegration";
			this.enableWindsorIntegration.Size = new System.Drawing.Size(232, 24);
			this.enableWindsorIntegration.TabIndex = 6;
			this.enableWindsorIntegration.Text = "Yes, please";
			this.enableWindsorIntegration.CheckedChanged += new System.EventHandler(this.enableWindsorIntegration_CheckedChanged);
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label8.Location = new System.Drawing.Point(40, 16);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(349, 23);
			this.label8.TabIndex = 40;
			this.label8.Text = "Select a View Engine";
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.veWebForms);
			this.groupBox6.Controls.Add(this.veBrail);
			this.groupBox6.Controls.Add(this.veNVelocity);
			this.groupBox6.Location = new System.Drawing.Point(0, 0);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(467, 120);
			this.groupBox6.TabIndex = 41;
			this.groupBox6.TabStop = false;
			// 
			// veWebForms
			// 
			this.veWebForms.Location = new System.Drawing.Point(64, 88);
			this.veWebForms.Name = "veWebForms";
			this.veWebForms.Size = new System.Drawing.Size(240, 24);
			this.veWebForms.TabIndex = 43;
			this.veWebForms.Text = "ASP.Net Web Forms";
			// 
			// veBrail
			// 
			this.veBrail.Location = new System.Drawing.Point(64, 64);
			this.veBrail.Name = "veBrail";
			this.veBrail.Size = new System.Drawing.Size(240, 24);
			this.veBrail.TabIndex = 42;
			this.veBrail.Text = "Brail";
			// 
			// veNVelocity
			// 
			this.veNVelocity.Checked = true;
			this.veNVelocity.Location = new System.Drawing.Point(64, 40);
			this.veNVelocity.Name = "veNVelocity";
			this.veNVelocity.Size = new System.Drawing.Size(240, 24);
			this.veNVelocity.TabIndex = 41;
			this.veNVelocity.TabStop = true;
			this.veNVelocity.Text = "NVelocity";
			// 
			// MRNewProjectDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.ClientSize = new System.Drawing.Size(614, 420);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.buttonHelp);
			this.Controls.Add(this.buttonBack);
			this.Controls.Add(this.buttonFinish);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "MRNewProjectDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New MonoRail project wizard";
			this.panel1.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void buttonFinish_Click(object sender, System.EventArgs e)
		{
			if (isStep1)
			{
				SetStep2();
			}
			else
			{
				result = wizardResult.wizardResultSuccess;
				Hide();
			}
		}

		private void buttonCancel_Click(object sender, System.EventArgs e)
		{
			Hide();
		}

		private void buttonBack_Click(object sender, System.EventArgs e)
		{
			SetStep1();
		}

		private void enableWindsorIntegration_CheckedChanged(object sender, System.EventArgs e)
		{
			facilities.Enabled = enableWindsorIntegration.Checked;
			facilitylabel.Enabled = enableWindsorIntegration.Checked;
		}

		public wizardResult WizardResult
		{
			get { return result; }
		}
	}
}
