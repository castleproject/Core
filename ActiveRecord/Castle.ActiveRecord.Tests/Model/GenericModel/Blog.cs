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

#if DOTNET2

namespace Castle.ActiveRecord.Tests.Model.GenericModel
{
	using System;
	using System.Collections;
	using System.Collections.Generic;

	using NHibernate;

    using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework;


	[ActiveRecord("BlogTable")]
	public class Blog : ActiveRecordBase<Blog>
	{
		private int _id;
		private String _name;
		private String _author;
		private IList<Post> _posts = new List<Post>();
		private IList _publishedposts;
		private IList _unpublishedposts;
		private IList _recentposts;

		[PrimaryKey]
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

		[HasMany(Table="Posts", ColumnKey="blogid")]
		public IList<Post> Posts
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

		public void CustomAction()
		{
			ActiveRecordMediator<Blog>.Execute(new NHibernateDelegate(MyCustomMethod), this);
		}

		private object MyCustomMethod(ISession session, object blogInstance)
		{
			session.Delete(blogInstance);
			session.Flush();

			return null;
		}

		internal static ISessionFactoryHolder Holder
		{
            get { return ActiveRecordMediator<Blog>.GetSessionFactoryHolder(); }
		}
    }
}
#endif