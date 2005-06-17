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
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;


	public class ProjectSaveAction : AbstractAction
	{
		private MenuItem _item;
		private ToolBarButton _button;

		public ProjectSaveAction()
		{
		}

		public override void Install(IWorkspace workspace, object parentMenu, object parentGroup)
		{
			base.Install(workspace, parentMenu, parentGroup);

			_item = new MenuItem("&Save");
			_item.Click += new EventHandler(OnSave);

			(parentMenu as MenuItem).MenuItems.Add(_item);

			_button = new ToolBarButton();
			_button.ToolTipText = "Save";
			_button.ImageIndex = 2;

			(parentGroup as ToolBar).Buttons.Add( _button );
			(parentGroup as ToolBar).ButtonClick += new ToolBarButtonClickEventHandler(OnButtonClick);
		}

		private void OnSave(object sender, EventArgs e)
		{
			if (Model.Filename == null)
			{
				using(SaveFileDialog dlg = new SaveFileDialog())
				{
					dlg.CheckPathExists = true;
					dlg.OverwritePrompt = true;
					dlg.CreatePrompt = false;
					dlg.DefaultExt = ".arproj";

					if (dlg.ShowDialog(Workspace.ActiveWindow) == DialogResult.OK)
					{
						Model.Filename = dlg.FileName;
					}
					else
					{
						return;
					}
				}
			}


			try
			{
				Model.CurrentProject.Name = new FileInfo(Model.Filename).Name;

				using(FileStream fs = new FileStream(Model.Filename, FileMode.Create, FileAccess.Write, FileShare.Write))
				{
					BinaryFormatter formatter = new BinaryFormatter();

					formatter.Serialize( fs, Model.CurrentProject );
				}

				Model.Update();
			}
			catch(Exception ex)
			{
				MessageBox.Show(Workspace.ActiveWindow, ex.Message, "Error saving project");
			}
		}

		private void OnButtonClick(object sender, ToolBarButtonClickEventArgs e)
		{
			if (e.Button == _button)
			{
				OnSave(sender, e);
			}
		}
	}
}
