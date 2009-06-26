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

namespace Castle.Facilities.NHibernateIntegration.Tests.Components
{
	using MicroKernel.Registration;
	using NHibernate;
	using NHibernate.Criterion;
	using NHibernateIntegration.Components.Dao;
	using NUnit.Framework;
	using System;

	[TestFixture]
	public class NHibernateGenericDaoTests : AbstractNHibernateTestCase
	{
		class NonPersistentClass
		{

		}
		private NHibernateGenericDao nhGenericDao;
		private NHibernateGenericDao nhGenericDao2;
		private ISessionManager sessionManager;
		public override void OnSetUp()
		{
			this.container.Register(Component.For<NHibernateGenericDao>()
										.ImplementedBy<NHibernateGenericDao>());
			this.sessionManager = this.container.Resolve<ISessionManager>();
			this.nhGenericDao = this.container.Resolve<NHibernateGenericDao>();
			this.nhGenericDao2 = new NHibernateGenericDao(this.sessionManager, "sessionFactory1");
			using (var session = sessionManager.OpenSession())
			{
				var blog1 = new Blog { Name = "myblog1" };
				var blog1Item = new BlogItem
									{
										ItemDate = DateTime.Now,
										ParentBlog = blog1,
										Text = "Hello",
										Title = "mytitle1"
									};
				blog1.Items.Add(blog1Item);

				var blog2 = new Blog { Name = "myblog2" };
				var blog2Item = new BlogItem
									{
										ItemDate = DateTime.Now,
										ParentBlog = blog1,
										Text = "Hello",
										Title = "mytitle2"
									};
				blog2.Items.Add(blog2Item);

				var blog3 = new Blog { Name = "myblog3" };
				var blog3Item = new BlogItem
				{
					ItemDate = DateTime.Now,
					ParentBlog = blog1,
					Text = "Hello3",
					Title = "mytitle3"
				};
				blog3.Items.Add(blog3Item);
				session.Save(blog1);
				session.Save(blog1Item);
				session.Save(blog2);
				session.Save(blog2Item);
				session.Save(blog3);
				session.Save(blog3Item);
			}

		}
		public override void OnTearDown()
		{
			using (var session = sessionManager.OpenSession())
			{
				session.Delete("from BlogItem");
				session.Delete("from Blog");
			}
		}
		[Test]
		public void SetUppedCorrectly()
		{

			Assert.That(nhGenericDao.SessionFactoryAlias, Is.Null);
			Assert.That(nhGenericDao2.SessionFactoryAlias, Is.EqualTo("sessionFactory1"));
		}
		[Test]
		public void CanGetById()
		{

			using (var sess = sessionManager.OpenSession())
			{
				var blog = nhGenericDao.FindById(typeof(Blog), 1) as Blog;
				Assert.That(blog.Id, Is.EqualTo(1));
			}
		}

		[Test]
		public void CanInitializeLazyProperty()
		{
			using (var sess = sessionManager.OpenSession())
			{
				Blog b = nhGenericDao.FindById(typeof(Blog), 1) as Blog;
				Assert.That(NHibernateUtil.IsInitialized(b.Items), Is.False);
				nhGenericDao.InitializeLazyProperty(b, "Items");
				Assert.That(NHibernateUtil.IsInitialized(b.Items), Is.True);
			}
		}
		[Test]
		public void ThrowsExceptionOnNonExistingProperty()
		{
			using (var sess = sessionManager.OpenSession())
			{
				Blog b = nhGenericDao.FindById(typeof(Blog), 1) as Blog;
				Assert.Throws<ArgumentOutOfRangeException>(() => nhGenericDao.InitializeLazyProperty(b, "Bla"));
			}
		}

		[Test]
		public void ThrowsExceptionWhenNullInstance()
		{
			using (var sess = sessionManager.OpenSession())
			{
				Blog b = nhGenericDao.FindById(typeof(Blog), 1) as Blog;
				Assert.Throws<ArgumentNullException>(() => nhGenericDao.InitializeLazyProperty(null, "Items"));
				Assert.Throws<ArgumentNullException>(() => nhGenericDao.InitializeLazyProperty(b, null));
			}
		}

		[Test]
		public void ThrowsExceptionWhenNullInstance2()
		{
			using (var sess = sessionManager.OpenSession())
			{
				Assert.Throws<ArgumentNullException>(() => nhGenericDao.InitializeLazyProperties(null));
			}
		}

		[Test]
		public void CanInitializeAllLazyProperties()
		{
			using (var sess = sessionManager.OpenSession())
			{
				Blog b = nhGenericDao.FindById(typeof(Blog), 1) as Blog;
				Assert.That(NHibernateUtil.IsInitialized(b.Items), Is.False);
				nhGenericDao.InitializeLazyProperties(b);
				Assert.That(NHibernateUtil.IsInitialized(b.Items), Is.True);
			}
		}

		[Test]
		public void CanSaveNewItem()
		{
			using (var sess = sessionManager.OpenSession())
			using (var tran = sess.BeginTransaction())
			{
				Blog b = new Blog();
				b.Name = "blah";
				nhGenericDao.Save(b);
				Assert.That(b.Id, Is.GreaterThan(0));
				tran.Rollback();
			}
		}


		[Test]
		public void CannotSaveNull()
		{
			using (var sess = sessionManager.OpenSession())
			{
				Assert.Throws<DataException>(() => nhGenericDao.Save(new NonPersistentClass()));
			}
		}

		[Test]
		public void CanFindAll()
		{
			using (var sess = sessionManager.OpenSession())
			{
				var results = nhGenericDao.FindAll(typeof(Blog));
				Assert.That(results, Has.Length.EqualTo(3));
			}
		}
		[Test]
		public void CanFindAllWithCriterion()
		{
			using (var sess = sessionManager.OpenSession())
			{
				var results = nhGenericDao.FindAll(typeof(Blog),
												   new[] { Restrictions.Eq("Name", "myblog2") });

				Assert.That(results, Has.Length.EqualTo(1));
				Assert.That(((Blog)results.GetValue(0)).Name, Is.EqualTo("myblog2"));
			}
		}
		[Test]
		public void CanFindAllWithCriterionOrderBy()
		{
			using (var sess = sessionManager.OpenSession())
			{
				var results = nhGenericDao.FindAll(typeof(BlogItem),
												   new[] { Restrictions.Eq("Text", "Hello") },
												   new[] { Order.Desc("Title") });

				Assert.That(results, Has.Length.EqualTo(2));
				Assert.That(((BlogItem)results.GetValue(0)).Title, Is.EqualTo("mytitle2"));
				Assert.That(((BlogItem)results.GetValue(1)).Title, Is.EqualTo("mytitle1"));
			}
		}

		[Test]
		public void CanFindAllWithCriterionOrderByLimits()
		{
			using (var sess = sessionManager.OpenSession())
			{
				var results = nhGenericDao.FindAll(typeof(BlogItem),
												   new[] { Restrictions.Eq("Text", "Hello") },
												   new[] { Order.Desc("Title") }, 1, 1);

				Assert.That(results, Has.Length.EqualTo(1));
				Assert.That(((BlogItem)results.GetValue(0)).Title, Is.EqualTo("mytitle1"));
			}
		}
		[Test]
		public void CanFindAllWithCriterionOrderByLimitsOutOfRangeReturnsEmptyArray()
		{
			using (var sess = sessionManager.OpenSession())
			{
				var results = nhGenericDao.FindAll(typeof(BlogItem),
												   new[] { Restrictions.Eq("Text", "Hello") },
												   new[] { Order.Desc("Title") }, 2, 3);

				Assert.That(results, Has.Length.EqualTo(0));
			}
		}

		[Test]
		public void CanFindAllWithLimits()
		{
			using (var sess = sessionManager.OpenSession())
			{
				var results = nhGenericDao.FindAll(typeof(BlogItem), 1, 2);
				Assert.That(results, Has.Length.EqualTo(2));
			}
		}

		[Test]
		public void CanFindAllWithLimitsOutOfRangeReturnsEmptyArray()
		{
			using (var sess = sessionManager.OpenSession())
			{
				var results = nhGenericDao.FindAll(typeof(BlogItem), 3, 4);
				Assert.That(results, Has.Length.EqualTo(0));
			}
		}

		[Test]
		public void CanFindAllWithCriterionLimit()
		{
			using (var sess = sessionManager.OpenSession())
			{
				var results = nhGenericDao.FindAll(typeof(BlogItem),
					new[] { Restrictions.Eq("Text", "Hello") },
					0, 1);
				Assert.That(results, Has.Length.EqualTo(1));
				Assert.That(((BlogItem)results.GetValue(0)).Title, Is.EqualTo("mytitle1"));
			}
		}

		[Test]
		public void FindAllWithCustomQuery()
		{
			using (var sess = sessionManager.OpenSession())
			{
				var results = nhGenericDao.FindAllWithCustomQuery("from BlogItem b where b.Text='Hello'");
				Assert.That(results, Has.Length.EqualTo(2));
			}
		}
		[Test]
		public void FindAllWithCustomQueryLimits()
		{
			using (var sess = sessionManager.OpenSession())
			{
				var results = nhGenericDao.FindAllWithCustomQuery("from BlogItem b where b.Text='Hello'", 1, 1);
				Assert.That(results, Has.Length.EqualTo(1));
			}
		}

		[Test]
		public void DeleteAllWithType()
		{
			using (var sess = sessionManager.OpenSession())
			using (var tran = sess.BeginTransaction())
			{
				nhGenericDao.DeleteAll(typeof(Blog));
				tran.Commit();
			}
			using (var sess = sessionManager.OpenSession())
			{
				var results = nhGenericDao.FindAll(typeof(Blog));
				Assert.That(results, Has.Length.EqualTo(0));
			}
		}

		[Test]
		public void Delete()
		{
			using (var sess = sessionManager.OpenSession())
			{
				var results = nhGenericDao.FindAll(typeof(Blog));
				Assert.That(results, Has.Length.EqualTo(3));
			}
			using (var sess = sessionManager.OpenSession())
			using (var tran = sess.BeginTransaction())
			{
				var blog = nhGenericDao.FindById(typeof(Blog), 1);
				nhGenericDao.Delete(blog);
				tran.Commit();
			}
			using (var sess = sessionManager.OpenSession())
			{
				var results = nhGenericDao.FindAll(typeof(Blog));
				Assert.That(results, Has.Length.EqualTo(2));
			}
		}
		[Test]
		public void CreateSavesObjectInTheDatabase()
		{
			using (var sess = sessionManager.OpenSession())
			{
				Blog b = new Blog { Name = "myblog4" };
				var id = nhGenericDao.Create(b);
				Assert.That(id, Is.GreaterThan(0));
			}
		}
		[Test]
		public void GetByNamedQuery()
		{
			using (var sess = sessionManager.OpenSession())
			{
				var results = nhGenericDao.FindAllWithNamedQuery("getAllBlogs");

				Assert.That(results, Has.Length.EqualTo(3));
			}
		}

		[Test]
		public void GetByNamedQueryThrowsExceptionWhenNullParameter()
		{
			using (var sess = sessionManager.OpenSession())
			{
				Assert.Throws<ArgumentNullException>(() => nhGenericDao.FindAllWithNamedQuery(null));
			}
		}

		[Test]
		public void GetByNamedQueryThrowsExceptionWhenNonExistingQuery()
		{
			using (var sess = sessionManager.OpenSession())
			{
				Assert.Throws<DataException>(() => nhGenericDao.FindAllWithNamedQuery("getMyBlogs"));
			}
		}
		[Test]
		public void GetByNamedQueryWithLimits()
		{
			using (var sess = sessionManager.OpenSession())
			{
				var results = nhGenericDao.FindAllWithNamedQuery("getAllBlogs", 1, 2);

				Assert.That(results, Has.Length.EqualTo(2));
			}
		}
	}
}
