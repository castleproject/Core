// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.WindsorExtension
{
	using System;

	using Castle.MonoRail.Framework;

	/// <summary>
	/// This interface allow a wizard controller
	/// to request wizard steps from the IKernel without
	/// directly refering to it
	/// </summary>
	public interface IWizardPageFactory
	{
		/// <summary>
		/// Requests a <see cref="WizardStepPage"/> by
		/// the key the component was registered on the 
		/// controller
		/// </summary>
		/// <param name="key">The key used to register the component</param>
		/// <returns>The step page instance</returns>
		IWizardStepPage CreatePage(String key);

		/// <summary>
		/// Requests a <see cref="WizardStepPage"/> by
		/// the key the component was registered on the 
		/// controller
		/// </summary>
		/// <param name="stepPageType"></param>
		/// <returns>The step page instance</returns>
		IWizardStepPage CreatePage(Type stepPageType);
	}
}
