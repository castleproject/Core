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
	using System.Reflection;
	using Framework;

	/// <summary>
	/// Base class for <see cref="AddEventListenerAttribute"/> and <see cref="IgnoreEventListenerAttribute"/>
	/// to hold common attributes and helpers.
	/// </summary>
	public abstract class EventListenerAssemblyAttribute : Attribute
	{
		/// <summary>
		/// The assembly to scan for event listeners or to ignore listeners from.
		/// </summary>
		public Assembly Assembly { get; protected set; }

		/// <summary>
		/// The event listener type to add or ignore
		/// </summary>
		public Type Type { get; protected set; }

		/// <summary>
		/// Parses the constructor argument and sets the specified object
		/// </summary>
		/// <param name="expression">the constructor arg</param>
		protected void ParseStringExpression(string expression)
		{
			try
			{
				Assembly = Assembly.Load(expression);
			}
			catch (Exception ex)
			{
				throw new ActiveRecordInitializationException(string.Format("The IgnoreEventListener attribute was initialized with an assembly named {0}. This assembly cannot be loaded. The reason can be found in the inner exception.", expression), ex);
			}			
		}
	}
}