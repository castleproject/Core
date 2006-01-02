// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
	using System.Collections;
	using System.Collections.Specialized;
	using System.ComponentModel.Design;
	using System.Web;

	using Castle.Components.Common.EmailSender;
	
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Core engine. Performs the base work or the
	/// framework, processing the URL and dispatching 
	/// the execution to the controller.
	/// </summary>
	/// <remarks>
	/// This is were all fun begins.
	/// </remarks>
	public class ProcessEngine : MarshalByRefObject, IServiceContainer
	{
		#region Fields

		internal static readonly String RailsContextKey = "rails.context";
		
		private readonly IControllerFactory controllerFactory;
		private readonly IDictionary _type2Service = new HybridDictionary();
		private readonly ExtensionComposite extensionComposite;

		#endregion

		#region Constructors

		public ProcessEngine(IControllerFactory controllerFactory, ControllerDescriptorBuilder controllerDescriptorBuilder, IViewEngine viewEngine) : 
			this(controllerFactory, controllerDescriptorBuilder, viewEngine, new DefaultFilterFactory(), 
			     new DefaultResourceFactory(), null, new DefaultViewComponentFactory(), new IMonoRailExtension[0], null)
		{
		}

		public ProcessEngine(IControllerFactory controllerFactory, ControllerDescriptorBuilder controllerDescriptorBuilder, IViewEngine viewEngine, IViewComponentFactory viewCompFactory) : 
			this(controllerFactory, controllerDescriptorBuilder, viewEngine, new DefaultFilterFactory(), 
				new DefaultResourceFactory(), null, viewCompFactory, new IMonoRailExtension[0], null)
		{
		}

		public ProcessEngine(IControllerFactory controllerFactory, ControllerDescriptorBuilder controllerDescriptorBuilder, 
			IViewEngine viewEngine, IFilterFactory filterFactory, 
			IResourceFactory resourceFactory, IScaffoldingSupport scaffoldingSupport, 
			IViewComponentFactory viewCompFactory, IMonoRailExtension[] extensions, IEmailSender emailSender)
		{
			this.controllerFactory = controllerFactory;
			this.extensionComposite = new ExtensionComposite(extensions);

			AddService(typeof(IControllerFactory), controllerFactory);
			AddService(typeof(IViewEngine), viewEngine);
			AddService(typeof(IFilterFactory), filterFactory);
			AddService(typeof(IResourceFactory), resourceFactory);
			AddService(typeof(IScaffoldingSupport), scaffoldingSupport);
			AddService(typeof(IViewComponentFactory), viewCompFactory);
			AddService(typeof(ControllerDescriptorBuilder), controllerDescriptorBuilder);
			AddService(typeof(IMonoRailExtension), extensionComposite);
			AddService(typeof(IEmailSender), emailSender);
		}

		#endregion

		#region Properties

		/// <summary>
		/// Returns the MonoRail context assosciated with the current
		/// request if one is available, otherwise <c>null</c>.
		/// </summary>
		public static IRailsEngineContext CurrentContext
		{
			get
			{
				HttpContext context = HttpContext.Current;
				
				// Are we in a web request?
				if (context == null) return null;
								
				return context.Items[RailsContextKey] as IRailsEngineContext;
			}
		}

		#endregion

		#region IServiceContainer

		public void AddService(Type serviceType, object serviceInstance)
		{
			if (serviceInstance != null)
			{
				_type2Service[serviceType] = serviceInstance;
			}
		}

		public void AddService(Type serviceType, object serviceInstance, bool promote)
		{
			throw new NotImplementedException();
		}

		public void AddService(Type serviceType, ServiceCreatorCallback callback)
		{
			throw new NotImplementedException();
		}

		public void AddService(Type serviceType, ServiceCreatorCallback callback, bool promote)
		{
			throw new NotImplementedException();
		}

		public void RemoveService(Type serviceType)
		{
			throw new NotImplementedException();
		}

		public void RemoveService(Type serviceType, bool promote)
		{
			throw new NotImplementedException();
		}

		public object GetService(Type serviceType)
		{
			return _type2Service[serviceType];
		}

		#endregion

		/// <summary>
		/// Performs the base work of MonoRail. Extracts 
		/// the information from the URL, obtain the controller 
		/// that matches this information and dispatch the execution 
		/// to it.
		/// </summary>
		/// <param name="context"></param>
		public virtual void Process( IRailsEngineContext context )
		{
			UrlInfo info = ExtractUrlInfo(context);

			Controller controller = controllerFactory.CreateController( info );

			if (controller == null)
			{
				String message = String.Format("No controller for {0}\\{1}", info.Area, info.Controller);
				
				throw new RailsException(message);
			}

			try
			{
				controller.Process( context, this, info.Area, info.Controller, info.Action );
			}
			finally
			{
				controllerFactory.Release(controller);

				// Remove items from flash before leaving the page
				context.Flash.Sweep();
	
				if (context.Flash.HasItemsToKeep)
				{
					context.Session[Flash.FlashKey] = context.Flash;
				}
				else if (context.Session.Contains(Flash.FlashKey))
				{
					context.Session.Remove(Flash.FlashKey);
				}
			}
		}

		/// <summary>
		/// Can be overriden so new semantics can be supported.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		protected virtual UrlInfo ExtractUrlInfo(IRailsEngineContext context)
		{
			String vdir = context.ApplicationPath;

			return UrlTokenizer.ExtractInfo(context.Url, vdir);
		}

		protected void RaiseEngineContextCreated(IRailsEngineContext context)
		{
			context.UnderlyingContext.Items[RailsContextKey] = context;

			if (!extensionComposite.HasExtension) return;

			extensionComposite.OnRailsContextCreated(context, this);
		}

		protected void RaiseEngineContextDiscarded(IRailsEngineContext context)
		{
			if (!extensionComposite.HasExtension) return;

			extensionComposite.OnRailsContextDiscarded(context, this);
		}
	}
}
