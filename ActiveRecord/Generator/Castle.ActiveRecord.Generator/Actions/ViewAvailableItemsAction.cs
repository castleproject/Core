using Castle.ActiveRecord.Generator.Parts;
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

	using Castle.ActiveRecord.Generator.Components;


	public class ViewAvailableItemsAction : AbstractAction
	{
		private MenuItem _item;
		private ToolBarButton _button;
		private AvailableShapes _availableShapes;

		public ViewAvailableItemsAction(AvailableShapes availableShapes)
		{
			_availableShapes = availableShapes;
		}

		#region IAction Members

		public override void Install(IWorkspace workspace, object parentMenu, object parentGroup)
		{
			base.Install(workspace, parentMenu, parentGroup);

			_item = new MenuItem("&Available Nodes");

			_item.Click += new EventHandler(OnNew);
			(parentMenu as MenuItem).MenuItems.Add(_item);

			_button = new ToolBarButton();
			_button.ToolTipText = "Available Nodes";
			_button.ImageIndex = 5;

			(parentGroup as ToolBar).Buttons.Add( _button );
			(parentGroup as ToolBar).ButtonClick += new ToolBarButtonClickEventHandler(ProjectNewAction_ButtonClick);
		}

		#endregion

		private void OnNew(object sender, EventArgs e)
		{
			if (sender == _item)
			{
				DoAction();
			}
		}

		private void DoAction()
		{
			_availableShapes.Show(Workspace.MainDockManager);
		}

		private void ProjectNewAction_ButtonClick(object sender, ToolBarButtonClickEventArgs e)
		{
			if (e.Button == _button)
			{
				DoAction();
			}
		}
	}
}
