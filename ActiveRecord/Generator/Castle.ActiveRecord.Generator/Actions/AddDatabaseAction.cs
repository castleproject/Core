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

namespace Castle.ActiveRecord.Generator.Actions
{
	using System;
	using System.Windows.Forms;

	using Castle.ActiveRecord.Generator.Dialogs;
	using Castle.ActiveRecord.Generator.Components.Database;


	public class AddDatabaseAction : AbstractAction
	{
		private System.Windows.Forms.ToolBarButton toolBarButton1;

		public AddDatabaseAction()
		{
			this.toolBarButton1 = new System.Windows.Forms.ToolBarButton();
		}

		#region IAction Members

		public override void Install(IWorkspace workspace, object parentMenu, object parentGroup)
		{
			base.Install(workspace,  parentMenu, parentGroup);

			// 
			// toolBarButton1
			// 
			this.toolBarButton1.ImageIndex = ImageConstants.Database_Tables;

			workspace.MainToolBar.Buttons.AddRange(new System.Windows.Forms.ToolBarButton[]
				{
					this.toolBarButton1
				});

			workspace.MainToolBar.ButtonClick += 
				new ToolBarButtonClickEventHandler(MainToolBar_ButtonClick);
		}

		#endregion

		private void MainToolBar_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
		{
			if (e.Button == toolBarButton1)
			{
				using(DatabaseConnectionDialog dialog = new DatabaseConnectionDialog())
				{
					DialogResult result = dialog.ShowDialog(Workspace.ActiveWindow);

					if (result == DialogResult.OK)
					{
						IDatabaseDefinitionBuilder defBuilder = 
							ServiceRegistry.Instance[ typeof(IDatabaseDefinitionBuilder) ] as IDatabaseDefinitionBuilder;

						DatabaseDefinition def = defBuilder.Build(dialog.Alias, dialog.ConnectionString);

						Model.CurrentProject.AddDatabaseDefinition(def);

						Model.Update();
					}
				}
			}
		}
	}
}
