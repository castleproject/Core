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

namespace Castle.Services.Transaction
{
	using System;
	using System.Collections;
	using System.ComponentModel;

	using Castle.Services.Logging;

	/// <summary>
	/// TODO: Ensure this class is thread-safe
	/// </summary>
	public class DefaultTransactionManager : MarshalByRefObject, ITransactionManager
	{
		private static readonly object TransactionCreatedEvent = new object();
		private static readonly object ChildTransactionCreatedEvent = new object();
		private static readonly object TransactionCommittedEvent = new object();
		private static readonly object TransactionRolledbackEvent = new object();
		private static readonly object TransactionDisposedEvent = new object();

		private EventHandlerList events = new EventHandlerList();
		private ILogger logger = new NullLogger();
		private Stack transactions = new Stack(5);

		public DefaultTransactionManager()
		{
		}

		public ILogger Logger
		{
			get { return logger; }
			set { logger = value; }
		}

		#region MarshalByRefObject

		public override object InitializeLifetimeService()
		{
			return null;
		}

		#endregion

		#region ITransactionManager Members

		public virtual ITransaction CreateTransaction(TransactionMode transactionMode, IsolationMode isolationMode)
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

					RaiseChildTransactionCreated(transaction, transactionMode, isolationMode);

					logger.Debug("Child Transaction {0} created", transaction.GetHashCode());
				}
			}

			if (transaction == null)
			{
				transaction = new StandardTransaction(
					new TransactionDelegate(RaiseTransactionCommitted), 
					new TransactionDelegate(RaiseTransactionRolledback) );

				RaiseTransactionCreated(transaction, transactionMode, isolationMode);

				logger.Debug("Transaction {0} created", transaction.GetHashCode());
			}

			transaction.Logger = logger.CreateChildLogger( transaction.GetType().FullName );

			transactions.Push(transaction);

			return transaction;
		}

		public event TransactionCreationInfoDelegate TransactionCreated
		{
			add { events.AddHandler(TransactionCreatedEvent, value); }
			remove { events.RemoveHandler(TransactionCreatedEvent, value); }
		}

		public event TransactionCreationInfoDelegate ChildTransactionCreated
		{
			add { events.AddHandler(ChildTransactionCreatedEvent, value); }
			remove { events.RemoveHandler(ChildTransactionCreatedEvent, value); }
		}

		public event TransactionDelegate TransactionCommitted
		{
			add { events.AddHandler(TransactionCommittedEvent, value); }
			remove { events.RemoveHandler(TransactionCommittedEvent, value); }
		}

		public event TransactionDelegate TransactionRolledback
		{
			add { events.AddHandler(TransactionRolledbackEvent, value); }
			remove { events.RemoveHandler(TransactionRolledbackEvent, value); }
		}

		public event TransactionDelegate TransactionDisposed
		{
			add { events.AddHandler(TransactionDisposedEvent, value); }
			remove { events.RemoveHandler(TransactionDisposedEvent, value); }
		}

		public virtual ITransaction CurrentTransaction
		{
			get
			{
				if (transactions.Count == 0)
				{
					return null;
				}
				return transactions.Peek() as ITransaction;
			}
		}

		public virtual void Dispose(ITransaction transaction)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction", "Tried to dispose a null transaction");
			}

			lock(transactions)
			{
				if (CurrentTransaction != transaction)
				{
					throw new ArgumentException("transaction", "Tried to dispose a transaction that is not on the current active transaction");
				}

				transactions.Pop();
			}

			if (transaction is IDisposable)
			{
				(transaction as IDisposable).Dispose();
			}

			RaiseTransactionDisposed(transaction);

			logger.Debug("Transaction {0} disposed successfully", transaction.GetHashCode());
		}

		#endregion

		protected void RaiseTransactionCreated(ITransaction transaction, TransactionMode transactionMode, IsolationMode isolationMode)
		{
			TransactionCreationInfoDelegate eventDelegate = (TransactionCreationInfoDelegate) events[TransactionCreatedEvent];
			
			if (eventDelegate != null)
			{
				eventDelegate(transaction, transactionMode, isolationMode);
			}
		}

		protected void RaiseChildTransactionCreated(ITransaction transaction, TransactionMode transactionMode, IsolationMode isolationMode)
		{
			TransactionCreationInfoDelegate eventDelegate = (TransactionCreationInfoDelegate) events[ChildTransactionCreatedEvent];
			
			if (eventDelegate != null)
			{
				eventDelegate(transaction, transactionMode, isolationMode);
			}
		}

		protected void RaiseTransactionDisposed(ITransaction transaction)
		{
			TransactionDelegate eventDelegate = (TransactionDelegate) events[TransactionDisposedEvent];
			
			if (eventDelegate != null)
			{
				eventDelegate(transaction);
			}
		}

		protected void RaiseTransactionCommitted(ITransaction transaction)
		{
			TransactionDelegate eventDelegate = (TransactionDelegate) events[TransactionCommittedEvent];
			
			if (eventDelegate != null)
			{
				eventDelegate(transaction);
			}
		}

		protected void RaiseTransactionRolledback(ITransaction transaction)
		{
			TransactionDelegate eventDelegate = (TransactionDelegate) events[TransactionRolledbackEvent];
			
			if (eventDelegate != null)
			{
				eventDelegate(transaction);
			}
		}

		protected virtual TransactionMode ObtainDefaultTransactionMode(TransactionMode transactionMode)
		{
			return TransactionMode.Requires;
		}

		private void CheckNotSupportedTransaction(TransactionMode transactionMode)
		{
			if (transactionMode == TransactionMode.NotSupported && 
				CurrentTransaction != null && 
				CurrentTransaction.Status == TransactionStatus.Active)
			{
				String message = "There is a transaction active and the transaction mode " + 
					"specified explicit says that no transaction is supported for this context";

				logger.Error(message);

				throw new TransactionException(message);
			}
		}
	}
}
