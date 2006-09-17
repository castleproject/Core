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

namespace Castle.Facilities.NHibernateIntegration.Tests.Transactions
{
	using System;

	using Castle.Services.Transaction;

	[Transactional]
	public class RootService : NHibernateGenericDao
	{
		private readonly FirstDao firstDao;
		private readonly SecondDao secondDao;
		private OrderDao orderDao;

		public RootService(FirstDao firstDao, SecondDao secondDao, ISessionManager sessManager) : base(sessManager)
		{
			this.firstDao = firstDao;
			this.secondDao = secondDao;
		}

		public OrderDao OrderDao
		{
			get { return orderDao; }
			set { orderDao = value; }
		}

		[Transaction]
		public virtual BlogItem SuccessFullCall()
		{
			Blog blog = firstDao.Create();
			return secondDao.Create(blog);
		}

		[Transaction]
		public virtual void CallWithException()
		{
			Blog blog = firstDao.Create();
			secondDao.CreateWithException(blog);
		}

		[Transaction]
		public virtual void CallWithException2()
		{
			Blog blog = firstDao.Create();
			secondDao.CreateWithException2(blog);
		}

		[Transaction]
		public virtual void DoBlogRefOperation(Blog blog)
		{
			BlogRef blogRef = new BlogRef();
			blogRef.ParentBlog = blog;
			blogRef.Title = "title";
			firstDao.AddBlogRef(blogRef);

			//constraint exception
			firstDao.Delete("Blog1");
		}
		
		[Transaction]
		public virtual void DoTwoDBOperation_Create(bool throwException)
		{
			Blog blog = firstDao.Create();
			secondDao.Create(blog);
			orderDao.Create(1.122f);
		}
	}
}
