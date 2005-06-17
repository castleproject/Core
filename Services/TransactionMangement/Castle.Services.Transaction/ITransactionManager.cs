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
	/// <summary>
	/// Manages the creation and disposal of <see cref="ITransaction"/> instances.
	/// </summary>
	public interface ITransactionManager
	{
		/// <summary>
		/// More information here.
		/// Can return null!
		/// </summary>
		/// <param name="transactionMode"></param>
		/// <param name="isolationMode"></param>
		/// <returns></returns>
		ITransaction CreateTransaction( TransactionMode transactionMode, IsolationMode isolationMode );

		/// <summary>
		/// Returns the current <see cref="ITransaction"/>. 
		/// The transaction manager will probably need to 
		/// hold the created transaction in the thread or in 
		/// some sort of context.
		/// </summary>
		ITransaction CurrentTransaction
		{
			get;
		}

		/// <summary>
		/// Should guarantee the correct disposal of transaction
		/// resources.
		/// </summary>
		/// <param name="transaction"></param>
		void Dispose(ITransaction transaction);
	}
}
