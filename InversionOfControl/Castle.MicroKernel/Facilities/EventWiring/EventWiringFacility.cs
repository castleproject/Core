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

	using Castle.Core;
	using Castle.Core.Configuration;

	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;

	/// <summary>
	/// Facility to allow components to dynamically subscribe to events offered by 
	/// other components. We call the component that offers events publishers and 
	/// the components that uses them, subscribers.
	/// </summary>
	/// <remarks>
	/// A component that wish to subscribe to an event must use the external configuration
	/// adding a node <c>subscribers</c> on the publisher. This node can have multiple entries using the 
	/// <c>subscriber</c> node.
	/// </remarks>
	/// <example>
	/// <para>This example shows two simple components: one is the event publisher and the other is the 
	/// subscriber. The subscription will be done by the facility, using the publisher associated configuration.</para>
	/// <para>The Publisher class:</para>
	/// <code>
	/// public class SimplePublisher
	///	{
	///		public event PublishEventHandler Event;
	///
	///		public void Trigger()
	///		{
	///			if (Event != null)
	///			{
	///				Event(this, new EventArgs()); 
	///			}
	///		}
	/// }
	/// </code>
	/// <para>The Subscriber class:</para>
	/// <code>
	/// public class SimpleListener
	/// {
	/// 	private bool _listened;
	/// 	private object _sender;
	/// 
	/// 	public void OnPublish(object sender, EventArgs e)
	/// 	{
	/// 		_sender = sender; 
	/// 		_listened = sender != null;
	/// 	}
	/// 
	/// 	public bool Listened
	/// 	{
	/// 		get { return _listened;	}
	/// 	}
	/// 
	/// 	public object Sender
	/// 	{
	/// 		get { return _sender; }
	/// 	}
	/// }
	/// </code>
	/// <para>The configuration file:</para>
	/// <code>
	/// <![CDATA[
	/// <?xml version="1.0" encoding="utf-8" ?>
	/// <configuration>
	/// 	<facilities>
	/// 		<facility 
	/// 			id="event.wiring"
	/// 			type="Castle.Facilities.EventWiring.EventWiringFacility, Castle.MicroKernel" />
	/// 	</facilities>
	/// 
	/// 	<components>
	/// 		<component 
	/// 			id="SimpleListener" 
	/// 			type="Castle.Facilities.EventWiring.Tests.Model.SimpleListener, Castle.Facilities.EventWiring.Tests" />
	/// 
	/// 		<component 
	/// 			id="SimplePublisher" 
	/// 			type="Castle.Facilities.EventWiring.Tests.Model.SimplePublisher, Castle.Facilities.EventWiring.Tests" >
	/// 			<subscribers>
	/// 				<subscriber id="SimpleListener" event="Event" handler="OnPublish"/>
	/// 			</subscribers>
	/// 		</component>
	/// 	</components>
	/// </configuration>
	/// ]]>
	/// </code>
	/// </example>
	public class EventWiringFacility : AbstractFacility
	{
		private const string SubscriberList = "evts.subscriber.list";
		
		/// <summary>
		/// Overriden. Initializes the facility, subscribing to the <see cref="IKernelEvents.ComponentModelCreated"/>,
		/// <see cref="IKernelEvents.ComponentCreated"/>, <see cref="IKernelEvents.ComponentDestroyed"/> Kernel events.
		/// </summary>
		protected override void Init()
		{
			Kernel.ComponentModelCreated += new ComponentModelDelegate(OnComponentModelCreated);
			Kernel.ComponentCreated += new ComponentInstanceDelegate(OnComponentCreated);
			Kernel.ComponentDestroyed += new ComponentInstanceDelegate(OnComponentDestroyed);
		}

		#region OnComponentModelCreated

		/// <summary>
		/// Checks if the component we're dealing is a publisher. If it is, 
		/// parses the configuration (the subscribers node) getting the event wiring info.
		/// </summary>
		/// <param name="model">The component model.</param>
		/// <exception cref="EventWiringException">Invalid and/or a error in the configuration</exception>
		private void OnComponentModelCreated(ComponentModel model)
		{
			ExtractAndRegisterEventInformation(model);
		}
		
		private void ExtractAndRegisterEventInformation(ComponentModel model)
		{
			if (IsNotPublishingEvents(model)) return;

			IConfiguration subscribersNode = model.Configuration.Children["subscribers"];

			if (subscribersNode.Children.Count < 1)
			{
				throw new EventWiringException(
					"The subscribers node must have at least an one subsciber child. Check node subscribers of the " 
					+ model.Name + " component");
			}
			
			IDictionary subscribers2Evts = new HybridDictionary();
			
			foreach (IConfiguration subscriber in subscribersNode.Children)
			{
				string subscriberKey = GetSubscriberKey(subscriber);

				AddSubscriberDependecyToModel(subscriberKey, model);

				ExtractAndAddEventInfo(subscribers2Evts, subscriberKey, subscriber, model);
			}

			model.ExtendedProperties[SubscriberList] = subscribers2Evts;
		}

		private void ExtractAndAddEventInfo(IDictionary subscribers2Evts, string subscriberKey, IConfiguration subscriber, ComponentModel model)
		{
			ArrayList wireInfoList = (ArrayList)subscribers2Evts[subscriberKey];

			if (wireInfoList == null)
			{
				wireInfoList = new ArrayList();
				subscribers2Evts[subscriberKey] = wireInfoList;
			}
			
			string eventName = subscriber.Attributes["event"];
			if (eventName == null || eventName.Length == 0)
			{
				throw new EventWiringException("You must supply an 'event' " +
					"attribute which is the event name on the publisher you want to subscribe." +
					" Check node 'subscriber' for component " + model.Name + "and id = " + subscriberKey);
			}

			string handlerMethodName = subscriber.Attributes["handler"];
			if (handlerMethodName == null || handlerMethodName.Length == 0)
			{
				throw new EventWiringException("You must supply an 'handler' attribute " +
					"which is the method on the subscriber that will handle the event." +
					" Check node 'subscriber' for component " + model.Name + "and id = " + subscriberKey);
			}

			wireInfoList.Add(new WireInfo(eventName, handlerMethodName));
		}

		private void AddSubscriberDependecyToModel(string subscriberKey, ComponentModel model)
		{
			DependencyModel dp = new DependencyModel(DependencyType.ServiceOverride, subscriberKey, null, false);
			
			if (!model.Dependencies.Contains(dp))
			{
				model.Dependencies.Add(dp);
			}
		}

		private static string GetSubscriberKey(IConfiguration subscriber)
		{
			string subscriberKey = subscriber.Attributes["id"];
			
			if (subscriberKey == null || subscriberKey.Length == 0)
			{
				throw new EventWiringException("The subscriber node must have a valid Id assigned");
			}
			
			return subscriberKey;
		}

		private bool IsNotPublishingEvents(ComponentModel model)
		{
			return (model.Configuration == null) || (model.Configuration.Children["subscribers"] == null);
		}

		#endregion

		#region OnComponentCreated
		
		/// <summary>
		/// Checks if the component we're dealing is a publisher. If it is, 
		/// iterates the subscribers starting them and wiring the events.
		/// </summary>
		/// <param name="model">The component model.</param>
		/// <param name="instance">The instance representing the component.</param>
		/// <exception cref="EventWiringException">When the subscriber is not found
		/// <br /> or <br/>
		/// The handler method isn't found
		/// <br /> or <br/>
		/// The event isn't found
		/// </exception>
		private void OnComponentCreated(ComponentModel model, object instance)
		{
			if (IsPublisher(model))
			{
				WirePublisher(model, instance);
			}
		}

		private void WirePublisher(ComponentModel model, object publisher)
		{
			StartAndWirePublisherSubscribers(model, publisher);
		}

		private bool IsPublisher(ComponentModel model)
		{
			return model.ExtendedProperties[SubscriberList] != null;
		}

		private void StartAndWirePublisherSubscribers(ComponentModel model, object publisher)
		{
			IDictionary subscribers = (IDictionary)model.ExtendedProperties[SubscriberList];

			if (subscribers == null) return;

			foreach (DictionaryEntry subscriberInfo in subscribers)
			{
				string subscriberKey = (string) subscriberInfo.Key;
				
				IList wireInfoList = (IList) subscriberInfo.Value;

				IHandler handler = Kernel.GetHandler(subscriberKey);

				AssertValidHandler(handler, subscriberKey);

				object subscriberInstance;

				try
				{
					subscriberInstance = handler.Resolve(CreationContext.Empty);
				}
				catch (Exception ex)
				{
					throw new EventWiringException("Failed to start subscriber " + subscriberKey, ex);
				}

				Type publisherType = model.Implementation;
				
				foreach (WireInfo wireInfo in wireInfoList)
				{
					String eventName = wireInfo.EventName;

					//TODO: Caching of EventInfos.
					EventInfo eventInfo = publisherType.GetEvent(eventName,
					                                             BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

					if (eventInfo == null)
					{
						throw new EventWiringException("Could not find event on publisher. Event " +
						                               eventName + " Publisher " + publisherType.FullName);
					}

					MethodInfo handlerMethod = subscriberInstance.GetType().GetMethod(wireInfo.Handler,
					                                                                  BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

					if (handlerMethod == null)
					{
						throw new EventWiringException("Could not find the method '" + wireInfo.Handler +
						                               "' to handle the event " + eventName + ". Subscriber " 
						                               + subscriberInstance.GetType().FullName);
					}

					Delegate delegateHandler = Delegate.CreateDelegate(eventInfo.EventHandlerType,
					                                                   subscriberInstance, wireInfo.Handler);

					eventInfo.AddEventHandler(publisher, delegateHandler);
				}
			}
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

		#endregion

		private void OnComponentDestroyed(ComponentModel model, object instance)
		{
			// TODO: Remove Listener
		}
	}

	/// <summary>
	/// Represents the information about an event.
	/// </summary>
	internal class WireInfo
	{
		private String eventName;
		
		private String handler;

		/// <summary>
		/// Initializes a new instance of the <see cref="WireInfo"/> class.
		/// </summary>
		/// <param name="eventName">Name of the event.</param>
		/// <param name="handler">The name of the handler method.</param>
		public WireInfo(string eventName, string handler)
		{
			this.eventName = eventName;
			this.handler = handler;
		}

		/// <summary>
		/// Gets the name of the event.
		/// </summary>
		/// <value>The name of the event.</value>
		public string EventName
		{
			get { return eventName; }
		}

		/// <summary>
		/// Gets the handler method name.
		/// </summary>
		/// <value>The handler.</value>
		public string Handler
		{
			get { return handler; }
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>
		/// A hash code for the current <see cref="T:System.Object"></see>.
		/// </returns>
		public override int GetHashCode()
		{
			return eventName.GetHashCode() + 29 * handler.GetHashCode();
		}

		/// <summary>
		/// Determines whether the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>.
		/// </summary>
		/// <param name="obj">The <see cref="T:System.Object"></see> to compare with the current <see cref="T:System.Object"></see>.</param>
		/// <returns>
		/// true if the specified <see cref="T:System.Object"></see> is equal to the current <see cref="T:System.Object"></see>; otherwise, false.
		/// </returns>
		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			
			WireInfo wireInfo = obj as WireInfo;
			if (wireInfo == null)
			{
				return false;
			}
			
			if (!Equals(eventName, wireInfo.eventName))
			{
				return false;
			}
			
			if (!Equals(handler, wireInfo.handler))
			{
				return false;
			}
			
			return true;
		}
	}
}