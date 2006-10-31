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
	using System.Collections;

	using Castle.ActiveRecord;

	[ActiveRecord]
	public class Blog : ActiveRecordBase
	{
		private int id;
		private String name;
		private String author;
		private IList posts = new ArrayList();

		public Blog()
		{
		}

		public Blog(String name)
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
		public String Name
		{
			get { return name; }
			set { name = value; }
		}

		[Property]
		public String Author
		{
			get { return author; }
			set { author = value; }
		}

		[HasMany(typeof(Post), 
			Table="Posts", ColumnKey="blogid", 
			Inverse=true, Cascade=ManyRelationCascadeEnum.AllDeleteOrphan)]
		public IList Posts
		{
			get { return posts; }
			set { posts = value; }
		}

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll(typeof(Blog));
		}

		public static Blog[] FindAll()
		{
			return (Blog[]) ActiveRecordBase.FindAll(typeof(Blog));
		}

		public static Blog Find(int id)
		{
			return (Blog) ActiveRecordBase.FindByPrimaryKey(typeof(Blog), id);
		}
	}
}