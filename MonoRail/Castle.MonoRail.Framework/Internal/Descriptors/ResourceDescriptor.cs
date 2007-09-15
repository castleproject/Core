// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Internal
{
	using System;

	/// <summary>
	/// Represents a resource configuration associated with a controller.
	/// </summary>
	public class ResourceDescriptor
	{
		private readonly Type resourceType;
		private readonly string name;
		private readonly string resourceName;
		private readonly string cultureName;
		private readonly string assemblyName;

		/// <summary>
		/// Initializes a new instance of the <see cref="ResourceDescriptor"/> class.
		/// </summary>
		/// <param name="resourceType">Type that has the resource.</param>
		/// <param name="name">The name.</param>
		/// <param name="resourceName">Name of the resource.</param>
		/// <param name="cultureName">Name of the culture.</param>
		/// <param name="assemblyName">Name of the assembly.</param>
		public ResourceDescriptor(Type resourceType, string name, string resourceName, 
		                          string cultureName, string assemblyName)
		{
			this.resourceType = resourceType;
			this.name = name;
			this.resourceName = resourceName;
			this.cultureName = cultureName;
			this.assemblyName = assemblyName;
		}

		/// <summary>
		/// Gets the type that has the resource.
		/// </summary>
		/// <value>The type that has the resource.</value>
		public Type ResourceType
		{
			get { return resourceType; }
		}

		/// <summary>
		/// Gets the name.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Gets the name of the resource.
		/// </summary>
		/// <value>The name of the resource.</value>
		public string ResourceName
		{
			get { return resourceName; }
		}

		/// <summary>
		/// Gets the name of the culture.
		/// </summary>
		/// <value>The name of the culture.</value>
		public string CultureName
		{
			get { return cultureName; }
		}

		/// <summary>
		/// Gets the name of the assembly.
		/// </summary>
		/// <value>The name of the assembly.</value>
		public string AssemblyName
		{
			get { return assemblyName; }
		}
	}
}
