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

namespace Castle.VSNetIntegration.CastleWizards.Shared
{
	using System;
	using System.Collections.Specialized;
	using System.IO;

	using EnvDTE;
	
	using Microsoft.Win32;

	public class ExtensionContext
	{
		private readonly string projectName;
		private readonly string localProjectPath;
		private readonly string installationDirectory;
		private readonly string solutionName;
		private readonly Properties properties = new Properties();
		private readonly ProjectCollection projects = new ProjectCollection();
		private readonly DTE dteInstance;

		public ExtensionContext(DTE dteInstance, String projectName, String localProjectPath, String installationDirectory, String solutionName)
		{
			this.dteInstance = dteInstance;
			this.projectName = projectName;
			this.localProjectPath = localProjectPath;
			this.installationDirectory = installationDirectory;
			this.solutionName = solutionName;
		}

		public string ProjectName
		{
			get { return projectName; }
		}

		public string LocalProjectPath
		{
			get { return localProjectPath; }
		}

		public string InstallationDirectory
		{
			get { return installationDirectory; }
		}

		public string SolutionName
		{
			get { return solutionName; }
		}

		public ProjectCollection Projects
		{
			get { return projects; }
		}

		public Properties Properties
		{
			get { return properties; }
		}

		private RegistryKey GetCastleReg()
		{
			return Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Castle");
		}

		public string GetTemplateFileName(string filename)
		{
			filename = Path.Combine(TemplatePath, filename);
			filename = filename.Replace(@"\\", @"\");
			return filename;
		}

		protected String TemplatePath
		{
			get
			{
				String version = dteInstance.Version.Substring(0, 1);

				return (String) GetCastleReg().GetValue("vs" + version + "templatelocation");
			}
		}

		public String CassiniLocation
		{
#if DOTNET2
			get { return (String) GetCastleReg().GetValue("cassini2location"); }
#else
			get { return (String) GetCastleReg().GetValue("cassinilocation"); }
#endif
		}

		public DTE DteInstance
		{
			get { return dteInstance; }
		}
	}

	public class Properties : NameObjectCollectionBase
	{
		public void Add(String key, object value)
		{
			BaseAdd(key, value);
		}

		public bool Contains(String key)
		{
			return BaseGet(key) != null;
		}

		public object this[String key]
		{
			get { return BaseGet(key); }
			set { BaseSet(key, value); }
		}
	}

	public class ProjectCollection : NameObjectCollectionBase
	{
		public void Add(String key, Project project)
		{
			BaseAdd(key, project);
		}

		public Project this[String key]
		{
			get { return (Project) BaseGet(key); }
		}
	}
}
