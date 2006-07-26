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
	using System.Runtime.InteropServices;
	using System.Windows.Forms;

	using Castle.VSNetIntegration.CastleWizards.Dialogs;
	
	using EnvDTE;

	
	// [Guid("50E5A1CA-8ABD-4AD2-8A60-176F5BFC9706")]
	// [ProgIdAttribute("Castle.ActiveRecordModelProjectWizard")]
	public class ActiveRecordModelProjectWizard2 : AbstractProjectWizard, IDTWizard, IWin32Window
	{
		private int owner;

		public void Execute(object Application, int hwndOwner, 
			ref object[] ContextParams, ref object[] CustomParams, ref EnvDTE.wizardResult retval)
		{
			dteInstance = (DTE) Application;
			owner = hwndOwner;

			projectName = (String) ContextParams[1];
			localProjectPath = (String) ContextParams[2];
			String installationDirectory = (String) ContextParams[3];
			bool exclusive = (bool) ContextParams[4];
			String solutionName = (String) ContextParams[5];
//			bool silent = (bool) ContextParams[6];

			if (exclusive)
			{
				vsPromptResult promptResult = dteInstance.ItemOperations.PromptToSave;

				if (promptResult == vsPromptResult.vsPromptResultCancelled)
				{
					retval = wizardResult.wizardResultCancel;
					return;
				}
			}

			using(ARNewProjectDialog dlg = new ARNewProjectDialog())
			{
				dlg.ShowDialog(this);
				retval = dlg.WizardResult;

				if (retval == wizardResult.wizardResultSuccess)
				{
					// Copy the params from the dialog, none in this case
				}
				else
				{
					retval = wizardResult.wizardResultCancel;
					return;
				}
			}

			String projectFile = GetTemplateFileName(@"CSharp\ARProject.csproj");
			String testProjectFile = GetTemplateFileName(@"CSharp\ARProjectTest.csproj");

			String localTestProjectPath = Path.Combine(localProjectPath, @"..\" + projectName + ".Tests");
			localTestProjectPath = new DirectoryInfo(localTestProjectPath).FullName;

			EnsureDirExists(localProjectPath);
			EnsureDirExists(localTestProjectPath);

			Project project = 
				dteInstance.Solution.AddFromTemplate(projectFile, localProjectPath, projectName + ".csproj", exclusive);
			
			project.Properties.Item("DefaultNamespace").Value = projectName;

			Project testProject = 
				dteInstance.Solution.AddFromTemplate(testProjectFile, localTestProjectPath, projectName + ".Tests.csproj", false);

			testProject.Properties.Item("DefaultNamespace").Value = projectName + ".Tests";

			AddReference(testProject, project);

			AddCommonPostBuildEvent(testProject);

			PerformReplacesOn(testProject, projectName, "AbstractModelTestCase.cs");
		}

		public IntPtr Handle
		{
			get { return new IntPtr(owner); }
		}	
	}
}
