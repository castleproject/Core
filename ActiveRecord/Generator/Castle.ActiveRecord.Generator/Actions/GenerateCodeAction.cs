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

namespace Castle.ActiveRecord.Generator.Actions
{
	using System;
	using System.Windows.Forms;

	using Castle.ActiveRecord.Generator.Dialogs;
	using Castle.ActiveRecord.Generator.Components.CodeGenerator;


	public class GenerateCodeAction : AbstractAction
	{
		private MenuItem _item;

		public override void Install(IWorkspace workspace, object parentMenu, object parentGroup)
		{
			base.Install(workspace, parentMenu, parentGroup);

			_item = new MenuItem("Generate code...");
			_item.Click += new EventHandler(OnGenerate);

			(parentMenu as MenuItem).MenuItems.Add(_item);
		}

		private void OnGenerate(object sender, EventArgs e)
		{
			using(GenCodeDialog dialog = new GenCodeDialog(Model))
			{
				if (dialog.ShowDialog(Workspace.ActiveWindow) == DialogResult.OK)
				{
					IProjectGenerator gen = ServiceRegistry.Instance[ typeof(IProjectGenerator) ] as IProjectGenerator; 
					gen.Generate( Model.CurrentProject );
				}
			}
		}
	}
}
