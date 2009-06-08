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
	using NUnit.Framework;

	using Castle.ActiveRecord.Tests.Model.GenericModel;

	using Castle.ActiveRecord.Queries;
	using System;

	[TestFixture]
	public class ScalarQueryTestCase : AbstractActiveRecordTest
	{
		[SetUp]
		public new void Init()
		{
			ActiveRecordStarter.ResetInitializationFlag();

			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Blog), typeof(Post));

			Recreate();

			var blog = new Blog() { Author = "Horatio", Name = "Murmurs" };
			blog.Save();

			var post = new Post(blog, "To Be Or Not To Be", "Nonsense", "monologues");
			post.Save();
		}

		[Test]
		public void TestSetup()
		{
			Assert.AreEqual(1, Post.FindAll().Length);
			Assert.AreEqual(1, Blog.FindAll().Length);
		}

		[Test]
		public void ValidResult()
		{
			ScalarQuery query = new ScalarQuery(typeof(Post), validQuery);
			long result = (long) ActiveRecordMediator<Post>.ExecuteQuery(query);
			Assert.AreEqual(1, result);
		}

		[Test]
		public void ValidResultGeneric()
		{
			ScalarQuery<long> query = new ScalarQuery<long>(typeof(Post), validQuery);
			long result = ActiveRecordMediator<Post>.ExecuteQuery2(query);
			Assert.AreEqual(1, result);
		}

		[Test]
		public void ValidGenericQueryWithDirectCall()
		{
			var query = new ScalarQuery<long>(typeof(Post), validQuery);
			Assert.AreEqual(1, query.Execute());
		}

		[Test]
		public void InvalidResultWithNonGenericQueryYieldsNull()
		{
			ScalarQuery query = new ScalarQuery(typeof(Post), invalidQuery);
			Assert.IsNull(ActiveRecordMediator<Post>.ExecuteQuery(query));
		}

		[Test]
		public void InvalidResultGeneric()
		{
			ScalarQuery<int> query = new ScalarQuery<int>(typeof(Post), invalidQuery);
			int result = ActiveRecordMediator<Post>.ExecuteQuery2(query);
			Assert.AreEqual(default(int), result);
		}

		[Test]
		public void InvalidGenericQueryWithDirectCall()
		{
			var query = new ScalarQuery<int>(typeof(Post), invalidQuery);
			Assert.AreEqual(default(int), query.Execute());
		}

		[Test]
		public void IncompatibleGenericTypesThrowMeaningfulErrorsOnDirectCall()
		{
			int result;
			var query = new ScalarQuery<int>(typeof(Post), validQuery);
			try
			{
				result = query.Execute();
			}
			catch (Exception ex)
			{
				Assert.IsInstanceOf(typeof(NHibernate.QueryException), ex);
				Assert.IsTrue(ex.Message.Contains("Int32"));
				Assert.IsTrue(ex.Message.Contains("Int64"));
			}
		}

		[Test]
		public void IncompatibleGenericTypesThrowMeaningfulErrorsOnMediatorCall()
		{
			int result;
			var query = new ScalarQuery<int>(typeof(Post), validQuery);
			try
			{
				result = ActiveRecordMediator<Post>.ExecuteQuery2(query);
			}
			catch (Exception ex)
			{
				Assert.IsInstanceOf(typeof(NHibernate.QueryException), ex);
				Assert.IsTrue(ex.Message.Contains("Int32"));
				Assert.IsTrue(ex.Message.Contains("Int64"));
			}
		}


		private string validQuery = "select count(*) from Post p";
		private string invalidQuery = "select max(p.Id) from Post p where p.Blog.Author='noone'"; 
	}
}
