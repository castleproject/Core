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


	[AttributeUsage(AttributeTargets.Property)]
	public class PropertyAttribute : Attribute
	{
		private string _column;
		private string _update;
		private string _insert;
		private string _formula;
		private string _length;
		private string _notNull;

		public PropertyAttribute()
		{
		}

		public string NotNull
		{
			get { return _notNull; }
			set { _notNull = value; }
		}

		public string Length
		{
			get { return _length; }
			set { _length = value; }
		}

		public string Column
		{
			get { return _column; }
			set { _column = value; }
		}

		public string Update
		{
			get { return _update; }
			set { _update = value; }
		}

		public string Insert
		{
			get { return _insert; }
			set { _insert = value; }
		}

		public string Formula
		{
			get { return _formula; }
			set { _formula = value; }
		}
	}
}
