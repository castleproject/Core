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

namespace Castle.ActiveRecord.Tests
{
	using System;
	using System.Collections;

	using NHibernate;

	using Castle.ActiveRecord.Framework;


	[ActiveRecord("BlogTable")]
	public class BlogLazy : ActiveRecordBase
	{
		private int _id;
		private String _name;
		private String _author;
		private IList _posts;
		private IList _publishedposts;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id
		{
			get { return _id; }
			set { _id = value; }
		}

		[Property]
		public String Name
		{
			get { return _name; }
			set { _name = value; }
		}

		[Property]
		public String Author
		{
			get { return _author; }
			set { _author = value; }
		}

		[HasMany(typeof(PostLazy), Table="Posts", ColumnKey="blogid", Lazy=true)]
		public IList Posts
		{
			get { return _posts; }
			set { _posts = value; }
		}

		[HasMany(typeof(PostLazy), Table="Posts", ColumnKey="blogid", Where="published = 1", Lazy=true)]
		public IList PublishedPosts
		{
			get { return _publishedposts; }
			set { _publishedposts = value; }
		}

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll( typeof(BlogLazy) );
		}

		public static BlogLazy[] FindAll()
		{
			return (BlogLazy[]) ActiveRecordBase.FindAll( typeof(BlogLazy) );
		}

		public static BlogLazy Find(int id)
		{
			return (BlogLazy) ActiveRecordBase.FindByPrimaryKey( typeof(BlogLazy), id );
		}

		/// <summary>
		/// We make it visible only to for test cases' assertions 
		/// </summary>
		public static ISessionFactoryHolder Holder
		{
			get { return ActiveRecordMediator.GetSessionFactoryHolder(); }
		}

		public void CustomAction()
		{
			Execute(new NHibernateDelegate(MyCustomMethod));
		}

		private object MyCustomMethod(ISession session, object blogInstance)
		{
			session.Delete(blogInstance);
			session.Flush();
			
			return null;
		}
	}
}
