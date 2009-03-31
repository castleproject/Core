// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord.Framework.Internal.EventListener
{
	using System;
	using System.Collections;
	using Iesi.Collections.Generic;
	using System.Collections.Generic;
	using NHibernate.Cfg;

	/// <summary>
	/// This contributor supports the complex configuration of event listeners.
	/// </summary>
	public class EventListenerContributor : AbstractNHContributor
	{
		private readonly Dictionary<Type, Set<Type>> listenersPerEvent = 
			new Dictionary<Type, Set<Type>>();

		private readonly Dictionary<Type, EventListenerConfig> listeners = new Dictionary<Type, EventListenerConfig>();

		/// <summary>
		/// Adds an event listener configuration
		/// </summary>
		/// <param name="config">the configuration to add</param>
		/// <returns>the added configuration</returns>
		/// <exception cref="ArgumentNullException">When the configuration is null</exception>
		/// <exception cref="ArgumentException">When the configuration is already present.</exception>
		public EventListenerConfig Add(EventListenerConfig config)
		{
			if (config == null) throw new ArgumentNullException("config");
			if (listeners.ContainsKey(config.ListenerType))
				throw new ArgumentException(string.Format("Configuration for Listener Type {0} is already present.", config.ListenerType.FullName), "config");
			var events = GetEventTypes(config.ListenerType);
			if (events.Length == 0)
				throw new ArgumentException(string.Format("The Listener of type {0} does not implement any known NHibernate event listener interfaces.", config.ListenerType.FullName), "config");

			listeners.Add(config.ListenerType, config);
			foreach (var eventType in events)
			{
				if (!listenersPerEvent.ContainsKey(eventType))
					listenersPerEvent.Add(eventType, new HashedSet<Type>());
				listenersPerEvent[eventType].Add(config.ListenerType);
			}
			return config;
		}

		/// <summary>
		/// Returns the listener config for a specified listener type.
		/// </summary>
		/// <param name="listenerType">the type to look for</param>
		/// <returns>the listener config or null if it does not exist</returns>
		public EventListenerConfig Get(Type listenerType)
		{
			if (listenerType == null) throw new ArgumentNullException("listenerType");
			return (listeners.ContainsKey(listenerType)) ?
				listeners[listenerType] :
				null;
		}

		/// <summary>
		/// Gets and removes the configuration for the type.
		/// </summary>
		/// <param name="listenerType">the type to look for</param>
		/// <returns>the configuration for the type</returns>
		public EventListenerConfig Remove(Type listenerType)
		{
			var config = Get(listenerType);
			if (config != null)
			{
				listeners.Remove(listenerType);
				foreach (var eventListenerCollection in listenersPerEvent.Values)
				{
					eventListenerCollection.Remove(listenerType);
				}
			}
			return config;
		}

		/// <summary>
		/// Configures the configuration with the registered listeners according the config
		/// </summary>
		/// <param name="configuration">the configuration object to add the listeners to</param>
		public override void Contribute(Configuration configuration)
		{
			if (currentActiveRecordConfig == null)
			{
				throw new Exception(@"An internal exception occured. 
The event listener contributor was asked to contribute to a NHibernate config without being asked whether it applies to it.
Please open an issue at support.castleproject.org using this text.");
			}

			foreach (var eventType in eventTypes)
			{
				if (!listenersPerEvent.ContainsKey(eventType) || listenersPerEvent[eventType] == null) 
					continue;

				var currentListeners = CollectListeners(eventType);
				var listenerInstances = Array.ConvertAll(currentListeners, config => GetInstance(config));
				bool replaceExistingListeners = Array.Exists(currentListeners, config => config.ReplaceExisting);

				var listenersToSet = new ArrayList();
				if (!replaceExistingListeners)
					listenersToSet.AddRange(GetExistingListeners(configuration, eventType));
				listenersToSet.AddRange(listenerInstances);

				SetListeners(configuration, eventType, listenersToSet.ToArray(eventType));
			}

		}

		private EventListenerConfig[] CollectListeners(Type type)
		{
			var collectedListeners = new List<EventListenerConfig>();
			foreach (var listenerType in listenersPerEvent[type])
			{
				var config = Get(listenerType);
				if (config.Ignore) continue;
				if (config.SkipEvent != null && Array.Exists(config.SkipEvent,t => t.Equals(type))) continue;
				if (config.Include != null && 
					config.Include.Length > 0 &&
					!Array.Exists(config.Include, t => t.Equals(currentActiveRecordConfig)))
					continue;
				if (config.Exclude != null &&
					config.Exclude.Length > 0 &&
					Array.Exists(config.Exclude, t => t.Equals(currentActiveRecordConfig)))
					continue;

				if (config.Singleton) config.CreateSingletonInstance();

				collectedListeners.Add(config);
			}
			return collectedListeners.ToArray();
		}

		private static object GetInstance(EventListenerConfig config)
		{
			return config.ListenerInstance ?? Activator.CreateInstance(config.ListenerType);
		}

		private static object[] GetExistingListeners(Configuration configuration, Type eventType)
		{
			var property = NHEventListeners.GetProperty(eventType);

			return (object[])property.GetValue(configuration.EventListeners, null);
		}

		private static void SetListeners(Configuration configuration, Type eventType, Array listenersToSet)
		{
			var property = NHEventListeners.GetProperty(eventType);
			property.SetValue(configuration.EventListeners, listenersToSet, null);
		}

		/// <summary>
		/// Creates a listener
		/// </summary>
		public EventListenerContributor()
		{
			AppliesToRootType = delegate(Type rootType)
			{
				currentActiveRecordConfig = rootType;
				return true;
			};
		}
		
		internal static Type[] GetEventTypes(Type listenerType)
		{
			return Array.FindAll(
				listenerType.GetInterfaces(),
				type => eventTypes.Contains(type));
		}

		private Type currentActiveRecordConfig;
		private static readonly List<Type> eventTypes = new List<Type>(NHEventListeners.GetEventListenerTypes());
	}
}
