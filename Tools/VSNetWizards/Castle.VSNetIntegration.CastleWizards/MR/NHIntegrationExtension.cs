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
	using Castle.VSNetIntegration.Shared;
	using Castle.VSNetIntegration.Shared.Dialogs;
	
	using EnvDTE;

	public class NHIntegrationExtension : IWizardExtension
	{
		private NHIntegrationPanel panel = new NHIntegrationPanel();
		private ConnStringPanel connPanel = new ConnStringPanel();

		public void Init(BaseProjectWizard wizard)
		{
			wizard.OnAddPanels += new WizardUIEventHandler(AddPanels);
			wizard.OnAddReferences += new WizardEventHandler(OnAddReferences);
			wizard.OnPostProcess += new WizardEventHandler(OnPostProcess);

			connPanel.DependencyKey = "NHibernate Integration";
			connPanel.Title = "NHibernate Integration";
		}

		public void Terminate(BaseProjectWizard wizard)
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
			Utils.AddReference(project, "Castle.Facilities.NHibernateIntegration.dll");
		}

		private void OnPostProcess(object sender, ExtensionContext context)
		{
			if (!HasSelected(context)) return;

			XmlDocument webConfigDom = (XmlDocument) context.Properties[MRConfigConstants.Web];

			RegisterSessionHttpModule(webConfigDom);

			XmlDocument facilitiesDom = (XmlDocument) context.Properties[MRConfigConstants.Facilities];

			RegisterAndConfigureFacility(facilitiesDom);

			XmlDocument propDom = (XmlDocument) context.Properties[MRConfigConstants.Properties];

			RegisterConnectionStringAsProperty(propDom);
		}

		private void RegisterConnectionStringAsProperty(XmlDocument dom)
		{
			XmlElement propertiesElem = (XmlElement) dom.SelectSingleNode("configuration/properties");

			XmlElement connStringElem = dom.CreateElement("nhConnectionString");
			
			connStringElem.AppendChild(dom.CreateTextNode(connPanel.ConnectionString));

			propertiesElem.AppendChild(connStringElem);
		}

		private void RegisterSessionHttpModule(XmlDocument dom)
		{
			XmlElement modulesElem = (XmlElement) dom.SelectSingleNode("configuration/system.web/httpModules");

			modulesElem.AppendChild(dom.CreateComment(" For more information on what this does "));
			modulesElem.AppendChild(dom.CreateComment(" visit http://www.castleproject.org/index.php/Facility:NHibernate_in_Web_apps "));

			XmlElement sessionModElem = dom.CreateElement("add");
			
			sessionModElem.SetAttribute("name", "NHibernateSessionWebModule");
			sessionModElem.SetAttribute("type", "Castle.Facilities.NHibernateIntegration.Components.SessionWebModule, Castle.Facilities.NHibernateIntegration");

			modulesElem.AppendChild(sessionModElem);
		}

		private void RegisterAndConfigureFacility(XmlDocument dom)
		{
			XmlElement facilitiesElem = (XmlElement) dom.SelectSingleNode("configuration/facilities");

			facilitiesElem.AppendChild(dom.CreateComment(" For more information on the configuration schema "));
			facilitiesElem.AppendChild(dom.CreateComment(" visit http://www.castleproject.org/index.php/Facility:NHibernate "));

			XmlElement nhElem = dom.CreateElement("facility");
			
			nhElem.SetAttribute("id", "nhibernatefacility");
			nhElem.SetAttribute("type", "Castle.Facilities.NHibernateIntegration.NHibernateFacility, Castle.Facilities.NHibernateIntegration");
			nhElem.SetAttribute("isWeb", "true");

			XmlElement factoryElem = dom.CreateElement("factory");
			factoryElem.SetAttribute("id", "nhibernate.factory");

			nhElem.AppendChild(factoryElem);

			XmlElement resourcesElem = dom.CreateElement("resources");
			factoryElem.AppendChild(resourcesElem);

			foreach(String file in panel.HbmFiles)
			{
				XmlElement elem = dom.CreateElement("resource");
				elem.SetAttribute("name", file);
				resourcesElem.AppendChild(elem);
			}

			XmlElement assembliesElem = dom.CreateElement("assemblies");
			factoryElem.AppendChild(assembliesElem);

			foreach(String assembly in panel.Assemblies)
			{
				XmlElement elem = dom.CreateElement("assembly");
				elem.AppendChild(dom.CreateTextNode(assembly));
				assembliesElem.AppendChild(elem);
			}

			XmlElement settingsElem = dom.CreateElement("settings");

			foreach(Pair pair in NHUtil.GetGeneralSettings())
			{
				XmlElement setting = dom.CreateElement("item");
				setting.SetAttribute("key", pair.First.ToString());
				setting.AppendChild(dom.CreateTextNode(pair.Second.ToString()));
				settingsElem.AppendChild(setting);
			}

			Pair[] pairs = NHUtil.GetSettingsFor(connPanel.Database);

			if (pairs != null)
			{
				foreach(Pair pair in pairs)
				{
					XmlElement setting = dom.CreateElement("item");
					setting.SetAttribute("key", pair.First.ToString());
					setting.AppendChild(dom.CreateTextNode(pair.Second.ToString()));
					settingsElem.AppendChild(setting);
				}
			}
			else
			{
				settingsElem.AppendChild(dom.CreateComment(" Could not infer driver or dialet... "));
				settingsElem.AppendChild(dom.CreateComment(" You need to provide those manually "));
			}

			XmlElement itemElem = dom.CreateElement("item");
			itemElem.SetAttribute("key", "hibernate.connection.connection_string");
			itemElem.AppendChild(dom.CreateTextNode("#{nhConnectionString}"));
			settingsElem.AppendChild(itemElem);

			factoryElem.AppendChild(settingsElem);

			facilitiesElem.AppendChild(nhElem);
		}

		private bool HasSelected(ExtensionContext context)
		{
			return ((bool) context.Properties["enableWindsorIntegration"]) == true &&
				context.Properties.Contains("ActiveRecord Integration") &&
				((bool) context.Properties["ActiveRecord Integration"]) == true;
		}
	}
}
