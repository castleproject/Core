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

	/// <summary>
	/// Summary description for ProcessEngine.
	/// </summary>
	public class ProcessEngine
	{
		private IControllerFactory _controllerFactory;
		private IViewEngine _viewEngine;

		public ProcessEngine(IControllerFactory controllerFactory, IViewEngine viewEngine)
		{
			_controllerFactory = controllerFactory;
			_viewEngine = viewEngine;
		}

		public virtual void Process( IRailsEngineContext context )
		{
			UrlInfo info = ExtractUrlInfo(context);

			Controller controller = _controllerFactory.GetController( info.Controller );

			try
			{
				controller.Process( 
					context, info.Area, info.Controller, info.Action, _viewEngine );
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
