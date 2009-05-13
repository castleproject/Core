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

namespace Castle.MicroKernel
{
	using System;
	using System.ComponentModel;
	using System.Runtime.Serialization;
	using Core;

	/// <summary>
	/// Default implementation of <see cref="IKernel"/>. 
	/// This implementation is complete and also support a kernel 
	/// hierarchy (sub containers).
	/// </summary>
#if !SILVERLIGHT
	public partial class DefaultKernel : MarshalByRefObject, IKernel, IKernelEvents, IDeserializationCallback
#else
	public partial class DefaultKernel : IKernel, IKernelEvents
#endif
	{
		private static readonly object HandlerRegisteredEvent = new object();
		private static readonly object ComponentRegisteredEvent = new object();
		private static readonly object ComponentUnregisteredEvent = new object();
		private static readonly object ComponentCreatedEvent = new object();
		private static readonly object ComponentDestroyedEvent = new object();
		private static readonly object AddedAsChildKernelEvent = new object();
		private static readonly object ComponentModelCreatedEvent = new object();
		private static readonly object DependencyResolvingEvent = new object();
		private static readonly object RemovedAsChildKernelEvent = new object();

		[NonSerialized]
		private readonly EventHandlerList events = new EventHandlerList();

#if !SILVERLIGHT
		public override object InitializeLifetimeService()
		{
			return null;
		}
#endif

		/// <summary>
		/// Pending
		/// </summary>
		public event HandlerDelegate HandlerRegistered
		{
			add { events.AddHandler(HandlerRegisteredEvent, value); }
			remove { events.RemoveHandler(HandlerRegisteredEvent, value); }
		}

		/// <summary>
		/// Pending
		/// </summary>
		/// <value></value>
		public event ComponentDataDelegate ComponentRegistered
		{
			add { events.AddHandler(ComponentRegisteredEvent, value); }
			remove { events.RemoveHandler(ComponentRegisteredEvent, value); }
		}

		/// <summary>
		/// Pending
		/// </summary>
		/// <value></value>
		public event ComponentDataDelegate ComponentUnregistered
		{
			add { events.AddHandler(ComponentUnregisteredEvent, value); }
			remove { events.RemoveHandler(ComponentUnregisteredEvent, value); }
		}

		/// <summary>
		/// Pending
		/// </summary>
		/// <value></value>
		public event ComponentInstanceDelegate ComponentCreated
		{
			add { events.AddHandler(ComponentCreatedEvent, value); }
			remove { events.RemoveHandler(ComponentCreatedEvent, value); }
		}

		/// <summary>
		/// Pending
		/// </summary>
		/// <value></value>
		public event ComponentInstanceDelegate ComponentDestroyed
		{
			add { events.AddHandler(ComponentDestroyedEvent, value); }
			remove { events.RemoveHandler(ComponentDestroyedEvent, value); }
		}

		/// <summary>
		/// Pending
		/// </summary>
		/// <value></value>
		public event EventHandler AddedAsChildKernel
		{
			add { events.AddHandler(AddedAsChildKernelEvent, value); }
			remove { events.RemoveHandler(AddedAsChildKernelEvent, value); }
		}

		/// <summary>
		/// Pending
		/// </summary>
		public event EventHandler RemovedAsChildKernel
		{
			add { events.AddHandler(RemovedAsChildKernelEvent, value); }
			remove { events.RemoveHandler(RemovedAsChildKernelEvent, value); }
		}

		/// <summary>
		/// Pending
		/// </summary>
		/// <value></value>
		public event ComponentModelDelegate ComponentModelCreated
		{
			add { events.AddHandler(ComponentModelCreatedEvent, value); }
			remove { events.RemoveHandler(ComponentModelCreatedEvent, value); }
		}

		public event DependencyDelegate DependencyResolving
		{
			add { events.AddHandler(DependencyResolvingEvent, value); }
			remove { events.RemoveHandler(DependencyResolvingEvent, value); }
		}

		protected virtual void RaiseComponentRegistered(String key, IHandler handler)
		{
			ComponentDataDelegate eventDelegate = (ComponentDataDelegate) events[ComponentRegisteredEvent];
			if (eventDelegate != null) eventDelegate(key, handler);
		}


		protected virtual void RaiseComponentUnregistered(String key, IHandler handler)
		{
			ComponentDataDelegate eventDelegate = (ComponentDataDelegate) events[ComponentUnregisteredEvent];
			if (eventDelegate != null) eventDelegate(key, handler);
		}

		public virtual void RaiseComponentCreated(ComponentModel model, object instance)
		{
			ComponentInstanceDelegate eventDelegate = (ComponentInstanceDelegate) events[ComponentCreatedEvent];
			if (eventDelegate != null) eventDelegate(model, instance);
		}

		public virtual void RaiseComponentDestroyed(ComponentModel model, object instance)
		{
			ComponentInstanceDelegate eventDelegate = (ComponentInstanceDelegate) events[ComponentDestroyedEvent];
			if (eventDelegate != null) eventDelegate(model, instance);
		}

		protected virtual void RaiseAddedAsChildKernel()
		{
			EventHandler eventDelegate = (EventHandler) events[AddedAsChildKernelEvent];
			if (eventDelegate != null) eventDelegate(this, EventArgs.Empty);
		}

		protected virtual void RaiseRemovedAsChildKernel()
		{
			EventHandler eventDelegate = (EventHandler) events[RemovedAsChildKernelEvent];
			if (eventDelegate != null) eventDelegate(this, EventArgs.Empty);
		}

		protected virtual void RaiseComponentModelCreated(ComponentModel model)
		{
			ComponentModelDelegate eventDelegate = (ComponentModelDelegate) events[ComponentModelCreatedEvent];
			if (eventDelegate != null) eventDelegate(model);
		}

		public virtual void RaiseHandlerRegistered(IHandler handler)
		{
			bool stateChanged = true;

			while(stateChanged)
			{
				stateChanged = false;
				HandlerDelegate eventDelegate = (HandlerDelegate) events[HandlerRegisteredEvent];
				if (eventDelegate != null) eventDelegate(handler, ref stateChanged);
			}
		}
		protected virtual void RaiseDependencyResolving(ComponentModel client, DependencyModel model, Object dependency)
		{
			DependencyDelegate eventDelegate = (DependencyDelegate) events[DependencyResolvingEvent];
			if (eventDelegate != null) eventDelegate(client, model, dependency);
		}

		#region IDeserializationCallback Members

		#endregion
	}
}
