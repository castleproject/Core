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

namespace Castle.Facilities.ActiveRecord
{
	using System;

	/// <summary>
	/// Summary description for HasManyAttribute.
	/// </summary>
	[AttributeUsage(AttributeTargets.Property)]
	public class HasManyAttribute : RelationAttribute
	{
		private Type _mapType;
		private string _key;
		private string _index;
		private string _table;
		private string _schema;
		private string _lazy;
		private string _inverse;
		private string _cascade;
		private string _sort;
		private string _orderBy;
		private string _where;

		public HasManyAttribute()
		{
		}

		public Type MapType
		{
			get { return _mapType; }
			set { _mapType = value; }
		}

		public string Key
		{
			get { return _key; }
			set { _key = value; }
		}

		public string Index
		{
			get { return _index; }
			set { _index = value; }
		}

		public string Table
		{
			get { return _table; }
			set { _table = value; }
		}

		public string Schema
		{
			get { return _schema; }
			set { _schema = value; }
		}

		public string Lazy
		{
			get { return _lazy; }
			set { _lazy = value; }
		}

		public string Inverse
		{
			get { return _inverse; }
			set { _inverse = value; }
		}

		public string Cascade
		{
			get { return _cascade; }
			set { _cascade = value; }
		}

		public string Sort
		{
			get { return _sort; }
			set { _sort = value; }
		}

		public string OrderBy
		{
			get { return _orderBy; }
			set { _orderBy = value; }
		}

		public string Where
		{
			get { return _where; }
			set { _where = value; }
		}
	}
}
