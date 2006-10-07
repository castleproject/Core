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
	/// Define the relation type for a relation.
	/// </summary>
	[Serializable]
	public enum RelationType
	{
		/// <summary>
		/// Let Active Record guess what is the type of the relation.
		/// </summary>
		Guess,
		/// <summary>
		/// An bag of items (allow duplicates)
		/// </summary>
		Bag,
		/// <summary>
		/// A set of unique items
		/// </summary>
		Set,
		/// <summary>
		/// A bag of items with id
		/// </summary>
		IdBag,
		/// <summary>
		/// Map of key/value pairs (IDictionary)
		/// </summary>
		Map,
		/// <summary>
		/// A list of items - position in the list has meaning
		/// </summary>
		List
	}

	/// <summary>
	/// Base class to define common relation information
	/// </summary>
	[AttributeUsage(AttributeTargets.Property), Serializable]
	public abstract class RelationAttribute : BaseAttribute
	{
		internal Type mapType;
		internal String table;
		internal String schema;
		internal String orderBy;
		internal String where;
		internal String sort;
		internal String index;
		internal String indexType;
		internal String element;
		internal bool lazy;
		internal bool inverse;
		internal ManyRelationCascadeEnum cascade = ManyRelationCascadeEnum.None;
		internal RelationType relType = RelationType.Guess;

		/// <summary>
		/// Gets or sets the type of the relation.
		/// </summary>
		/// <value>The type of the relation.</value>
		public RelationType RelationType
		{
			get { return relType; }
			set { relType = value; }
		}

		/// <summary>
		/// Gets or sets the type of the map.
		/// </summary>
		/// <value>The type of the map.</value>
		public Type MapType
		{
			get { return mapType; }
			set { mapType = value; }
		}

		/// <summary>
		/// Gets or sets the table for this relation
		/// </summary>
		/// <value>The table.</value>
		public String Table
		{
			get { return table; }
			set { table = value; }
		}

		/// <summary>
		/// Gets or sets the schema for this relation (dbo., etc)
		/// </summary>
		/// <value>The schema.</value>
		public String Schema
		{
			get { return schema; }
			set { schema = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="RelationAttribute"/> is lazy.
		/// </summary>
		/// <value><c>true</c> if lazy; otherwise, <c>false</c>.</value>
		public bool Lazy
		{
			get { return lazy; }
			set { lazy = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="RelationAttribute"/> is inverse.
		/// </summary>
		/// <value><c>true</c> if inverse; otherwise, <c>false</c>.</value>
		public bool Inverse
		{
			get { return inverse; }
			set { inverse = value; }
		}

		/// <summary>
		/// Gets or sets the cascade options for this <see cref="RelationAttribute"/>
		/// </summary>
		/// <value>The cascade.</value>
		public ManyRelationCascadeEnum Cascade
		{
			get { return cascade; }
			set { cascade = value; }
		}

		/// <summary>
		/// Gets or sets the order by clause for this relation
		/// </summary>
		public String OrderBy
		{
			get { return orderBy; }
			set { orderBy = value; }
		}

		/// <summary>
		/// Gets or sets the where clause for this relation
		/// </summary>
		public String Where
		{
			get { return where; }
			set { where = value; }
		}

		/// <summary>
		/// Only used with sets
		/// </summary>
		public String Sort
		{
			get { return sort; }
			set { sort = value; }
		}

		/// <summary>
		/// Only used with maps or list
		/// </summary>
		public string Index
		{
			get { return index; }
			set { index = value; }
		}

		/// <summary>
		/// Only used with maps
		/// </summary>
		public string IndexType
		{
			get { return indexType; }
			set { indexType = value; }
		}

		/// <summary>
		/// Use for simple types.
		/// </summary>
		public string Element
		{
			get { return element; }
			set { element = value; }
		}
	}
}
