namespace Castle.VSNetIntegration.CastleWizards.Dialogs.Panels
{
	using System;
	using System.Windows.Forms;

	using Castle.VSNetIntegration.Shared;

	/// <summary>
	/// Summary description for ContainerIntegrationPanel.
	/// </summary>
	public class ContainerIntegrationPanel : WizardPanel
	{
		private System.Windows.Forms.GroupBox groupBox6;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.CheckedListBox checkedListBox1;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ContainerIntegrationPanel()
		{
			InitializeComponent();
		}

		public override bool WantsToShow(ExtensionContext context)
		{
			return ((bool) context.Properties["enableWindsorIntegration"]) == true;
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
			this.groupBox6 = new System.Windows.Forms.GroupBox();
			this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
			this.label8 = new System.Windows.Forms.Label();
			this.groupBox6.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox6
			// 
			this.groupBox6.Controls.Add(this.checkedListBox1);
			this.groupBox6.Controls.Add(this.label8);
			this.groupBox6.Location = new System.Drawing.Point(-8, 8);
			this.groupBox6.Name = "groupBox6";
			this.groupBox6.Size = new System.Drawing.Size(480, 320);
			this.groupBox6.TabIndex = 45;
			this.groupBox6.TabStop = false;
			// 
			// checkedListBox1
			// 
			this.checkedListBox1.CheckOnClick = true;
			this.checkedListBox1.Items.AddRange(new object[] {
																 "ActiveRecord Integration",
																 "NHibernate Integration",
																 "Automatic Transaction Management",
																 "Logging"});
			this.checkedListBox1.Location = new System.Drawing.Point(74, 64);
			this.checkedListBox1.Name = "checkedListBox1";
			this.checkedListBox1.Size = new System.Drawing.Size(332, 214);
			this.checkedListBox1.TabIndex = 45;
			this.checkedListBox1.ThreeDCheckBoxes = true;
			this.checkedListBox1.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox1_ItemCheck_1);
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label8.Location = new System.Drawing.Point(40, 16);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(349, 23);
			this.label8.TabIndex = 44;
			this.label8.Text = "Do you want to enable any integration?";
			// 
			// ContainerIntegrationPanel
			// 
			this.Controls.Add(this.groupBox6);
			this.Name = "ContainerIntegrationPanel";
			this.Size = new System.Drawing.Size(464, 336);
			this.groupBox6.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private void checkedListBox1_ItemCheck_1(object sender, ItemCheckEventArgs e)
		{
			String key = (String) checkedListBox1.Items[e.Index];

			Context.Properties[ key ] = e.NewValue == CheckState.Checked;

			RaiseOnChange();
		}
	}
}