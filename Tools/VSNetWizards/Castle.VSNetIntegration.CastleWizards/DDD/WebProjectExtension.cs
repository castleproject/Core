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

	public class WebProjectExtension : IWizardExtension
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
			string projectFile = context.GetTemplateFileName(@"CSharp\DDDproject\src\Web\Web.csproj");
			string projectDir = Path.Combine(Path.Combine(context.LocalProjectPath, "src"), context.ProjectName + ".Web");
			
			Project project = context.DteInstance.Solution.AddFromTemplate(projectFile, projectDir,
				context.ProjectName + ".Web.csproj", false);
			
			project.Properties.Item("DefaultNameSpace").Value = context.ProjectName + ".Web";
			
			context.Projects.Add(Constants.ProjectWeb, project);
		}

		private void OnAddReferences(object sender, ExtensionContext context)
		{
			Project project = context.Projects[Constants.ProjectWeb];

			string[] libs = {
				"System.Web.dll", "Castle.MicroKernel.dll", "Castle.Core.dll",
				"Castle.Facilities.Logging.dll", "Castle.Components.Binder.dll",
				"Castle.Components.Validator.dll", "Castle.Services.Transaction.dll",
				"Castle.Monorail.Framework.dll", "Castle.Monorail.Framework.Views.NVelocity.dll",
				"Castle.Monorail.WindsorExtension.dll", "Castle.Windsor.dll", 
				"NHibernate.dll", "NVelocity.dll", "Nullables.dll", "Nullables.NHibernate.dll",
				"Iesi.Collections.dll", "Castle.ActiveRecord.dll", "Castle.Facilities.ActiveRecordIntegration.dll", 
				"Castle.Facilities.AutomaticTransactionManagement", "Castle.Services.Logging.Log4netIntegration",
				context.GetTemplateFileName(@"CSharp\DDDProject\libs\Castle.MonoRail.ViewComponents.dll"), 
				"Castle.Monorail.ActiveRecordSupport.dll"
			};

			foreach (string lib in libs)
			{
				Utils.AddReference(project, lib);
			}

			Utils.AddReference(project, context.Projects[Constants.ProjectMain]);
		}

		private void OnPostProcess(object sender, ExtensionContext context)
		{
			Project project = context.Projects[Constants.ProjectWeb];
			PerformFileReplaces(project, context);
		}

		private void PerformFileReplaces(Project project, ExtensionContext context)
		{
			PerformNamespaceReplace(project, context, "global.asax");
			PerformNamespaceReplace(project, context, "GlobalApplication.cs");
			PerformNamespaceReplace(project, context, @"config\controllers.config");
			PerformNamespaceReplace(project, context, @"config\components.config");
			PerformNamespaceReplace(project, context, @"config\facilities.config");
			PerformNamespaceReplace(project, context, @"config\properties.config");
			PerformNamespaceReplace(project, context, @"config\productmodule\components.config");
			PerformNamespaceReplace(project, context, @"Views\layouts\default.vm");
			PerformNamespaceReplace(project, context, @"Controllers\BaseController.cs");
			PerformNamespaceReplace(project, context, @"Controllers\ProductController.cs");
			PerformNamespaceReplace(project, context, @"Controllers\CategoryController.cs");
		}

		private void PerformNamespaceReplace(Project project, ExtensionContext context, string file)
		{
			Utils.PerformReplacesOn(project, context.ProjectName,
				string.Format(@"{0}\src\{1}", context.LocalProjectPath, project.Name),
				file);
		}
	}
}
