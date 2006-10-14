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

namespace Castle.VSNetIntegration.CastleWizards
{
	using System;
	using System.IO;
	using Castle.VSNetIntegration.CastleWizards.Dialogs.Panels;
	using Castle.VSNetIntegration.CastleWizards.Shared;
	using Castle.VSNetIntegration.CastleWizards.Shared.Dialogs;
	using EnvDTE;
	using Constants=Castle.VSNetIntegration.CastleWizards.Shared.Constants;

	[System.Runtime.InteropServices.ComVisible(false)]
	public class TestProjectExtension : IWizardExtension
	{
		private string localTestProjectPath;
		private MRTestPanel panel;

		#region IWizardExtension implementation
		
		public void Init(ICastleWizard wizard)
		{
			wizard.OnAddPanels += new WizardUIEventHandler(AddPanels);
			wizard.OnAddProjects += new WizardEventHandler(OnAddProjects);
			wizard.OnAddReferences += new WizardEventHandler(OnAddReferences);
			wizard.OnPostProcess += new WizardEventHandler(OnPostProcess);
			wizard.OnSetupBuildEvents += new WizardEventHandler(OnSetupBuildEvent);
		}

		public void Terminate(ICastleWizard wizard)
		{
			wizard.OnAddPanels -= new WizardUIEventHandler(AddPanels);
			wizard.OnAddProjects -= new WizardEventHandler(OnAddProjects);
			wizard.OnAddReferences -= new WizardEventHandler(OnAddReferences);
			wizard.OnPostProcess -= new WizardEventHandler(OnPostProcess);
			wizard.OnSetupBuildEvents -= new WizardEventHandler(OnSetupBuildEvent);
		}
		
		#endregion

		private void OnAddProjects(object sender, ExtensionContext context)
		{
			if (!panel.WantsTestProject) return;
			
			String testProjectFile = context.GetTemplateFileName(@"CSharp\MRProjectTest\MRProjectTest.csproj");

			localTestProjectPath = new DirectoryInfo(Path.Combine(context.LocalProjectPath, @"..\" + context.ProjectName + ".Tests")).FullName;

			Utils.EnsureDirExists(localTestProjectPath);

			Project testProject = 
				context.DteInstance.Solution.AddFromTemplate(testProjectFile, 
				                                             localTestProjectPath, 
				                                             context.ProjectName + ".Tests.csproj", false);

			context.Projects.Add(Constants.ProjectTest, testProject);
		}

		private void OnAddReferences(object sender, ExtensionContext context)
		{
		}

		private void OnPostProcess(object sender, ExtensionContext context)
		{
			if (!panel.WantsTestProject) return;

			Project project = context.Projects[Constants.ProjectMain];
			Project testProject = context.Projects[Constants.ProjectTest];

			String name = project.Name;
			
			Utils.PerformReplacesOn(testProject, name, localTestProjectPath, "HomeControllerTestCase.cs");
			Utils.PerformReplacesOn(testProject, name, context.LocalProjectPath, "App.config");
		}
		
		private void AddPanels(object sender, WizardDialog dlg, ExtensionContext context)
		{
			panel = new MRTestPanel();
			
			dlg.AddPanel(panel);
		}
		
		private void OnSetupBuildEvent(object sender, ExtensionContext context)
		{
			if (!panel.WantsTestProject) return;
			
			Project testProject = context.Projects[Constants.ProjectTest];

			Utils.AddCommonPostBuildEvent(testProject);
		}
	}
}
