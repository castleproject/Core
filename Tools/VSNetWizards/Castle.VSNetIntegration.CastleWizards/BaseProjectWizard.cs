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
	using System.Collections;
	using System.IO;
	using System.Reflection;
	using System.Windows.Forms;

	using Castle.VSNetIntegration.CastleWizards.Dialogs;
	
	using EnvDTE;

	using Microsoft.Win32;

	/// <summary>
	/// 
	/// </summary>
	public delegate void WizardEventHandler(object sender, ExtensionContext context);

	/// <summary>
	/// 
	/// </summary>
	public delegate void WizardUIEventHandler(object sender, WizardDialog dlg, ExtensionContext context);

	/// <summary>
	/// 
	/// </summary>
	public abstract class BaseProjectWizard : IDTWizard, IWin32Window
	{
		private int owner;
		private DTE dteInstance;
		private String projectName;
		private String localProjectPath;
		private String installationDirectory;
		private String solutionName;
		private bool exclusive;
		private ExtensionContext context;

		#region Events

		public event WizardEventHandler OnAddProjects;

		public event WizardEventHandler OnSetupProjectsProperties;

		public event WizardEventHandler OnAddReferences;

		public event WizardEventHandler OnSetupBuildEvents;

		public event WizardEventHandler OnPostProcess;

		public event WizardUIEventHandler OnAddPanels;

		#endregion

		public void Execute(object Application, int hwndOwner, 
			ref object[] ContextParams, ref object[] CustomParams, ref EnvDTE.wizardResult retval)
		{
			// TODO: Add magic here
			IWizardExtension[] extensions = LoadExtensions(CustomParams);

			foreach(IWizardExtension extension in extensions)
			{
				extension.Init(this);
			}

			try
			{
				dteInstance = (DTE) Application;
				owner = hwndOwner;

				projectName = (String) ContextParams[1];
				localProjectPath = (String) ContextParams[2];
				installationDirectory = (String) ContextParams[3];
				exclusive = (bool) ContextParams[4];
				solutionName = (String) ContextParams[5];

				context = new ExtensionContext(projectName, localProjectPath, installationDirectory, solutionName);

				if (exclusive)
				{
					vsPromptResult promptResult = dteInstance.ItemOperations.PromptToSave;

					if (promptResult == vsPromptResult.vsPromptResultCancelled)
					{
						retval = wizardResult.wizardResultCancel;
						return;
					}
				}

				using(WizardDialog dlg = new WizardDialog())
				{
					dlg.WizardTitle = WizardTitle;
					
					AddPanels(dlg);

					if (OnAddPanels != null)
					{
						OnAddPanels(this, dlg, context);
					}

					dlg.ShowDialog(this);
					retval = dlg.WizardResult;

					if (retval == wizardResult.wizardResultSuccess)
					{
						CreateProject(dlg);
					}
					else
					{
						retval = wizardResult.wizardResultCancel;
						return;
					}
				}
			}
			finally
			{
				foreach(IWizardExtension extension in extensions)
				{
					extension.Terminate(this);
				}
			}
		}

		protected void CreateProject(WizardDialog dlg)
		{
			AddProjects(context);

			SetupProjectsProperties(context);

			AddReferences(context);

			SetupBuildEvents(context);

			PostProcess(context);
		}

		protected virtual void PostProcess(ExtensionContext context)
		{
			if (OnPostProcess != null)
			{
				OnPostProcess(this, context);
			}
		}

		protected virtual void SetupBuildEvents(ExtensionContext context)
		{
			if (OnSetupBuildEvents != null)
			{
				OnSetupBuildEvents(this, context);
			}
		}

		protected virtual void AddReferences(ExtensionContext context)
		{
			if (OnAddReferences != null)
			{
				OnAddReferences(this, context);
			}
		}

		protected virtual void SetupProjectsProperties(ExtensionContext context)
		{
			if (OnSetupProjectsProperties != null)
			{
				OnSetupProjectsProperties(this, context);
			}
		}

		protected virtual void AddProjects(ExtensionContext context)
		{
			if (OnAddProjects != null)
			{
				OnAddProjects(this, context);
			}
		}

		protected abstract void AddPanels(WizardDialog dlg);

		protected abstract String WizardTitle { get; }

		#region IWin32Window

		public IntPtr Handle
		{
			get { return new IntPtr(owner); }
		}

		#endregion

		#region Helper methods

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

		public DTE DteInstance
		{
			get { return dteInstance; }
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

		public bool Exclusive
		{
			get { return exclusive; }
		}

		protected string GetTemplateFileName(string filename)
		{
			filename = Path.Combine(TemplatePath, filename);

			filename = filename.Replace(@"\\", @"\");

			return filename;
		}
	
		protected void EnsureDirExists(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		#endregion

		private IWizardExtension[] LoadExtensions(object[] customParams)
		{
			ArrayList extensions = new ArrayList();

			foreach(String param in customParams)
			{
				String[] parts = param.Split('|');
				String assemblyFile = parts[0];
				String typeName = parts[1];

				Assembly assembly = Assembly.LoadFile(assemblyFile);

				foreach(Type type2 in assembly.GetTypes())
				{
					Console.Write(type2.Name);
				}

				Type type = assembly.GetType("TestExtensions2.RydexARExtension");
				type = assembly.GetType("RydexARExtension");
				type = assembly.GetType("TestExtensions2.RydexARExtension, TestExtensions2");

				extensions.Add( Activator.CreateInstance(type) );
			}

			return (IWizardExtension[]) extensions.ToArray(typeof(IWizardExtension));
		}
	}
}
