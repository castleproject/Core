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

	using Castle.Services.Transaction;

	using ITransaction = NHibernate.ITransaction;

	using NHibernate;

	/// <summary>
	/// Summary description for NHibernateTransaction.
	/// </summary>
	/// <summary>
	/// 
	/// </summary>
	public class NHibernateTransaction : AbstractTransaction
	{
		private ISession _session;
		private ITransaction _innerTransaction;

		internal NHibernateTransaction(ISession session)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}

			_session = session;
		}

		public override void Enlist(IResource resource)
		{
			throw new NotImplementedException();
		}

		public override void Rollback()
		{
			base.Rollback();

			_innerTransaction.Rollback();
		}

		public override void Commit()
		{
			base.Commit();
			
			_innerTransaction.Commit();
		}

		public override void Begin()
		{
			base.Begin();
			
			_innerTransaction = _session.BeginTransaction();
		}

		public override bool WasRolledBack
		{
			get
			{
				CheckState();
				return _innerTransaction.WasRolledBack;
			}
		}

		public override bool WasCommitted
		{
			get
			{
				CheckState();
				return _innerTransaction.WasCommitted;
			}
		}

		protected override void CheckState()
		{
			if (_innerTransaction == null)
			{
				throw new ApplicationException(
					"You must start, i.e. Begin(), the transaction first");
			}
		}

		public Services.Transaction.ITransaction CreateVoteTransaction()
		{
			return new VoteTransaction(this);
		}
		
		public virtual void RegisterCommitVote()
		{
			// This implementation only cares for rollbacks
		}

		public virtual void RegisterRollbackVote()
		{
//			hasNestedRollback 
		}

		/// <summary>
		/// 
		/// </summary>
		private class VoteTransaction : AbstractTransaction
		{
			private NHibernateTransaction _transaction;

			public VoteTransaction(NHibernateTransaction transaction)
			{
				_transaction = transaction;
			}

			public override void Enlist(IResource resource)
			{
				throw new NotImplementedException();
			}

			public override void Rollback()
			{
				base.Rollback();

				_transaction.RegisterRollbackVote();
			}

			public override void Commit()
			{
				base.Commit();

				_transaction.RegisterCommitVote();
			}

			public override void Begin()
			{
				base.Begin();
			}

			public override bool WasRolledBack
			{
				get { return _state == TransactionState.Rolledback; }
			}

			public override bool WasCommitted
			{
				get { return _state == TransactionState.Committed; }
			}
		}
	}
}