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

namespace Castle.VSNetIntegration.Shared.Dialogs
{
	using System;
	using System.Collections;
	using System.Windows.Forms;

	using EnvDTE;

	/// <summary>
	/// Summary description for WizardDialog.
	/// </summary>
	public class WizardDialog : System.Windows.Forms.Form
	{
		private readonly ExtensionContext context;
		private System.Windows.Forms.Button buttonHelp;
		private System.Windows.Forms.Button buttonBack;
		private System.Windows.Forms.Button buttonFinish;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label1;
		private wizardResult result = wizardResult.wizardResultCancel;
		private IList panels = new ArrayList();
		private int panelIndex;

		private System.ComponentModel.Container components = null;

		public WizardDialog()
		{
			InitializeComponent();
		}

		public WizardDialog(ExtensionContext context) : this()
		{
			this.context = context;
		}

		public void AddPanel(WizardPanel control)
		{
			control.SetContext(context);
			control.OnChange += new EventHandler(PanelChanged);

			control.SuspendLayout();
			control.SetBounds(152,46,464,336);

			panels.Add(control);

			Controls.Add(control);

			control.ResumeLayout(true);
		}

		protected void ShowCurrentPanel()
		{
			HideAllPanels();

			Control control = (Control) panels[panelIndex];

			control.Visible = true;

			buttonBack.Enabled = panelIndex != 0;

			PanelChanged(this, EventArgs.Empty);

//			if (panelIndex == panels.Count - 1)
//			{
//				EnableFinishButton();
//			}
//			else
//			{
//				EnableNextButton();
//			}
		}

		private void HideAllPanels()
		{
			foreach(Control control in panels)
			{
				control.Visible = false;
			}
		}

		public wizardResult WizardResult
		{
			get { return result; }
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(WizardDialog));
			this.buttonHelp = new System.Windows.Forms.Button();
			this.buttonBack = new System.Windows.Forms.Button();
			this.buttonFinish = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.SuspendLayout();
			// 
			// buttonHelp
			// 
			this.buttonHelp.Enabled = false;
			this.buttonHelp.Location = new System.Drawing.Point(176, 400);
			this.buttonHelp.Name = "buttonHelp";
			this.buttonHelp.Size = new System.Drawing.Size(72, 24);
			this.buttonHelp.TabIndex = 49;
			this.buttonHelp.Text = "&Help";
			// 
			// buttonBack
			// 
			this.buttonBack.Enabled = false;
			this.buttonBack.Location = new System.Drawing.Point(352, 400);
			this.buttonBack.Name = "buttonBack";
			this.buttonBack.Size = new System.Drawing.Size(68, 24);
			this.buttonBack.TabIndex = 46;
			this.buttonBack.Text = "<&Back";
			this.buttonBack.Click += new System.EventHandler(this.buttonBack_Click);
			// 
			// buttonFinish
			// 
			this.buttonFinish.Location = new System.Drawing.Point(424, 400);
			this.buttonFinish.Name = "buttonFinish";
			this.buttonFinish.Size = new System.Drawing.Size(68, 24);
			this.buttonFinish.TabIndex = 47;
			this.buttonFinish.Text = "&Next >";
			this.buttonFinish.Click += new System.EventHandler(this.buttonFinish_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(536, 400);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(68, 24);
			this.buttonCancel.TabIndex = 48;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.label1.Font = new System.Drawing.Font("Tahoma", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(222)));
			this.label1.ForeColor = System.Drawing.Color.FromArgb(((System.Byte)(0)), ((System.Byte)(0)), ((System.Byte)(192)));
			this.label1.Location = new System.Drawing.Point(144, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(479, 40);
			this.label1.TabIndex = 45;
			this.label1.Text = "xxxxx";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// pictureBox1
			// 
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(0, -64);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(144, 520);
			this.pictureBox1.TabIndex = 44;
			this.pictureBox1.TabStop = false;
			// 
			// WizardDialog
			// 
			this.AcceptButton = this.buttonFinish;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(624, 436);
			this.Controls.Add(this.buttonHelp);
			this.Controls.Add(this.buttonBack);
			this.Controls.Add(this.buttonFinish);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox1);
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Name = "WizardDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Castle Wizard";
			this.ResumeLayout(false);

		}

		#endregion

		public String WizardTitle
		{
			get { return label1.Text; }
			set { label1.Text = value; }
		}

		protected virtual void FinishWizard()
		{
			result = wizardResult.wizardResultSuccess;

			Hide();
		}

		protected virtual void CancelWizard()
		{
			result = wizardResult.wizardResultCancel;

			Hide();
		}

		private void buttonBack_Click(object sender, EventArgs e)
		{
			if (panelIndex > 0)
			{
				panelIndex = GetPreviousPanelIndex();

				if (panelIndex == -1)
				{
				}
				else
				{
					ShowCurrentPanel();
				}
			}
		}

		private void buttonFinish_Click(object sender, EventArgs e)
		{
			if (panelIndex < panels.Count - 1)
			{
				panelIndex = GetNextPanelIndex();

				if (panelIndex == -1)
				{
					FinishWizard();
				}
				else
				{
					ShowCurrentPanel();
				}
			}
			else
			{
				FinishWizard();
			}
		}

		private int GetPreviousPanelIndex()
		{
			for(int i=panelIndex - 1; i >= 0; i--)
			{
				WizardPanel panel = (WizardPanel) panels[i];

				if (panel.WantsToShow(context))
				{
					return i;
				}
			}

			return -1;
		}

		private int GetNextPanelIndex()
		{
			for(int i=panelIndex + 1; i < panels.Count; i++)
			{
				WizardPanel panel = (WizardPanel) panels[i];

				if (panel.WantsToShow(context))
				{
					return i;
				}
			}

			return -1;
		}

		private void buttonCancel_Click(object sender, EventArgs e)
		{
			CancelWizard();
		}

		/// <summary>
		/// Fired by panels
		/// </summary>
		private void PanelChanged(object sender, EventArgs e)
		{
			if (GetNextPanelIndex() == -1)
			{
				EnableFinishButton();	
			}
			else
			{
				EnableNextButton();
			}
		}

		private void EnableNextButton()
		{
			buttonFinish.Text = "&Next >";
		}

		private void EnableFinishButton()
		{
			buttonFinish.Text = "&Finish";
		}
	}
}