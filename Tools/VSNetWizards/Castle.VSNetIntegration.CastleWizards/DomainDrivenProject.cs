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

namespace Castle.VSNetIntegration.CastleWizards
{
	using System.Collections;
	using System.IO;
	using System.Runtime.InteropServices;
	using Castle.VSNetIntegration.CastleWizards.DDD;
	using Castle.VSNetIntegration.CastleWizards.Dialogs.Panels;
	using Castle.VSNetIntegration.CastleWizards.Shared;
	using Castle.VSNetIntegration.CastleWizards.Shared.Dialogs;
	using Castle.VSNetIntegration.Shared;
	using EnvDTE;
	using Constants=Castle.VSNetIntegration.CastleWizards.Shared.Constants;

	[Guid("C9D68471-3AC6-4102-AE91-EB3DD37061E1")]
	[ProgId("Castle.DomainDrivenProjectWizardVS8")]
	[ComDefaultInterface(typeof(IDTWizard))]
	[ComVisibleAttribute(true)]
	public class DomainDrivenProject : BaseProjectWizard
	{
		private DDDPanel panel;

		protected override void AddProjects(ExtensionContext context)
		{
			CreateCoreProject(context);
			CreateBaseDirectories();
			CopyLibraries();
			base.AddProjects(context);
		}

		protected override void AddExtensions(IList extensions)
		{
			extensions.Add(new DDD.TestProjectExtension());
			extensions.Add(new WebProjectExtension());
		}

		private void CreateBaseDirectories()
		{
			Utils.EnsureDirExists(Path.Combine(LocalProjectPath, "libs"));
			Utils.EnsureDirExists(Path.Combine(LocalProjectPath, "docs"));
			Utils.EnsureDirExists(Path.Combine(LocalProjectPath, "tools"));
		}

		private void CopyLibraries()
		{
			FileInfo[] files = new DirectoryInfo(
				Context.GetTemplateFileName(
					@"CSharp\DDDProject\libs")).GetFiles("*.dll");

			foreach (FileInfo file in files)
			{
				File.Copy(file.FullName, Path.Combine(Path.Combine(LocalProjectPath, "libs"), file.Name));
			}
		}

		private void CreateCoreProject(ExtensionContext context)
		{
			string projectFile = context.GetTemplateFileName(@"CSharp\DDDProject\src\Core\Core.csproj");

			Utils.EnsureDirExists(LocalProjectPath);

			string projectDir = Path.Combine(Path.Combine(LocalProjectPath, "src"), ProjectName + ".Core");

			Project project = context.DteInstance.Solution.AddFromTemplate(projectFile,
				projectDir, ProjectName + ".Core.csproj", Exclusive);
			project.Properties.Item("DefaultNameSpace").Value = ProjectName + ".Core";

			context.Projects.Add(Constants.ProjectMain, project);

			PerformFileReplaces(project);
			UpdateReference(project);
		}

		private void UpdateReference(Project project)
		{
			Utils.AddReference(project, "Castle.DynamicProxy.dll");
			Utils.AddReference(project, "Castle.MicroKernel.dll");
			Utils.AddReference(project, "Castle.Services.Transaction.dll");
			Utils.AddReference(project, "Castle.Core.dll");
			Utils.AddReference(project, "Castle.Windsor.dll");
			Utils.AddReference(project, "NHibernate.dll");
			Utils.AddReference(project, "Nullables.dll");
			Utils.AddReference(project, "Nullables.NHibernate.dll");
			Utils.AddReference(project, "Iesi.Collections.dll");
			Utils.AddReference(project, "Castle.ActiveRecord.dll");
		}

		private void PerformFileReplaces(Project project)
		{
			PerformNamespaceReplace(project, @"ProductModule\Product.cs");
			PerformNamespaceReplace(project, @"ProductModule\Category.cs");
			PerformNamespaceReplace(project, @"Infrastructure\BaseARRepository.cs");
			PerformNamespaceReplace(project, @"Infrastructure\IIdentifiable.cs");
			PerformNamespaceReplace(project, @"Infrastructure\IRepository.cs");
			
			PerformNamespaceReplace(project, @"ProductModule\Repositories\IProductRepository.cs");
			PerformNamespaceReplace(project, @"ProductModule\Repositories\ProductRepository.cs");
			PerformNamespaceReplace(project, @"ProductModule\Services\ProductService.cs");

			PerformNamespaceReplace(project, @"ProductModule\Repositories\ICategoryRepository.cs");
			PerformNamespaceReplace(project, @"ProductModule\Repositories\CategoryRepository.cs");
			PerformNamespaceReplace(project, @"ProductModule\Services\CategoryService.cs");
		}

		private void PerformNamespaceReplace(Project project, string file)
		{
			Utils.PerformReplacesOn(project, ProjectName, 
				string.Format(@"{0}\src\{1}.Core", LocalProjectPath, ProjectName), 
				file);
		}

		protected override void AddPanels(WizardDialog dlg)
		{
			panel = new DDDPanel();
			dlg.AddPanel(panel);
		}

		protected override string WizardTitle
		{
			get { return "New Castle DDD Project"; }
		}
	}
}
