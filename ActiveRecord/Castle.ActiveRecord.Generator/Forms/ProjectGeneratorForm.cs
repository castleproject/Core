namespace Castle.ActiveRecord.Generator.Forms
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;

	/// <summary>
	/// Summary description for ProjectGeneratorForm.
	/// </summary>
	public class ProjectGeneratorForm : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox outputDir;
		private System.Windows.Forms.Button browseButton;
		private System.Windows.Forms.Button generateButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public ProjectGeneratorForm()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label2 = new System.Windows.Forms.Label();
			this.outputDir = new System.Windows.Forms.TextBox();
			this.browseButton = new System.Windows.Forms.Button();
			this.generateButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(20, 136);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(448, 20);
			this.label1.TabIndex = 0;
			this.label1.Text = "Warning: The process might overwrite modification on the classes";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.browseButton);
			this.groupBox1.Controls.Add(this.outputDir);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Location = new System.Drawing.Point(8, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(464, 116);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Info:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 36);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(236, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "Output &directory:";
			// 
			// outputDir
			// 
			this.outputDir.Location = new System.Drawing.Point(16, 56);
			this.outputDir.Name = "outputDir";
			this.outputDir.Size = new System.Drawing.Size(400, 21);
			this.outputDir.TabIndex = 1;
			this.outputDir.Text = "";
			// 
			// browseButton
			// 
			this.browseButton.Location = new System.Drawing.Point(420, 56);
			this.browseButton.Name = "browseButton";
			this.browseButton.Size = new System.Drawing.Size(32, 24);
			this.browseButton.TabIndex = 2;
			this.browseButton.Text = "...";
			this.browseButton.Click += new System.EventHandler(this.browseButton_Click);
			// 
			// generateButton
			// 
			this.generateButton.Location = new System.Drawing.Point(480, 16);
			this.generateButton.Name = "generateButton";
			this.generateButton.Size = new System.Drawing.Size(84, 23);
			this.generateButton.TabIndex = 2;
			this.generateButton.Text = "&Generate";
			this.generateButton.Click += new System.EventHandler(this.generateButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(480, 48);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(84, 23);
			this.cancelButton.TabIndex = 3;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// ProjectGeneratorForm
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(570, 168);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.generateButton);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.label1);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.Name = "ProjectGeneratorForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Project Generator";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void browseButton_Click(object sender, System.EventArgs e)
		{
			folderBrowserDialog1.SelectedPath = outputDir.Text;
			
			if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
			{
				outputDir.Text = folderBrowserDialog1.SelectedPath;
			}
		}

		private void generateButton_Click(object sender, System.EventArgs e)
		{
		
		}

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
		}
	}
}
