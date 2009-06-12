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

namespace Castle.ActiveRecord.Framework
{
	using System;
	using System.Collections;
	using Iesi.Collections;
	using NHibernate.Event;
	using System.Collections.Generic;
	using NHibernate.Cfg;
	using System.Reflection;

	/// <summary>
	/// This contributor allows easy adding of NHibernate event listeners to
	/// configurations. It implements a kind of multiple-strongly-typed collection
	/// for all event listener types. Those types will be added to all
	/// <see cref="NHibernate.Cfg.Configuration"/>-objects that are not filtered
	/// by the <see cref="INHContributor.AppliesToRootType"/> double dispatch filter.
	/// </summary>
	public class NHEventListeners : AbstractNHContributor
	{
		private Dictionary<Type, ArrayList> listeners = new Dictionary<Type, ArrayList>();

		/// <summary>
		/// Replaces existing listeners instead of adding them if set to <code>true</code>.
		/// </summary>
		public bool ReplaceExistingListeners { get; set; }

		/// <summary>
		/// Adds an event listener that will be added to all configurations served.
		/// </summary>
		/// <typeparam name="TListener">the event listener type to add</typeparam>
		/// <param name="listenerInstance">the listener instance to add</param>
		public void Add<TListener>(TListener listenerInstance)
		{
			var eventTypes = GetEventTypes<TListener>(listenerInstance);
			if (listenerInstance == null) throw new ArgumentNullException("listenerInstance");
			if (eventTypes.Length == 0) throw new ArgumentException("listenerInstance is not an instance of a recognized NHibernate event listener type", "listenerInstance");

			lock (listeners)
			{
				foreach (var eventType in eventTypes)
				{
					if (!listeners.ContainsKey(eventType))
						listeners.Add(eventType, new ArrayList());
					listeners[eventType].Add(listenerInstance);
				}
			}
		}

		/// <summary>
		/// Removes a previously added listener instance
		/// </summary>
		/// <typeparam name="TListener">the type of the instance</typeparam>
		/// <param name="listenerInstance">the instance to remove</param>
		public void Remove<TListener>(TListener listenerInstance)
		{
			var eventTypes = GetEventTypes<TListener>(listenerInstance);
			if (eventTypes.Length == 0) throw new ArgumentException("listenerInstance is not an instance of a recognized NHibernate event listener type", "listenerInstance");

			lock (listeners)
			{
				foreach (var eventType in eventTypes)
				{
					if (listeners.ContainsKey(eventType))
						listeners[eventType].Remove(listenerInstance);
					if (listeners.ContainsKey(eventType) && listeners[eventType].Count == 0)
						listeners.Remove(eventType);
				}
			}
		}

		/// <summary>
		/// Tests if a listener instance has been added
		/// </summary>
		/// <typeparam name="TListener">the instance's type</typeparam>
		/// <param name="listenerInstance">the instance to test for</param>
		public bool Contains<TListener>(TListener listenerInstance)
		{
			var eventTypes = GetEventTypes<TListener>(listenerInstance);
			if (eventTypes.Length==0) throw new ArgumentException("listenerInstance is not an instance of a recognized NHibernate event listener type", "listenerInstance");

			if (listenerInstance == null) return false;

			lock (listeners)
			{
				foreach (var eventType in eventTypes)
				{
					if (listeners.ContainsKey(eventType) && listeners[eventType].Contains(listenerInstance))
						return true;
				}
				return false;
			}
		}

		/// <summary>
		/// Enumerates all listener instances of the given type
		/// </summary>
		/// <typeparam name="TEventType">the requested event type</typeparam>
		/// <returns>all listeners of the requested type</returns>
		public IEnumerable<TEventType> Enumerate<TEventType>()
		{
			if (!listeners.ContainsKey(typeof(TEventType)))
				yield break;

			object[] requestedListeners;
			lock (listeners)
			{
				requestedListeners = listeners[typeof(TEventType)].ToArray();
			}
			foreach (var listener in requestedListeners)
			{
				yield return (TEventType) listener;
			}
		}

		/// <summary>
		/// Configures the configuration with all registered listeners
		/// </summary>
		/// <param name="configuration">the configuration object to add the listeners to</param>
		public override void Contribute(Configuration configuration)
		{
			foreach (var listenerType in listeners.Keys)
			{
				ConfigureListenerType(configuration, listenerType);
			}
		}

		/// <summary>
		/// Configures the configures with the registered listeners of the given type
		/// </summary>
		/// <param name="configuration">the configuration object to add the listeners to</param>
		/// <param name="listenerType">the listener type to use</param>
		public void ConfigureListenerType(Configuration configuration, Type listenerType)
		{
			if (!listeners.ContainsKey(listenerType))
				return;

			var property = GetProperty(listenerType);

			object[] existingListeners = (object[])property.GetValue(configuration.EventListeners, null);
			ArrayList listenersToSet = new ArrayList(existingListeners);
			listenersToSet.AddRange(listeners[listenerType]);
			property.SetValue(configuration.EventListeners, listenersToSet.ToArray(listenerType), null);
		}

		/// <summary>
		/// Returns all event listener interfaces defined by NHibernate
		/// </summary>
		/// <returns>event listener interface types</returns>
		public static IEnumerable<Type> GetEventListenerTypes()
		{
			if (cachedListenerTypes == null)
			{
				cachedListenerTypes = new HashSet<Type>();//List<Type>();
				foreach (var propertyInfo in typeof(EventListeners).GetProperties())
				{
					cachedListenerTypes.Add(propertyInfo.PropertyType.GetElementType());
				}
			}
			return cachedListenerTypes;
		}

		private static HashSet<Type> cachedListenerTypes;

		/// <summary>
		/// Returns the PropertyInfo of the <see cref="EventListeners"/>-class for
		/// a given EventListener-interface.
		/// </summary>
		/// <param name="listenerType">The listener interface</param>
		/// <returns>the property info object</returns>
		public static PropertyInfo GetProperty(Type listenerType)
		{
			foreach (var propertyInfo in typeof(EventListeners).GetProperties())
			{
				if (propertyInfo.PropertyType.GetElementType().Equals(listenerType))
					return propertyInfo;
			}
			return null;
		}

		private static Type[] GetEventTypes<TListenerType>(object listener)
		{
			if (cachedListenerTypes == null)
				GetEventListenerTypes();

			Type typeToInspect = (listener != null) ? listener.GetType() : typeof(TListenerType);

			return Array.FindAll(
				typeToInspect.GetInterfaces(), 
				type=>cachedListenerTypes.Contains(type));
		}
	}
}
