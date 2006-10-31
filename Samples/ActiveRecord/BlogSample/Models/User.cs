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

namespace BlogSample
{
	using System;
	using Castle.ActiveRecord;

	using NHibernate.Expression;

	[ActiveRecord("[User]")]
	public class User : ActiveRecordBase
	{
		private int id;
		private string username;
		private string password;

		public User()
		{
			
		}

		public User(string username, string password)
		{
			this.username = username;
			this.password = password;
		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public string Username
		{
			get { return username; }
			set { username = value; }
		}

		[Property]
		public string Password
		{
			get { return password; }
			set { password = value; }
		}
		
		public static int GetUsersCount()
		{
			return CountAll(typeof(User));
		}

		public static User FindByUserName(string userName)
		{
			// Note that we use the property name, _not_ the column name
			return (User) FindOne(typeof(User), Expression.Eq("Username", userName));
		}
	}
}
