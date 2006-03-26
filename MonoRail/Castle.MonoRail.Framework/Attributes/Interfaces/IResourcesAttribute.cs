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

namespace Castle.MonoRail.Framework
{
	using System;

	public interface IResourcesAttribute
	{
		/// <summary>
		/// Gets the list of resources to be loaded.
		/// This list should be fixed, as this method will only be called once,
		/// when creating the controller and action descriptors.
		/// </summary>
		ResourceItem[] GetResources();
	}

	public sealed class ResourceItem : IResourceDefinition
	{
		private readonly String key, resourceName;
		private String cultureName, assemblyName;
		private Type resourceType;

		public ResourceItem(string key, string resourceName)
		{
			this.key = key;
			this.resourceName = resourceName;
		}

		public ResourceItem(string key, string resourceName, string cultureName, string assemblyName, Type resourceType)
		{
			this.key = key;
			this.resourceName = resourceName;
			this.cultureName = cultureName;
			this.assemblyName = assemblyName;
			this.resourceType = resourceType;
		}

		public string Key
		{
			get { return key; }
		}

		public string ResourceName
		{
			get { return resourceName; }
		}

		public string CultureName
		{
			get { return cultureName; }
			set { cultureName = value; }
		}

		public string AssemblyName
		{
			get { return assemblyName; }
			set { assemblyName = value; }
		}

		public Type ResourceType
		{
			get { return resourceType; }
			set { resourceType = value; }
		}
	}
}