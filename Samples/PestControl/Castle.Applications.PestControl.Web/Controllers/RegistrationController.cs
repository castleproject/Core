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

namespace Castle.Applications.PestControl.Web.Controllers
{
	using System;

	using Bamboo.Prevalence;

	using Castle.Model;

	using Castle.CastleOnRails.Framework;

	using Castle.Applications.PestControl.Model;

	/// <summary>
	/// Summary description for RegistrationController.
	/// </summary>
	[Layout("default")]
	public class RegistrationController : SmartDispatcherController
	{
		private PrevalenceEngine _engine;

		public RegistrationController( PrevalenceEngine engine )
		{
			_engine = engine;
		}

		public void Signup()
		{
			
		}

		public void RegisterUser(String name, String email, String passwd)
		{
			User user = (User)
				_engine.ExecuteCommand( new CreateUserCommand(name, passwd, email) );

			HttpContext.User = user;

			Redirect("home", "index");
		}
	}
}
