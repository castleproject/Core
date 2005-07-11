#region Copyright
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
#endregion

namespace Castle.Windsor.Adapters.ComponentModel
{
	using System;
	using System.Collections;
	using System.ComponentModel;
	using System.ComponentModel.Design;
	using System.Threading;

	using Castle.Model;
	using Castle.MicroKernel;

	/// <summary>
	/// Implementation of <see cref="IContainerAdapter"/> that does not assume ownership of the
	/// wrapped <see cref="IWindsorContainer"/>. 
	/// </summary>
	public class ContainerWrapper : IContainerAdapter
	{
		#region ContainerAdapter Fields

		private ISite _site;
		private IWindsorContainer _container;
		private IServiceProvider _parentProvider;
		private IList _sites = new ArrayList();
		private readonly ReaderWriterLock _lock;

		#endregion

		#region ContainerWrapper Constructors 

		/// <summary>
		/// Constructs an initial ContainerWrapper.
		/// </summary>
		/// <param name="container">The <see cref="IWindsorContainer"/> to adapt.</param>
		public ContainerWrapper(IWindsorContainer container)
			: this( container, null )
		{
			// Empty
		}

		/// <summary>
		/// Constructs an initial ContainerWrapper.
		/// </summary>
		/// <param name="container">The <see cref="IWindsorContainer"/> to adapt.</param>
		/// <param name="parentProvider">The parent <see cref="IServiceProvider"/>.</param>
		public ContainerWrapper(IWindsorContainer container, IServiceProvider parentProvider)
		{
			if (container == null)
			{
				container = CreateDefaultWindsorContainer();
			}

			if (container == null)
			{
				throw new ArgumentNullException("container");
			}

			_container = container;
			_parentProvider = parentProvider;
			_lock = new ReaderWriterLock();

			RegisterAdapterWithKernel();
		}

		#endregion

		#region IComponent Members

		/// <summary>
		/// Gets or sets the <see cref="ISite"/> associated with the <see cref="IComponent"/>.
		/// </summary>
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Browsable(false)]
		public virtual ISite Site
		{
			get { return _site; }
			set { _site = value; }
		}
 
		/// <summary>
		/// Event that notifies the disposal of the <see cref="IComponent"/>.
		/// </summary>
		[EditorBrowsable(EditorBrowsableState.Advanced), Browsable(false)]
		public event EventHandler Disposed;

		#endregion

		#region IContainer Members

		/// <summary>
		/// Gets all the components in the <see cref="IContainer"/>.
		/// </summary>
		public virtual ComponentCollection Components
		{
			get
			{
				_lock.AcquireReaderLock( Timeout.Infinite );

				try
				{
					IComponent[] components = new IComponent[_sites.Count];

					for (int i = 0; i < _sites.Count; ++i)
					{
						components[i] = ((ISite)_sites[i]).Component;
					}
					
					return new ComponentCollection(components);
				}
				finally
				{
					_lock.ReleaseReaderLock();
				}
			}
		}

		/// <summary>
		/// Adds the specified <see cref="IComponent"/> to the <see cref="IContainer"/> at the end of the list.
		/// </summary>
		/// <param name="component">The <see cref="IComponent"/> to add.</param>
		public virtual void Add(IComponent component)
		{
			Add( component, null );
		}

		/// <summary>
		/// Adds the specified <see cref="IComponent"/> to the <see cref="IContainer"/> at the end of the list,
		/// and assigns a name to the component.
		/// </summary>
		/// <param name="component">The <see cref="IComponent"/> to add.</param>
		/// <param name="name">The unique, case-insensitive name to assign to the component, or null.</param>
		public virtual void Add(IComponent component, String name)
		{
			if (component != null)
			{
				_lock.AcquireWriterLock( Timeout.Infinite );

				try
				{
					ISite site = component.Site;
				
					if ((site == null) || (site.Container != this))
					{
						IContainerAdapterSite newSite = CreateSite( component, name );

						try
						{
							Kernel.AddComponentInstance( newSite.EffectiveName, typeof(IComponent), component );
						}
						catch (ComponentRegistrationException ex)
						{
							throw new ArgumentException( ex.Message );
						}

						if (site != null)
						{
							site.Container.Remove( component );
						}

						component.Site = newSite;
						_sites.Add( newSite );
					}
				}
				finally
				{
					_lock.ReleaseWriterLock();
				}
			}
		}

		/// <summary>
		/// Removes a component from the <see cref="IContainer"/>.
		/// </summary>
		/// <param name="component">The <see cref="IComponent"/> to remove</param>
		public virtual void Remove(IComponent component)
		{
			Remove(component, true);
		}

		private void Remove(IComponent component, bool fromKernel)
		{
			if (component != null)
			{
				_lock.AcquireWriterLock( Timeout.Infinite );
				
				try
				{
					IContainerAdapterSite site = component.Site as ContainerAdapterSite;
				
					if (site != null && site.Container == this)
					{
						if (fromKernel)
						{
							if (!Kernel.RemoveComponent( site.EffectiveName ))
							{
								throw new ArgumentException("Unable to remove the requested component");
							}
						}

						component.Site = null;

						_sites.Remove( site );
					}
				}
				finally
				{
					_lock.ReleaseWriterLock();
				}
			}
		}

		protected virtual IWindsorContainer CreateDefaultWindsorContainer()
		{
			return null;
		}

		protected virtual IContainerAdapterSite CreateSite(IComponent component, String name)
		{
			return new ContainerAdapterSite( component, this, name );
		}

		#endregion

		#region IServiceContainer Members

		/// <summary>
		/// Gets the service object of the specified type.
		/// </summary>
		/// <param name="serviceType">The type of service.</param>
		/// <returns>An object inplementing service, or null.</returns>
		public virtual object GetService(Type serviceType)
		{
			object service = null;

			// Check for instrinsic services.
			if (serviceType == typeof(IWindsorContainer) ||
				serviceType == typeof(IServiceContainer) ||
				serviceType == typeof(IContainer))
			{
				service =  this;
			}
			else if (serviceType == typeof(IKernel))
			{
				service = Kernel;
			}
			else
			{
				// Then, check the Windsor Container.
				try
				{
					service = _container[serviceType];
				}
				catch (ComponentNotFoundException)
				{
					// Fall through
				}

				// Otherwise, check the parent service provider.
				if ((service == null) && (_parentProvider != null))
				{
					service = _parentProvider.GetService( serviceType );
				}

				// Finally, check the chained container.
				if ((service  == null) && (_site != null))
				{
					service = _site.GetService( serviceType );
				}
			}

			return service;
		}

		/// <summary>
		/// Adds the specified service to the service container.
		/// </summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="serviceInstance">The instance of the service to add.</param>
		public virtual void AddService(Type serviceType, object serviceInstance)
		{
			AddService( serviceType, serviceInstance, false );
		}

		/// <summary>
		/// Adds the specified service to the service container.
		/// </summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="callback">A callback object that is used to create the service.</param>
		public virtual void AddService(Type serviceType, ServiceCreatorCallback callback)
		{
			AddService( serviceType, callback, false );
		}

		/// <summary>
		/// Adds the specified service to the service container, and optionally
		/// promotes the service to any parent service containers.
		/// </summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="serviceInstance">The instance of the service to add.</param>
		/// <param name="promote">true to promote this request to any parent service containers.</param>
		public virtual void AddService(Type serviceType, object serviceInstance, bool promote)
		{
			if (serviceInstance is ServiceCreatorCallback)
			{
				AddService( serviceType, (ServiceCreatorCallback) serviceInstance, promote);
				return;
			}

			if (promote)
			{
				IServiceContainer parentServices = ParentServices;

				if (parentServices != null)
				{
					parentServices.AddService( serviceType, serviceInstance, promote );
					return;
				}
			}

			if (serviceType == null)
			{
				throw new ArgumentNullException("serviceType");
			}

			if (serviceInstance == null)
			{
				throw new ArgumentNullException("serviceInstance");
			}

			if (!( serviceInstance.GetType().IsCOMObject || 
				serviceType.IsAssignableFrom( serviceInstance.GetType() ) ))
			{
				throw new ArgumentException( String.Format(
					"Invalid service '{0}' for type '{1}'",
					serviceInstance.GetType().FullName, serviceType.FullName ) );
			}

			if (HasService( serviceType ))
			{
				throw new ArgumentException( String.Format(
					"A service for type '{0}' already exists", serviceType.FullName ) );
			}

			String serviceName = GetServiceName( serviceType );
			Kernel.AddComponentInstance( serviceName, serviceType, serviceInstance );
		}

		/// <summary>
		/// Adds the specified service to the service container, and optionally 
		/// promotes the service to parent service containers.
		/// </summary>
		/// <param name="serviceType">The type of service to add.</param>
		/// <param name="callback">A callback object that is used to create the service.</param>
		/// <param name="promote">true to promote this request to any parent service containers.</param>
		public virtual void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
		{
			if (promote)
			{
				IServiceContainer parentServices = ParentServices;

				if (parentServices != null)
				{
					parentServices.AddService( serviceType, callback, promote );
					return;
				}
			}

			if (serviceType == null)
			{
				throw new ArgumentNullException("serviceType");
			}

			if (callback == null)
			{
				throw new ArgumentNullException("callback");
			}

			if (HasService( serviceType ))
			{
				throw new ArgumentException( String.Format(
					"A service for type '{0}' already exists", serviceType.FullName ),
					"serviceType" );
			}

			String serviceName = GetServiceName( serviceType );
			ComponentModel model = new ComponentModel( serviceName, serviceType, null );
			model.ExtendedProperties.Add( ServiceCreatorCallbackActivator.ServiceContainerKey,
				GetService(typeof(IServiceContainer)) );
			model.ExtendedProperties.Add( ServiceCreatorCallbackActivator.ServiceCreatorCallbackKey, callback );
			model.LifestyleType = LifestyleType.Singleton;
			model.CustomComponentActivator = typeof( ServiceCreatorCallbackActivator );
			Kernel.AddCustomComponent( model );
		}

		/// <summary>
		/// Removes the specified service type from the service container.
		/// </summary>
		/// <param name="serviceType">The type of service to remove.</param>
		public virtual void RemoveService(Type serviceType)
		{
			RemoveService(serviceType, false);
		}

		/// <summary>
		/// Removes the specified service type from the service container, 
		/// and optionally promotes the service to parent service containers.
		/// </summary>
		/// <param name="serviceType">The type of service to remove.</param>
		/// <param name="promote">true to promote this request to any parent service containers.</param>
		public virtual void RemoveService(Type serviceType, bool promote)
		{
			if (promote)
			{
				IServiceContainer parentServices = ParentServices;

				if (parentServices != null)
				{
					parentServices.RemoveService( serviceType, promote );
					return;
				}
			}

			if (serviceType == null)
			{
				throw new ArgumentNullException( "serviceType" );
			}

			if (IsIntrinsicService( serviceType ))
			{
				throw new ArgumentException( "Cannot remove an instrinsic service" );
			}

			String serviceName = GetServiceName( serviceType );

			if (!Kernel.RemoveComponent( serviceName ))
			{
				throw new ArgumentException("Unable to remove the requested service");
			}
		}

		/// <summary>
		/// Determins if the service type represents an intrinsic service.
		/// </summary>
		/// <param name="serviceType">The type of service to remove.</param>
		/// <returns>true if the service type is an intrinsic service.</returns>
		private bool IsIntrinsicService(Type serviceType)
		{
			return serviceType == typeof(IWindsorContainer) ||
				serviceType == typeof(IServiceContainer) ||
				serviceType == typeof(IContainer) ||
				serviceType == typeof(IKernel);
		}

		/// <summary>
		/// Determins if the specified service type exists in the service container.
		/// </summary>
		/// <param name="serviceType">The type of service to remove.</param>
		/// <returns>true if the service type exists.</returns>
		private bool HasService(Type serviceType)
		{
			return IsIntrinsicService( serviceType ) ||
				Kernel.HasComponent( serviceType );
		}

		#endregion

		#region IContainerAccessor Members

		/// <summary>
		/// Gets the adapted <see cref="IWindsorContainer"/>
		/// </summary>
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IWindsorContainer Container
		{
			get { return _container; }
		}

		#endregion

		#region IDisposable Members

		/// <summary>
		/// Releases the resources used by the component.
		/// </summary>
		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		/// <summary>
		/// Releases the resources used by the component.
		/// </summary>
		/// <param name="disposing">true if disposing.</param>
		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				_lock.AcquireWriterLock( Timeout.Infinite );

				try
				{
					DisposeContainer();
					DisposeComponent();
					RaiseDisposed();
				}
				finally
				{
					_lock.ReleaseWriterLock();
				}
			}
		}

		private void DisposeComponent()
		{
			if ((_site != null) && (_site.Container != null))
			{
				_site.Container.Remove(this);
			}
		}

		private void DisposeContainer()
		{
			Kernel.ComponentUnregistered -= new ComponentDataDelegate(OnComponentUnregistered);

			for (int i = 0; i < _sites.Count; ++i)
			{
				ISite site = (ISite) _sites[i];
				site.Component.Site = null;
				site.Component.Dispose();
			}
			_sites.Clear();

			InternalDisposeContainer();
		}

		protected virtual void InternalDisposeContainer()
		{
			// Empty
		}

		private void OnComponentUnregistered(String key, IHandler handler)
		{
			IComponent component = handler.Resolve() as IComponent;

			if (component == this)
			{
				Dispose();
			}
			else if (component != null) 
			{
				Remove( component, false );
			}
		}

		private void RaiseDisposed()
		{
			if (Disposed != null) 
			{
				Disposed(this, EventArgs.Empty);
				Disposed = null;
			}
		}

		~ContainerWrapper()
		{
			Dispose(false);
		}

		#endregion

		#region ContainerWrapper Members

		private IKernel Kernel
		{
			get { return _container.Kernel; }
		}
		
		private IServiceContainer ParentServices
		{
			get
			{
				IServiceContainer parentServices = null;

				if (_parentProvider != null)
				{
					parentServices = (IServiceContainer) _parentProvider.GetService( typeof(IServiceContainer) );
				}

				if (_site != null)
				{
					parentServices = (IServiceContainer) _site.GetService( typeof(IServiceContainer) );
				}

				return parentServices;
			}
		}

		private void RegisterAdapterWithKernel()
		{
			String adapterName = String.Format("#ContainerAdapter:{0}#", Guid.NewGuid());
			Kernel.AddComponentInstance(adapterName, this);

			Kernel.ComponentUnregistered += new ComponentDataDelegate(OnComponentUnregistered);
		}

		private String GetServiceName(Type serviceType)
		{
			return String.Format("#ContainerAdapterService:{0}#", serviceType.FullName);
		}

		#endregion
	}
}
