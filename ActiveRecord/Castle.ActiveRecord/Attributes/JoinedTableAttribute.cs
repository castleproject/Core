// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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
	/// Associates another table with the mapping.
	/// </summary>
	/// <example>
	/// <code>
	/// [JoinedTable("tb_Address")]
	/// public class Order : ActiveRecordBase
	/// {
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true), Serializable]
	public class JoinedTableAttribute : Attribute
	{
		private readonly String table;
		private String schema;
		private String column;
		private FetchEnum fetchMethod = FetchEnum.Unspecified;
		private bool inverse;
		private bool optional;

		/// <summary>
		///  Joins the specified table with the target type.
		/// </summary>
		/// <param name="table"></param>
		public JoinedTableAttribute(String table)
		{
			this.table = table;
		}

		/// <summary>
		/// Gets or sets the table name joined with the type.
		/// </summary>
		public String Table
		{
			get { return table; }
		}

		/// <summary>
		/// Gets or sets the schema name of the joined table.
		/// </summary>
		public String Schema
		{
			get { return schema; }
			set { schema = value; }
		}

		/// <summary>
		/// Defines the column used for joining (usually a foreign key)
		/// </summary>
		public String Column
		{
			get { return column; }
			set { column = value; }
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
		/// Gets or sets a value indicating whether this <see cref="JoinedTableAttribute"/> is inverse.
		/// </summary>
		/// <value><c>true</c> if inverse; otherwise, <c>false</c>.</value>
		public bool Inverse
		{
			get { return inverse; }
			set { inverse = value; }
		}

		/// <summary>
		/// Determines if the join is optional.
		/// <value><c>true</c> if optional; otherwise, <c>false</c>.</value>
		/// </summary>
		public bool Optional
		{
			get { return optional; }
			set { optional = value; }
		}
	}
}
