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
	public partial interface IKernel : IServiceProviderEx, IKernelEvents, IDisposable
	{
		/// <summary>
		/// Registers the components provided by the <see cref="IRegistration"/>s
		/// with the <see cref="IKernel"/>.
		/// <para />
		/// Create a new registration using <see cref="Component"/>.For() or <see cref="AllTypes"/>.
		/// </summary>
		/// <example>
		/// <code>
		/// kernel.Register(Component.For&lt;IService&gt;().ImplementedBy&lt;DefaultService&gt;());
		/// </code>
		/// </example>
		/// <param name="registrations">The component registrations.</param>
		/// <returns>The kernel.</returns>
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
		/// <param name="key"></param>
		/// <param name="onCreate">The callback for creation.</param>
		IKernel AddFacility<T>(String key, Action<T> onCreate) 
			where T : IFacility, new();

		/// <summary>
		/// Creates and adds an <see cref="IFacility"/> facility to the kernel.
		/// </summary>
		/// <typeparam name="T">The facility type.</typeparam>
		/// <param name="key"></param>
		/// <param name="onCreate">The callback for creation.</param>
		IKernel AddFacility<T>(String key, Func<T, object> onCreate)
			where T : IFacility, new();

		/// <summary>
		/// Creates and adds an <see cref="IFacility"/> facility to the kernel.
		/// </summary>
		/// <typeparam name="T">The facility type.</typeparam>
		/// <returns></returns>
		IKernel AddFacility<T>() where T : IFacility, new();

		/// <summary>
		/// Creates and adds an <see cref="IFacility"/> facility to the kernel.
		/// </summary>
		/// <typeparam name="T">The facility type.</typeparam>
		/// <param name="onCreate">The callback for creation.</param>
		/// <returns></returns>
		IKernel AddFacility<T>(Action<T> onCreate) 
			where T : IFacility, new();

		/// <summary>
		/// Creates and adds an <see cref="IFacility"/> facility to the kernel.
		/// </summary>
		/// <typeparam name="T">The facility type.</typeparam>
		/// <param name="onCreate">The callback for creation.</param>
		/// <returns></returns>
		IKernel AddFacility<T>(Func<T, object> onCreate)
			where T : IFacility, new();

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

		void RaiseHandlersChanged();

		/// <summary>
		/// Registers the <paramref name="forwardedType"/> to be forwarded 
		/// to the component registered with <paramref name="name"/>.
		/// </summary>
		/// <param name="forwardedType">The service type that gets forwarded.</param>
		/// <param name="name">The name of the component to forward to.</param>
		void RegisterHandlerForwarding(Type forwardedType, string name);

        /// <summary>
        /// Register a new component resolver that can take part in the decision
        /// making about which handler to resolve
        /// </summary>
        void AddHandlerSelector(IHandlerSelector selector);
	}
}
