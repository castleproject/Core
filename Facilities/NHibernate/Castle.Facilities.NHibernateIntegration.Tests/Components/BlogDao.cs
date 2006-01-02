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

namespace Castle.Facilities.NHibernateIntegration.Tests
{
	using System;
	using System.Collections;
	using Castle.MicroKernel;
	using NHibernate;

	using Castle.Services.Transaction;
	using Castle.Facilities.NHibernateExtension;
	using NUnit.Framework;


	[UsesAutomaticSessionCreation]
	public class BlogDao
	{
		protected readonly IKernel kernel;

		public BlogDao(IKernel kernel)
		{
			this.kernel = kernel;
		}

		[SessionFlush(FlushOption.Force)]
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

		[SessionFlush(FlushOption.Force)]
		public virtual void DeleteAll()
		{
			ISession session = SessionManager.CurrentSession;
			session.Delete("from Blog");
		}
	}
}
