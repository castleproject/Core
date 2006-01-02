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

namespace Extending2.Components
{
	using System;
	using System.Data;
	using System.Collections;

	using Castle.Model;

	[PerThread]
	public class DefaultTransactionManager : ITransactionManager
	{
		private SimpleTransaction _current;

		public ITransaction CreateTransaction()
		{
			_current = new SimpleTransaction();
			return _current;
		}

		public ITransaction CurrentTransaction
		{
			get { return _current; }
		}

		public void Release(ITransaction transaction)
		{
			// We could ensure that the transaction
			// was properly closed here

			(transaction as SimpleTransaction).Dispose();

			if (transaction == _current)
			{
				_current = null;
			}
		}
	}

	public class SimpleTransaction : ITransaction, IDisposable
	{
		IList _enlistedResources = new ArrayList();

		public void Enlist(IDbConnection connection)
		{
			_enlistedResources.Add( connection.BeginTransaction() );
		}

		public void Enlist(IDbCommand command)
		{
			foreach(IDbTransaction transaction in _enlistedResources)
			{
				if (transaction.Connection == command.Connection)
				{
					command.Transaction = transaction;
					break;
				}
			}
		}

		public void Commit()
		{
			foreach(IDbTransaction transaction in _enlistedResources)
			{
				if (transaction.Connection.State != ConnectionState.Closed)
				{
					transaction.Commit();
				}
			}
		}

		public void Rollback()
		{
			foreach(IDbTransaction transaction in _enlistedResources)
			{
				if (transaction.Connection.State != ConnectionState.Closed)
				{
					transaction.Rollback();
				}
			}
		}

		public void Dispose()
		{
			foreach(IDbTransaction transaction in _enlistedResources)
			{
				if (transaction.Connection.State != ConnectionState.Closed)
				{
					transaction.Connection.Close();
				}
			}

			_enlistedResources.Clear();
		}
	}
}
