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

namespace Castle.Applications.PestControl.Model
{
	using System;

	using Bamboo.Prevalence;

	/// <summary>
	/// Summary description for UserCommands.
	/// </summary>
	[Serializable]
	public class CreateUserCommand : ICommand
	{
		String _name; String _pass; String _email;

		public CreateUserCommand(String name, String pass, String email)
		{
			_name = name; _pass = pass; _email = email;
		}

		public object Execute(object system)
		{
			UserCollection users = (system as PestControlModel).Users;

			User user = users.FindByEmail( _email);

			if (user != null)
			{
				throw new ApplicationException("User with same email already registered");
			}

			user = new User(_name, _email, _pass);

			users.Add( user );

			return user;
		}
	}
}
