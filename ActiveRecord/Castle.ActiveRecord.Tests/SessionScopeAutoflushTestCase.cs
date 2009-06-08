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
	using System.Collections;
	using System.Collections.Generic;
	using Model;
	using NUnit.Framework;
	using Castle.ActiveRecord;
	using NHibernate;

	[TestFixture]
	public class SessionScopeAutoflushTestCase : AbstractActiveRecordTest
	{
		[Test]
		public void ActiveRecordUsingSessionScope()
		{
			InitModel();
			using (new SessionScope())
			{
				new SSAFEntity("example").Save();
				Assert.AreEqual(1, SSAFEntity.FindAll().Length);
			}
			Assert.AreEqual(1, SSAFEntity.FindAll().Length);
		}

		public void ActiveRecordUsingNoFlushSessionScope()
		{
			InitModel();
			using (new SessionScope(FlushAction.Never))
			{
				new SSAFEntity("example").Save();
				Assert.AreEqual(0, SSAFEntity.FindAll().Length);
			}
			Assert.AreEqual(0, SSAFEntity.FindAll().Length);
		}

		[Test]
		public void ActiveRecordUsingSessionScopeUsingExplicitFlush()
		{
			InitModel();
			using (new SessionScope())
			{
				new SSAFEntity("example").Save();
				SessionScope.Current.Flush();
				Assert.AreEqual(1, SSAFEntity.FindAll().Length);
			}
			Assert.AreEqual(1, SSAFEntity.FindAll().Length);
		}

		[Test]
		public void ActiveRecordUsingTransactionScope()
		{
			InitModel();
			using (new TransactionScope())
			{
				new SSAFEntity("example").Save();
				//Assert.AreEqual(1, SSAFEntity.FindAll().Length);
			}
			Assert.AreEqual(1, SSAFEntity.FindAll().Length);
		}

		[Test]
		public void ActiveRecordUsingTransactionScopeWithRollback()
		{
			InitModel();
			using (TransactionScope scope = new TransactionScope())
			{
				new SSAFEntity("example").Save();
				//Assert.AreEqual(1, SSAFEntity.FindAll().Length);
				scope.VoteRollBack();
			}
			Assert.AreEqual(0, SSAFEntity.FindAll().Length);
		}

		[Test][Ignore("This is worth debate")]
		public void ActiveRecordUsingTransactionScopeWithRollbackAndInnerSessionScope()
		{
			InitModel();
			using (TransactionScope scope = new TransactionScope())
			{
				using (new SessionScope())
				{
					new SSAFEntity("example").Save();
					Assert.AreEqual(1, SSAFEntity.FindAll().Length);
				}
				Assert.AreEqual(1, SSAFEntity.FindAll().Length);
				scope.VoteRollBack();
			}
			Assert.AreEqual(0, SSAFEntity.FindAll().Length);
		}

		public void ActiveRecordUsingNestedTransactionScopesWithRollback()
		{
			InitModel();
			using (TransactionScope scope = new TransactionScope())
			{
				using (new TransactionScope(TransactionMode.Inherits))
				{
					new SSAFEntity("example").Save();
					Assert.AreEqual(1, SSAFEntity.FindAll().Length);
				}
				Assert.AreEqual(1, SSAFEntity.FindAll().Length);
				scope.VoteRollBack();
			}
			Assert.AreEqual(0, SSAFEntity.FindAll().Length);
		}

		public void ActiveRecordUsingTransactionScopeWithCommitAndInnerSessionScope()
		{
			InitModel();
			using (TransactionScope scope = new TransactionScope())
			{
				using (new SessionScope())
				{
					new SSAFEntity("example").Save();
					Assert.AreEqual(1, SSAFEntity.FindAll().Length);
				}
				//Assert.AreEqual(1, SSAFEntity.FindAll().Length);
				scope.VoteCommit();
			}
			Assert.AreEqual(1, SSAFEntity.FindAll().Length);
		}


		[Test]
		public void NHibernateVerification()
		{
			InitModel();
			using (ISession session = ActiveRecordMediator.GetSessionFactoryHolder().GetSessionFactory(typeof(ActiveRecordBase)).OpenSession())
			using (ITransaction tx = session.BeginTransaction())
			{
				session.Save(new SSAFEntity("example"));
				Assert.AreEqual(1, session.CreateQuery("from SSAFEntity").List<SSAFEntity>().Count);
			}
		}

		[Test]
		public void SessionTxVerification()
		{
			InitModel();
			using (ISession session = ActiveRecordMediator.GetSessionFactoryHolder().GetSessionFactory(typeof(ActiveRecordBase)).OpenSession())
			{
				Assert.IsFalse(session.Transaction.IsActive);
				using (ITransaction tx = session.BeginTransaction())
				{
					Assert.AreSame(tx, session.BeginTransaction());
					Assert.AreSame(tx, session.Transaction);
				}
			}
		}


		[Test]
		[Ignore("Expected to fail")]
		public void NHibernateNoTxVerification()
		{
			InitModel();
			using (ISession session = ActiveRecordMediator.GetSessionFactoryHolder().GetSessionFactory(typeof(ActiveRecordBase)).OpenSession())
			{
				session.Save(new SSAFEntity("example"));
				Assert.AreEqual(1, session.CreateQuery("from SSAFEntity").List<SSAFEntity>().Count);
			}
		}

		private void InitModel()
		{
			ActiveRecordStarter.Initialize(GetConfigSource(), typeof(SSAFEntity));
			Recreate();
		}

	}

	#region Model

	#endregion
}