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

	using NHibernate;


	public class TransactionScope : SessionScope
	{
		private IList _transactions = new ArrayList();
		private bool _rollbackOnly;

		public TransactionScope()
		{
		}

		public void VoteRollBack()
		{
			_rollbackOnly = true;
		}

		public void VoteCommit()
		{
			// Nothing to do as it's always assume commit
		}

		protected override void Initialize(ISession session)
		{
			session.FlushMode = FlushMode.Commit;
			ITransaction transaction = session.BeginTransaction();

			_transactions.Add(transaction);
		}

		protected override void PerformDisposal(ICollection sessions)
		{
			foreach(ITransaction transaction in _transactions)
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

			base.PerformDisposal(sessions);
		}
	}
}
