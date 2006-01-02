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


	public class HelpActionSet : IActionSet
	{
		private CastleSiteAction siteAction;
		private AboutAction aboutAction;

		public HelpActionSet()
		{
		}

		public void Init(Model model)
		{
			siteAction = new CastleSiteAction();
			aboutAction = new AboutAction();

			siteAction.Init(model);
			aboutAction.Init(model);
		}

		public void Install(IWorkspace workspace)
		{
			MenuItem item = new MenuItem("Help");
			workspace.MainMenu.MenuItems.Add(item);

			siteAction.Install(workspace, item, null);
			aboutAction.Install(workspace, item, null);
		}
	}
}
