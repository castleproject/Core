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

	/// <summary>
	/// Maps a standard column of the table.
	/// </summary>
	/// <example>
	/// In the following example, the column is also
	/// called 'name', so you don't have to specify.
	/// <code>
	/// public class Blog : ActiveRecordBase
	/// {
	///		[Field]
	///		string name;
	///		
	///		
	///	</code>
	/// </example>
	[AttributeUsage(AttributeTargets.Field), Serializable]
	public class FieldAttribute : WithAccessAttribute
	{
		private String column;
		private String formula;
		private String type;
		private String unsavedValue;
		private int length;
		private bool notNull;
		private bool unique;
		private bool update = true;
		private bool insert = true;
		
		public FieldAttribute() 
		{
			Access = PropertyAccess.Field;
		}

		public FieldAttribute(String column) : this()
		{
			this.column = column;
		}

		public FieldAttribute(String column, String type) : this(column)
		{
			this.type = type;
		}

		public bool NotNull
		{
			get { return notNull; }
			set { notNull = value; }
		}

		public int Length
		{
			get { return length; }
			set { length = value; }
		}

		public String Column
		{
			get { return column; }
			set { column = value; }
		}

		/// <summary>
		/// Set to <c>false</c> to ignore this 
		/// field when updating entities of this ActiveRecord class.
		/// </summary>
		public bool Update
		{
			get { return update; }
			set { update = value; }
		}

		/// <summary>
		/// Set to <c>false</c> to ignore this 
		/// field when inserting entities of this ActiveRecord class.
		/// </summary>
		public bool Insert
		{
			get { return insert; }
			set { insert = value; }
		}

		public bool Unique
		{
			get { return unique; }
			set { unique = value; }
		}

		public String Formula
		{
			get { return formula; }
			set { formula = value; }
		}

		public String ColumnType
		{
			get { return type; }
			set { type = value; }
		}

		public String UnsavedValue
		{
			get { return unsavedValue; }
			set { unsavedValue = value; }
		}
	}
}
