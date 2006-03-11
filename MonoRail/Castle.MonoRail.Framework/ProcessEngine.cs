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
	/// Core engine. Performs the base work or the
	/// framework, processing the URL and dispatching 
	/// the execution to the controller.
	/// </summary>
	/// <remarks>
	/// This is were all fun begins.
	/// </remarks>
	[Obsolete("This class is no longer used, use MonoRailHttpHandler.CurrentContext")]
	public class ProcessEngine
	{
		/// <summary>
		/// Returns the MonoRail context assosciated with the current
		/// request if one is available, otherwise <c>null</c>.
		/// </summary>
		[Obsolete("Use MonoRailHttpHandler.CurrentContext instead")]
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
