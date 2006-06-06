using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

using NHibernate;
using NHibernate.Cfg;

using nUGSoft.BusinessEntities;
using NUnit.Framework;

namespace nUGSoft.Tests.DALTests
{
    [TestFixture]
    public class FooBarNHibernateTests
    {
        #region Private Members
              
        ISessionFactory factory;
              
        #endregion


        [SetUp]
        public void TestSetup()
        {
            Configuration cfg = new Configuration();

            cfg.AddXmlFile(@"C:\Documents and Settings\James Avery\My Documents\My Projects\NHibernate.Nullables2\nhibtest\FooBar.hbm.xml");

            factory = cfg.BuildSessionFactory();
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
            foo.TestGuid = new Guid(); ;

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

            System.Collections.IList foos = session.CreateCriteria(typeof(FooBar)).List();

            session.Flush();
            session.Disconnect();
            foreach (FooBar foo in foos)
            {
                Console.WriteLine(foo.Id);
            }
        }















    }
}
