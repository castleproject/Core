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

namespace Castle.ActiveRecord
{
	using System;

	/// <summary>
	/// Maps a property of a child object to its parent object.
	/// </summary>
	/// <example>
	/// The following code illustrates the use of a 
	/// parent <c>Company</c> class
	/// <code>
	///     public class PostalAddress
	/// 	{
	///         private Company _company;
	/// 		private String _address;
	/// 		private String _city;
	/// 		private String _state;
	/// 		private String _zipcode;
	/// 	
	///         [Parent]
	///         public Company Parent
	///         {
	///             get { return _company; }
	///             set { _company = value; }
	///         }
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
	///
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
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false), Serializable]
	public class NestedParentReferenceAttribute : Attribute
	{
		/// <summary>
		/// Informs ActiveRecord that the marked property is the parent of a nested element
		/// </summary>
		public NestedParentReferenceAttribute() {}
    }
}
