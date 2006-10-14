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

namespace Castle.VSNetIntegration.CastleWizards.Shared
{
	using System;
	using System.Windows.Forms;

	public class WizardPanel : UserControl
	{
		private ExtensionContext context;

		public event EventHandler OnChange;

		public ExtensionContext Context
		{
			get { return context; }
		}

		public void SetContext(ExtensionContext context)
		{
			this.context = context;
		}

		public virtual bool WantsToShow(ExtensionContext context)
		{
			return true;
		}

		protected void RaiseOnChange()
		{
			if (OnChange != null)
			{
				OnChange(this, EventArgs.Empty);
			}
		}
	}
}
