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

	/// <summary>
	/// 
	/// </summary>
	public enum TransactionStatus
	{
		NoTransaction,
		Active,
		Committed,
		RolledBack,
		Invalid
	}

	/// <summary>
	/// Represents the contract for a transaction.
	/// </summary>
	public interface ITransaction
	{
		/// <summary>
		/// Starts the transaction. Implementors
		/// should activate the apropriate resources
		/// in order to start the underlying transaction
		/// </summary>
		void Begin();

		/// <summary>
		/// Succeed the transaction, persisting the
		/// modifications
		/// </summary>
		void Commit();

		/// <summary>
		/// Cancels the transaction, rolling back the 
		/// modifications
		/// </summary>
		void Rollback();

		/// <summary>
		/// Returns the current transaction status.
		/// </summary>
		TransactionStatus Status { get; }
		
		/// <summary>
		/// Register a participant on the transaction.
		/// </summary>
		/// <param name="resource"></param>
		void Enlist(IResource resource);

		/// <summary>
		/// Registers a synchronization object that will be 
		/// invoked prior and after the transaction completion
		/// (commit or rollback)
		/// </summary>
		/// <param name="synchronization"></param>
		void RegisterSynchronization(ISynchronization synchronization);

		/// <summary>
		/// Transaction context. Can be used by applications.
		/// </summary>
		IDictionary Context { get; }
	}
}
