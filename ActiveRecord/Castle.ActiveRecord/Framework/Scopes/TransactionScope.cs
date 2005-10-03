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

namespace Castle.ActiveRecord
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

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
		private readonly TransactionMode mode;
		private IDictionary _transactions = new HybridDictionary();
		private TransactionScope parentTransactionScope;
		private AbstractScope parentSimpleScope;
		private bool _rollbackOnly;

		public TransactionScope(TransactionMode mode) : base(SessionScopeType.Transactional)
		{
			this.mode = mode;

			ISessionScope previousScope = FindPreviousScope( 
				mode == TransactionMode.Inherits ? true : false );

			if (previousScope != null)
			{
				if (previousScope.ScopeType == SessionScopeType.Transactional)
				{
					parentTransactionScope = previousScope as TransactionScope;
				}
				else
				{
					parentSimpleScope = (AbstractScope) previousScope;

					foreach(ISession session in parentSimpleScope.GetSessions())
					{
						EnsureHasTransaction(session);
					}
				}
			}
		}

		public TransactionScope() : this(TransactionMode.New)
		{
		}

		public void VoteRollBack()
		{
			if (mode == TransactionMode.Inherits && parentTransactionScope != null)
			{
				parentTransactionScope.VoteRollBack();
			}
			_rollbackOnly = true;
		}

		public void VoteCommit()
		{
			if (_rollbackOnly)
			{
				throw new TransactionException("The transaction was marked as rollback only - by itself or one of the nested transactions");
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

		protected void EnsureHasTransaction(ISession session)
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
				if (_rollbackOnly)
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
				if (_rollbackOnly)
				{
					// Cancel all pending changes (not sure whether this is a good idea, it should be scoped

					foreach( ISession session in parentSimpleScope.GetSessions() )
					{
						session.Clear();
					}
				}
			}
		}

		private ISessionScope FindPreviousScope(bool transactional)
		{
			object[] items = ThreadScopeAccessor.Instance.CurrentStack.ToArray();

			ISessionScope first = null;

			for (int i = 0; i < items.Length; i++)
			{
				ISessionScope scope = items[i] as ISessionScope;

				if (scope == this) continue;

				if (first == null) first = scope;

				if (!transactional) break;

				if (transactional && scope.ScopeType == SessionScopeType.Transactional)
				{
					return scope;
				}
			}

			return first;
		}
	}
}