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

namespace Castle.ActiveRecord.Generator
{
	using System;
	using System.IO;
	using System.Windows.Forms;

	using WeifenLuo.WinFormsUI;

	using Castle.ActiveRecord.Generator.Actions;
	using Castle.ActiveRecord.Generator.Parts;


	public class ARGeneratorLayout : IApplicationLayout
	{
		private Model _model;
		private ActiveRecordGraphView arGraph;
		private OutputView outView;
		private ProjectExplorer projExplorer;
		private AvailableShapes avaShapes;

		public ARGeneratorLayout(Model model)
		{
			_model = model;
		}

		#region IApplicationLayout Members

		public void Install(IWorkspace workspace)
		{
			// Add parts

			arGraph = new ActiveRecordGraphView(_model);
			arGraph.ParentWorkspace = workspace;

			outView = new OutputView(_model);
//			outView.ParentWorkspace = workspace;

			projExplorer = new ProjectExplorer(_model);
			projExplorer.ParentWorkspace = workspace;

			avaShapes = new AvailableShapes(_model);
			avaShapes.ParentWorkspace = workspace;

			// Register Actions
			
			FileActionGroup group1 = new FileActionGroup();
			group1.Init(_model);
			group1.Install(workspace);

			ViewActionSet group2 = new ViewActionSet(arGraph, outView, projExplorer, avaShapes);
			group2.Init(_model);
			group2.Install(workspace);

			HelpActionSet group3 = new HelpActionSet();
			group3.Init(_model);
			group3.Install(workspace);

		}

		public void Persist(IWorkspace workspace)
		{
			String configFile = Path.Combine(
				Path.GetDirectoryName(Application.ExecutablePath), "Layout.config");
			workspace.MainDockManager.SaveAsXml(configFile);
		}

		public void Restore(IWorkspace workspace)
		{
			String configFile = Path.Combine(
				Path.GetDirectoryName(Application.ExecutablePath), "Layout.config");

//			if (File.Exists(configFile))
//			{
//				workspace.MainDockManager.LoadFromXml(configFile, 
//					new GetContentCallback(GetContentFromPersistString));
//			}
//			else
			{
				arGraph.Show(workspace.MainDockManager);
				outView.Show(workspace.MainDockManager);
				projExplorer.Show(workspace.MainDockManager);
				avaShapes.Show(workspace.MainDockManager);
			}
		}

		#endregion

		private DockContent GetContentFromPersistString(String persistString)
		{
			if (persistString == typeof(ActiveRecordGraphView).ToString())
				return arGraph;
			else if (persistString == typeof(OutputView).ToString())
				return outView;
			else if (persistString == typeof(ProjectExplorer).ToString())
				return projExplorer;
			else if (persistString == typeof(AvailableShapes).ToString())
				return avaShapes;
			else
			{
				return null;
			}
		}
	}
}
