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
	
	using NHibernate;


	[Transactional]
	public class SecondDao
	{
		private readonly ISessionManager sessManager;

		public SecondDao(ISessionManager sessManager)
		{
			this.sessManager = sessManager;
		}

		[Transaction]
		public virtual BlogItem Create(Blog blog)
		{
			using(ISession session = sessManager.OpenSession())
			{
				NUnit.Framework.Assert.IsNotNull(session.Transaction);

				BlogItem item = new BlogItem();
				
				item.ParentBlog = blog;
				item.ItemDate = DateTime.Now;
				item.Text = "x";
				item.Title = "splinter cell is cool!";
				
				session.Save(item);
				
				return item;
			}
		}

		[Transaction]
		public virtual BlogItem CreateWithException(Blog blog)
		{
			using(ISession session = sessManager.OpenSession())
			{
				NUnit.Framework.Assert.IsNotNull(session.Transaction);

				BlogItem item = new BlogItem();
				
				item.ParentBlog = blog;
				item.ItemDate = DateTime.Now;
				item.Text = "x";
				item.Title = "splinter cell is cool!";

				throw new NotSupportedException("I dont feel like supporting this");
			}
		}

		[Transaction]
		public virtual BlogItem CreateWithException2(Blog blog)
		{
			using(ISession session = sessManager.OpenSession())
			{
				NUnit.Framework.Assert.IsNotNull(session.Transaction);

				BlogItem item = new BlogItem();
				
				item.ParentBlog = blog;
				item.ItemDate = DateTime.Now;
				item.Text = "x";
				item.Title = "splinter cell is cool!";
				
				session.Save(item);
				
				throw new NotSupportedException("I dont feel like supporting this");
			}
		}
	}
}
