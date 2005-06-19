// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

	public enum RelationType
	{
		Guess,
		Bag,
		Set,
		IdBag,
		// Map,
		// List,
		// Array,
		// PrimitiveArray
	}

	[AttributeUsage(AttributeTargets.Property)]
	public abstract class RelationAttribute : Attribute
	{
		protected Type _mapType;
		protected String _table;
		protected String _schema;
		protected String _orderBy;
		protected String _where;
		protected String _sort;
		protected bool _lazy;
		protected bool _inverse;
		protected ManyRelationCascadeEnum _cascade = ManyRelationCascadeEnum.None;
		protected RelationType _relType = RelationType.Guess;

		public RelationType RelationType
		{
			get { return _relType; }
			set { _relType = value; }
		}

		public Type MapType
		{
			get { return _mapType; }
			set { _mapType = value; }
		}

		public String Table
		{
			get { return _table; }
			set { _table = value; }
		}

		public String Schema
		{
			get { return _schema; }
			set { _schema = value; }
		}

		public bool Lazy
		{
			get { return _lazy; }
			set { _lazy = value; }
		}

		public bool Inverse
		{
			get { return _inverse; }
			set { _inverse = value; }
		}
	
		public ManyRelationCascadeEnum Cascade
		{
			get { return _cascade; }
			set { _cascade = value; }
		}

		public String OrderBy
		{
			get { return _orderBy; }
			set { _orderBy = value; }
		}

		public String Where
		{
			get { return _where; }
			set { _where = value; }
		}

		/// <summary>
		/// Only used with sets
		/// </summary>
		public String Sort
		{
			get { return _sort; }
			set { _sort = value; }
		}
	}
}