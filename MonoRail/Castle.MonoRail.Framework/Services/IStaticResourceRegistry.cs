// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Services
{
	using System;

	/// <summary>
	/// Pendent
	/// </summary>
	public interface IStaticResourceRegistry
	{
		/// <summary>
		/// Registers an assembly resource.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="location">The location.</param>
		/// <param name="version">The version.</param>
		/// <param name="assemblyName">Name of the assembly.</param>
		/// <param name="resourceName">Resource name.</param>
		/// <param name="resourceEntry">The resource entry name/key.</param>
		/// <param name="mimeType">Mime-type.</param>
		/// <param name="lastModified">The last modified.</param>
		void RegisterAssemblyResource(string name, string location, string version,
		                              string assemblyName, string resourceName,
		                              string resourceEntry, string mimeType, DateTime? lastModified);

		/// <summary>
		/// Registers a custom resource.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="location">The location.</param>
		/// <param name="version">The version.</param>
		/// <param name="resource">The resource.</param>
		/// <param name="mimeType">Mime-type.</param>
		/// <param name="lastModified">The last modified.</param>
		void RegisterCustomResource(string name, string location, string version,
		                            Castle.Core.Resource.IResource resource,
		                            string mimeType, DateTime? lastModified);

		/// <summary>
		/// Checks whether the resource exists for name, location and version
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="location">The location.</param>
		/// <param name="version">The version.</param>
		/// <returns></returns>
		bool Exists(string name, string location, string version);

		/// <summary>
		/// Gets the a resource last modified.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="location">The location.</param>
		/// <param name="version">The version.</param>
		/// <returns></returns>
		DateTime? GetLastModified(string name, string location, string version);

		/// <summary>
		/// Gets the resource content identified by the name, location and version.
		/// </summary>
		/// <param name="name">The name.</param>
		/// <param name="location">The location.</param>
		/// <param name="version">The version.</param>
		/// <param name="mimeType">Mime-type.</param>
		/// <param name="lastModified">The last modified.</param>
		/// <returns></returns>
		string GetResource(string name, string location, string version, out string mimeType, out DateTime? lastModified);
	}
}