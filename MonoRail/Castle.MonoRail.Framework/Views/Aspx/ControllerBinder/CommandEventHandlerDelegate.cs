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

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System.Collections;
	using System.Web.UI.WebControls;

	internal class CommandEventHandlerDelegate<EventArgType> : AbstractEventScope
		where EventArgType : CommandEventArgs
	{
		private ActionArgumentCollection actionArgs;

		public CommandEventHandlerDelegate(BindingContext context)
			: base(context)
		{
		}

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

		protected override void AddActionArguments(BindingContext context,
												   IDictionary resolvedActionArgs)
		{
			if (actionArgs != null)
			{
				context.ResolveActionArguments(actionArgs, resolvedActionArgs);
			}
		}

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
