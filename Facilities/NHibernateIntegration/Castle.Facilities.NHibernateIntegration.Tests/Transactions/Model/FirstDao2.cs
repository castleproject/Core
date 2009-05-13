// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.NHibernateIntegration.Tests.Transactions
{
	using System;
	using Castle.Services.Transaction;
	using NHibernate;

	[Transactional]
	public class FirstDao2
	{
		private readonly ISessionManager sessManager;

		public FirstDao2(ISessionManager sessManager)
		{
			this.sessManager = sessManager;
		}

		[Transaction(Distributed = true)]
		public virtual Blog Create()
		{
			return Create("xbox blog");
		}

		[Transaction(Distributed = true)]
		public virtual Blog Create(String name)
		{
			NHibernate.ITransaction tran;

			using(ISession session = sessManager.OpenSession())
			{
				tran = session.Transaction;

				// NUnit.Framework.Assert.IsNotNull(session.Transaction);
				// NUnit.Framework.Assert.IsTrue(session.Transaction.IsActive);

				Blog blog = new Blog();
				blog.Name = name;
				session.Save(blog);
				return blog;
			}
		}

		[Transaction(Distributed = true)]
		public virtual void Delete(String name)
		{
			using(ISession session = sessManager.OpenSession())
			{
				NUnit.Framework.Assert.IsNotNull(session.Transaction);

				session.Delete("from Blog b where b.Name ='" + name + "'");
			}
		}

		[Transaction(Distributed = true)]
		public virtual void AddBlogRef(BlogRef blogRef)
		{
			using(ISession session = sessManager.OpenSession())
			{
				session.Save(blogRef);
			}
		}
	}
}