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
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.ComponentModel.Design;
	using System.Web.UI;

	internal class ActionBindingTypeConverter : ExpandableObjectConverter
	{
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context,
																   object value, Attribute[] attributes)
		{
			PropertyDescriptorCollection properties =
				base.GetProperties(context, value, attributes);

			ActionBinding binding = value as ActionBinding;

			if (binding != null)
			{
				Control control = GetBindingControl(binding.Parent, context);

				if (control != null)
				{
					if (EnsureEventName(control, binding)) context.OnComponentChanged();

					bool isCommandEvent = EventUtil.IsCommandEvent(control, binding.EventName);

					if (!isCommandEvent)
					{
						List<PropertyDescriptor> effective = new List<PropertyDescriptor>();

						foreach (PropertyDescriptor property in properties)
						{
							if (property.Name != "CommandBindings")
							{
								effective.Add(property);
							}
						}

						properties = new PropertyDescriptorCollection(effective.ToArray(), true);
					}

					binding.ResetCommandDefaults(isCommandEvent);
				}
			}

			return properties;
		}

		private Control GetBindingControl(ControllerBinding binding, ITypeDescriptorContext context)
		{
			Control control = binding.ControlInstance;

			if (control == null && binding.ControlID != null)
			{
				IDesignerHost host = (IDesignerHost)context.GetService(typeof(IDesignerHost));

				if (host != null)
				{
					foreach (IComponent component in host.Container.Components)
					{
						control = component as Control;

						if (control != null) break;
					}
				}
			}

			return control;
		}

		private bool EnsureEventName(Control control, ActionBinding binding)
		{
			if (string.IsNullOrEmpty(binding.EventName))
			{
				binding.EventName = SelectDefaultEvent(control, binding.Parent);
				return !string.IsNullOrEmpty(binding.EventName);
			}

			return false;
		}

		private string SelectDefaultEvent(Control control, ControllerBinding binding)
		{
			EventDescriptor defaultEvent = EventUtil.GetDefaultEvent(
				control, delegate(EventDescriptor eventDescriptor)
				{
					string eventName = eventDescriptor.Name;
					return binding.ActionBindings[eventName] == null;
				});

			return (defaultEvent != null) ? defaultEvent.Name : null;
		}
	}
}

#endif