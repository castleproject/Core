using Castle.ActiveRecord.Generator.Components.Database;
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

namespace Castle.ActiveRecord.Generator.Actions
{
	using System;

	using Castle.ActiveRecord.Generator.Parts.Shapes;

	public class ActiveRecordGraphViewActionSet : IActionSet
	{
		private ShowPropertiesAction showProperties;
		private NewArDescriptorUsingWizard newUsingWizard;
		private CreateSubClassAction createSubClass;
		private CreateJoinedSubClassAction createJoinedSub;
		private CodePreviewAction codePreviewAction;

		public ActiveRecordGraphViewActionSet()
		{
		}

		#region IActionSet Members

		public void Init(Model model)
		{
			showProperties = new ShowPropertiesAction();
			newUsingWizard = new NewArDescriptorUsingWizard();
			createSubClass = new CreateSubClassAction();
			createJoinedSub = new CreateJoinedSubClassAction();
			codePreviewAction = new CodePreviewAction();

			showProperties.Init(model);
			newUsingWizard.Init(model);
			createSubClass.Init(model);
			createJoinedSub.Init(model);
			codePreviewAction.Init(model);
		}

		public void Install(IWorkspace workspace)
		{
			showProperties.Install(workspace, null, null);
			newUsingWizard.Install(workspace, null, null);
			createSubClass.Install(workspace, null, null);
			createJoinedSub.Install(workspace, null, null);
			codePreviewAction.Install(workspace, null, null);
		}

		#endregion

		public bool DoNewARWizard()
		{
			return newUsingWizard.Run(null);
		}

		public void CreateSubClass(ActiveRecordDescriptor descriptor)
		{
			createSubClass.Run(descriptor);
		}

		public void CreateJoinedSubClass(ActiveRecordDescriptor descriptor)
		{
			createJoinedSub.Run(descriptor);
		}

		public void PreviewCode(IActiveRecordDescriptor descriptor)
		{
			codePreviewAction.Run(descriptor);
		}
	}
}
