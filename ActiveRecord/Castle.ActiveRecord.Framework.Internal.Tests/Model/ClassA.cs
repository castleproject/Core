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

namespace Castle.ActiveRecord.Framework.Internal.Tests.Model
{
	using System;

	[ActiveRecord]
	public class ClassA : ActiveRecordBase
	{
		private int id;
		private String name1;
		private String name2;
		private String name3;
		private String text;

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property(Insert=false, Update=false)]
		public string Name1
		{
			get { return name1; }
			set { name1 = value; }
		}

		[Property]
		public string Name2
		{
			get { return name2; }
			set { name2 = value; }
		}

		[Property(Unique=true, NotNull=true)]
		public string Name3
		{
			get { return name3; }
			set { name3 = value; }
		}

		[Property(ColumnType="StringClob")]
		public string Text
		{
			get { return text; }
			set { text = value; }
		}
	}
}
