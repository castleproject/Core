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

namespace Castle.Applications.MindDump.Web.Controllers.Filters
{
	using System;

	using Castle.CastleOnRails.Framework;

	using Castle.Applications.MindDump.Services;

	/// <summary>
	/// This filter tries to authenticate the user by 
	/// looking for the cookie. However, if the cookie 
	/// is not found, that's ok. We go on with
	/// the page processing
	/// </summary>
	public class AuthenticationAttemptFilter : AuthenticationCheckFilter
	{
		public AuthenticationAttemptFilter(AccountService accountService, EncryptionService encryptionService) : base(accountService, encryptionService)
		{
		}

		public override bool Perform(ExecuteEnum exec, IRailsEngineContext context, Controller controller)
		{
			if (context.CurrentUser.Identity.IsAuthenticated)
			{
				return true;
			}

			base.PerformAuthentication(context);

			return true;
		}
	}
}
