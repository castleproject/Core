// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

#if DOTNET2

namespace Castle.MonoRail.Framework.Views.Aspx.Design
{
	using System;
	using System.ComponentModel;
	using System.Drawing.Design;
	using System.Windows.Forms;
	using System.Windows.Forms.Design;

	public class ActionArgumentEditor : UITypeEditor
	{
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		public override object EditValue(ITypeDescriptorContext context,
		                                 IServiceProvider provider, object value)
		{
			ActionArgumentCollection actionArgs = value as ActionArgumentCollection;

			if (actionArgs != null && provider != null)
			{
				IWindowsFormsEditorService editorService =
					provider.GetService(typeof(IWindowsFormsEditorService)) as
					IWindowsFormsEditorService;

				if (editorService != null)
				{
					ActionArgumentCollection workingArgs = CreateWorkingArguments(actionArgs);

					ActionArgumentsEditorForm actionEditor = new ActionArgumentsEditorForm(workingArgs, context);

					try
					{
						if (editorService.ShowDialog(actionEditor) == DialogResult.OK)
						{
							context.OnComponentChanging();

							CommitWorkingArguments(actionArgs, workingArgs);

							context.OnComponentChanged();
						}

						return actionArgs;
					}
					finally
					{
						actionEditor.Dispose();
					}
				}
			}

			return value;
		}

		private ActionArgumentCollection CreateWorkingArguments(ActionArgumentCollection originalArguments)
		{
			ActionArgumentCollection workingArgs = new ActionArgumentCollection();

			foreach(ActionArgument actionArg in originalArguments)
			{
				ActionArgument actionArgCopy = (ActionArgument) actionArg.Clone();
				workingArgs.Add(actionArgCopy);
			}

			return workingArgs;
		}

		private void CommitWorkingArguments(ActionArgumentCollection originalArguments,
		                                    ActionArgumentCollection workingArguments)
		{
			originalArguments.Clear();

			foreach(ActionArgument workingArgument in workingArguments)
			{
				originalArguments.Add(workingArgument);
			}
		}
	}
}

#endif
