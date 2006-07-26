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

	using Castle.VSNetIntegration.CastleWizards.Dialogs;
	using Castle.VSNetIntegration.CastleWizards.Dialogs.Panels;

	public class RydexARExtension : IWizardExtension
	{
		private RydexARPanel panel = new RydexARPanel();

		public void Init(BaseProjectWizard wizard)
		{
			wizard.OnAddPanels += new WizardUIEventHandler(wizard_OnAddPanels);
			wizard.OnAddReferences += new WizardEventHandler(wizard_OnAddReferences);
		}

		public void Terminate(BaseProjectWizard wizard)
		{
			wizard.OnAddPanels -= new WizardUIEventHandler(wizard_OnAddPanels);
			wizard.OnAddReferences -= new WizardEventHandler(wizard_OnAddReferences);
		}

		private void wizard_OnAddPanels(object sender, WizardDialog dlg, ExtensionContext context)
		{
			dlg.AddPanel(panel);
		}

		private void wizard_OnAddReferences(object sender, ExtensionContext context)
		{
			if (panel.UseRydexAR)
			{
				Utils.AddReference(context.Projects[ARConstants.ProjectARMain], "Rydex.Framework.ActiveRecord.dll");
			}
		}
	}
}
