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
	using System.Collections;

	using Castle.Model;

	/// <summary>
	/// The <code>IKernel</code> interface exposes all the functionality
	/// the MicroKernel must implement.
	/// </summary>
	public interface IKernel : IKernelEvents, IDisposable
	{
		/// <summary>
		/// Adds a concrete class as a component
		/// </summary>
		/// <param name="key"></param>
		/// <param name="classType"></param>
		void AddComponent( String key, Type classType );

		/// <summary>
		/// Adds a concrete class and an interface 
		/// as a component
		/// </summary>
		/// <param name="key"></param>
		/// <param name="serviceType"></param>
		/// <param name="classType"></param>
		void AddComponent( String key, Type serviceType, Type classType );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="classType"></param>
		/// <param name="extendedProperties"></param>
		void AddComponentWithProperties( String key, Type classType, IDictionary extendedProperties );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="serviceType"></param>
		/// <param name="classType"></param>
		/// <param name="extendedProperties"></param>
		void AddComponentWithProperties( String key, Type serviceType, Type classType, IDictionary extendedProperties );

		/// <summary>
		/// Returns true if the specified component was 
		/// found and could be removed (i.e. no other component depends on it)
		/// </summary>
		/// <param name="key">The component's key</param>
		/// <returns></returns>
		bool RemoveComponent( String key );

		/// <summary>
		/// Returns true if the specified key was registered
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		bool HasComponent( String key );

		/// <summary>
		/// Returns true if the specified service was registered
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		bool HasComponent( Type service );

		/// <summary>
		/// 
		/// </summary>
		object this[String key]
		{
			get;
		}

		/// <summary>
		/// 
		/// </summary>
		object this[Type key]
		{
			get;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="instance"></param>
		void ReleaseComponent( object instance );
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		IComponentActivator CreateComponentActivator(ComponentModel model);

		/// <summary>
		/// 
		/// </summary>
		IComponentModelBuilder ComponentModelBuilder
		{
			get; 
		}

		/// <summary>
		/// 
		/// </summary>
		IHandlerFactory HandlerFactory
		{
			get;
		}

		/// <summary>
		/// 
		/// </summary>
		IConfigurationStore ConfigurationStore
		{
			get; set;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		IHandler GetHandler(String key);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		IHandler GetHandler(Type service);

		/// <summary>
		/// Return handlers for components that 
		/// implements the specified service.
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		IHandler[] GetHandlers(Type service);

		/// <summary>
		/// 
		/// </summary>
		IReleasePolicy ReleasePolicy
		{
		 	get;
		}

		/// <summary>
		/// 
		/// </summary>
		IDependencyResolver Resolver
		{
			get;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="facility"></param>
		void AddFacility(String key, IFacility facility);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="subsystem"></param>
		void AddSubSystem(String key, ISubSystem subsystem);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		ISubSystem GetSubSystem(String key);

		/// <summary>
		/// Allows the customization of Proxy creation
		/// </summary>
		IProxyFactory ProxyFactory
		{
			get; set;
		}

		/// <summary>
		/// Returns the parent kernel
		/// </summary>
		IKernel Parent
		{
			get; set;
		}

		/// <summary>
		/// Support for kernel hierarchy
		/// </summary>
		/// <param name="kernel"></param>
		void AddChildKernel(IKernel kernel);
	}
}
