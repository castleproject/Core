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
	using System.Collections;
	using System.Collections.Specialized;

	using Castle.Model;
	
	using Castle.MicroKernel.Handlers;
	using Castle.MicroKernel.ModelBuilder;
	using Castle.MicroKernel.Resolvers;
	using Castle.MicroKernel.Releasers;
	using Castle.MicroKernel.ComponentActivator;
	using Castle.MicroKernel.Proxy;
	using Castle.MicroKernel.SubSystems.Configuration;

	/// <summary>
	/// Default implementation of <see cref="IKernel"/>. 
	/// This implementation is complete and also support a kernel 
	/// hierarchy (sub containers).
	/// </summary>
	public class DefaultKernel : KernelEventSupport, IKernel
	{
		#region Fields

		/// <summary>
		/// The parent kernel, if exists.
		/// </summary>
		private IKernel _parentKernel;

		/// <summary>
		/// The implementation of <see cref="IHandlerFactory"/>
		/// </summary>
		private IHandlerFactory _handlerFactory;

		/// <summary>
		/// The implementation of <see cref="IComponentModelBuilder"/>
		/// </summary>
		private IComponentModelBuilder _modelBuilder;

		/// <summary>
		/// The dependency resolver.
		/// </summary>
		private IDependencyResolver _resolver;

		/// <summary>
		/// Implements a policy to control component's
		/// disposal that the usef forgot.
		/// </summary>
		private IReleasePolicy _releaserPolicy;

		/// <summary>
		/// Holds the implementation of <see cref="IProxyFactory"/>
		/// </summary>
		private IProxyFactory _proxyFactory;

		/// <summary>
		/// List of <see cref="IFacility"/> registered.
		/// </summary>
		private IList _facilities;

		/// <summary>
		/// Map of subsystems registered.
		/// </summary>
		private IDictionary _subsystems;
		
		/// <summary>
		/// List of sub containers.
		/// </summary>
		private IList _childKernels;

		/// <summary>
		/// Map(String, IHandler) to map component keys
		/// to <see cref="IHandler"/>
		/// </summary>
		private IDictionary _key2Handler;

		/// <summary>
		/// Map(Type, IHandler) to map services 
		/// to <see cref="IHandler"/>
		/// </summary>
		private IDictionary _service2Handler;

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
		/// implementation of <see cref="IProxyFactory"/>
		/// </summary>
		public DefaultKernel(IProxyFactory proxyFactory)
		{
			_proxyFactory = proxyFactory;

			_key2Handler = new HybridDictionary();
			_service2Handler = new Hashtable();
			_childKernels = new ArrayList();
			_facilities = new ArrayList();
			_subsystems = new Hashtable();

			AddSubSystem( SubSystemConstants.ConfigurationStoreKey, 
				new DefaultConfigurationStore() );
			
			AddSubSystem( SubSystemConstants.ConversionManagerKey, 
				new SubSystems.Conversion.DefaultConversionManager() );

			_releaserPolicy = new LifecycledComponentsReleasePolicy();
			_handlerFactory = new DefaultHandlerFactory(this);
			_modelBuilder = new DefaultComponentModelBuilder(this);
			_resolver = new DefaultDependecyResolver(this);
		}

		#endregion

		#region IKernel Members

		public virtual void AddComponent(String key, Type classType)
		{
			ComponentModel model = ComponentModelBuilder.BuildModel(key, classType, classType, null);
			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(key, handler);
		}

		public virtual void AddComponent(String key, Type serviceType, Type classType)
		{
			ComponentModel model = ComponentModelBuilder.BuildModel(key, serviceType, classType, null);
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
		public virtual void AddComponentWithProperties( String key, Type classType, IDictionary parameters )
		{
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
		public virtual void AddComponentWithProperties( String key, Type serviceType, Type classType, IDictionary parameters )
		{
			ComponentModel model = ComponentModelBuilder.BuildModel(key, classType, classType, parameters);
			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(key, handler);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="model"></param>
		public virtual void AddCustomComponent( ComponentModel model )
		{
			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(model.Name, handler);
		}

		public virtual bool RemoveComponent(String key)
		{
			return false;
		}

		public virtual bool HasComponent(String key)
		{
			if (_key2Handler.Contains(key))
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
			if (_service2Handler.Contains(serviceType))
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
				if (!HasComponent(service))
				{
					throw new ComponentNotFoundException(service);
				}

				IHandler handler = GetHandler(service);

				return ResolveComponent(handler);
			}
		}

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
			get { return _handlerFactory; }
		}

		public IComponentModelBuilder ComponentModelBuilder
		{
			get { return _modelBuilder; }
		}

		public IProxyFactory ProxyFactory
		{
			get { return _proxyFactory; }
			set { _proxyFactory = value; }
		}

		public virtual IConfigurationStore ConfigurationStore
		{
			get { return GetSubSystem(SubSystemConstants.ConfigurationStoreKey) as IConfigurationStore; }
			set { AddSubSystem(SubSystemConstants.ConfigurationStoreKey, value); }
		}

		public virtual IHandler GetHandler(String key)
		{
			IHandler handler = _key2Handler[key] as IHandler;

			if (handler == null && Parent != null)
			{
				handler = Parent.GetHandler(key);
			}

			return handler;
		}

		public virtual IHandler GetHandler(Type service)
		{
			IHandler handler = _service2Handler[service] as IHandler;

			if (handler == null && Parent != null)
			{
				handler = Parent.GetHandler(service);
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
			ArrayList list = new ArrayList();

			foreach( IHandler handler in _key2Handler.Values )
			{
				if (handler.ComponentModel.Service == service)
				{
					list.Add(handler);
				}
			}

			return (IHandler[]) list.ToArray( typeof(IHandler) );
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
			ArrayList list = new ArrayList();

			foreach( IHandler handler in _key2Handler.Values )
			{
				if ( service.IsAssignableFrom(handler.ComponentModel.Service))
				{
					list.Add(handler);
				}
			}

			return (IHandler[]) list.ToArray( typeof(IHandler) );
		}

		public IReleasePolicy ReleasePolicy
		{
			get { return _releaserPolicy; }
		}

		public virtual void AddFacility(String key, IFacility facility)
		{
			facility.Init(this, ConfigurationStore.GetFacilityConfiguration(key));

			_facilities.Add(facility);
		}

		public virtual void AddSubSystem(String key, ISubSystem subsystem)
		{
			subsystem.Init(this);
			_subsystems[key] = subsystem;
		}

		public virtual ISubSystem GetSubSystem(String key)
		{
			return _subsystems[key] as ISubSystem;
		}

		// void ConfigureExternalComponent(object component);

		// void ConfigureExternalComponent(object component, ComponentModel model);

		public virtual void AddChildKernel(IKernel childKernel)
		{
			childKernel.Parent = this;
			_childKernels.Add(childKernel);
		}

		public virtual IKernel Parent
		{
			get { return _parentKernel; }
			set
			{
				// TODO: Assert no previous parent was setted
				// TODO: Assert value is not null

				_parentKernel = value;

				_parentKernel.ComponentRegistered += new ComponentDataDelegate(RaiseComponentRegistered);
				_parentKernel.ComponentUnregistered += new ComponentDataDelegate(RaiseComponentUnregistered);

				RaiseAddedAsChildKernel();
			}
		}

		public IDependencyResolver Resolver
		{
			get { return _resolver; }
		}

		public virtual IComponentActivator CreateComponentActivator(ComponentModel model)
		{
			IComponentActivator activator = null;

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
						        model, this,
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

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Starts the process of component disposal.
		/// </summary>
		public virtual void Dispose()
		{
			_releaserPolicy.Dispose();

			foreach (DictionaryEntry entry in _key2Handler)
			{
				IHandler handler = (IHandler) entry.Value;

				if (handler is IDisposable)
				{
					((IDisposable) handler).Dispose();
				}
			}

			if (Parent != null)
			{
				Parent.ComponentRegistered -= new ComponentDataDelegate(RaiseComponentRegistered);
				Parent.ComponentUnregistered -= new ComponentDataDelegate(RaiseComponentUnregistered);
			}
		}

		#endregion

		#region Protected members

		protected void RegisterHandler(String key, IHandler handler)
		{
			Type service = handler.ComponentModel.Service;

			if (_key2Handler.Contains(key))
			{
				throw new ComponentRegistrationException(
					String.Format("There is a component already registered for the given key {0}", key));
			}

			if (!_service2Handler.Contains(service))
			{
				_service2Handler[service] = handler;
			}

			_key2Handler[key] = handler;

			base.RaiseHandlerRegistered(handler);
			base.RaiseComponentRegistered(key, handler);
		}

		protected object ResolveComponent(IHandler handler)
		{
			object instance = handler.Resolve();

			ReleasePolicy.Track(instance, handler);

			return instance;
		}

		#endregion
	}
}