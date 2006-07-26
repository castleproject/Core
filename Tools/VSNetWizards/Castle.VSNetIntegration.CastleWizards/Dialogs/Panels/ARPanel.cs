namespace Castle.VSNetIntegration.CastleWizards.Dialogs.Panels
{
	using System.Windows.Forms;
	using Castle.VSNetIntegration.Shared;

	/// <summary>
	/// Summary description for ARPanel.
	/// </summary>
	public class ARPanel : WizardPanel
	{
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox checkBoxUI;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.RadioButton radioButtonVB;
		private System.Windows.Forms.RadioButton radioButtonCS;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ARPanel()
		{
			InitializeComponent();
		}

		public bool TestOptionEnabled
		{
			set 
			{ 
				checkBoxUI.Enabled = value;
				checkBoxUI.CheckState = value ? 
					CheckState.Checked : CheckState.Indeterminate;
			}
			get { return checkBoxUI.Enabled; }
		}

		public bool WantsTestProject
		{
			get { return checkBoxUI.Checked; }
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
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.checkBoxUI = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.radioButtonVB = new System.Windows.Forms.RadioButton();
			this.radioButtonCS = new System.Windows.Forms.RadioButton();
			this.groupBox3.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.checkBoxUI);
			this.groupBox3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte) (0)));
			this.groupBox3.Location = new System.Drawing.Point(-8, 112);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(480, 78);
			this.groupBox3.TabIndex = 47;
			this.groupBox3.TabStop = false;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte) (0)));
			this.label3.Location = new System.Drawing.Point(48, 16);
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
			this.checkBoxUI.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte) (0)));
			this.checkBoxUI.Location = new System.Drawing.Point(64, 41);
			this.checkBoxUI.Name = "checkBoxUI";
			this.checkBoxUI.Size = new System.Drawing.Size(232, 24);
			this.checkBoxUI.TabIndex = 6;
			this.checkBoxUI.Text = "Yes, I\'m a &TDD enthusiast!";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.radioButtonVB);
			this.groupBox1.Controls.Add(this.radioButtonCS);
			this.groupBox1.Location = new System.Drawing.Point(-8, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(480, 112);
			this.groupBox1.TabIndex = 46;
			this.groupBox1.TabStop = false;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte) (0)));
			this.label2.Location = new System.Drawing.Point(48, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(349, 23);
			this.label2.TabIndex = 48;
			this.label2.Text = "Select a Programming Language";
			// 
			// radioButtonVB
			// 
			this.radioButtonVB.Enabled = false;
			this.radioButtonVB.Location = new System.Drawing.Point(64, 72);
			this.radioButtonVB.Name = "radioButtonVB";
			this.radioButtonVB.Size = new System.Drawing.Size(240, 24);
			this.radioButtonVB.TabIndex = 47;
			this.radioButtonVB.Text = "&Visual Basic";
			// 
			// radioButtonCS
			// 
			this.radioButtonCS.Checked = true;
			this.radioButtonCS.Location = new System.Drawing.Point(64, 40);
			this.radioButtonCS.Name = "radioButtonCS";
			this.radioButtonCS.Size = new System.Drawing.Size(240, 24);
			this.radioButtonCS.TabIndex = 46;
			this.radioButtonCS.TabStop = true;
			this.radioButtonCS.Text = "Visual &C#";
			// 
			// ARPanel
			// 
			this.BackColor = System.Drawing.SystemColors.Control;
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox1);
			this.Name = "ARPanel";
			this.Size = new System.Drawing.Size(464, 336);
			this.groupBox3.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
	}
}