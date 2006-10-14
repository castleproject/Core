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

namespace Castle.VSNetIntegration.CastleWizards
{
	using System;
	using System.IO;
	using System.Runtime.InteropServices;

	using Castle.VSNetIntegration.CastleWizards.Dialogs.Panels;
	using Castle.VSNetIntegration.CastleWizards.Shared;
	using Castle.VSNetIntegration.CastleWizards.Shared.Dialogs;
	using Castle.VSNetIntegration.Shared;

	using EnvDTE;
	using Constants=Castle.VSNetIntegration.CastleWizards.Shared.Constants;

#if DOTNET2
	[Guid("1D90721C-B0AA-4F9A-BDE5-A588911E38B6")]
	[ProgId("Castle.ActiveRecordModelProjectWizardVS8")]
	[ComDefaultInterface(typeof(IDTWizard))]
#else
	[Guid("50E5A1CA-8ABD-4AD2-8A60-176F5BFC9706")]
	[ProgId("Castle.ActiveRecordModelProjectWizardVS7")]
#endif
	[ComVisibleAttribute(true)]
	public class ActiveRecordModelProjectWizard : BaseProjectWizard
	{
		private ARPanel panel;

		protected override void AddPanels(WizardDialog dlg)
		{
			panel = new ARPanel();

			dlg.AddPanel(panel);
		}

		protected override String WizardTitle
		{
			get { return "New ActiveRecord project"; }
		}

		protected override void AddProjects(ExtensionContext context)
		{
			String projectFile = context.GetTemplateFileName(@"CSharp\ARProject\ARProject.csproj");
			String testProjectFile = context.GetTemplateFileName(@"CSharp\ARProjectTest\ARProjectTest.csproj");

			String localTestProjectPath = Path.Combine(LocalProjectPath, @"..\" + ProjectName + ".Tests");
			localTestProjectPath = new DirectoryInfo(localTestProjectPath).FullName;

			Utils.EnsureDirExists(LocalProjectPath);
			Utils.EnsureDirExists(localTestProjectPath);

			Project project = 
				context.DteInstance.Solution.AddFromTemplate(projectFile, LocalProjectPath, ProjectName + ".csproj", Exclusive);
			
			project.Properties.Item("DefaultNamespace").Value = ProjectName;

			Project testProject = 
				context.DteInstance.Solution.AddFromTemplate(testProjectFile, localTestProjectPath, ProjectName + ".Tests.csproj", false);

			testProject.Properties.Item("DefaultNamespace").Value = ProjectName + ".Tests";

			context.Projects.Add(Constants.ProjectMain, project);
			context.Projects.Add(Constants.ProjectTest, testProject);

			base.AddProjects(context);
		}

		protected override void AddReferences(ExtensionContext context)
		{
			Project project = context.Projects[Constants.ProjectMain];
			Project testProject = context.Projects[Constants.ProjectTest];

			Utils.AddReference(testProject, project);

			base.AddReferences(context);
		}

		protected override void SetupBuildEvents(ExtensionContext context)
		{
			Project testProject = context.Projects[Constants.ProjectTest];

			Utils.AddCommonPostBuildEvent(testProject);

			base.SetupBuildEvents(context);
		}

		protected override void PostProcess(ExtensionContext context)
		{
			Project testProject = context.Projects[Constants.ProjectTest];

			Utils.PerformReplacesOn(testProject, ProjectName, LocalProjectPath, "AbstractModelTestCase.cs");

			base.PostProcess(context);
		}
	}
}
