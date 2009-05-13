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

#if !MONO

namespace Castle.Services.Transaction
{
	using System.Transactions;

	public class TransactionScopeResourceAdapter : IResource
	{
		private readonly TransactionMode mode;
		private readonly IsolationMode isolationMode;
		private TransactionScope scope;

		/// <summary>
		/// Initializes a new instance of the <see cref="TransactionScopeResourceAdapter"/> class.
		/// </summary>
		/// <param name="mode">The mode.</param>
		/// <param name="isolationMode">The isolation mode.</param>
		public TransactionScopeResourceAdapter(TransactionMode mode, IsolationMode isolationMode)
		{
			this.mode = mode;
			this.isolationMode = isolationMode;
		}

		/// <summary>
		/// Implementors should start the
		/// transaction on the underlying resource
		/// </summary>
		public void Start()
		{
			TransactionScopeOption scopeOption = mode == TransactionMode.Requires ? TransactionScopeOption.Required : TransactionScopeOption.RequiresNew;
			
			TransactionOptions options = new TransactionOptions();
			
			switch(isolationMode)
			{
				case IsolationMode.ReadCommitted:
					options.IsolationLevel = IsolationLevel.ReadCommitted;
					break;
				case IsolationMode.Chaos:
					options.IsolationLevel = IsolationLevel.Chaos;
					break;
				case IsolationMode.ReadUncommitted:
					options.IsolationLevel = IsolationLevel.ReadUncommitted;
					break;
				case IsolationMode.Serializable:
					options.IsolationLevel = IsolationLevel.Serializable;
					break;
				case IsolationMode.Unspecified:
					options.IsolationLevel = IsolationLevel.Unspecified;
					break;
			}

			scope = new TransactionScope(scopeOption, options);
		}

		/// <summary>
		/// Implementors should commit the
		/// transaction on the underlying resource
		/// </summary>
		public void Commit()
		{
			scope.Complete();
			scope.Dispose();
		}

		/// <summary>
		/// Implementors should rollback the
		/// transaction on the underlying resource
		/// </summary>
		public void Rollback()
		{
			scope.Dispose();
		}
	}
}

#endif