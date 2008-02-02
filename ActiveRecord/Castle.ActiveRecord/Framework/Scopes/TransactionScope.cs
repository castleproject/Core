// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveRecord
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Data;
	using NHibernate;
	using Castle.ActiveRecord.Framework.Scopes;

	/// <summary>
	/// Defines the transaction scope behavior
	/// </summary>
	public enum TransactionMode
	{
		/// <summary>
		/// Inherits a transaction previously create on 
		/// the current context.
		/// </summary>
		Inherits,
		/// <summary>
		/// Always create an isolated transaction context.
		/// </summary>
		New
	}
	
	/// <summary>
	/// Governs the <see cref="TransactionScope"/> behavior 
	/// on dispose if neither <see cref="TransactionScope.VoteCommit"/>
	/// nor <see cref="TransactionScope.VoteRollBack"/> was called
	/// </summary>
	public enum OnDispose
	{
		/// <summary>
		/// Should commit the transaction, unless <see cref="TransactionScope.VoteRollBack"/>
		/// was called before the disposing the scope (this is the default behavior)
		/// </summary>
		Commit,
		/// <summary>
		/// Should rollback the transaction, unless <see cref="TransactionScope.VoteCommit"/>
		/// was called before the disposing the scope
		/// </summary>
		Rollback
	}

	/// <summary>
	/// Implementation of <see cref="ISessionScope"/> to 
	/// provide transaction semantics
	/// </summary>
	public class TransactionScope : SessionScope
	{
		private static readonly object CompletedEvent = new object();

		private readonly TransactionMode mode;
		private readonly IsolationLevel isolationLevel;
		private readonly OnDispose onDisposeBehavior;
		private readonly IDictionary<ISession, ITransaction> transactions = new Dictionary<ISession, ITransaction>();
		private readonly TransactionScope parentTransactionScope;
		private readonly AbstractScope parentSimpleScope;
		private readonly EventHandlerList events = new EventHandlerList();
		private bool rollbackOnly, setForCommit;

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionScope"/> class.
		/// </summary>
		public TransactionScope() : this(TransactionMode.New)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionScope"/> class.
		/// </summary>
		/// <param name="onDisposeBehavior">The on dispose behavior.</param>
		public TransactionScope(OnDispose onDisposeBehavior) : this(TransactionMode.New, onDisposeBehavior)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionScope"/> class.
		/// </summary>
		/// <param name="mode">Whatever to create a new transaction or inherits an existing one</param>
		public TransactionScope(TransactionMode mode) : this(mode, IsolationLevel.Unspecified, OnDispose.Commit)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionScope"/> class.
		/// </summary>
		/// <param name="mode">Whatever to create a new transaction or inherits an existing one</param>
		/// <param name="onDisposeBehavior">The on dispose behavior.</param>
		public TransactionScope(TransactionMode mode, OnDispose onDisposeBehavior) : this(mode, IsolationLevel.Unspecified, onDisposeBehavior)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionScope"/> class.
		/// </summary>
		/// <param name="mode">Whatever to create a new transaction or inherits an existing one</param>
		/// <param name="isolationLevel">The transaction isolation level.</param>
		/// <param name="onDisposeBehavior">The on dispose behavior.</param>
		public TransactionScope(TransactionMode mode, IsolationLevel isolationLevel, OnDispose onDisposeBehavior)
			: base(FlushAction.Auto, SessionScopeType.Transactional)
		{
			this.mode = mode;
			this.isolationLevel = isolationLevel;
			this.onDisposeBehavior = onDisposeBehavior;

			bool preferenceForTransactionScope = mode == TransactionMode.Inherits ? true : false;

			ISessionScope previousScope = ScopeUtil.FindPreviousScope(this, preferenceForTransactionScope);

			if (previousScope != null)
			{
				if (previousScope.ScopeType == SessionScopeType.Transactional)
				{
					parentTransactionScope = previousScope as TransactionScope;
				}
				else
				{
					// This is not a safe cast. Reconsider it
					parentSimpleScope = (AbstractScope) previousScope;

					foreach(ISession session in parentSimpleScope.GetSessions())
					{
						EnsureHasTransaction(session);
					}
				}
			}
		}

		#region OnTransactionCompleted event

		/// <summary>
		/// This event is raised when a transaction is completed
		/// </summary>
		public event EventHandler OnTransactionCompleted
		{
			add
			{
				if (parentTransactionScope != null)
				{
					parentTransactionScope.OnTransactionCompleted += value;
				}
				else
				{
					events.AddHandler(CompletedEvent, value);
				}
			}
			remove
			{
				if (parentTransactionScope != null)
				{
					parentTransactionScope.OnTransactionCompleted -= value;
				}
				else
				{
					events.RemoveHandler(CompletedEvent, value);
				}
			}
		}

		#endregion

		/// <summary>
		/// Votes to roll back the transaction
		/// </summary>
		public void VoteRollBack()
		{
			if (mode == TransactionMode.Inherits && parentTransactionScope != null)
			{
				parentTransactionScope.VoteRollBack();
			}
			rollbackOnly = true;
		}

		/// <summary>
		/// Votes to commit the transaction
		/// </summary>
		public void VoteCommit()
		{
			if (rollbackOnly)
			{
				throw new TransactionException("The transaction was marked as rollback " + "only - by itself or one of the nested transactions");
			}
			setForCommit = true;
		}

		/// <summary>
		/// This method is invoked when the
		/// <see cref="Castle.ActiveRecord.Framework.ISessionFactoryHolder"/>
		/// instance needs a session instance. Instead of creating one it interrogates
		/// the active scope for one. The scope implementation must check if it
		/// has a session registered for the given key.
		/// <seealso cref="RegisterSession"/>
		/// </summary>
		/// <param name="key">an object instance</param>
		/// <returns>
		/// 	<c>true</c> if the key exists within this scope instance
		/// </returns>
		public override bool IsKeyKnown(object key)
		{
			if (mode == TransactionMode.Inherits && parentTransactionScope != null)
			{
				return parentTransactionScope.IsKeyKnown(key);
			}

			bool keyKnown = false;

			if (parentSimpleScope != null)
			{
				keyKnown = parentSimpleScope.IsKeyKnown(key);
			}

			return keyKnown ? true : base.IsKeyKnown(key);
		}

		/// <summary>
		/// This method is invoked when no session was available
		/// at and the <see cref="Castle.ActiveRecord.Framework.ISessionFactoryHolder"/>
		/// just created one. So it registers the session created
		/// within this scope using a key. The scope implementation
		/// shouldn't make any assumption on what the key
		/// actually is as we reserve the right to change it
		/// <seealso cref="IsKeyKnown"/>
		/// </summary>
		/// <param name="key">an object instance</param>
		/// <param name="session">An instance of <c>ISession</c></param>
		public override void RegisterSession(object key, ISession session)
		{
			if (mode == TransactionMode.Inherits && parentTransactionScope != null)
			{
				parentTransactionScope.RegisterSession(key, session);
			}
			else if (parentSimpleScope != null)
			{
				parentSimpleScope.RegisterSession(key, session);
			}

			base.RegisterSession(key, session);
		}

		/// <summary>
		/// This method should return the session instance associated with the key.
		/// </summary>
		/// <param name="key">an object instance</param>
		/// <returns>
		/// the session instance or null if none was found
		/// </returns>
		public override ISession GetSession(object key)
		{
			if (mode == TransactionMode.Inherits && parentTransactionScope != null)
			{
				return parentTransactionScope.GetSession(key);
			}

			ISession session = null;

			if (parentSimpleScope != null)
			{
				session = parentSimpleScope.GetSession(key);
			}

			session = session ?? base.GetSession(key);

			EnsureHasTransaction(session);

			return session;
		}

		/// <summary>
		/// Flushes the sessions that this scope 
		/// or its parents are maintaining
		/// </summary>
		public override void Flush()
		{
			if (mode == TransactionMode.Inherits && parentTransactionScope != null)
			{
				parentTransactionScope.Flush();
			}

			if (parentSimpleScope != null)
			{
				parentSimpleScope.Flush();
			}

			base.Flush();
		}

		/// <summary>
		/// Ensures that a transaction exist, creating one if neccecary
		/// </summary>
		/// <param name="session">The session.</param>
		protected internal void EnsureHasTransaction(ISession session)
		{
			if (!transactions.ContainsKey(session))
			{
				session.FlushMode = FlushMode.Commit;

				ITransaction transaction;

				if (isolationLevel == IsolationLevel.Unspecified)
				{
					transaction = session.BeginTransaction();
				}
				else
				{
					transaction = session.BeginTransaction(isolationLevel);
				}

				transactions.Add(session, transaction);
			}
		}

		/// <summary>
		/// Initializes the current transaction scope using the session
		/// </summary>
		/// <param name="session">The session.</param>
		protected override void Initialize(ISession session)
		{
			if (mode == TransactionMode.Inherits && parentTransactionScope != null)
			{
				parentTransactionScope.EnsureHasTransaction(session);
				return;
			}

			EnsureHasTransaction(session);
		}

		/// <summary>
		/// Dispose of this scope
		/// </summary>
		/// <param name="sessions">The sessions.</param>
		protected override void PerformDisposal(ICollection<ISession> sessions)
		{
			if (!setForCommit && !rollbackOnly) // Neither VoteCommit or VoteRollback were called
			{
				if (onDisposeBehavior == OnDispose.Rollback)
				{
					VoteRollBack();
				}
			}
			
			if (mode == TransactionMode.Inherits && parentTransactionScope != null)
			{
				// In this case it's not up to this instance to perform the clean up
				return;
			}

			Exception transactionError = null;

			foreach(ITransaction transaction in transactions.Values)
			{
				try
				{
					if (rollbackOnly)
					{
						transaction.Rollback();
					}
					else
					{
						transaction.Commit();
					}
				}
				catch(Exception ex)
				{
					transactionError = ex;

					transaction.Dispose();
				}
			}

			if (parentSimpleScope == null)
			{
				// No flush necessary, but we should close the session

				PerformDisposal(sessions, false, true);
			}
			else
			{
				if (rollbackOnly)
				{
					// Cancel all pending changes 
					// (not sure whether this is a good idea, it should be scoped

					foreach(ISession session in parentSimpleScope.GetSessions())
					{
						session.Clear();
					}
				}
			}

			RaiseOnCompleted();

			if (transactionError != null)
			{
				throw new TransactionException("An error occured when trying to dispose the transaction", transactionError);
			}
		}

		/// <summary>
		/// This is called when a session has a failure
		/// </summary>
		/// <param name="session">the session</param>
		public override void FailSession(ISession session)
		{
			VoteRollBack();
		}

		/// <summary>
		/// Discards the sessions.
		/// </summary>
		/// <param name="sessions">The sessions.</param>
		protected internal override void DiscardSessions(ICollection<ISession> sessions)
		{
			if (parentSimpleScope != null)
			{
				parentSimpleScope.DiscardSessions(sessions);
			}
		}

		/// <summary>
		/// Raises the on completed event
		/// </summary>
		private void RaiseOnCompleted()
		{
			EventHandler handler = (EventHandler) events[CompletedEvent];

			if (handler != null)
			{
				handler(this, EventArgs.Empty);
			}
		}
	}
}
