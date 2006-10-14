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

namespace Castle.VSNetIntegration.Shared
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.Reflection;
	using System.Runtime.InteropServices;
	using System.Windows.Forms;

	using Castle.VSNetIntegration.CastleWizards.Shared;
	using Castle.VSNetIntegration.CastleWizards.Shared.Dialogs;
	
	using EnvDTE;


	/// <summary>
	/// 
	/// </summary>
	[ComVisible(false)]
	public abstract class BaseProjectWizard : IDTWizard, IWin32Window, ICastleWizard
	{
		private static readonly object AddProjectsEventKey = new object();
		private static readonly object SetupProjectsPropertiesEventKey = new object();
		private static readonly object AddReferencesEventKey = new object();
		private static readonly object SetupBuildEventsEventKey = new object();
		private static readonly object PostProcessEventKey = new object();
		private static readonly object AddPanelsEventKey = new object();
		
		private int owner;
		private DTE dteInstance;
		private String projectName;
		private String localProjectPath;
		private String installationDirectory;
		private String solutionName;
		private bool exclusive;
		private ExtensionContext context;
		private EventHandlerList eventList = new EventHandlerList();
		
		#region ICastleWizard

		public event WizardEventHandler OnAddProjects
		{
			add { eventList.AddHandler(AddProjectsEventKey, value); }
			remove { eventList.RemoveHandler(AddProjectsEventKey, value); }
		}

		public event WizardEventHandler OnSetupProjectsProperties
		{
			add { eventList.AddHandler(SetupProjectsPropertiesEventKey, value); }
			remove { eventList.RemoveHandler(SetupProjectsPropertiesEventKey, value); }
		}

		public event WizardEventHandler OnAddReferences
		{
			add { eventList.AddHandler(AddReferencesEventKey, value); }
			remove { eventList.RemoveHandler(AddReferencesEventKey, value); }
		}

		public event WizardEventHandler OnSetupBuildEvents
		{
			add { eventList.AddHandler(SetupBuildEventsEventKey, value); }
			remove { eventList.RemoveHandler(SetupBuildEventsEventKey, value); }
		}

		public event WizardEventHandler OnPostProcess
		{
			add { eventList.AddHandler(PostProcessEventKey, value); }
			remove { eventList.RemoveHandler(PostProcessEventKey, value); }
		}

		public event WizardUIEventHandler OnAddPanels
		{
			add { eventList.AddHandler(AddPanelsEventKey, value); }
			remove { eventList.RemoveHandler(AddPanelsEventKey, value); }
		}

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

				context = new ExtensionContext(dteInstance, projectName, localProjectPath, installationDirectory, solutionName);

				if (exclusive)
				{
					vsPromptResult promptResult = dteInstance.ItemOperations.PromptToSave;

					if (promptResult == vsPromptResult.vsPromptResultCancelled)
					{
						retval = wizardResult.wizardResultCancel;
						return;
					}
				}

				using(WizardDialog dlg = new WizardDialog(context))
				{
					dlg.WizardTitle = WizardTitle;
					
					AddPanels(dlg);

					WizardUIEventHandler eventHandler = (WizardUIEventHandler) eventList[AddPanelsEventKey];
					
					if (eventHandler != null)
					{
						eventHandler(this, dlg, context);
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
			catch(Exception ex)
			{
				String message = ex.GetType().Name + "\r\n\r\n" + ex.Message + "\r\n\r\n" + ex.StackTrace;
				
				MessageBox.Show(this, "Exception during project creation. \r\n" + message, 
					"Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				
				throw;
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
			WizardEventHandler eventHandler = (WizardEventHandler) eventList[PostProcessEventKey];
					
			if (eventHandler != null)
			{
				eventHandler(this, context);
			}
		}

		protected virtual void SetupBuildEvents(ExtensionContext context)
		{
			WizardEventHandler eventHandler = (WizardEventHandler) eventList[SetupBuildEventsEventKey];
					
			if (eventHandler != null)
			{
				eventHandler(this, context);
			}
		}

		protected virtual void AddReferences(ExtensionContext context)
		{
			WizardEventHandler eventHandler = (WizardEventHandler) eventList[AddReferencesEventKey];
					
			if (eventHandler != null)
			{
				eventHandler(this, context);
			}
		}

		protected virtual void SetupProjectsProperties(ExtensionContext context)
		{
			WizardEventHandler eventHandler = (WizardEventHandler) eventList[SetupProjectsPropertiesEventKey];
					
			if (eventHandler != null)
			{
				eventHandler(this, context);
			}
		}

		protected virtual void AddProjects(ExtensionContext context)
		{
			WizardEventHandler eventHandler = (WizardEventHandler) eventList[AddProjectsEventKey];
					
			if (eventHandler != null)
			{
				eventHandler(this, context);
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

		#endregion

		public ExtensionContext Context
		{
			get { return context; }
		}

		private IWizardExtension[] LoadExtensions(object[] customParams)
		{
			ArrayList extensions = new ArrayList();

			AddExtensions(extensions);

			try
			{
				foreach(String param in customParams)
				{
					if (param == String.Empty) continue;

					String[] parts = param.Split('|');
					String assemblyFile = parts[0];
					String typeName = parts[1];

					// Assembly assembly = Assembly.LoadFile(assemblyFile, AppDomain.CurrentDomain.Evidence);
					Assembly assembly = Assembly.LoadFrom(assemblyFile, AppDomain.CurrentDomain.Evidence);

					Type type = assembly.GetType(typeName);

					extensions.Add( Activator.CreateInstance(type) );
				}
			}
			catch(Exception ex)
			{
				MessageBox.Show(String.Format("{0} {1}", ex.Message, ex.StackTrace));
			}

			return (IWizardExtension[]) extensions.ToArray(typeof(IWizardExtension));
		}

		protected virtual void AddExtensions(IList extensions)
		{
		}
	}
}
