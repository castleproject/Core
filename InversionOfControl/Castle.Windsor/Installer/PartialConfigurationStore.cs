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
// See the License for the specific la

namespace Castle.Windsor.Installer
{
	using System;
	using Castle.Core.Configuration;
	using Castle.Core.Resource;
	using Castle.MicroKernel;
	using Castle.MicroKernel.SubSystems.Configuration;

	internal class PartialConfigurationStore : IConfigurationStore, IDisposable
	{
		private readonly IConfigurationStore inner;
		private readonly IConfigurationStore partial;
		
		public PartialConfigurationStore(IKernel kernel)
		{
			inner = kernel.ConfigurationStore;
			partial = new DefaultConfigurationStore();
			partial.Init(kernel);
		}
		
		#region IConfigurationStore Members

		public void AddFacilityConfiguration(String key, IConfiguration config)
		{
			inner.AddFacilityConfiguration(key, config);
			partial.AddFacilityConfiguration(key, config);
		}

		public void AddComponentConfiguration(String key, IConfiguration config)
		{
			inner.AddComponentConfiguration(key, config);
			partial.AddComponentConfiguration(key, config);
		}

		public void AddBootstrapComponentConfiguration(String key, IConfiguration config)
		{
			inner.AddBootstrapComponentConfiguration(key, config);
			partial.AddBootstrapComponentConfiguration(key, config);			
		}

		public void AddChildContainerConfiguration(String name, IConfiguration config)
		{
			inner.AddChildContainerConfiguration(name, config);
			partial.AddChildContainerConfiguration(name, config);			
		}

		public IConfiguration GetChildContainerConfiguration(String key)
		{
			return partial.GetChildContainerConfiguration( key );
		}

		public IConfiguration GetFacilityConfiguration(String key)
		{
			return partial.GetFacilityConfiguration(key);
		}

		public IConfiguration GetComponentConfiguration(String key)
		{
			return partial.GetComponentConfiguration(key);
		}

		public IConfiguration GetBootstrapComponentConfiguration(String key)
		{
			return partial.GetBootstrapComponentConfiguration(key);
		}

		public IConfiguration[] GetFacilities()
		{
			return partial.GetFacilities();
		}

		public IConfiguration[] GetComponents()
		{
			return partial.GetComponents();
		}

		public IConfiguration[] GetBootstrapComponents()
		{
			return partial.GetBootstrapComponents();
		}

		public IConfiguration[] GetConfigurationForChildContainers()
		{
			return partial.GetConfigurationForChildContainers();
		}

		public IResource GetResource(String resourceUri, IResource resource)
		{
			return inner.GetResource(resourceUri, resource);
		}

		#endregion

		#region ISubSystem Members

		public void Init(IKernel kernel)
		{
			partial.Init(kernel);
		}

		public void Terminate()
		{
			partial.Terminate();
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			Terminate();
		}

		#endregion
	}
}
