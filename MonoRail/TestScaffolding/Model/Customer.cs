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

namespace TestScaffolding.Model
{
	using System;

	using Castle.ActiveRecord;

	public enum CallBy
	{
		Mr,
		Ms,
		Lord,
		Duke
	}

	[ActiveRecord(DiscriminatorColumn="type", DiscriminatorValue="customer")]
	public class Customer : Person
	{
		private ContactInfo info;
//		private CallBy callby;

		public Customer()
		{
		}

		[Nested]
		public ContactInfo Contact
		{
			get { return info; }
			set { info = value; }
		}

//		[Property("call", "Int32")]
//		public CallBy Callby
//		{
//			get { return callby; }
//			set { callby = value; }
//		}
	}

	public class ContactInfo
	{
		private string email1;
		private string email2;
		private string phone1;
		private string phone2;

		[Property]
		public string Email1
		{
			get { return email1; }
			set { email1 = value; }
		}

		[Property]
		public string Email2
		{
			get { return email2; }
			set { email2 = value; }
		}

		[Property]
		public string Phone1
		{
			get { return phone1; }
			set { phone1 = value; }
		}

		[Property]
		public string Phone2
		{
			get { return phone2; }
			set { phone2 = value; }
		}
	}
}
