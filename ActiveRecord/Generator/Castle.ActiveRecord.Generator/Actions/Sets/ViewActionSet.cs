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

	using Castle.ActiveRecord.Generator.Parts;


	public class ViewActionSet : IActionSet
	{
		private ViewProjectExplorerAction projExAction;
		private ViewAvailableItemsAction avalItemsAction;
		private ActiveRecordGraphView _graph;
		private OutputView _outputview;
		private ProjectExplorer _explorer;
		private AvailableShapes _shapes;

		public ViewActionSet(ActiveRecordGraphView graph, OutputView outputview, 
			ProjectExplorer explorer, AvailableShapes shapes)
		{
			_graph = graph;
			_outputview = outputview;
			_explorer = explorer;
			_shapes = shapes;
		}

		#region IActionSet Members

		public void Init(Model model)
		{
			projExAction = new ViewProjectExplorerAction(_explorer);
			avalItemsAction = new ViewAvailableItemsAction(_shapes);

			projExAction.Init(model);
			avalItemsAction.Init(model);
		}

		public void Install(IWorkspace workspace)
		{
			MenuItem item = new MenuItem("View");
			workspace.MainMenu.MenuItems.Add(item);

			ToolBar toolbar = workspace.MainToolBar;

			projExAction.Install(workspace, item, toolbar);
			avalItemsAction.Install(workspace, item, toolbar);

			ToolBarButton sep = new ToolBarButton();
			sep.Style = ToolBarButtonStyle.Separator;
			
			toolbar.Buttons.Add( sep );
		}

		#endregion
	}
}
