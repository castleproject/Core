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
	/// Maps a standard column of the table.
	/// </summary>
	/// <example>
	/// In the following example, the column is also
	/// called 'name', so you don't have to specify.
	/// <code>
	/// public class Blog : ActiveRecordBase
	/// {
	///		...
	///		
	///		[Property]
	///		public int Name
	///		{
	///			get { return _id; }
	///			set { _id = value; }
	///		}
	///	</code>
	/// To map a column name, use 
	/// <code>
	///		[Property("blog_name")]
	///		public int Name
	///		{
	///			get { return _id; }
	///			set { _id = value; }
	///		}
	///	</code>
	/// </example>
	[AttributeUsage(AttributeTargets.Property)]
	public class PropertyAttribute : Attribute
	{
		private String _column;
		private String _formula;
		private String _type;
		private String _unsavedValue;
		private int _length;
		private bool _notNull;
		private bool _unique;
		private bool _update = true;
		private bool _insert = true;

		public PropertyAttribute()
		{
		}

		public PropertyAttribute(String column)
		{
			_column = column;
		}

		public PropertyAttribute(String column, String type)
		{
			_column = column;
			_type = type;
		}

		public bool NotNull
		{
			get { return _notNull; }
			set { _notNull = value; }
		}

		public int Length
		{
			get { return _length; }
			set { _length = value; }
		}

		public String Column
		{
			get { return _column; }
			set { _column = value; }
		}

		public bool Update
		{
			get { return _update; }
			set { _update = value; }
		}

		public bool Insert
		{
			get { return _insert; }
			set { _insert = value; }
		}

		public bool Unique
		{
			get { return _unique; }
			set { _unique = value; }
		}

		public String Formula
		{
			get { return _formula; }
			set { _formula = value; }
		}

		public String ColumnType
		{
			get { return _type; }
			set { _type = value; }
		}

		public String UnsavedValue
		{
			get { return _unsavedValue; }
			set { _unsavedValue = value; }
		}
	}
}
