// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.Facilties.AutomaticTransactionManagement.Tests
{
	using System;

	using Castle.Services.Transaction;

	/// <summary>
	/// Summary description for MockTransactionManager.
	/// </summary>
	public class MockTransactionManager : ITransactionManager
	{
		private MockTransaction _current;
		private int _transactions;
		private int _committedCount;
		private int _rolledBackCount;

		public MockTransactionManager()
		{
		}

		public ITransaction CreateTransaction(TransactionMode transactionMode, IsolationMode isolationMode)
		{
			_current = new MockTransaction();

			_transactions++;

			return _current;
		}

		public void Dispose(ITransaction tran)
		{
			MockTransaction transaction = (MockTransaction) tran;

			if (transaction.Status == TransactionStatus.Committed)
			{
				_committedCount++;
			}
			else
			{
				_rolledBackCount++;
			}

			_current = null;
		}

		public ITransaction CurrentTransaction
		{
			get { return _current; }
		}

		public int TransactionCount
		{
			get { return _transactions; }
		}

		public int CommittedCount
		{
			get { return _committedCount; }
		}

		public int RolledBackCount
		{
			get { return _rolledBackCount; }
		}
	}

	enum TransactionStatus
	{
		NotStarted,
		Started,
		Committed,
		Rolledback
	}

	public class MockTransaction : ITransaction
	{
		private TransactionStatus _status = TransactionStatus.NotStarted;

		internal TransactionStatus Status
		{
			get { return _status; }
		}

		public void Begin()
		{
			if (_status != TransactionStatus.NotStarted) 
				throw new InvalidOperationException("Invalid transaction status");
			_status = TransactionStatus.Started;
		}

		public void Commit()
		{
			if (_status != TransactionStatus.Started) 
				throw new InvalidOperationException("Invalid transaction status");
			_status = TransactionStatus.Committed;
		}

		public void Rollback()
		{
			if (_status != TransactionStatus.Started) 
				throw new InvalidOperationException("Invalid transaction status");
			_status = TransactionStatus.Rolledback;
		}

		public void Enlist(IResource resource)
		{
			throw new NotImplementedException();
		}

		public bool WasRolledBack
		{
			get { return _status == TransactionStatus.Rolledback; }
		}

		public bool WasCommitted
		{
			get { return _status == TransactionStatus.Committed; }
		}
	}
}
