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
	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Config;
	using Castle.ActiveRecord.Framework.Scopes;
	using Castle.ActiveRecord.Tests.Model;
	using Castle.ActiveRecord.Tests.Model.LazyModel;
	using NHibernate;
	using NUnit.Framework;

	[TestFixture]
	public class ScopeDefaultFlushingBehaviourTestCase : AbstractActiveRecordTest
	{

		[Test] 
		public void TestClassicBehaviour() {TestBehaviour(DefaultFlushType.Classic, FlushMode.Auto, FlushMode.Commit);}
		[Test]
		public void TestAutoBehaviour() { TestBehaviour(DefaultFlushType.Auto, FlushMode.Auto, FlushMode.Auto); }
		[Test]
		public void TestLeaveBehaviour() { TestBehaviour(DefaultFlushType.Leave, FlushMode.Commit, FlushMode.Commit); }
		[Test]
		public void TestTransactionBehaviour() { TestBehaviour(DefaultFlushType.Transaction, FlushMode.Never, FlushMode.Auto); }
		
		private void TestBehaviour(DefaultFlushType flushType, FlushMode sessionScopeMode, FlushMode transactionScopeMode)
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(Post), typeof(Blog));
			Recreate();

			Post.DeleteAll();
			Blog.DeleteAll();

			DefaultFlushType originalDefaultFlushType = ActiveRecordStarter.ConfigurationSource.DefaultFlushType;
			try
			{
				((InPlaceConfigurationSource)ActiveRecordStarter.ConfigurationSource).DefaultFlushType = flushType;

				Blog blog = new Blog(); // just for CurrentSession

				using (new SessionScope())
				{
					Blog.FindAll();
					Assert.AreEqual(sessionScopeMode, blog.CurrentSession.FlushMode);

					using (new TransactionScope())
					{
						Blog.FindAll();
						Assert.AreEqual(transactionScopeMode, blog.CurrentSession.FlushMode);
					}

					// Properly reset?
					Blog.FindAll();
					Assert.AreEqual(sessionScopeMode, blog.CurrentSession.FlushMode);
				}
			}
			finally
			{
				// Restore Default Flush type we corrupted before.
				((InPlaceConfigurationSource)ActiveRecordStarter.ConfigurationSource).DefaultFlushType = originalDefaultFlushType;
			}
		}
	}
}
