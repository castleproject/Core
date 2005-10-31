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
	using System.Collections;
	using Castle.MicroKernel;

	using Castle.Windsor.Configuration;
	using Castle.Windsor.Configuration.Interpreters;

	/// <summary>
	/// Implementation of <see cref="IWindsorContainer"/>
	/// which delegates to <see cref="IKernel"/> implementation.
	/// </summary>
	[Serializable]
	public class WindsorContainer : MarshalByRefObject, IWindsorContainer
	{
		#region Fields

		private IKernel _kernel;
		private IWindsorContainer _parent;
		private IComponentsInstaller _installer;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a container without any external 
		/// configuration reference
		/// </summary>
		public WindsorContainer() : this(new DefaultKernel(), new Installer.DefaultComponentInstaller())
		{
		}

		/// <summary>
		/// Constructs a container using the specified 
		/// <see cref="IConfigurationStore"/> implementation.
		/// </summary>
		/// <param name="store"></param>
		public WindsorContainer(IConfigurationStore store) : this()
		{
			Kernel.ConfigurationStore = store;

			RunInstaller();
		}

		/// <summary>
		/// Constructs a container using the specified 
		/// <see cref="IConfigurationInterpreter"/> implementation.
		/// </summary>
		/// <param name="interpreter"></param>
		public WindsorContainer(IConfigurationInterpreter interpreter) : this()
		{
			if (interpreter == null) throw new ArgumentNullException("interpreter");

			interpreter.ProcessResource(interpreter.Source, Kernel.ConfigurationStore);

			RunInstaller();
		}

		[Obsolete("Use includes instead of cascade configurations")]
		public WindsorContainer(IConfigurationInterpreter parent, IConfigurationInterpreter child) : this()
		{
			if (parent == null) throw new ArgumentNullException("parent");
			if (child == null) throw new ArgumentNullException("child");

			Kernel.ConfigurationStore = new CascadeConfigurationStore(parent, child);

			RunInstaller();
		}

		public WindsorContainer(String xmlFile) : this(new XmlInterpreter(xmlFile))
		{
		}

		public WindsorContainer(String parentXmlFile, String childXmlFile) : 
			this(new XmlInterpreter(parentXmlFile), new XmlInterpreter(childXmlFile))
		{
		}

		/// <summary>
		/// Constructs a container using the specified <see cref="IKernel"/>
		/// implementation. Rarely used.
		/// </summary>
		/// <remarks>
		/// This constructs sets the Kernel.ProxyFactory property to
		/// <see cref="Proxy.DefaultProxyFactory"/>
		/// </remarks>
		/// <param name="kernel"></param>
		public WindsorContainer(IKernel kernel, IComponentsInstaller installer)
		{
			_kernel = kernel;
			_kernel.ProxyFactory = new Proxy.ProxySmartFactory();

			_installer = installer;
		}

		public WindsorContainer(IProxyFactory proxyFactory) 
		{
			_kernel = new DefaultKernel(proxyFactory);

			_installer = new Installer.DefaultComponentInstaller();
		}

		#endregion

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
			set
			{
				_parent = value;

				if (value != null) Kernel.Parent = value.Kernel;
			}
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

        public void AddComponentWithProperties(string key, Type classType, IDictionary extendedProperties)
        {
            Kernel.AddComponentWithProperties(key, classType, extendedProperties);
        }

        public void AddComponentWithProperties(string key, Type serviceType, Type classType, IDictionary extendedProperties)
        {
            Kernel.AddComponentWithProperties(key, serviceType, classType, extendedProperties);
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

		#region Protected Operations Members

		protected virtual void RunInstaller()
		{
			if (_installer != null)
			{
				_installer.SetUp(this, _kernel.ConfigurationStore);
			}
		}

		#endregion
	}
}