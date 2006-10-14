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

namespace Castle.VSNetIntegration.Shared
{
	using System;
	using Castle.VSNetIntegration.Shared.Dialogs;

	/// <summary>
	/// 
	/// </summary>
	public delegate void WizardEventHandler(object sender, ExtensionContext context);

	/// <summary>
	/// 
	/// </summary>
	public delegate void WizardUIEventHandler(object sender, WizardDialog dlg, ExtensionContext context);

	public interface ICastleWizard
	{
		event WizardEventHandler OnAddProjects;

		event WizardEventHandler OnSetupProjectsProperties;

		event WizardEventHandler OnAddReferences;

		event WizardEventHandler OnSetupBuildEvents;

		event WizardEventHandler OnPostProcess;

		event WizardUIEventHandler OnAddPanels;

		
	}
}
