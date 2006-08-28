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

	/// <summary>
	/// Coordinates the creation of new <see cref="MonoRailHttpHandler"/> 
	/// and uses the configuration to obtain the correct factories 
	/// instances.
	/// </summary>
	public class MonoRailHttpHandlerFactory : IHttpHandlerFactory
	{
		public MonoRailHttpHandlerFactory()
		{
		}

		public virtual IHttpHandler GetHandler(HttpContext context, 
			String requestType, String url, String pathTranslated)
		{
#if ALLOWTEST
			String isTest = context.Request.Headers["IsTestWorkerRequest"];
			
			if ("true" == isTest)
			{
				Castle.MonoRail.Framework.Internal.Test.TestContextHolder.SetContext(context);
			}
#endif

			if (!EngineContextModule.Initialized)
			{
				throw new RailsException("Looks like you forgot to register the http module " +
					typeof(EngineContextModule).FullName + "\r\nAdd '<add name=\"monorail\" type=\"Castle.MonoRail.Framework.EngineContextModule, Castle.MonoRail.Framework\" />' " +
					"to the <httpModules> section on your web.config");
			}

			IRailsEngineContext mrContext = EngineContextModule.ObtainRailsEngineContext(context);

			return ObtainMonoRailHandler(mrContext);
		}

		public virtual void ReleaseHandler(IHttpHandler handler)
		{
			HttpContext httpContext = HttpContext.Current;

			if (httpContext != null)
			{
				IRailsEngineContext mrContext = EngineContextModule.ObtainRailsEngineContext(HttpContext.Current);

				if (mrContext != null)
				{
					IMonoRailHttpHandlerProvider provider = ObtainMonoRailHandlerProvider(mrContext);
					if (provider != null) provider.ReleaseHandler(handler);
				}
			}
		}

		private IHttpHandler ObtainMonoRailHandler(IRailsEngineContext mrContext)
		{
			IHttpHandler mrHandler = null;
			IMonoRailHttpHandlerProvider provider = ObtainMonoRailHandlerProvider(mrContext);

			if (provider != null)
			{
				mrHandler = provider.ObtainMonoRailHttpHandler(mrContext);
			}
			
			if (mrHandler == null) mrHandler = new MonoRailHttpHandler(); 

			return mrHandler;
		}

		private IMonoRailHttpHandlerProvider ObtainMonoRailHandlerProvider(IRailsEngineContext mrContext)
		{
			return (IMonoRailHttpHandlerProvider) mrContext.GetService(typeof(IMonoRailHttpHandlerProvider));
		}
	}
}
