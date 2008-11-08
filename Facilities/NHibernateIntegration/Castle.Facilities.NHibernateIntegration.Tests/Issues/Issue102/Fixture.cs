// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
#region

using NHibernate;
using NUnit.Framework;

#endregion

namespace Castle.Facilities.NHibernateIntegration.Tests.Issues.Issue102
{
	[TestFixture]
	public class Fixture : IssueTestCase
	{
		[Test]
		public void HasAliassedSessionHasFlushModeSet()
		{
			ISessionManager manager = container.Resolve<ISessionManager>();
			FlushMode previous = manager.DefaultFlushMode;
			manager.DefaultFlushMode = (FlushMode) 100;
			ISession session = manager.OpenSession("intercepted");
			Assert.AreEqual(manager.DefaultFlushMode, session.FlushMode);
			manager.DefaultFlushMode = previous;
		}

		[Test]
		public void SessionHasFlushModeSet()
		{
			ISessionManager manager = container.Resolve<ISessionManager>();
			FlushMode previous = manager.DefaultFlushMode;
			manager.DefaultFlushMode = (FlushMode) 100;
			ISession session = manager.OpenSession();
			Assert.AreEqual(manager.DefaultFlushMode, session.FlushMode);
			manager.DefaultFlushMode = previous;
		}
	}
}
