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

namespace NVelocity.Context
{
	using System;

	/// <summary>
	/// Interface describing the application data context.  This set of
	/// routines is used by the application to set and remove 'named' data
	/// object to pass them to the template engine to use when rendering
	/// a template.
	///
	/// This is the same set of methods supported by the original Context
	/// class
	/// </summary>
	/// <seealso cref="NVelocity.Context.AbstractContext"/>
	/// <seealso cref="NVelocity.VelocityContext"/>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a></author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a></author>
	public interface IContext
	{
		/// <summary>
		/// Returns the number of elements in this context
		/// </summary>
		/// <value>The count.</value>
		int Count { get; }

		/// <summary>
		/// Adds a name/value pair to the context.
		/// </summary>
		/// <param name="key">The name to key the provided value with.</param>
		/// <param name="value">The corresponding value.</param>
		Object Put(String key, Object value);

		/// <summary>
		/// Gets the value corresponding to the provided key from the context.
		/// </summary>
		/// <param name="key">The name of the desired value.</param>
		/// <returns>The value corresponding to the provided key.</returns>
		Object Get(String key);

		/// <summary>
		/// Indicates whether the specified key is in the context.
		/// </summary>
		/// <param name="key">The key to look for.</param>
		/// <returns>Whether the key is in the context.</returns>
		Boolean ContainsKey(Object key);

		/// <summary>
		/// Get all the keys for the values in the context
		/// </summary>
		Object[] Keys { get; }

		/// <summary>
		/// Removes the value associated with the specified key from the context.
		/// </summary>
		/// <param name="key">The name of the value to remove.</param>
		/// <returns>The value that the key was mapped to, or <code>null</code> if unmapped.</returns>
		Object Remove(Object key);
	}
}