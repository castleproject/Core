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

namespace Castle.Applications.MindDump.Presentation.Filters
{
	using System;

	using Castle.MonoRail.Framework;

	using Castle.Applications.MindDump.Model;
	using Castle.Applications.MindDump.Services;
	using Castle.Applications.MindDump.Adapters;

	/// <summary>
	/// Looks for a cookie "authenticationticket", decrypt it and obtains the 
	/// login name. It then obtains the Author based on the login and
	/// associate the user with the context.
	/// </summary>
	public class AuthenticationCheckFilter : IFilter
	{
		private EncryptionService _encryptionService;
		private AccountService _accountService;

		public AuthenticationCheckFilter(AccountService accountService, 
			EncryptionService encryptionService)
		{
			_accountService = accountService;
			_encryptionService = encryptionService;
		}

		public virtual bool Perform(ExecuteEnum exec, IRailsEngineContext context, Controller controller)
		{
			if (!PerformAuthentication(context))
			{
				context.Response.Redirect("account", "authentication");
				return false;
			}

			return true;
		}

		protected bool PerformAuthentication(IRailsEngineContext context)
		{
			String contents = context.Request.ReadCookie("authenticationticket"); 
	
			if (contents == null)
			{
				return false;
			}
	
			String login = _encryptionService.Decrypt(contents);
	
			Author author = _accountService.ObtainAuthor(login);
	
			if (author == null)
			{
				return false;
			}
	
			context.CurrentUser = new PrincipalAuthorAdapter(author);
			
			return true;
		}
	}
}
