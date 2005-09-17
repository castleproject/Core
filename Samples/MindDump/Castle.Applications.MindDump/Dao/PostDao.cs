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

namespace Castle.Applications.MindDump.Dao
{
	using System;
	using System.Collections;

	using NHibernate;

	using Castle.Applications.MindDump.Model;

	using Castle.Facilities.NHibernateExtension;


	[UsesAutomaticSessionCreation]
	public class PostDao
	{
		public virtual Post Create(Post post)
		{
			ISession session = SessionManager.CurrentSession;

			session.Save(post);

			return post;
		}

		public virtual void Update(Post post)
		{
			ISession session = SessionManager.CurrentSession;

			session.Update(post);
		}

		/// <summary>
		/// Usually will be invoked only be the
		/// test cases
		/// </summary>
		[SessionFlush(FlushOption.Force)]
		public virtual void DeleteAll()
		{
			SessionManager.CurrentSession.Delete("from Post");
		}

		public virtual IList Find()
		{
			return SessionManager.CurrentSession.Find("from Post order by id desc");
		}

		public virtual IList Find(Blog blog)
		{
			IList list = SessionManager.CurrentSession.Find(
				"from Post as a where a.Blog.Id=:name", blog.Id, NHibernateUtil.Int64);

			return list;
		}

		public virtual Post Find(long id)
		{
			IList list = SessionManager.CurrentSession.Find(
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

		public virtual IList FindLatest(int howMany)
		{
			IList list = SessionManager.CurrentSession.Find("from Post order by id desc");

			if (list.Count > howMany)
			{
				list = ListUtil.Limit(howMany, list);
			}

			return list;
		}
	}
}
