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

	using NHibernate;

	using Castle.Applications.MindDump.Model;

	using Castle.Facilities.NHibernateExtension;

	[UsesAutomaticSessionCreation]
	public class BlogDao
	{
		public virtual Blog Create(Blog blog)
		{
			ISession session = SessionManager.CurrentSession;

			if (blog.Posts == null)
			{
				blog.Posts = new ArrayList();
			}

			session.Save(blog);

			return blog;
		}

		public virtual void Update(Blog blog)
		{
			ISession session = SessionManager.CurrentSession;

			session.Update(blog);
		}

		/// <summary>
		/// Usually will be invoked only by the
		/// test cases
		/// </summary>
		[SessionFlush(FlushOption.Force)]
		public virtual void DeleteAll()
		{
			SessionManager.CurrentSession.Delete("from Blog");
		}

		public virtual IList Find()
		{
			return SessionManager.CurrentSession.Find("from Blog");
		}

		public virtual Blog Find(String blogName)
		{
			IList list = SessionManager.CurrentSession.Find(
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

		public virtual IList FindLatest(int howMany)
		{
			IList list = SessionManager.CurrentSession.Find("from Blog order by id desc");

			if (list.Count > howMany)
			{
				list = ListUtil.Limit(howMany, list);
			}

			return list;
		}
	}
}
