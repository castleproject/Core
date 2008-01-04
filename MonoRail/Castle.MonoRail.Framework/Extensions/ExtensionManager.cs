// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="context"></param>
	public delegate void ExtensionHandler(IEngineContext context);

	/// <summary>
	/// MonoRail's extension manager. 
	/// It fires events related to MonoRail that can be used to add additional behaviour.
	/// </summary>
	public class ExtensionManager : MarshalByRefObject
	{
		private static readonly object ActionExceptionEvent = new object();
		private static readonly object UnhandledExceptionEvent = new object();
		private static readonly object AcquireSessionStateEvent = new object();
		private static readonly object ReleaseSessionStateEvent = new object();
		private static readonly object PreProcessControllerEvent = new object();
		private static readonly object PostProcessControllerEvent = new object();

		private readonly EventHandlerList events;
		private readonly IMonoRailServices serviceProvider;
		private readonly List<IMonoRailExtension> extensions = new List<IMonoRailExtension>();

		/// <summary>
		/// Initializes a new instance of the <see cref="ExtensionManager"/> class.
		/// </summary>
		/// <param name="serviceContainer">The service container.</param>
		public ExtensionManager(IMonoRailServices serviceContainer)
		{
			events = new EventHandlerList();
			serviceProvider = serviceContainer;
		}

		/// <summary>
		/// Gets the service container.
		/// </summary>
		/// <value>The service container.</value>
		public IMonoRailServices ServiceProvider
		{
			get { return serviceProvider; }
		}

		/// <summary>
		/// Gets the extensions.
		/// </summary>
		/// <value>The extensions.</value>
		public List<IMonoRailExtension> Extensions
		{
			get { return extensions; }
		}

		/// <summary>
		/// Occurs when an action throws an exception.
		/// </summary>
		public event ExtensionHandler ActionException
		{
			add { events.AddHandler(ActionExceptionEvent, value); }
			remove { events.RemoveHandler(ActionExceptionEvent, value); }
		}

		/// <summary>
		/// Occurs when an unhandled exception is thrown.
		/// </summary>
		public event ExtensionHandler UnhandledException
		{
			add { events.AddHandler(UnhandledExceptionEvent, value); }
			remove { events.RemoveHandler(UnhandledExceptionEvent, value); }
		}

		/// <summary>
		/// Occurs when a session is adquired.
		/// </summary>
		public event ExtensionHandler AcquireSessionState
		{
			add { events.AddHandler(AcquireSessionStateEvent, value); }
			remove { events.RemoveHandler(AcquireSessionStateEvent, value); }
		}

		/// <summary>
		/// Occurs when a session is released.
		/// </summary>
		public event ExtensionHandler ReleaseSessionState
		{
			add { events.AddHandler(ReleaseSessionStateEvent, value); }
			remove { events.RemoveHandler(ReleaseSessionStateEvent, value); }
		}

		/// <summary>
		/// Occurs before processing controller.
		/// </summary>
		public event ExtensionHandler PreControllerProcess
		{
			add { events.AddHandler(PreProcessControllerEvent, value); }
			remove { events.RemoveHandler(PreProcessControllerEvent, value); }
		}

		/// <summary>
		/// Occurs after processing controller.
		/// </summary>
		public event ExtensionHandler PostControllerProcess
		{
			add { events.AddHandler(PostProcessControllerEvent, value); }
			remove { events.RemoveHandler(PostProcessControllerEvent, value); }
		}

		internal void RaiseReleaseRequestState(IEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[ReleaseSessionStateEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaiseAcquireRequestState(IEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[AcquireSessionStateEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaiseUnhandledError(IEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[UnhandledExceptionEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaiseActionError(IEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[ActionExceptionEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaisePostProcessController(IEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[PostProcessControllerEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaisePreProcessController(IEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[PreProcessControllerEvent];
			if (eventDelegate != null) eventDelegate(context);
		}
	}
}