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

namespace AuthenticationUsingForms.Controllers
{
	using System;
	using AuthenticationUsingFilters.Model;
	using Castle.MonoRail.Framework;

	[Layout("default")]
	public class LoginController : SmartDispatcherController
	{
		public void Index()
		{
		}
		
		public void LogIn(String username, String password, bool rememberme, string ReturnUrl)
		{
			// We should authenticate against a database table or something similar
			// but here, everything is ok as long as the 
			// password and username are non-empty
			
			if (IsValid(username, password))
			{
				CancelView();
				
				// Ideally we would look up an user from the database
				// The domain model that represents the user
				// could implement IPrincipal or we could use an adapter
				
				User user = new User(username, new string[0]);
				
				Session["user"] = user;
				
				Redirect(ReturnUrl);
			}
			else
			{
				// If we got here then something is wrong 
				// with the supplied username/password
			
				Flash["error"] = "Invalid user name or password. Try again.";
				RedirectToAction("Index", "ReturnUrl=" + ReturnUrl);
			}
		}

		private bool IsValid(string username, string password)
		{
			return username != null && password != null;
		}
	}
}
