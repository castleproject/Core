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

namespace Castle.MicroKernel
{
	using System;
	using System.Collections;

	using Castle.Model;

	/// <summary>
	/// The <c>IKernel</c> interface exposes all the functionality
	/// the MicroKernel implements.
	/// </summary>
	/// <remarks>
	/// It allows you to register components and
	/// request them by the key or the service they implemented.
	/// It also allow you to register facilities and subsystem, thus 
	/// augmenting the functionality exposed by the kernel alone to fits 
	/// your needs.
	/// <seealso cref="IFacility"/>
	/// <seealso cref="ISubSystem"/>
	/// </remarks>
	public interface IKernel : IKernelEvents, IDisposable
	{
		/// <summary>
		/// Adds a concrete class as a component
		/// </summary>
		/// <param name="key"></param>
		/// <param name="classType"></param>
		void AddComponent(String key, Type classType);

		/// <summary>
		/// Adds a concrete class and an interface 
		/// as a component
		/// </summary>
		/// <param name="key"></param>
		/// <param name="serviceType"></param>
		/// <param name="classType"></param>
		void AddComponent(String key, Type serviceType, Type classType);

		/// <summary>
		/// Adds a concrete class as a component and specify the extended properties.
		/// Used by facilities, mostly.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="classType"></param>
		/// <param name="extendedProperties"></param>
		void AddComponentWithProperties(String key, Type classType, IDictionary extendedProperties);

		/// <summary>
		/// Adds a concrete class and an interface 
		/// as a component and specify the extended properties.
		/// Used by facilities, mostly.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="serviceType"></param>
		/// <param name="classType"></param>
		/// <param name="extendedProperties"></param>
		void AddComponentWithProperties(String key, Type serviceType, Type classType, IDictionary extendedProperties);

		/// <summary>
		/// Adds a custom made <see cref="ComponentModel"/>.
		/// Used by facilities.
		/// </summary>
		/// <param name="model"></param>
		void AddCustomComponent(ComponentModel model);

		/// <summary>
		/// Used mostly by facilities. Adds an instance
		/// to be used as a component.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="instance"></param>
		void AddComponentInstance(String key, object instance);

		/// <summary>
		/// Used mostly by facilities. Adds an instance
		/// to be used as a component.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="serviceType"></param>
		/// <param name="instance"></param>
		void AddComponentInstance(String key, Type serviceType, object instance);

		/// <summary>
		/// Returns true if the specified component was 
		/// found and could be removed (i.e. no other component depends on it)
		/// </summary>
		/// <param name="key">The component's key</param>
		/// <returns></returns>
		bool RemoveComponent(String key);

		/// <summary>
		/// Returns true if the specified key was registered
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		bool HasComponent(String key);

		/// <summary>
		/// Returns true if the specified service was registered
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		bool HasComponent(Type service);

		/// <summary>
		/// Returns the component instance by the key
		/// </summary>
		object this[String key] { get; }

		/// <summary>
		/// Returns the component instance by the Type
		/// </summary>
		object this[Type key] { get; }

        /// <summary>
		/// Releases a component instance. This allows
		/// the kernel to execute the proper decomission 
		/// lifecycles on the component instance.
		/// </summary>
		/// <param name="instance"></param>
		void ReleaseComponent(object instance);
		
		/// <summary>
		/// Constructs an implementation of <see cref="IComponentActivator"/>
		/// for the given <see cref="ComponentModel"/>
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		IComponentActivator CreateComponentActivator(ComponentModel model);

		/// <summary>
		/// Returns the implementation of <see cref="IComponentModelBuilder"/>
		/// </summary>
		IComponentModelBuilder ComponentModelBuilder { get; }

		/// <summary>
		/// Returns the implementation of <see cref="IHandlerFactory"/>
		/// </summary>
		IHandlerFactory HandlerFactory { get; }

		/// <summary>
		/// Gets or sets the implementation of <see cref="IConfigurationStore"/>
		/// </summary>
		IConfigurationStore ConfigurationStore { get; set; }

		/// <summary>
		/// Returns the <see cref="IHandler"/>
		/// for the specified component key.
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		IHandler GetHandler(String key);

		/// <summary>
		/// Returns the <see cref="IHandler"/>
		/// for the specified service.
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		IHandler GetHandler(Type service);


        /// <summary>
        /// Returns the <see cref="IHandler"/>
        /// for the specified service.
        /// This overload is intended for use mainly to resolve
        /// the key handler by the generic type as well.
        /// It is expected that it will be mainly called by <see cref="IDependencyResolver"/>
        /// implementations.
        /// </summary>
        /// <param name="key">the key to match by</param>
        /// <param name="service">The type to match</param>
        /// <returns></returns>
        IHandler GetHandler(string key, Type service);
	    

		/// <summary>
		/// Return handlers for components that 
		/// implements the specified service.
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		IHandler[] GetHandlers(Type service);

		/// <summary>
		/// Return handlers for components that 
		/// implements the specified service. 
		/// The check is made using IsAssignableFrom
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		IHandler[] GetAssignableHandlers(Type service);

		/// <summary>
		/// Returns the implementation for <see cref="IReleasePolicy"/>
		/// </summary>
		IReleasePolicy ReleasePolicy { get; }

		/// <summary>
		/// Returns the implementation for <see cref="IDependencyResolver"/>
		/// </summary>
		IDependencyResolver Resolver { get; }

		/// <summary>
		/// Adds a <see cref="IFacility"/> to the kernel.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="facility"></param>
		void AddFacility(String key, IFacility facility);

		/// <summary>
		/// Returns the facilities registered on the kernel.
		/// </summary>
		/// <returns></returns>
		IFacility[] GetFacilities();

		/// <summary>
		/// Adds (or replaces) an <see cref="ISubSystem"/>
		/// </summary>
		/// <param name="key"></param>
		/// <param name="subsystem"></param>
		void AddSubSystem(String key, ISubSystem subsystem);

		/// <summary>
		/// Returns an implementation of <see cref="ISubSystem"/>
		/// for the specified key. 
		/// <seealso cref="SubSystemConstants"/>
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		ISubSystem GetSubSystem(String key);

		/// <summary>
		/// Gets or sets the implementation of <see cref="IProxyFactory"/>
		/// allowing different strategies for proxy creation.
		/// </summary>
		IProxyFactory ProxyFactory { get; set; }

		/// <summary>
		/// Returns the parent kernel
		/// </summary>
		IKernel Parent { get; set; }

		/// <summary>
		/// Support for kernel hierarchy
		/// </summary>
		/// <param name="kernel"></param>
		void AddChildKernel(IKernel kernel);

		/// <summary>
		/// Remove child kernel
		/// </summary>
		/// <param name="kernel"></param>
		void RemoveChildKernel(IKernel kernel);

		/// <summary>
		/// Graph of components and iteractions.
		/// </summary>
		GraphNode[] GraphNodes { get; }

	}
}
