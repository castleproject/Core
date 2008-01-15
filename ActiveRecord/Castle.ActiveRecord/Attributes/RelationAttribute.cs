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
	using Castle.ActiveRecord.Framework.Internal;

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
		internal String element, elementType;
		internal bool lazy;
		internal bool lazySpecified = false;
		internal bool inverse;
		internal ManyRelationCascadeEnum cascade = ManyRelationCascadeEnum.None;
		internal RelationType relType = RelationType.Guess;
		internal NotFoundBehaviour notFoundBehaviour = NotFoundBehaviour.Default;
	    private int batchSize = 1;

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
		/// <value>The schema name.</value>
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
			get
			{
				if(lazySpecified)
					return lazy;
				return ActiveRecordModel.isLazyByDefault;
			}
			set
			{
				lazy = value;
				lazySpecified = true;
			}
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
		/// Gets or sets the order by clause for this relation. This is a SQL order, not HQL.
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
		/// Only used with sets. The value can be <c>unsorted</c>, <c>natural</c> and the name of a class implementing <c>System.Collections.IComparer</c>
		/// </summary>
		public String Sort
		{
			get { return sort; }
			set { sort = value; }
		}

		/// <summary>
		/// Only used with maps or lists
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

		/// <summary>
		/// Use for simple types.
		/// </summary>
		public string ElementType
		{
			get { return elementType; }
			set { elementType = value; }
		}

		/// <summary>
		/// Gets or sets the way broken relations are handled.
		/// </summary>
		/// <value>The behaviour.</value>
		public NotFoundBehaviour NotFoundBehaviour
		{
			get { return notFoundBehaviour; }
			set { notFoundBehaviour = value; }
		}

	    /// <summary>
	    /// From NHibernate documentation:
	    /// Specify a "batch size" for batch fetching of collections.
	    /// </summary>
	    public int BatchSize
	    {
	        get { return batchSize; }
	        set { batchSize = value; }
	    }
	}
}
