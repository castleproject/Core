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
	using System.Web.SessionState;

	using Castle.Components.Common.EmailSender;
	
	using Castle.MonoRail.Framework.Adapters;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Extends the <see cref="ProcessEngine"/> and implements 
	/// <see cref="IHttpHandler"/> to dispatch the web
	/// requests. 
	/// </summary>
	public class MonoRailHttpHandler : ProcessEngine, IHttpHandler, IRequiresSessionState
	{
		private String _url;

		public MonoRailHttpHandler( String url, IViewEngine viewEngine, 
			IControllerFactory controllerFactory, ControllerDescriptorBuilder controllerDescriptorBuilder, IFilterFactory filterFactory, 
			IResourceFactory resourceFactory, IScaffoldingSupport scaffoldingSupport, 
			IViewComponentFactory viewCompFactory, IMonoRailExtension[] extensions, IEmailSender sender)
			: base(controllerFactory, controllerDescriptorBuilder, viewEngine, filterFactory, resourceFactory, 
			       scaffoldingSupport, viewCompFactory, extensions, sender)
		{
			_url = url;
		}

		public void ProcessRequest(HttpContext context)
		{
			RailsEngineContextAdapter mrContext = new RailsEngineContextAdapter(context, _url);

			RaiseEngineContextCreated(mrContext);

			try
			{
				Process(mrContext);
			}
			finally
			{
				RaiseEngineContextDiscarded(mrContext);

				AddFlashToSessionIfUsed(mrContext);
			}
		}

		public bool IsReusable
		{
			get { return true; }
		}

		private static void AddFlashToSessionIfUsed(RailsEngineContextAdapter mrContext)
		{
			// Remove items from flash before leaving the page
			mrContext.Flash.Sweep();
	
			if (mrContext.Flash.Count != 0)
			{
				mrContext.Session[Flash.FlashKey] = mrContext.Flash;
			}
		}
	}
}
