using Castle.Facilities.ActiveRecordGenerator.Action.Standard;
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

namespace Castle.Facilities.ActiveRecordGenerator
{
	using System;
	using System.Windows.Forms;

	using Castle.Facilities.TypedFactory;

	using Castle.Facilities.ActiveRecordGenerator.Forms;
	using Castle.Facilities.ActiveRecordGenerator.Components;
	using Castle.Facilities.ActiveRecordGenerator.Action;


	public class App
	{
		[STAThread]
		public static void Main() 
		{
			ActiveRecordGeneratorContainer container = new ActiveRecordGeneratorContainer();

			AddFacilities(container);

			AddComponents(container);

			StartApplication(container);
		}

		private static void StartApplication(ActiveRecordGeneratorContainer container)
		{
			MainForm form = (MainForm) container[ typeof(MainForm) ];
	
			Application.Run(form);
		}

		private static void AddComponents(ActiveRecordGeneratorContainer container)
		{
			container.AddComponent("newproject.form", typeof(NewProjectForm));
			container.AddComponent("mainform.form", typeof(MainForm));

			container.AddComponent( ActionConstants.New_Project, typeof(IAction), typeof(NewProjectAction) );
			container.AddComponent( ActionConstants.Exit, typeof(IAction), typeof(ExitAction) );
		}

		private static void AddFacilities(ActiveRecordGeneratorContainer container)
		{
			TypedFactoryFacility typedFactory = new TypedFactoryFacility();
	
			container.AddFacility( "typedFactory", typedFactory );

			typedFactory.AddTypedFactoryEntry( 
				new FactoryEntry("action.factory", typeof(IActionFactory), "Create", "") );
		}
	}
}
