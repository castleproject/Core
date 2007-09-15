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
	[DefaultProperty("EventName")]
	[ParseChildren(true), PersistChildren(false)]
	[TypeConverter(typeof(ActionBindingTypeConverter))]
	public class ActionBinding : AbstractBindingComponent
	{
		private string eventName = "";
		private string actionName = "";
		private ControllerBinding parent;
		private ActionArgumentCollection actionArguments;
		private CommandBindingCollection commandBindings;

		/// <summary>
		/// Initializes a new instance of the <see cref="ActionBinding"/> class.
		/// </summary>
		public ActionBinding()
		{
			ResetCommandDefaults(false);
		}

		/// <summary>
		/// Gets or sets the name of the action.
		/// </summary>
		/// <value>The name of the action.</value>
		[Category("Behavior"), DefaultValue("")]
		[Description("The name of the controller action to call.")]
		[TypeConverter(typeof(ActionListConverter))]
		public string ActionName
		{
			get { return Trim(actionName); }
			set { actionName = value; }
		}

		/// <summary>
		/// Gets or sets the name of the event.
		/// </summary>
		/// <value>The name of the event.</value>
		[Category("Behavior"), DefaultValue("")]
		[Description("The name of the control event to track.")]
		[TypeConverter(typeof(EventListConverter))]
		[RefreshProperties(RefreshProperties.All)]
		public string EventName
		{
			get { return Trim(eventName); }
			set { eventName = value; }
		}

		/// <summary>
		/// Gets the command bindings.
		/// </summary>
		/// <value>The command bindings.</value>
		[Category("Behavior")]
		[PersistenceMode(PersistenceMode.InnerProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
		[Description("Maps control commands to controller actions.")]
		public CommandBindingCollection CommandBindings
		{
			get
			{
				if (commandBindings == null)
				{
					commandBindings = new CommandBindingCollection();
				}

				return commandBindings;
			}
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
			if (string.IsNullOrEmpty(eventName))
			{
				this["EventName"] = "must be specified";
			}
		}

		/// <summary>
		/// Gets or sets the parent.
		/// </summary>
		/// <value>The parent.</value>
		internal ControllerBinding Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		/// <summary>
		/// Gets the control instance.
		/// </summary>
		/// <value>The control instance.</value>
		internal Control ControlInstance
		{
			get { return (parent != null) ? parent.ControlInstance : null; }
		}

		/// <summary>
		/// Reset the command defaults so they don't get serialized in the html markup.
		/// </summary>
		/// <param name="isCommandEvent">if set to <c>true</c> [is command event].</param>
		internal void ResetCommandDefaults(bool isCommandEvent)
		{
			if (!isCommandEvent)
			{
				if (commandBindings != null)
				{
					commandBindings.Clear();
				}
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
			if (string.IsNullOrEmpty(eventName))
			{
				return "Pick an event";
			}

			if (parent != null && !string.IsNullOrEmpty(parent.ControlID))
			{
				return string.Format("{0}.{1}", parent.ControlID, eventName);
			}

			return eventName;
		}
	}
}

#endif
