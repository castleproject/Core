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

#if DOTNET2

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System;
	using System.ComponentModel;
	using System.Web.UI;

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

		public ActionBinding()
		{
			ResetCommandDefaults(false);
		}

		[Category("Behavior"), DefaultValue("")]
		[Description("The name of the controller action to call.")]
		[TypeConverter(typeof(ActionListConverter))]
		public string ActionName
		{
			get { return Trim(actionName); }
			set { actionName = value; }
		}

		[Category("Behavior"), DefaultValue("")]
		[Description("The name of the control event to track.")]
		[TypeConverter(typeof(EventListConverter))]
		[RefreshProperties(RefreshProperties.All)]
		public string EventName
		{
			get { return Trim(eventName); }
			set { eventName = value; }
		}

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

		protected override void Validate()
		{
			if (string.IsNullOrEmpty(eventName))
			{
				this["EventName"] = "must be specified";
			}
		}

		internal ControllerBinding Parent
		{
			get { return parent; }
			set { parent = value; }
		}

		internal Control ControlInstance
		{
			get { return (parent != null) ? parent.ControlInstance : null; }
		}

		/// <summary>
		/// Reset the command defaults so they don't get serialized in the html markup.
		/// <param name="isCommandEvent"></param>
		/// </summary>
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
