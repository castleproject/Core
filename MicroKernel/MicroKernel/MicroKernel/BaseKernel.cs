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

	using Apache.Avalon.Framework;
	using Castle.MicroKernel.Graph;
	using Castle.MicroKernel.Model;
	using Castle.MicroKernel.Handler.Default;
	using Castle.MicroKernel.Lifestyle.Default;
	using Castle.MicroKernel.Model.Default;

	/// <summary>
	/// Base implementation of <see cref="IKernel"/>
	/// </summary>
	public class BaseKernel : AbstractKernelEvents, IKernel
	{
		protected IList m_componentsInstances = new ArrayList();

		protected Hashtable m_key2Handler;

		protected Hashtable m_subsystems;

		protected Hashtable m_facilities;

		protected IHandlerFactory m_handlerFactory;

		protected IComponentModelBuilder m_componentModelBuilder;

		protected ILifestyleManagerFactory m_lifestyleManagerFactory;

		/// <summary>
		/// 
		/// </summary>
		public BaseKernel() : base()
		{
			m_key2Handler = new Hashtable(CaseInsensitiveHashCodeProvider.Default, CaseInsensitiveComparer.Default);
			m_subsystems = new Hashtable();
			m_facilities = new Hashtable();
			HandlerFactory = new BaseHandlerFactory();
			ModelBuilder = new DefaultComponentModelBuilder(this);
			LifestyleManagerFactory = new SimpleLifestyleManagerFactory();

			InitializeSubsystems();
		}

		#region IKernel Members

		/// <summary>
		/// Adds a component to kernel.
		/// </summary>
		/// <param name="key">The unique key that identifies the component</param>
		/// <param name="service">The service exposed by this component</param>
		/// <param name="implementation">The actual implementation</param>
		public void AddComponent(String key, Type service, Type implementation)
		{
			AssertUtil.ArgumentNotNull(key, "key");
			AssertUtil.ArgumentNotNull(service, "service");
			AssertUtil.ArgumentNotNull(implementation, "implementation");
			AssertUtil.ArgumentMustBeInterface(service, "service");
			AssertUtil.ArgumentMustNotBeInterface(implementation, "implementation");
			AssertUtil.ArgumentMustNotBeAbstract(implementation, "implementation");

			if (!service.IsAssignableFrom(implementation))
			{
				throw new ArgumentException("The specified implementation does not implement the service interface");
			}

			IComponentModel model = ModelBuilder.BuildModel(key, service, implementation);
			OnModelConstructed(model, key);

			IHandler handler = HandlerFactory.CreateHandler(model);
			handler.Init(this);

			m_key2Handler[ key ] = handler;
			OnComponentRegistered(model, key, handler);
		}

		/// <summary>
		/// Pending.
		/// </summary>
		/// <param name="key"></param>
		public void RemoveComponent(String key)
		{
			AssertUtil.ArgumentNotNull(key, "key");

			IHandler handler = this[key];

			if (handler != null)
			{
				OnComponentUnregistered(handler.ComponentModel, key, handler);

				m_key2Handler.Remove(key);

				HandlerFactory.ReleaseHandler(handler);
			}
		}

		/// <summary>
		/// Adds a subsystem.
		/// </summary>
		/// <param name="key">Name of this subsystem</param>
		/// <param name="system">Subsystem implementation</param>
		public void AddSubsystem(String key, IKernelSubsystem system)
		{
			AssertUtil.ArgumentNotNull(key, "key");
			AssertUtil.ArgumentNotNull(system, "system");

			system.Init(this);

			m_subsystems[ key ] = system;
		}

		/// <summary>
		/// Adds a <see cref="IKernelFacility"/> implementation to 
		/// the kernel.
		/// </summary>
		/// <param name="key">Facility id</param>
		/// <param name="facility">Facility instance</param>
		public virtual void AddFacility(String key, IKernelFacility facility)
		{
			AssertUtil.ArgumentNotNull(key, "key");
			AssertUtil.ArgumentNotNull(facility, "facility");

			facility.Init(this);

			m_facilities[ key ] = facility;
		}

		/// <summary>
		/// Removes a <see cref="IKernelFacility"/> from the kernel.
		/// </summary>
		/// <param name="key">Facility id</param>
		public virtual void RemoveFacility(String key)
		{
			AssertUtil.ArgumentNotNull(key, "key");

			IKernelFacility facility = m_facilities[ key ] as IKernelFacility;

			RemoveFacility( facility );

			m_facilities.Remove(key);
		}

		/// <summary>
		/// 
		/// </summary>
		public IHandler this[String key]
		{
			get { return (IHandler) m_key2Handler[ key ]; }
		}

		public IHandler GetHandler(String key, object criteria)
		{
			// TODO: Delegates invocation to LookupSubsystem
			return null;
		}

		public IHandlerFactory HandlerFactory
		{
			get { return m_handlerFactory; }
			set
			{
				AssertUtil.ArgumentNotNull(value, "value");
				m_handlerFactory = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public ILifestyleManagerFactory LifestyleManagerFactory
		{
			get { return m_lifestyleManagerFactory; }
			set
			{
				AssertUtil.ArgumentNotNull(value, "value");
				m_lifestyleManagerFactory = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public IComponentModelBuilder ModelBuilder
		{
			get { return m_componentModelBuilder; }
			set
			{
				AssertUtil.ArgumentNotNull(value, "value");
				m_componentModelBuilder = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		public bool HasService(Type service)
		{
			return m_service2Key.Contains(service);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="service"></param>
		/// <param name="depDelegate"></param>
		public void AddDependencyListener(Type service, DependencyListenerDelegate depDelegate)
		{
			lock (m_dependencyToSatisfy)
			{
				Delegate del = m_dependencyToSatisfy[ service ] as Delegate;

				if (del == null)
				{
					m_dependencyToSatisfy[ service ] = depDelegate;
				}
				else
				{
					del = Delegate.Combine(del, depDelegate);
					m_dependencyToSatisfy[ service ] = del;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="service"></param>
		/// <returns></returns>
		public IHandler GetHandlerForService(Type service)
		{
			String key = (String) m_service2Key[ service ];
			return key == null ? null : (IHandler) m_key2Handler[ key ];
		}

		/// <summary>
		/// Returns a registered subsystem;
		/// </summary>
		/// <param name="key">Key used when registered subsystem</param>
		/// <returns>Subsystem implementation</returns>
		public IKernelSubsystem GetSubsystem(String key)
		{
			return (IKernelSubsystem) m_subsystems[ key ];
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			DisposeStartedInstances();
			DisposeHandlers();
			DisposeFacilities();
		}

		#endregion

		protected virtual void DisposeStartedInstances()
		{
			foreach(PairHandlerComponent pair in m_componentsInstances)
			{
				pair.Handler.Release(pair.Instance);
			}
			m_componentsInstances.Clear();
		}

		protected virtual void DisposeHandlers()
		{
			SimpleGraph graph = BuildHandlerGraph();
			Vertex[] vertices = TopologicalSort.Perform( graph );
			
			foreach( Vertex vertex in vertices )
			{
				IHandler handler = GetHandlerForService( vertex.Content as Type );
				HandlerFactory.ReleaseHandler( handler );
			}
		}

		protected virtual void DisposeFacilities()
		{
			foreach( IKernelFacility facility in m_facilities.Values )
			{
				RemoveFacility( facility );
			}
			m_facilities.Clear();
		}

		protected virtual void RemoveFacility( IKernelFacility facility )
		{
			if (facility != null)
			{
				facility.Terminate(this);
			}
		}

		protected virtual SimpleGraph BuildHandlerGraph()
		{
			SimpleGraph graph = new SimpleGraph();

			foreach( IHandler handler in m_key2Handler.Values )
			{
				graph.CreateVertex( handler.ComponentModel.Service );
			}

			foreach( IHandler handler in m_key2Handler.Values )
			{
				Type service = handler.ComponentModel.Service;
				IDependencyModel[] dependencies = handler.ComponentModel.Dependencies;

				foreach(IDependencyModel dependency in dependencies)
				{
					graph.CreateEdge( graph[service], graph[dependency.Service] );
				}
			}

			return graph;
		}

		/// <summary>
		/// 
		/// </summary>
		protected virtual void InitializeSubsystems()
		{
			// Examples:
			// AddSubsystem( KernelConstants.LOOKUP, new LookupCriteriaMatcher() );
			// AddSubsystem( KernelConstants.EVENTS, new EventManager() );
		}

		/// <summary>
		/// Starts the component if the activation policy for 
		/// the component is 'Start' and if the component's dependencies are satisfied.
		/// </summary>
		/// <param name="model">Component model</param>
		/// <param name="handler">Handler responsible for the component</param>
		protected virtual void StartComponentIfPossible(IComponentModel model, IHandler handler)
		{
			if (model.ActivationPolicy == Activation.Start)
			{
				if (handler.ActualState == State.Valid)
				{
					StartComponent(handler);
				}
				else if (handler.ActualState == State.WaitingDependency)
				{
					handler.AddChangeStateListener(new ChangeStateListenerDelegate(StartComponent));
				}
			}
		}

		protected virtual void StartComponent(IHandler handler)
		{
			object instance = handler.Resolve();
			m_componentsInstances.Add(new PairHandlerComponent(handler, instance));
		}

		private void OnModelConstructed(IComponentModel model, String key)
		{
			RaiseModelConstructed(model, key);
		}

		private void OnComponentRegistered(IComponentModel model, String key, IHandler handler)
		{
			m_service2Key[ model.Service ] = key;

			RaiseDependencyEvent(model.Service, handler);

			RaiseComponentRegistered(model, key, handler);

			StartComponentIfPossible(model, handler);
		}

		private void OnComponentUnregistered(IComponentModel model, String key, IHandler handler)
		{
			m_service2Key.Remove(model.Service);

			RaiseComponentUnregistered(model, key, handler);
		}
	}

	/// <summary>
	/// 
	/// </summary>
	internal class PairHandlerComponent
	{
		private IHandler m_handler;
		private object m_instance;

		public PairHandlerComponent(IHandler handler, object instance)
		{
			m_handler = handler;
			m_instance = instance;
		}

		public IHandler Handler
		{
			get { return m_handler; }
		}

		public object Instance
		{
			get { return m_instance; }
		}
	}
}