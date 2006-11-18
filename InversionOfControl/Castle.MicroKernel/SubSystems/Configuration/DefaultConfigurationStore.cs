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

namespace Castle.MicroKernel.SubSystems.Configuration
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Runtime.CompilerServices;

	using Castle.Core.Configuration;
	using Castle.Core.Resource;
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
		private readonly IDictionary facilities = new HybridDictionary();
		private readonly IDictionary components = new HybridDictionary();
		private readonly IDictionary bootstrapcomponents = new HybridDictionary();
		private readonly ArrayList facilitiesList = new ArrayList();
		private readonly ArrayList componentsList = new ArrayList();
		private readonly ArrayList bootstrapComponentsList = new ArrayList();

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultConfigurationStore"/> class.
		/// </summary>
		public DefaultConfigurationStore()
		{
		}

		/// <summary>
		/// Associates a configuration node with a facility key
		/// </summary>
		/// <param name="key">item key</param>
		/// <param name="config">Configuration node</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddFacilityConfiguration(String key, IConfiguration config)
		{
			facilitiesList.Add(config);

			facilities[key] = config;
		}

		/// <summary>
		/// Associates a configuration node with a component key
		/// </summary>
		/// <param name="key">item key</param>
		/// <param name="config">Configuration node</param>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddComponentConfiguration(String key, IConfiguration config)
		{
			componentsList.Add(config);

			components[key] = config;
		}

		/// <summary>
		/// Associates a configuration node with a bootstrap component key
		/// </summary>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddBootstrapComponentConfiguration(string key, IConfiguration config)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Returns the configuration node associated with
		/// the specified facility key. Should return null
		/// if no association exists.
		/// </summary>
		/// <param name="key">item key</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public IConfiguration GetFacilityConfiguration(String key)
		{
			return facilities[key] as IConfiguration;
		}

		/// <summary>
		/// Returns the configuration node associated with
		/// the specified component key. Should return null
		/// if no association exists.
		/// </summary>
		/// <param name="key">item key</param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public IConfiguration GetComponentConfiguration(String key)
		{
		    return components[key] as IConfiguration;
		}

		/// <summary>
		/// Returns the configuration node associated with 
		/// the specified component key. Should return null
		/// if no association exists.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public IConfiguration GetBootstrapComponentConfiguration(string key)
		{
			return bootstrapcomponents[key] as IConfiguration;
		}

		/// <summary>
		/// Returns all configuration nodes for facilities
		/// </summary>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public IConfiguration[] GetFacilities()
		{
			return (IConfiguration[]) facilitiesList.ToArray(typeof(IConfiguration));
		}

		/// <summary>
		/// Returns all configuration nodes for bootstrap components
		/// </summary>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public IConfiguration[] GetBootstrapComponents()
		{
			return (IConfiguration[]) bootstrapComponentsList.ToArray(typeof(IConfiguration));
		}

		/// <summary>
		/// Returns all configuration nodes for components
		/// </summary>
		/// <returns></returns>
		[MethodImpl(MethodImplOptions.Synchronized)]
		public IConfiguration[] GetComponents()
		{
			return (IConfiguration[]) componentsList.ToArray(typeof(IConfiguration));
		}

		public IResource GetResource(String resourceUri, IResource resource)
		{
			if (resourceUri.IndexOf(Uri.SchemeDelimiter) == -1)
			{
				return resource.CreateRelative(resourceUri);
			}

			IResourceSubSystem subSystem = (IResourceSubSystem)
				Kernel.GetSubSystem(SubSystemConstants.ResourceKey);

			return subSystem.CreateResource(resourceUri, resource.FileBasePath);
		}
	}
}
