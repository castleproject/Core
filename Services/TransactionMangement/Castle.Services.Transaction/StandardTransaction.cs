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

	/// <summary>
	/// Implements a transaction root.
	/// </summary>
	public class StandardTransaction : AbstractTransaction
	{
		private readonly TransactionDelegate onTransactionCommitted;
		private readonly TransactionDelegate onTransactionRolledback;

		private IList children = ArrayList.Synchronized( new ArrayList() );
		private bool rollbackOnly;

		public StandardTransaction(TransactionDelegate onTransactionCommitted, TransactionDelegate onTransactionRolledback)
		{
			this.onTransactionCommitted = onTransactionCommitted;
			this.onTransactionRolledback = onTransactionRolledback;
		}

		public StandardTransaction()
		{
		}

		public StandardTransaction CreateChildTransaction()
		{
			ChildTransaction child = new ChildTransaction(this);
			
			children.Add(child);

			return child;
		}

		public override bool IsChildTransaction
		{
			get { return false; }
		}

		public override void Commit()
		{
			if (rollbackOnly)
			{
				throw new TransactionException("Can't commit as one of the child transactions rolledback");
			}

			base.Commit();

			if (onTransactionCommitted != null) onTransactionCommitted(this);
		}

		public override void Rollback()
		{
			base.Rollback();

			if (onTransactionRolledback != null) onTransactionRolledback(this);
		}

		public override bool IsRollbackOnlySet
		{
			get { return rollbackOnly; }
		}

		/// <summary>
		/// Invoked by child transactions, meaning that 
		/// some sort of error has occured, so 
		/// </summary>
		public virtual void ChildTransactionRolledBack()
		{
			rollbackOnly = true;
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

		public override bool IsChildTransaction
		{
			get { return true; }
		}

		public override bool IsRollbackOnlySet
		{
			get { return _parent.IsRollbackOnlySet; }
		}

		public override void ChildTransactionRolledBack()
		{
			_parent.ChildTransactionRolledBack();
		}
	}
}