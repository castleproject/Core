// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.VSNetIntegration.CastleWizards.DDD
{
	using System.IO;
	using Castle.VSNetIntegration.CastleWizards.Shared;
	using EnvDTE;
	using Constants=Castle.VSNetIntegration.CastleWizards.Shared.Constants;

	[System.Runtime.InteropServices.ComVisible(false)]
	public class TestProjectExtension : IWizardExtension
	{
		public void Init(ICastleWizard wizard)
		{
			wizard.OnAddProjects += new WizardEventHandler(OnAddProjects);
			wizard.OnAddReferences += new WizardEventHandler(OnAddReferences);
			wizard.OnPostProcess += new WizardEventHandler(OnPostProcess);
		}

		public void Terminate(ICastleWizard wizard)
		{
			wizard.OnAddProjects -= new WizardEventHandler(OnAddProjects);
			wizard.OnAddReferences -= new WizardEventHandler(OnAddReferences);
			wizard.OnPostProcess -= new WizardEventHandler(OnPostProcess);
		}

		private void OnAddProjects(object sender, ExtensionContext context)
		{
			string projectFile = context.GetTemplateFileName(@"CSharp\DDDproject\src\Tests\Tests.csproj");
			string projectDir = Path.Combine(Path.Combine(context.LocalProjectPath, "src"), context.ProjectName + ".Tests");
			
			Project project = context.DteInstance.Solution.AddFromTemplate(projectFile, projectDir,
				context.ProjectName + ".Tests.csproj", false);
			
			project.Properties.Item("DefaultNameSpace").Value = context.ProjectName + ".Tests";
			
			context.Projects.Add(Constants.ProjectTest, project);
		}

		private void OnAddReferences(object sender, ExtensionContext context)
		{
			Project project = context.Projects[Constants.ProjectTest];

			Utils.AddReference(project, "Castle.MicroKernel.dll");
			Utils.AddReference(project, "Castle.Windsor.dll");
			Utils.AddReference(project, "NHibernate.dll");
			Utils.AddReference(project, "Nullables.dll");
			Utils.AddReference(project, "Castle.Monorail.Framework.dll");
			Utils.AddReference(project, "Nullables.NHibernate.dll");
			Utils.AddReference(project, "Iesi.Collections.dll");
			Utils.AddReference(project, "NUnit.Framework.dll");
			Utils.AddReference(project, "Castle.ActiveRecord.dll");
			Utils.AddReference(project, "Castle.Facilities.ActiveRecordIntegration.dll");
			Utils.AddReference(project, Path.Combine(context.LocalProjectPath, @"libs\Rhino.Mocks.dll"));
			Utils.AddReference(project, Path.Combine(context.LocalProjectPath, @"libs\Castle.Monorail.TestSupport.dll"));
			Utils.AddReference(project, context.Projects[Constants.ProjectMain]);
			Utils.AddReference(project, context.Projects[Constants.ProjectWeb]);
		}

		private void OnPostProcess(object sender, ExtensionContext context)
		{
			Project project = context.Projects[Constants.ProjectTest];
			PerformFileReplaces(project, context);
		}

		private void PerformFileReplaces(Project project, ExtensionContext context)
		{
			PerformNamespaceReplace(project, context, @"Controllers\ProductControllerTestCase.cs");
			PerformNamespaceReplace(project, context, @"Controllers\CategoryControllerTestCase.cs");
		}

		private void PerformNamespaceReplace(Project project, ExtensionContext context, string file)
		{
			Utils.PerformReplacesOn(project, context.ProjectName,
				string.Format(@"{0}\src\{1}", context.LocalProjectPath, project.Name),
				file);
		}
	}
}
