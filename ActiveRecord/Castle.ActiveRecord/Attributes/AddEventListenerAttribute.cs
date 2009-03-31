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

namespace Castle.ActiveRecord.Attributes
{
	using System;

	/// <summary>
	/// Instructs <see cref="ActiveRecordStarter"/> to add the specified type
	/// or assembly to the event listeners
	/// </summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true)]
	public class AddEventListenerAttribute : EventListenerAssemblyAttribute
	{
		/// <summary>
		/// Adds an assembly to the event listener list. All types that implement
		/// at least one IXxxEventListener interface (<see cref="NHibernate.Event"/>)
		/// are used as event listeners.
		/// </summary>
		/// <param name="assemblyName">The name of the assembly. A partial name is sufficient.</param>
		public AddEventListenerAttribute(string assemblyName)
		{
			ParseStringExpression(assemblyName);
		}

		/// <summary>
		/// Adds the specified type as an event listener.
		/// </summary>
		/// <param name="type">A type that implements at least one event listener inzterface.</param>
		public AddEventListenerAttribute(Type type)
		{
			Type = type;
		}

		/// <summary>
		/// Holds connections to exclude the listener from
		/// </summary>
		public Type[] Exclude { get; set; }

		/// <summary>
		/// Holds connections that the listener will be exclusively added to.
		/// </summary>
		public Type[] Include { get; set; }

		/// <summary>
		/// Holds events to exclude the listener from
		/// </summary>
		public Type[] ExcludeEvent { get; set; }

		/// <summary>
		/// Holds events that the listener will be exclusively added to.
		/// </summary>
		public Type[] IncludeEvent { get; set; }

		/// <summary>
		/// Specifies that all events for all configurations should be served by a single instance
		/// </summary>
		public bool Singleton { get; set; }

		/// <summary>
		/// If <c>true</c>, any existing listeners for that Event will be replaced.
		/// Otherwise the listener will be added without removing the existing listeners.
		/// </summary>
		public bool ReplaceExisting { get; set; }

	}
}