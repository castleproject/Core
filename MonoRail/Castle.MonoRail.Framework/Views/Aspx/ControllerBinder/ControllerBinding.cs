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
	[DefaultProperty("ActionBindings")]
	[ParseChildren(true, "ActionBindings"), PersistChildren(false)]
	[TypeConverter(typeof(ExpandableObjectConverter))]
	public class ControllerBinding : AbstractBindingComponent
	{
		private string controlID = "";
		private Control controlInstance;
		private ControllerBinder binder;
		private ActionBindingCollection actionBindings;

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerBinding"/> class.
		/// </summary>
		public ControllerBinding()
		{
			actionBindings = new ActionBindingCollection(this);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ControllerBinding"/> class.
		/// </summary>
		/// <param name="binder">The binder.</param>
		public ControllerBinding(ControllerBinder binder) : this()
		{
			Binder = binder;
		}

		/// <summary>
		/// Gets or sets the binder.
		/// </summary>
		/// <value>The binder.</value>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ControllerBinder Binder
		{
			get { return binder; }
			set { binder = value; }
		}

		/// <summary>
		/// Gets or sets the control ID.
		/// </summary>
		/// <value>The control ID.</value>
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

		/// <summary>
		/// Gets or sets the control instance.
		/// </summary>
		/// <value>The control instance.</value>
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

		/// <summary>
		/// Gets the action bindings.
		/// </summary>
		/// <value>The action bindings.</value>
		[Category("Behavior")]
		[PersistenceMode(PersistenceMode.InnerDefaultProperty)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public ActionBindingCollection ActionBindings
		{
			get { return actionBindings; }
		}

		/// <summary>
		/// Validates this instance.
		/// </summary>
		protected override void Validate()
		{
			if (string.IsNullOrEmpty(controlID))
			{
				this["ControlId"] = "must be specified";
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
			int actionCount = ActionBindings.Count;

			return string.Format("{0} ({1} action{2})",
				controlID, actionCount, actionCount != 1 ? "s" : "");
		}
	}
}

#endif
