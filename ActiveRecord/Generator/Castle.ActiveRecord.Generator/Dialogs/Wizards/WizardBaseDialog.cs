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

namespace Castle.ActiveRecord.Generator.Dialogs.Wizards
{
	using System;
	using System.Drawing;
	using System.Collections;
	using System.ComponentModel;
	using System.Windows.Forms;


	/// <summary>
	/// Summary description for WizardBaseDialog.
	/// </summary>
	public class WizardBaseDialog : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label title;
		private System.Windows.Forms.PictureBox pageContainer;
		private System.Windows.Forms.Button cancelButton;
		private System.Windows.Forms.Button backButton;
		private System.Windows.Forms.Button nextButton;
		private System.Windows.Forms.Button finishButton;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private ArrayList _pages = new ArrayList();
		private int _curPageIndex = 0;
		private IWizardPage _curPage;
		private System.Windows.Forms.Label label1;
		private Model _model;
		private IDictionary _context = new Hashtable();

		public WizardBaseDialog(Model model)
		{
			_model = model;

			InitializeComponent();

			this.Load += new EventHandler(WizardBaseDialog_Load);
		}

		/// <summary>
		/// Only to make VS.Net happy
		/// </summary>
		public WizardBaseDialog()
		{
			
		}

		public void AddPage(IWizardPage page)
		{
			page.Initialize(_model, _context);

			_pages.Add(page);
		}

		private void WizardBaseDialog_Load(object sender, EventArgs e)
		{
			ShowPage(0);
		}

		protected virtual void ShowPage(int pageIndex)
		{
			_curPageIndex = pageIndex;

			if (_curPage != null)
			{
				_curPage.Deactivated(_context);

				// Perform clean up
				_curPage.OnChange -= new EventHandler(page_OnChange);
			}

			_curPage = _pages[pageIndex] as IWizardPage;
			_curPage.Activated(_context);

			_curPage.OnChange += new EventHandler(page_OnChange);

			pageContainer.Controls.Clear();
			pageContainer.Controls.Add(_curPage.InnerControl);

			UpdateNavButtons();
		}

		private void UpdateNavButtons()
		{
			nextButton.Enabled = _curPage.IsValid() && (_curPageIndex + 1 < _pages.Count);
			backButton.Enabled = (_curPageIndex != 0);
			finishButton.Enabled = (_curPageIndex + 1 == _pages.Count);
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
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.title = new System.Windows.Forms.Label();
			this.pageContainer = new System.Windows.Forms.PictureBox();
			this.cancelButton = new System.Windows.Forms.Button();
			this.backButton = new System.Windows.Forms.Button();
			this.nextButton = new System.Windows.Forms.Button();
			this.finishButton = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.Color.White;
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Top;
			this.pictureBox1.Location = new System.Drawing.Point(0, 0);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(648, 48);
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// title
			// 
			this.title.AutoSize = true;
			this.title.BackColor = System.Drawing.Color.White;
			this.title.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.title.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.title.Location = new System.Drawing.Point(16, 8);
			this.title.Name = "title";
			this.title.Size = new System.Drawing.Size(92, 23);
			this.title.TabIndex = 1;
			this.title.Text = "Wizard Title";
			// 
			// pageContainer
			// 
			this.pageContainer.Location = new System.Drawing.Point(16, 56);
			this.pageContainer.Name = "pageContainer";
			this.pageContainer.Size = new System.Drawing.Size(616, 344);
			this.pageContainer.TabIndex = 2;
			this.pageContainer.TabStop = false;
			// 
			// cancelButton
			// 
			this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cancelButton.Location = new System.Drawing.Point(262, 416);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(82, 23);
			this.cancelButton.TabIndex = 3;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// backButton
			// 
			this.backButton.Location = new System.Drawing.Point(358, 416);
			this.backButton.Name = "backButton";
			this.backButton.Size = new System.Drawing.Size(82, 23);
			this.backButton.TabIndex = 4;
			this.backButton.Text = "< &Back";
			this.backButton.Click += new System.EventHandler(this.backButton_Click);
			// 
			// nextButton
			// 
			this.nextButton.Location = new System.Drawing.Point(446, 416);
			this.nextButton.Name = "nextButton";
			this.nextButton.Size = new System.Drawing.Size(82, 23);
			this.nextButton.TabIndex = 5;
			this.nextButton.Text = "&Next >";
			this.nextButton.Click += new System.EventHandler(this.nextButton_Click);
			// 
			// finishButton
			// 
			this.finishButton.Location = new System.Drawing.Point(542, 416);
			this.finishButton.Name = "finishButton";
			this.finishButton.Size = new System.Drawing.Size(82, 23);
			this.finishButton.TabIndex = 6;
			this.finishButton.Text = "&Finish";
			this.finishButton.Click += new System.EventHandler(this.finishButton_Click);
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.White;
			this.label1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.ForeColor = System.Drawing.Color.DarkRed;
			this.label1.Location = new System.Drawing.Point(24, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(600, 16);
			this.label1.TabIndex = 7;
			// 
			// WizardBaseDialog
			// 
			this.AcceptButton = this.finishButton;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 14);
			this.CancelButton = this.cancelButton;
			this.ClientSize = new System.Drawing.Size(648, 450);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.finishButton);
			this.Controls.Add(this.nextButton);
			this.Controls.Add(this.backButton);
			this.Controls.Add(this.cancelButton);
			this.Controls.Add(this.pageContainer);
			this.Controls.Add(this.title);
			this.Controls.Add(this.pictureBox1);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "WizardBaseDialog";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "WizardBaseDialog";
			this.ResumeLayout(false);

		}
		#endregion

		private void cancelButton_Click(object sender, System.EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void finishButton_Click(object sender, System.EventArgs e)
		{
			if (!CheckIsValid())
			{
				return;
			}

			_curPage.Deactivated(_context);

			DialogResult = DialogResult.OK;

			DoProcess();
			
			Close();
		}

		private void nextButton_Click(object sender, System.EventArgs e)
		{
			ShowNextPage();
		}

		private void backButton_Click(object sender, System.EventArgs e)
		{
			ShowPreviousPage();
		}

		private void page_OnChange(object sender, EventArgs e)
		{
			UpdateNavButtons();

			CheckIsValid();
		}

		private bool CheckIsValid()
		{
			if (_curPage != null && !_curPage.IsValid())
			{
				if (_curPage.ErrorMessage != null)
				{
					ErrorMessage = _curPage.ErrorMessage;
				}
				else
				{
					ErrorMessage = "Error: missing fields or invalid values for this page.";
				}

				return false;
			}
			else
			{
				ErrorMessage = "";
			}

			return true;
		}

		public String Title
		{
			get { return title.Text; }
			set
			{
				this.Text = value;
				title.Text = value;
			}
		}

		public String ErrorMessage
		{
			get { return label1.Text; }
			set { label1.Text = value; }
		}

		private void ShowPreviousPage()
		{
			ShowPage(_curPageIndex - 1);
		}

		private void ShowNextPage()
		{
			ShowPage(_curPageIndex + 1);
		}

		protected Model Model
		{
			get { return _model; }
		}

		protected IDictionary Context
		{
			get { return _context; }
		}

		protected virtual void DoProcess()
		{

		}
	}
}
