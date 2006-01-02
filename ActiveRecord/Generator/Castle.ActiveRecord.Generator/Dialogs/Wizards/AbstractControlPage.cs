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

namespace Castle.ActiveRecord.Generator.Dialogs.Wizards
{
	using System;
	using System.Collections;
	using System.Windows.Forms;

	/// <summary>
	/// Damn VS.Net designer requires that this
	/// class be non-abstract
	/// </summary>
	public class AbstractControlPage : System.Windows.Forms.UserControl, IWizardPage
	{
		private Model _model;
		private System.Windows.Forms.Label title;
		private IDictionary _context;

		public AbstractControlPage(String title)
		{
			InitializeComponent();
			Title = title;
		}

		/// <summary>
		/// Only to make VS.Net happy (Dont use it)
		/// </summary>
		public AbstractControlPage()
		{
		}

		public String Title
		{
			get { return title.Text; }
			set { title.Text = value; }
		}

		protected Model model
		{
			get { return _model; }
		}

		protected IDictionary context
		{
			get { return _context; }
		}

		protected void RaiseChange(object sender, EventArgs e)
		{
			if (OnChange != null)
			{
				OnChange(sender, e);
			}
		}

		#region IWizardPage Members

		public virtual void Initialize(Model model, System.Collections.IDictionary context)
		{
			_model = model;
			_context = context;
		}

		public virtual void Activated(System.Collections.IDictionary context)
		{
		}

		public virtual void Deactivated(System.Collections.IDictionary context)
		{
		}

		private void InitializeComponent()
		{
			this.title = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// title
			// 
			this.title.Font = new System.Drawing.Font("Tahoma", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.title.Location = new System.Drawing.Point(16, 16);
			this.title.Name = "title";
			this.title.Size = new System.Drawing.Size(576, 23);
			this.title.TabIndex = 0;
			this.title.Text = "label1";
			// 
			// AbstractControlPage
			// 
			this.Controls.Add(this.title);
			this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Name = "AbstractControlPage";
			this.Size = new System.Drawing.Size(616, 344);
			this.ResumeLayout(false);

		}

		public virtual bool IsValid()
		{
			return true;
		}

		public virtual String ErrorMessage
		{
			get { return null; }
		}

		public event System.EventHandler OnChange;

		public System.Windows.Forms.Control InnerControl
		{
			get { return this; }
		}

		#endregion
	}
}