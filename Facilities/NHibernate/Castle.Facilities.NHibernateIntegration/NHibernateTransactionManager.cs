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

namespace Castle.Facilities.NHibernateIntegration
{
	using System;

	using Castle.Model;

	using Castle.MicroKernel;

	using Castle.Services.Transaction;

	using Castle.Facilities.NHibernateExtension;
	
	using NHibernate;

	/// <summary>
	/// Dispatch the transaction management to the
	/// Session transaction implementation.
	/// </summary>
	[PerThread]
	public class NHibernateTransactionManager : ITransactionManager
	{
		private IKernel _kernel;
		private NHibernateTransaction _currentTransaction;

		/// <summary>
		/// Constructs a NHibernateTransactionManager. 
		/// </summary>
		/// <param name="kernel"></param>
		public NHibernateTransactionManager(IKernel kernel)
		{
			_kernel = kernel;
		}

		public Castle.Services.Transaction.ITransaction CreateTransaction(
			TransactionMode transactionMode, IsolationMode isolationMode)
		{
			if (_currentTransaction != null)
			{
				if (transactionMode != TransactionMode.RequiresNew)
				{
					return _currentTransaction.CreateVoteTransaction();
				}
			}

			ISession session = SessionManager.CurrentSession;

			if (session == null)
			{
				String message = "The NHibernateTransactionManager requires that " +
					"the ISession is available through the ISessionManager.";
				throw new ApplicationException(message);
			}

			_currentTransaction = new NHibernateTransaction(session);

			return _currentTransaction;
		}

		public void Dispose(Castle.Services.Transaction.ITransaction transaction)
		{
			_currentTransaction = null;
		}

		public Castle.Services.Transaction.ITransaction CurrentTransaction
		{
			get { return _currentTransaction; }
		}
	}

	internal class TransactionStack
	{
		private NHibernateTransaction[] _transactions;
		private int _top = -1;

		public TransactionStack(int size)
		{
			_transactions = new NHibernateTransaction[size];
		}

		public TransactionStack() : this(100)
		{
		}

		public void Push(NHibernateTransaction transaction)
		{
			_transactions[++_top] = transaction;
		}

		public NHibernateTransaction Peek
		{
			get { return _transactions[_top]; }
		}

		public NHibernateTransaction Pop()
		{
			return _transactions[_top--];
		}
	}
}