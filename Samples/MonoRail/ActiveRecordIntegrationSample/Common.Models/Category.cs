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

namespace Common.Models
{
	using System;

	using Castle.ActiveRecord;

	[ActiveRecord("TSAS_Categories")]
	public class Category : ActiveRecordBase
	{
		private Guid id;
		private String name;

		public Category()
		{
		}

		public Category(String name)
		{
			this.name = name;
		}

		[PrimaryKey(PrimaryKeyType.Guid)]
		public Guid Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property, ValidateNotEmptyAttribute]
		public String Name
		{
			get { return name; }
			set { name = value; }
		}

		public override string ToString()
		{
			return name;
		}

		public static Category[] FindAll()
		{
			return (Category[]) FindAll(typeof(Category));
		}
	}
}
