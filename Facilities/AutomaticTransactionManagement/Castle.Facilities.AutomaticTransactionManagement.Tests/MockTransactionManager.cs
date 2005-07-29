// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.AutomaticTransactionManagement.Tests
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

		public event TransactionCreationInfoDelegate TransactionCreated;

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

	public class MockTransaction : AbstractTransaction
	{

	}
}
