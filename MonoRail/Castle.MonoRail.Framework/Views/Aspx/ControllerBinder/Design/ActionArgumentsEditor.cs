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

#if NET

namespace Castle.MonoRail.Framework.Views.Aspx.Design
{
	using System;
	using System.ComponentModel;
	using System.Drawing.Design;
	using System.Windows.Forms;
	using System.Windows.Forms.Design;

	/// <summary>
	/// Pendent
	/// </summary>
	public class ActionArgumentEditor : UITypeEditor
	{
		/// <summary>
		/// Gets the editor style used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"></see> method.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that can be used to gain additional context information.</param>
		/// <returns>
		/// A <see cref="T:System.Drawing.Design.UITypeEditorEditStyle"></see> value that indicates the style of editor used by the <see cref="M:System.Drawing.Design.UITypeEditor.EditValue(System.IServiceProvider,System.Object)"></see> method. If the <see cref="T:System.Drawing.Design.UITypeEditor"></see> does not support this method, then <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"></see> will return <see cref="F:System.Drawing.Design.UITypeEditorEditStyle.None"></see>.
		/// </returns>
		public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
		{
			return UITypeEditorEditStyle.Modal;
		}

		/// <summary>
		/// Edits the specified object's value using the editor style indicated by the <see cref="M:System.Drawing.Design.UITypeEditor.GetEditStyle"></see> method.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"></see> that can be used to gain additional context information.</param>
		/// <param name="provider">An <see cref="T:System.IServiceProvider"></see> that this editor can use to obtain services.</param>
		/// <param name="value">The object to edit.</param>
		/// <returns>
		/// The new value of the object. If the value of the object has not changed, this should return the same object it was passed.
		/// </returns>
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
