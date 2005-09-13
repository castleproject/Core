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
	/// Associate meta information related to the
	/// desired table mapping.
	/// </summary>
	/// <example>
	/// <code>
	/// [ActiveRecord("tb_Order")]
	/// public class Order : ActiveRecordBase
	/// {
	/// }
	/// </code>
	/// </example>
	/// <remarks>
	/// If no table is specified, the class name 
	/// is used as table name
	/// </remarks>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false), Serializable]
	public class ActiveRecordAttribute : BaseAttribute
	{ 
		private String table;
		private String schema;
		private String discriminatorType;
		private String discriminatorValue;
		private String discriminatorColumn;
		private String where;
		private Type proxy;
		private bool lazy;

		/// <summary>
		/// Uses the class name as table name
		/// </summary>
		public ActiveRecordAttribute()
		{
			
		}

		/// <summary>
		/// Associates the specified table with the target type
		/// </summary>
		/// <param name="table"></param>
		public ActiveRecordAttribute(String table)
		{
			this.table = table;
		}

		/// <summary>
		/// Associates the specified table and schema with the target type
		/// </summary>
		public ActiveRecordAttribute(String table, String schema)
		{
			this.table = table;
			this.schema = schema;
		}

		/// <summary>
		/// Gets or sets the table name associated with the type
		/// </summary>
		public String Table
		{
			get { return table; }
			set { table = value; }
		}

		/// <summary>
		/// Gets or sets the schema name associated with the type
		/// </summary>
		public String Schema
		{
			get { return schema; }
			set { schema = value; }
		}

		/// <summary>
		/// Associates a proxy type with the target type
		/// </summary>
		public Type Proxy
		{
			get { return proxy; }
			set { proxy = value; }
		}

		/// <summary>
		/// Gets or sets the Discriminator column for
		/// a table inheritance modeling
		/// </summary>
		public String DiscriminatorColumn
		{
			get { return discriminatorColumn; }
			set { discriminatorColumn = value; }
		}

		/// <summary>
		/// Gets or sets the column type (like string or integer)
		/// for the discriminator column
		/// </summary>
		public String DiscriminatorType
		{
			get { return discriminatorType; }
			set { discriminatorType = value; }
		}

		/// <summary>
		/// Gets or sets the value that represents the
		/// target class on the discriminator column
		/// </summary>
		public String DiscriminatorValue
		{
			get { return discriminatorValue; }
			set { discriminatorValue = value; }
		}

		/// <summary>
		/// SQL condition to retrieve objects
		/// </summary>
		public String Where
		{
			get { return where; }
			set { where = value; }
		}

		/// <summary>
		/// Enable lazy loading for the type
		/// </summary>
		public bool Lazy
		{
			get { return lazy; }
			set { lazy = value; }
		}
	}
}
