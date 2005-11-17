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

namespace PetStore.Web.Controllers.Admin
{
	using System;

	using Castle.MonoRail.Framework;

	/// <summary>
	/// Note that this controller extends the secure controller
	/// </summary>
	[Layout("admin")] // see views/layouts/admin.vm
	public class AdminController : AbstractSecureController
	{
		/// <summary>
		/// Presents the login page.
		/// </summary>
		/// <remarks>
		/// As this controller extends the secure controller, which
		/// adds a before-action filter, we must use the
		/// SkipFilterAttribute so a non-authenticated user
		/// can perform the action
		/// </remarks>
		[SkipFilter]
		public void Login()
		{
		}

		/// <summary>
		/// When the login form is submitted, this action 
		/// is invoked. See the form action on the view
		/// views/admin/login.vm
		/// </summary>
		/// <param name="user"></param>
		/// <param name="password"></param>
		[SkipFilter]
		public void Authenticate(String user, String password)
		{
			Flash["error"] = "User cannot be found";

			RenderView("Login");
		}
	}
}
