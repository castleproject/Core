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
	using System.Security.Principal;

	/// <summary>
	/// This component renders different inner
	/// sections based on the current principal state (authenticated or not)
	/// <code>
	/// #blockcomponent(AuthenticatedContent)
	///	#logged
	///   Welcome back $context.CurrentUser.Identity.Name
	/// #end
	/// #notlogged
	///   Create your account by clicking here.
	/// #end
	/// #end
	/// </code>
	/// </summary>
	public class AuthenticatedContent : ViewComponent
	{
		private const string LoggedSection = "logged";
		private const string NotLoggedSection = "notlogged";

		/// <summary>
		/// Called by the framework so the component can
		/// render its content
		/// </summary>
		public override void Render()
		{
			if (IsAuthenticated())
			{
				if (Context.HasSection(LoggedSection))
				{
					Context.RenderSection(LoggedSection);
				}
			}
			else
			{
				if (Context.HasSection(NotLoggedSection))
				{
					Context.RenderSection(NotLoggedSection);
				}
			}
		}

		/// <summary>
		/// Implementor should return true only if the
		/// <c>name</c> is a known section the view component
		/// supports.
		/// </summary>
		/// <param name="name">section being added</param>
		/// <returns>
		/// 	<see langword="true"/> if section is supported
		/// </returns>
		public override bool SupportsSection(string name)
		{
			return name == LoggedSection || name == NotLoggedSection;
		}

		private bool IsAuthenticated()
		{
			IPrincipal user = RailsContext.CurrentUser;

			return user != null && user.Identity != null && user.Identity.IsAuthenticated;
		}
	}
}
