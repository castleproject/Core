// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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
	using System.Collections;

	[Serializable]
	public class User : Identifiable
	{
		private string name;
		private string email;
		private string passwd;

		public User(string name, string email, string passwd)
		{
			this.name = name;
			this.email = email;
			this.passwd = passwd;
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public string Email
		{
			get { return email; }
			set { email = value; }
		}

		public string Password
		{
			get { return passwd; }
			set { passwd = value; }
		}
	}

	/// <summary>
	/// Summary description for UserCollection.
	/// </summary>
	[Serializable]
	public class UserCollection : ReadOnlyCollectionBase
	{
		public void Add(User user)
		{
			lock(this.InnerList.SyncRoot)
			{
				InnerList.Add(user);
			}
		}
	}
}
