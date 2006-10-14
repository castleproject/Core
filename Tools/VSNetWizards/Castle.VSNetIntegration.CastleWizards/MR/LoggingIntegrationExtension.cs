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
	using System.Xml;

	using Castle.VSNetIntegration.CastleWizards.Dialogs.Panels;
	using Castle.VSNetIntegration.CastleWizards.Shared;
	using Castle.VSNetIntegration.CastleWizards.Shared.Dialogs;
	using EnvDTE;
	using Constants=Castle.VSNetIntegration.CastleWizards.Shared.Constants;

	
	[System.Runtime.InteropServices.ComVisible(false)]
	public class LoggingIntegrationExtension : IWizardExtension
	{
		private LoggingPanel panel = new LoggingPanel();

		public void Init(ICastleWizard wizard)
		{
			wizard.OnAddPanels += new WizardUIEventHandler(AddPanels);
			wizard.OnAddReferences += new WizardEventHandler(OnAddReferences);
			wizard.OnPostProcess += new WizardEventHandler(OnPostProcess);
		}

		public void Terminate(ICastleWizard wizard)
		{
			wizard.OnAddPanels -= new WizardUIEventHandler(AddPanels);
			wizard.OnAddReferences -= new WizardEventHandler(OnAddReferences);
			wizard.OnPostProcess -= new WizardEventHandler(OnPostProcess);
		}

		private void AddPanels(object sender, WizardDialog dlg, ExtensionContext context)
		{
			dlg.AddPanel(panel);
		}

		private void OnAddReferences(object sender, ExtensionContext context)
		{
			if (!HasSelected(context)) return;

			Project project = context.Projects[Constants.ProjectMain];

			Utils.AddReference(project, "Castle.Facilities.Logging.dll");

			if (panel.LogingApi == "Log4net")
			{
				Utils.AddReference(project, "log4net.dll");
				Utils.AddReference(project, "Castle.Services.Logging.Log4netIntegration.dll");
			}
			else if (panel.LogingApi == "NLog")
			{
				Utils.AddReference(project, "NLog.dll");
				Utils.AddReference(project, "NLog.DotNet.dll");
				Utils.AddReference(project, "Castle.Services.Logging.NLogIntegration.dll");
			}
		}

		private void OnPostProcess(object sender, ExtensionContext context)
		{
			if (!HasSelected(context)) return;

			XmlDocument facilitiesDom = (XmlDocument) context.Properties[MRConfigConstants.Facilities];

			RegisterAndConfigureFacility(facilitiesDom, context);
		}

		private void RegisterAndConfigureFacility(XmlDocument dom, ExtensionContext context)
		{
			XmlElement facilitiesElem = (XmlElement) dom.SelectSingleNode("configuration/facilities");

			XmlElement logElem = dom.CreateElement("facility");
			
			logElem.SetAttribute("id", "loggingfacility");
			logElem.SetAttribute("type", "Castle.Facilities.Logging.LoggingFacility, Castle.Facilities.Logging");
			logElem.SetAttribute("loggingApi", panel.LogingApi);

			if (panel.LogingApi == "Log4net")
			{
				logElem.SetAttribute("configFile", "logging.config");

				Project project = context.Projects[Constants.ProjectMain];

				String projectFile = context.GetTemplateFileName(@"CSharp\MRProject\logging.config");
				project.ProjectItems.AddFromTemplate(projectFile, "logging.config");
			}

			facilitiesElem.AppendChild(logElem);
		}

		private bool HasSelected(ExtensionContext context)
		{
			return ((bool) context.Properties["enableWindsorIntegration"]) == true &&
				context.Properties.Contains("Logging") &&
				((bool) context.Properties["Logging"]) == true;
		}
	}
}
