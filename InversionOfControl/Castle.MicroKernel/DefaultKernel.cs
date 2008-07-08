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
	using System.Collections.Generic;
	using System.Reflection;
	using System.Runtime.Serialization;
	using Castle.Core;
	using Castle.Core.Internal;
	using Castle.MicroKernel.ComponentActivator;
	using Castle.MicroKernel.Handlers;
	using Castle.MicroKernel.ModelBuilder;
	using Castle.MicroKernel.Proxy;
	using Castle.MicroKernel.Registration;
	using Castle.MicroKernel.Releasers;
	using Castle.MicroKernel.Resolvers;
	using Castle.MicroKernel.SubSystems.Configuration;
	using Castle.MicroKernel.SubSystems.Conversion;
	using Castle.MicroKernel.SubSystems.Naming;
	using Castle.MicroKernel.SubSystems.Resource;

	/// <summary>
	/// Default implementation of <see cref="IKernel"/>. 
	/// This implementation is complete and also support a kernel 
	/// hierarchy (sub containers).
	/// </summary>
	[Serializable]
	public class DefaultKernel : KernelEventSupport, IKernel, IDeserializationCallback
	{
		#region Fields

		/// <summary>
		/// The parent kernel, if exists.
		/// </summary>
		private IKernel parentKernel;

		/// <summary>
		/// The implementation of <see cref="IHandlerFactory"/>
		/// </summary>
		private IHandlerFactory handlerFactory;

		/// <summary>
		/// The implementation of <see cref="IComponentModelBuilder"/>
		/// </summary>
		private IComponentModelBuilder modelBuilder;

		/// <summary>
		/// The dependency resolver.
		/// </summary>
		private IDependencyResolver resolver;

		/// <summary>
		/// Implements a policy to control component's
		/// disposal that the usef forgot.
		/// </summary>
		private IReleasePolicy releaserPolicy;

		/// <summary>
		/// Holds the implementation of <see cref="IProxyFactory"/>
		/// </summary>
		private IProxyFactory proxyFactory;

		/// <summary>
		/// List of <see cref="IFacility"/> registered.
		/// </summary>
		private IList facilities;

		/// <summary>
		/// Map of subsystems registered.
		/// </summary>
		private IDictionary subsystems;

		/// <summary>
		/// List of sub containers.
		/// </summary>
		private IList childKernels;

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a DefaultKernel with no component
		/// proxy support.
		/// </summary>
		public DefaultKernel() : this(new NotSupportedProxyFactory())
		{
		}

		/// <summary>
		/// Constructs a DefaultKernel with the specified
		/// implementation of <see cref="IProxyFactory"/> and <see cref="IDependencyResolver"/>
		/// </summary>
		/// <param name="resolver"></param>
		/// <param name="proxyFactory"></param>
		public DefaultKernel(IDependencyResolver resolver, IProxyFactory proxyFactory)
			: this(proxyFactory)
		{
			this.resolver = resolver;
			this.resolver.Initialize(RaiseDependencyResolving);
		}

		/// <summary>
		/// Constructs a DefaultKernel with the specified
		/// implementation of <see cref="IProxyFactory"/>
		/// </summary>
		public DefaultKernel(IProxyFactory proxyFactory)
		{
			this.proxyFactory = proxyFactory;

			childKernels = new ArrayList();
			facilities = new ArrayList();
			subsystems = new Hashtable();

			RegisterSubSystems();

			releaserPolicy = new LifecycledComponentsReleasePolicy();
			handlerFactory = new DefaultHandlerFactory(this);
			modelBuilder = new DefaultComponentModelBuilder(this);
			resolver = new DefaultDependencyResolver(this);
			resolver.Initialize(new DependencyDelegate(RaiseDependencyResolving));
		}

		public DefaultKernel(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			MemberInfo[] members = FormatterServices.GetSerializableMembers(GetType(), context);

			object[] kernelmembers = (object[])info.GetValue("members", typeof(object[]));

			FormatterServices.PopulateObjectMembers(this, members, kernelmembers);
		}

		#endregion

		#region Overridables

		protected virtual void RegisterSubSystems()
		{
			AddSubSystem(SubSystemConstants.ConfigurationStoreKey,
						 new DefaultConfigurationStore());

			AddSubSystem(SubSystemConstants.ConversionManagerKey,
						 new DefaultConversionManager());

			AddSubSystem(SubSystemConstants.NamingKey,
						 new DefaultNamingSubSystem());

			AddSubSystem(SubSystemConstants.ResourceKey,
						 new DefaultResourceSubSystem());
		}

		#endregion

		#region IKernel Members

		public virtual void AddComponent(String key, Type classType)
		{
			AddComponent(key, classType, classType);
		}

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
		/// <see cref="IKernel.AddComponent(string,Type,LifestyleType,bool)"/> method.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="key"/> or <paramref name="classType"/>
		/// are <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException">
		/// Thrown if <paramref name="lifestyle"/> is <see cref="LifestyleType.Undefined"/>.
		/// </exception>
		public void AddComponent(string key, Type classType, LifestyleType lifestyle)
		{
			AddComponent(key, classType, classType, lifestyle);
		}

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
		/// <see cref="IKernel.AddComponent(string,Type,Type,LifestyleType,bool)"/> method.
		/// </remarks>
		/// <exception cref="ArgumentNullException">
		/// Thrown if <paramref name="key"/> or <paramref name="classType"/>
		/// are <see langword="null"/>.
		/// </exception>
		/// <exception cref="ArgumentException" />
		/// Thrown if <paramref name="lifestyle"/> is <see cref="LifestyleType.Undefined"/>.
		public void AddComponent(string key, Type classType, LifestyleType lifestyle, bool overwriteLifestyle)
		{
			AddComponent(key, classType, classType, lifestyle, overwriteLifestyle);
		}

		public virtual void AddComponent(String key, Type serviceType, Type classType)
		{
			AddComponent(key, serviceType, classType, LifestyleType.Singleton);
		}

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
		public void AddComponent(string key, Type serviceType, Type classType, LifestyleType lifestyle)
		{
			AddComponent(key, serviceType, classType, lifestyle, false);
		}

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
		public void AddComponent(string key, Type serviceType, Type classType, LifestyleType lifestyle,
								 bool overwriteLifestyle)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (serviceType == null) throw new ArgumentNullException("serviceType");
			if (classType == null) throw new ArgumentNullException("classType");
			if (LifestyleType.Undefined == lifestyle)
				throw new ArgumentException("The specified lifestyle must be Thread, Transient, or Singleton.", "lifestyle");

			ComponentModel model = ComponentModelBuilder.BuildModel(key, serviceType, classType, null);

			if (overwriteLifestyle || LifestyleType.Undefined == model.LifestyleType)
			{
				model.LifestyleType = lifestyle;
			}

			RaiseComponentModelCreated(model);

			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(key, handler);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="classType"></param>
		/// <param name="parameters"></param>
		public virtual void AddComponentWithExtendedProperties(String key, Type classType, IDictionary parameters)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (parameters == null) throw new ArgumentNullException("parameters");
			if (classType == null) throw new ArgumentNullException("classType");

			ComponentModel model = ComponentModelBuilder.BuildModel(key, classType, classType, parameters);
			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(key, handler);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="serviceType"></param>
		/// <param name="classType"></param>
		/// <param name="parameters"></param>
		public virtual void AddComponentWithExtendedProperties(String key, Type serviceType, Type classType,
															   IDictionary parameters)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (parameters == null) throw new ArgumentNullException("parameters");
			if (serviceType == null) throw new ArgumentNullException("serviceType");
			if (classType == null) throw new ArgumentNullException("classType");

			ComponentModel model = ComponentModelBuilder.BuildModel(key, serviceType, classType, parameters);
			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(key, handler);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="model"></param>
		public virtual void AddCustomComponent(ComponentModel model)
		{
			if (model == null) throw new ArgumentNullException("model");

			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);

			object skipRegistration = model.ExtendedProperties[ComponentModel.SkipRegistration];

			if (skipRegistration != null)
			{
				RegisterHandler(model.Name, handler, (bool)skipRegistration);
			}
			else
			{
				RegisterHandler(model.Name, handler);
			}
		}

		/// <summary>
		/// Used mostly by facilities. Adds an instance
		/// to be used as a component.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="instance"></param>
		public void AddComponentInstance(String key, object instance)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (instance == null) throw new ArgumentNullException("instance");

			Type classType = instance.GetType();

			ComponentModel model = new ComponentModel(key, classType, classType);
			model.CustomComponentActivator = typeof(ExternalInstanceActivator);
			model.ExtendedProperties["instance"] = instance;

			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(key, handler);
		}

		/// <summary>
		/// Used mostly by facilities. Adds an instance
		/// to be used as a component.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="serviceType"></param>
		/// <param name="instance"></param>
		public void AddComponentInstance(String key, Type serviceType, object instance)
		{
			AddComponentInstance(key, serviceType, instance.GetType(), instance);
		}

		public void AddComponentInstance(string key, Type serviceType, Type classType, object instance)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (serviceType == null) throw new ArgumentNullException("serviceType");
			if (instance == null) throw new ArgumentNullException("instance");
			if (classType == null) throw new ArgumentNullException("classType");

			ComponentModel model = new ComponentModel(key, serviceType, classType);
			model.CustomComponentActivator = typeof(ExternalInstanceActivator);
			model.ExtendedProperties["instance"] = instance;

			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(key, handler);
		}

		/// <summary>
		/// Adds a concrete class as a component
		/// </summary>
		public void AddComponent<T>()
		{
			Type classType = typeof(T);
			AddComponent(classType.FullName, classType);
		}

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
		public void AddComponent<T>(LifestyleType lifestyle)
		{
			Type classType = typeof(T);
			AddComponent(classType.FullName, classType, lifestyle);
		}

		/// <summary>
		/// Adds a concrete class
		/// as a component with the specified <paramref name="lifestyle"/>.
		/// </summary>
		/// <param name="lifestyle">The specified <see cref="LifestyleType"/> for the component.</param>
		/// <param name="overwriteLifestyle">If <see langword="true"/>, then ignores all other configurations
		/// for lifestyle and uses the value in the <paramref name="lifestyle"/> parameter.</param>
		/// <remarks>
		/// If you have indicated a lifestyle for the specified T using
		/// attributes, this method will not overwrite that lifestyle. To do that, use the
		/// <see cref="AddComponent(string,Type,LifestyleType,bool)"/> method.
		/// </remarks>
		/// <exception cref="ArgumentException"/>
		/// Thrown if 
		/// <paramref name="lifestyle"/>
		///  is 
		/// <see cref="LifestyleType.Undefined"/>
		/// .
		public void AddComponent<T>(LifestyleType lifestyle, bool overwriteLifestyle)
		{
			Type classType = typeof(T);
			AddComponent(classType.FullName, classType, lifestyle, overwriteLifestyle);
		}

		/// <summary>
		/// Adds a concrete class and an interface
		/// as a component
		/// </summary>
		/// <param name="serviceType">The service <see cref="Type"/> that this component implements.</param>
		public void AddComponent<T>(Type serviceType)
		{
			Type classType = typeof(T);
			AddComponent(classType.FullName, serviceType, classType);
		}

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
		public void AddComponent<T>(Type serviceType, LifestyleType lifestyle)
		{
			Type classType = typeof(T);
			AddComponent(classType.FullName, serviceType, classType, lifestyle);
		}

		/// <summary>
		/// Adds a concrete class and an interface
		/// as a component with the specified <paramref name="lifestyle"/>.
		/// </summary>
		/// <param name="serviceType">The service <see cref="Type"/> that this component implements.</param>
		/// <param name="lifestyle">The specified <see cref="LifestyleType"/> for the component.</param>
		/// <param name="overwriteLifestyle">If <see langword="true"/>, then ignores all other configurations
		/// for lifestyle and uses the value in the <paramref name="lifestyle"/> parameter.</param>
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
		public void AddComponent<T>(Type serviceType, LifestyleType lifestyle, bool overwriteLifestyle)
		{
			Type classType = typeof(T);
			AddComponent(classType.FullName, serviceType, classType, lifestyle, overwriteLifestyle);
		}

		/// <summary>
		/// Used mostly by facilities. Adds an instance
		/// to be used as a component.
		/// </summary>
		/// <param name="instance"></param>
		public void AddComponentInstance<T>(object instance)
		{
			Type serviceType = typeof(T);
			AddComponentInstance(serviceType.FullName, serviceType, instance);
		}

		/// <summary>
		/// Used mostly by facilities. Adds an instance
		/// to be used as a component.
		/// </summary>
		/// <param name="serviceType"></param>
		/// <param name="instance"></param>
		public void AddComponentInstance<T>(Type serviceType, object instance)
		{
			Type classType = typeof(T);
			AddComponentInstance(classType.FullName, serviceType, classType, instance);
		}

		/// <summary>
		/// Registers the components described by the <see cref="ComponentRegistration{S}"/>s
		/// with the <see cref="IKernel"/>.
		/// <param name="registrations">The component registrations.</param>
		/// <returns>The kernel.</returns>
		/// </summary>
		public IKernel Register(params IRegistration[] registrations)
		{
			if (registrations == null)
			{
				throw new ArgumentNullException("registrations");
			}

			foreach(IRegistration registration in registrations)
			{
				registration.Register(this);
			}

			return this;
		}

		/// <summary>
		/// Returns the component instance by the service type
		/// using dynamic arguments
		/// </summary>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public T Resolve<T>(IDictionary arguments)
		{
			Type serviceType = typeof(T);
			return (T)Resolve(serviceType, arguments);
		}

		/// <summary>
		/// Returns the component instance by the service type
		/// using dynamic arguments
		/// </summary>
		/// <param name="argumentsAsAnonymousType"></param>
		/// <returns></returns>
		public T Resolve<T>(object argumentsAsAnonymousType)
		{
			return Resolve<T>(new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		/// <summary>
		/// Returns the component instance by the component key
		/// </summary>
		/// <returns></returns>
		public T Resolve<T>()
		{
			Type serviceType = typeof(T);
			return (T)Resolve(serviceType);
		}

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key">Component's key</param>
		/// <typeparam name="T">Service type</typeparam>
		/// <returns>The Component instance</returns>
		public T Resolve<T>(String key)
		{
			Type serviceType = typeof(T);
			return (T)Resolve(key, serviceType);
		}

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <typeparam name="T">Service type</typeparam>
		/// <param name="key">Component's key</param>
		/// <param name="arguments"></param>
		/// <returns>The Component instance</returns>
		public T Resolve<T>(String key, IDictionary arguments)
		{
			Type serviceType = typeof(T);
			return (T)Resolve(key, serviceType, arguments);
		}

		/// <summary>
		/// Returns true if the specified component was
		/// found and could be removed (i.e. no other component depends on it)
		/// </summary>
		/// <param name="key">The component's key</param>
		/// <returns></returns>
		public virtual bool RemoveComponent(String key)
		{
			if (key == null) throw new ArgumentNullException("key");

			if (NamingSubSystem.Contains(key))
			{
				IHandler handler = GetHandler(key);

				if (handler.ComponentModel.Dependers.Length == 0)
				{
					NamingSubSystem.UnRegister(key);

					Type service = handler.ComponentModel.Service;
					IHandler[] assignableHandlers = NamingSubSystem.GetAssignableHandlers(service);
					if (assignableHandlers.Length > 0)
					{
						NamingSubSystem[handler.ComponentModel.Service] = assignableHandlers[0];
					}
					else
					{
						NamingSubSystem.UnRegister(service);
					}

					foreach (ComponentModel model in handler.ComponentModel.Dependents)
					{
						model.RemoveDepender(handler.ComponentModel);
					}

					RaiseComponentUnregistered(key, handler);

					DisposeHandler(handler);

					return true;
				}
				else
				{
					// We can't remove this component as there are
					// others which depends on it

					return false;
				}
			}

			if (Parent != null)
			{
				return Parent.RemoveComponent(key);
			}

			return false;
		}

		public virtual bool HasComponent(String key)
		{
			if (key == null) throw new ArgumentNullException("key");

			if (NamingSubSystem.Contains(key))
			{
				return true;
			}

			if (Parent != null)
			{
				return Parent.HasComponent(key);
			}

			return false;
		}

		public virtual bool HasComponent(Type serviceType)
		{
			if (serviceType == null) throw new ArgumentNullException("serviceType");

			if (NamingSubSystem.Contains(serviceType))
			{
				return true;
			}

			if (serviceType.IsGenericType && NamingSubSystem.Contains(serviceType.GetGenericTypeDefinition()))
			{
				return true;
			}

			if (Parent != null)
			{
				return Parent.HasComponent(serviceType);
			}

			return false;
		}

		public virtual object this[String key]
		{
			get
			{
				if (key == null) throw new ArgumentNullException("key");

				if (!HasComponent(key))
				{
					throw new ComponentNotFoundException(key);
				}

				IHandler handler = GetHandler(key);

				return ResolveComponent(handler);
			}
		}

		public virtual object this[Type service]
		{
			get
			{
				if (service == null) throw new ArgumentNullException("service");

				if (!HasComponent(service))
				{
					throw new ComponentNotFoundException(service);
				}

				IHandler handler = GetHandler(service);

				return ResolveComponent(handler, service);
			}
		}

		/// <summary>
		/// Returns the component instance by the service type
		/// </summary>
		public object Resolve(Type service)
		{
			if (service == null) throw new ArgumentNullException("service");

			return this[service];
		}

		/// <summary>
		/// Returns all the valid component instances by
		/// the service type
		/// </summary>
		/// <param name="service">The service type</param>
		/// <param name="arguments">Arguments to resolve the services</param>
		public Array ResolveAll(Type service, IDictionary arguments)
		{
			ArrayList list = new ArrayList();
			IHandler[] handlers = GetAssignableHandlers(service);
			foreach (IHandler handler in handlers)
			{
				if(handler.CurrentState!=HandlerState.Valid)
					continue;

				object component = ResolveComponent(handler, service, arguments);
				list.Add(component);
			}
			return list.ToArray(service);
		}

		/// <summary>
		/// Returns all the valid component instances by
		/// the service type
		/// </summary>
		/// <param name="service">The service type</param>
		/// <param name="argumentsAsAnonymousType">Arguments to resolve the services</param>
		public Array ResolveAll(Type service, object argumentsAsAnonymousType)
		{
			return ResolveAll(service, new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}


		/// <summary>
		/// Returns component instances that implement TService
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <param name="argumentsAsAnonymousType"></param>
		/// <returns></returns>
		public TService[] ResolveAll<TService>(object argumentsAsAnonymousType)
		{
			return (TService[]) ResolveAll(typeof(TService), argumentsAsAnonymousType);
		}

		/// <summary>
		/// Returns component instances that implement TService
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public TService[] ResolveAll<TService>(IDictionary arguments)
		{
			return (TService[])ResolveAll(typeof(TService), arguments);
		}



		/// <summary>
		/// Returns the component instance by the service type
		/// using dynamic arguments
		/// </summary>
		/// <param name="service"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public object Resolve(Type service, IDictionary arguments)
		{
			if (service == null) throw new ArgumentNullException("service");
			if (arguments == null) throw new ArgumentNullException("arguments");

			if (!HasComponent(service))
			{
				throw new ComponentNotFoundException(service);
			}

			IHandler handler = GetHandler(service);

			return ResolveComponent(handler, service, arguments);
		}

		/// <summary>
		/// Returns the component instance by the service type
		/// using dynamic arguments
		/// </summary>
		/// <param name="service"></param>
		/// <param name="argumentsAsAnonymousType"></param>
		/// <returns></returns>
		public object Resolve(Type service, object argumentsAsAnonymousType)
		{
			return Resolve(service, new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		/// <summary>
		/// Returns the component instance by the component key
		/// using dynamic arguments
		/// </summary>
		/// <param name="key"></param>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public object Resolve(string key, IDictionary arguments)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (arguments == null) throw new ArgumentNullException("arguments");

			if (!HasComponent(key))
			{
				throw new ComponentNotFoundException(key);
			}

			IHandler handler = GetHandler(key);

			return ResolveComponent(handler, arguments);
		}

		/// <summary>
		/// Returns the component instance by the component key
		/// using dynamic arguments
		/// </summary>
		/// <param name="key"></param>
		/// <param name="argumentsAsAnonymousType"></param>
		/// <returns></returns>
		public object Resolve(string key, object argumentsAsAnonymousType)
		{
			return Resolve(key, new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		/// <summary>
		/// Associates objects with a component handler,
		/// allowing it to use the specified dictionary
		/// when resolving dependencies
		/// </summary>
		/// <param name="service"></param>
		/// <param name="dependencies"></param>
		public void RegisterCustomDependencies(Type service, IDictionary dependencies)
		{
			IHandler handler = GetHandler(service);

			foreach (DictionaryEntry entry in dependencies)
			{
				handler.AddCustomDependencyValue(entry.Key.ToString(), entry.Value);
			}
		}

		/// <summary>
		/// Associates objects with a component handler,
		/// allowing it to use the specified dictionary
		/// when resolving dependencies
		/// </summary>
		/// <param name="service"></param>
		/// <param name="dependenciesAsAnonymousType"></param>
		public void RegisterCustomDependencies(Type service, object dependenciesAsAnonymousType)
		{
			RegisterCustomDependencies(service, new ReflectionBasedDictionaryAdapter(dependenciesAsAnonymousType));
		}

		/// <summary>
		/// Associates objects with a component handler,
		/// allowing it to use the specified dictionary
		/// when resolving dependencies
		/// </summary>
		/// <param name="key"></param>
		/// <param name="dependencies"></param>
		public void RegisterCustomDependencies(String key, IDictionary dependencies)
		{
			IHandler handler = GetHandler(key);

			foreach (DictionaryEntry entry in dependencies)
			{
				handler.AddCustomDependencyValue(entry.Key.ToString(), entry.Value);
			}
		}

		/// <summary>
		/// Associates objects with a component handler,
		/// allowing it to use the specified dictionary
		/// when resolving dependencies
		/// </summary>
		/// <param name="key"></param>
		/// <param name="dependenciesAsAnonymousType"></param>
		public void RegisterCustomDependencies(String key, object dependenciesAsAnonymousType)
		{
			RegisterCustomDependencies(key, new ReflectionBasedDictionaryAdapter(dependenciesAsAnonymousType));
		}

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
		/// <param name="service"></param>
		/// <returns></returns>
		public virtual object Resolve(String key, Type service)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (service == null) throw new ArgumentNullException("service");

			if (!HasComponent(key))
			{
				throw new ComponentNotFoundException(key);
			}

			IHandler handler = GetHandler(key);

			return ResolveComponent(handler, service);
		}


		/// <summary>
		/// Returns component instances that implement TService
		/// </summary>
		/// <typeparam name="TService"></typeparam>
		/// <returns></returns>
		public TService[] ResolveAll<TService>()
		{
			return (TService[]) ResolveAll(typeof (TService), new Hashtable());
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
			if (key == null) throw new ArgumentNullException("key");
			if (service == null) throw new ArgumentNullException("service");

			if (!HasComponent(key))
			{
				throw new ComponentNotFoundException(key);
			}

			IHandler handler = GetHandler(key);

			return ResolveComponent(handler, service, arguments);
		}

		/// <summary>
		/// Resolves the specified key.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="service">The service.</param>
		/// <param name="argumentsAsAnonymousType">Type of the arguments as anonymous.</param>
		/// <returns></returns>
		public virtual object Resolve(String key, Type service, object argumentsAsAnonymousType)
		{
			return Resolve(key, service, new ReflectionBasedDictionaryAdapter(argumentsAsAnonymousType));
		}

		/// <summary>
		/// Releases a component instance. This allows
		/// the kernel to execute the proper decomission
		/// lifecycles on the component instance.
		/// </summary>
		/// <param name="instance"></param>
		public virtual void ReleaseComponent(object instance)
		{
			if (ReleasePolicy.HasTrack(instance))
			{
				ReleasePolicy.Release(instance);
			}
			else
			{
				if (Parent != null)
				{
					Parent.ReleaseComponent(instance);
				}
			}
		}

		public IHandlerFactory HandlerFactory
		{
			get { return handlerFactory; }
		}

		public IComponentModelBuilder ComponentModelBuilder
		{
			get { return modelBuilder; }
			set { modelBuilder = value; }
		}

		public IProxyFactory ProxyFactory
		{
			get { return proxyFactory; }
			set { proxyFactory = value; }
		}

		public virtual IConfigurationStore ConfigurationStore
		{
			get { return GetSubSystem(SubSystemConstants.ConfigurationStoreKey) as IConfigurationStore; }
			set { AddSubSystem(SubSystemConstants.ConfigurationStoreKey, value); }
		}

		public virtual IHandler GetHandler(String key)
		{
			if (key == null) throw new ArgumentNullException("key");

			IHandler handler = NamingSubSystem.GetHandler(key);

			if (handler == null && Parent != null)
			{
				handler = WrapParentHandler(Parent.GetHandler(key));
			}

			return handler;
		}

		public virtual IHandler GetHandler(Type service)
		{
			if (service == null) throw new ArgumentNullException("service");

			IHandler handler = NamingSubSystem.GetHandler(service);

			if (handler == null && service.IsGenericType)
			{
				handler = NamingSubSystem.GetHandler(service.GetGenericTypeDefinition());
			}

			if (handler == null && Parent != null)
			{
				handler = WrapParentHandler(Parent.GetHandler(service));
			}

			return handler;
		}

		/// <summary>
		/// Return handlers for components that 
		/// implements the specified service.
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		public virtual IHandler[] GetHandlers(Type service)
		{
			IHandler[] result = NamingSubSystem.GetHandlers(service);

			// a complete generic type, Foo<Bar>, need to check if Foo<T> is registered
			if (service.IsGenericType && !service.IsGenericTypeDefinition)
			{
				IHandler[] genericResult = NamingSubSystem.GetHandlers(service.GetGenericTypeDefinition());

				if (result.Length > 0)
				{
					IHandler[] mergedResult = new IHandler[result.Length + genericResult.Length];
					result.CopyTo(mergedResult, 0);
					genericResult.CopyTo(mergedResult, result.Length);
					result = mergedResult;
				}
				else
				{
					result = genericResult;
				}
			}

			// If a parent kernel exists, we merge both results
			if (Parent != null)
			{
				IHandler[] parentResult = Parent.GetHandlers(service);

				if (parentResult.Length > 0)
				{
					IHandler[] newResult = new IHandler[result.Length + parentResult.Length];
					result.CopyTo(newResult, 0);
					parentResult.CopyTo(newResult, result.Length);
					result = newResult;
				}
			}

			return result;
		}

		/// <summary>
		/// Return handlers for components that 
		/// implements the specified service. 
		/// The check is made using IsAssignableFrom
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		public virtual IHandler[] GetAssignableHandlers(Type service)
		{
			IHandler[] result = NamingSubSystem.GetAssignableHandlers(service);

			// If a parent kernel exists, we merge both results
			if (Parent != null)
			{
				IHandler[] parentResult = Parent.GetAssignableHandlers(service);

				if (parentResult.Length > 0)
				{
					IHandler[] newResult = new IHandler[result.Length + parentResult.Length];
					result.CopyTo(newResult, 0);
					parentResult.CopyTo(newResult, result.Length);
					result = newResult;
				}
			}

			return result;
		}

		public virtual IReleasePolicy ReleasePolicy
		{
			get { return releaserPolicy; }
			set { releaserPolicy = value; }
		}

		public virtual IKernel AddFacility(String key, IFacility facility)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (facility == null) throw new ArgumentNullException("facility");

			facility.Init(this, ConfigurationStore.GetFacilityConfiguration(key));

			facilities.Add(facility);
			return this;
		}

		public IKernel AddFacility<T>(String key) where T : IFacility, new()
		{
			return AddFacility(key, new T());
		}

		public IKernel AddFacility<T>() where T : IFacility, new()
		{
			return AddFacility<T>(typeof(T).FullName);
		}

		/// <summary>
		/// Returns the facilities registered on the kernel.
		/// </summary>
		/// <returns></returns>
		public virtual IFacility[] GetFacilities()
		{
			IFacility[] list = new IFacility[facilities.Count];
			facilities.CopyTo(list, 0);
			return list;
		}

		public virtual void AddSubSystem(String key, ISubSystem subsystem)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (subsystem == null) throw new ArgumentNullException("facility");

			subsystem.Init(this);
			subsystems[key] = subsystem;
		}

		public virtual ISubSystem GetSubSystem(String key)
		{
			if (key == null) throw new ArgumentNullException("key");

			return subsystems[key] as ISubSystem;
		}

		public virtual void AddChildKernel(IKernel childKernel)
		{
			if (childKernel == null) throw new ArgumentNullException("childKernel");

			childKernel.Parent = this;
			childKernels.Add(childKernel);
		}

		public virtual IKernel Parent
		{
			get { return parentKernel; }
			set
			{
				// TODO: should the raise add/removed as child kernel methods be invoked from within the subscriber/unsubscribe methods?

				if (value == null)
				{
					if (parentKernel != null)
					{
						UnsubscribeFromParentKernel();
						RaiseRemovedAsChildKernel();
					}

					parentKernel = null;
				}
				else
				{
					if ((parentKernel != value) && (parentKernel != null))
					{
						throw new KernelException(
							"You can not change the kernel parent once set, use the RemoveChildKernel and AddChildKernel methods together to achieve this.");
					}
					parentKernel = value;
					SubscribeToParentKernel();
					RaiseAddedAsChildKernel();
				}
			}
		}

		public IDependencyResolver Resolver
		{
			get { return resolver; }
		}

		public virtual IComponentActivator CreateComponentActivator(ComponentModel model)
		{
			if (model == null) throw new ArgumentNullException("model");

			IComponentActivator activator;

			if (model.CustomComponentActivator == null)
			{
				activator = new DefaultComponentActivator(model, this,
														  new ComponentInstanceDelegate(RaiseComponentCreated),
														  new ComponentInstanceDelegate(RaiseComponentDestroyed));
			}
			else
			{
				try
				{
					activator = (IComponentActivator)
								Activator.CreateInstance(model.CustomComponentActivator,
														 new object[]
					                                     	{
					                                     		model,
					                                     		this,
					                                     		new ComponentInstanceDelegate(RaiseComponentCreated),
					                                     		new ComponentInstanceDelegate(RaiseComponentDestroyed)
					                                     	});
				}
				catch (Exception e)
				{
					throw new KernelException("Could not instantiate custom activator", e);
				}
			}

			return activator;
		}

		/// <summary>
		/// Graph of components and iteractions.
		/// </summary>
		public GraphNode[] GraphNodes
		{
			get
			{
				GraphNode[] nodes = new GraphNode[NamingSubSystem.ComponentCount];

				int index = 0;

				IHandler[] handlers = NamingSubSystem.GetHandlers();

				foreach (IHandler handler in handlers)
				{
					nodes[index++] = handler.ComponentModel;
				}

				return nodes;
			}
		}

		public void RegisterHandlerForwarding(Type forwardedType, string name)
		{
			IHandler target = GetHandler(name);
			if(target==null)
				throw new InvalidOperationException("There is no handler named " + name);
			IHandler handler = HandlerFactory.CreateForwarding(target, forwardedType);
			RegisterHandler(forwardedType.FullName + ": Forward to: " + name, handler);
		}

		public virtual void RemoveChildKernel(IKernel childKernel)
		{
			if (childKernel == null) throw new ArgumentNullException("childKernel");
			childKernel.Parent = null;
			childKernels.Remove(childKernel);
		}

		#endregion

		#region IServiceProviderEx Members

		/// <summary>
		/// Gets the service object of the specified type.
		/// </summary>
		///
		/// <returns>
		/// A service object of type serviceType.
		/// </returns>
		///
		/// <param name="serviceType">An object that specifies the type of service object to get. </param>
		public object GetService(Type serviceType)
		{
			if (!HasComponent(serviceType))
			{
				return null;
			}

			return Resolve(serviceType);
		}

		/// <summary>
		/// Gets the service object of the specified type.
		/// </summary>
		///
		/// <returns>
		/// A service object of type serviceType.
		/// </returns>
		public T GetService<T>() where T : class
		{
			Type serviceType = typeof(T);

			if (!HasComponent(serviceType))
			{
				return null;
			}

			return (T)Resolve(serviceType);
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Starts the process of component disposal.
		/// </summary>
		public virtual void Dispose()
		{
			DisposeSubKernels();
			TerminateFacilities();
			DisposeComponentsInstancesWithinTracker();
			DisposeHandlers();
			UnsubscribeFromParentKernel();
		}

		private void TerminateFacilities()
		{
			foreach (IFacility facility in facilities)
			{
				facility.Terminate();
			}
		}

		private void DisposeHandlers()
		{
			GraphNode[] nodes = GraphNodes;
			IVertex[] vertices = TopologicalSortAlgo.Sort(nodes);

			for (int i = 0; i < vertices.Length; i++)
			{
				ComponentModel model = (ComponentModel)vertices[i];

				// Prevent the removal of a component that belongs 
				// to other container
				if (!NamingSubSystem.Contains(model.Name)) continue;

				RemoveComponent(model.Name);
			}
		}

		private void UnsubscribeFromParentKernel()
		{
			if (parentKernel != null)
			{
				parentKernel.HandlerRegistered -= new HandlerDelegate(HandlerRegisteredOnParentKernel);
				parentKernel.ComponentRegistered -= new ComponentDataDelegate(RaiseComponentRegistered);
				parentKernel.ComponentUnregistered -= new ComponentDataDelegate(RaiseComponentUnregistered);
			}
		}

		private void SubscribeToParentKernel()
		{
			if (parentKernel != null)
			{
				parentKernel.HandlerRegistered += new HandlerDelegate(HandlerRegisteredOnParentKernel);
				parentKernel.ComponentRegistered += new ComponentDataDelegate(RaiseComponentRegistered);
				parentKernel.ComponentUnregistered += new ComponentDataDelegate(RaiseComponentUnregistered);
			}
		}

		private void HandlerRegisteredOnParentKernel(IHandler handler, ref bool stateChanged)
		{
			RaiseHandlerRegistered(handler);
		}

		private void DisposeComponentsInstancesWithinTracker()
		{
			ReleasePolicy.Dispose();
		}

		private void DisposeSubKernels()
		{
			foreach (IKernel childKernel in childKernels)
			{
				childKernel.Dispose();
			}
		}

		protected void DisposeHandler(IHandler handler)
		{
			if (handler == null) return;

			if (handler is IDisposable)
			{
				((IDisposable)handler).Dispose();
			}
		}

		#endregion

		#region Protected members

		protected virtual IHandler WrapParentHandler(IHandler parentHandler)
		{
			if (parentHandler == null) return null;

			// This has a very destructive side-effect. While the goal is to resolve on same-level containers,
			// the resolver will invoke GetHandler recursively, leading to stack overflows
			// return new ParentHandlerWithChildResolver(parentHandler, Resolver);
			return parentHandler;
		}

		protected INamingSubSystem NamingSubSystem
		{
			get { return GetSubSystem(SubSystemConstants.NamingKey) as INamingSubSystem; }
		}

		protected void RegisterHandler(String key, IHandler handler)
		{
			RegisterHandler(key, handler, false);
		}

		protected void RegisterHandler(String key, IHandler handler, bool skipRegistration)
		{
			if (!skipRegistration)
			{
				NamingSubSystem.Register(key, handler);
			}

			base.RaiseHandlerRegistered(handler);
			base.RaiseComponentRegistered(key, handler);
		}

		protected object ResolveComponent(IHandler handler)
		{
			return ResolveComponent(handler, handler.ComponentModel.Service);
		}

		protected object ResolveComponent(IHandler handler, Type service)
		{
			return ResolveComponent(handler, service, null);
		}

		protected object ResolveComponent(IHandler handler, IDictionary additionalArguments)
		{
			return ResolveComponent(handler, handler.ComponentModel.Service, additionalArguments);
		}

		protected object ResolveComponent(IHandler handler, Type service, IDictionary additionalArguments)
		{
			CreationContext context = CreateCreationContext(handler, service, additionalArguments);

			object instance = handler.Resolve(context);

			ReleasePolicy.Track(instance, handler);

			return instance;
		}

		protected CreationContext CreateCreationContext(IHandler handler, Type typeToExtractGenericArguments,
		                                                IDictionary additionalArguments)
		{
			return new CreationContext(handler, typeToExtractGenericArguments, additionalArguments);
		}

		#endregion

		#region Serialization and Deserialization

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			MemberInfo[] members = FormatterServices.GetSerializableMembers(GetType(), context);

			object[] kernelmembers = FormatterServices.GetObjectData(this, members);

			info.AddValue("members", kernelmembers, typeof(object[]));
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
		}

		#endregion
	}
}
