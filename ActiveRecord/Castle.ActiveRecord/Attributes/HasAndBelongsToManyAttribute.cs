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

namespace Castle.ActiveRecord
{
	using System;

	/// <summary>
	/// Maps a many to many association with an association table.
	/// </summary>
	/// <example><code>
	/// public class Company : ActiveRecordBase
	/// {
	///   ...
	///   
	///   [HasAndBelongsToMany( typeof(Person), RelationType.Bag, Table="PeopleCompanies", Column="person_id", ColumnKey="company_id" )]
	///   public IList People
	///   {
	///   	get { return _people; }
	///   	set { _people = value; }
	///   }
	/// }
	/// </code></example>
	/// <remarks>The <see cref="ColumnKey"/> must specify the key on the 
	/// association table that points to the primary key of this class. In 
	/// the example, 'company_id' points to 'Company'.
	/// </remarks>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false), Serializable]
	public class HasAndBelongsToManyAttribute : RelationAttribute
	{
		private String columnRef;	
		private String columnKey;

		public HasAndBelongsToManyAttribute( Type mapType )
		{
			this.mapType = mapType;
		}

		public HasAndBelongsToManyAttribute( Type mapType, RelationType type ) : this(mapType)
		{
			base.relType = type;
		}

		public String ColumnRef
		{
			get { return columnRef; }
			set { columnRef = value; }
		}

		public String ColumnKey
		{
			get { return columnKey; }
			set { columnKey = value; }
		}
	}
}
