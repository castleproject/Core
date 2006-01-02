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

	/// <summary> Class to manage the text resource for the Velocity
	/// Runtime.
	/// *
	/// </summary>
	/// <author> <a href="mailto:jvanzyl@apache.org">Jason van Zyl</a>
	/// </author>
	/// <author> <a href="mailto:paulo.gaspar@krankikom.de">Paulo Gaspar</a>
	/// </author>
	/// <author> <a href="mailto:geirm@optonline.net">Geir Magnusson Jr.</a>
	/// </author>
	/// <version> $Id: ResourceManager.cs,v 1.4 2004/01/19 20:33:14 corts Exp $
	///
	/// </version>
	public struct ResourceManager_Fields
	{
		public const int RESOURCE_TEMPLATE = 1;
		public const int RESOURCE_CONTENT = 2;
	}

	public interface ResourceManager
	{
		/// <summary> A template resources.
		/// </summary>
		/// <summary> A static content resource.
		/// </summary>
		/// <summary> Initialize the ResourceManager.
		/// </summary>
		void initialize(RuntimeServices rs);

		/// <summary> Gets the named resource.  Returned class type corresponds to specified type
		/// (i.e. <code>Template</code> to <code>RESOURCE_TEMPLATE</code>).
		/// *
		/// </summary>
		/// <param name="resourceName">The name of the resource to retrieve.
		/// </param>
		/// <param name="resourceType">The type of resource (<code>RESOURCE_TEMPLATE</code>,
		/// <code>RESOURCE_CONTENT</code>, etc.).
		/// </param>
		/// <param name="encoding"> The character encoding to use.
		/// </param>
		/// <returns>Resource with the template parsed and ready.
		/// @throws ResourceNotFoundException if template not found
		/// from any available source.
		/// @throws ParseErrorException if template cannot be parsed due
		/// to syntax (or other) error.
		/// @throws Exception if a problem in parse
		///
		/// </returns>
		Resource getResource(String resourceName, int resourceType, String encoding);

		/// <summary>  Determines is a template exists, and returns name of the loader that
		/// provides it.  This is a slightly less hokey way to support
		/// the Velocity.templateExists() utility method, which was broken
		/// when per-template encoding was introduced.  We can revisit this.
		/// </summary>
		/// <param name="resourceName">Name of template or content resource
		/// </param>
		/// <returns>class name of loader than can provide it
		///
		/// </returns>
		String getLoaderNameForResource(String resourceName);
	}
}