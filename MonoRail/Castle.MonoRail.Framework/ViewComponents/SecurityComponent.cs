// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.ViewComponents
{
	/// <summary>
	/// Only renders the body if the current user has the specified role
	/// <example>
	/// <code>
	/// #blockcomponent(SecurityComponent with "role=IsAdmin")
	///		Content only available to admin
	/// #end
	/// </code>
	/// </example>
	/// <para>or for multiple roles (using "or")</para>
	/// <example>
	/// <code>
	/// #blockcomponent(SecurityComponent with "roles=Manager,Admin")
	///		Content only available to admin or managers
	/// #end
	/// </code>
	/// </example>
	/// </summary>
	public class SecurityComponent : ViewComponent
	{
		private bool shouldRender;

		/// <summary>
		/// Called by the framework once the component instance
		/// is initialized
		/// </summary>
		public override void Initialize()
		{
			string role = (string) ComponentParams["role"];
			string roles = (string) ComponentParams["roles"];

			if (role == null && roles == null)
			{
				throw new MonoRailException("SecurityComponent: you must supply a role (or roles) parameter");
			}

			shouldRender = false;

			if (RailsContext.CurrentUser != null)
			{
				if (role != null)
				{
					shouldRender = RailsContext.CurrentUser.IsInRole(role);
				}
				else
				{
					foreach(string itRole in roles.Split(','))
					{
						if (RailsContext.CurrentUser.IsInRole(itRole.Trim()))
						{
							shouldRender = true;
							break;
						}
					}
				}
			}
		}

		/// <summary>
		/// Called by the framework so the component can
		/// render its content
		/// </summary>
		public override void Render()
		{
			if (shouldRender)
			{
				Context.RenderBody();
			}
		}
	}
}
