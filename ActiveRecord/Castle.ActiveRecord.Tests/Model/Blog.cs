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

	using NHibernate;
	
	using Castle.ActiveRecord.Framework;


	[ActiveRecord("BlogTable")]
	public class Blog
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

		[HasMany(typeof (Post), Table="Posts", ColumnKey="blogid")]
		public IList Posts
		{
			get { return _posts; }
			set { _posts = value; }
		}

		[HasMany(typeof (Post), Table="Posts", ColumnKey="blogid", Where="published = 1")]
		public IList PublishedPosts
		{
			get { return _publishedposts; }
			set { _publishedposts = value; }
		}

		[HasMany(typeof (Post), Table="Posts", ColumnKey="blogid", Where="published = 0")]
		public IList UnPublishedPosts
		{
			get { return _unpublishedposts; }
			set { _unpublishedposts = value; }
		}

		[HasMany(typeof (Post), Table="Posts", ColumnKey="blogid", OrderBy="created desc")]
		public IList RecentPosts
		{
			get { return _recentposts; }
			set { _recentposts = value; }
		}

		public static void DeleteAll()
		{
			ActiveRecordMediator.DeleteAll(typeof (Blog));
		}

		public static Blog[] FindAll()
		{
			return (Blog[]) ActiveRecordMediator.FindAll(typeof (Blog));
		}

		public static Blog Find(int id)
		{
			return (Blog) ActiveRecordMediator.FindByPrimaryKey(typeof (Blog), id);
		}

		public void CustomAction()
		{
			ActiveRecordMediator.Execute(typeof (Blog), new NHibernateDelegate(MyCustomMethod), this);
		}

		private object MyCustomMethod(ISession session, object blogInstance)
		{
			session.Delete(blogInstance);
			session.Flush();

			return null;
		}

		internal void Save()
		{
			ActiveRecordMediator.Save(this);
		}

		internal void Create()
		{
			ActiveRecordMediator.Create(this);
		}

		internal void Update()
		{
			ActiveRecordMediator.Update(this);
		}

		internal void Delete()
		{
			ActiveRecordMediator.Delete(this);
		}

		internal static ISessionFactoryHolder Holder
		{
			get { return ActiveRecordMediator.GetSessionFactoryHolder(); }
		}
	}
}