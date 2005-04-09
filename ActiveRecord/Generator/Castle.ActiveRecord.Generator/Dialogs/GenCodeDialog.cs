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

namespace Castle.ActiveRecord.Generator.Dialogs
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.IO;
	using System.Windows.Forms;

	using Castle.ActiveRecord.Generator.Components.CodeGenerator;

	/// <summary>
	/// Summary description for GenCodeDialog.
	/// </summary>
	public class GenCodeDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.TextBox ns;
		private System.Windows.Forms.TextBox outDir;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.CheckBox overwriteCheck;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
		private System.Windows.Forms.ComboBox languageCombo;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private ICodeProviderFactory codeproviderFactory;
		private Model _model;


		protected GenCodeDialog()
		{
			codeproviderFactory = 
				ServiceRegistry.Instance[ typeof(ICodeProviderFactory) ] as ICodeProviderFactory;

			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			languageCombo.ValueMember = "Label";

			foreach(CodeProviderInfo info in codeproviderFactory.GetAvailableProviders())
			{
				languageCombo.Items.Add(info);
			}

			languageCombo.SelectedIndex = 0;
		}

		public GenCodeDialog(Model model) : this()
		{
			_model = model;

			ns.Text = model.CurrentProject.Namespace;
			outDir.Text = model.CurrentProject.LastOutDir;
			overwriteCheck.Checked = model.CurrentProject.OverwriteFiles;
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.languageCombo = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.overwriteCheck = new System.Windows.Forms.CheckBox();
			this.button3 = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.outDir = new System.Windows.Forms.TextBox();
			this.ns = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.languageCombo);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.overwriteCheck);
			this.groupBox1.Controls.Add(this.button3);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.outDir);
			this.groupBox1.Controls.Add(this.ns);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(16, 16);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(424, 184);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Options:";
			// 
			// languageCombo
			// 
			this.languageCombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.languageCombo.Location = new System.Drawing.Point(104, 96);
			this.languageCombo.Name = "languageCombo";
			this.languageCombo.Size = new System.Drawing.Size(200, 21);
			this.languageCombo.TabIndex = 7;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 96);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(80, 23);
			this.label3.TabIndex = 6;
			this.label3.Text = "Language:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// overwriteCheck
			// 
			this.overwriteCheck.Location = new System.Drawing.Point(104, 136);
			this.overwriteCheck.Name = "overwriteCheck";
			this.overwriteCheck.Size = new System.Drawing.Size(208, 24);
			this.overwriteCheck.TabIndex = 5;
			this.overwriteCheck.Text = "Overwrite existing files";
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(368, 64);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(32, 24);
			this.button3.TabIndex = 4;
			this.button3.Text = "...";
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 23);
			this.label2.TabIndex = 3;
			this.label2.Text = "Output dir:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// outDir
			// 
			this.outDir.Location = new System.Drawing.Point(104, 64);
			this.outDir.Name = "outDir";
			this.outDir.Size = new System.Drawing.Size(256, 21);
			this.outDir.TabIndex = 2;
			this.outDir.Text = "";
			// 
			// ns
			// 
			this.ns.Location = new System.Drawing.Point(104, 32);
			this.ns.Name = "ns";
			this.ns.Size = new System.Drawing.Size(296, 21);
			this.ns.TabIndex = 1;
			this.ns.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(80, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Namespace:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button1.Location = new System.Drawing.Point(368, 216);
			this.button1.Name = "button1";
			this.button1.TabIndex = 1;
			this.button1.Text = "Generate";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.Location = new System.Drawing.Point(280, 216);
			this.button2.Name = "button2";
			this.button2.TabIndex = 2;
			this.button2.Text = "Close";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// GenCodeDialog
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.button2;
			this.ClientSize = new System.Drawing.Size(456, 250);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.groupBox1);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "GenCodeDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Generate Code Options";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void button3_Click(object sender, System.EventArgs e)
		{
			folderBrowserDialog1.SelectedPath = outDir.Text;
			
			if (folderBrowserDialog1.ShowDialog(this) == DialogResult.OK)
			{
				outDir.Text = folderBrowserDialog1.SelectedPath;
			}
		}

		private void button2_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void button1_Click(object sender, System.EventArgs e)
		{
			DirectoryInfo dirInfo = new DirectoryInfo(outDir.Text);

			if (outDir.Text == String.Empty)
			{
				MessageBox.Show(this, "You must specify an output directory.", "Field is required", MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			if (!dirInfo.Exists)
			{
				if (MessageBox.Show(this, "Output directory does not exists. Create it?", "Question", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					dirInfo.Create();
				}
				else
				{
					return;
				}
			}

			_model.CurrentProject.CodeInfo = languageCombo.SelectedItem as CodeProviderInfo;
			_model.CurrentProject.LastOutDir = outDir.Text;
			_model.CurrentProject.Namespace = ns.Text;
			_model.CurrentProject.OverwriteFiles = overwriteCheck.Checked;

			DialogResult = DialogResult.OK;
			Close();
		}
	}
}
