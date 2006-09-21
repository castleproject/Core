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

namespace Castle.MicroKernel
{
	using System;
	using System.Reflection;
	using System.Collections;
	using System.Runtime.Serialization;

	using Castle.Core;
	using Castle.Core.Internal;
	
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
		public DefaultKernel(IDependencyResolver resolver, IProxyFactory proxyFactory) : this(proxyFactory)
		{
			this.resolver = resolver;
			this.resolver.Initialize(new DependencyDelegate(RaiseDependencyResolving));
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

		public DefaultKernel(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			MemberInfo[] members = FormatterServices.GetSerializableMembers(GetType(), context);
			
			object[] kernelmembers = (object[]) info.GetValue("members", typeof(object[]));
			
			FormatterServices.PopulateObjectMembers(this, members, kernelmembers);
		}

		#endregion

		#region Overridables

		protected virtual void RegisterSubSystems()
		{
			AddSubSystem(SubSystemConstants.ConfigurationStoreKey, 
				new DefaultConfigurationStore());
	
			AddSubSystem(SubSystemConstants.ConversionManagerKey, 
				new SubSystems.Conversion.DefaultConversionManager());
	
			AddSubSystem(SubSystemConstants.NamingKey, 
				new SubSystems.Naming.DefaultNamingSubSystem());

			AddSubSystem(SubSystemConstants.ResourceKey, 
				new SubSystems.Resource.DefaultResourceSubSystem());
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
		public virtual void AddComponentWithExtendedProperties(String key, Type serviceType, Type classType, IDictionary parameters)
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
			RegisterHandler(model.Name, handler);
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
#if DOTNET2
			if (serviceType.IsGenericType && NamingSubSystem.Contains(serviceType.GetGenericTypeDefinition()))
			{
				return true;
			}
#endif

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

		public void RegisterCustomDependencies(Type service, IDictionary dependencies)
		{
			IHandler handler = GetHandler(service);

			foreach(DictionaryEntry entry in dependencies)
			{
				handler.AddCustomDependencyValue(entry.Key.ToString(), entry.Value);
			}
		}

		public void RegisterCustomDependencies(String key, IDictionary dependencies)
		{
			IHandler handler = GetHandler(key);

			foreach(DictionaryEntry entry in dependencies)
			{
				handler.AddCustomDependencyValue(entry.Key.ToString(), entry.Value);
			}
		}

		#if DOTNET2

		/// <summary>
		/// Returns a component instance by the key
		/// </summary>
		/// <param name="key"></param>
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

		#endif

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
				handler = Parent.GetHandler(key);
			}

			return handler;
		}

		public virtual IHandler GetHandler(Type service)
		{
			if (service == null) throw new ArgumentNullException("service");

			IHandler handler = NamingSubSystem.GetHandler(service);

#if DOTNET2
			if (handler == null && service.IsGenericType)
			{
				handler = NamingSubSystem.GetHandler(service.GetGenericTypeDefinition());
			}
#endif
			if (handler == null && Parent != null)
			{
				handler = Parent.GetHandler(service);
			}

			return handler;
		}

//        public virtual IHandler GetHandler(String key, Type service)
//        {
//            if (key == null) throw new ArgumentNullException("key"); 
//            if (service == null) throw new ArgumentNullException("service");
//
//            IHandler handler = NamingSubSystem.GetHandler(key, service);
//
//            if (handler == null && Parent != null)
//            {
//                handler = Parent.GetHandler(key, service);
//            }
//
//            return handler;
//        }

		/// <summary>
		/// Return handlers for components that 
		/// implements the specified service.
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		public virtual IHandler[] GetHandlers(Type service)
		{
			IHandler[] result = NamingSubSystem.GetHandlers(service);

#if DOTNET2
			// a complete generic type, Foo<Bar>, need to check if Foo<T> is registered
			if (result.Length == 0 && service.IsGenericType && !service.IsGenericTypeDefinition) 
			{
				result = NamingSubSystem.GetHandlers(service.GetGenericTypeDefinition());
			}
#endif	

			// If a parent kernel exists, we merge both results
			if (Parent != null) 
			{
				IHandler[] parentResult = Parent.GetHandlers(service);

				if (parentResult.Length != 0)
				{
					IHandler[] newResult = new IHandler[result.Length + parentResult.Length];
					
					Array.Copy(result, newResult, result.Length);
					Array.Copy(parentResult, 0, newResult, result.Length, parentResult.Length);

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
			return NamingSubSystem.GetAssignableHandlers(service);
		}

		public virtual IReleasePolicy ReleasePolicy
		{
			get { return releaserPolicy; }
		}

		public virtual void AddFacility(String key, IFacility facility)
		{
			if (key == null) throw new ArgumentNullException("key");
			if (facility == null) throw new ArgumentNullException("facility");

			facility.Init(this, ConfigurationStore.GetFacilityConfiguration(key));

			facilities.Add(facility);
		}

		/// <summary>
		/// Returns the facilities registered on the kernel.
		/// </summary>
		/// <returns></returns>
		public virtual IFacility[] GetFacilities()
		{
			IFacility[] list = new IFacility[ facilities.Count ];
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
						throw new KernelException("You can not change the kernel parent once set, use the RemoveChildKernel and AddChildKernel methods together to achieve this.");
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

		public virtual void RemoveChildKernel(IKernel childKernel)
		{
			if (childKernel == null) throw new ArgumentNullException("childKernel");
			childKernel.Parent = null;
			childKernels.Remove(childKernel);
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
			foreach(IFacility facility in facilities)
			{
				facility.Terminate();
			}
		}

		private void DisposeHandlers()
		{
			GraphNode[] nodes = GraphNodes;
			IVertex[] vertices = TopologicalSortAlgo.Sort(nodes);
	
			for(int i=0; i < vertices.Length; i++)
			{
				ComponentModel model = (ComponentModel) vertices[i];

				// Prevent the removal of a component that belongs 
				// to other container
				if (!NamingSubSystem.Contains(model.Name)) continue;
				
				bool successOnRemoval = RemoveComponent(model.Name);

				System.Diagnostics.Debug.Assert(successOnRemoval);
			}
		}

		private void UnsubscribeFromParentKernel()
		{
			if (Parent != null)
			{
				Parent.ComponentRegistered -= new ComponentDataDelegate(RaiseComponentRegistered);
			}
		}

		private void SubscribeToParentKernel()
		{
			if (Parent != null)
			{
				parentKernel.ComponentRegistered += new ComponentDataDelegate(RaiseComponentRegistered);
				parentKernel.ComponentUnregistered += new ComponentDataDelegate(RaiseComponentUnregistered);
			}
		}

		private void DisposeComponentsInstancesWithinTracker()
		{
			ReleasePolicy.Dispose();
		}

		private void DisposeSubKernels()
		{
			foreach(IKernel childKernel in childKernels)
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
#if DOTNET2
			return new CreationContext(handler, typeToExtractGenericArguments, additionalArguments);
#else
			return new CreationContext(handler, additionalArguments);
#endif
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
