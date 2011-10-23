// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.Components.Validator.Tests.Models
{
	public class Client : Person
	{
		private string email, password, confirmation;

		public Client()
		{
		}

		public Client(int id, int age, string name, string address,
		              string email, string password, string confirmation) : base(id, age, name, address)
		{
			this.email = email;
			this.password = password;
			this.confirmation = confirmation;
		}

		[ValidateGroupNotEmpty("EmailOrPass")]
		[ValidateEmail]
		public string Email
		{
			get { return email; }
			set { email = value; }
		}

		[ValidateGroupNotEmpty("EmailOrPass")]
		[ValidateLength(3, 12, "Invalid password length")]
		public string Password
		{
			get { return password; }
			set { password = value; }
		}

		[ValidateSameAs("Password", RunWhen=RunWhen.Custom)]
		public string Confirmation
		{
			get { return confirmation; }
			set { confirmation = value; }
		}
	}
}