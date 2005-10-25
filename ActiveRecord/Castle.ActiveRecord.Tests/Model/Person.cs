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

namespace Castle.ActiveRecord.Tests.Model
{
	using System;
	using System.Collections;

	[ActiveRecord("People")]
	public class Person : ActiveRecordBase
	{
		private int _id;
		private String _name;
		private IList _companies;

		public Person()
		{
			_companies = new ArrayList();
		}

		[PrimaryKey]
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[Property]
		public string Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[HasAndBelongsToMany( typeof(Company), RelationType.Bag, Table="PeopleCompanies", ColumnRef="company_id", ColumnKey="person_id" )]
		public IList Companies
		{
			get { return _companies; }
			set { _companies = value; }
		}

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll( typeof(Person) );
		}

		public static Person[] FindAll()
		{
			return (Person[]) ActiveRecordBase.FindAll( typeof(Person) );
		}

		public static Person Find(int id)
		{
			return (Person) ActiveRecordBase.FindByPrimaryKey( typeof(Person), id );
		}
	}
}
