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

namespace Castle.ActiveRecord.Tests.Model.CompositeUserType
{
	using System;
	using System.Data;
	using NHibernate;
	using NHibernate.Engine;
	using NHibernate.Type;
	using NHibernate.UserTypes;

	[ActiveRecord(DiscriminatorColumn = "CitizenType", DiscriminatorType = "byte", DiscriminatorValue = "1")]
	public class Citizen : ActiveRecordBase<Citizen>
	{
		private int id;
		private string[] name;
		private string[] manufacturerName;
		private string[] inventorsName;
		[CompositeUserType(typeof(DoubleStringType), 
			new string[] { "Seller_FirstName", "Seller_LastName" })]
		public string[] SellersName;
		
		[PrimaryKey()]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[CompositeUserType(typeof(DoubleStringType), new string[] {"Product_FirstName", "Product_LastName"})]
		public string[] Name
		{
			get { return name; }
			set { name = value; }
		}

		[CompositeUserType(
			typeof(DoubleStringType), 
			new string[]{"Manufacturer_FirstName", "Manufacturer_LastName"}, 
			Length = new int[] {4, 5} )]
		public string[] ManufacturerName
		{
			get { return manufacturerName; }
			set { manufacturerName = value; }
		}

		[CompositeUserType(
			"Castle.ActiveRecord.Tests.Model.CompositeUserType.DoubleStringType, Castle.ActiveRecord.Tests",
			new string[] { "Inventors_FirstName", "Inventors_LastName" })]
		public string[] InventorsName
		{
			get { return inventorsName; }
			set { inventorsName = value; }
		}


	}

	[ActiveRecord(DiscriminatorValue = "2")]
	public class SecondCitizen : Citizen
	{
	}

	[ActiveRecord]
	public class NestedCitizen : ActiveRecordBase<NestedCitizen>
	{
		private int id;
		private Names name = new Names();
		private Names manufacturerName = new Names();

		[PrimaryKey()]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Nested]
		public Names Name
		{
			get { return name; }
			set { name = value; }
		}

		[Nested(ColumnPrefix = "Manufacturer_")]
		public Names ManufacturerName
		{
			get { return manufacturerName; }
			set { manufacturerName = value; }
		}
	}

	public class Names
	{
		private string[] name;

		[CompositeUserType(typeof(DoubleStringType), new string[] { "FirstName", "LastName" })]
		public string[] Name
		{
			get { return name; }
			set { name = value; }
		}
	}
}
