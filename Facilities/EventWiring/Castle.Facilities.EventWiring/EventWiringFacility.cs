// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.EventWiring
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Reflection;

	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;
	using Castle.Model;
	using Castle.Model.Configuration;


	public class EventWiringFacility : AbstractFacility
	{
		private IDictionary _listeners;

		public EventWiringFacility()
		{
		}

		protected override void Init()
		{
			_listeners = new HybridDictionary(true);

			Kernel.ComponentModelCreated += new ComponentModelDelegate(OnComponentModelCreated);
			Kernel.ComponentCreated += new ComponentInstanceDelegate(OnComponentCreated);
			Kernel.ComponentDestroyed += new ComponentInstanceDelegate(OnComponentDestroyed);
		}

		private void OnComponentModelCreated(ComponentModel model)
		{
			RegisterEventListeners(model);
		}

		private void RegisterEventListeners(ComponentModel model)
		{
			if (model.Configuration != null)
			{
				IConfiguration listenersNode = model.Configuration.Children["listeners"];

				if (listenersNode != null)
				{
					//TODO: Validate the config.

					_listeners.Add(model.Name, listenersNode);
				}
			}
		}

		private void OnComponentDestroyed(ComponentModel model, object instance)
		{
			//TODO: Remove Listener
		}

		private void OnComponentCreated(ComponentModel model, object instance)
		{
			WireEventsIfNeeded(model, instance);
		}

		private void WireEventsIfNeeded(ComponentModel model, object instance)
		{
			if (_listeners.Contains(model.Name))
			{
				IConfiguration listenersNode = (IConfiguration) _listeners[model.Name];

				foreach (IConfiguration listenerNode in listenersNode.Children)
				{
					object publisher = GetPublisherInstance(listenerNode);

					EventInfo eventInfo = GetEventInfo(publisher, listenerNode);

					string handlerMethodName = listenerNode.Attributes["handler"];
					Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, instance, handlerMethodName);

					eventInfo.AddEventHandler(publisher, handler);
				}
			}
		}

		private EventInfo GetEventInfo(object publisher, IConfiguration listenerNode)
		{
			string eventName = listenerNode.Attributes["listening"];

			Type publisherType = publisher.GetType();

			EventInfo eventInfo = publisherType.GetEvent(eventName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);

			if (eventInfo == null)
			{
				throw new EventWiringException(string.Format("Event Not Found. Event Name: {0}. Publisher: {1}", eventName, publisherType.FullName));
			}

			return eventInfo;
		}

		private object GetPublisherInstance(IConfiguration listenerNode)
		{
			string publisherId = listenerNode.Attributes["publisherId"];

			//TODO: Check cyclic dependency

			try
			{
				return Kernel[publisherId];
			}
			catch (Exception e)
			{
				throw new EventWiringException("Error resolving publisher", e);
			}
		}
	}
}
