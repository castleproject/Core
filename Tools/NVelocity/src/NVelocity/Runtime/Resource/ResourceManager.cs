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

namespace NVelocity.Runtime.Resource
{
	using System;
	using NVelocity.Exception;

	/// <summary> 
	/// Class to manage the text resource for the Velocity
	/// Runtime.
	/// </summary>
	public enum ResourceType
	{
		/// <summary>
		/// A template resources.
		/// </summary>
		Template = 1,

		/// <summary>
		/// A static content resource.
		/// </summary>
		Content = 2,
	}

	public interface IResourceManager
	{
		/// <summary>
		/// Initialize the ResourceManager.
		/// </summary>
		void Initialize(IRuntimeServices rs);

		/// <summary> 
		/// Gets the named resource. Returned class type corresponds to specified type
		/// (i.e. <c>Template</c> to <c>Template</c>).
		/// </summary>
		/// <param name="resourceName">The name of the resource to retrieve.</param>
		/// <param name="resourceType">The type of resource (<code>Template</code>, <code>Content</code>, etc.).</param>
		/// <param name="encoding">The character encoding to use.</param>
		/// <returns>Resource with the template parsed and ready.</returns>
		/// <exception cref="ResourceNotFoundException">
		/// if template not found from any available source.
		/// </exception>
		/// <exception cref="ParseErrorException">
		/// if template cannot be parsed due to syntax (or other) error.
		/// </exception>
		/// <exception cref="Exception">
		/// if a problem in parse
		/// </exception>
		Resource GetResource(string resourceName, ResourceType resourceType, string encoding);

		/// <summary> 
		/// Determines is a template exists, and returns name of the loader that
		/// provides it.  This is a slightly less hokey way to support
		/// the Velocity.templateExists() utility method, which was broken
		/// when per-template encoding was introduced.  We can revisit this.
		/// </summary>
		/// <param name="resourceName">Name of template or content resource</param>
		/// <returns>class name of loader than can provide it</returns>
		String GetLoaderNameForResource(String resourceName);
	}
}
