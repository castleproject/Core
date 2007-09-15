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

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System.Collections;
	using System.Web.UI.WebControls;

	internal class CommandEventHandlerDelegate<EventArgType> : AbstractEventScope
		where EventArgType : CommandEventArgs
	{
		private ActionArgumentCollection actionArgs;

		/// <summary>
		/// Initializes a new instance of the <see cref="CommandEventHandlerDelegate&lt;EventArgType&gt;"/> class.
		/// </summary>
		/// <param name="context">The context.</param>
		public CommandEventHandlerDelegate(BindingContext context) : base(context)
		{
		}

		/// <summary>
		/// Handles the event.
		/// </summary>
		/// <param name="sender">The sender.</param>
		/// <param name="e">The e.</param>
		public void HandleEvent(object sender, EventArgType e)
		{
			string actionName = ObtainCommandActionName(e);

			if (!string.IsNullOrEmpty(actionName) ||
				!string.IsNullOrEmpty(Context.Action.ActionName))
			{
				if (string.IsNullOrEmpty(actionName))
				{
					actionName = Context.Action.ActionName;
				}

				DispatchAction(sender, e, actionName);
			}
		}

		/// <summary>
		/// Adds the action arguments.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="resolvedActionArgs">The resolved action args.</param>
		protected override void AddActionArguments(BindingContext context,
												   IDictionary resolvedActionArgs)
		{
			if (actionArgs != null)
			{
				context.ResolveActionArguments(actionArgs, resolvedActionArgs);
			}
		}

		/// <summary>
		/// Obtains the name of the command action.
		/// </summary>
		/// <param name="e">The <see cref="System.Web.UI.WebControls.CommandEventArgs"/> instance containing the event data.</param>
		/// <returns></returns>
		private string ObtainCommandActionName(CommandEventArgs e)
		{
			string actionName = null;

			foreach(CommandBinding command in Context.Action.CommandBindings)
			{
				if (command.CommandName == e.CommandName)
				{
					if (!string.IsNullOrEmpty(command.ActionName))
					{
						actionName = command.ActionName;
						actionArgs = command.ActionArguments;
					}

					break;
				}
			}

			return actionName;
		}
	}
}

#endif
