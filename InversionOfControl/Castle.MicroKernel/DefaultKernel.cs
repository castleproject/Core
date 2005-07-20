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

namespace Castle.MicroKernel
{
	using System;
	using System.Reflection;
	using System.Collections;
	using System.Runtime.Serialization;

	using Castle.Model;
	using Castle.Model.Internal;
	
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
	[Serializable]
	public class DefaultKernel : KernelEventSupport, IKernel, IDeserializationCallback
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
		public DefaultKernel(IDependencyResolver resolver, IProxyFactory proxyFactory) : this(proxyFactory)
		{
			_resolver = resolver;
		}

		/// <summary>
		/// Constructs a DefaultKernel with the specified
		/// implementation of <see cref="IProxyFactory"/>
		/// </summary>
		public DefaultKernel(IProxyFactory proxyFactory)
		{
			_proxyFactory = proxyFactory;

			_childKernels = new ArrayList();
			_facilities = new ArrayList();
			_subsystems = new Hashtable();

			AddSubSystem( SubSystemConstants.ConfigurationStoreKey, 
				new DefaultConfigurationStore() );
			
			AddSubSystem( SubSystemConstants.ConversionManagerKey, 
				new SubSystems.Conversion.DefaultConversionManager() );

			AddSubSystem( SubSystemConstants.NamingKey, 
				new SubSystems.Naming.DefaultNamingSubSystem() );

			_releaserPolicy = new LifecycledComponentsReleasePolicy();
			_handlerFactory = new DefaultHandlerFactory(this);
			_modelBuilder = new DefaultComponentModelBuilder(this);
			_resolver = new DefaultDependencyResolver(this);
		}

		public DefaultKernel(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			MemberInfo[] members = FormatterServices.GetSerializableMembers( GetType(), context );
			
			object[] kernelmembers = (object[]) info.GetValue( "members", typeof(object[]) );
			
			FormatterServices.PopulateObjectMembers( this, members, kernelmembers );
		}

		#endregion

		#region IKernel Members

		public virtual void AddComponent(String key, Type classType)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (classType == null) throw new ArgumentNullException("classType");

			ComponentModel model = ComponentModelBuilder.BuildModel(key, classType, classType, null);
			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(key, handler);
		}

		public virtual void AddComponent(String key, Type serviceType, Type classType)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (serviceType == null) throw new ArgumentNullException("serviceType");
			if (classType == null) throw new ArgumentNullException("classType");

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
		public virtual void AddComponentWithProperties( String key, Type serviceType, Type classType, IDictionary parameters )
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
		public virtual void AddCustomComponent( ComponentModel model )
		{
			if (model == null) throw new ArgumentNullException("model");

			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(model.Name, handler);
		}

		/// <summary>
		/// Used mostly by facilities. Adds an instance
		/// to be used as a component.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="instance"></param>
		public void AddComponentInstance( String key, object instance )
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
		public void AddComponentInstance( String key, Type serviceType, object instance )
		{
			if (key == null) throw new ArgumentNullException("key");
			if (serviceType == null) throw new ArgumentNullException("serviceType");
			if (instance == null) throw new ArgumentNullException("instance");
			
			Type classType = instance.GetType();

			ComponentModel model = new ComponentModel(key, serviceType, classType);
			model.CustomComponentActivator = typeof(ExternalInstanceActivator);
			model.ExtendedProperties["instance"] = instance;

			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(key, handler);
		}

		public virtual bool RemoveComponent(String key)
		{
			if (key == null) throw new ArgumentNullException("key");

			if (NamingSubSystem.Contains(key))
			{
				IHandler handler = GetHandler(key);

				if (handler.ComponentModel.Dependers.Length == 0)
				{
					NamingSubSystem.UnRegister(key);

					if (GetHandler(handler.ComponentModel.Service) == handler)
					{
						NamingSubSystem.UnRegister(handler.ComponentModel.Service);
					}

					foreach(ComponentModel model in handler.ComponentModel.Dependents)
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
			if (key == null) throw new ArgumentNullException("key");

			IHandler handler = NamingSubSystem.GetHandler(key);

			if (handler == null && Parent != null)
			{
				handler = Parent.GetHandler(key);
			}

			return handler;
		}

		public virtual IHandler GetHandler(Type service)
		{
			if (service == null) throw new ArgumentNullException("service");

			IHandler handler = NamingSubSystem.GetHandler(service);

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
			return NamingSubSystem.GetHandlers(service);
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
			return NamingSubSystem.GetAssignableHandlers(service);
		}

		public virtual IReleasePolicy ReleasePolicy
		{
			get { return _releaserPolicy; }
		}

		public virtual void AddFacility(String key, IFacility facility)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (facility == null) throw new ArgumentNullException("facility");

			facility.Init(this, ConfigurationStore.GetFacilityConfiguration(key));

			_facilities.Add(facility);
		}

		/// <summary>
		/// Returns the facilities registered on the kernel.
		/// </summary>
		/// <returns></returns>
		public virtual IFacility[] GetFacilities()
		{
			IFacility[] list = new IFacility[ _facilities.Count ];
			_facilities.CopyTo(list, 0);
			return list;
		}

		public virtual void AddSubSystem(String key, ISubSystem subsystem)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (subsystem == null) throw new ArgumentNullException("facility");

			subsystem.Init(this);
			_subsystems[key] = subsystem;
		}

		public virtual ISubSystem GetSubSystem(String key)
		{
			if (key == null) throw new ArgumentNullException("key");

			return _subsystems[key] as ISubSystem;
		}

		public virtual void AddChildKernel(IKernel childKernel)
		{
			if (childKernel == null) throw new ArgumentNullException("childKernel");

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
			if (model == null) throw new ArgumentNullException("model");

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

		/// <summary>
		/// Graph of components and iteractions.
		/// </summary>
		public GraphNode[] GraphNodes 
		{ 
			get
			{
				GraphNode[] nodes = new GraphNode[ NamingSubSystem.ComponentCount ];

				int index = 0;

				IHandler[] handlers = NamingSubSystem.GetHandlers();

				foreach(IHandler handler in handlers)
				{
					nodes[index++] = handler.ComponentModel;
				}

				return nodes;
			} 
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
			foreach(IFacility facility in _facilities)
			{
				facility.Terminate();
			}
		}

		private void DisposeHandlers()
		{
			GraphNode[] nodes = GraphNodes;
			IVertex[] vertices = TopologicalSortAlgo.Sort( nodes );
	
			for(int i=0; i < vertices.Length; i++)
			{
				ComponentModel model = (ComponentModel) vertices[i];

				// Prevent the removal of a component that belongs 
				// to other container
				if (!NamingSubSystem.Contains(model.Name)) continue;
				
				bool successOnRemoval = RemoveComponent( model.Name );

				System.Diagnostics.Debug.Assert( successOnRemoval );
			}
		}

		private void UnsubscribeFromParentKernel()
		{
			if (Parent != null)
			{
				Parent.ComponentRegistered -= new ComponentDataDelegate(RaiseComponentRegistered);
				Parent.ComponentUnregistered -= new ComponentDataDelegate(RaiseComponentUnregistered);
			}
		}

		private void DisposeComponentsInstancesWithinTracker()
		{
			ReleasePolicy.Dispose();
		}

		private void DisposeSubKernels()
		{
			foreach(IKernel childKernel in _childKernels)
			{
				childKernel.Dispose();
			}
		}

		protected void DisposeHandler(IHandler handler)
		{
			if (handler == null) return;

			if (handler is IDisposable)
			{
				((IDisposable) handler).Dispose();
			}
		}

		#endregion

		#region Protected members

		protected INamingSubSystem NamingSubSystem
		{
			get { return GetSubSystem(SubSystemConstants.NamingKey) as INamingSubSystem; }
		}

		protected void RegisterHandler(String key, IHandler handler)
		{
			NamingSubSystem.Register(key, handler);

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

		#region Serialization and Deserialization

		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			MemberInfo[] members = FormatterServices.GetSerializableMembers( GetType(), context );

			object[] kernelmembers = FormatterServices.GetObjectData(this, members);

			info.AddValue( "members", kernelmembers, typeof(object[]) );
		}

		void IDeserializationCallback.OnDeserialization(object sender)
		{
		}

		#endregion
	}
}
