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

#if (DOTNET2 && NET)

namespace Castle.MonoRail.Framework.Views.Aspx.Design
{
	using System;
	using System.ComponentModel;
	using System.ComponentModel.Design;
	using System.Text;
	using System.Windows.Forms;
	using WebControl=System.Web.UI.Control;

	public partial class ActionArgumentsEditorForm : Form
	{
		private readonly ActionArgumentCollection actionArgs;
		private readonly ITypeDescriptorContext context;

		public ActionArgumentsEditorForm(ActionArgumentCollection actionArgs,
		                                 ITypeDescriptorContext context)
		{
			InitializeComponent();

			this.actionArgs = actionArgs;
			this.context = context;
		}

		private void ActionArgumentsEditorForm_Load(object sender, EventArgs e)
		{
			PopulateTargetControls();
			RefreshRemovalCandidates();

			pgActionArguments.SelectedObject = new ActionArgumentsPropertyGridAdapter(actionArgs);
		}

		private void AddActionArgument()
		{
			string error;

			try
			{
				ActionArgument actionArg = new ActionArgument();
				actionArg.Name = txtName.Text;
				actionArg.Expression = BuildExpression();

				if (actionArg.IsValid())
				{
					actionArgs.Add(actionArg);
					RefreshActionArguments();

					txtName.Text = string.Empty;
					cbTarget.Text = string.Empty;
					cbPropertyOrValue.Text = string.Empty;
					ckIgnoreErrors.Checked = false;

					return;
				}
				else
				{
					error = actionArg.Error;
				}
			}
			catch(Exception e)
			{
				error = e.Message;
			}

			MessageBox.Show(error, "Add Action Argument",
			                MessageBoxButtons.OK, MessageBoxIcon.Error);
		}

		private string BuildExpression()
		{
			if (string.IsNullOrEmpty(cbTarget.Text))
			{
				return cbPropertyOrValue.Text;
			}

			StringBuilder expression = new StringBuilder("$");
			if (ckIgnoreErrors.Checked) expression.Append('!');

			expression.Append(cbTarget.Text);

			if (!string.IsNullOrEmpty(cbPropertyOrValue.Text))
			{
				expression.Append('.').Append(cbPropertyOrValue.Text);
			}

			return expression.ToString();
		}

		private void RemoveActionArgument()
		{
			string name = ddlName.SelectedItem as string;

			if (name != null && actionArgs.Remove(name))
			{
				RefreshActionArguments();
			}
		}

		private void RefreshActionArguments()
		{
			pgActionArguments.Refresh();
			RefreshRemovalCandidates();
		}

		private void RefreshRemovalCandidates()
		{
			ddlName.BeginUpdate();

			ddlName.Items.Clear();

			foreach(ActionArgument actionArg in actionArgs)
			{
				ddlName.Items.Add(actionArg.Name);
			}

			if (ddlName.Items.Count > 0)
			{
				ddlName.SelectedIndex = 0;
				cmdRemove.Enabled = true;
			}
			else
			{
				cmdRemove.Enabled = false;
			}

			ddlName.EndUpdate();
		}

		private void AddDefaultTargets(ActionBinding action)
		{
			WebControl activeControl = action.ControlInstance;

			if (activeControl != null)
			{
				cbTarget.Items.Add(new StandardTarget(activeControl, "this"));

				if (!string.IsNullOrEmpty(action.EventName))
				{
					Type activeEvent = EventUtil.GetEventArgsType(activeControl, action.EventName);

					if (activeEvent != null)
					{
						cbTarget.Items.Add(new StandardTarget(activeEvent, "event"));
					}
					else
					{
						cbTarget.Items.Add(new StandardTarget("event"));
					}
				}
			}
		}

		private void PopulateTargetControls()
		{
			IDesignerHost host = (IDesignerHost) context.GetService(typeof(IDesignerHost));

			if (host != null)
			{
				IContainer container = host.Container;
				IComponent target = context.Instance as IComponent;

				if ((target != null) && (target.Site != null))
				{
					container = target.Site.Container;
				}

				if (container != null)
				{
					WebControl activeControl = null;
					ActionBinding activeAction = ObtainActiveAction();

					if (activeAction != null)
					{
						AddDefaultTargets(activeAction);
						activeControl = activeAction.ControlInstance;
					}

					foreach(IComponent component in container.Components)
					{
						if (component is ControllerBinder) continue;

						WebControl control = component as WebControl;

						if ((control != null) && (control != activeControl) &&
						    (control != host.RootComponent) && !string.IsNullOrEmpty(control.ID))
						{
							cbTarget.Items.Add(new StandardTarget(control, control.ID));
						}
					}
				}
			}
		}

		private void PopulatePropertyNames(ITarget target)
		{
			cbPropertyOrValue.Items.Clear();

			if (target != null)
			{
				if (target.PropertyNames.Length > 0)
				{
					cbPropertyOrValue.Items.Add(string.Empty);

					foreach (string propertyName in target.PropertyNames)
					{
						cbPropertyOrValue.Items.Add(propertyName);
					}
				}
			}

			cbPropertyOrValue.Refresh();
		}

		private ActionBinding ObtainActiveAction()
		{
			return context.Instance as ActionBinding;
		}

		#region Event Handlers

		private void cmdAdd_Click(object sender, EventArgs e)
		{
			AddActionArgument();
		}

		private void cmdRemove_Click(object sender, EventArgs e)
		{
			RemoveActionArgument();
		}

		private void btnOk_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}

		private void cbTarget_SelectedIndexChanged(object sender, EventArgs e)
		{
			PopulatePropertyNames(cbTarget.SelectedItem as ITarget);
		}

		private void cbPropertyOrValue_SelectedIndexChanged(object sender, EventArgs e)
		{
		}

		private void pgActionArguments_SelectedGridItemChanged(
			object sender, SelectedGridItemChangedEventArgs e)
		{
			ddlName.SelectedItem = e.NewSelection.Label;
		}

		#endregion
	}
}

#endif
