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

namespace PetStore.Model
{
	using System;

	using Castle.ActiveRecord;


	[ActiveRecord(DiscriminatorColumn="type", DiscriminatorType="String", DiscriminatorValue="user")]
	public class User : ActiveRecordBase
	{
		private int id;
		private string name;
		private string email;
		private string password;
		private string userType;

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[Property]
		public string Email
		{
			get { return email; }
			set { email = value; }
		}

		[Property]
		public string Password
		{
			get { return password; }
			set { password = value; }
		}

		/// <summary>
		/// Exposing the discriminator column
		/// has its restrictions
		/// </summary>
		[Property("type", Insert=false, Update=false)]
		public string UserType
		{
			get { return userType; }
			set { userType = value; }
		}
	}
}
