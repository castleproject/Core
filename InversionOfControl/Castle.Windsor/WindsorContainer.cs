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

	using Castle.Windsor.Configuration.AppDomain;

	/// <summary>
	/// Implementation of <see cref="IWindsorContainer"/>
	/// which delegates to <see cref="IKernel"/> implementation.
	/// </summary>
	[Serializable]
	public class WindsorContainer : MarshalByRefObject, IWindsorContainer
	{
		private IKernel _kernel;
		private IWindsorContainer _parent;

		/// <summary>
		/// Constructs a container using the <see cref="AppDomainConfigurationStore"/>
		/// as the <see cref="IConfigurationStore"/>
		/// </summary>
		public WindsorContainer() : this(new AppDomainConfigurationStore())
		{			
		}

		/// <summary>
		/// Constructs a container using the specified <see cref="IConfigurationStore"/>
		/// implementation.
		/// </summary>
		/// <param name="store"></param>
		public WindsorContainer(IConfigurationStore store) : this(new DefaultKernel())
		{
			Kernel.ConfigurationStore = store;
		}

		/// <summary>
		/// Constructs a container using the specified <see cref="IKernel"/>
		/// implementation. Rarely used.
		/// </summary>
		/// <param name="kernel"></param>
		public WindsorContainer(IKernel kernel)
		{
			_kernel = kernel;
			_kernel.ProxyFactory = new Proxy.DefaultProxyFactory();
		}

		#region IWindsorContainer Members

		/// <summary>
		/// Returns the inner instance of the MicroKernel
		/// </summary>
		public IKernel Kernel
		{
			get { return _kernel; }
		}

		/// <summary>
		/// Gets or sets the parent container if this instance
		/// is a sub container.
		/// </summary>
		public IWindsorContainer Parent
		{
			get { return _parent; }
			set { _parent = value; }
		}

		/// <summary>
		/// Registers a facility within the kernel.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="facility"></param>
		public void AddFacility(String key, IFacility facility)
		{
			_kernel.AddFacility(key, facility);
		}

		/// <summary>
		/// Adds a component to be managed by the container
		/// </summary>
		/// <param name="key"></param>
		/// <param name="classType"></param>
		public void AddComponent(String key, Type classType)
		{
			Kernel.AddComponent(key, classType);
		}

		/// <summary>
		/// Adds a component to be managed by the container
		/// </summary>
		/// <param name="key"></param>
		/// <param name="serviceType"></param>
		/// <param name="classType"></param>
		public void AddComponent(String key, Type serviceType, Type classType)
		{
			Kernel.AddComponent(key, serviceType, classType);
		}

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public object Resolve(String key)
		{
			return Kernel[key];
		}

		/// <summary>
		/// Returns a component instance by the service
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		public object Resolve(Type service)
		{
			return Kernel[service];
		}

		/// <summary>
		/// Shortcut to the method <see cref="Resolve"/>
		/// </summary>
		public object this [String key]
		{
			get { return Resolve(key); }
		}

		/// <summary>
		/// Shortcut to the method <see cref="Resolve"/>
		/// </summary>
		public object this [Type service]
		{
			get { return Resolve(service); }
		}

		/// <summary>
		/// Releases a component instance
		/// </summary>
		/// <param name="instance"></param>
		public void Release(object instance)
		{
			Kernel.ReleaseComponent(instance);
		}

		/// <summary>
		/// Registers a subcontainer. The components exposed
		/// by this container will be accessible from subcontainers.
		/// </summary>
		/// <param name="childContainer"></param>
		public void AddChildContainer(IWindsorContainer childContainer)
		{
			childContainer.Parent = this;
			Kernel.AddChildKernel(childContainer.Kernel);
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Executes Dispose on underlying <see cref="IKernel"/>
		/// </summary>
		public void Dispose()
		{
			_kernel.Dispose();
		}

		#endregion
	}
}