// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

	[ActiveRecord("People"), 
	 JoinedTable("Addresses", Column = "person_id")]
	public class Person : ActiveRecordBase
	{
		private int _id;
		private String _name;
		private FullName _fullName;
		private String _address;
		private IList _companies;
		private Blog _blog;

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

		[Nested("name_", Table = "Addresses")]
		public FullName FullName
		{
			get { return _fullName; }
			set { _fullName = value; }
		}

		[Property(Table = "Addresses")]
		public string Address
		{
			get { return _address; }
			set { _address = value; }
		}

		[Field(Table = "Addresses")]
		public string City;

		[BelongsTo("blogid", Table = "Addresses")]
		public Blog Blog
		{
			get { return _blog; }
			set { _blog = value; }
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

	public class FullName
	{
		private String _first;
		private String _middle;

		[Property]
		public String First
		{
			get { return _first; }
			set { _first = value; }
		}

		[Property]
		public String Middle
		{
			get { return _middle; }
			set { _middle = value; }
		}

		[Field]
		public String Last;
	}
}
