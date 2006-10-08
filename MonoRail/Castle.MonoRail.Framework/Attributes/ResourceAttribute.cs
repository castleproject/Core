// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework
{
	using System;

	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Declares that for the specified class or method, the given resource file should be 
	/// loaded and set available in the PropertyBag with the specified name.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple=true, Inherited=true), Serializable]
	public class ResourceAttribute : Attribute, IResourceDescriptorBuilder
	{
		private String name, resourceName, cultureName, assemblyName;
		private Type resourceType;
		
		/// <summary>
		/// Constructs a resource attribute, with the specified name, based
		/// on the resource in a satellite assembly.
		/// </summary>
		/// <param name="name">Name the resource will be available as in the PropertyBag</param>
		/// <param name="resourceName">Fully qualified name of the resource in the sattelite assembly</param>
		public ResourceAttribute( String name, String resourceName )
		{
			this.name = name;
			this.resourceName = resourceName;

			if (resourceName.IndexOf(',') > 0)
			{
				String[] pair = resourceName.Split(',');
				this.resourceName = pair[0].Trim();
				this.assemblyName = pair[1].Trim();
			}
		}

		/// <summary>
		/// Gets or sets the Name the of resource that will be available in the PropertyBag.
		/// </summary>
		/// <value>The name.</value>
		public String Name
		{
			get { return name; }
			set { name = value; }
		}

		/// <summary>
		/// Gets or sets the Fully qualified name of the resource in the sattelite assembly.
		/// </summary>
		/// <value>The name of the resource.</value>
		public String ResourceName
		{
			get { return resourceName; }
			set { resourceName = value; }
		}

		/// <summary>
		/// Gets or sets the name of the culture.
		/// </summary>
		/// <value>The name of the culture.</value>
		public String CultureName
		{
			get { return cultureName; }
			set { cultureName = value; }
		}

		/// <summary>
		/// Gets or sets the name of the assembly.
		/// </summary>
		/// <value>The name of the assembly.</value>
		public String AssemblyName
		{
			get { return assemblyName; }
			set { assemblyName = value; }
		}

		/// <summary>
		/// Gets or sets the type of the resource.
		/// </summary>
		/// <value>The type of the resource.</value>
		public Type ResourceType
		{
			get { return resourceType; }
			set { resourceType = value; }
		}

		/// <summary>
		/// <see cref="IResourceDescriptorBuilder"/> implementation.
		/// Builds the resource descriptors.
		/// </summary>
		/// <returns></returns>
		public ResourceDescriptor[] BuildResourceDescriptors()
		{
			return new ResourceDescriptor[] { new ResourceDescriptor(resourceType, name, resourceName, cultureName, assemblyName) };
		}
	}
}
