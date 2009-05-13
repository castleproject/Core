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

namespace Castle.Windsor
{
	using System;
	using System.Collections;

	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Registration;
	using Castle.Windsor.Configuration;
	using Castle.Windsor.Configuration.Interpreters;
	using Castle.Windsor.Installer;

	/// <summary>
	/// Implementation of <see cref="IWindsorContainer"/>
	/// which delegates to <see cref="IKernel"/> implementation.
	/// </summary>
	[Serializable]
	public class WindsorContainer : MarshalByRefObject, IWindsorContainer
	{
		#region Fields

		private readonly string name = Guid.NewGuid().ToString();
		private readonly IDictionary childContainers = Hashtable.Synchronized(new Hashtable());

		private IKernel kernel;
		private IWindsorContainer parent;
		private IComponentsInstaller installer;

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
		/// <param name="store">The instance of an <see cref="IConfigurationStore"/> implementation.</param>
		public WindsorContainer(IConfigurationStore store) : this()
		{
			kernel.ConfigurationStore = store;

			RunInstaller();
		}

		/// <summary>
		/// Constructs a container using the specified 
		/// <see cref="IConfigurationInterpreter"/> implementation.
		/// </summary>
		/// <param name="interpreter">The instance of an <see cref="IConfigurationInterpreter"/> implementation.</param>
		public WindsorContainer(IConfigurationInterpreter interpreter) : this()
		{
			if (interpreter == null) throw new ArgumentNullException("interpreter");

			interpreter.ProcessResource(interpreter.Source, kernel.ConfigurationStore);

			RunInstaller();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WindsorContainer"/> class.
		/// </summary>
		/// <param name="interpreter">The interpreter.</param>
		/// <param name="environmentInfo">The environment info.</param>
		public WindsorContainer(IConfigurationInterpreter interpreter, IEnvironmentInfo environmentInfo) : this()
		{
			if (interpreter == null) throw new ArgumentNullException("interpreter");
			if (environmentInfo == null) throw new ArgumentNullException("environmentInfo");

			interpreter.EnvironmentName = environmentInfo.GetEnvironmentName();
			interpreter.ProcessResource(interpreter.Source, kernel.ConfigurationStore);

			RunInstaller();
		}

#if !SILVERLIGHT
		/// <summary>
		/// Initializes a new instance of the <see cref="WindsorContainer"/> class using a
		/// xml file to configure it.
		/// <para>
		/// Equivalent to the use of <c>new WindsorContainer(new XmlInterpreter(xmlFile))</c>
		/// </para>
		/// </summary>
		/// <param name="xmlFile">The XML file.</param>
		public WindsorContainer(String xmlFile) : this(new XmlInterpreter(xmlFile))
		{
		}
#endif

		/// <summary>
		/// Constructs a container using the specified <see cref="IKernel"/>
		/// implementation. Rarely used.
		/// </summary>
		/// <remarks>
		/// This constructs sets the Kernel.ProxyFactory property to
		/// <c>Proxy.DefaultProxyFactory</c>
		/// </remarks>
		/// <param name="kernel">Kernel instance</param>
		/// <param name="installer">Installer instance</param>
		public WindsorContainer(IKernel kernel, IComponentsInstaller installer) : this(Guid.NewGuid().ToString(), kernel, installer)
		{
		}

		/// <summary>
		/// Constructs a container using the specified <see cref="IKernel"/>
		/// implementation. Rarely used.
		/// </summary>
		/// <remarks>
		/// This constructs sets the Kernel.ProxyFactory property to
		/// <c>Proxy.DefaultProxyFactory</c>
		/// </remarks>
		/// <param name="name">Container's name</param>
		/// <param name="kernel">Kernel instance</param>
		/// <param name="installer">Installer instance</param>
		public WindsorContainer(String name, IKernel kernel, IComponentsInstaller installer)
		{
			if (name == null) throw new ArgumentNullException("name");
			if (kernel == null) throw new ArgumentNullException("kernel");
			if (installer == null) throw new ArgumentNullException("installer");

			this.name = name;
			this.kernel = kernel;
#if !SILVERLIGHT
			this.kernel.ProxyFactory = new Proxy.DefaultProxyFactory();
#endif
			this.installer = installer;
		}

		/// <summary>
		/// Constructs with a given <see cref="IProxyFactory"/>.
		/// </summary>
		/// <param name="proxyFactory">A instance of an <see cref="IProxyFactory"/>.</param>
		public WindsorContainer(IProxyFactory proxyFactory)
		{
			if (proxyFactory == null) throw new ArgumentNullException("proxyFactory");

			kernel = new DefaultKernel(proxyFactory);

			installer = new Installer.DefaultComponentInstaller();
		}

		/// <summary>
		/// Constructs a container assigning a parent container 
		/// before starting the dependency resolution.
		/// </summary>
		/// <param name="parent">The instance of an <see cref="IWindsorContainer"/></param>
		/// <param name="interpreter">The instance of an <see cref="IConfigurationInterpreter"/> implementation</param>
		public WindsorContainer(IWindsorContainer parent, IConfigurationInterpreter interpreter) : this()
		{
			if (parent == null) throw new ArgumentNullException("parent");
			if (interpreter == null) throw new ArgumentNullException("interpreter");

			parent.AddChildContainer(this);

			interpreter.ProcessResource(interpreter.Source, kernel.ConfigurationStore);

			RunInstaller();
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="WindsorContainer"/> class.
		/// </summary>
		/// <param name="name">The container's name.</param>
		/// <param name="parent">The parent.</param>
		/// <param name="interpreter">The interpreter.</param>
		public WindsorContainer(string name, IWindsorContainer parent, IConfigurationInterpreter interpreter) : this()
		{
			if (name == null) throw new ArgumentNullException("name");
			if (parent == null) throw new ArgumentNullException("parent");
			if (interpreter == null) throw new ArgumentNullException("interpreter");

			this.name = name;

			parent.AddChildContainer(this);

			interpreter.ProcessResource(interpreter.Source, kernel.ConfigurationStore);

			RunInstaller();
		}

		#endregion

		#region IWindsorContainer Members

		/// <summary>
		/// Gets the container's name
		/// </summary>
		/// <remarks>
		/// Only useful when child containers are being used
		/// </remarks>
		/// <value>The container's name.</value>
		public string Name
		{
			get { return name; }
		}

		/// <summary>
		/// Returns the inner instance of the MicroKernel
		/// </summary>
		public virtual IKernel Kernel
		{
			get { return kernel; }
		}

		/// <summary>
		/// Gets or sets the parent container if this instance
		/// is a sub container.
		/// </summary>
		public virtual IWindsorContainer Parent
		{
			get { return parent; }
			set
			{
				if( value == null )
				{
					if (parent != null)
					{
						parent.RemoveChildContainer(this);
						parent = null;
					}
				}
				else
				{
					if (value != parent)
					{
						parent = value;
						parent.AddChildContainer(this);
					}
				}
			}
		}

		/// <summary>
		/// Registers a facility within the kernel.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="facility"></param>
		public virtual IWindsorContainer AddFacility(String key, IFacility facility)
		{
			kernel.AddFacility(key, facility);
			return this;
		}

		/// <summary>
		/// Creates and adds an <see cref="IFacility"/> facility to the container.
		/// </summary>
		/// <typeparam name="T">The facility type.</typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		public IWindsorContainer AddFacility<T>(String key) where T : IFacility, new()
		{
			kernel.AddFacility<T>(key);
			return this;
		}

		/// <summary>
		/// Creates and adds an <see cref="IFacility"/> facility to the container.
		/// </summary>
		/// <typeparam name="T">The facility type.</typeparam>
		/// <param name="key"></param>
		/// <param name="onCreate">The callback for creation.</param>
		/// <returns></returns>
		public IWindsorContainer AddFacility<T>(String key, Action<T> onCreate)
			where T : IFacility, new()
		{
			kernel.AddFacility<T>(key, onCreate);
			return this;
		}

		/// <summary>
		/// Creates and adds an <see cref="IFacility"/> facility to the container.
		/// </summary>
		/// <typeparam name="T">The facility type.</typeparam>
		/// <param name="key"></param>
		/// <param name="onCreate">The callback for creation.</param>
		/// <returns></returns>
		public IWindsorContainer AddFacility<T>(String key, Func<T, object> onCreate)
			where T : IFacility, new()
		{
			kernel.AddFacility<T>(key, onCreate);
			return this;
		}

		/// <summary>
		/// Creates and adds an <see cref="IFacility"/> facility to the container.
		/// </summary>
		/// <typeparam name="T">The facility type.</typeparam>
		/// <returns></returns>
		public IWindsorContainer AddFacility<T>() where T : IFacility, new()
		{
			kernel.AddFacility<T>();
			return this;
		}

		/// <summary>
		/// Creates and adds an <see cref="IFacility"/> facility to the container.
		/// </summary>
		/// <typeparam name="T">The facility type.</typeparam>
		/// <param name="onCreate">The callback for creation.</param>
		/// <returns></returns>
		public IWindsorContainer AddFacility<T>(Action<T> onCreate)
			where T : IFacility, new()
		{
			kernel.AddFacility<T>(onCreate);
			return this;
		}

		/// <summary>
		/// Creates and adds an <see cref="IFacility"/> facility to the container.
		/// </summary>
		/// <typeparam name="T">The facility type.</typeparam>
		/// <param name="onCreate">The callback for creation.</param>
		/// <returns></returns>
		public IWindsorContainer AddFacility<T>(Func<T, object> onCreate)
			where T : IFacility, new()
		{
			kernel.AddFacility<T>(onCreate);
			return this;
		}

		/// <summary>
		/// Adds a component to be managed by the container
		/// </summary>
		/// <param name="key"></param>
		/// <param name="classType"></param>
		public virtual IWindsorContainer AddComponent(String key, Type classType)
		{
			kernel.AddComponent(key, classType);
			return this;
		}

		/// <summary>
		/// Adds a component to be managed by the container
		/// </summary>
		/// <param name="key"></param>
		/// <param name="serviceType"></param>
		/// <param name="classType"></param>
		public virtual IWindsorContainer AddComponent(String key, Type serviceType, Type classType)
		{
			kernel.AddComponent(key, serviceType, classType);
			return this;
		}

		/// <summary>
		/// Adds a component to be managed by the container
		/// </summary>
		/// <param name="key">The key by which the component gets indexed.</param>
		/// <param name="classType">The <see cref="Type"/> to manage.</param>
		/// <param name="lifestyle">The <see cref="LifestyleType"/> with which to manage the component.</param>
		public IWindsorContainer AddComponentLifeStyle(string key, Type classType, LifestyleType lifestyle)
		{
			kernel.AddComponent(key, classType, lifestyle, true);
			return this;
		}

		/// <summary>
		/// Adds a component to be managed by the container
		/// </summary>
		/// <param name="key">The key by which the component gets indexed.</param>
		/// <param name="serviceType">The service <see cref="Type"/> that the component implements.</param>
		/// <param name="classType">The <see cref="Type"/> to manage.</param>
		/// <param name="lifestyle">The <see cref="LifestyleType"/> with which to manage the component.</param>
		public IWindsorContainer AddComponentLifeStyle(string key, Type serviceType, Type classType, LifestyleType lifestyle)
		{
			kernel.AddComponent(key, serviceType, classType, lifestyle, true);
			return this;
		}

		public virtual IWindsorContainer AddComponentWithProperties(string key, Type classType, IDictionary extendedProperties)
		{
			kernel.AddComponentWithExtendedProperties(key, classType, extendedProperties);
			return this;
		}

		public virtual IWindsorContainer AddComponentWithProperties(string key, Type serviceType, Type classType,
		                                               IDictionary extendedProperties)
		{
			kernel.AddComponentWithExtendedProperties(key, serviceType, classType, extendedProperties);
			return this;
		}

		/// <summary>
		/// Adds a component to be managed by the container.
		/// The key to obtain the component will be the FullName of the type.
		/// </summary>
		/// <typeparam name="T">The <see cref="Type"/> to manage.</typeparam>
		public IWindsorContainer AddComponent<T>()
		{
			Type t = typeof(T);
			AddComponent(t.FullName, t);
			return this;
		}

		/// <summary>
		/// Adds a component to be managed by the container
		/// </summary>
		/// <typeparam name="T">The <see cref="Type"/> to manage.</typeparam>
		/// <param name="key">The key by which the component gets indexed.</param>		
		public IWindsorContainer AddComponent<T>(string key)
		{
			AddComponent(key, typeof(T));
			return this;
		}

		/// <summary>
		/// Adds a component to be managed by the container.
		/// The key to obtain the component will be the FullName of the type.
		/// </summary>
		/// <typeparam name="T">The <see cref="Type"/> to manage.</typeparam>
		/// <param name="lifestyle">The <see cref="LifestyleType"/> with which to manage the component.</param>
		public IWindsorContainer AddComponentLifeStyle<T>(LifestyleType lifestyle)
		{
			Type t = typeof(T);
			AddComponentLifeStyle(t.FullName, t, lifestyle);
			return this;
		}

		/// <summary>
		/// Adds a component to be managed by the container
		/// </summary>
		/// <typeparam name="I">The service <see cref="Type"/> that the component implements.</typeparam>
		/// <typeparam name="T">The <see cref="Type"/> to manage.</typeparam>
		/// <param name="key">The key by which the component gets indexed.</param>
		public IWindsorContainer AddComponent<I, T>(string key) where T : class
		{
			AddComponent(key, typeof(I), typeof(T));
			return this;
		}

		/// <summary>
		/// Adds a component to be managed by the container
		/// The key to obtain the component will be the FullName of the type.
		/// </summary>
		/// <typeparam name="I">The service <see cref="Type"/> that the component implements.</typeparam>
		/// <typeparam name="T">The <see cref="Type"/> to manage.</typeparam>
		/// <param name="lifestyle">The <see cref="LifestyleType"/> with which to manage the component.</param>
		public IWindsorContainer AddComponentLifeStyle<I, T>(LifestyleType lifestyle) where T : class
		{
			Type t = typeof(T);
			AddComponentLifeStyle(t.FullName, typeof(I), t, lifestyle);
			return this;
		}

		/// <summary>
		/// Adds a component to be managed by the container
		/// </summary>
		/// <typeparam name="T">The <see cref="Type"/> to manage.</typeparam>
		/// <param name="key">The key by which the component gets indexed.</param>		
		/// <param name="lifestyle">The <see cref="LifestyleType"/> with which to manage the component.</param>
		public IWindsorContainer AddComponentLifeStyle<T>(string key, LifestyleType lifestyle)
		{
			AddComponentLifeStyle(key, typeof(T), lifestyle);
			return this;
		}

		/// <summary>
		/// Adds a component to be managed by the container
		/// The key to obtain the component will be the FullName of the type.
		/// </summary>
		/// <typeparam name="I">The service <see cref="Type"/> that the component implements.</typeparam>
		/// <typeparam name="T">The <see cref="Type"/> to manage.</typeparam>
		public IWindsorContainer AddComponent<I, T>() where T : class
		{
			Type t = typeof(T);
			AddComponent(t.FullName, typeof(I), t);
			return this;
		}

		/// <summary>
		/// Adds a component to be managed by the container
		/// </summary>
		/// <typeparam name="I">The service <see cref="Type"/> that the component implements.</typeparam>
		/// <typeparam name="T">The <see cref="Type"/> to manage.</typeparam>
		/// <param name="key">The key by which the component gets indexed.</param>
		/// <param name="lifestyle">The <see cref="LifestyleType"/> with which to manage the component.</param>
		public IWindsorContainer AddComponentLifeStyle<I, T>(string key, LifestyleType lifestyle) where T : class
		{
			AddComponentLifeStyle(key, typeof(I), typeof(T), lifestyle);
			return this;
		}

		/// <summary>
		/// Adds a concrete class as a component and specify the extended properties.
		/// Used by facilities, mostly.
		/// The key to obtain the component will be the FullName of the type.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="extendedProperties"></param>
		public IWindsorContainer AddComponentWithProperties<T>(IDictionary extendedProperties)
		{
			Type t = typeof(T);
			AddComponentWithProperties(t.FullName, t, extendedProperties);
			return this;
		}

		/// <summary>
		/// Adds a concrete class as a component and specify the extended properties.
		/// Used by facilities, mostly.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>		
		/// <param name="extendedProperties"></param>
		public IWindsorContainer AddComponentWithProperties<T>(string key, IDictionary extendedProperties)
		{
			AddComponentWithProperties(key, typeof(T), extendedProperties);
			return this;
		}

		/// <summary>
		/// Adds a concrete class and an interface 
		/// as a component and specify the extended properties.
		/// Used by facilities, mostly.
		/// The key to obtain the component will be the FullName of the type.
		/// </summary>
		/// <typeparam name="I"></typeparam>
		/// <typeparam name="T"></typeparam>
		/// <param name="extendedProperties"></param>
		public IWindsorContainer AddComponentProperties<I, T>(IDictionary extendedProperties) where T : class
		{
			Type t = typeof(T);
			AddComponentWithProperties(t.FullName, typeof(I), t, extendedProperties);
			return this;
		}

		/// <summary>
		/// Adds a concrete class and an interface 
		/// as a component and specify the extended properties.
		/// Used by facilities, mostly.
		/// </summary>
		/// <typeparam name="I"></typeparam>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <param name="extendedProperties"></param>
		public IWindsorContainer AddComponentProperties<I, T>(string key, IDictionary extendedProperties) where T : class
		{
			AddComponentWithProperties(key, typeof(I), typeof(T), extendedProperties);
			return this;
		}

		/// <summary>
		/// Registers the components described by the <see cref="ComponentRegistration{S}"/>s
		/// with the <see cref="IWindsorContainer"/>.
		/// <param name="registrations">The component registrations.</param>
		/// <returns>The container.</returns>
		/// </summary>
		public IWindsorContainer Register(params IRegistration[] registrations)
		{
			Kernel.Register(registrations);
			return this;
		}

		/// <summary>
		/// Installs the components provided by the <see cref="IWindsorInstaller"/>s
		/// with the <see cref="IWindsorContainer"/>.
		/// <param name="installers">The component installers.</param>
		/// <returns>The container.</returns>
		/// </summary>
		public IWindsorContainer Install(params IWindsorInstaller[] installers)
		{
			if (installers == null)
			{
				throw new ArgumentNullException("installers");
			}

			IComponentsInstaller scope = new Installer.DefaultComponentInstaller();

			using (PartialConfigurationStore store = new PartialConfigurationStore(kernel))
			{
				foreach (IWindsorInstaller windsorInstaller in installers)
				{
					windsorInstaller.Install(this, store);
				}

				scope.SetUp(this, store);
			}

			return this;
		}
		
		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public virtual object Resolve(String key)
		{
			return kernel[key];
		}

		/// <summary>
		/// Returns a component instance by the service
		/// </summary>
		/// <param name="service"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public virtual object Resolve(Type service, IDictionary arguments)
		{
			return kernel.Resolve(service, arguments);
		}

		/// <summary>
		/// Returns a component instance by the service
		/// </summary>
		/// <param name="service"></param>
		/// <param name="argumentsAsAnonymousType"></param>
		/// <returns></returns>
		public virtual object Resolve(Type service, object argumentsAsAnonymousType)
		{
			return Resolve(service, new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public virtual object Resolve(String key, IDictionary arguments)
		{
			return kernel.Resolve(key, arguments);
		}

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="argumentsAsAnonymousType"></param>
		/// <returns></returns>
		public virtual object Resolve(String key, object argumentsAsAnonymousType)
		{
			return Resolve(key, new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		/// <summary>
		/// Returns a component instance by the service
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		public virtual object Resolve(Type service)
		{
			return kernel[service];
		}

		/// <summary>
		/// Resolve all valid components that match this type.
		/// </summary>
		/// <typeparam name="T">The service type</typeparam>
		public T[] ResolveAll<T>()
		{
			return (T[]) ResolveAll(typeof(T));
		}

		public Array ResolveAll(Type service)
		{
			return kernel.ResolveAll(service, new Hashtable());
		}

		public Array ResolveAll(Type service, IDictionary arguments)
		{
			return kernel.ResolveAll(service, arguments);
		}

		public Array ResolveAll(Type service, object argumentsAsAnonymousType)
		{
			return ResolveAll(service, new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		/// <summary>
		/// Resolve all valid components that match this type.
		/// <typeparam name="T">The service type</typeparam>
		/// <param name="arguments">Arguments to resolve the service</param>
		/// </summary>
		public T[] ResolveAll<T>(IDictionary arguments)
		{
			return (T[]) ResolveAll(typeof (T), arguments);
		}

		/// <summary>
		/// Resolve all valid components that match this type.
		/// <typeparam name="T">The service type</typeparam>
		/// <param name="argumentsAsAnonymousType">Arguments to resolve the service</param>
		/// </summary>
		public T[] ResolveAll<T>(object argumentsAsAnonymousType)
		{
			return ResolveAll<T>(new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		/// <summary>
		/// Shortcut to the method <see cref="Resolve(String)"/>
		/// </summary>
		public virtual object this[String key]
		{
			get { return Resolve(key); }
		}

		/// <summary>
		/// Shortcut to the method <see cref="Resolve(Type)"/>
		/// </summary>
		public virtual object this[Type service]
		{
			get { return Resolve(service); }
		}

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="service"></param>
		/// <returns></returns>
		public virtual object Resolve(String key, Type service)
		{
            return kernel.Resolve(key, service);
		}

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="service"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public virtual object Resolve(String key, Type service, IDictionary arguments)
		{
			return kernel.Resolve(key, service, arguments);
		}

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="service"></param>
		/// <param name="argumentsAsAnonymousType"></param>
		/// <returns></returns>
		public virtual object Resolve(String key, Type service, object argumentsAsAnonymousType)
		{
			return Resolve(key, service, new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}
		
		/// <summary>
		/// Returns a component instance by the service 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public T Resolve<T>(IDictionary arguments)
		{
			return (T) Resolve(typeof(T), arguments);
		}

		/// <summary>
		/// Returns a component instance by the service 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="argumentsAsAnonymousType"></param>
		/// <returns></returns>
		public T Resolve<T>(object argumentsAsAnonymousType)
		{
			return Resolve<T>(new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public virtual T Resolve<T>(String key, IDictionary arguments)
		{
			return (T) Resolve(key, typeof(T), arguments);
		}

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="argumentsAsAnonymousType"></param>
		/// <returns></returns>
		public virtual T Resolve<T>(String key, object argumentsAsAnonymousType)
		{
			return Resolve<T>(key, new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		/// <summary>
		/// Returns a component instance by the service 
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T Resolve<T>()
		{
			return (T) Resolve(typeof(T));
		}

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public virtual T Resolve<T>(String key)
		{
			return (T) Resolve(key, typeof(T));
		}

		/// <summary>
		/// Releases a component instance
		/// </summary>
		/// <param name="instance"></param>
		public virtual void Release(object instance)
		{
			kernel.ReleaseComponent(instance);
		}

		/// <summary>
		/// Registers a subcontainer. The components exposed
		/// by this container will be accessible from subcontainers.
		/// </summary>
		/// <param name="childContainer"></param>
		public virtual void AddChildContainer(IWindsorContainer childContainer)
		{
			if (childContainer == null) throw new ArgumentNullException("childContainer");

			if (!childContainers.Contains(childContainer.Name))
			{
				lock (childContainers.SyncRoot)
				{
					if (!childContainers.Contains(childContainer.Name))
					{
						kernel.AddChildKernel(childContainer.Kernel);
						childContainers.Add(childContainer.Name, childContainer);
						childContainer.Parent = this;
					}
				}
			}
		}

		/// <summary>
		/// Removes (unregisters) a subcontainer.  The components exposed by this container
		/// will no longer be accessible to the child container.
		/// </summary>
		/// <param name="childContainer"></param>
		public virtual void RemoveChildContainer(IWindsorContainer childContainer)
		{
			if (childContainer == null) throw new ArgumentNullException("childContainer");

			if (childContainers.Contains(childContainer.Name))
			{
				lock (childContainers.SyncRoot)
				{
					if (childContainers.Contains(childContainer.Name))
					{
						kernel.RemoveChildKernel(childContainer.Kernel);
						childContainers.Remove(childContainer.Name);
						childContainer.Parent = null;
					}
				}
			}
		}

		/// <summary>
		/// Gets a child container instance by name.
		/// </summary>
		/// <param name="name">The container's name.</param>
		/// <returns>The child container instance or null</returns>
		public IWindsorContainer GetChildContainer(string name)
		{
			return childContainers[name] as IWindsorContainer;
		}

		#endregion

		#region IServiceProviderEx Members

		/// <summary>
		/// Gets the service object of the specified type.
		/// </summary>
		/// <returns>
		/// A service object of type serviceType.
		/// </returns>
		/// <param name="serviceType">An object that specifies the type of service object to get. </param>
		public object GetService(Type serviceType)
		{
			return kernel.GetService(serviceType);
		}

		/// <summary>
		/// Gets the service object of the specified type.
		/// </summary>
		/// <returns>
		/// A service object of type serviceType.
		/// </returns>
		public T GetService<T>() where T : class
		{
			return kernel.GetService<T>();
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Executes Dispose on underlying <see cref="IKernel"/>
		/// </summary>
		public virtual void Dispose()
		{
			Parent = null;
			childContainers.Clear();
			kernel.Dispose();
		}

		#endregion

		#region Protected Operations Members

		public IComponentsInstaller Installer
		{
			get { return installer; }
		}

		protected virtual void RunInstaller()
		{
			if (installer != null)
			{
				installer.SetUp(this, kernel.ConfigurationStore);
			}
		}

		#endregion
	}
}
