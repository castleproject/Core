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

namespace Castle.MonoRail.Framework.Configuration
{
	using System;

	public class SmtpConfig
	{
		private String _host = "localhost";
		private String _username, _password;

		public String Host
		{
			get { return _host; }
			set { _host = value; }
		}

		public String Username
		{
			get { return _username; }
			set { _username = value; }
		}

		public String Password
		{
			get { return _password; }
			set { _password = value; }
		}
	}
}
