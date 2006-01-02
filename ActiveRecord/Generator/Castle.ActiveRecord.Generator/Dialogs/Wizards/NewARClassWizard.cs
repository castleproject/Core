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

using Castle.ActiveRecord.Generator.Components;

namespace Castle.ActiveRecord.Generator.Dialogs.Wizards
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Drawing;
	using System.Windows.Forms;
	using Netron.GraphLib;

	using Castle.ActiveRecord.Generator.Components.Database;
	using Castle.ActiveRecord.Generator.Parts.Shapes;


	public class NewARClassWizard : WizardBaseDialog
	{
		private System.ComponentModel.IContainer components = null;
		private ActiveRecordDescriptor _arDesc;
		private ActiveRecordDescriptor[] _dependents;

		public NewARClassWizard(Model model) : base(model)
		{
//			if (shape.ActiveRecordDescriptor == null)
//			{
//				throw new ArgumentException("No AR instance in shape");
//			}

			_arDesc = new ActiveRecordDescriptor();
			Context["ardesc"] = _arDesc;

			InitializeComponent();

			Title = "New ActiveRecord class";

			AddPage(new WelcomePage());
			AddPage(new TableSelectionPage());
			AddPage(new MappingPage());
			AddPage(new RelationsPage());
			AddPage(new ClassNamePage());
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
			components = new System.ComponentModel.Container();
		}
		#endregion

		protected override void DoProcess()
		{
			ActiveRecordDescriptor ar = Context["ardesc"] as ActiveRecordDescriptor;

			ar.Table = (Context["selectedtable"] as TableDefinition);
			ar.Table.RelatedDescriptor = ar;
//			_shape.ActiveRecordDescriptor = ar;

			// Build dependencies

			IActiveRecordDescriptorBuilder builder = 
				ServiceRegistry.Instance[ typeof(IActiveRecordDescriptorBuilder) ] 
				as IActiveRecordDescriptorBuilder;

			BuildContext buildContext = Context["buildcontext"] as BuildContext;
			Dependents = builder.Build( buildContext );
		}

		public ActiveRecordDescriptor ActiveRecordDescriptor
		{
			get { return _arDesc; }
		}

		public ActiveRecordDescriptor[] Dependents
		{
			set { _dependents = value; }
			get { return _dependents;  }
		}
	}
}

