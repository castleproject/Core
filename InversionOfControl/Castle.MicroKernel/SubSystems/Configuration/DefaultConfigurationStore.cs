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

	/// <summary>
	/// This implementation of <see cref="IConfigurationStore"/>
	/// does not try to obtain an external configuration by any means.
	/// Its only purpose is to serve as a base class for subclasses
	/// that might obtain the configuration node from anywhere.
	/// </summary>
	public class DefaultConfigurationStore : IConfigurationStore
	{
		private IDictionary _facilities;
		private IDictionary _components;

		public DefaultConfigurationStore()
		{
			_facilities = new HybridDictionary();
			_components = new HybridDictionary();
		}

		#region IConfigurationStore Members

		public void AddFacilityConfiguration(String key, IConfiguration config)
		{
			_facilities[key] = config;
		}

		public void AddComponentConfiguration(String key, IConfiguration config)
		{
			_components[key] = config;
		}

		public IConfiguration GetFacilityConfiguration(String key)
		{
			return _facilities[key] as IConfiguration;
		}

		public IConfiguration GetComponentConfiguration(String key)
		{
			return _components[key] as IConfiguration;
		}

		public IConfiguration[] GetFacilities()
		{
			IConfiguration[] array = new IConfiguration[_facilities.Count];
			
			_facilities.Values.CopyTo(array, 0);

			return array;
		}

		public IConfiguration[] GetComponents()
		{
			IConfiguration[] array = new IConfiguration[_components.Count];
			
			_components.Values.CopyTo(array, 0);

			return array;
		}

		#endregion

		#region ISubSystem Members

		public void Init(IKernel kernel)
		{
		}

		public void Terminate()
		{
		}

		#endregion
	}
}
