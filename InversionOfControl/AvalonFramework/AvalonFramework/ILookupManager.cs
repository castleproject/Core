// Copyright 2003-2004 The Apache Software Foundation
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

namespace Apache.Avalon.Framework
{
	using System;

	/// <summary>
	/// A <c>ILookupManager</c> selects components based on a
	/// role. The contract is that all the components implement the
	/// differing roles and there is one component per role.
	/// <para>
	/// Roles are usually the full interface name. A role is better understood 
	/// by the analogy of a play. There are many different roles in a script.
	/// Any actor or actress can play any given part and you get 
	/// the same results (phrases said, movements made, etc.). The exact
	/// nuances of the performance is different.
	/// </para>
	/// </summary>
	public interface ILookupManager
	{
		/// <summary>
		/// Gets the resource associated with the given role.
		/// </summary>
		object this[string role] 
		{
			get; 
		}

		/// <summary>
		/// Checks to see if a component exists for a role.
		/// </summary>
		/// <param name="role">A String identifying the lookup name to check.</param>
		/// <returns>True if the resource exists; otherwise, false.</returns>
		bool Contains(string role);

		/// <summary>
		/// Return the resource when you are finished with it.
		/// This allows the <see cref="ILookupManager"/> to handle 
		/// the End-Of-Life Lifecycle events associated with the component.
		/// </summary>
		/// <remarks>
		/// Please note, that no Exceptions should be thrown at this point.
		/// This is to allow easy use of the <see cref="ILookupManager"/> system without
		/// having to trap Exceptions on a release.
		/// </remarks>
		/// <param name="resource">The resource we are releasing.</param>
		void Release(object resource);

		/// <summary>
		/// Returns a component instance that matches the specified 
		/// criteria.
		/// </summary>
		/// <param name="role">A String identifying the lookup name to check.</param>
		/// <param name="criteria">A specific criteria</param>
		/// <returns>Component instance</returns>
		object LookUp( string role, object criteria );
	}
}
