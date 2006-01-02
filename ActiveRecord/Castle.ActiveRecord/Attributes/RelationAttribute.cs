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

	[Serializable]
	public enum RelationType
	{
		Guess,
		Bag,
		Set,
		IdBag,
		Map
	}

	[AttributeUsage(AttributeTargets.Property), Serializable]
	public abstract class RelationAttribute : BaseAttribute
	{
		protected Type mapType;
		protected String table;
		protected String schema;
		protected String orderBy;
		protected String where;
		protected String sort;
		protected String index;
		protected String indexType;
		protected bool lazy;
		protected bool inverse;
		protected ManyRelationCascadeEnum cascade = ManyRelationCascadeEnum.None;
		protected RelationType relType = RelationType.Guess;

		public RelationType RelationType
		{
			get { return relType; }
			set { relType = value; }
		}

		public Type MapType
		{
			get { return mapType; }
			set { mapType = value; }
		}

		public String Table
		{
			get { return table; }
			set { table = value; }
		}

		public String Schema
		{
			get { return schema; }
			set { schema = value; }
		}

		public bool Lazy
		{
			get { return lazy; }
			set { lazy = value; }
		}

		public bool Inverse
		{
			get { return inverse; }
			set { inverse = value; }
		}
	
		public ManyRelationCascadeEnum Cascade
		{
			get { return cascade; }
			set { cascade = value; }
		}

		public String OrderBy
		{
			get { return orderBy; }
			set { orderBy = value; }
		}

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
		/// Only used with maps
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
	}
}