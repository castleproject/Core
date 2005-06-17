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

namespace Castle.Windsor
{
	using System;

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
		/// <param name="key"></param>
		/// <param name="facility"></param>
		void AddFacility( String key, IFacility facility );

		/// <summary>
		/// Adds a component to be managed by the container
		/// </summary>
		/// <param name="key"></param>
		/// <param name="classType"></param>
		void AddComponent( String key, Type classType );

		/// <summary>
		/// Adds a component to be managed by the container
		/// </summary>
		/// <param name="key"></param>
		/// <param name="serviceType"></param>
		/// <param name="classType"></param>
		void AddComponent( String key, Type serviceType, Type classType );

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		object Resolve( String key );

		/// <summary>
		/// Returns a component instance by the service
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		object Resolve( Type service );

		/// <summary>
		/// Releases a component instance
		/// </summary>
		/// <param name="instance"></param>
		void Release( object instance );

		/// <summary>
		/// Registers a subcontainer. The components exposed
		/// by this container will be accessible from subcontainers.
		/// </summary>
		/// <param name="childContainer"></param>
		void AddChildContainer(IWindsorContainer childContainer);

		/// <summary>
		/// Shortcut to the method <see cref="Resolve"/>
		/// </summary>
		object this [String key]
		{
			get;
		}

		/// <summary>
		/// Shortcut to the method <see cref="Resolve"/>
		/// </summary>
		object this [Type service]
		{
			get;
		}

		/// <summary>
		/// Returns the inner instance of the MicroKernel
		/// </summary>
		IKernel Kernel
		{
			get;
		}

		/// <summary>
		/// Gets or sets the parent container if this instance
		/// is a sub container.
		/// </summary>
		IWindsorContainer Parent
		{
			get; set;
		}
	}
}
