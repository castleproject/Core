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

namespace Castle.MonoRail.Engine
{
	using System;
	using System.Web;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Core engine. Performs the base work or the
	/// framework, processing the URL and dispatching 
	/// the execution to the controller.
	/// </summary>
	/// <remarks>
	/// This is were all fun begins.
	/// </remarks>
	public class ProcessEngine
	{
		private IControllerFactory controllerFactory;
		private IViewEngine viewEngine;
		private IFilterFactory filterFactory;
		private IResourceFactory resourceFactory;
		private IScaffoldingSupport scaffoldingSupport;
		private IViewComponentFactory viewCompFactory;
		private ControllerDescriptorBuilder controllerDescriptorBuilder = new ControllerDescriptorBuilder();

		public ProcessEngine(IControllerFactory controllerFactory, IViewEngine viewEngine) : 
			this(controllerFactory, viewEngine, new DefaultFilterFactory(), 
			     new DefaultResourceFactory(), null, new DefaultViewComponentFactory())
		{
		}

		public ProcessEngine(IControllerFactory controllerFactory, IViewEngine viewEngine, IViewComponentFactory viewCompFactory) : 
			this(controllerFactory, viewEngine, new DefaultFilterFactory(), 
				new DefaultResourceFactory(), null, viewCompFactory)
		{
		}

		public ProcessEngine(IControllerFactory controllerFactory, 
			IViewEngine viewEngine, IFilterFactory filterFactory, 
			IResourceFactory resourceFactory, IScaffoldingSupport scaffoldingSupport, IViewComponentFactory viewCompFactory)
		{
			this.controllerFactory = controllerFactory;
			this.viewEngine = viewEngine;
			this.filterFactory = filterFactory;
			this.resourceFactory = resourceFactory;
			this.scaffoldingSupport = scaffoldingSupport;
			this.viewCompFactory = viewCompFactory;
		}

		public IControllerFactory ControllerFactory
		{
			get { return controllerFactory; }
		}

		public IViewEngine ViewEngine
		{
			get { return viewEngine; }
		}

		public IFilterFactory FilterFactory
		{
			get { return filterFactory; }
		}

		public IResourceFactory ResourceFactory
		{
			get { return resourceFactory; }
		}

		public IScaffoldingSupport ScaffoldingSupport
		{
			get { return scaffoldingSupport; }
		}

		public IViewComponentFactory ViewComponentFactory
		{
			get { return viewCompFactory; }
		}

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

			controller.MetaDescriptor = controllerDescriptorBuilder.BuildDescriptor(controller);

			try
			{
				controller.Process( 
					context, filterFactory, resourceFactory, 
					info.Area, info.Controller, info.Action, viewEngine, scaffoldingSupport, viewCompFactory );
			}
			finally
			{
				controllerFactory.Release(controller);
			}
		}

		/// <summary>
		/// Can be overriden so new semantics can be supported.
		/// </summary>
		/// <param name="context"></param>
		/// <returns></returns>
		protected virtual UrlInfo ExtractUrlInfo(IRailsEngineContext context)
		{
			String vdir = null;

			// Null means 'testing' mode
			if (HttpContext.Current != null)
			{
				vdir = HttpContext.Current.Request.ApplicationPath;
			}

			return UrlTokenizer.ExtractInfo(context.Url, vdir);
		}
	}
}
