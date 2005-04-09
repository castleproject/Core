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
	using System.IO;
	using System.Windows.Forms;
	using System.Runtime.Serialization.Formatters.Binary;

	using Castle.ActiveRecord.Generator.Components;


	public class ProjectOpenAction : AbstractAction
	{
		private ToolBarButton _button;

		public ProjectOpenAction()
		{
		}

		#region IAction Members

		public override void Install(IWorkspace workspace, object parentMenu, object parentGroup)
		{
			base.Install(workspace, parentMenu, parentGroup);

			MenuItem item = new MenuItem("&Open...");

			item.Click += new EventHandler(OnOpen);

			(parentMenu as MenuItem).MenuItems.Add(item);

			_button = new ToolBarButton();
			_button.ToolTipText = "Open";
			_button.ImageIndex = 1;

			(parentGroup as ToolBar).Buttons.Add( _button );
			(parentGroup as ToolBar).ButtonClick += new ToolBarButtonClickEventHandler(OnButtonClick);
		}

		#endregion

		private void OnOpen(object sender, EventArgs e)
		{
			String filename = null;

			using (OpenFileDialog dlg = new OpenFileDialog())
			{
				dlg.CheckFileExists = true;
				dlg.CheckPathExists = true;
				dlg.Multiselect = false;
				dlg.ShowReadOnly = false;
				dlg.DefaultExt = ".arproj";
				
				if (dlg.ShowDialog(Workspace.ActiveWindow) == DialogResult.OK)
				{
					filename = dlg.FileName;
				}
			}

			try
			{
				using(FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					BinaryFormatter formatter = new BinaryFormatter();

					Model.CurrentProject = formatter.Deserialize( fs ) as Project;
				}

				Model.Filename = filename;
			}
			catch(Exception ex)
			{
				Model.CurrentProject = new Project();

				MessageBox.Show(Workspace.ActiveWindow, ex.Message, "Error opening project");
			}
		}

		private void OnButtonClick(object sender, ToolBarButtonClickEventArgs e)
		{
			if (e.Button == _button)
			{
				OnOpen(sender, e);
			}
		}
	}
}
