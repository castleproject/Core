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

#if dotNet2
namespace Castle.ActiveRecord.Tests.Model.GenericModel
{
	using System;
	using System.Collections;

	[ActiveRecord( "Companies", DiscriminatorColumn = "type", DiscriminatorType = "String", DiscriminatorValue = "company" )]
	public class Company : ActiveRecordBase<Company>
	{
		private int id;
		private String name;
		private IList _people;
		private PostalAddress _address;

		public Company()
		{
		}

		public Company( string name )
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

		[Nested]
		public PostalAddress Address
		{
			get { return _address; }
			set { _address = value; }
		}

		[HasAndBelongsToMany( typeof( Person ), RelationType.Bag,
			 Table = "PeopleCompanies", ColumnRef = "person_id", ColumnKey = "company_id" )]
		public IList People
		{
			get { return _people; }
			set { _people = value; }
		}
	}

	public class PostalAddress
	{
		private String _address;
		private String _city;
		private String _state;
		private String _zipcode;

		public PostalAddress()
		{
		}

		public PostalAddress( String address, String city,
			String state, String zipcode )
		{
			_address = address;
			_city = city;
			_state = state;
			_zipcode = zipcode;
		}

		[Property]
		public String Address
		{
			get { return _address; }
			set { _address = value; }
		}

		[Property]
		public String City
		{
			get { return _city; }
			set { _city = value; }
		}

		[Property]
		public String State
		{
			get { return _state; }
			set { _state = value; }
		}

		[Property]
		public String ZipCode
		{
			get { return _zipcode; }
			set { _zipcode = value; }
		}
	}
} 
#endif
