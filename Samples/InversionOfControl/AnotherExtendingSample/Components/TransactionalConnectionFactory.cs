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

	/// <summary>
	/// Summary description for TransactionalConnectionFactory.
	/// </summary>
	public class TransactionalConnectionFactory : IConnectionFactory
	{
		private ITransactionManager _manager;
		private String _connectionString;
		private Type _dbConnectionType;

		public TransactionalConnectionFactory(
			ITransactionManager manager, String connectionString,
			Type dbConnectionType)
		{
			_manager = manager;
			_connectionString = connectionString;
			_dbConnectionType = dbConnectionType;
		}

		public IDbConnection CreateConnection()
		{
			IDbConnection connection = (IDbConnection) 
				Activator.CreateInstance(_dbConnectionType);

			connection.ConnectionString = _connectionString;
			connection.Open();

//			if (_manager.CurrentTransaction != null)
//			{
//				_manager.CurrentTransaction.Enlist( connection );
//				return new ConnectionProxy( connection, _manager );
//			}
//			else
			{
				return connection;
			}
		}
	}

	public class ConnectionProxy : IDbConnection
	{
		private IDbConnection _connection;
		private ITransactionManager _manager;

		public ConnectionProxy(IDbConnection connection, ITransactionManager manager)
		{
			_connection = connection;
			_manager = manager;
		}

		public IDbTransaction BeginTransaction()
		{
			return _connection.BeginTransaction();
		}

		public IDbTransaction BeginTransaction(IsolationLevel il)
		{
			return _connection.BeginTransaction(il);
		}

		public void Close()
		{
		}

		public void ChangeDatabase(string databaseName)
		{
			_connection.ChangeDatabase(databaseName);
		}

		public IDbCommand CreateCommand()
		{
			IDbCommand command = _connection.CreateCommand();

			_manager.CurrentTransaction.Enlist(command);

			return command;
		}

		public void Open()
		{
			_connection.Open();
		}

		public string ConnectionString
		{
			get { return _connection.ConnectionString; }
			set { _connection.ConnectionString = value; }
		}

		public int ConnectionTimeout
		{
			get { return _connection.ConnectionTimeout; }
		}

		public string Database
		{
			get { return _connection.Database; }
		}

		public ConnectionState State
		{
			get { return _connection.State; }
		}

		public void Dispose()
		{
		}
	}
}
