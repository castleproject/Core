// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using Castle.Core;
	using Castle.MicroKernel.Registration;

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
	public interface IKernel : IServiceProviderEx, IKernelEvents, IDisposable
	{
		/// <summary>
		/// Adds a concrete class as a component
		/// </summary>
		/// <param name="key"></param>
		/// <param name="classType"></param>
		void AddComponent(String key, Type classType);

		/// <summary>
		/// Adds a concrete class
		/// as a component with the specified <paramref name="lifestyle"/>.
		/// </summary>
		/// <param name="key">The key with which to index the component.</param>
		/// <param name="classType">The <see cref="Type"/> of the component.</param>
		/// <param name="lifestyle">The specified <see cref="LifestyleType"/> for the component.</param>
		/// <remarks>
		/// If you have indicated a lifestyle for the specified <paramref name="classType"/> using
		/// attributes, this method will not overwrite that lifestyle. To do that, use the
		/// <see cref="AddComponent(string,Type,LifestyleType,bool)"/> method.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="key"/>, or <paramref name="classType"/>
		/// are <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="lifestyle"/> is <see cref="LifestyleType.Undefined"/>.
		/// </exception>
		void AddComponent(String key, Type classType, LifestyleType lifestyle);

		/// <summary>
		/// Adds a concrete class
		/// as a component with the specified <paramref name="lifestyle"/>.
		/// </summary>
		/// <param name="key">The key with which to index the component.</param>
		/// <param name="classType">The <see cref="Type"/> of the component.</param>
		/// <param name="lifestyle">The specified <see cref="LifestyleType"/> for the component.</param>
		/// <param name="overwriteLifestyle">
		/// If <see langword="true"/>, then ignores all other configurations
		/// for lifestyle and uses the value in the <paramref name="lifestyle"/> parameter.
		/// </param>
		/// <remarks>
		/// If you have indicated a lifestyle for the specified <paramref name="classType"/> using
		/// attributes, this method will not overwrite that lifestyle. To do that, use the
		/// <see cref="AddComponent(string,Type,Type,LifestyleType,bool)"/> method.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="key"/> or <paramref name="classType"/>
		/// are <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException" />
		/// Thrown if <paramref name="lifestyle"/> is <see cref="LifestyleType.Undefined"/>.
		void AddComponent(String key, Type classType, LifestyleType lifestyle, bool overwriteLifestyle);

		/// <summary>
		/// Adds a concrete class and an interface 
		/// as a component
		/// </summary>
		/// <param name="key">The key with which to index the component.</param>
		/// <param name="serviceType">The service <see cref="Type"/> that this component implements.</param>
		/// <param name="classType">The <see cref="Type"/> of the component.</param>
		void AddComponent(String key, Type serviceType, Type classType);

		/// <summary>
		/// Adds a concrete class and an interface 
		/// as a component with the specified <paramref name="lifestyle"/>.
		/// </summary>
		/// <param name="key">The key with which to index the component.</param>
		/// <param name="serviceType">The service <see cref="Type"/> that this component implements.</param>
		/// <param name="classType">The <see cref="Type"/> of the component.</param>
		/// <param name="lifestyle">The specified <see cref="LifestyleType"/> for the component.</param>
		/// <remarks>
		/// If you have indicated a lifestyle for the specified <paramref name="classType"/> using
		/// attributes, this method will not overwrite that lifestyle. To do that, use the
		/// <see cref="AddComponent(string,Type,Type,LifestyleType,bool)"/> method.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="key"/>, <paramref name="serviceType"/>, or <paramref name="classType"/>
		/// are <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="lifestyle"/> is <see cref="LifestyleType.Undefined"/>.
		/// </exception>
		void AddComponent(String key, Type serviceType, Type classType, LifestyleType lifestyle);

		/// <summary>
		/// Adds a concrete class and an interface 
		/// as a component with the specified <paramref name="lifestyle"/>.
		/// </summary>
		/// <param name="key">The key with which to index the component.</param>
		/// <param name="serviceType">The service <see cref="Type"/> that this component implements.</param>
		/// <param name="classType">The <see cref="Type"/> of the component.</param>
		/// <param name="lifestyle">The specified <see cref="LifestyleType"/> for the component.</param>
		/// <param name="overwriteLifestyle">
		/// If <see langword="true"/>, then ignores all other configurations
		/// for lifestyle and uses the value in the <paramref name="lifestyle"/> parameter.
		/// </param>
		/// <remarks>
		/// If you have indicated a lifestyle for the specified <paramref name="classType"/> using
		/// attributes, this method will not overwrite that lifestyle. To do that, use the
		/// <see cref="AddComponent(string,Type,Type,LifestyleType,bool)"/> method.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="key"/>, <paramref name="serviceType"/>, or <paramref name="classType"/>
		/// are <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="lifestyle"/> is <see cref="LifestyleType.Undefined"/>.
		/// </exception>
		void AddComponent(string key, Type serviceType, Type classType, LifestyleType lifestyle, bool overwriteLifestyle);

		/// <summary>
		/// Adds a concrete class as a component
		/// </summary>
		void AddComponent<T>();

		/// <summary>
		/// Adds a concrete class
		/// as a component with the specified <paramref name="lifestyle"/>.
		/// </summary>
		/// <param name="lifestyle">The specified <see cref="LifestyleType"/> for the component.</param>
		/// <remarks>
		/// If you have indicated a lifestyle for the specified T using
		/// attributes, this method will not overwrite that lifestyle. To do that, use the
		/// <see cref="AddComponent(string,Type,LifestyleType,bool)"/> method.
		/// </remarks>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="lifestyle"/> is <see cref="LifestyleType.Undefined"/>.
		/// </exception>
		void AddComponent<T>(LifestyleType lifestyle);

		/// <summary>
		/// Adds a concrete class
		/// as a component with the specified <paramref name="lifestyle"/>.
		/// </summary>
		/// <param name="lifestyle">The specified <see cref="LifestyleType"/> for the component.</param>
		/// <param name="overwriteLifestyle">
		/// If <see langword="true"/>, then ignores all other configurations
		/// for lifestyle and uses the value in the <paramref name="lifestyle"/> parameter.
		/// </param>
		/// <remarks>
		/// If you have indicated a lifestyle for the specified T using
		/// attributes, this method will not overwrite that lifestyle. To do that, use the
		/// <see cref="AddComponent(string,Type,LifestyleType,bool)"/> method.
		/// </remarks>
		/// <exception cref="ArgumentException" />
		/// Thrown if <paramref name="lifestyle"/> is <see cref="LifestyleType.Undefined"/>.
		void AddComponent<T>(LifestyleType lifestyle, bool overwriteLifestyle);

		/// <summary>
		/// Adds a concrete class and an interface 
		/// as a component
		/// </summary>
		/// <param name="serviceType">The service <see cref="Type"/> that this component implements.</param>
		void AddComponent<T>(Type serviceType);

		/// <summary>
		/// Adds a concrete class and an interface 
		/// as a component with the specified <paramref name="lifestyle"/>.
		/// </summary>
		/// <param name="serviceType">The service <see cref="Type"/> that this component implements.</param>
		/// <param name="lifestyle">The specified <see cref="LifestyleType"/> for the component.</param>
		/// <remarks>
		/// If you have indicated a lifestyle for the specified T using
		/// attributes, this method will not overwrite that lifestyle. To do that, use the
		/// <see cref="AddComponent(string,Type,Type,LifestyleType,bool)"/> method.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// are <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="lifestyle"/> is <see cref="LifestyleType.Undefined"/>.
		/// </exception>
		void AddComponent<T>(Type serviceType, LifestyleType lifestyle);

		/// <summary>
		/// Adds a concrete class and an interface 
		/// as a component with the specified <paramref name="lifestyle"/>.
		/// </summary>
		/// <param name="serviceType">The service <see cref="Type"/> that this component implements.</param>
		/// <param name="lifestyle">The specified <see cref="LifestyleType"/> for the component.</param>
		/// <param name="overwriteLifestyle">
		/// If <see langword="true"/>, then ignores all other configurations
		/// for lifestyle and uses the value in the <paramref name="lifestyle"/> parameter.
		/// </param>
		/// <remarks>
		/// attributes, this method will not overwrite that lifestyle. To do that, use the
		/// <see cref="AddComponent(string,Type,Type,LifestyleType,bool)"/> method.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// are <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="lifestyle"/> is <see cref="LifestyleType.Undefined"/>.
		/// </exception>
		void AddComponent<T>(Type serviceType, LifestyleType lifestyle, bool overwriteLifestyle);

		/// <summary>
		/// Used mostly by facilities. Adds an instance
		/// to be used as a component.
		/// </summary>
		/// <param name="instance"></param>
		void AddComponentInstance<T>(object instance);

		/// <summary>
		/// Used mostly by facilities. Adds an instance
		/// to be used as a component.
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="instance"></param>
		void AddComponentInstance<T>(Type serviceType, object instance);

		/// <summary>
		/// Adds a concrete class as a component and specify the extended properties.
		/// Used by facilities, mostly.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="classType"></param>
		/// <param name="extendedProperties"></param>
		void AddComponentWithExtendedProperties(String key, Type classType, IDictionary extendedProperties);

		/// <summary>
		/// Adds a concrete class and an interface 
		/// as a component and specify the extended properties.
		/// Used by facilities, mostly.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="serviceType"></param>
		/// <param name="classType"></param>
		/// <param name="extendedProperties"></param>
		void AddComponentWithExtendedProperties(String key, Type serviceType, Type classType, IDictionary extendedProperties);

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
		/// Used mostly by facilities. Adds an instance
		/// to be used as a component.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="serviceType"></param>
		/// <param name="instance"></param>
		/// <param name="classType"></param>
		void AddComponentInstance(string key, Type serviceType, Type classType, object instance);

		/// <summary>
		/// Registers the components provided by the <see cref="IRegistration"/>s
		/// with the <see cref="IKernel"/>.
		/// <param name="registrations">The component registrations.</param>
		/// <returns>The kernel.</returns>
		/// </summary>
		IKernel Register(params IRegistration[] registrations);

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
		/// Returns the component instance by the service type
		/// </summary>
		object this[Type service] { get; }

		/// <summary>
		/// Returns the component instance by the service type
		/// </summary>
		object Resolve(Type service);

		/// <summary>
		/// Returns all the valid component instances by
		/// the service type
		/// </summary>
		/// <param name="service">The service type</param>
		/// <param name="arguments">Arguments to resolve the services</param>
		Array ResolveAll(Type service, IDictionary arguments);

		/// <summary>
		/// Returns all the valid component instances by
		/// the service type
		/// </summary>
		/// <param name="service">The service type</param>
		/// <param name="argumentsAsAnonymousType">Arguments to resolve the services</param>
		Array ResolveAll(Type service, object argumentsAsAnonymousType);

		/// <summary>
		/// Returns the component instance by the service type
		/// using dynamic arguments
		/// </summary>
		/// <param name="service"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		object Resolve(Type service, IDictionary arguments);

		/// <summary>
		/// Returns the component instance by the component key
		/// using dynamic arguments
		/// </summary>
		/// <param name="key"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		object Resolve(String key, IDictionary arguments);

		/// <summary>
		/// Returns the component instance by the service type
		/// using dynamic arguments
		/// </summary>
		/// <param name="service">Service to resolve</param>
		/// <param name="argumentsAsAnonymousType">Arguments to resolve the services</param>
		/// <returns></returns>
		object Resolve(Type service, object argumentsAsAnonymousType);

		/// <summary>
		/// Returns the component instance by the component key
		/// using dynamic arguments
		/// </summary>
		/// <param name="key">Key to resolve</param>
		/// <param name="argumentsAsAnonymousType">Arguments to resolve the services</param>
		/// <returns></returns>
		object Resolve(String key, object argumentsAsAnonymousType);

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="service"></param>
		/// <returns></returns>
		object Resolve(String key, Type service);

		/// <summary>
		/// Returns the component instance by the service type
		/// using dynamic arguments
		/// </summary>
		/// <param name="arguments"></param>
		/// <returns></returns>
		T Resolve<T>(IDictionary arguments);

		/// <summary>
		/// Returns the component instance by the service type
		/// using dynamic arguments
		/// </summary>
		/// <param name="argumentsAsAnonymousType">Arguments to resolve the services</param>
		/// <returns></returns>
		T Resolve<T>(object argumentsAsAnonymousType);

		/// <summary>
		/// Returns the component instance by the component key
		/// </summary>
		/// <returns></returns>
		T Resolve<T>();

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key">Component's key</param>
		/// <typeparam name="T">Service type</typeparam>
		/// <returns>The Component instance</returns>
		T Resolve<T>(String key);

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <typeparam name="T">Service type</typeparam>
		/// <param name="key">Component's key</param>
		/// <param name="arguments"></param>
		/// <returns>The Component instance</returns>
		T Resolve<T>(String key, IDictionary arguments);
		
		/// <summary>
		/// Returns component instances that implement TService
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <returns></returns>
		TService[] ResolveAll<TService>();

		/// <summary>
		/// Returns component instances that implement TService
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <returns></returns>
		TService[] ResolveAll<TService>(IDictionary arguments);

		/// <summary>
		/// Returns component instances that implement TService
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <returns></returns>
		TService[] ResolveAll<TService>(object argumentsAsAnonymousType);

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="service"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		object Resolve(String key, Type service, IDictionary arguments);

		/// <summary>
		/// Associates objects with a component handler,
		/// allowing it to use the specified dictionary
		/// when resolving dependencies
		/// </summary>
		/// <param name="service"></param>
		/// <param name="dependencies"></param>
		void RegisterCustomDependencies(Type service, IDictionary dependencies);

		/// <summary>
		/// Associates objects with a component handler,
		/// allowing it to use the specified dictionary
		/// when resolving dependencies
		/// </summary>
		/// <param name="service"></param>
		/// <param name="dependenciesAsAnonymousType"></param>
		void RegisterCustomDependencies(Type service, object dependenciesAsAnonymousType);

		/// <summary>
		/// Associates objects with a component handler,
		/// allowing it to use the specified dictionary
		/// when resolving dependencies
		/// </summary>
		/// <param name="key"></param>
		/// <param name="dependencies"></param>
		void RegisterCustomDependencies(String key, IDictionary dependencies);

		/// <summary>
		/// Associates objects with a component handler,
		/// allowing it to use the specified dictionary
		/// when resolving dependencies
		/// </summary>
		/// <param name="key"></param>
		/// <param name="dependenciesAsAnonymousType"></param>
		void RegisterCustomDependencies(String key, object dependenciesAsAnonymousType);

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
		/// Gets or sets the implementation for <see cref="IReleasePolicy"/>
		/// </summary>
		IReleasePolicy ReleasePolicy { get; set; }

		/// <summary>
		/// Returns the implementation for <see cref="IDependencyResolver"/>
		/// </summary>
		IDependencyResolver Resolver { get; }

		/// <summary>
		/// Adds a <see cref="IFacility"/> to the kernel.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="facility"></param>
		/// <returns></returns>
		IKernel AddFacility(String key, IFacility facility);

		/// <summary>
		/// Creates and adds an <see cref="IFacility"/> facility to the kernel.
		/// </summary>
		/// <typeparam name="T">The facility type.</typeparam>
		/// <param name="key"></param>
		IKernel AddFacility<T>(String key) where T : IFacility, new();

		/// <summary>
		/// Creates and adds an <see cref="IFacility"/> facility to the kernel.
		/// </summary>
		/// <typeparam name="T">The facility type.</typeparam>
		/// <returns></returns>
		IKernel AddFacility<T>() where T : IFacility, new();

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

		/// <summary>
		/// Raise the hanlder registered event, required so
		/// dependant handlers will be notified about their dependant moving
		/// to valid state.
		/// </summary>
		/// <param name="handler"></param>
		void RaiseHandlerRegistered(IHandler handler);

		void RegisterHandlerForwarding(Type forwardedType, string name);
	}
}
