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

namespace BinderSample.Web.Model
{
	using System;

	using Castle.ActiveRecord;

	[ActiveRecord]
	public class Book : ActiveRecordBase
	{
		private int id;
		private String name;
		private String author;
		private Publisher publisher;

		public Book()
		{
		}

		public Book(string name)
		{
			this.name = name;
		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		[Property]
		public string Author
		{
			get { return author; }
			set { author = value; }
		}

		[BelongsTo( "publisher_id" )]
		public Publisher Publisher
		{
			get { return publisher; }
			set { publisher = value; }
		}

		public static Book Find(int id)
		{
			return (Book) FindByPrimaryKey(typeof(Book), id);
		}

		public override string ToString()
		{
			return "" + name;
		}
	}
}
