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

	/// <summary>
	/// Used when a constraint requires a hilo algorithm 
	/// </summary>
	/// <example><code>
	/// public class Blog : ActiveRecordBase
	/// {
	///		...
	///		
	///		[HasManyAndBelongs/HasMany,
	///		CollectionID(CollectionIDAttribute.HiLo),
	///		Hilo]
	///		public int Id
	///		{
	///			get { return _id; }
	///			set { _id = value; }
	///		}
	/// </code></example>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false), Serializable]
	public class HiloAttribute : Attribute
	{
		private String _column;
		private String _table;
		private int _maxlo;

		/// <summary>
		/// Initializes a new instance of the <see cref="HiloAttribute"/> class.
		/// </summary>
		public HiloAttribute() : this("hi_value", "next_value", 100)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HiloAttribute"/> class.
		/// </summary>
		/// <param name="table">The table.</param>
		/// <param name="column">The column.</param>
		/// <param name="maxlo">The maxlo.</param>
		public HiloAttribute(string  table, String column, int maxlo)
		{
			_column = column;
			_table = table;
			_maxlo = maxlo;
		}

		/// <summary>
		/// Gets or sets the column name
		/// </summary>
		/// <value>The column.</value>
		public String Column
		{
			get { return _column; }
			set { _column = value; }
		}

		/// <summary>
		/// Gets or sets the table name
		/// </summary>
		/// <value>The table.</value>
		public String Table
		{
			get { return _table; }
			set { _table = value; }
		}

		/// <summary>
		/// Gets or sets the max low value
		/// </summary>
		/// <value>The max lo.</value>
		public int MaxLo
		{
			get { return _maxlo; }
			set { _maxlo = value; }
		}
	}
}
