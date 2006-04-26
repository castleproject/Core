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

namespace Castle.Facilities.EventWiring
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Reflection;

	using Castle.Model;
	using Castle.Model.Configuration;

	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;

	/// <summary>
	/// Facility to allow components to dynamically subscribe to events offered by 
	/// other components. We call the component that offers events publishers and 
	/// the components that uses them, subscribers.
	/// </summary>
	/// <remarks>
	/// A component that wish to subscribe to an event must use the external configuration
	/// adding a node <c>subscribes</c>. This node can have multiple entries using the 
	/// <c>subscribe-to</c> node.
	/// <para>TODO: Add a configuration sample to this documentation</para>
	/// </remarks>
	public class EventWiringFacility : AbstractFacility
	{
		/// <summary>
		/// Maps a <see cref="ComponentModel"/> to a <see cref="IConfiguration"/> that represents 
		/// the <c>subscribes</c> node
		/// </summary>
		private readonly IDictionary model2SubcribeNode = new HybridDictionary();
		
		/// <summary>
		/// Maps the <see cref="ComponentModel"/> of a subscriber to a list of publishers. The M
		/// </summary>
		private readonly IDictionary publishers = new HybridDictionary();

		/// <summary>
		/// Constructs the facility
		/// </summary>
		public EventWiringFacility()
		{
		}

		protected override void Init()
		{
			Kernel.ComponentModelCreated += new ComponentModelDelegate(OnComponentModelCreated);
			Kernel.ComponentCreated += new ComponentInstanceDelegate(OnComponentCreated);
			Kernel.ComponentDestroyed += new ComponentInstanceDelegate(OnComponentDestroyed);
		}

		private void OnComponentModelCreated(ComponentModel model)
		{
			ExtractAndRegisterEventInformation(model);
		}

		private void OnComponentCreated(ComponentModel model, object instance)
		{
			// If the component is a publisher
			StartAndWireSubscribers(model, instance);
		}

		private void OnComponentDestroyed(ComponentModel model, object instance)
		{
			// TODO: Remove Listener
		}

		/// <summary>
		/// Checks whether the component model has, on its configuration, 
		/// a <c>subscribes</c> node. If so adds it to a hashtable associated
		/// with the model, and updates the publisher information adding it 
		/// as a subscriber
		/// </summary>
		/// <param name="model"></param>
		private void ExtractAndRegisterEventInformation(ComponentModel model)
		{
			if (model.Configuration == null) return;

			IConfiguration subscribersNode = model.Configuration.Children["subscribes"];

			if (subscribersNode == null) return;

			// TODO: Validate the config.

			model2SubcribeNode.Add(model, subscribersNode);

			foreach (IConfiguration subscriberNode in subscribersNode.Children)
			{
				String publisherKey = GetPublisherKey(subscriberNode);

				IList subscriberList = null;

				if (!publishers.Contains(publisherKey))
				{
					subscriberList = new ArrayList();

					publishers[publisherKey] = subscriberList;
				}
				else
				{
					subscriberList = (IList) publishers[publisherKey];
				}

				String eventName = subscriberNode.Attributes["event"];

				if (eventName == null || eventName.Length == 0)
				{
					throw new EventWiringException("You must supply an 'event' " + 
						"attribute which is the event name on the publisher you want to subscribe." + 
						" Check node 'subscribe-to' for component " + model.Name);
				}

				String handlerMethodName = subscriberNode.Attributes["handler"];

				if (handlerMethodName == null || handlerMethodName.Length == 0)
				{
					throw new EventWiringException("You must supply an 'handler' attribute " + 
						"which is the method on the subscriber that will handle the event." + 
						" Check node 'subscribe-to' for component " + model.Name);
				}

				subscriberList.Add(new WireInfo(model, eventName, handlerMethodName));
			}
		}

		/// <summary>
		/// Checks if the component we're dealing is a publisher. If it is, 
		/// iterates the subscribers starting them.
		/// </summary>
		/// <param name="model"></param>
		/// <param name="publisher"></param>
		private void StartAndWireSubscribers(ComponentModel model, object publisher)
		{
			IList subscriberList = (IList) publishers[model.Name];

			if (subscriberList == null) return;

			IDictionary createdInstances = new Hashtable();

			foreach(WireInfo wireInfo in subscriberList)
			{
				String subscriberKey = wireInfo.subscriberModel.Name;

				IHandler handler = Kernel.GetHandler(subscriberKey);

				AssertValidHandler(handler, subscriberKey);

				object subscriberInstance = createdInstances[handler];

				try
				{
					if (subscriberInstance == null)
					{
						// TODO: I think there might be a GC issue here
						// meaning that the subscriber can be collected anytime and we need to
						// prevent it

						subscriberInstance = handler.Resolve();

						createdInstances[handler] = subscriberInstance;
					}
				}
				catch(Exception ex)
				{
					throw new EventWiringException("Failed to start subscriber " + subscriberKey, ex);
				}

				Type publisherType = model.Implementation;

				String eventName = wireInfo.eventName;

				EventInfo eventInfo = publisherType.GetEvent(eventName, 
					BindingFlags.Static|BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);

				if (eventInfo == null)
				{
					throw new EventWiringException("Could not find event on publisher. Event " + 
						eventName + " Publisher " + publisherType.FullName);
				}

				MethodInfo handlerMethod = subscriberInstance.GetType().GetMethod(wireInfo.handler,
					BindingFlags.Instance|BindingFlags.Public|BindingFlags.NonPublic);

				if (handlerMethod == null)
				{
					throw new EventWiringException("Could not find the method '" + wireInfo.handler + 
						"' to handle the event " + eventName + ". Subscriber " + subscriberInstance.GetType().FullName);
				}

				Delegate delegateHandler = Delegate.CreateDelegate(eventInfo.EventHandlerType, 
					subscriberInstance, wireInfo.handler);

				eventInfo.AddEventHandler(publisher, delegateHandler);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="model"></param>
		/// <param name="instance"></param>
		private void WireEventsIfNeeded(ComponentModel model, object instance)
		{
//			if (model2SubcribeNode.Contains(model))
//			{
//				IConfiguration subscribersNode = (IConfiguration) model2SubcribeNode[model];
//
//				foreach (IConfiguration subscriberNode in subscribersNode.Children)
//				{
//					object publisher = GetPublisherInstance(subscriberNode);
//
//					EventInfo eventInfo = GetEventInfo(publisher, subscriberNode);
//
//					String handlerMethodName = subscriberNode.Attributes["handler"];
//					Delegate handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, instance, handlerMethodName);
//
//					eventInfo.AddEventHandler(publisher, handler);
//				}
//			}
		}

//		private EventInfo GetEventInfo(object publisher, IConfiguration subscriberNode)
//		{
//			String eventName = subscriberNode.Attributes["event"];
//
//			Type publisherType = publisher.GetType();
//
//			EventInfo eventInfo = publisherType.GetEvent(eventName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
//
//			if (eventInfo == null)
//			{
//				throw new EventWiringException(String.Format("Event Not Found. Event Name: {0}. Publisher: {1}", eventName, publisherType.FullName));
//			}
//
//			return eventInfo;
//		}
//
//		private object GetPublisherInstance(IConfiguration subscriberNode)
//		{
//			String publisherKey = GetPublisherKey(subscriberNode);
//
//			//TODO: Check cyclic dependency
//
//			try
//			{
//				return Kernel[publisherKey];
//			}
//			catch (Exception e)
//			{
//				throw new EventWiringException("Error resolving publisher", e);
//			}
//		}

		private static String GetPublisherKey(IConfiguration subscriberNode)
		{
			return subscriberNode.Attributes["publisher"];
		}

		private static void AssertValidHandler(IHandler handler, string subscriberKey)
		{
			if (handler == null)
			{
				throw new EventWiringException("Publisher tried to start subscriber " + subscriberKey + " that was not found");
			}
	
			if (handler.CurrentState == HandlerState.WaitingDependency)
			{
				throw new EventWiringException("Publisher tried to start subscriber " + subscriberKey + " that is waiting for a dependency");
			}
		}
	}

	/// <summary>
	/// 
	/// </summary>
	internal class WireInfo
	{
		public ComponentModel subscriberModel;
		public String eventName;
		public String handler;

		public WireInfo(ComponentModel subscriberModel, string eventName, string handler)
		{
			this.subscriberModel = subscriberModel;
			this.eventName = eventName;
			this.handler = handler;
		}
	}
}