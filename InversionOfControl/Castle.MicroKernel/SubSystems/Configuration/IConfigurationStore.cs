// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.MicroKernel
{
	using System;

	using Castle.Model.Configuration;

	/// <summary>
	/// The contract used by the kernel to obtain
	/// external configuration for the components and
	/// facilities.
	/// </summary>
	public interface IConfigurationStore : ISubSystem
	{
		/// <summary>
		/// Associates a configuration node with a facility key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="config"></param>
		void AddFacilityConfiguration( String key, IConfiguration config );

		/// <summary>
		/// Associates a configuration node with a component key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="config"></param>
		void AddComponentConfiguration( String key, IConfiguration config );

		/// <summary>
		/// Returns the configuration node associated with 
		/// the specified facility key. Should return null
		/// if no association exists.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		IConfiguration GetFacilityConfiguration( String key );

		/// <summary>
		/// Returns the configuration node associated with 
		/// the specified component key. Should return null
		/// if no association exists.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		IConfiguration GetComponentConfiguration( String key );

		/// <summary>
		/// Returns all configuration nodes for facilities
		/// </summary>
		/// <returns></returns>
		IConfiguration[] GetFacilities();

		/// <summary>
		/// Returns all configuration nodes for components
		/// </summary>
		/// <returns></returns>
		IConfiguration[] GetComponents();
	}
}
