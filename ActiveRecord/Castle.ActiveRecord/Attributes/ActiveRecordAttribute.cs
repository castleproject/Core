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
	[AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
	public class ActiveRecordAttribute : Attribute
	{ 
		private String _table;
		private String _schema;
		private String _discriminatorType;
		private String _discriminatorValue;
		private String _discriminatorColumn;
		private bool _proxy;

		public ActiveRecordAttribute()
		{
			
		}

		public ActiveRecordAttribute(String table)
		{
			_table = table;
		}

		public ActiveRecordAttribute(String table, String schema)
		{
			_table = table;
			_schema = schema;
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

		public bool Proxy
		{
			get { return _proxy; }
			set { _proxy = value; }
		}

		/// <summary>
		/// Gets or sets the Discriminator column for
		/// a table inheritance modeling
		/// </summary>
		public String DiscriminatorColumn
		{
			get { return _discriminatorColumn; }
			set { _discriminatorColumn = value; }
		}

		/// <summary>
		/// Gets or sets the column type (like string or integer)
		/// for the discriminator column
		/// </summary>
		public String DiscriminatorType
		{
			get { return _discriminatorType; }
			set { _discriminatorType = value; }
		}

		/// <summary>
		/// Gets or sets the value that represents the
		/// target class on the discriminator column
		/// </summary>
		public String DiscriminatorValue
		{
			get { return _discriminatorValue; }
			set { _discriminatorValue = value; }
		}
	}
}
