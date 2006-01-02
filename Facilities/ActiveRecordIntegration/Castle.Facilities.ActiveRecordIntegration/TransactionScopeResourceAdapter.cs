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

namespace Castle.Facilities.ActiveRecordIntegration
{
	using System;
	
	using Castle.ActiveRecord;

	using Castle.Services.Transaction;

	using TransactionMode = Castle.Services.Transaction.TransactionMode;


	public class TransactionScopeResourceAdapter : IResource
	{
		private readonly TransactionMode transactionMode;
		
		private TransactionScope scope;


		public TransactionScopeResourceAdapter(TransactionMode transactionMode)
		{
			this.transactionMode = transactionMode;
		}

		public void Start()
		{
			Castle.ActiveRecord.TransactionMode mode = (transactionMode == TransactionMode.Requires)
				? Castle.ActiveRecord.TransactionMode.Inherits : Castle.ActiveRecord.TransactionMode.New;

			scope = new TransactionScope( mode );
		}

		public void Commit()
		{
			scope.VoteCommit();

			Dispose();
		}

		public void Rollback()
		{
			scope.VoteRollBack();

			Dispose();
		}

		protected void Dispose()
		{
			scope.Dispose();
		}
	}
}
