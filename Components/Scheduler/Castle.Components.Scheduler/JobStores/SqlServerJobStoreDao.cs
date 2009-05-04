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

namespace Castle.Components.Scheduler.JobStores
{
	using System;
	using System.Data;
	using System.Data.SqlClient;

	/// <summary>
	/// A job store DAO for SQL Server databases.
	/// </summary>
	public class SqlServerJobStoreDao : AdoNetJobStoreDao
	{
		/// <summary>
		/// Creates a SQL Server job store.
		/// </summary>
		/// <param name="connectionString">The database connection string</param>
		/// <exception cref="ArgumentNullException">Thrown if <paramref name="connectionString"/> is null</exception>
		public SqlServerJobStoreDao(string connectionString)
			: base(connectionString, "@")
		{
		}

		/// <inheritdoc />
		protected override IDbConnection CreateConnection()
		{
			return new SqlConnection(ConnectionString);
		}
	}
}