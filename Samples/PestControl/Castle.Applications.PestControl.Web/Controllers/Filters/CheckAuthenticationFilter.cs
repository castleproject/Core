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

namespace Castle.Applications.PestControl.Web.Controllers.Filters
{
	using System;
	using System.Security.Principal;

	using Castle.MonoRail.Framework;

	public class CheckAuthenticationFilter : IFilter
	{
		public bool Perform(ExecuteEnum when, IRailsEngineContext context, Controller controller)
		{
			context.CurrentUser = context.Session["user"] as IPrincipal;

			if (context.CurrentUser == null || 
				context.CurrentUser.Identity == null || 
				!context.CurrentUser.Identity.IsAuthenticated)
			{
				context.Session["FromUrl"] = context.Url;
				
				context.Response.Redirect("home", "authentication");
				
				return false;
			}
			return true;
		}
	}
}
