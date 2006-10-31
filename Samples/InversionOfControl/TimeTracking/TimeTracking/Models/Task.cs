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

namespace TimeTracking.Models
{
	using System;

	public class Task
	{
		private int id;
		private string name;
		private string comment;

		public Task()
		{
		}

		public Task(string name, string comment)
		{
			this.name = name;
			this.comment = comment;
		}

		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public string Comment
		{
			get { return comment; }
			set { comment = value; }
		}

		public override string ToString()
		{
			return name;
		}
	}
}
