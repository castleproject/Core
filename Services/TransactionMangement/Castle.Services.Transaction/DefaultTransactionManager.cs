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

namespace Castle.Services.Transaction
{
	using System;
	using System.Collections;

	public class DefaultTransactionManager : ITransactionManager
	{
		private Stack _transactions = new Stack(5);

		public DefaultTransactionManager()
		{
		}

		#region ITransactionManager Members

		public ITransaction CreateTransaction(TransactionMode transactionMode, IsolationMode isolationMode)
		{
			if (transactionMode == TransactionMode.Unspecified)
			{
				transactionMode = ObtainDefaultTransactionMode(transactionMode);
			}

			CheckNotSupportedTransaction(transactionMode);

			if (CurrentTransaction == null && 
				(transactionMode == TransactionMode.Supported || 
				 transactionMode == TransactionMode.NotSupported))
			{
				return null;
			}

			StandardTransaction transaction = null;

			if (CurrentTransaction != null)
			{
				if (transactionMode == TransactionMode.Requires || transactionMode == TransactionMode.Supported)
				{
					transaction = (CurrentTransaction as StandardTransaction).CreateChildTransaction();
				}
			}

			if (transaction == null)
			{
				transaction = new StandardTransaction();
			}

			_transactions.Push(transaction);

			return transaction;
		}

		private void CheckNotSupportedTransaction(TransactionMode transactionMode)
		{
			if (transactionMode == TransactionMode.NotSupported && 
				CurrentTransaction != null && 
				CurrentTransaction.Status == TransactionStatus.Active)
			{
				throw new TransactionException("There is a transaction active and the transaction mode " + 
					"specified explicit says that no transaction is supported for this context");
			}
		}

		public ITransaction CurrentTransaction
		{
			get
			{
				if (_transactions.Count == 0)
				{
					return null;
				}
				return _transactions.Peek() as ITransaction;
			}
		}

		public void Dispose(ITransaction transaction)
		{
			if (transaction is IDisposable)
			{
				(transaction as IDisposable).Dispose();
			}
		}

		#endregion

		protected virtual TransactionMode ObtainDefaultTransactionMode(TransactionMode transactionMode)
		{
			return TransactionMode.Requires;
		}
	}
}
