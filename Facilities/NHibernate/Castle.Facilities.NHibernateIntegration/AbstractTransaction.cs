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

	internal enum TransactionState
	{
		Undefined,
		Started,
		Committed,
		Rolledback
	}

	/// <summary>
	/// Summary description for AbstractTransaction.
	/// </summary>
	public abstract class AbstractTransaction : Castle.Services.Transaction.ITransaction
	{
		internal TransactionState _state = TransactionState.Undefined;

		public AbstractTransaction()
		{
		}

		public virtual void Enlist(IResource resource)
		{
			throw new NotImplementedException();
		}

		public virtual void Rollback()
		{
			AssertState(TransactionState.Started);
			_state = TransactionState.Rolledback;
		}

		public virtual void Commit()
		{
			AssertState(TransactionState.Started);
			_state = TransactionState.Committed;
		}

		public virtual void Begin()
		{
			AssertState(TransactionState.Undefined);
			_state = TransactionState.Started;
		}

		abstract public bool WasRolledBack
		{
			get;
		}

		abstract public bool WasCommitted
		{
			get;
		}

		internal virtual void AssertState(TransactionState state)
		{
			if (_state != state)
			{
				throw new ApplicationException(
					"Invalid state in the transaction to perform the requested action.");
			}
		}

		protected virtual void CheckState()
		{
		}
	}
}
