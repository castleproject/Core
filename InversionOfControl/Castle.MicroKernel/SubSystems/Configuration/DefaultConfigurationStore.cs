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

using Castle.MicroKernel.Util;

namespace Castle.MicroKernel.SubSystems.Configuration
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Runtime.CompilerServices;

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
		private readonly IDictionary facilities = new HybridDictionary();
		private readonly IDictionary components = new HybridDictionary();
		private readonly ArrayList facilitiesList = new ArrayList();
		private readonly ArrayList componentsList = new ArrayList();

		public DefaultConfigurationStore()
		{
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddFacilityConfiguration(String key, IConfiguration config)
		{
			facilitiesList.Add(config);

			facilities[key] = config;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public void AddComponentConfiguration(String key, IConfiguration config)
		{
			componentsList.Add(config);

			components[key] = config;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public IConfiguration GetFacilityConfiguration(String key)
		{
			return facilities[key] as IConfiguration;
		}

		[MethodImpl(MethodImplOptions.Synchronized)]
		public IConfiguration GetComponentConfiguration(String key)
		{
#if DOTNET2
            key = GenericTypeNameProvider.StripGenericTypeName(key);
#endif
		    return components[key] as IConfiguration;
		}

	    

	    [MethodImpl(MethodImplOptions.Synchronized)]
		public IConfiguration[] GetFacilities()
		{
			return (IConfiguration[]) facilitiesList.ToArray(typeof(IConfiguration));
		}

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
				Kernel.GetSubSystem( SubSystemConstants.ResourceKey );

			return subSystem.CreateResource(resourceUri, resource.FileBasePath);
		}
	}
}
