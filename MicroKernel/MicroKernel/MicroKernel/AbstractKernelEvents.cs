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
	using System.ComponentModel;

	using Castle.MicroKernel.Model;
	using Castle.MicroKernel.Interceptor;
	using Castle.MicroKernel.Interceptor.Default;

	/// <summary>
	/// Implements the basic <see cref="IKernelEvents"/> functionality.
	/// </summary>
	public abstract class AbstractKernelEvents : IKernelEvents
	{
		private static readonly object ComponentRegisteredEvent = new object();
		private static readonly object ComponentUnregisteredEvent = new object();
		private static readonly object ComponentWrapEvent = new object();
		private static readonly object ComponentUnWrapEvent = new object();
		private static readonly object ComponentReadyEvent = new object();
		private static readonly object ComponentReleasedEvent = new object();
		private static readonly object ComponentModelConstructedEvent = new object();

		private EventHandlerList m_events;

		protected Hashtable m_proxy2ComponentWrapper;

		protected Hashtable m_service2Key;

		protected Hashtable m_dependencyToSatisfy;

		protected IInterceptedComponentBuilder m_interceptedComponentBuilder;

		public AbstractKernelEvents()
		{
			m_events = new EventHandlerList();
			m_service2Key = new Hashtable();
			m_dependencyToSatisfy = new Hashtable();
			m_proxy2ComponentWrapper = new Hashtable();
			InterceptedComponentBuilder = new DefaultInterceptedComponentBuilder();
		}

		#region IKernelEvents

		/// <summary>
		/// Pending
		/// </summary>
		/// <value></value>
		public event ComponentDataDelegate ComponentRegistered
		{
			add { m_events.AddHandler(ComponentRegisteredEvent, value); }
			remove { m_events.RemoveHandler(ComponentRegisteredEvent, value); }
		}

		/// <summary>
		/// Pending
		/// </summary>
		/// <value></value>
		public event ComponentDataDelegate ComponentUnregistered
		{
			add { m_events.AddHandler(ComponentUnregisteredEvent, value); }
			remove { m_events.RemoveHandler(ComponentUnregisteredEvent, value); }
		}

		/// <summary>
		/// Pending
		/// </summary>
		/// <value></value>
		public event WrapDelegate ComponentWrap
		{
			add { m_events.AddHandler(ComponentWrapEvent, value); }
			remove { m_events.RemoveHandler(ComponentWrapEvent, value); }
		}

		/// <summary>
		/// Pending
		/// </summary>
		/// <value></value>
		public event WrapDelegate ComponentUnWrap
		{
			add { m_events.AddHandler(ComponentUnWrapEvent, value); }
			remove { m_events.RemoveHandler(ComponentUnWrapEvent, value); }
		}

		/// <summary>
		/// Pending
		/// </summary>
		/// <value></value>
		public event ComponentInstanceDelegate ComponentReady
		{
			add { m_events.AddHandler(ComponentReadyEvent, value); }
			remove { m_events.RemoveHandler(ComponentReadyEvent, value); }
		}

		/// <summary>
		/// Pending
		/// </summary>
		/// <value></value>
		public event ComponentInstanceDelegate ComponentReleased
		{
			add { m_events.AddHandler(ComponentReleasedEvent, value); }
			remove { m_events.RemoveHandler(ComponentReleasedEvent, value); }
		}

		/// <summary>
		/// Pending
		/// </summary>
		/// <value></value>
		public event ComponentModelDelegate ComponentModelConstructed
		{
			add { m_events.AddHandler(ComponentModelConstructedEvent, value); }
			remove { m_events.RemoveHandler(ComponentModelConstructedEvent, value); }
		}

		public virtual void RaiseComponentReadyEvent(IHandler handler, object instance)
		{
			ComponentInstanceDelegate eventDelegate = (ComponentInstanceDelegate) m_events[ComponentReadyEvent];

			if (eventDelegate != null)
			{
				IComponentModel model = handler.ComponentModel;
				String key = (String) m_service2Key[model.Service];

				eventDelegate(model, key, handler, instance);
			}
		}

		public virtual void RaiseComponentReleasedEvent(IHandler handler, object instance)
		{
			ComponentInstanceDelegate eventDelegate = (ComponentInstanceDelegate) m_events[ComponentReleasedEvent];

			if (eventDelegate != null)
			{
				IComponentModel model = handler.ComponentModel;
				String key = (String) m_service2Key[model.Service];

				eventDelegate(model, key, handler, instance);
			}
		}

		public virtual object RaiseWrapEvent(IHandler handler, object instance)
		{
			WrapDelegate eventDelegate = (WrapDelegate) m_events[ComponentWrapEvent];

			if (eventDelegate != null)
			{
				IComponentModel model = handler.ComponentModel;
				String key = (String) m_service2Key[model.Service];
				InterceptedComponentWrapper wrapper =
					new InterceptedComponentWrapper(m_interceptedComponentBuilder, instance, model.Service);

				eventDelegate(model, key, handler, wrapper);

				if (wrapper.IsProxiedCreated)
				{
					object proxy = wrapper.ProxiedInstance;
					m_proxy2ComponentWrapper[proxy] = wrapper;

					// From now on, the outside world will have 
					// a proxy pointer, not the instance anymore.
					instance = proxy;
				}
			}

			return instance;
		}

		public virtual object RaiseUnWrapEvent(IHandler handler, object instance)
		{
			WrapDelegate eventDelegate = (WrapDelegate) m_events[ComponentUnWrapEvent];

			// We can have a null wrapper here
			InterceptedComponentWrapper wrapper = m_proxy2ComponentWrapper[instance] as InterceptedComponentWrapper;

			if (wrapper != null)
			{
				m_proxy2ComponentWrapper.Remove(instance);
			}

			if (eventDelegate != null)
			{
				IComponentModel model = handler.ComponentModel;
				String key = (String) m_service2Key[model.Service];

				eventDelegate(model, key, handler, wrapper);
			}

			return wrapper != null ? wrapper.Instance : instance;
		}

		#endregion

		protected virtual void RaiseDependencyEvent(Type service, IHandler handler)
		{
			DependencyListenerDelegate del = (DependencyListenerDelegate) m_dependencyToSatisfy[service];

			if (del != null)
			{
				del(service, handler);
			}
		}

		protected virtual void RaiseComponentRegistered(IComponentModel model, String key, IHandler handler)
		{
			ComponentDataDelegate eventDelegate = (ComponentDataDelegate) m_events[ComponentRegisteredEvent];

			if (eventDelegate != null)
			{
				eventDelegate(model, key, handler);
			}
		}

		protected virtual void RaiseComponentUnregistered(IComponentModel model, String key, IHandler handler)
		{
			ComponentDataDelegate eventDelegate = (ComponentDataDelegate) m_events[ComponentUnregisteredEvent];

			if (eventDelegate != null)
			{
				eventDelegate(model, key, handler);
			}
		}

		protected virtual void RaiseModelConstructed(IComponentModel model, String key)
		{
			ComponentModelDelegate eventDelegate = (ComponentModelDelegate) m_events[ComponentModelConstructedEvent];

			if (eventDelegate != null)
			{
				eventDelegate(model, key);
			}
		}

		public virtual IInterceptedComponentBuilder InterceptedComponentBuilder
		{
			get { return m_interceptedComponentBuilder; }
			set
			{
				AssertUtil.ArgumentNotNull(value, "value");
				m_interceptedComponentBuilder = value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public class InterceptedComponentWrapper : IInterceptedComponent
		{
			private IInterceptedComponentBuilder m_interceptedComponentBuilder;
			private IInterceptedComponent m_delegate;
			private object m_instance;
			private Type m_service;

			public InterceptedComponentWrapper(IInterceptedComponentBuilder interceptedComponentBuilder,
			                                   object instance, Type service)
			{
				m_interceptedComponentBuilder = interceptedComponentBuilder;
				m_instance = instance;
				m_service = service;
			}

			public object Instance
			{
				get { return m_instance; }
			}

			public object ProxiedInstance
			{
				get
				{
					EnsureDelegate();
					return m_delegate.ProxiedInstance;
				}
			}

			public void Add(IInterceptor interceptor)
			{
				EnsureDelegate();
				m_delegate.Add(interceptor);
			}

			public IInterceptor InterceptorChain
			{
				get
				{
					EnsureDelegate();
					return m_delegate.InterceptorChain;
				}
			}

			public bool IsProxiedCreated
			{
				get { return m_delegate != null; }
			}

			private void EnsureDelegate()
			{
				if (m_delegate == null)
				{
					m_delegate = m_interceptedComponentBuilder.CreateInterceptedComponent(
						m_instance, m_service);
				}
			}
		}
	}
}