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
	/// Maps properties of a child object to columns of the table 
	/// of a parent class.
	/// </summary>
	/// <example>
	/// The following code illustrates the use of a 
	/// nested <c>PostalAddress</c> class
	/// <code>
	/// 	[ActiveRecord("Companies")]
	/// 	public class Company : ActiveRecordBase
	/// 	{
	/// 		private int id;
	/// 		private PostalAddress _address;
	/// 	
	/// 		public Company()
	/// 		{
	/// 		}
	/// 	
	/// 		public Company(string name)
	/// 		{
	/// 			this.name = name;
	/// 		}
	/// 	
	/// 		[PrimaryKey]
	/// 		public int Id
	/// 		{
	/// 			get { return id; }
	/// 			set { id = value; }
	/// 		}
	/// 	
	/// 		[Nested]
	/// 		public PostalAddress Address
	/// 		{
	/// 			get { return _address; }
	/// 			set { _address = value; }
	/// 		}
	/// 	}
	/// 	
	/// 	public class PostalAddress
	/// 	{
	/// 		private String _address;
	/// 		private String _city;
	/// 		private String _state;
	/// 		private String _zipcode;
	/// 	
	/// 		[Property]
	/// 		public String Address
	/// 		{
	/// 			get { return _address; }
	/// 			set { _address = value; }
	/// 		}
	/// 	
	/// 		[Property]
	/// 		public String City
	/// 		{
	/// 			get { return _city; }
	/// 			set { _city = value;}
	/// 		}
	/// 	
	/// 		[Property]
	/// 		public String State
	/// 		{
	/// 			get { return _state; }
	/// 			set { _state = value; }
	/// 		}
	/// 	
	/// 		[Property]
	/// 		public String ZipCode
	/// 		{
	/// 			get { return _zipcode; }
	/// 			set { _zipcode = value; }
	/// 		}
	/// 	}
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false), Serializable]
	public class NestedAttribute : WithAccessOptionalTableAttribute
	{
		private bool update = true;
		private bool insert = true;
		private Type mapType;
		private String columnPrefix;
		
		/// <summary>
		/// Informs ActiveRecord that the marked property contains nested elements, contained
		/// in a separate, reusable class.
		/// </summary>
		public NestedAttribute() {}

		/// <summary>
		/// Informs ActiveRecord that the marked property contains nested elements, contained
		/// in a separate, reusable class.
		/// </summary>
		/// <param name="columnPrefix">A prefix to insert before each column in the nested component</param>
		public NestedAttribute(String columnPrefix)
		{
			this.columnPrefix = columnPrefix;
		}

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
		/// Set to <c>false</c> to ignore this nested component when updating entities of this ActiveRecord class.
		/// </summary>
		public bool Update
		{
			get { return update; }
			set { update = value; }
		}

		/// <summary>
		/// Set to <c>false</c> to ignore this nested component when inserting entities of this ActiveRecord class.
		/// </summary>
		public bool Insert
		{
			get { return insert; }
			set { insert = value; }
		}

		/// <summary>
		/// A prefix to insert before each column in the nested component.
		/// </summary>
		public String ColumnPrefix
		{
			get { return this.columnPrefix; }
			set { this.columnPrefix = value; }
		}
	}
}
