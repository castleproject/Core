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

namespace Castle.CastleOnRails.Engine
{
	using System;

	using Castle.CastleOnRails.Framework;
	using Castle.CastleOnRails.Framework.Internal;

	/// <summary>
	/// Core engine. Performs the base work or the
	/// framework, processing the URL and dispatching 
	/// the execution to the controller.
	/// </summary>
	public class ProcessEngine
	{
		private String _virtualRootDir;
		private IControllerFactory _controllerFactory;
		private IViewEngine _viewEngine;
		private IFilterFactory _filterFactory;

		public ProcessEngine(String virtualRootDir, IControllerFactory controllerFactory, 
			IViewEngine viewEngine) : 
			this(virtualRootDir, controllerFactory, viewEngine, new DefaultFilterFactory())
		{
		}

		public ProcessEngine(String virtualRootDir, IControllerFactory controllerFactory, 
			IViewEngine viewEngine, IFilterFactory filterFactory)
		{
			_virtualRootDir = virtualRootDir;
			_controllerFactory = controllerFactory;
			_viewEngine = viewEngine;
			_filterFactory = filterFactory;
		}

		public String VirtualRootDir
		{
			get { return _virtualRootDir; }
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

		/// <summary>
		/// Performs the base work of Castle on Rails. Extracts 
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
					context, _filterFactory, 
					info.Area, info.Controller, info.Action, _viewEngine );
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
			return UrlTokenizer.ExtractInfo(context.Url, VirtualRootDir);
		}
	}
}
