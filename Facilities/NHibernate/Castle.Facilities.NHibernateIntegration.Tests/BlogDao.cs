// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.Facilities.NHibernateIntegration.Tests
{
	using System;
	using System.Collections;

	using NHibernate;

	using Castle.Facilities.NHibernateExtension;

	/// <summary>
	/// Summary description for BlogDao.
	/// </summary>
	[UsesAutomaticSessionCreation]
	public class BlogDao
	{
		public virtual Blog CreateBlog( String name )
		{
			ISession session = SessionManager.CurrentSession;

			Blog blog = new Blog();
			blog.Name = name;
			blog.Items = new ArrayList();

			session.Save(blog);

			return blog;
		}

		public virtual IList ObtainBlogs()
		{
			ISession session = SessionManager.CurrentSession;
			return session.Find("from Blog");
		}
	}
}
