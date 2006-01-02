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

namespace TableHierarchySample
{
	using System;
	using System.Collections;

	using Castle.ActiveRecord;


	[ActiveRecord("Companies", DiscriminatorColumn="type", DiscriminatorType="String", DiscriminatorValue="company")]
	public class Company : ActiveRecordBase
	{
		private int id;
		private String name;
		private IList _people;

		public Company()
		{
		}

		public Company(string name)
		{
			this.name = name;
		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public String Name
		{
			get { return name; }
			set { name = value; }
		}

		[HasAndBelongsToMany( typeof(Person), Table="PeopleCompanies", ColumnRef="person_id", ColumnKey="company_id" )]
		public IList People
		{
			get { return _people; }
			set { _people = value; }
		}

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll( typeof(Company) );
		}

		public static Company[] FindAll()
		{
			return (Company[]) ActiveRecordBase.FindAll( typeof(Company) );
		}

		public static Company Find(int id)
		{
			return (Company) ActiveRecordBase.FindByPrimaryKey( typeof(Company), id );
		}
	}
}
