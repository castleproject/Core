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


	public class AboutAction : AbstractAction
	{
		private MenuItem _item;

		public AboutAction()
		{
		}

		public override void Install(IWorkspace workspace, object parentMenu, object parentGroup)
		{
			base.Install(workspace, parentMenu, parentGroup);

			_item = new MenuItem("About...");
			_item.Click += new EventHandler(OnShowAbout);

			(parentMenu as MenuItem).MenuItems.Add(_item);
		}

		private void OnShowAbout(object sender, EventArgs e)
		{
			using(AboutDialog dialog = new AboutDialog())
			{
				dialog.ShowDialog(Workspace.ActiveWindow);
			}
		}
	}
}
