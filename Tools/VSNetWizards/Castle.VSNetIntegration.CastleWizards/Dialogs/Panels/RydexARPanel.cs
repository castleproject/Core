namespace Castle.VSNetIntegration.CastleWizards.Dialogs.Panels
{
	using System.Windows.Forms;

	/// <summary>
	/// Summary description for RydexARPanel.
	/// </summary>
	public class RydexARPanel : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.CheckBox checkBoxRydexAR;
		private System.Windows.Forms.Label label1;
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public RydexARPanel()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call

		}

		public bool UseRydexAR
		{
			get { return checkBoxRydexAR.Checked; }
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
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.checkBoxRydexAR = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label1);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.checkBoxRydexAR);
			this.groupBox3.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.groupBox3.Location = new System.Drawing.Point(-8, 0);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(480, 168);
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
			this.label3.Text = "Would you like to use Rydex.Framework.ActiveRecord?";
			// 
			// checkBoxRydexAR
			// 
			this.checkBoxRydexAR.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.checkBoxRydexAR.Location = new System.Drawing.Point(64, 41);
			this.checkBoxRydexAR.Name = "checkBoxRydexAR";
			this.checkBoxRydexAR.Size = new System.Drawing.Size(232, 24);
			this.checkBoxRydexAR.TabIndex = 6;
			this.checkBoxRydexAR.Text = "Yes, please";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(32, 96);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(416, 56);
			this.label1.TabIndex = 17;
			this.label1.Text = "Rydex.Framework.ActiveRecord is an extension to ActiveRecord that adds common fun" +
				"ctionality and yada yada yada";
			// 
			// RydexARPanel
			// 
			this.Controls.Add(this.groupBox3);
			this.Name = "RydexARPanel";
			this.Size = new System.Drawing.Size(464, 336);
			this.groupBox3.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
