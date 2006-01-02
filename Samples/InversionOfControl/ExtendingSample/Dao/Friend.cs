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

namespace ExtendingSample.Dao
{
	using System;

	/// <summary>
	/// Summary description for Friend.
	/// </summary>
	public class Friend
	{
		private String _name;
		private String _email;

		public Friend(String name, String email)
		{
			_name = name;
			_email = email;
		}

		public String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		public String Email
		{
			get { return _email; }
			set { _email = value; }
		}
	}
}
