// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace PetStore.Web.Controllers
{
	using Castle.MonoRail.Framework;


	public sealed class AuthenticationFilter : IFilter
	{
		public bool Perform(ExecuteWhen exec, IEngineContext context, IController controller, IControllerContext controllerContext)
		{
			if (context.CurrentUser == null || !context.CurrentUser.Identity.IsAuthenticated)
			{
				context.Response.Redirect("admin", "login");

				return false;
			}

			return true;
		}
	}
}
