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
	using System.ComponentModel;

	using Castle.Model;

	/// <summary>
	/// Summary description for KernelEventSupport.
	/// </summary>
	public abstract class KernelEventSupport : IKernelEvents
	{
		private static readonly object HandlerRegisteredEvent = new object();
		private static readonly object ComponentRegisteredEvent = new object();
		private static readonly object ComponentUnregisteredEvent = new object();
		private static readonly object ComponentCreatedEvent = new object();
		private static readonly object ComponentDestroyedEvent = new object();
		private static readonly object AddedAsChildKernelEvent = new object();
		private static readonly object ComponentModelCreatedEvent = new object();

		private EventHandlerList _events;

		public KernelEventSupport()
		{
			_events = new EventHandlerList();
		}

		/// <summary>
		/// Pending
		/// </summary>
		public event HandlerDelegate HandlerRegistered
		{
			add { _events.AddHandler(HandlerRegisteredEvent, value); }
			remove { _events.RemoveHandler(HandlerRegisteredEvent, value); }
		}

		/// <summary>
		/// Pending
		/// </summary>
		/// <value></value>
		public event ComponentDataDelegate ComponentRegistered
		{
			add { _events.AddHandler(ComponentRegisteredEvent, value); }
			remove { _events.RemoveHandler(ComponentRegisteredEvent, value); }
		}

		/// <summary>
		/// Pending
		/// </summary>
		/// <value></value>
		public event ComponentDataDelegate ComponentUnregistered
		{
			add { _events.AddHandler(ComponentUnregisteredEvent, value); }
			remove { _events.RemoveHandler(ComponentUnregisteredEvent, value); }
		}


		/// <summary>
		/// Pending
		/// </summary>
		/// <value></value>
		public event ComponentInstanceDelegate ComponentCreated
		{
			add { _events.AddHandler(ComponentCreatedEvent, value); }
			remove { _events.RemoveHandler(ComponentCreatedEvent, value); }
		}

		/// <summary>
		/// Pending
		/// </summary>
		/// <value></value>
		public event ComponentInstanceDelegate ComponentDestroyed
		{
			add { _events.AddHandler(ComponentDestroyedEvent, value); }
			remove { _events.RemoveHandler(ComponentDestroyedEvent, value); }
		}

		/// <summary>
		/// Pending
		/// </summary>
		/// <value></value>
		public event EventHandler AddedAsChildKernel
		{
			add { _events.AddHandler(AddedAsChildKernelEvent, value); }
			remove { _events.RemoveHandler(AddedAsChildKernelEvent, value); }
		}

		/// <summary>
		/// Pending
		/// </summary>
		/// <value></value>
		public event ComponentModelDelegate ComponentModelCreated
		{
			add { _events.AddHandler(ComponentModelCreatedEvent, value); }
			remove { _events.RemoveHandler(ComponentModelCreatedEvent, value); }
		}

		protected virtual void RaiseComponentRegistered(String key, IHandler handler)
		{
			ComponentDataDelegate eventDelegate = (ComponentDataDelegate) _events[ComponentRegisteredEvent];
			if (eventDelegate != null) eventDelegate(key, handler);
		}


		protected virtual void RaiseComponentUnregistered(String key, IHandler handler)
		{
			ComponentDataDelegate eventDelegate = (ComponentDataDelegate) _events[ComponentUnregisteredEvent];
			if (eventDelegate != null) eventDelegate(key, handler);
		}

		protected virtual void RaiseComponentCreated(ComponentModel model, object instance)
		{
			ComponentInstanceDelegate eventDelegate = (ComponentInstanceDelegate) _events[ComponentCreatedEvent];
			if (eventDelegate != null) eventDelegate(model, instance);
		}

		protected virtual void RaiseComponentDestroyed(ComponentModel model, object instance)
		{
			ComponentInstanceDelegate eventDelegate = (ComponentInstanceDelegate) _events[ComponentDestroyedEvent];
			if (eventDelegate != null) eventDelegate(model, instance);
		}

		protected virtual void RaiseAddedAsChildKernel()
		{
			EventHandler eventDelegate = (EventHandler) _events[AddedAsChildKernelEvent];
			if (eventDelegate != null) eventDelegate(this, EventArgs.Empty);
		}

		protected virtual void RaiseComponentModelCreated(ComponentModel model)
		{
			ComponentModelDelegate eventDelegate = (ComponentModelDelegate) _events[ComponentModelCreatedEvent];
			if (eventDelegate != null) eventDelegate(model);
		}

		protected virtual void RaiseHandlerRegistered(IHandler handler)
		{
			HandlerDelegate eventDelegate = (HandlerDelegate) _events[HandlerRegisteredEvent];
			if (eventDelegate != null) eventDelegate(handler);
		}
	}
}
