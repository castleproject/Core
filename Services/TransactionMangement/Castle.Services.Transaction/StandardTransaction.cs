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

	/// <summary>
	/// Implements a transaction root.
	/// </summary>
	public class StandardTransaction : AbstractTransaction
	{
		private IList _childs = new ArrayList();
		private bool _rollbackOnly;

		public StandardTransaction CreateChildTransaction()
		{
			ChildTransaction child = new ChildTransaction(this);
			_childs.Add(child);
			return child;
		}

		public override void Commit()
		{
			if (_rollbackOnly)
			{
				throw new TransactionException("Can't commit as one of the child transactions rolledback");
			}

			base.Commit();
		}

		/// <summary>
		/// Invoked by child transactions, meaning that 
		/// some sort of error has occured, so 
		/// </summary>
		public virtual void ChildTransactionRolledBack()
		{
			_rollbackOnly = true;
		}
	}

	/// <summary>
	/// Emulates a standalone transaction but in fact it 
	/// just propages a transaction. 
	/// </summary>
	public class ChildTransaction : StandardTransaction
	{
		private StandardTransaction _parent;

		public ChildTransaction(StandardTransaction parent)
		{
			_parent = parent;
		}

		public override void Enlist(IResource resource)
		{
			_parent.Enlist(resource);
		}

		public override void Begin()
		{
			// Ignored
		}

		public override void Rollback()
		{
			// Vote as rollback

			_parent.ChildTransactionRolledBack();
		}

		public override void Commit()
		{
			// Vote as commit
		}

		public override void RegisterSynchronization(ISynchronization synchronization)
		{
			_parent.RegisterSynchronization(synchronization);
		}

		public override IDictionary Context
		{
			get { return _parent.Context; }
		}

		public override void ChildTransactionRolledBack()
		{
			_parent.ChildTransactionRolledBack();
		}
	}
}
