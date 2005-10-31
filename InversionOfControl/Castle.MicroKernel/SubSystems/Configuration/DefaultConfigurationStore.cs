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

namespace Castle.MicroKernel.SubSystems.Configuration
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	using Castle.Model.Configuration;
	using Castle.Model.Resource;

	using Castle.MicroKernel.SubSystems.Resource;

	/// <summary>
	/// This implementation of <see cref="IConfigurationStore"/>
	/// does not try to obtain an external configuration by any means.
	/// Its only purpose is to serve as a base class for subclasses
	/// that might obtain the configuration node from anywhere.
	/// </summary>
	[Serializable]
	public class DefaultConfigurationStore : AbstractSubSystem, IConfigurationStore
	{
		private IDictionary facilities;
		private IDictionary components;

		public DefaultConfigurationStore()
		{
			facilities = new HybridDictionary();
			components = new HybridDictionary();
		}

		public void AddFacilityConfiguration(String key, IConfiguration config)
		{
			facilities[key] = config;
		}

		public void AddComponentConfiguration(String key, IConfiguration config)
		{
			components[key] = config;
		}

		public IConfiguration GetFacilityConfiguration(String key)
		{
			return facilities[key] as IConfiguration;
		}

		public IConfiguration GetComponentConfiguration(String key)
		{
			return components[key] as IConfiguration;
		}

		public IConfiguration[] GetFacilities()
		{
			IConfiguration[] array = new IConfiguration[facilities.Count];
			
			facilities.Values.CopyTo(array, 0);

			return array;
		}

		public IConfiguration[] GetComponents()
		{
			IConfiguration[] array = new IConfiguration[components.Count];
			
			components.Values.CopyTo(array, 0);

			return array;
		}

		public IResource GetResource(String resourceUri, IResource resource)
		{
			if (resourceUri.IndexOf(Uri.SchemeDelimiter) == -1)
			{
				return resource.CreateRelative(resourceUri);
			}

			IResourceSubSystem subSystem = (IResourceSubSystem)
				Kernel.GetSubSystem( SubSystemConstants.ResourceKey );

			return subSystem.CreateResource(resourceUri, resource.FileBasePath);
		}
	}
}
