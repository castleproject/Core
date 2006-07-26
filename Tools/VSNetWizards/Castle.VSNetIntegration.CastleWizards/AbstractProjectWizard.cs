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

	using EnvDTE;

	using Microsoft.Win32;

	public abstract class AbstractProjectWizard
	{
		protected String projectName;
		protected String localProjectPath;
		protected DTE dteInstance;

		private RegistryKey GetCastleReg()
		{
			return Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Castle");
		}

		protected String TemplatePath
		{
			get
			{
				String version = dteInstance.Version.Substring(0, 1);

				return (String) GetCastleReg().GetValue("vs" + version + "templatelocation");
			}
		}

		protected String CassiniLocation
		{
			get { return (String) GetCastleReg().GetValue("cassinilocation"); }
		}

		protected string GetTemplateFileName(string filename)
		{
			filename = Path.Combine(TemplatePath, filename);

			filename = filename.Replace(@"\\", @"\");

			return filename;
		}
	
		protected void AddReference(Project project, Project otherProject)
		{
			VSLangProj.VSProject vsProject;

			vsProject = (VSLangProj.VSProject) project.Object;
			vsProject.References.AddProject(otherProject);
		}

		protected void AddReference(Project project, String assembly)
		{
			VSLangProj.VSProject vsProject;

			vsProject = (VSLangProj.VSProject) project.Object;
			vsProject.References.Add(assembly);
		}

		protected void PerformReplacesOn(Project project, string projectName, string filename)
		{
			String[] elems = filename.Split('\\');

			ProjectItem item = null;

			foreach(String elem in elems)
			{
				if (item == null)
				{
					item = project.ProjectItems.Item(elem);
				}
				else
				{
					item = item.ProjectItems.Item(elem);
				}
			}

			PerformReplacesOn(project, projectName, item);
		}

		protected void PerformReplacesOn(Project project, string projectName, ProjectItem item)
		{
			Window codeWindow = item.Open(Constants.vsViewKindTextView);
	
			codeWindow.Activate();
	
			ReplaceToken(codeWindow, "!NAMESPACE!", projectName);
			ReplaceToken(codeWindow, "!APPPHYSICALDIR!", localProjectPath);

			codeWindow.Close(vsSaveChanges.vsSaveChangesYes);
		}

		protected void ReplaceToken(Window window, string token, string replaceWith)
		{
			window.Document.ReplaceText(token, replaceWith, 256);// vsFindOptions.vsFindOptionsFromStart
		}

		protected void AddCommonPostBuildEvent(Project project)
		{
			VSLangProj.VSProject vsProject = (VSLangProj.VSProject) project.Object;

			vsProject.Project.Properties.Item("PostBuildEvent").Value = "copy \"$(ProjectDir)\\App.config\" \"$(TargetPath).config\"";
		}

		protected void EnsureDirExists(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}
	}
}
