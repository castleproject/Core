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
	public class ProcessEngine
	{
		private IControllerFactory _controllerFactory;
		private IViewEngine _viewEngine;
		private IFilterFactory _filterFactory;
		private IResourceFactory _resourceFactory;
		private IScaffoldingSupport _scaffoldingSupport;

		public ProcessEngine(IControllerFactory controllerFactory, IViewEngine viewEngine) : 
			this(controllerFactory, viewEngine, new DefaultFilterFactory(), new DefaultResourceFactory(), null)
		{
		}

		public ProcessEngine(IControllerFactory controllerFactory, 
			IViewEngine viewEngine, IFilterFactory filterFactory, 
			IResourceFactory resourceFactory, IScaffoldingSupport scaffoldingSupport)
		{
			_controllerFactory = controllerFactory;
			_viewEngine = viewEngine;
			_filterFactory = filterFactory;
			_resourceFactory = resourceFactory;
			_scaffoldingSupport = scaffoldingSupport;
		}

		public IControllerFactory ControllerFactory
		{
			get { return _controllerFactory; }
		}

		public IViewEngine ViewEngine
		{
			get { return _viewEngine; }
		}

		public IFilterFactory FilterFactory
		{
			get { return _filterFactory; }
		}

		public IResourceFactory ResourceFactory
		{
			get { return _resourceFactory; }
		}

		public IScaffoldingSupport ScaffoldingSupport
		{
			get { return _scaffoldingSupport; }
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

			Controller controller = _controllerFactory.GetController( info );

			if (controller == null)
			{
				String message = String.Format("No controller for {0}\\{1}", info.Area, info.Controller);
				throw new RailsException(message);
			}

			try
			{
				controller.Process( 
					context, _filterFactory, _resourceFactory, 
					info.Area, info.Controller, info.Action, _viewEngine, _scaffoldingSupport );
			}
			finally
			{
				_controllerFactory.Release(controller);
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
