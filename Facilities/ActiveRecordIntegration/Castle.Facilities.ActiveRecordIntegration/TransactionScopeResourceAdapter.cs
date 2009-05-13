// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Facilities.ActiveRecordIntegration
{
	using Castle.ActiveRecord;
	using Castle.Services.Transaction;
	using TransactionMode = Castle.Services.Transaction.TransactionMode;

	/// <summary>
	/// 
	/// </summary>
	public class TransactionScopeResourceAdapter : IResource
	{
		private readonly TransactionMode transactionMode;
		private TransactionScope scope;

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionScopeResourceAdapter"/> class.
		/// </summary>
		/// <param name="transactionMode">The transaction mode.</param>
		public TransactionScopeResourceAdapter(TransactionMode transactionMode)
		{
			this.transactionMode = transactionMode;
		}

		/// <summary>
		/// Implementors should start the
		/// transaction on the underlying resource
		/// </summary>
		public void Start()
		{
			Castle.ActiveRecord.TransactionMode mode = (transactionMode == TransactionMode.Requires)
			                                           	? Castle.ActiveRecord.TransactionMode.Inherits
			                                           	: Castle.ActiveRecord.TransactionMode.New;

			scope = new TransactionScope(mode);
		}

		/// <summary>
		/// Implementors should commit the
		/// transaction on the underlying resource
		/// </summary>
		public void Commit()
		{
			scope.VoteCommit();

			Dispose();
		}

		/// <summary>
		/// Implementors should rollback the
		/// transaction on the underlying resource
		/// </summary>
		public void Rollback()
		{
			scope.VoteRollBack();

			Dispose();
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		protected void Dispose()
		{
			scope.Dispose();
		}
	}
}