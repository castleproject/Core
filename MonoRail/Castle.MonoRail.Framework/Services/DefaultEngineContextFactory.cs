// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Services
{
	using System.Collections;
	using System.Web;
	using System.Web.SessionState;
	using Castle.MonoRail.Framework.Adapters;
	using Castle.MonoRail.Framework.Container;

	/// <summary>
	/// Pendent
	/// </summary>
	public class DefaultEngineContextFactory : IEngineContextFactory
	{
		/// <summary>
		/// Pendent.
		/// </summary>
		/// <param name="container"></param>
		/// <param name="urlInfo"></param>
		/// <param name="context"></param>
		/// <returns></returns>
		public IEngineContext Create(IMonoRailContainer container, UrlInfo urlInfo, HttpContext context)
		{
			IDictionary session = ResolveRequestSession(container, urlInfo, context);

			IUrlBuilder urlBuilder = container.UrlBuilder;

			return new DefaultEngineContext(container, urlInfo, context,
			                                new ServerUtilityAdapter(context.Server),
			                                new RequestAdapter(context.Request),
											new ResponseAdapter(context.Response, urlInfo, urlBuilder),
											new TraceAdapter(context.Trace), session);
		}

		/// <summary>
		/// Resolves the request session.
		/// </summary>
		protected virtual IDictionary ResolveRequestSession(IMonoRailContainer container, UrlInfo urlInfo, HttpContext context)
		{
			return null;
		}
	}
}