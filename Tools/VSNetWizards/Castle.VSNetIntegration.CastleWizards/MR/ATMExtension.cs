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
	using System.Xml;

	using Castle.VSNetIntegration.Shared;
	
	using EnvDTE;

	public class ATMExtension : IWizardExtension
	{
		public void Init(BaseProjectWizard wizard)
		{
			wizard.OnAddReferences += new WizardEventHandler(OnAddReferences);
			wizard.OnPostProcess += new WizardEventHandler(OnPostProcess);
		}

		public void Terminate(BaseProjectWizard wizard)
		{
			wizard.OnAddReferences -= new WizardEventHandler(OnAddReferences);
			wizard.OnPostProcess -= new WizardEventHandler(OnPostProcess);
		}

		private void OnAddReferences(object sender, ExtensionContext context)
		{
			if (!HasSelected(context)) return;

			Project project = context.Projects[Constants.ProjectMain];

			Utils.AddReference(project, "Castle.Services.Transaction.dll");
			Utils.AddReference(project, "Castle.Facilities.AutomaticTransactionManagement.dll");
		}

		private void OnPostProcess(object sender, ExtensionContext context)
		{
			if (!HasSelected(context)) return;

			XmlDocument facilitiesDom = (XmlDocument) context.Properties[MRConfigConstants.Facilities];

			RegisterAndConfigureFacility(facilitiesDom);
		}

		private void RegisterAndConfigureFacility(XmlDocument dom)
		{
			XmlElement facilitiesElem = (XmlElement) dom.SelectSingleNode("configuration/facilities");

			XmlElement facilityElem = dom.CreateElement("facility");
			
			facilityElem.SetAttribute("id", "transaction.management.facility");
			facilityElem.SetAttribute("type", "Castle.Facilities.AutomaticTransactionManagement.TransactionFacility, Castle.Facilities.AutomaticTransactionManagement");

			facilitiesElem.AppendChild(facilityElem);
		}

		private bool HasSelected(ExtensionContext context)
		{
			return ((bool) context.Properties["enableWindsorIntegration"]) == true &&
				context.Properties.Contains("Automatic Transaction Management") &&
				((bool) context.Properties["Automatic Transaction Management"]) == true;
		}
	}
}
