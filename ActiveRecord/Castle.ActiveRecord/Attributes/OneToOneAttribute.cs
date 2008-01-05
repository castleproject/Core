// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord
{
	using System;
	
	/// <summary>
	/// Associates a foreign table where the current class
	/// and the target class share their primary key.
	/// </summary>
	/// <example>
	/// The following code exemplifies two classes that maps 
	/// to two tables sharing the primary key:
	/// <code>
	/// 	[ActiveRecord("Employee")]
	/// 	public class Employee : ActiveRecordBase
	/// 	{
	/// 		private int id;
	/// 		private Award award;
	/// 	
	/// 		[PrimaryKey(PrimaryKeyType.Native, "EmployeeID")]
	/// 		public int ID
	/// 		{
	/// 			get { return this.id; }
	/// 			set { this.id = value; }
	/// 		}
	/// 	
	/// 		[OneToOne]
	/// 		public Award Award
	/// 		{
	/// 			get { return this.award; }
	/// 			set { this.award = value; }
	/// 		}
	/// 	}
	/// 	
	/// 	[ActiveRecord("Award")]
	/// 	public class Award : ActiveRecordBase
	/// 	{
	/// 		private Employee employee;
	/// 		private int id;
	/// 	
	/// 		public Award()
	/// 		{
	/// 		}
	/// 	
	/// 		public Award(Employee employee)
	/// 		{
	/// 			this.employee = employee;
	/// 		}
	/// 	
	/// 		[OneToOne]
	/// 		public Employee Employee
	/// 		{
	/// 			get { return this.employee; }
	/// 			set { this.employee = value; }
	/// 		}
	/// 	
	/// 		[PrimaryKey(PrimaryKeyType.Foreign, "EmployeeID")]
	/// 		public int ID
	/// 		{
	/// 			get { return this.id; }
	/// 			set { this.id = value; }
	/// 		}
	/// 	
	/// 		public static Award[] FindAll()
	/// 		{
	/// 			return ((Award[]) (ActiveRecordBase.FindAll(typeof(Award))));
	/// 		}
	/// 	
	/// 		public static void DeleteAll()
	/// 		{
	/// 			ActiveRecordBase.DeleteAll( typeof(Award) );
	/// 		}
	/// 	}
	/// 	Employee emp = new Employee();
	/// 	emp.Name = "john doe";
	/// 	emp.Save();
	/// 	
	/// 	Award award = new Award(emp);
	/// 	award.Description = "Invisible employee";
	/// 	award.Save();
	/// </code>
	/// </example>
	/// <remarks>
	/// Usually classes that uses the primary key
	/// generated elsewhere (foreign) uses the PrimaryKey attribute with the
	/// generator type <c>PrimaryKeyType.Foreign</c>
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false), Serializable]
	public class OneToOneAttribute : WithAccessAttribute
	{
		private CascadeEnum cascade = CascadeEnum.None;
		private FetchEnum fetch = FetchEnum.Unspecified;
		private String propertyRef;
		private Type mapType;
		private bool constrained;
		private string foreignKey;

		/// <summary>
		/// Allows one to reference a different type
		/// than the property type
		/// </summary>
		public Type MapType
		{
			get { return mapType; }
			set { mapType = value; }
		}

		/// <summary>
		/// From NHibernate docs: specifies which operations should be 
		/// cascaded from the parent object to the associated object.
		/// </summary>
		public CascadeEnum Cascade
		{
			get { return cascade; }
			set { cascade = value; }
		}

		/// <summary>
		/// From NHibernate docs: Chooses between outer-join fetching 
		/// or sequential select fetching.
		/// </summary>
		/// <remarks>
		/// Defaults to <see cref="FetchEnum.Select"/>
		/// </remarks>
		public FetchEnum Fetch
		{
			get { return fetch; }
			set { fetch = value; }
		}

		/// <summary>
		/// From NHibernate docs: The name of a property of the 
		/// associated class that is joined to the primary key 
		/// of this class. If not specified, the primary key of 
		/// the associated class is used.
		/// </summary>
		public string PropertyRef
		{
			get { return propertyRef; }
			set { propertyRef = value; }
		}

		/// <summary>
		/// From NHibernate docs: specifies that a foreign key 
		/// constraint on the primary key of the mapped table 
		/// references the table of the associated class. 
		/// This option affects the order in which Save() and 
		/// Delete() are cascaded (and is also used by the 
		/// schema export tool).
		/// </summary>
		public bool Constrained
		{
			get { return constrained; }
			set { constrained = value; }
		}

		/// <summary>
		/// Gets or sets the name of the foreign key constraint generated for 
		/// an association. NHibernate will only use the ForeignKey name one 
		/// the inherited class and Constrained = true.
		/// </summary>
		public string ForeignKey
		{
			get { return foreignKey; }
			set { foreignKey = value; }	
		}
	}
}
