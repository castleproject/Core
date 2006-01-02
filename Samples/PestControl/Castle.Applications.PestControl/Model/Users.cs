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

namespace Castle.Applications.PestControl.Model
{
	using System;
	using System.Security.Principal;
	using System.Collections;

	[Serializable]
	public class User : Identifiable, IPrincipal, IIdentity
	{
		String _name; String _pass; String _email;

		public User(string name, string email, string passwd)
		{
			_name = name;
			_email = email;
			_pass = passwd;
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Email
		{
			get { return _email; }
			set { _email = value; }
		}

		public string Password
		{
			get { return _pass; }
			set { _pass = value; }
		}

		public bool IsInRole(string role)
		{
			return false;
		}

		public IIdentity Identity
		{
			get { return this; }
		}

		#region IIdentity Members

		public bool IsAuthenticated
		{
			get { return true; }
		}

		string System.Security.Principal.IIdentity.Name
		{
			get { return _name; }
		}

		public string AuthenticationType
		{
			get { return "custom"; }
		}

		#endregion
	}

	/// <summary>
	/// Summary description for UserCollection.
	/// </summary>
	[Serializable]
	public class UserCollection : ReadOnlyCollectionBase
	{
		public void Add(User user)
		{
			lock (this.InnerList.SyncRoot)
			{
				InnerList.Add(user);
			}
		}

		public User FindByEmail(String email)
		{
			lock (this.InnerList.SyncRoot)
			{
				foreach (User user in this)
				{
					if (CaseInsensitiveComparer.Default.Compare(user.Email, email) == 0)
					{
						return user;
					}
				}
			}

			return null;
		}

		public bool Authenticate(String email, String password)
		{
			User user = FindByEmail(email);

			if (user == null) return false;

			if (user.Password.Equals(password))
			{
				return true;
			}

			return false;
		}
	}
}