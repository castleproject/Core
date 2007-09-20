// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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
	using System.ComponentModel;
	using Castle.Core.Logging;

	/// <summary>
	/// TODO: Ensure this class is thread-safe
	/// </summary>
	public class DefaultTransactionManager : MarshalByRefObject, ITransactionManager
	{
		private static readonly object TransactionCreatedEvent = new object();
		private static readonly object TransactionCommittedEvent = new object();
		private static readonly object TransactionRolledbackEvent = new object();
		private static readonly object TransactionDisposedEvent = new object();
		private static readonly object ChildTransactionCreatedEvent = new object();

		private EventHandlerList events = new EventHandlerList();
		private ILogger logger = NullLogger.Instance;
		private IActivityManager activityManager;

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultTransactionManager"/> class.
		/// </summary>
		public DefaultTransactionManager() : this(new CallContextActivityManager())
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DefaultTransactionManager"/> class.
		/// </summary>
		/// <param name="activityManager">The activity manager.</param>
		public DefaultTransactionManager(IActivityManager activityManager)
		{
			this.activityManager = activityManager;
		}

		/// <summary>
		/// Gets or sets the activity manager.
		/// </summary>
		/// <value>The activity manager.</value>
		public IActivityManager ActivityManager
		{
			get { return activityManager; }
			set { activityManager = value; }
		}

		/// <summary>
		/// Gets or sets the logger.
		/// </summary>
		/// <value>The logger.</value>
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

		/// <summary>
		/// Creates a transaction.
		/// </summary>
		/// <param name="transactionMode">The transaction mode.</param>
		/// <param name="isolationMode">The isolation mode.</param>
		/// <returns></returns>
		public virtual ITransaction CreateTransaction(TransactionMode transactionMode, IsolationMode isolationMode)
		{
			return CreateTransaction(transactionMode, isolationMode, false);
		}

		/// <summary>
		/// Creates a transaction.
		/// </summary>
		/// <param name="transactionMode">The transaction mode.</param>
		/// <param name="isolationMode">The isolation mode.</param>
		/// <param name="distributedTransaction">if set to <c>true</c>, the TM will create a distributed transaction.</param>
		/// <returns></returns>
		public virtual ITransaction CreateTransaction(TransactionMode transactionMode, IsolationMode isolationMode, bool distributedTransaction)
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

			AbstractTransaction transaction = null;

			if (CurrentTransaction != null)
			{
				if (transactionMode == TransactionMode.Requires || transactionMode == TransactionMode.Supported)
				{
					transaction = ((StandardTransaction) CurrentTransaction).CreateChildTransaction();

					RaiseChildTransactionCreated(transaction, transactionMode, isolationMode, distributedTransaction);

					logger.DebugFormat("Child Transaction {0} created", transaction.GetHashCode());
				}
			}

			if (transaction == null)
			{
				transaction = InstantiateTransaction(transactionMode, isolationMode, distributedTransaction);

				if (distributedTransaction)
				{
#if MONO
					throw new TransactionException("Distributed transactions are not supported on Mono");
#else
					transaction.Enlist(new TransactionScopeResourceAdapter(transactionMode, isolationMode));
#endif
				}

				RaiseTransactionCreated(transaction, transactionMode, isolationMode, distributedTransaction);

				logger.DebugFormat("Transaction {0} created", transaction.GetHashCode());
			}

			transaction.Logger = logger.CreateChildLogger(transaction.GetType().FullName);

			activityManager.CurrentActivity.Push(transaction);

			return transaction;
		}

		/// <summary>
		/// Factory method for creating a transaction.
		/// </summary>
		/// <param name="transactionMode">The transaction mode.</param>
		/// <param name="isolationMode">The isolation mode.</param>
		/// <param name="distributedTransaction">if set to <c>true</c>, the TM will create a distributed transaction.</param>
		/// <returns>A transaction</returns>
		protected virtual AbstractTransaction InstantiateTransaction(TransactionMode transactionMode, IsolationMode isolationMode, bool distributedTransaction)
		{
			return new StandardTransaction(
				new TransactionDelegate(RaiseTransactionCommitted),
				new TransactionDelegate(RaiseTransactionRolledback), transactionMode, isolationMode, distributedTransaction);
		}

		public virtual ITransaction CurrentTransaction
		{
			get { return activityManager.CurrentActivity.CurrentTransaction; }
		}

		#region events

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

		protected void RaiseTransactionCreated(ITransaction transaction, TransactionMode transactionMode,
		                                       IsolationMode isolationMode, bool distributedTransaction)
		{
			TransactionCreationInfoDelegate eventDelegate = (TransactionCreationInfoDelegate) events[TransactionCreatedEvent];

			if (eventDelegate != null)
			{
				eventDelegate(transaction, transactionMode, isolationMode, distributedTransaction);
			}
		}

		protected void RaiseChildTransactionCreated(ITransaction transaction, TransactionMode transactionMode,
													IsolationMode isolationMode, bool distributedTransaction)
		{
			TransactionCreationInfoDelegate eventDelegate =
				(TransactionCreationInfoDelegate) events[ChildTransactionCreatedEvent];

			if (eventDelegate != null)
			{
				eventDelegate(transaction, transactionMode, isolationMode, distributedTransaction);
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

		#endregion

		public virtual void Dispose(ITransaction transaction)
		{
			if (transaction == null)
			{
				throw new ArgumentNullException("transaction", "Tried to dispose a null transaction");
			}

			if (CurrentTransaction != transaction)
			{
				throw new ArgumentException("transaction",
				                            "Tried to dispose a transaction that is not on the current active transaction");
			}

			activityManager.CurrentActivity.Pop();

			if (transaction is IDisposable)
			{
				(transaction as IDisposable).Dispose();
			}

			RaiseTransactionDisposed(transaction);

			logger.DebugFormat("Transaction {0} disposed successfully", transaction.GetHashCode());
		}

		#endregion

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
				                 "explicit says that no transaction is supported for this context";

				logger.Error(message);

				throw new TransactionException(message);
			}
		}
	}
}
