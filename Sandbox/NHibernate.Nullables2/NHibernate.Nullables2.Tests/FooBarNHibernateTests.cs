using System;
using System.Collections;
using System.IO;

using NHibernate;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;

using nUGSoft.BusinessEntities;

using NUnit.Framework;

namespace nUGSoft.Tests.DALTests
{
	[TestFixture]
	public class FooBarNHibernateTests
	{
		#region Private Members
		Configuration cfg;
		ISessionFactory factory;
		#endregion

		[TestFixtureSetUp]
		public void SetupTestCase()
		{
			cfg = new Configuration();
			cfg.AddXmlFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FooBar.hbm.xml"));

			SchemaExport export = new SchemaExport(cfg);
			export.Create(false, true);

			factory = cfg.BuildSessionFactory();
		}
		
		[TestFixtureTearDown]
		public void TearDownTestCase()
		{
			factory.Close();

			SchemaExport export = new SchemaExport(cfg);
			export.Drop(false, true);
		}
		
		[SetUp]
		public void TestSetup()
		{
		}

		[Test]
		public void TestAddFooBarBooleanNull()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarBooleanNotNull()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();
			foo.TestBool = false;

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarByteNull()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarByteNotNull()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();
			foo.TestByte = 0;

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarCharNull()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarCharNotNull()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();
			foo.TestChar = '0';

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarDateTimeNull()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarDateTimeNotNull()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();
			foo.TestDateTime = DateTime.Now;

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarDecimalNull()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarDecimalNotNull()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();
			foo.TestDecimal = 0;

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarDoubleNull()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarDoubleNotNull()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();
			foo.TestDouble = 0;

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarGuidNull()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarGuidNotNull()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();
			foo.TestGuid = new Guid();
			;

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarInt16Null()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarInt16NotNull()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();
			foo.TestInt16 = 0;

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarInt32Null()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarInt32NotNull()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();
			foo.TestInt32 = 0;

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarInt64Null()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarInt64NotNull()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();
			foo.TestInt64 = 0;

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarSByteNull()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarSByteNotNull()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();
			foo.TestSByte = 0;

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarSingleNull()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestAddFooBarSingleNotNull()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			FooBar foo = new FooBar();
			foo.TestSingle = 0;

			session.Save(foo);
			transaction.Commit();
			session.Close();
		}

		[Test]
		public void TestGets()
		{
			ISession session = factory.OpenSession();

			ITransaction transaction = session.BeginTransaction();

			IList foos = session.CreateCriteria(typeof(FooBar)).List();

			transaction.Commit();

			session.Flush();
			session.Disconnect();
			
			foreach (FooBar foo in foos)
			{
				Console.WriteLine(foo.Id);
			}
		}
	}
}