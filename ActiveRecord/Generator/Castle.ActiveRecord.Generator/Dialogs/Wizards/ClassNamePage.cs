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
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;
	using Castle.ActiveRecord.Generator.Components;
	using Castle.ActiveRecord.Generator.Components.Database;

	public class ClassNamePage : Castle.ActiveRecord.Generator.Dialogs.Wizards.AbstractControlPage
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox className;
		private System.ComponentModel.IContainer components = null;
		private TableDefinition _oldTable;

		public ClassNamePage() : base("We're ready! Are you?")
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.label1 = new System.Windows.Forms.Label();
			this.className = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(24, 72);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(568, 48);
			this.label1.TabIndex = 0;
			this.label1.Text = "We\'re ready to create the ActiveRecord definition for you. Would you like to chan" +
				"ge the suggested class name?";
			// 
			// className
			// 
			this.className.Location = new System.Drawing.Point(176, 168);
			this.className.Name = "className";
			this.className.Size = new System.Drawing.Size(232, 20);
			this.className.TabIndex = 1;
			this.className.Text = "";
			// 
			// ClassNamePage
			// 
			this.Controls.Add(this.className);
			this.Controls.Add(this.label1);
			this.Name = "ClassNamePage";
			this.Size = new System.Drawing.Size(616, 344);
			this.ResumeLayout(false);

		}
		#endregion

		public override bool IsValid()
		{
			return className.Text.Trim().Length != 0;
		}

		public override void Activated(System.Collections.IDictionary context)
		{
			TableDefinition table = context["selectedtable"] as TableDefinition;
			
			if (table != _oldTable)
			{
				_oldTable = table;
				
				INamingService naming = ServiceRegistry.Instance[ typeof(INamingService) ] as INamingService;

				className.Text = naming.CreateClassName(table.Name);
			}
		}

		public override void Deactivated(System.Collections.IDictionary context)
		{
			base.Deactivated(context);

			ActiveRecordDescriptor desc = context["ardesc"] as ActiveRecordDescriptor;

			desc.ClassName = className.Text;
		}
	}
}

