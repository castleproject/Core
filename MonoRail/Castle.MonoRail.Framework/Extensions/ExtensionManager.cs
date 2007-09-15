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
	using System.ComponentModel;
	using System.ComponentModel.Design;

	/// <summary>
	/// 
	/// </summary>
	/// <param name="context"></param>
	public delegate void ExtensionHandler(IRailsEngineContext context);

	/// <summary>
	/// MonoRail's extension manager. 
	/// It fires events related to MonoRail that can be used to add additional behaviour.
	/// </summary>
	public class ExtensionManager : MarshalByRefObject
	{
		private static readonly object ContextCreatedEvent = new object();
		private static readonly object ContextDisposedEvent = new object();
		private static readonly object ActionExceptionEvent = new object();
		private static readonly object UnhandledExceptionEvent = new object();
		private static readonly object AcquireSessionStateEvent = new object();
		private static readonly object ReleaseSessionStateEvent = new object();
		private static readonly object PreProcessEvent = new object();
		private static readonly object PostProcessEvent = new object();
		private static readonly object AuthorizeRequestEvent = new object();
		private static readonly object AuthenticateRequestEvent = new object();
		private static readonly object ResolveRequestCacheEvent = new object();
		private static readonly object UpdateRequestCacheEvent = new object();

		private EventHandlerList events;

		private IServiceContainer serviceContainer;

		/// <summary>
		/// Initializes a new instance of the <see cref="ExtensionManager"/> class.
		/// </summary>
		/// <param name="serviceContainer">The service container.</param>
		public ExtensionManager(IServiceContainer serviceContainer)
		{
			events = new EventHandlerList();
			this.serviceContainer = serviceContainer;
		}

		/// <summary>
		/// Gets the service container.
		/// </summary>
		/// <value>The service container.</value>
		public IServiceContainer ServiceContainer
		{
			get { return serviceContainer; }
		}

		/// <summary>
		/// Occurs when a context is created.
		/// </summary>
		public event ExtensionHandler ContextCreated
		{
			add { events.AddHandler(ContextCreatedEvent, value); }
			remove { events.RemoveHandler(ContextCreatedEvent, value); }
		}

		/// <summary>
		/// Occurs when a context is disposed.
		/// </summary>
		public event ExtensionHandler ContextDisposed		
		{
			add { events.AddHandler(ContextDisposedEvent, value); }
			remove { events.RemoveHandler(ContextDisposedEvent, value); }
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
		/// Occurs before pre process a handler.
		/// </summary>
		public event ExtensionHandler PreProcess
		{
			add { events.AddHandler(PreProcessEvent, value); }
			remove { events.RemoveHandler(PreProcessEvent, value); }
		}

		/// <summary>
		/// Occurs after process a handler.
		/// </summary>
		public event ExtensionHandler PostProcess
		{
			add { events.AddHandler(PostProcessEvent, value); }
			remove { events.RemoveHandler(PostProcessEvent, value); }
		}

		/// <summary>
		/// Occurs when a request needs to authenticate.
		/// </summary>
		public event ExtensionHandler AuthenticateRequest
		{
			add { events.AddHandler(AuthenticateRequestEvent, value); }
			remove { events.RemoveHandler(AuthenticateRequestEvent, value); }
		}

		/// <summary>
		/// Occurs when a request needs to be authorized.
		/// </summary>
		public event ExtensionHandler AuthorizeRequest
		{
			add { events.AddHandler(AuthorizeRequestEvent, value); }
			remove { events.RemoveHandler(AuthorizeRequestEvent, value); }
		}

		/// <summary>
		/// Occurs upon request cache resolval.
		/// </summary>
		public event ExtensionHandler ResolveRequestCache
		{
			add { events.AddHandler(ResolveRequestCacheEvent, value); }
			remove { events.RemoveHandler(ResolveRequestCacheEvent, value); }
		}

		/// <summary>
		/// Occurs when a cache need to be updated.
		/// </summary>
		public event ExtensionHandler UpdateRequestCache
		{
			add { events.AddHandler(UpdateRequestCacheEvent, value); }
			remove { events.RemoveHandler(UpdateRequestCacheEvent, value); }
		}

		internal void RaiseContextCreated(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[ContextCreatedEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaiseContextDisposed(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[ContextDisposedEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaisePostProcess(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[PostProcessEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaisePreProcess(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[PreProcessEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaiseReleaseRequestState(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[ReleaseSessionStateEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaiseAcquireRequestState(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[AcquireSessionStateEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaiseUnhandledError(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[UnhandledExceptionEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaiseActionError(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[ActionExceptionEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaiseAuthenticateRequest(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[AuthenticateRequestEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaiseAuthorizeRequest(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[AuthorizeRequestEvent];
			if (eventDelegate != null) eventDelegate(context);
		}

		internal void RaiseResolveRequestCache(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[UpdateRequestCacheEvent];
			if (eventDelegate != null) eventDelegate(context);
		}
		
		internal void RaiseUpdateRequestCache(IRailsEngineContext context)
		{
			ExtensionHandler eventDelegate = (ExtensionHandler) events[UpdateRequestCacheEvent];
			if (eventDelegate != null) eventDelegate(context);
		}
	}
}
