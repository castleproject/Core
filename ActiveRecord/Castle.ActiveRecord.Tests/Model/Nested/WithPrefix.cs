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

namespace Castle.ActiveRecord.Tests.Model.Nested
{
	using System;

	[ActiveRecord]
	public class NestedWithPrefix : ActiveRecordBase
	{
		private int id;
		private Name self, other;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id
		{
			get { return this.id; }
			set { this.id = value; }
		}

		[Nested]
		public Name Self
		{
			get { return this.self; }
			set { this.self = value; }
		}

		[Nested(ColumnPrefix = "othername_")]
		public Name Other
		{
			get { return this.other; }
			set { this.other = value; }
		}

		#region Public Operations

		public static void DeleteAll()
		{
			DeleteAll(typeof(NestedWithPrefix));
		}

		public static NestedWithPrefix[] FindAll()
		{
			return (NestedWithPrefix[]) FindAll(typeof(NestedWithPrefix));
		}

		public static NestedWithPrefix Find(int id)
		{
			return (NestedWithPrefix) FindByPrimaryKey(typeof(NestedWithPrefix), id);
		}

		#endregion
	}
	
	public class Name
	{
		private String first, last;

		[Property]
		public string First
		{
			get { return this.first; }
			set { this.first = value; }
		}

		[Property("last")]
		public string Last
		{
			get { return this.last; }
			set { this.last = value; }
		}
	}
}