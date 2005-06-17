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
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
	public class HasAndBelongsToManyAttribute : RelationAttribute
	{
		private Type _mapType;
		private String _key;
		private String _index;
		private String _table;
		private String _schema;
		private bool _lazy;
		private bool _inverse;
		private String _cascade;
//		private String _sort;
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

		public String Cascade
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
	}
}
