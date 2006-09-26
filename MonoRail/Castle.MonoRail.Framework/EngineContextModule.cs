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

namespace Castle.MonoRail.Framework
{
	using System;
	using System.Web;

	using Castle.MonoRail.Framework.Adapters;

	/// <summary>
	/// Provides the services used and shared by the framework. Also 
	/// is in charge of creating an implementation of <see cref="IRailsEngineContext"/>
	/// upon the start of a new request.
	/// </summary>
	public class EngineContextModule : IHttpModule 
	{
		internal static readonly String RailsContextKey = "rails.context";

		private static MonoRailServiceContainer container;
			
		/// <summary>
		/// Configures the framework, starts the services
		/// and application hooks.
		/// </summary>
		/// <param name="context"></param>
		public void Init(HttpApplication context)
		{
			// Possible race condition here, but not really important
			if (container == null)
			{
				container = new MonoRailServiceContainer();

				container.Start();
			}

			SubscribeToApplicationHooks(context);
		}

		public void Dispose()
		{
		}
		
		/// <summary>
		/// Registers to <c>HttpApplication</c> events
		/// </summary>
		/// <param name="context">The application instance</param>
		private void SubscribeToApplicationHooks(HttpApplication context)
		{
			context.BeginRequest += new EventHandler(OnBeginRequest);
			context.EndRequest += new EventHandler(OnEndRequest);
			context.AcquireRequestState += new EventHandler(OnAcquireRequestState);
			context.ReleaseRequestState += new EventHandler(OnReleaseRequestState);
			context.PreRequestHandlerExecute += new EventHandler(OnPreRequestHandlerExecute);
			context.PostRequestHandlerExecute += new EventHandler(OnPostRequestHandlerExecute);
			context.Error += new EventHandler(OnError);		    
		}

		#region Hooks dispatched to extensions

		private void OnBeginRequest(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication) sender;
			HttpContext context = app.Context;

			IRailsEngineContext mrContext = CreateRailsEngineContext(context);

			container.extensionManager.RaiseContextCreated(mrContext);
		}

		private void OnEndRequest(object sender, EventArgs e)
		{
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			container.extensionManager.RaiseContextDisposed(mrContext);
		}

		private void OnAcquireRequestState(object sender, EventArgs e)
		{
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			container.extensionManager.RaiseAcquireRequestState(mrContext);
		}

		private void OnReleaseRequestState(object sender, EventArgs e)
		{
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			container.extensionManager.RaiseReleaseRequestState(mrContext);
		}

		private void OnPreRequestHandlerExecute(object sender, EventArgs e)
		{
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			container.extensionManager.RaisePreProcess(mrContext);
		}

		private void OnPostRequestHandlerExecute(object sender, EventArgs e)
		{
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			container.extensionManager.RaisePostProcess(mrContext);
		}

		private void OnError(object sender, EventArgs e)
		{
			IRailsEngineContext mrContext = ObtainContextFromApplication(sender);

			mrContext.LastException = mrContext.UnderlyingContext.Server.GetLastError();

			container.extensionManager.RaiseUnhandledError(mrContext);
		}

		private static IRailsEngineContext ObtainContextFromApplication(object sender)
		{
			HttpApplication app = (HttpApplication) sender;
			HttpContext context = app.Context;

			return ObtainRailsEngineContext(context);
		}

		#endregion
		
		private IRailsEngineContext CreateRailsEngineContext(HttpContext context)
		{
			IRailsEngineContext mrContext = ObtainRailsEngineContext(context);

			if (mrContext == null)
			{
				mrContext = new DefaultRailsEngineContext(container, context);

				context.Items[RailsContextKey] = mrContext;
			}

			return mrContext;
		}

		internal static bool Initialized
		{
			get { return container != null; }
		}

		internal static IRailsEngineContext ObtainRailsEngineContext(HttpContext context)
		{
			return (IRailsEngineContext) context.Items[RailsContextKey];
		}
	}
}
