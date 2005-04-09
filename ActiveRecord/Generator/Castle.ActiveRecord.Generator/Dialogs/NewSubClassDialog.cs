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
	using System.Windows.Forms;

	/// <summary>
	/// Summary description for NewSubClassDialog.
	/// </summary>
	public class NewSubClassDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Label cl;
		private System.Windows.Forms.TextBox className;
		private System.Windows.Forms.TextBox discValue;
		private System.Windows.Forms.Label label2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public NewSubClassDialog()
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
			this.okButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.cl = new System.Windows.Forms.Label();
			this.className = new System.Windows.Forms.TextBox();
			this.discValue = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(328, 152);
			this.okButton.Name = "okButton";
			this.okButton.TabIndex = 0;
			this.okButton.Text = "OK";
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(240, 152);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// cl
			// 
			this.cl.Location = new System.Drawing.Point(48, 32);
			this.cl.Name = "cl";
			this.cl.Size = new System.Drawing.Size(120, 23);
			this.cl.TabIndex = 2;
			this.cl.Text = "Class name:";
			this.cl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// className
			// 
			this.className.Location = new System.Drawing.Point(184, 32);
			this.className.Name = "className";
			this.className.Size = new System.Drawing.Size(192, 21);
			this.className.TabIndex = 3;
			this.className.Text = "";
			// 
			// discValue
			// 
			this.discValue.Location = new System.Drawing.Point(184, 72);
			this.discValue.Name = "discValue";
			this.discValue.Size = new System.Drawing.Size(192, 21);
			this.discValue.TabIndex = 5;
			this.discValue.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(48, 72);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(120, 23);
			this.label2.TabIndex = 4;
			this.label2.Text = "Discriminator Value:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// NewSubClassDialog
			// 
			this.AcceptButton = this.okButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(426, 188);
			this.Controls.Add(this.discValue);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.className);
			this.Controls.Add(this.cl);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.okButton);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.Name = "NewSubClassDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New SubClass";
			this.ResumeLayout(false);

		}
		#endregion

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void okButton_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		public String ClassName
		{
			get { return className.Text; }
		}

		public String DiscriminatorValue
		{
			get { return discValue.Text; }
		}
	}
}
