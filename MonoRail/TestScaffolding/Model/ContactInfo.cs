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

namespace TestScaffolding.Model
{
	using System;
	using Castle.ActiveRecord;


	public class ContactInfo
	{
		private String telephone1;
		private String telephone2;
		private int countrycode;
		private int regioncode;

		[Property]
		public string Telephone1
		{
			get { return telephone1; }
			set { telephone1 = value; }
		}

		[Property]
		public string Telephone2
		{
			get { return telephone2; }
			set { telephone2 = value; }
		}

		[Property]
		public int Countrycode
		{
			get { return countrycode; }
			set { countrycode = value; }
		}

		[Property]
		public int Regioncode
		{
			get { return regioncode; }
			set { regioncode = value; }
		}
	}
}
