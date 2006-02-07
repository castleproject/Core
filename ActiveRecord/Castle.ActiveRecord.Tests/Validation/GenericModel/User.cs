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
#if dotNet2
namespace Castle.ActiveRecord.Tests.Validation.GenericModel
{
	using System;

	[ActiveRecord("users")]
	public class User : ActiveRecordValidationBase<User>
	{
		private String login;
		private String name;
		private String email;
		private String password;
		private String confirmationPassword;
		private int id;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[ValidateNotEmpty, Property]
		public string Login
		{
			get { return login; }
			set { login = value; }
		}

		[ValidateNotEmpty, Property]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[ValidateEmail, ValidateNotEmpty, Property]
		public string Email
		{
			get { return email; }
			set { email = value; }
		}

		[ValidateConfirmation("ConfirmationPassword"), ValidateNotEmpty, Property]
		public string Password
		{
			get { return password; }
			set { password = value; }
		}

		[ValidateNotEmpty, Property]
		public string ConfirmationPassword
		{
			get { return confirmationPassword; }
			set { confirmationPassword = value; }
		}
	}
}
#endif