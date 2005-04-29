using NHibernate;
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

namespace Castle.ActiveRecord.Tests
{
	using System;
	using System.Collections;

	using Castle.ActiveRecord.Framework;


	[ActiveRecord("Blogs")]
	public class Blog : ActiveRecordBase
	{
		private int _id;
		private String _name;
		private String _author;
		private IList _posts;
		private IList _publishedposts;
		private IList _unpublishedposts;
		private IList _recentposts;

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

		[HasMany(typeof(Post), RelationType.Bag, Key="Posts", Table="Posts", Column="blogid")]
		public IList Posts
		{
			get { return _posts; }
			set { _posts = value; }
		}

		[HasMany(typeof(Post), RelationType.Bag, Key="PublishedPosts", Table="Posts", Column="blogid", Where="published = 1")]
		public IList PublishedPosts
		{
			get { return _publishedposts; }
			set { _publishedposts = value; }
		}

		[HasMany(typeof(Post), RelationType.Bag, Key="UnPublishedPosts", Table="Posts", Column="blogid", Where="published = 0")]
		public IList UnPublishedPosts
		{
			get { return _unpublishedposts; }
			set { _unpublishedposts = value; }
		}

		[HasMany(typeof(Post), RelationType.Bag, Key="RecentPosts", Table="Posts", Column="blogid", OrderBy="created desc")]
		public IList RecentPosts
		{
			get { return _recentposts; }
			set { _recentposts = value; }
		}

		public static void DeleteAll()
		{
			ActiveRecordBase.DeleteAll( typeof(Blog) );
		}

		public static Blog[] FindAll()
		{
			return (Blog[]) ActiveRecordBase.FindAll( typeof(Blog) );
		}

		public static Blog Find(int id)
		{
			return (Blog) ActiveRecordBase.FindByPrimaryKey( typeof(Blog), id );
		}

		/// <summary>
		/// We make it visible only to for test cases' assertions 
		/// </summary>
		public static ISessionFactoryHolder Holder
		{
			get { return _holder; }
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
