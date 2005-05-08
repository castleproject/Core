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

namespace Castle.Applications.MindDump.Presentation.Controllers
{
	using System;
	using System.Collections;

	using Castle.MonoRail.Framework;

	using Castle.Applications.MindDump.Model;
	using Castle.Applications.MindDump.Services;


	[Layout("default")]
	public class AccountController : AbstractSecureController
	{
		private AccountService _accountService;
		private AuthenticationService _authenticationService;
		private EncryptionService _encryptionService;

		public AccountController(AccountService accountService, 
			AuthenticationService authenticationService, 
			EncryptionService encryptionService)
		{
			_accountService = accountService;
			_authenticationService = authenticationService;
			_encryptionService = encryptionService;
		}

		[SkipFilter]
		public void New()
		{
		}

		[SkipFilter]
		[Rescue("errorcreatingaccount")]
		public void CreateAccount(String login, String name, String email,
		                      String pwd, String pwd2, String blogname,
		                      String blogdesc, String theme)
		{
			// Perform some simple validation
			if (!IsValid(login, name, email, pwd, pwd2, blogname, blogdesc, theme))
			{
				RenderView("new");
				return;
			}
 
			Author author = new Author(name, login, pwd);
			Blog blog = new Blog(blogname, blogdesc, theme, author);

			_accountService.CreateAccountAndBlog( blog );

			// Done, now lets log on into the system
			PerformLogin(login, pwd);
		}

		[SkipFilter]
		public void Authentication()
		{
		}

		[SkipFilter]
		public void PerformLogin(String login, String pwd)
		{
			if (!_authenticationService.Authenticate(login, pwd))
			{
				Context.Flash["errormessage"] = "User not found or incorrect password.";
			
				RenderView("Authentication");
			}
			else
			{
				DateTime twoWeeks = DateTime.Now.Add( new TimeSpan(14,0,0,0) ); 

				Context.Response.CreateCookie("authenticationticket", 
					_encryptionService.Encrypt(login), twoWeeks );
				
				Redirect("Maintenance", "newentry");
			}
		}

		//
		// Private operations to handle common tasks
		//

		private bool IsValid(string login, string name, string email, string pwd, string pwd2, string blogname, string blogdesc, string theme)
		{
			ArrayList errors = new ArrayList();

			if (login.Trim().Length == 0)
			{
				errors.Add("You must supply a valid login name");
			}
			if (name.Trim().Length == 0)
			{
				errors.Add("You must supply a valid name");
			}
			if (pwd.Trim().Length == 0)
			{
				errors.Add("You must supply a valid password");
			}
			if (pwd2.Trim().Length == 0)
			{
				errors.Add("You must supply a valid password confirmation");
			}
			else if (!pwd.Equals(pwd2))
			{
				errors.Add("Passwords don't match...");
			}
			if (blogname.Trim().Length == 0)
			{
				errors.Add("You must supply a valid blog name");
			}

			if (errors.Count == 0) return true;

			Context.Flash["errormessages"] = errors;
			
			return false;
		}
	}
}