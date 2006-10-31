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

namespace BlogSample
{
	using System;

	using Castle.ActiveRecord;


	[ActiveRecord]
	public class Post : ActiveRecordBase
	{
		private int id;
		private String title;
		private String contents;
		private String category;
		private DateTime created;
		private bool published;
		private Blog blog;

		public Post()
		{
			created = DateTime.Now;
		}

		public Post(Blog blog, String title, String contents, String category) : this()
		{
			this.blog = blog;
			this.title = title;
			this.contents = contents;
			this.category = category;
		}

		[PrimaryKey]
		public int Id
		{
			get { return id; }
			set { id = value; }
		}

		[Property]
		public String Title
		{
			get { return title; }
			set { title = value; }
		}

		[Property(ColumnType="StringClob")]
		public String Contents
		{
			get { return contents; }
			set { contents = value; }
		}

		[Property]
		public String Category
		{
			get { return category; }
			set { category = value; }
		}

		[BelongsTo("blogid")]
		public Blog Blog
		{
			get { return blog; }
			set { blog = value; }
		}

		[Property("created")]
		public DateTime Created
		{
			get { return created; }
			set { created = value; }
		}

		[Property("published")]
		public bool Published
		{
			get { return published; }
			set { published = value; }
		}

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll( typeof(Post) );
		}

		public static Post[] FindAll()
		{
			return (Post[]) ActiveRecordBase.FindAll( typeof(Post) );
		}
	}
}
