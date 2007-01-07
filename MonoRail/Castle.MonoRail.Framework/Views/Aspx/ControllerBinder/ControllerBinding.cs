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
	using System;
	using System.ComponentModel;
	using System.Web.UI;

	[Serializable]
	[DefaultProperty("ActionBindings")]
	[ParseChildren(true, "ActionBindings"), PersistChildren(false)]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ControllerBinding : AbstractBindingComponent
	{
		private string controlID = "";
		private Control controlInstance;
		private ControllerBinder binder;
		private ActionBindingCollection actionBindings;

		public ControllerBinding()
		{
			actionBindings = new ActionBindingCollection(this);
		}

		public ControllerBinding(ControllerBinder binder) : this()
		{
			Binder = binder;
		}

		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ControllerBinder Binder
		{
			get { return binder; }
			set { binder = value; }
		}

		[Browsable(true)]
		[NotifyParentProperty(true)]
		[Description("The ID of the control to that is bound."), DefaultValue("")]
		[TypeConverter(typeof(BindableControlIDConverter))]
		public string ControlID
		{
			get { return controlID; }
			set
			{
				value = Trim(value);

				if (controlID != value)
				{
					controlID = value;
					ActionBindings.Clear();
				}
			}
		}

		[Browsable(false)]
		[NotifyParentProperty(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Control ControlInstance
		{
			get { return controlInstance; }
			set
			{
				if (controlInstance != value)
				{
					controlInstance = value;

					if (controlInstance != null)
					{
						ControlID = controlInstance.ID;
					}
				}
			}
		}

		[Category("Behavior")]
		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ActionBindingCollection ActionBindings
		{
			get { return actionBindings; }
		}

		protected override void Validate()
		{
			if (string.IsNullOrEmpty(controlID))
			{
				this["ControlId"] = "must be specified";
			}
		}

		public override string ToString()
		{
			int actionCount = ActionBindings.Count;

			return string.Format("{0} ({1} action{2})",
				controlID, actionCount, actionCount != 1 ? "s" : "");
		}
	}
}

#endif
