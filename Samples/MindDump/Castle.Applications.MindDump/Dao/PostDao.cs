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

namespace Castle.Applications.MindDump.Dao
{
	using System.Collections;
	
	using Castle.Applications.MindDump.Model;
	using Castle.Facilities.NHibernateIntegration;

	using NHibernate;

	public class PostDao
	{
		private readonly ISessionManager sessionManager;

		public PostDao(ISessionManager sessionManager)
		{
			this.sessionManager = sessionManager;
		}

		public Post Create(Post post)
		{
			using(ISession session = sessionManager.OpenSession())
			{
				session.Save(post);

				return post;
			}
		}

		public void Update(Post post)
		{
			using(ISession session = sessionManager.OpenSession())
			{
				session.Update(post);
			}
		}

		/// <summary>
		/// Usually will be invoked only be the
		/// test cases
		/// </summary>
		public void DeleteAll()
		{
			using(ISession session = sessionManager.OpenSession())
			{
				session.Delete("from Post");
			}
		}

		public IList Find()
		{
			using(ISession session = sessionManager.OpenSession())
			{
				return session.Find("from Post order by id desc");
			}
		}

		public IList Find(Blog blog)
		{
			using(ISession session = sessionManager.OpenSession())
			{
				IList list = session.Find(
					"from Post as a where a.Blog.Id=:name", blog.Id, NHibernateUtil.Int64);

				return list;
			}
		}

		public Post Find(long id)
		{
			using(ISession session = sessionManager.OpenSession())
			{
				IList list = session.Find(
					"from Post as a where a.id=:name order by id desc", id, NHibernateUtil.Int64);

				if (list.Count == 1)
				{
					return list[0] as Post;
				}
				else
				{
					return null;
				}
			}
		}

		public IList FindLatest(int howMany)
		{
			using(ISession session = sessionManager.OpenSession())
			{
				IList list = session.Find("from Post order by id desc");

				if (list.Count > howMany)
				{
					list = ListUtil.Limit(howMany, list);
				}

				return list;
			}
		}
	}
}
