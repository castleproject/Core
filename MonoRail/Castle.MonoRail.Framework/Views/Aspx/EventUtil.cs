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

#if DOTNET2

namespace Castle.MonoRail.Framework.Views.Aspx
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Reflection;
	using System.Web.UI;
	using System.Web.UI.WebControls;

	internal static class EventUtil
	{
		public static string GetDefaultEventName(Control control)
		{
			EventDescriptor defaultEvent = GetDefaultEvent(control);

			return (defaultEvent != null) ? defaultEvent.Name : "";
		}

		public static EventDescriptor GetDefaultEvent(Control control)
		{
			return GetDefaultEvent(control, null);
		}

		public static EventDescriptor GetEvent(Control control, string eventName)
		{
			EventDescriptorCollection events = GetCompatibleEvents(control);
			return events[eventName];
		}

		public static bool IsCommandEvent(Control control, string eventName)
		{
			return IsCommandEvent(GetEvent(control, eventName));
		}

		public static Type GetEventArgsType(Control control, string eventName)
		{
			EventDescriptor eventDescriptor = GetEvent(control, eventName);

			if (eventDescriptor != null)
			{
				return GetEventArgsType(eventDescriptor);
			}

			return null;
		}

		public static EventDescriptor GetDefaultEvent(Control control,
		                                              Predicate<EventDescriptor> filter)
		{
			EventDescriptor defaultEvent = TypeDescriptor.GetDefaultEvent(control);
			EventDescriptorCollection available = GetCompatibleEvents(control, filter);

			if (available.Count > 0)
			{
				if (available.Find(defaultEvent.Name, false) == null)
				{
					defaultEvent = available[0];
				}
			}

			return defaultEvent;
		}

		public static EventDescriptorCollection GetCompatibleEvents(Control control)
		{
			return GetCompatibleEvents(control, null);
		}

		public static bool HasCompatibleEvents(Control control)
		{
			return GetCompatibleEvents(control).Count > 0;
		}

		public static EventDescriptorCollection GetCompatibleEvents(Control control,
		                                                            Predicate<EventDescriptor> filter)
		{
			List<EventDescriptor> matches = new List<EventDescriptor>();

			EventDescriptorCollection rawEvents = TypeDescriptor.GetEvents(control);

			foreach(EventDescriptor eventDescriptor in rawEvents)
			{
				if (IsCompatibleEvent(eventDescriptor, typeof(EventArgs)) &&
				    (filter == null || filter(eventDescriptor)))
				{
					matches.Add(eventDescriptor);
				}
			}

#if MONO
			return new EventDescriptorCollection(matches.ToArray());
#else
			return new EventDescriptorCollection(matches.ToArray(), true);
#endif
		}

		private static bool IsCompatibleEvent(EventDescriptor eventDescriptor,
		                                      Type eventArgsType)
		{
			if (eventDescriptor == null ||
			    !typeof(EventArgs).IsAssignableFrom(eventArgsType))
			{
				return false;
			}

			if (eventDescriptor.ComponentType == typeof(Control) ||
			    eventDescriptor.ComponentType == typeof(WebControl) ||
				eventDescriptor.ComponentType == typeof(TemplateControl))
			{
				return false;
			}

			Type actualEventArgsType = GetEventArgsType(eventDescriptor);

			return (actualEventArgsType != null && eventArgsType.IsAssignableFrom(actualEventArgsType));
		}

		public static bool IsCommandEvent(EventDescriptor eventDescriptor)
		{
			return IsCompatibleEvent(eventDescriptor, typeof(CommandEventArgs));
		}

		public static Type GetEventArgsType(EventDescriptor eventDescriptor)
		{
			MethodInfo eventMethod = eventDescriptor.EventType.GetMethod("Invoke");

			ParameterInfo[] eventParams = eventMethod.GetParameters();

			if (eventMethod.ReturnType.Equals(typeof(void)) && (eventParams.Length == 2))
			{
				return eventParams[1].ParameterType;
			}

			return null;
		}
	}
}

#endif
