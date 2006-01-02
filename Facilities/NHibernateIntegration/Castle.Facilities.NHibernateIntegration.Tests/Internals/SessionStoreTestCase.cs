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
	using System.Threading;
	using NUnit.Framework;

	using NHibernate;

	/// <summary>
	/// Tests the default implementation of ISessionStore
	/// </summary>
	[TestFixture]
	public class SessionStoreTestCase : AbstractNHibernateTestCase
	{
		private AutoResetEvent arEvent = new AutoResetEvent(false);

		[Test]
		[ExpectedException( typeof(ArgumentNullException) )]
		public void NullAlias()
		{
			ISessionStore store = (ISessionStore) container[typeof(ISessionStore)];

			store.FindCompatibleSession(null);
		}

		[Test]
		public void FindCompatibleSession()
		{
			ISessionStore store = (ISessionStore) container[typeof(ISessionStore)];
			ISessionFactory factory = (ISessionFactory) container[typeof(ISessionFactory)];

			ISession session = store.FindCompatibleSession( Constants.DefaultAlias );

			Assert.IsNull(session);

			session = factory.OpenSession();

			SessionDelegate sessDelegate = new SessionDelegate(true, session, store);

			store.Store( Constants.DefaultAlias, sessDelegate );
			
			Assert.IsNotNull(sessDelegate.SessionStoreCookie);

			ISession session2 = store.FindCompatibleSession( "something in the way she moves" );

			Assert.IsNull(session2);

			session2 = store.FindCompatibleSession( Constants.DefaultAlias );

			Assert.IsNotNull(session2);
			Assert.AreSame(sessDelegate, session2);

			session.Dispose();

			store.Remove(sessDelegate);

			session = store.FindCompatibleSession( Constants.DefaultAlias );

			Assert.IsNull(session);

			Assert.IsTrue( store.IsCurrentActivityEmptyFor( Constants.DefaultAlias ) );
		}

		[Test]
		public void FindCompatibleSessionWithTwoThreads()
		{
			ISessionStore store = (ISessionStore) container[typeof(ISessionStore)];
			ISessionFactory factory = (ISessionFactory) container[typeof(ISessionFactory)];

			ISession session = factory.OpenSession();

			SessionDelegate sessDelegate = new SessionDelegate(true, session, store);

			store.Store( Constants.DefaultAlias, sessDelegate );

			ISession session2 = store.FindCompatibleSession( Constants.DefaultAlias );

			Assert.IsNotNull(session2);
			Assert.AreSame(sessDelegate, session2);

			Thread newThread = new Thread(new ThreadStart(FindCompatibleSessionOnOtherThread));
			newThread.Start();

			arEvent.WaitOne();

			sessDelegate.Dispose();

			Assert.IsTrue( store.IsCurrentActivityEmptyFor( Constants.DefaultAlias ) );
		}

		private void FindCompatibleSessionOnOtherThread()
		{
			ISessionStore store = (ISessionStore) container[typeof(ISessionStore)];

			ISession session = store.FindCompatibleSession( "something in the way she moves" );

			Assert.IsNull(session);

			ISession session2 = store.FindCompatibleSession( Constants.DefaultAlias );

			Assert.IsNull(session2);

			arEvent.Set();
		}
	}
}
