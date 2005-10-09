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
	using System.ComponentModel;

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

		private EventHandlerList _events = new EventHandlerList();

		private Stack _transactions = new Stack(5);

		public DefaultTransactionManager()
		{
		}

		public override object InitializeLifetimeService()
		{
			return null;
		}

		#region ITransactionManager Members

		public event TransactionCreationInfoDelegate TransactionCreated
		{
			add { _events.AddHandler(TransactionCreatedEvent, value); }
			remove { _events.RemoveHandler(TransactionCreatedEvent, value); }
		}

		public event TransactionCreationInfoDelegate ChildTransactionCreated
		{
			add { _events.AddHandler(ChildTransactionCreatedEvent, value); }
			remove { _events.RemoveHandler(ChildTransactionCreatedEvent, value); }
		}

		public event TransactionDelegate TransactionCommitted
		{
			add { _events.AddHandler(TransactionCommittedEvent, value); }
			remove { _events.RemoveHandler(TransactionCommittedEvent, value); }
		}

		public event TransactionDelegate TransactionRolledback
		{
			add { _events.AddHandler(TransactionRolledbackEvent, value); }
			remove { _events.RemoveHandler(TransactionRolledbackEvent, value); }
		}

		public event TransactionDelegate TransactionDisposed
		{
			add { _events.AddHandler(TransactionDisposedEvent, value); }
			remove { _events.RemoveHandler(TransactionDisposedEvent, value); }
		}

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
				}
			}

			if (transaction == null)
			{
				transaction = new StandardTransaction(
					new TransactionDelegate(RaiseTransactionCommitted), 
					new TransactionDelegate(RaiseTransactionRolledback) );

				RaiseTransactionCreated(transaction, transactionMode, isolationMode);
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

		public virtual ITransaction CurrentTransaction
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

		public virtual void Dispose(ITransaction transaction)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction", "Tried to dispose a null transaction");
			}

			lock(_transactions)
			{
				if (CurrentTransaction != transaction)
				{
					throw new ArgumentException("transaction", "Tried to dispose a transaction that is not on the current active transaction");
				}

				_transactions.Pop();
			}

			if (transaction is IDisposable)
			{
				(transaction as IDisposable).Dispose();
			}

			RaiseTransactionDisposed(transaction);
		}

		#endregion

		protected void RaiseTransactionCreated(ITransaction transaction, TransactionMode transactionMode, IsolationMode isolationMode)
		{
			TransactionCreationInfoDelegate eventDelegate = (TransactionCreationInfoDelegate) _events[TransactionCreatedEvent];
			
			if (eventDelegate != null)
			{
				eventDelegate(transaction, transactionMode, isolationMode);
			}
		}

		protected void RaiseChildTransactionCreated(ITransaction transaction, TransactionMode transactionMode, IsolationMode isolationMode)
		{
			TransactionCreationInfoDelegate eventDelegate = (TransactionCreationInfoDelegate) _events[ChildTransactionCreatedEvent];
			
			if (eventDelegate != null)
			{
				eventDelegate(transaction, transactionMode, isolationMode);
			}
		}

		protected void RaiseTransactionDisposed(ITransaction transaction)
		{
			TransactionDelegate eventDelegate = (TransactionDelegate) _events[TransactionDisposedEvent];
			
			if (eventDelegate != null)
			{
				eventDelegate(transaction);
			}
		}

		protected void RaiseTransactionCommitted(ITransaction transaction)
		{
			TransactionDelegate eventDelegate = (TransactionDelegate) _events[TransactionCommittedEvent];
			
			if (eventDelegate != null)
			{
				eventDelegate(transaction);
			}
		}

		protected void RaiseTransactionRolledback(ITransaction transaction)
		{
			TransactionDelegate eventDelegate = (TransactionDelegate) _events[TransactionRolledbackEvent];
			
			if (eventDelegate != null)
			{
				eventDelegate(transaction);
			}
		}

		protected virtual TransactionMode ObtainDefaultTransactionMode(TransactionMode transactionMode)
		{
			return TransactionMode.Requires;
		}
	}
}
