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

namespace Castle.ActiveRecord
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;
	using System.ComponentModel;

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
	/// Implementation of <see cref="ISessionScope"/> to 
	/// provide transaction semantics
	/// </summary>
	public class TransactionScope : SessionScope
	{
		private static readonly object CompletedEvent = new object();

		private readonly TransactionMode mode;
		private IDictionary _transactions = new HybridDictionary();
		private TransactionScope parentTransactionScope;
		private AbstractScope parentSimpleScope;
		private EventHandlerList events = new EventHandlerList();
		private bool rollbackOnly;

		public TransactionScope() : this(TransactionMode.New)
		{
		}

		public TransactionScope(TransactionMode mode) : base(FlushAction.Auto, SessionScopeType.Transactional)
		{
			this.mode = mode;

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

		public void VoteRollBack()
		{
			if (mode == TransactionMode.Inherits && parentTransactionScope != null)
			{
				parentTransactionScope.VoteRollBack();
			}
			rollbackOnly = true;
		}

		public void VoteCommit()
		{
			if (rollbackOnly)
			{
				throw new TransactionException("The transaction was marked as rollback " + 
					"only - by itself or one of the nested transactions");
			}
		}

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

			session = session != null ? session : base.GetSession(key);

			EnsureHasTransaction(session);

			return session;
		}

		protected internal void EnsureHasTransaction(ISession session)
		{
			if (!_transactions.Contains(session))
			{
				session.FlushMode = FlushMode.Commit;

				ITransaction transaction = session.BeginTransaction();

				_transactions.Add(session, transaction);
			}
		}

		protected override void Initialize(ISession session)
		{
			if (mode == TransactionMode.Inherits && parentTransactionScope != null)
			{
				parentTransactionScope.EnsureHasTransaction(session);
				return;
			}

			EnsureHasTransaction(session);
		}

		protected override void PerformDisposal(ICollection sessions)
		{
			if (mode == TransactionMode.Inherits && parentTransactionScope != null)
			{
				// In this case it's not up to this instance to perform the clean up
				return;
			}

			foreach (ITransaction transaction in _transactions.Values)
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

			if (parentSimpleScope == null)
			{
				// No flush necessary, but we should close the session

				base.PerformDisposal(sessions, false, true);
			}
			else
			{
				if (rollbackOnly)
				{
					// Cancel all pending changes 
					// (not sure whether this is a good idea, it should be scoped

					foreach( ISession session in parentSimpleScope.GetSessions() )
					{
						session.Clear();
					}
				}
			}

			RaiseOnCompleted();
		}

		protected internal override void DiscardSessions(ICollection sessions)
		{
			if (parentSimpleScope != null)
			{
				parentSimpleScope.DiscardSessions(sessions);
			}
		}

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