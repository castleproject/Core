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

	using Castle.MicroKernel;

	using Castle.Facilities.TypedFactory;
	using Castle.Facilities.ActiveRecordGenerator.Forms;
	using Castle.Facilities.ActiveRecordGenerator.Components;
	using Castle.Facilities.ActiveRecordGenerator.Action;
	using Castle.Facilities.ActiveRecordGenerator.Action.Standard;

	using Castle.Components.Winforms.AssemblyResolver;


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
		}

		private static void AddComponents(ActiveRecordGeneratorContainer container)
		{
			AddForms(container);

			AddActions(container);

			AddMvcComponents(container);

			container.AddComponent( "assembly.resolver", typeof(AssemblyResolverComponent) );
		}

		private static void AddMvcComponents(ActiveRecordGeneratorContainer container)
		{
			container.AddComponent( "controller", 
				typeof(IApplicationController), typeof(ApplicationController) );
			container.AddComponent( "model", 
				typeof(IApplicationModel), typeof(ApplicationModel) );
		}

		private static void AddActions(ActiveRecordGeneratorContainer container)
		{
			container.AddComponent( ActionConstants.New_Project, 
				typeof(IAction), typeof(NewProjectAction) );
			container.AddComponent( ActionConstants.New_Active_Record, 
				typeof(IAction), typeof(NewActiveRecordAction) );
			container.AddComponent( ActionConstants.Exit, 
				typeof(IAction), typeof(ExitAction) );
			container.AddComponent( ActionConstants.Save_Project, 
				typeof(IAction), typeof(SaveAction) );
			container.AddComponent( ActionConstants.SaveAs_Project, 
				typeof(IAction), typeof(SaveAsAction) );
		}

		private static void AddForms(ActiveRecordGeneratorContainer container)
		{
			container.AddComponent("newproject.form", typeof(NewProjectForm));
			container.AddComponent("mainform.form", typeof(MainForm));
			container.AddComponent("ardefinition.form", typeof(ActiveRecordDefForm));
		}

		private static void AddFacilities(ActiveRecordGeneratorContainer container)
		{
			IFacility[] facilities = container.Kernel.GetFacilities();

			foreach(IFacility facility in facilities)
			{
				TypedFactoryFacility typedFactory = (facility as TypedFactoryFacility);

				if (typedFactory != null)
				{
					typedFactory.AddTypedFactoryEntry( 
						new FactoryEntry("action.factory", typeof(IActionFactory), "Create", "") );
		
					break;
				}
			}
		}
	}
}
