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
	using System;
	using System.ComponentModel;
	using System.Web.UI;

	/// <summary>
	/// Pendent
	/// </summary>
	[Serializable]
	[TypeConverter(typeof(ExpandableObjectConverter))]	
	public class CommandBinding : AbstractBindingComponent
	{
		private string actionName = "";
		private string commandName = "";
		private ActionArgumentCollection actionArguments;

		/// <summary>
		/// Gets or sets the name of the action.
		/// </summary>
		/// <value>The name of the action.</value>
		[Category("Behavior"), DefaultValue("")]
		[Description("The name of the controller action to call for the command.")]
		[TypeConverter(typeof(ActionListConverter))]
		public string ActionName
		{
			get { return Trim(actionName); }
			set { actionName = value; }
		}

		/// <summary>
		/// Gets or sets the name of the command.
		/// </summary>
		/// <value>The name of the command.</value>
		[Category("Behavior"), DefaultValue("")]
		[Description("The name command to that will trigger the controller action.")]
		public string CommandName
		{
			get { return Trim(commandName); }
			set { commandName = value; }
		}

		/// <summary>
		/// Gets the action arguments.
		/// </summary>
		/// <value>The action arguments.</value>
		[Category("Behavior")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Description("Arguments passed to the controller action.")]
		public ActionArgumentCollection ActionArguments
		{
			get
			{
				if (actionArguments == null)
				{
					actionArguments = new ActionArgumentCollection();
				}

				return actionArguments;
			}
		}

		/// <summary>
		/// Validates this instance.
		/// </summary>
		protected override void Validate()
		{
			if (string.IsNullOrEmpty(commandName))
			{
				this["CommandName"] = "must be specified";
			}
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </returns>
		public override string ToString()
		{
			if (string.IsNullOrEmpty(commandName))
			{
				return "Command";
			}

			if (!string.IsNullOrEmpty(actionName))
			{
				return string.Format("{0} => {1}", commandName, actionName);
			}
			
			return commandName;
		}
	}
}

#endif
