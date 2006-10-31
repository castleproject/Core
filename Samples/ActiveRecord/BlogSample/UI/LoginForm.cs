// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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

namespace BlogSample.UI
{
	using System.Windows.Forms;

	public class LoginForm : Form
	{
		private GroupBox groupBox1;
		private Button logInButton;
		private Button closeButton;
		private Label label1;
		private TextBox loginText;
		private Label label2;
		private TextBox passwordText;
		private System.ComponentModel.Container components = null;

		public LoginForm()
		{
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

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.passwordText = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.loginText = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.logInButton = new System.Windows.Forms.Button();
			this.closeButton = new System.Windows.Forms.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.passwordText);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.loginText);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(16, 8);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(448, 136);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Login";
			// 
			// passwordText
			// 
			this.passwordText.Location = new System.Drawing.Point(200, 80);
			this.passwordText.Name = "passwordText";
			this.passwordText.PasswordChar = '*';
			this.passwordText.Size = new System.Drawing.Size(160, 21);
			this.passwordText.TabIndex = 3;
			this.passwordText.Text = "";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(88, 88);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 16);
			this.label2.TabIndex = 2;
			this.label2.Text = "Password:";
			// 
			// loginText
			// 
			this.loginText.Location = new System.Drawing.Point(200, 40);
			this.loginText.Name = "loginText";
			this.loginText.Size = new System.Drawing.Size(160, 21);
			this.loginText.TabIndex = 1;
			this.loginText.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(88, 48);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Login:";
			// 
			// logInButton
			// 
			this.logInButton.Location = new System.Drawing.Point(122, 160);
			this.logInButton.Name = "logInButton";
			this.logInButton.Size = new System.Drawing.Size(112, 24);
			this.logInButton.TabIndex = 1;
			this.logInButton.Text = "Log In";
			this.logInButton.Click += new System.EventHandler(this.logInButton_Click);
			// 
			// closeButton
			// 
			this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.closeButton.Location = new System.Drawing.Point(246, 160);
			this.closeButton.Name = "closeButton";
			this.closeButton.Size = new System.Drawing.Size(112, 24);
			this.closeButton.TabIndex = 2;
			this.closeButton.Text = "Close";
			this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
			// 
			// LoginForm
			// 
			this.AcceptButton = this.logInButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.closeButton;
			this.ClientSize = new System.Drawing.Size(480, 204);
			this.Controls.Add(this.closeButton);
			this.Controls.Add(this.logInButton);
			this.Controls.Add(this.groupBox1);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LoginForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Log in";
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		private void logInButton_Click(object sender, System.EventArgs e)
		{
			User user = User.FindByUserName(loginText.Text);
			
			if (user == null)
			{
				MessageBox.Show(this, "User not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			
			if (user.Password != passwordText.Text)
			{
				MessageBox.Show(this, "Wrong password", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
			
			DialogResult = DialogResult.OK;
			Hide();
		}

		private void closeButton_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Hide();
		}
	}
}
