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

namespace WizardSampleSite.Model
{
	using System;

	[Serializable]
	public class Account
	{
		private String name;
		private String email;
		private String username;
		private String pwd;
		private String pwdconfirmation;

		private String[] interests;

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

		public string Username
		{
			get { return username; }
			set { username = value; }
		}

		public string Pwd
		{
			get { return pwd; }
			set { pwd = value; }
		}

		public string PwdConfirmation
		{
			get { return pwdconfirmation; }
			set { pwdconfirmation = value; }
		}

		public string[] Interests
		{
			get { return interests; }
			set { interests = value; }
		}
	}
}
