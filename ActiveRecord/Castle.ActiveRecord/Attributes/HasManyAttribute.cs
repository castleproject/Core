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

	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
	public class HasAndBelongsToManyAttribute : RelationAttribute
	{
		private Type _mapType;
		private String _key;
		private String _index;
		private String _table;
		private String _schema;
		private String _lazy;
		private String _inverse;
		private String _cascade;
		private String _sort;
		private String _orderBy;
		private String _where;
		private String _column;
		private String _columnKey;

		public HasAndBelongsToManyAttribute( Type mapType, RelationType relationType ) : base(relationType)
		{
			_mapType = mapType;
		}

		public Type MapType
		{
			get { return _mapType; }
			set { _mapType = value; }
		}

		public String Key
		{
			get { return _key; }
			set { _key = value; }
		}

		public String Column
		{
			get { return _column; }
			set { _column = value; }
		}

		public String ColumnKey
		{
			get { return _columnKey; }
			set { _columnKey = value; }
		}

		public String Index
		{
			get { return _index; }
			set { _index = value; }
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

		public String Lazy
		{
			get { return _lazy; }
			set { _lazy = value; }
		}

		public String Inverse
		{
			get { return _inverse; }
			set { _inverse = value; }
		}

		public String Cascade
		{
			get { return _cascade; }
			set { _cascade = value; }
		}

		public String Sort
		{
			get { return _sort; }
			set { _sort = value; }
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
	}

	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
	public class HasManyAttribute : RelationAttribute
	{
		private Type _mapType;
		private String _key;
		private String _index;
		private String _table;
		private String _schema;
		private String _lazy;
		private String _inverse;
		private String _cascade;
		private String _sort;
		private String _orderBy;
		private String _where;
		private String _column;

		public HasManyAttribute( Type mapType, RelationType relationType ) : base(relationType)
		{
			_mapType = mapType;
		}

		public Type MapType
		{
			get { return _mapType; }
			set { _mapType = value; }
		}

		public String Key
		{
			get { return _key; }
			set { _key = value; }
		}

		public String Column
		{
			get { return _column; }
			set { _column = value; }
		}

		public String Index
		{
			get { return _index; }
			set { _index = value; }
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

		public String Lazy
		{
			get { return _lazy; }
			set { _lazy = value; }
		}

		public String Inverse
		{
			get { return _inverse; }
			set { _inverse = value; }
		}

		public String Cascade
		{
			get { return _cascade; }
			set { _cascade = value; }
		}

		public String Sort
		{
			get { return _sort; }
			set { _sort = value; }
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
	}
}
