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


	[AttributeUsage(AttributeTargets.Property)]
	public class PropertyAttribute : Attribute
	{
		private String _column;
		private String _update;
		private String _insert;
		private String _formula;
		private String _type;
		private int _length;
		private bool _notNull;

		public PropertyAttribute()
		{
		}

		public PropertyAttribute(String column)
		{
			_column = column;
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

		public String Update
		{
			get { return _update; }
			set { _update = value; }
		}

		public String Insert
		{
			get { return _insert; }
			set { _insert = value; }
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
	}
}
