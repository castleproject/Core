// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
		private string testProjectName;
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
			testProjectName = context.ProjectName + ".Tests";
			string nameSpace = Utils.CreateValidIdentifierFromName(context.ProjectName);

			localTestProjectPath = new DirectoryInfo(Path.Combine(context.LocalProjectPath, 
				@"..\" + testProjectName)).FullName;

			Utils.EnsureDirExists(localTestProjectPath);

			Project testProject = context.DteInstance.Solution
				.AddFromTemplate(testProjectFile, 
					localTestProjectPath, 
					context.ProjectName + ".Tests.csproj", false);

			Utils.AddReference(testProject, context.Projects[Constants.ProjectMain]);

			Utils.PerformReplacesOn(testProject, nameSpace, localTestProjectPath, "Controllers\\ContactControllerTestCase.cs");
			Utils.PerformReplacesOn(testProject, nameSpace, localTestProjectPath, "Controllers\\HomeControllerTestCase.cs");
			Utils.PerformReplacesOn(testProject, nameSpace, localTestProjectPath, "Controllers\\LoginControllerTestCase.cs");

			context.Projects.Add(Constants.ProjectTest, testProject);
		}

		private void OnAddReferences(object sender, ExtensionContext context)
		{
		}

		private void OnPostProcess(object sender, ExtensionContext context)
		{
		}
		
		private void AddPanels(object sender, WizardDialog dlg, ExtensionContext context)
		{
			panel = new MRTestPanel();
			
			dlg.AddPanel(panel);
		}
		
		private void OnSetupBuildEvent(object sender, ExtensionContext context)
		{
		}
	}
}
