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
	using System;
	using System.Collections;

	using Castle.Applications.MindDump.Model;
	using Castle.Facilities.NHibernateIntegration;

	using NHibernate;

	public class BlogDao
	{
		private readonly ISessionManager sessionManager;

		public BlogDao(ISessionManager sessionManager)
		{
			this.sessionManager = sessionManager;
		}

		public Blog Create(Blog blog)
		{
			using(ISession session = sessionManager.OpenSession())
			{
				if (blog.Posts == null)
				{
					blog.Posts = new ArrayList();
				}

				session.Save(blog);
			}

			return blog;
		}

		public void Update(Blog blog)
		{
			using(ISession session = sessionManager.OpenSession())
			{
				session.Update(blog);
			}
		}

		/// <summary>
		/// Usually will be invoked only by the
		/// test cases
		/// </summary>
		public void DeleteAll()
		{
			using(ISession session = sessionManager.OpenSession())
			{
				session.Delete("from Blog");
			}
		}

		public IList Find()
		{
			using(ISession session = sessionManager.OpenSession())
			{
				return session.Find("from Blog");
			}
		}

		public Blog Find(String blogName)
		{
			using(ISession session = sessionManager.OpenSession())
			{
				IList list = session.Find(
					"from Blog as a where a.Name=:name", blogName, NHibernateUtil.String);

				if (list.Count == 1)
				{
					return list[0] as Blog;
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
				IList list = session.Find("from Blog order by id desc");

				if (list.Count > howMany)
				{
					list = ListUtil.Limit(howMany, list);
				}

				return list;
			}
		}
	}
}
