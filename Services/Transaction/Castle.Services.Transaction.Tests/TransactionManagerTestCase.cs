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

namespace Castle.Services.Transaction.Tests
{
	using System;

	using NUnit.Framework;

	[TestFixture]
	public class TransactionManagerTestCase
	{
		private DefaultTransactionManager tm;

		[SetUp]
		public void Init()
		{
			tm = new DefaultTransactionManager(new TransientActivityManager());
		}

		[Test]
		public void SynchronizationsAndCommit()
		{
			ITransaction transaction = 
				tm.CreateTransaction(TransactionMode.Unspecified, IsolationMode.Unspecified);

			transaction.Begin();
	
			SynchronizationImpl sync = new SynchronizationImpl();

			transaction.RegisterSynchronization( sync );

			Assert.AreEqual( DateTime.MinValue, sync.Before );
			Assert.AreEqual( DateTime.MinValue, sync.After );

			transaction.Commit();

			Assert.IsTrue( sync.Before > DateTime.MinValue );
			Assert.IsTrue( sync.After > DateTime.MinValue );
		}

		[Test]
		public void SynchronizationsAndRollback()
		{
			ITransaction transaction = 
				tm.CreateTransaction(TransactionMode.Unspecified, IsolationMode.Unspecified);

			transaction.Begin();
	
			SynchronizationImpl sync = new SynchronizationImpl();

			transaction.RegisterSynchronization( sync );

			Assert.AreEqual( DateTime.MinValue, sync.Before );
			Assert.AreEqual( DateTime.MinValue, sync.After );

			transaction.Rollback();

			Assert.IsTrue( sync.Before > DateTime.MinValue );
			Assert.IsTrue( sync.After > DateTime.MinValue );
		}

		[Test]
		public void ResourcesAndCommit()
		{
			ITransaction transaction = 
				tm.CreateTransaction(TransactionMode.Unspecified, IsolationMode.Unspecified);
	
			ResourceImpl resource = new ResourceImpl();

			transaction.Enlist( resource );

			Assert.IsFalse( resource.Started );
			Assert.IsFalse( resource.Committed );
			Assert.IsFalse( resource.Rolledback );

			transaction.Begin();

			Assert.IsTrue( resource.Started );
			Assert.IsFalse( resource.Committed );
			Assert.IsFalse( resource.Rolledback );

			transaction.Commit();

			Assert.IsTrue( resource.Started );
			Assert.IsTrue( resource.Committed );
			Assert.IsFalse( resource.Rolledback );
		}

		[Test]
		public void ResourcesAndRollback()
		{
			ITransaction transaction = 
				tm.CreateTransaction(TransactionMode.Unspecified, IsolationMode.Unspecified);
	
			ResourceImpl resource = new ResourceImpl();

			transaction.Enlist( resource );

			Assert.IsFalse( resource.Started );
			Assert.IsFalse( resource.Committed );
			Assert.IsFalse( resource.Rolledback );

			transaction.Begin();

			Assert.IsTrue( resource.Started );
			Assert.IsFalse( resource.Committed );
			Assert.IsFalse( resource.Rolledback );

			transaction.Rollback();

			Assert.IsTrue( resource.Started );
			Assert.IsTrue( resource.Rolledback );
			Assert.IsFalse( resource.Committed );
		}

		[Test]
		[ExpectedException( typeof(TransactionException) )]
		public void InvalidBegin()
		{
			ITransaction transaction = tm.CreateTransaction(
				TransactionMode.Requires, IsolationMode.Unspecified);

			transaction.Begin();
			transaction.Begin();
		}

		[Test]
		[ExpectedException( typeof(TransactionException) )]
		public void InvalidCommit()
		{
			ITransaction transaction = tm.CreateTransaction(
				TransactionMode.Requires, IsolationMode.Unspecified);

			transaction.Begin();
			transaction.Rollback();
			
			transaction.Commit();
		}

		[Test]
		public void TransactionCreatedEvent()
		{
			bool transactionCreatedEventTriggered = false;

			tm.TransactionCreated += delegate { transactionCreatedEventTriggered = true; };

			Assert.IsFalse(transactionCreatedEventTriggered);

			ITransaction transaction = tm.CreateTransaction(
				TransactionMode.Requires, IsolationMode.Unspecified);

			Assert.IsTrue(transactionCreatedEventTriggered);
		}

		[Test]
		public void TransactionDisposedEvent()
		{
			bool transactionDisposedEventTriggered = false;

			tm.TransactionDisposed += delegate { transactionDisposedEventTriggered = true; };

			ITransaction transaction = tm.CreateTransaction(
				TransactionMode.Requires, IsolationMode.Unspecified);

			Assert.IsFalse(transactionDisposedEventTriggered);

			transaction.Begin();

			Assert.IsFalse(transactionDisposedEventTriggered);

			transaction.Commit();

			Assert.IsFalse(transactionDisposedEventTriggered);

			tm.Dispose(transaction);

			Assert.IsTrue(transactionDisposedEventTriggered);
		}

		[Test]
		public void TransactionCommittedEvent()
		{
			bool transactionCommittedEventTriggered = false;
			bool transactionRolledBackEventTriggered = false;
			bool transactionFailedEventTriggered = false;

			tm.TransactionCommitted += delegate { transactionCommittedEventTriggered = true; };
			tm.TransactionRolledback += delegate { transactionRolledBackEventTriggered = true; };
			tm.TransactionFailed += delegate { transactionFailedEventTriggered = true; };

			ITransaction transaction = tm.CreateTransaction(
				TransactionMode.Requires, IsolationMode.Unspecified);

			ResourceImpl resource = new ResourceImpl();

			transaction.Enlist(resource);

			Assert.IsFalse(transactionCommittedEventTriggered);
			Assert.IsFalse(transactionRolledBackEventTriggered);
			Assert.IsFalse(transactionFailedEventTriggered);

			transaction.Begin();

			Assert.IsFalse(transactionCommittedEventTriggered);
			Assert.IsFalse(transactionRolledBackEventTriggered);
			Assert.IsFalse(transactionFailedEventTriggered);

			transaction.Commit();

			Assert.IsTrue(transactionCommittedEventTriggered);
			Assert.IsFalse(transactionRolledBackEventTriggered);
			Assert.IsFalse(transactionFailedEventTriggered);
		}

		[Test]
		public void TransactionRolledBackEvent()
		{
			bool transactionCommittedEventTriggered = false;
			bool transactionRolledBackEventTriggered = false;
			bool transactionFailedEventTriggered = false;

			tm.TransactionCommitted += delegate { transactionCommittedEventTriggered = true; };
			tm.TransactionRolledback += delegate { transactionRolledBackEventTriggered = true; };
			tm.TransactionFailed += delegate { transactionFailedEventTriggered = true; };

			ITransaction transaction = tm.CreateTransaction(
				TransactionMode.Requires, IsolationMode.Unspecified);

			ResourceImpl resource = new ResourceImpl();

			transaction.Enlist(resource);

			Assert.IsFalse(transactionCommittedEventTriggered);
			Assert.IsFalse(transactionRolledBackEventTriggered);
			Assert.IsFalse(transactionFailedEventTriggered);

			transaction.Begin();

			Assert.IsFalse(transactionCommittedEventTriggered);
			Assert.IsFalse(transactionRolledBackEventTriggered);
			Assert.IsFalse(transactionFailedEventTriggered);

			transaction.Rollback();

			Assert.IsFalse(transactionCommittedEventTriggered);
			Assert.IsTrue(transactionRolledBackEventTriggered);
			Assert.IsFalse(transactionFailedEventTriggered);
		}

		[Test]
		public void TransactionFailedOnCommitEvent()
		{
			bool transactionCommittedEventTriggered = false;
			bool transactionRolledBackEventTriggered = false;
			bool transactionFailedEventTriggered = false;

			tm.TransactionCommitted += delegate { transactionCommittedEventTriggered = true; };
			tm.TransactionRolledback += delegate { transactionRolledBackEventTriggered = true; };
			tm.TransactionFailed += delegate { transactionFailedEventTriggered = true; };

			ITransaction transaction = tm.CreateTransaction(
				TransactionMode.Requires, IsolationMode.Unspecified);

			ResourceImpl resource = new ThrowsExceptionResourceImpl(true, false);

			transaction.Enlist(resource);

			Assert.IsFalse(transactionCommittedEventTriggered);
			Assert.IsFalse(transactionRolledBackEventTriggered);
			Assert.IsFalse(transactionFailedEventTriggered);

			transaction.Begin();

			Assert.IsFalse(transactionCommittedEventTriggered);
			Assert.IsFalse(transactionRolledBackEventTriggered);
			Assert.IsFalse(transactionFailedEventTriggered);

			TransactionException exception = null;

			try
			{
				transaction.Commit();
			}
			catch (TransactionException transactionError)
			{
				exception = transactionError;
			}

			Assert.IsFalse(transactionCommittedEventTriggered);
			Assert.IsFalse(transactionRolledBackEventTriggered);
			Assert.IsTrue(transactionFailedEventTriggered);

			Assert.IsNotNull(exception);
			Assert.IsInstanceOf(typeof(CommitResourceException), exception);
		}

		[Test]
		public void TransactionFailedOnRollbackEvent()
		{
			bool transactionCommittedEventTriggered = false;
			bool transactionRolledBackEventTriggered = false;
			bool transactionFailedEventTriggered = false;

			tm.TransactionCommitted += delegate { transactionCommittedEventTriggered = true; };
			tm.TransactionRolledback += delegate { transactionRolledBackEventTriggered = true; };
			tm.TransactionFailed += delegate { transactionFailedEventTriggered = true; };

			ITransaction transaction = tm.CreateTransaction(
				TransactionMode.Requires, IsolationMode.Unspecified);

			ResourceImpl resource = new ThrowsExceptionResourceImpl(false, true);

			transaction.Enlist(resource);

			Assert.IsFalse(transactionCommittedEventTriggered);
			Assert.IsFalse(transactionRolledBackEventTriggered);
			Assert.IsFalse(transactionFailedEventTriggered);

			transaction.Begin();

			Assert.IsFalse(transactionCommittedEventTriggered);
			Assert.IsFalse(transactionRolledBackEventTriggered);
			Assert.IsFalse(transactionFailedEventTriggered);

			TransactionException exception = null;

			try
			{
				transaction.Rollback();
			}
			catch (TransactionException transactionError)
			{
				exception = transactionError;
			}

			Assert.IsFalse(transactionCommittedEventTriggered);
			Assert.IsFalse(transactionRolledBackEventTriggered);
			Assert.IsTrue(transactionFailedEventTriggered);

			Assert.IsNotNull(exception);
			Assert.IsInstanceOf(typeof(RollbackResourceException), exception);
		}

		[Test]
		public void TransactionResources_AreDisposed()
		{
			var t = tm.CreateTransaction(TransactionMode.Requires, IsolationMode.Unspecified);
			var resource = new ResourceImpl();

			t.Enlist(resource);
			t.Begin();

			// lalala

			t.Rollback();
			tm.Dispose(t);

			Assert.IsTrue(resource.wasDisposed);
		}
	}
}
