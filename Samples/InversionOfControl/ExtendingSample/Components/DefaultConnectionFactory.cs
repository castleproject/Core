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

namespace ExtendingSample.Components
{
	using System;
	using System.Data;

	/// <summary>
	/// Example of a real connection factory class.
	/// Note that the Type of the connection is a property
	/// of the class, so its easy to change to 
	/// SqlConnection, Odbc, Oracle, MySql etc. 
	/// </summary>
	public class DefaultConnectionFactory : IConnectionFactory
	{
		private Type connectionType = typeof(FakeConnection);
		private String _connectionString;

		public DefaultConnectionFactory( String connectionString )
		{
			_connectionString = connectionString;
		}

		public Type ConnectionType
		{
			get { return connectionType; }
			set { connectionType = value; }
		}

		public IDbConnection CreateConnection()
		{
			return new FakeConnection();
		}
	}

	class FakeConnection : IDbConnection
	{
		#region IDbConnection Members

		public void ChangeDatabase(string databaseName)
		{
		}

		public IDbTransaction BeginTransaction(System.Data.IsolationLevel il)
		{
			return null;
		}

		IDbTransaction System.Data.IDbConnection.BeginTransaction()
		{
			return null;
		}

		public System.Data.ConnectionState State
		{
			get { return new System.Data.ConnectionState(); }
		}

		public string ConnectionString
		{
			get { return null; }
			set
			{
			}
		}

		public IDbCommand CreateCommand()
		{
			return null;
		}

		public void Open()
		{
		}

		public void Close()
		{
		}

		public string Database
		{
			get { return null; }
		}

		public int ConnectionTimeout
		{
			get { return 0; }
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
		}

		#endregion
	}
}