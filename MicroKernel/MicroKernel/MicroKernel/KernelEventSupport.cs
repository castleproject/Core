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
	using System.ComponentModel;

	using Castle.Model;

	/// <summary>
	/// Summary description for KernelEventSupport.
	/// </summary>
	public abstract class KernelEventSupport : IKernelEvents
	{
		private static readonly object ComponentRegisteredEvent = new object();
		private static readonly object ComponentUnregisteredEvent = new object();
		private static readonly object AddedAsChildKernelEvent = new object();

		private EventHandlerList _events;

		public KernelEventSupport()
		{
			_events = new EventHandlerList();
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
		public event EventHandler AddedAsChildKernel
		{
			add { _events.AddHandler(AddedAsChildKernelEvent, value); }
			remove { _events.RemoveHandler(AddedAsChildKernelEvent, value); }
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

		protected virtual void RaiseAddedAsChildKernel()
		{
			EventHandler eventDelegate = (EventHandler) _events[AddedAsChildKernelEvent];
			if (eventDelegate != null) eventDelegate(this, EventArgs.Empty);
		}
	}
}
