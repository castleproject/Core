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
	public class Person
	{
		private int id, age;
		private string name;
		private string address;

		public Person()
		{
		}

		public Person(int id, int age, string name, string address)
		{
			this.id = id;
			this.age = age;
			this.name = name;
			this.address = address;
		}

		[ValidateNonEmpty]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[ValidateNonEmpty]
		public int Age
		{
			get { return age; }
			set { age = value; }
		}

		[ValidateNonEmpty]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[ValidateNonEmpty]
		public string Address
		{
			get { return address; }
			set { address = value; }
		}
	}
}