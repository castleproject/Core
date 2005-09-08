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
		private IDictionary _subscribers;

		public EventWiringFacility()
		{
		}

		protected override void Init()
		{
			_subscribers = new HybridDictionary(true);

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
				IConfiguration subscribersNode = model.Configuration.Children["subscribes"];

				if (subscribersNode != null)
				{
					//TODO: Validate the config.

					_subscribers.Add(model.Name, subscribersNode);
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
			if (_subscribers.Contains(model.Name))
			{
				IConfiguration subscribersNode = (IConfiguration) _subscribers[model.Name];

				foreach (IConfiguration subscriberNode in subscribersNode.Children)
				{
					object publisher = GetPublisherInstance(subscriberNode);

					EventInfo eventInfo = GetEventInfo(publisher, subscriberNode);

					string handlerMethodName = subscriberNode.Attributes["handler"];
					Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, instance, handlerMethodName);

					eventInfo.AddEventHandler(publisher, handler);
				}
			}
		}

		private EventInfo GetEventInfo(object publisher, IConfiguration subscriberNode)
		{
			string eventName = subscriberNode.Attributes["event"];

			Type publisherType = publisher.GetType();

			EventInfo eventInfo = publisherType.GetEvent(eventName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);

			if (eventInfo == null)
			{
				throw new EventWiringException(String.Format("Event Not Found. Event Name: {0}. Publisher: {1}", eventName, publisherType.FullName));
			}

			return eventInfo;
		}

		private object GetPublisherInstance(IConfiguration subscriberNode)
		{
			string publisherId = subscriberNode.Attributes["publisher"];

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
