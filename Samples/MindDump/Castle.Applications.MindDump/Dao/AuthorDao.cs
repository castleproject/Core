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
	public class AuthorDao
	{
		public virtual Author Create(Author author)
		{
			ISession session = SessionManager.CurrentSession;

			if (author.Blogs == null)
			{
				author.Blogs = new ArrayList();
			}

			session.Save(author);

			return author;
		}

		/// <summary>
		/// Usually will be invoked only be the
		/// test cases
		/// </summary>
		public virtual void DeleteAll()
		{
			SessionManager.CurrentSession.Delete("from Author");
		}

		public virtual IList Find()
		{
			return SessionManager.CurrentSession.Find("from Author");
		}

		public virtual Author Find(String login)
		{
			IList list = SessionManager.CurrentSession.Find(
				"from Author as a where a.Login=:name", login, NHibernate.String);

			if (list.Count == 1)
			{
				return list[0] as Author;
			}
			else
			{
				return null;
			}
		}
	}
}
