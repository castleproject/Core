using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace Castle.Facilities.ActiveRecordGenerator.Forms
{
	public class LocateAssembly : System.Windows.Forms.Form
	{
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button browseButton;
		private System.Windows.Forms.TextBox location;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox assemblyName;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private String _assemblyName;

		public LocateAssembly(String assemblyName)
		{
			_assemblyName = assemblyName;

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

		private void browseButton_Click(object sender, System.EventArgs e)
		{
			folderBrowserDialog1.SelectedPath = AppDomain.CurrentDomain.BaseDirectory;

			if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
			{
				location.Text = folderBrowserDialog1.SelectedPath;
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.browseButton = new System.Windows.Forms.Button();
			this.location = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.assemblyName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.browseButton);
			this.groupBox1.Controls.Add(this.location);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.assemblyName);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(8, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(488, 168);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Assembly Resolve:";
			// 
			// browseButton
			// 
			this.browseButton.Location = new System.Drawing.Point(400, 124);
			this.browseButton.Name = "browseButton";
			this.browseButton.Size = new System.Drawing.Size(75, 24);
			this.browseButton.TabIndex = 9;
			this.browseButton.Text = "&Browse...";
			this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
			// 
			// location
			// 
			this.location.Location = new System.Drawing.Point(16, 124);
			this.location.Name = "location";
			this.location.Size = new System.Drawing.Size(376, 21);
			this.location.TabIndex = 8;
			this.location.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 104);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 18);
			this.label2.TabIndex = 7;
			this.label2.Text = "Location:";
			// 
			// assemblyName
			// 
			this.assemblyName.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.assemblyName.Location = new System.Drawing.Point(16, 52);
			this.assemblyName.Multiline = true;
			this.assemblyName.Name = "assemblyName";
			this.assemblyName.ReadOnly = true;
			this.assemblyName.Size = new System.Drawing.Size(456, 42);
			this.assemblyName.TabIndex = 6;
			this.assemblyName.Text = "";
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(16, 28);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(360, 18);
			this.label1.TabIndex = 5;
			this.label1.Text = "Could not find the following assembly:";
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(420, 188);
			this.okButton.Name = "okButton";
			this.okButton.TabIndex = 6;
			this.okButton.Text = "OK";
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(336, 188);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.TabIndex = 7;
			this.cancelButton.Text = "Cancel";
			// 
			// LocateAssembly
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 14);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(508, 218);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Controls.Add(this.groupBox1);
			this.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.Name = "LocateAssembly";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Could not resolve assembly";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion
	}
}
