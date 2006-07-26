namespace Castle.VSNetIntegration.CastleWizards.Dialogs.Panels
{
	using System;

	using Castle.VSNetIntegration.Shared;

	public class MRPanel : WizardPanel
	{
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox enableWindsorIntegration;
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.RadioButton veWebForms;
		private System.Windows.Forms.RadioButton veBrail;
		private System.Windows.Forms.RadioButton veNVelocity;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox enableRouting;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MRPanel()
		{
			InitializeComponent();
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
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.enableWindsorIntegration = new System.Windows.Forms.CheckBox();
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.label8 = new System.Windows.Forms.Label();
			this.veWebForms = new System.Windows.Forms.RadioButton();
			this.veBrail = new System.Windows.Forms.RadioButton();
			this.veNVelocity = new System.Windows.Forms.RadioButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label1 = new System.Windows.Forms.Label();
			this.enableRouting = new System.Windows.Forms.CheckBox();
			this.groupBox5.SuspendLayout();
			this.groupBox6.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.label7);
			this.groupBox5.Controls.Add(this.enableWindsorIntegration);
			this.groupBox5.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox5.Location = new System.Drawing.Point(-8, 128);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(480, 80);
			this.groupBox5.TabIndex = 45;
			this.groupBox5.TabStop = false;
			// 
			// label7
			// 
			this.label7.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label7.Location = new System.Drawing.Point(40, 16);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(392, 16);
			this.label7.TabIndex = 16;
			this.label7.Text = "Enable Windsor (inversion of control) integration?";
			// 
			// enableWindsorIntegration
			// 
			this.enableWindsorIntegration.Location = new System.Drawing.Point(64, 41);
			this.enableWindsorIntegration.Name = "enableWindsorIntegration";
			this.enableWindsorIntegration.Size = new System.Drawing.Size(232, 24);
			this.enableWindsorIntegration.TabIndex = 6;
			this.enableWindsorIntegration.Text = "Yes, &please";
			this.enableWindsorIntegration.CheckedChanged += new System.EventHandler(this.OnEnableIntegrationChanged);
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.label8);
			this.groupBox6.Controls.Add(this.veWebForms);
			this.groupBox6.Controls.Add(this.veBrail);
			this.groupBox6.Controls.Add(this.veNVelocity);
			this.groupBox6.Location = new System.Drawing.Point(-8, 8);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(480, 120);
			this.groupBox6.TabIndex = 44;
			this.groupBox6.TabStop = false;
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label8.Location = new System.Drawing.Point(40, 16);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(349, 23);
			this.label8.TabIndex = 44;
			this.label8.Text = "Select a View Engine";
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
			this.veBrail.Text = "Br&ail";
			// 
			// veNVelocity
			// 
			this.veNVelocity.Checked = true;
			this.veNVelocity.Location = new System.Drawing.Point(64, 40);
			this.veNVelocity.Name = "veNVelocity";
			this.veNVelocity.Size = new System.Drawing.Size(240, 24);
			this.veNVelocity.TabIndex = 41;
			this.veNVelocity.TabStop = true;
			this.veNVelocity.Text = "N&Velocity";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.enableRouting);
			this.groupBox1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox1.Location = new System.Drawing.Point(-8, 208);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(480, 104);
			this.groupBox1.TabIndex = 46;
			this.groupBox1.TabStop = false;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(40, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(392, 16);
			this.label1.TabIndex = 16;
			this.label1.Text = "More";
			// 
			// enableRouting
			// 
			this.enableRouting.Location = new System.Drawing.Point(64, 48);
			this.enableRouting.Name = "enableRouting";
			this.enableRouting.Size = new System.Drawing.Size(232, 24);
			this.enableRouting.TabIndex = 6;
			this.enableRouting.Text = "Enable &routing";
			// 
			// MRPanel
			// 
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.groupBox6);
			this.Name = "MRPanel";
			this.Size = new System.Drawing.Size(464, 336);
			this.VisibleChanged += new System.EventHandler(this.MRPanel_VisibleChanged);
			this.groupBox5.ResumeLayout(false);
			this.groupBox6.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		public bool EnableWindsorIntegration
		{
			get { return enableWindsorIntegration.Checked; }
		}

		public bool VeWebForms
		{
			get { return veWebForms.Checked; }
		}

		public bool VeBrail
		{
			get { return veBrail.Checked; }
		}

		public bool VeNVelocity
		{
			get { return veNVelocity.Checked; }
		}

		public bool EnableRouting
		{
			get { return enableRouting.Checked; }
		}

		private void OnEnableIntegrationChanged(object sender, EventArgs e)
		{
			PopulateContext();

			RaiseOnChange();
		}

		private void PopulateContext()
		{
			Context.Properties["enableWindsorIntegration"] = enableWindsorIntegration.Checked;
		}

		private void MRPanel_VisibleChanged(object sender, System.EventArgs e)
		{
			OnEnableIntegrationChanged(sender, e);
		}
	}
}