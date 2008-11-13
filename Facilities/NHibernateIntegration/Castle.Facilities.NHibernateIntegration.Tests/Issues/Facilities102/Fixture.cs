using System;
using System.Collections.Generic;
using System.Text;
using NHibernate;
using NUnit.Framework;

namespace Castle.Facilities.NHibernateIntegration.Tests.Issues.Facilities102
{
	[TestFixture]
	public class Fixture : IssueTestCase
	{
		[Test]
		public void HasAliassedSessionHasFlushModeSet()
		{
			ISessionManager manager = container.Resolve<ISessionManager>();
			FlushMode previous = manager.DefaultFlushMode;
			manager.DefaultFlushMode = (FlushMode)100;
			ISession session = manager.OpenSession("intercepted");
			Assert.AreEqual(manager.DefaultFlushMode, session.FlushMode);
			manager.DefaultFlushMode = previous;
		}

		[Test]
		public void SessionHasFlushModeSet()
		{
			ISessionManager manager = container.Resolve<ISessionManager>();
			FlushMode previous = manager.DefaultFlushMode;
			manager.DefaultFlushMode = (FlushMode)100;
			ISession session = manager.OpenSession();
			Assert.AreEqual(manager.DefaultFlushMode, session.FlushMode);
			manager.DefaultFlushMode = previous;
		}
	}
}
