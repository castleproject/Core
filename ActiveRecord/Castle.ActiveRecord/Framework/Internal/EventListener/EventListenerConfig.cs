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
	using Iesi.Collections;
	using NHibernate.Event;
	using System.Collections.Generic;
	using NHibernate.Cfg;
	using System.Reflection;

	/// <summary>
	/// A configuration class for event listeners
	/// </summary>
	public class EventListenerConfig
	{
		/// <summary>
		/// Whether to replace existing listeners 
		/// </summary>
		public bool ReplaceExisting { get; set; }

		/// <summary>
		/// Whether to ignore the listener completely
		/// </summary>
		public bool Ignore { get; set; }
		
		/// <summary>
		/// Defines that a single event should be skipped although it is defined in the
		/// listener
		/// </summary>
		public Type[] SkipEvent { get; set; }

		/// <summary>
		/// Specifies that all events for all configurations should be served by a single instance
		/// </summary>
		public bool Singleton { get; set; }


		/// <summary>
		/// Defines the base types for which the listener will be added.
		/// </summary>
		public Type[] Include { get; set; }

		/// <summary>
		/// Defines the base types for which the listener will not be added.
		/// </summary>
		public Type[] Exclude { get; set; }

		/// <summary>
		/// The type of the listener
		/// </summary>
		public Type ListenerType { get; private set; }

		/// <summary>
		/// The specific instance to use. If <code>null</code> then a new instance of the
		/// configured <see cref="ListenerType"/> will be created.
		/// </summary>
		public object ListenerInstance { get; private set; }

		/// <summary>
		/// Creates an instance for the given type
		/// </summary>
		/// <param name="listenerType">The listener type to use</param>
		public EventListenerConfig(Type listenerType)
		{
			ListenerType = listenerType;
		}

		/// <summary>
		/// Creates an instance for the given instance
		/// </summary>
		/// <param name="listenerInstance">The listener object to use</param>
		public EventListenerConfig(object listenerInstance)
		{
			ListenerInstance = listenerInstance;
			ListenerType = listenerInstance.GetType();
		}

		/// <summary>
		/// Creates the singleton instance. If the instance is already set, the method does not replace it.
		/// </summary>
		public void CreateSingletonInstance()
		{
			if (ListenerInstance != null) return;
			SetSingletonInstance(Activator.CreateInstance(ListenerType));
		}

		/// <summary>
		/// Sets the singleton instance. If the instance is already set, the method does not replace it.
		/// </summary>
		/// <param name="instance">the instance to set</param>
		public void SetSingletonInstance(object instance)
		{
			if (instance == null) throw new ArgumentNullException("instance");
			if (!ListenerType.IsAssignableFrom(instance.GetType()))
				throw new ArgumentException(string.Format("Singleton instance must be of type {0}, but is of type {1}, which cannnot be assigned to {0}", ListenerType.FullName, instance.GetType().FullName));
			if (ListenerInstance != null) return;
			ListenerInstance = instance;
		}

		/// <summary>
		/// Compares the instance with another one for equality.
		/// </summary>
		/// <param name="obj">The config to compare with</param>
		/// <returns>true if the obj is for the same <see cref="ListenerType"/></returns>
		public override bool Equals(object obj)
		{
			return 
				(obj != null) &&
				(obj is EventListenerConfig) &&
				(ListenerType.Equals(((EventListenerConfig)obj).ListenerType));
		}

		/// <summary>
		/// Object infrastructure
		/// </summary>
		/// <returns>The hashcode</returns>
		public override int GetHashCode()
		{
			return ListenerType.GetHashCode();
		}
	}
}
