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
	/// Maps a one to many association.
	/// </summary>
	/// <example><code>
	/// public class Blog : ActiveRecordBase
	/// {
	///		...
	///		
	/// 	[HasMany(typeof(Post), RelationType.Bag, ColumnKey="Posts", Table="Posts")]
	///		public IList Posts
	///		{
	///			get { return _posts; }
	///			set { _posts = value; }
	///		}
	/// </code></example>
	[AttributeUsage(AttributeTargets.Property), Serializable]
	public class HasManyAttribute : RelationAttribute
	{
		/// <summary>
		/// The key column
		/// Cannot exist if compositeKeyColumns has a value
		/// </summary>
		protected String keyColumn;
		/// <summary>
		/// The composite columns
		/// Cannot exist with keyColumn != null
		/// </summary>
		protected String[] compositeKeyColumns;

		/// <summary>
		/// Whether the target type is for dependent objects or not
		/// </summary>
		protected bool hasDependentObjects;

		/// <summary>
		/// Whether we do outer join fetching for this collection
		/// </summary>
		protected FetchEnum fetchMethod = FetchEnum.Unspecified;

		/// <summary>
		/// Provides a custom collection type.
		/// </summary>
		protected Type customCollectionType;

		/// <summary>
		/// Initializes a new instance of the <see cref="HasManyAttribute"/> class.
		/// </summary>
		public HasManyAttribute()
		{

		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HasManyAttribute"/> class.
		/// </summary>
		/// <param name="mapType">Type of the map.</param>
		public HasManyAttribute(Type mapType)
		{
			base.mapType = mapType;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HasManyAttribute"/> class.
		/// </summary>
		/// <param name="mapType">Type of items in this association</param>
		/// <param name="keyColumn">The key column.</param>
		/// <param name="table">The table.</param>
		public HasManyAttribute(Type mapType, String keyColumn, String table)
		{
			this.keyColumn = keyColumn;
			base.mapType = mapType;
			base.table = table;
		}

		/// <summary>
		/// Gets or sets the key column name.
		/// </summary>
		/// <value>The column key.</value>
		public String ColumnKey
		{
			get { return keyColumn; }
			set { keyColumn = value; }
		}

		/// <summary>
		/// Gets or sets the names of the column in composite key scenarios.
		/// </summary>
		/// <value>The composite key column keys.</value>
		public String[] CompositeKeyColumnKeys
		{
			get { return compositeKeyColumns; }
			set { compositeKeyColumns = value; }
		}

		/// <summary>
		/// Whether or not the target type is a dependent object.
		/// </summary>
		/// <value>true = the target type is a dependent object</value>
		public bool DependentObjects
		{
			get { return hasDependentObjects; }
			set { hasDependentObjects = value; }
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
