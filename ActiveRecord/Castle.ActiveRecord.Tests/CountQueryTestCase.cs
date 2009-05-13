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

namespace Castle.ActiveRecord.Tests
{
	using System;
	using System.Diagnostics;

	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Queries;

	using Castle.ActiveRecord.Tests.Model;

	using NHibernate.Criterion;

	using NUnit.Framework;

	[TestFixture]
	public class CountQueryTestCase : AbstractActiveRecordTest
	{
		// overrides the setup in the base class
		public void Prepare()
		{
			ActiveRecordStarter.ResetInitializationFlag();
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Blog), typeof(Post));

			ActiveRecordStarter.CreateSchema();

			Post.DeleteAll();
			Blog.DeleteAll();

			Blog blog = new Blog();
			blog.Name = "hammett's blog";
			blog.Author = "hamilton verissimo";

			blog.Save();

			Post p;

			p = new Post(blog, "a", "b", "c");
			p.Save();

			p = new Post(blog, "d", "e", "c");
			p.Save();

			p = new Post(blog, "f", "g", "h");
			p.Save();
		}

		// Test CountQuery(type)
		[Test]
		public void CountQuery_ByType()
		{
			Prepare();
			CountQuery cq = new CountQuery(typeof(Post));
			int recordCount = (int)ActiveRecordMediator.ExecuteQuery(cq);
			Assert.AreEqual(3, recordCount);
		}

		// Test CountQuery(type, criteria)
		[Test]
		public void CountQuery_ByTypeAndCriteria()
		{
			Prepare();
			ICriterion[] criterionArray = new ICriterion[] 
            { 
                Expression.Eq("Category", "c")
            };
			CountQuery cq = new CountQuery(typeof(Post), criterionArray);
			int recordCount = (int)ActiveRecordMediator.ExecuteQuery(cq);
			Assert.AreEqual(2, recordCount);
		}

		// Test CountQuery(type, filter, positional_parameters)
		[Test]
		public void CountQuery_PositionalParameters()
		{
			Prepare();
			object[] parameters = new object[] { "c" };
			CountQuery cq = new CountQuery(typeof(Post), "Category = ?", parameters);
			int recordCount = (int)ActiveRecordMediator.ExecuteQuery(cq);
			Assert.AreEqual(2, recordCount);
		}

		// Test CountQuery(type, Detached Criteria)
		[Test]
		public void CountQuery_DetachedCriteria()
		{
			Prepare();
			DetachedCriteria detachedCriteria = DetachedCriteria.For(typeof(Post));
			detachedCriteria.Add(Expression.Eq("Category", "c"));
			CountQuery cq = new CountQuery(typeof(Post), detachedCriteria);
			int recordCount = (int)ActiveRecordMediator.ExecuteQuery(cq);
			Assert.AreEqual(2, recordCount);
		}

		// Test CountQuery(type, criteria)
		[Test, ExpectedException(typeof(Castle.ActiveRecord.Framework.ActiveRecordException))]
		public void CountQuery_ByTypeAndBadCriteria()
		{
			Prepare();
			ICriterion[] criterionArray = new ICriterion[] 
            { 
                // misspelling on purpose
                Expression.Eq("Category;", "c")
            };
			CountQuery cq = new CountQuery(typeof(Post), criterionArray);
			int recordCount = (int)ActiveRecordMediator.ExecuteQuery(cq);
			Assert.AreEqual(2, recordCount);
		}

	}
}
