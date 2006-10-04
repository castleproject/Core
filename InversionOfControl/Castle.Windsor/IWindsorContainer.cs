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

namespace Castle.Windsor
{
	using System;
	using System.Collections;

	using Castle.Core;
	using Castle.MicroKernel;

	/// <summary>
	/// The <c>IWindsorContainer</c> interface exposes all the 
	/// functionality the Windsor implements.
	/// </summary>
	public interface IWindsorContainer : IDisposable
	{
		/// <summary>
		/// Registers a facility within the kernel.
		/// </summary>
		/// <param name="key">The key by which the <see cref="IFacility"/> gets indexed.</param>
		/// <param name="facility">The <see cref="IFacility"/> to add to the container.</param>
		void AddFacility(String key, IFacility facility);

		/// <summary>
		/// Adds a component to be managed by the container
		/// </summary>
		/// <param name="key">The key by which the component gets indexed.</param>
		/// <param name="classType">The <see cref="Type"/> to manage.</param>
		void AddComponent(String key, Type classType);

		/// <summary>
		/// Adds a component to be managed by the container
		/// </summary>
		/// <param name="key">The key by which the component gets indexed.</param>
		/// <param name="serviceType">The service <see cref="Type"/> that the component implements.</param>
		/// <param name="classType">The <see cref="Type"/> to manage.</param>
		void AddComponent(String key, Type serviceType, Type classType);

		/// <summary>
		/// Adds a component to be managed by the container
		/// </summary>
		/// <param name="key">The key by which the component gets indexed.</param>
		/// <param name="classType">The <see cref="Type"/> to manage.</param>
		/// <param name="lifestyle">The <see cref="LifestyleType"/> with which to manage the component.</param>
		void AddComponentWithLifestyle(String key, Type classType, LifestyleType lifestyle);

		/// <summary>
		/// Adds a component to be managed by the container
		/// </summary>
		/// <param name="key">The key by which the component gets indexed.</param>
		/// <param name="serviceType">The service <see cref="Type"/> that the component implements.</param>
		/// <param name="classType">The <see cref="Type"/> to manage.</param>
		/// <param name="lifestyle">The <see cref="LifestyleType"/> with which to manage the component.</param>
		void AddComponentWithLifestyle(String key, Type serviceType, Type classType, LifestyleType lifestyle);

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
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		object Resolve(String key);

		#if DOTNET2

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="service"></param>
		/// <returns></returns>
		object Resolve(String key, Type service);

		#endif

		/// <summary>
		/// Returns a component instance by the service
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		object Resolve(Type service);

		/// <summary>
		/// Releases a component instance
		/// </summary>
		/// <param name="instance"></param>
		void Release(object instance);

		/// <summary>
		/// Registers a subcontainer. The components exposed
		/// by this container will be accessible from subcontainers.
		/// </summary>
		/// <param name="childContainer"></param>
		void AddChildContainer(IWindsorContainer childContainer);

		/// <summary>
		/// Remove a child container
		/// </summary>
		/// <param name="childContainer"></param>
		void RemoveChildContainer(IWindsorContainer childContainer);

		/// <summary>
		/// Shortcut to the method <see cref="Resolve(string)"/>
		/// </summary>
		object this [String key] { get; }

		/// <summary>
		/// Shortcut to the method <see cref="Resolve(Type)"/>
		/// </summary>
		object this [Type service] { get; }

		#if DOTNET2

		/// <summary>
		/// Returns a component instance by the service
		/// </summary>
		/// <typeparam name="T">Service type</typeparam>
		/// <returns>The component instance</returns>
		T Resolve<T>();

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key">Component's key</param>
		/// <typeparam name="T">Service type</typeparam>
		/// <returns>The Component instance</returns>
		T Resolve<T>(String key);

		#endif

		/// <summary>
		/// Returns the inner instance of the MicroKernel
		/// </summary>
		IKernel Kernel { get; }

		/// <summary>
		/// Gets or sets the parent container if this instance
		/// is a sub container.
		/// </summary>
		IWindsorContainer Parent { get; set; }
	}
}
