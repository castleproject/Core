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

namespace Castle.Facilities.NHibernateIntegration.Tests.Internals
{
	using System;

	using NUnit.Framework;

	using NHibernate;
	
	using Castle.MicroKernel.Facilities;
	using Castle.Services.Transaction;
	using ITransaction = Castle.Services.Transaction.ITransaction;

	/// <summary>
	/// Tests the default implementation of ISessionStore
	/// </summary>
	[TestFixture]
	public class SessionManagerTestCase : AbstractNHibernateTestCase
	{
		[Test]
		public void TwoDatabases()
		{
			ISessionManager manager = (ISessionManager) 
				container[typeof(ISessionManager)];

			ISession session1 = manager.OpenSession();
			ISession session2 = manager.OpenSession("db2");

			Assert.IsNotNull(session1);
			Assert.IsNotNull(session2);

			Assert.IsFalse( Object.ReferenceEquals(session1, session2) );

			session2.Dispose();
			session1.Dispose();

			Assert.IsTrue( (container[typeof(ISessionStore)] 
				as ISessionStore).IsCurrentActivityEmptyFor( Constants.DefaultAlias ) );
		}

		[Test]
		[ExpectedException(typeof(FacilityException))]
		public void NonExistentAlias()
		{
			ISessionManager manager = (ISessionManager) 
				container[typeof(ISessionManager)];

			manager.OpenSession("something in the way she moves");
		}

		[Test]
		public void SharedSession()
		{
			ISessionManager manager = (ISessionManager) 
				container[typeof(ISessionManager)];

			ISession session1 = manager.OpenSession();
			ISession session2 = manager.OpenSession();
			ISession session3 = manager.OpenSession();

			Assert.IsNotNull(session1);
			Assert.IsNotNull(session2);
			Assert.IsNotNull(session3);

			Assert.AreSame(session1, session2);
			Assert.AreSame(session1, session3);

			session3.Dispose();
			session2.Dispose();
			session1.Dispose();

			Assert.IsTrue( (container[typeof(ISessionStore)] 
				as ISessionStore).IsCurrentActivityEmptyFor( Constants.DefaultAlias ) );
		}

		/// <summary>
		/// This test ensures that the transaction takes 
		/// ownership of the session and disposes it at the end
		/// of the transaction
		/// </summary>
		[Test]
		public void NewTransactionBeforeUsingSession()
		{
			ISessionManager manager = (ISessionManager) 
				container[typeof(ISessionManager)];

			ITransactionManager tmanager = (ITransactionManager)
				container[typeof(ITransactionManager)];

			ITransaction transaction = tmanager.CreateTransaction( 
				TransactionMode.Requires, IsolationMode.Serializable );

			transaction.Begin();

			ISession session = manager.OpenSession();			

			Assert.IsNotNull(session);
			Assert.IsNotNull(session.Transaction);

			transaction.Commit();

			Assert.IsTrue(session.Transaction.WasCommitted);
			Assert.IsTrue(session.IsConnected); 

			session.Dispose();

			Assert.IsTrue( (container[typeof(ISessionStore)] 
				as ISessionStore).IsCurrentActivityEmptyFor( Constants.DefaultAlias ) );
		}

		/// <summary>
		/// In this case the transaction should not take
		/// ownership of the session (not dipose it at the 
		/// end of the transaction)
		/// </summary>
		[Test]
		public void NewTransactionAfterUsingSession()
		{
			ISessionManager manager = (ISessionManager) 
				container[typeof(ISessionManager)];

			ISession session1 = manager.OpenSession();			

			ITransactionManager tmanager = (ITransactionManager)
				container[typeof(ITransactionManager)];

			ITransaction transaction = tmanager.CreateTransaction( 
				TransactionMode.Requires, IsolationMode.Serializable );

			transaction.Begin();

			// Nested

			using(ISession session2 = manager.OpenSession())
			{
				Assert.IsNotNull(session2);

				Assert.IsNotNull(session1);
				Assert.IsNotNull(session1.Transaction);
			}

			transaction.Commit();

			Assert.IsTrue(session1.Transaction.WasCommitted);
			Assert.IsTrue(session1.IsConnected); 

			session1.Dispose();

			Assert.IsTrue( (container[typeof(ISessionStore)] 
				as ISessionStore).IsCurrentActivityEmptyFor( Constants.DefaultAlias ) );
		}

		/// <summary>
		/// This test ensures that the transaction enlists the 
		/// the sessions of both database connections
		/// </summary>
		[Test]
		public void NewTransactionBeforeUsingSessionWithTwoDatabases()
		{
			ISessionManager manager = (ISessionManager) 
				container[typeof(ISessionManager)];

			ITransactionManager tmanager = (ITransactionManager)
				container[typeof(ITransactionManager)];

			ITransaction transaction = tmanager.CreateTransaction( 
				TransactionMode.Requires, IsolationMode.Serializable );

			transaction.Begin();

			ISession session1 = manager.OpenSession();
			ISession session2 = manager.OpenSession("db2");

			Assert.IsNotNull(session1);
			Assert.IsNotNull(session1.Transaction);
			Assert.IsNotNull(session2);
			Assert.IsNotNull(session2.Transaction);

			transaction.Commit();

			Assert.IsTrue(session1.Transaction.WasCommitted);
			Assert.IsTrue(session1.IsConnected); 
			Assert.IsTrue(session2.Transaction.WasCommitted);
			Assert.IsTrue(session2.IsConnected);
 
			session2.Dispose();
			session1.Dispose();

			Assert.IsTrue( (container[typeof(ISessionStore)] 
				as ISessionStore).IsCurrentActivityEmptyFor( Constants.DefaultAlias ) );
		}

		[Test]
		public void NewTransactionAfterUsingSessionWithTwoDatabases()
		{
			ISessionManager manager = (ISessionManager) 
				container[typeof(ISessionManager)];

			ISession session1 = manager.OpenSession();			

			ITransactionManager tmanager = (ITransactionManager)
				container[typeof(ITransactionManager)];

			ITransaction transaction = tmanager.CreateTransaction( 
				TransactionMode.Requires, IsolationMode.Serializable );

			transaction.Begin();

			// Nested

			using(ISession session2 = manager.OpenSession())
			{
				Assert.IsNotNull(session2);

				Assert.IsNotNull(session1);
				Assert.IsNotNull(session1.Transaction);
			}

			transaction.Commit();

			Assert.IsTrue(session1.Transaction.WasCommitted);
			Assert.IsTrue(session1.IsConnected); 

			session1.Dispose();

			Assert.IsTrue( (container[typeof(ISessionStore)] 
				as ISessionStore).IsCurrentActivityEmptyFor( Constants.DefaultAlias ) );
		}
	}
}
