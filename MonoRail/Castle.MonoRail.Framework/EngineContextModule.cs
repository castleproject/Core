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
	using System.Reflection;

	using Castle.Core.Logging;

	/// <summary>
	/// Provides the services used and shared by the framework. Also 
	/// is in charge of creating an implementation of <see cref="IRailsEngineContext"/>
	/// upon the start of a new request.
	/// </summary>
	public class EngineContextModule : IHttpModule 
	{
		internal const String RailsContextKey = "rails.context";
		
		private static bool initialized;
		private static EngineContextService engineService;
		private static object locker = new object();

		private ILogger log;

		/// <summary>
		/// Configures the framework, starts the services
		/// and application hooks.
		/// </summary>
		public void Init(HttpApplication context)
		{
			GetLoggerFromContext(context);
			
			log.Debug("Initializing Module");
			// In some weird circunstances 
			// IIS 6 worker process seems to initialize the module
			// more than once. We protect ourselves from this locking
			// the initialization process
			lock(locker)
			{
				if (!initialized)
				{
					engineService = new EngineContextService(this, log);
					engineService.Initialize();
					
					initialized = true;
				}
				else
				{
					log.Debug("Skipped initialization");
				}
			}

			engineService.InitApplicationHooks(context);
		}
		
		public void Dispose()
		{
			log.Debug("Dispose");
			//engineService.Dispose();
		}
		
		/// <remarks>
		/// The code here looks somewhat ugly, but it was the best I could get.
		/// </remarks>
		private void GetLoggerFromContext(HttpApplication context)
		{
			if (log == null)
			{
				log = NullLogger.Instance;
				
				BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance;
				if (context.GetType().GetProperty("Container", bindingFlags) != null)
				{
					Object container = context.GetType().GetProperty("Container", bindingFlags).GetValue(context, null);
					if (container != null)
					{
						MethodInfo resolveMethod = container.GetType().GetMethod("Resolve", bindingFlags, null, new Type[] { typeof(Type) }, null);
						ILoggerFactory loggerFactory = (ILoggerFactory) resolveMethod.Invoke(container, new Object[] { typeof(ILoggerFactory) });
						log = loggerFactory.Create(GetType());
					}
				}
			}
		}
		
		internal static bool Initialized
		{
			get { return engineService.Initialized; }
		}

		internal static IRailsEngineContext ObtainRailsEngineContext(HttpContext context)
		{
			return engineService.ObtainRailsEngineContext(context);
		}
	}
}
