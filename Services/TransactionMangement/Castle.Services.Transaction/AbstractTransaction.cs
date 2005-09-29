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

// This supporting service was inspired by
// http://www.codeproject.com/cs/database/dal.asp
// by Deyan Petrov

namespace Castle.Services.Transaction
{
	using System;
	using System.Collections;
	using System.Collections.Specialized;

	/// <summary>
	/// Helper abstract class for <see cref="ITransaction"/> implementors. 
	/// </summary>
	public abstract class AbstractTransaction : ITransaction, IDisposable
	{
		private HybridDictionary _context;
		private IList _synchronizations;
		private TransactionStatus _state = TransactionStatus.NoTransaction;
		protected IList _resources;

		public AbstractTransaction()
		{
			_resources = new ArrayList();
			_synchronizations = new ArrayList();
			_context = new HybridDictionary(true);
		}

		#region ITransaction

		public virtual void Enlist(IResource resource)
		{
			if (resource == null) throw new ArgumentNullException("resource");

			// We can't add the resource more than once
			if (_resources.Contains(resource)) return;
			
			if (Status == TransactionStatus.Active)
			{
				try
				{
					resource.Start();
				}
				catch(Exception ex)
				{
					_state = TransactionStatus.Invalid;
					throw ex;
				}
			}

			_resources.Add(resource);
		}

		public virtual void Begin()
		{
			AssertState(TransactionStatus.NoTransaction);
			_state = TransactionStatus.Active;

			foreach(IResource resource in _resources)
			{
				try
				{
					resource.Start();
				}
				catch(Exception ex)
				{
					_state = TransactionStatus.Invalid;
					throw ex;
				}
			}
		}

		public virtual void Rollback()
		{
			AssertState(TransactionStatus.Active);
			_state = TransactionStatus.RolledBack;

			PerformSynchronizations(false);

			Exception error = null;

			foreach(IResource resource in _resources)
			{
				try
				{
					resource.Rollback();
				}
				catch(Exception ex)
				{
					_state = TransactionStatus.Invalid;
					error = ex;
				}
			}

			PerformSynchronizations(true);

			if (error != null)
			{
				throw new TransactionException("Could not rollback transaction, one of the resources failed", error);
			}
		}

		public virtual void Commit()
		{
			AssertState(TransactionStatus.Active);
			_state = TransactionStatus.Committed;

			PerformSynchronizations(false);

			Exception error = null;

			foreach(IResource resource in _resources)
			{
				try
				{
					resource.Commit();
				}
				catch(Exception ex)
				{
					_state = TransactionStatus.Invalid;
					error = ex;
				}
			}

			PerformSynchronizations(true);

			if (error != null)
			{
				throw new TransactionException("Could not commit transaction, one of the resources failed", error);
			}
		}

		public TransactionStatus Status
		{
			get { return _state; }
		}

		public virtual void RegisterSynchronization(ISynchronization synchronization)
		{
			if (synchronization == null) throw new ArgumentNullException("synchronization");

			_synchronizations.Add(synchronization);
		}

		public virtual IDictionary Context
		{
			get { return _context; }
		}

		public abstract bool IsChildTransaction { get; }

		#endregion

		#region IDisposable Members

		public virtual void Dispose()
		{
			_resources.Clear();
			_synchronizations.Clear();
		}

		#endregion

		#region Helper methods

		protected virtual void AssertState(TransactionStatus state)
		{
			if (_state != state)
			{
				throw new TransactionException("Invalid transaction state to perform the requested action");
			}
		}

		private void PerformSynchronizations(bool runAfterCompletion)
		{
			foreach(ISynchronization sync in _synchronizations)
			{
				try
				{
					if (runAfterCompletion)
					{
						sync.AfterCompletion();
					}
					else
					{
						sync.BeforeCompletion();
					}
				}
				catch(Exception)
				{
					// Exceptions should not be threw by syncs.
					// It will be swalled
				}
			}
		}

		#endregion
	}
}
