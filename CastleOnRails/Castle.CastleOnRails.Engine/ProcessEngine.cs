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
	/// Summary description for ProcessEngine.
	/// </summary>
	public class ProcessEngine
	{
		private IControllerFactory _controllerFactory;
		private IViewEngine _viewEngine;
		private IFilterFactory _filterFactory;

		public ProcessEngine(IControllerFactory controllerFactory, IViewEngine viewEngine) : 
			this(controllerFactory, viewEngine, new DefaultFilterFactory())
		{
		}

		public ProcessEngine(IControllerFactory controllerFactory, IViewEngine viewEngine, 
			IFilterFactory filterFactory)
		{
			_controllerFactory = controllerFactory;
			_viewEngine = viewEngine;
			_filterFactory = filterFactory;
		}

		public virtual void Process( IRailsEngineContext context )
		{
			UrlInfo info = ExtractUrlInfo(context);

			Controller controller = _controllerFactory.GetController( info );

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

		protected virtual UrlInfo ExtractUrlInfo(IRailsEngineContext context)
		{
			return UrlTokenizer.ExtractInfo(context.Url);
		}
	}
}
