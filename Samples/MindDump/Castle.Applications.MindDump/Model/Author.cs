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

namespace Castle.Applications.MindDump.Model
{
	using System;
	using System.Collections;

	/// <summary>
	/// Summary description for Author.
	/// </summary>
	public class Author
	{
		private long _id;
		private string _name;
		private string _login;
		private string _password;
		private IList _posts;

		public Author()
		{
		}

		public long Id
		{
			get { return _id; }
			set { _id = value; }
		}

		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public string Login
		{
			get { return _login; }
			set { _login = value; }
		}

		public string Password
		{
			get { return _password; }
			set { _password = value; }
		}

		public IList Posts
		{
			get { return _posts; }
			set { _posts = value; }
		}
	}
}
