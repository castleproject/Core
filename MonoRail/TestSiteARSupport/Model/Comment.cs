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

namespace TestSiteARSupport.Model
{
	using System;
	using Castle.ActiveRecord;

	[ActiveRecord]
	public class Comment : ActiveRecordBase
	{
		private int id;
		private string from;
		private string text;
		private Condition condition;

		public Comment()
		{
		}

		public Comment(string from, string text)
		{
			this.from = from;
			this.text = text;
		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property("`From`")]
		public string From
		{
			get { return from; }
			set { from = value; }
		}

		[Property(ColumnType="StringClob")]
		public string Text
		{
			get { return text; }
			set { text = value; }
		}

		[BelongsTo]
		public Condition Condition
		{
			get { return condition; }
			set { condition = value; }
		}

		public static Comment[] FindAll()
		{
			return (Comment[]) FindAll(typeof(Comment));
		}
	}
}
