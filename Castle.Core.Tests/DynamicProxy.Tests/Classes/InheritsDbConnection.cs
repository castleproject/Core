// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.DynamicProxy.Tests.Classes
{
	using System;
	using System.Data;
	using System.Data.Common;

	public class InheritsDbConnection : DbConnection
	{
		private string dataSource = "";
		private string database = "";

		private string serverVersion = "1";

		private ConnectionState state = ConnectionState.Closed;

		public InheritsDbConnection()
		{
			ConnectionString = "";
		}

		public override string ConnectionString { get; set; }

		public override string DataSource
		{
			get { return dataSource; }
		}

		public override string Database
		{
			get { return database; }
		}

		public override string ServerVersion
		{
			get { return serverVersion; }
		}

		public override ConnectionState State
		{
			get { return state; }
		}

		public override void ChangeDatabase(string databaseName)
		{
		}

		public override void Close()
		{
		}

		public override void Open()
		{
		}

		protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
		{
			return null;
		}

		protected override DbCommand CreateDbCommand()
		{
			return null;
		}
	}
}