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
	using System.Windows.Forms;

	using Castle.ActiveRecord.Generator.Components;


	public class Starter
	{
		private IApplicationLayout _layout;
		private MainForm _main;
		private Model _model;

		public Starter()
		{
			_model = new Model();
			_layout = new ARGeneratorLayout(_model);

			_main = new MainForm(_model);
			_main.Load += new EventHandler(OnWorkspaceLoad);
			_main.Closed += new EventHandler(OnWorkspaceClosed);

			Application.Run(_main);
		}

		[STAThread]
		public static void Main()
		{
			new Starter();
		}

		private void OnWorkspaceLoad(object sender, EventArgs e)
		{
			// Installs the application UI layout
			_layout.Install(_main);

			// Restore pesisted changes to layout
			_layout.Restore(_main);

			_model.CurrentProject = new Project();
		}

		private void OnWorkspaceClosed(object sender, EventArgs e)
		{
			// Persist changes to layout
			_layout.Persist(_main);
		}
	}
}
