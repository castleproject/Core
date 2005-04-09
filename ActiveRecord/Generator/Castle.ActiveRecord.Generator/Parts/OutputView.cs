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

namespace Castle.ActiveRecord.Generator.Parts
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;
	using WeifenLuo.WinFormsUI;
	using Castle.ActiveRecord.Generator.Components;


	/// <summary>
	/// Summary description for OutputView.
	/// </summary>
	public class OutputView : DockContent, ILogger, ISubWorkspace
	{
		private System.Windows.Forms.TextBox textBox1;
		private long _start;
		private long _end;

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private IWorkspace _parentWs;

		public OutputView()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			ServiceRegistry.Instance.Kernel.AddComponentInstance("logger", typeof (ILogger), this);
		}

		public OutputView(Model model) : this()
		{
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.BackColor = System.Drawing.Color.White;
			this.textBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textBox1.Location = new System.Drawing.Point(0, 2);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox1.Size = new System.Drawing.Size(255, 361);
			this.textBox1.TabIndex = 1;
			this.textBox1.Text = "";
			this.textBox1.WordWrap = false;
			// 
			// OutputView
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(255, 365);
			this.Controls.Add(this.textBox1);
			this.DockableAreas = ((WeifenLuo.WinFormsUI.DockAreas)(((((WeifenLuo.WinFormsUI.DockAreas.Float | WeifenLuo.WinFormsUI.DockAreas.DockLeft) 
				| WeifenLuo.WinFormsUI.DockAreas.DockRight) 
				| WeifenLuo.WinFormsUI.DockAreas.DockTop) 
				| WeifenLuo.WinFormsUI.DockAreas.DockBottom)));
			this.DockPadding.Bottom = 2;
			this.DockPadding.Top = 2;
			this.HideOnClose = true;
			this.Name = "OutputView";
			this.ShowHint = WeifenLuo.WinFormsUI.DockState.DockBottomAutoHide;
			this.Text = "OutputView";
			this.ResumeLayout(false);

		}

		#endregion

		public void Info(String message)
		{
			EnsureVisible();

			textBox1.Text += message + "\r\n";
		}

		public void Start()
		{
			textBox1.Text = "--------------------- Starting ---------------------\r\n\r\n";
			_start = DateTime.Now.Ticks;
		}

		public void End()
		{
			textBox1.Text += "\r\n--------------------- Finished ---------------------";
			_end = DateTime.Now.Ticks;

			TimeSpan diff = new TimeSpan(_end - _start);

			textBox1.Text += "\r\n\r\nCompleted in " + diff.TotalSeconds + " seconds";
		}

		private void EnsureVisible()
		{
			this.Show( _parentWs.MainDockManager );
		}

		#region ISubWorkspace Members

		public IWorkspace ParentWorkspace
		{
			get { return _parentWs; }
			set { _parentWs = value; }
		}

		#endregion

		#region IWorkspace Members

		public IWin32Window ActiveWindow
		{
			get { return this; }
		}

		public MainMenu MainMenu
		{
			get { return null; }
		}

		public ToolBar MainToolBar
		{
			get { return null; }
		}

		public StatusBar MainStatusBar
		{
			get { return null; }
		}

		public DockPanel MainDockManager
		{
			get { return null; }
		}

		#endregion
	}
}