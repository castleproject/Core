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
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Reflection;

	internal class EventHandlerFactory
	{
		private class EventHandlerInfo
		{
			public Type EventHandlerType;
			public MethodInfo EventMethod;
		}

		private IDictionary<Type, EventHandlerInfo> EventHandlerCache =
			new Dictionary<Type, EventHandlerInfo>();

		public Delegate CreateActionDelegate(Type genericEventHandlerType, EventDescriptor eventDescriptor,
		                                     BindingContext context)
		{
			Type eventArgsType = EventUtil.GetEventArgsType(eventDescriptor);
			EventHandlerInfo eventHandlerInfo =
				GetEventHandlerInfo(genericEventHandlerType, eventArgsType);

			if (eventHandlerInfo != null)
			{
				object eventHandler = Activator.CreateInstance(
					eventHandlerInfo.EventHandlerType, new object[] {context});

				return Delegate.CreateDelegate(eventDescriptor.EventType, eventHandler,
				                               eventHandlerInfo.EventMethod);
			}

			return null;
		}

		private EventHandlerInfo GetEventHandlerInfo(Type genericEventHandlerType, Type eventArgType)
		{
			EventHandlerInfo eventHandlerInfo;

			if (EventHandlerCache.ContainsKey(eventArgType))
			{
				eventHandlerInfo = EventHandlerCache[eventArgType];
			}
			else
			{
				eventHandlerInfo = new EventHandlerInfo();

				eventHandlerInfo.EventHandlerType
					= genericEventHandlerType.MakeGenericType(new Type[] {eventArgType});
				eventHandlerInfo.EventMethod = eventHandlerInfo.EventHandlerType.GetMethod("HandleEvent");

				EventHandlerCache.Add(eventArgType, eventHandlerInfo);
			}

			return eventHandlerInfo;
		}
	}
}

#endif
