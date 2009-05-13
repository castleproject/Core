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

namespace Castle.ActiveRecord
{
	using System;

	/// <summary>
	/// Marks an NHibernate 2.0 event listener. The decorated class must implement
	/// at least one of the I...EventListener-interfaces at NHibernate.Event
	/// </summary>
	[AttributeUsage(AttributeTargets.Class,AllowMultiple=false)]
	public class EventListenerAttribute : Attribute
	{
		/// <summary>
		/// If <c>true</c>, any existing listeners for that Event will be replaced.
		/// Otherwise the listener will be added without removing the existing listeners.
		/// </summary>
		public bool ReplaceExisting { get; set; }

		/// <summary>
		/// If <c>true</c>, the listener won't be registered at all.
		/// </summary>
		public bool Ignore { get; set; }

		/// <summary>
		/// Defines that a single event should be skipped although it is defined in the
		/// listener
		/// </summary>
		public Type[] SkipEvent { get; set;}

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
	}
}
