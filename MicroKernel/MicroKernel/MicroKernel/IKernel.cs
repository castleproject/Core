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

	using Castle.Model;

	/// <summary>
	/// Summary description for IKernel.
	/// </summary>
	public interface IKernel : IKernelEvents, IDisposable
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="classType"></param>
		void AddComponent( String key, Type classType );

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="serviceType"></param>
		/// <param name="classType"></param>
		void AddComponent( String key, Type serviceType, Type classType );

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

		IComponentModelBuilder ComponentModelBuilder
		{
			get; 
		}

		IHandlerFactory HandlerFactory
		{
			get;
		}

		IConfigurationStore ConfigurationStore
		{
			get; set;
		}

		IHandler GetHandler(String key);

		IHandler GetHandler(Type service);

		IReleasePolicy ReleasePolicy
		{
		 	get;
		}

		IDependecyResolver Resolver
		{
			get;
		}

		void AddFacility(IFacility facility);

		void AddSubSystem(String key, ISubSystem subsystem);

		// void ConfigureExternalComponent(object component);

		// void ConfigureExternalComponent(object component, ComponentModel model);
		
		IProxyFactory ProxyFactory
		{
			get; set;
		}

		IKernel Parent
		{
			get; set;
		}

		void AddChildKernel(IKernel kernel);

		IComponentActivator CreateComponentActivator(ComponentModel model);
	}
}
