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
	using Constants = Castle.VSNetIntegration.CastleWizards.Shared.Constants;

	[System.Runtime.InteropServices.ComVisible(false)]
	public class ARIntegrationExtension : IWizardExtension
	{
		private ARIntegrationPanel panel = new ARIntegrationPanel();
		private ConnStringPanel connPanel = new ConnStringPanel();

		public void Init(ICastleWizard wizard)
		{
			wizard.OnAddPanels += new WizardUIEventHandler(AddPanels);
			wizard.OnAddReferences += new WizardEventHandler(OnAddReferences);
			wizard.OnPostProcess += new WizardEventHandler(OnPostProcess);

			connPanel.DependencyKey = "ActiveRecord Integration";
			connPanel.Title = "ActiveRecord Integration";
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
			dlg.AddPanel(connPanel);
		}

		private void OnAddReferences(object sender, ExtensionContext context)
		{
			if (!HasSelected(context)) return;

			Project project = context.Projects[Constants.ProjectMain];

			Utils.AddReference(project, "NHibernate.dll");
			Utils.AddReference(project, "Nullables.dll");
			Utils.AddReference(project, "Nullables.NHibernate.dll");
			Utils.AddReference(project, "Iesi.Collections.dll");
			Utils.AddReference(project, "log4net.dll");
			Utils.AddReference(project, "Castle.ActiveRecord.dll");
			Utils.AddReference(project, "Castle.Facilities.ActiveRecordIntegration.dll");
		}

		private void OnPostProcess(object sender, ExtensionContext context)
		{
			if (!HasSelected(context)) return;

			XmlDocument webConfigDom = (XmlDocument) context.Properties[MRConfigConstants.Web];

			RegisterSessionScopeHttpModule(webConfigDom);

			XmlDocument facilitiesDom = (XmlDocument) context.Properties[MRConfigConstants.Facilities];

			RegisterAndConfigureFacility(facilitiesDom);

			XmlDocument propDom = (XmlDocument) context.Properties[MRConfigConstants.Properties];

			RegisterConnectionStringAsProperty(propDom);
		}

		private void RegisterConnectionStringAsProperty(XmlDocument dom)
		{
			XmlElement propertiesElem = (XmlElement) dom.SelectSingleNode("configuration/properties");

			XmlElement connStringElem = dom.CreateElement("connectionString");
			
			connStringElem.AppendChild(dom.CreateTextNode(connPanel.ConnectionString));

			propertiesElem.AppendChild(connStringElem);
		}

		private void RegisterSessionScopeHttpModule(XmlDocument dom)
		{
			XmlElement modulesElem = (XmlElement) dom.SelectSingleNode("configuration/system.web/httpModules");

			XmlElement sessionModElem = dom.CreateElement("add");
			
			sessionModElem.SetAttribute("name", "ar_sessionscope");
			sessionModElem.SetAttribute("type", "Castle.ActiveRecord.Framework.SessionScopeWebModule, Castle.ActiveRecord");

			modulesElem.AppendChild(sessionModElem);
		}

		private void RegisterAndConfigureFacility(XmlDocument dom)
		{
			XmlElement facilitiesElem = (XmlElement) dom.SelectSingleNode("configuration/facilities");

			facilitiesElem.AppendChild(dom.CreateComment(" For more information on ActiveRecord configuration "));
			facilitiesElem.AppendChild(dom.CreateComment(" visit http://www.castleproject.org/index.php/ActiveRecord:Configuration_Reference "));

			XmlElement arElem = dom.CreateElement("facility");
			
			arElem.SetAttribute("id", "arintegration");
			arElem.SetAttribute("type", "Castle.Facilities.ActiveRecordIntegration.ActiveRecordFacility, Castle.Facilities.ActiveRecordIntegration");
			arElem.SetAttribute("isWeb", "true");

			XmlElement assembliesElem = dom.CreateElement("assemblies");

			foreach(String assembly in panel.ARAssemblies)
			{
				XmlElement elem = dom.CreateElement("item");
				elem.AppendChild(dom.CreateTextNode(assembly));
				assembliesElem.AppendChild(elem);
			}

			arElem.AppendChild(assembliesElem);

			XmlElement configElem = dom.CreateElement("config");
			
			foreach(Pair pair in NHUtil.GetGeneralSettings())
			{
				XmlElement setting = dom.CreateElement("add");
				setting.SetAttribute("key", pair.First.ToString());
				setting.SetAttribute("value", pair.Second.ToString());
				configElem.AppendChild(setting);
			}

			Pair[] pairs = NHUtil.GetSettingsFor(connPanel.Database);

			if (pairs != null)
			{
				foreach(Pair pair in pairs)
				{
					XmlElement setting = dom.CreateElement("add");
					setting.SetAttribute("key", pair.First.ToString());
					setting.SetAttribute("value", pair.Second.ToString());
					configElem.AppendChild(setting);
				}
			}
			else
			{
				configElem.AppendChild(dom.CreateComment(" Could not infer driver or dialet... "));
				configElem.AppendChild(dom.CreateComment(" You need to provide those manually "));
			}

			XmlElement itemElem = dom.CreateElement("add");
			itemElem.SetAttribute("key", "hibernate.connection.connection_string");
			itemElem.SetAttribute("value", "#{connectionString}");
			configElem.AppendChild(itemElem);

			arElem.AppendChild(configElem);

			facilitiesElem.AppendChild(arElem);
		}

		private bool HasSelected(ExtensionContext context)
		{
			return ((bool) context.Properties["enableWindsorIntegration"]) == true &&
				context.Properties.Contains("ActiveRecord Integration") &&
				((bool) context.Properties["ActiveRecord Integration"]) == true;
		}
	}
}
