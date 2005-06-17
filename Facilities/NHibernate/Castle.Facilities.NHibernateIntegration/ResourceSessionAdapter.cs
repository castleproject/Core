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

using ITransaction = NHibernate.ITransaction;

namespace Castle.Facilities.NHibernateIntegration
{
	using NHibernate;

	/// <summary>
	/// Adapts the IResource interface the the 
	/// underlying NHibernate's transaction.
	/// </summary>
	public class ResourceSessionAdapter : Castle.Services.Transaction.IResource
	{
		private ITransaction _transaction;

		public ResourceSessionAdapter(ITransaction transaction)
		{
			_transaction = transaction;
		}

		public void Start()
		{
			// Nothing do to
		}

		public void Commit()
		{
			_transaction.Commit();
		}

		public void Rollback()
		{
			_transaction.Rollback();
		}
	}
}
