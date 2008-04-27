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
	/// Maps a many to many association with an association table.
	/// </summary>
	/// <example><code>
	/// public class Company : ActiveRecordBase
	/// {
	///   ...
	///   
	///   [HasAndBelongsToMany( typeof(Person), RelationType.Bag, Table="PeopleCompanies", ColumnRef="person_id", ColumnKey="company_id" )]
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
		private String[] compositeKeyColumnRefs;
		private String columnKey;
		private String[] compositeKeyColumnKeys;
		private FetchEnum fetchMethod = FetchEnum.Unspecified;
		private Type customCollectionType;

		/// <summary>
		/// Initializes a new instance of the <see cref="HasAndBelongsToManyAttribute"/> class.
		/// </summary>
		/// <param name="mapType">Type of the map.</param>
		public HasAndBelongsToManyAttribute( Type mapType )
		{
			this.mapType = mapType;
		}


		/// <summary>
		/// Initializes a new instance of the <see cref="HasAndBelongsToManyAttribute"/> class.
		/// </summary>
		public HasAndBelongsToManyAttribute()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HasAndBelongsToManyAttribute"/> class.
		/// </summary>
		/// <param name="mapType">Type of the map.</param>
		/// <param name="type">The type.</param>
		public HasAndBelongsToManyAttribute( Type mapType, RelationType type ) : this(mapType)
		{
			base.relType = type;
		}

		/// <summary>
		/// Gets or sets the column that represent the other side on the assoication table
		/// </summary>
		/// <value>The column ref.</value>
		public String ColumnRef
		{
			get { return columnRef; }
			set { columnRef = value; }
		}

		/// <summary>
		/// Gets or sets the composite key columns that represent the other side on the assoication table
		/// </summary>
		/// <value>The composite key column refs.</value>
		public String[] CompositeKeyColumnRefs
		{
			get { return compositeKeyColumnRefs; }
			set { compositeKeyColumnRefs = value; }
		}

		/// <summary>
		/// Gets or sets the key column name
		/// </summary>
		/// <value>The column key.</value>
		public String ColumnKey
		{
			get { return columnKey; }
			set { columnKey = value; }
		}

		/// <summary>
		/// Gets or sets the composite key columns names.
		/// </summary>
		/// <value>The composite key column keys.</value>
		public String[] CompositeKeyColumnKeys
		{
			get { return compositeKeyColumnKeys; }
			set { compositeKeyColumnKeys = value; }
		}

		/// <summary>
		/// Chooses between outer-join fetching
		/// or sequential select fetching.
		/// </summary>
		public FetchEnum Fetch
		{
			get { return fetchMethod; }
			set { fetchMethod = value; }
		}

		/// <summary>
		/// Provides a custom collection type.
		/// </summary>
		public Type CollectionType
		{
			get { return customCollectionType; }
			set { customCollectionType = value; }
		}
	}
}
