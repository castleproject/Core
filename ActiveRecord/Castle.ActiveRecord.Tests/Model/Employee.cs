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

namespace Castle.ActiveRecord.Tests.Model
{
	using System;

	[ActiveRecord("Employee")]
	public class Employee : ActiveRecordBase
	{
		private int id;
		private string firstName;
		private string lastName;
		private Award award;

		[PrimaryKey(PrimaryKeyType.Native, "EmployeeID")]
		public int ID
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public string FirstName
		{
			get { return firstName; }
			set { firstName = value; }
		}

		[Property]
		public string LastName
		{
			get { return lastName; }
			set { lastName = value; }
		}

		[OneToOne(Constrained=true)]
		public Award Award
		{
			get { return award; }
			set { award = value; }
		}

		public static Employee Find(int id)
		{
			return ((Employee) (ActiveRecordBase.FindByPrimaryKey(typeof(Employee), id)));
		}

		public static Employee[] FindAll()
		{
			return ((Employee[]) (ActiveRecordBase.FindAll(typeof(Employee))));
		}

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll(typeof(Employee));
		}
	}

	[ActiveRecord("Award")]
	public class Award : ActiveRecordBase
	{
		private Employee employee;
		private string description;
		private int id;

		public Award()
		{
		}

		public Award(Employee employee)
		{
			this.employee = employee;
		}

		[OneToOne]
		public Employee Employee
		{
			get { return employee; }
			set { employee = value; }
		}

		[PrimaryKey(PrimaryKeyType.Foreign, "EmployeeID")]
		public int ID
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		public static Award[] FindAll()
		{
			return ((Award[]) (ActiveRecordBase.FindAll(typeof(Award))));
		}

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll(typeof(Award));
		}
	}
}