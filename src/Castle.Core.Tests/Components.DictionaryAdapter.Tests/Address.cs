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

namespace Castle.Components.DictionaryAdapter.Tests
{
	public class Address : InfrastructureStub,  IAddress
	{
		private string line1;
		private string line2;
		private string city;
		private string state;
		private string zipCode;
		private IPhone phone;
		private IPhone mobile;
		private IPhone emergency;

		public string Line1
		{
			get { return line1; }
			set { line1 = value; }
		}

		public string Line2
		{
			get { return line2; }
			set { line2 = value; }
		}

		public string City
		{
			get { return city; }
			set { city = value; }
		}

		public string State
		{
			get { return state; }
			set { state = value; }
		}

		public string ZipCode
		{
			get { return zipCode; }
			set { zipCode = value; }
		}

		public IPhone Phone
		{
			get
			{
				if (phone == null)
				{
					phone = new Phone();
				}
				return phone;
			}
		}

		public IPhone Mobile
		{
			get
			{
				if (mobile == null)
				{
					mobile = new Phone();
				}
				return mobile;
			}
		}

		public IPhone Emergency
		{
			get
			{
				if (emergency == null)
				{
					emergency = new Phone();
				}
				return emergency;
			}
		}
	}
}