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
	using System.Web;
	using System.Web.SessionState;

	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Implements <see cref="IHttpHandler"/> to dispatch the web
	/// requests. 
	/// </summary>
	public class MonoRailHttpHandler : MarshalByRefObject, IHttpHandler, IRequiresSessionState
	{
		public void ProcessRequest(HttpContext context)
		{
			IRailsEngineContext mrContext = EngineContextModule.ObtainRailsEngineContext(context);

			Process(mrContext);
		}

		public bool IsReusable
		{
			get { return true; }
		}

		/// <summary>
		/// Performs the base work of MonoRail. Extracts 
		/// the information from the URL, obtain the controller 
		/// that matches this information and dispatch the execution 
		/// to it.
		/// </summary>
		/// <param name="context"></param>
		public virtual void Process(IRailsEngineContext context)
		{
			UrlInfo info = ExtractUrlInfo(context);

			IControllerFactory controllerFactory = (IControllerFactory) context.GetService(typeof(IControllerFactory));

			Controller controller = controllerFactory.CreateController(info);

			if (controller == null)
			{
				String message = String.Format("No controller for {0}\\{1}", info.Area, info.Controller);
				
				throw new RailsException(message);
			}

			try
			{
				controller.Process(context, info.Area, info.Controller, info.Action);
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
			return context.UrlInfo;
		}

		public static IRailsEngineContext CurrentContext
		{
			get
			{
				HttpContext context = HttpContext.Current;
				
				// Are we in a web request?
				if (context == null) return null;
								
				return EngineContextModule.ObtainRailsEngineContext(context);
			}
		}
	}
}
