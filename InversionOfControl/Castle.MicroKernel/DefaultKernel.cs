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
	using Castle.MicroKernel.Exceptions;
	using Castle.MicroKernel.SubSystems.Configuration;

	/// <summary>
	/// Summary description for DefaultKernel.
	/// </summary>
	public class DefaultKernel : KernelEventSupport, IKernel
	{
		private static readonly String ConfigurationStoreKey = "config.store";

		private IKernel _parentKernel;
		private IHandlerFactory _handlerFactory;
		private IComponentModelBuilder _modelBuilder;
		private IDependecyResolver _resolver;
		private IReleasePolicy _releaserPolicy;
		private IProxyFactory _proxyFactory;
		private IList _facilities;
		private IDictionary _subsystems;
		private IList _childKernels;

		private IDictionary _key2Handler;
		private IDictionary _service2Handler;

		public DefaultKernel() : this(new NotSupportedProxyFactory())
		{
			_key2Handler = new HybridDictionary();
			_service2Handler = new Hashtable();
			_childKernels = new ArrayList();
			_facilities = new ArrayList();
			_subsystems = new Hashtable();

			_releaserPolicy = new LifecycledComponentsReleasePolicy();
			_resolver = new DefaultDependecyResolver(this);
			_handlerFactory = new DefaultHandlerFactory(this);
			_modelBuilder = new DefaultComponentModelBuilder(this);
			_proxyFactory = new NotSupportedProxyFactory();
			ConfigurationStore = new DefaultConfigurationStore();
		}

		public DefaultKernel(IProxyFactory proxyFactory)
		{
			_proxyFactory = proxyFactory;
		}

		#region IKernel Members

		public void AddComponent(String key, Type classType)
		{
			ComponentModel model = ComponentModelBuilder.BuildModel(key, classType, classType);
			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(key, handler);
		}

		public void AddComponent(String key, Type serviceType, Type classType)
		{
			ComponentModel model = ComponentModelBuilder.BuildModel(key, classType, classType);
			RaiseComponentModelCreated(model);
			IHandler handler = HandlerFactory.Create(model);
			RegisterHandler(key, handler);
		}

		public bool RemoveComponent(String key)
		{
			return false;
		}

		public bool HasComponent(String key)
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

		public bool HasComponent(Type serviceType)
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

		public object this[String key]
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

		public object this[Type service]
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

		public void ReleaseComponent(object instance)
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

		public IConfigurationStore ConfigurationStore
		{
			get { return GetSubSystem(ConfigurationStoreKey) as IConfigurationStore; }
			set { AddSubSystem(ConfigurationStoreKey, value); }
		}

		public IHandler GetHandler(String key)
		{
			IHandler handler = _key2Handler[key] as IHandler;

			if (handler == null && Parent != null)
			{
				handler = Parent.GetHandler(key);
			}

			return handler;
		}

		public IHandler GetHandler(Type service)
		{
			IHandler handler = _service2Handler[service] as IHandler;

			if (handler == null && Parent != null)
			{
				handler = Parent.GetHandler(service);
			}

			return handler;
		}

		public IReleasePolicy ReleasePolicy
		{
			get { return _releaserPolicy; }
		}

		public void AddFacility(String key, IFacility facility)
		{
			facility.Init(this, ConfigurationStore.GetFacilityConfiguration(key));

			_facilities.Add(facility);
		}

		public void AddSubSystem(String key, ISubSystem subsystem)
		{
			subsystem.Init(this);

			_subsystems[key] = subsystem;
		}

		public ISubSystem GetSubSystem(String key)
		{
			return _subsystems[key] as ISubSystem;
		}

		// void ConfigureExternalComponent(object component);

		// void ConfigureExternalComponent(object component, ComponentModel model);

		public void AddChildKernel(IKernel childKernel)
		{
			_childKernels.Add(childKernel);

			childKernel.Parent = this;
		}

		public IKernel Parent
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

		public IDependecyResolver Resolver
		{
			get { return _resolver; }
		}

		public IComponentActivator CreateComponentActivator(ComponentModel model)
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

			base.RaiseComponentRegistered(key, handler);
		}

		protected object ResolveComponent(IHandler handler)
		{
			object instance = handler.Resolve();

			ReleasePolicy.Track(instance, handler);

			return instance;
		}
	}
}