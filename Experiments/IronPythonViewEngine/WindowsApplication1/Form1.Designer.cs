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

namespace WindowsApplication1
{
	partial class Form1
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabPage1 = new System.Windows.Forms.TabPage();
			this.tabPage2 = new System.Windows.Forms.TabPage();
			this.functions = new System.Windows.Forms.TextBox();
			this.script = new System.Windows.Forms.TextBox();
			this.output = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.tabControl1.SuspendLayout();
			this.tabPage1.SuspendLayout();
			this.tabPage2.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabPage1);
			this.tabControl1.Controls.Add(this.tabPage2);
			this.tabControl1.Location = new System.Drawing.Point(10, 12);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(762, 431);
			this.tabControl1.TabIndex = 0;
			// 
			// tabPage1
			// 
			this.tabPage1.Controls.Add(this.functions);
			this.tabPage1.Location = new System.Drawing.Point(4, 22);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(754, 405);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "functions";
			this.tabPage1.UseVisualStyleBackColor = true;
			// 
			// tabPage2
			// 
			this.tabPage2.Controls.Add(this.script);
			this.tabPage2.Location = new System.Drawing.Point(4, 22);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(545, 405);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "script";
			this.tabPage2.UseVisualStyleBackColor = true;
			// 
			// functions
			// 
			this.functions.Location = new System.Drawing.Point(7, 8);
			this.functions.Multiline = true;
			this.functions.Name = "functions";
			this.functions.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.functions.Size = new System.Drawing.Size(525, 374);
			this.functions.TabIndex = 0;
			// 
			// script
			// 
			this.script.Location = new System.Drawing.Point(15, 23);
			this.script.Multiline = true;
			this.script.Name = "script";
			this.script.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.script.Size = new System.Drawing.Size(514, 321);
			this.script.TabIndex = 0;
			// 
			// output
			// 
			this.output.Location = new System.Drawing.Point(10, 461);
			this.output.Multiline = true;
			this.output.Name = "output";
			this.output.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.output.Size = new System.Drawing.Size(624, 100);
			this.output.TabIndex = 1;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(650, 460);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(118, 23);
			this.button1.TabIndex = 2;
			this.button1.Text = "Exec";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(650, 536);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(118, 25);
			this.button2.TabIndex = 3;
			this.button2.Text = "Clear";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(650, 489);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(118, 23);
			this.button3.TabIndex = 4;
			this.button3.Text = "Exec 2";
			this.button3.UseVisualStyleBackColor = true;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// Form1
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(796, 626);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.output);
			this.Controls.Add(this.tabControl1);
			this.Name = "Form1";
			this.Text = "Form1";
			this.tabControl1.ResumeLayout(false);
			this.tabPage1.ResumeLayout(false);
			this.tabPage1.PerformLayout();
			this.tabPage2.ResumeLayout(false);
			this.tabPage2.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabPage1;
		private System.Windows.Forms.TextBox functions;
		private System.Windows.Forms.TabPage tabPage2;
		private System.Windows.Forms.TextBox script;
		private System.Windows.Forms.TextBox output;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Button button3;
	}
}

