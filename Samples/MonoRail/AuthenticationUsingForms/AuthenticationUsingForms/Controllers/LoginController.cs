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
	using System.Web.Security;

	using Castle.MonoRail.Framework;

	[Layout("default")]
	public class LoginController : SmartDispatcherController
	{
		public void Index()
		{
		}
		
		public void LogIn(String username, String password, bool rememberme, string ReturnUrl)
		{
			// We authenticate against the users defined on the web.config.
			
			//	<credentials passwordFormat="Clear">
			//		<user name="admin" password="admin" />
			//		<user name="user" password="user" />
			//	</credentials>
			
			if (FormsAuthentication.Authenticate(username, password))
			{
				CancelView();
				
				FormsAuthentication.RedirectFromLoginPage(username, rememberme, Context.ApplicationPath);

//				The RedirectFromLoginPage is roughly equivalent to 
//
//				FormsAuthentication.SetAuthCookie(username, rememberme, Context.ApplicationPath);
//				
//				if (ReturnUrl != null)
//				{
//					Redirect(ReturnUrl);
//				}
//				else
//				{
//					Redirect("home", "index");
//				}
				
				return;
			}
			
			// If we got here then something is wrong with the supplied username/password
			
			Flash["error"] = "Invalid user name or password. Try again.";
			RedirectToAction("Index", "ReturnUrl=" + ReturnUrl);
		}
	}
}
