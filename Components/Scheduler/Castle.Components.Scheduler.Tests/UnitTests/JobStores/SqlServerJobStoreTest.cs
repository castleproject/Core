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

namespace Castle.Components.Scheduler.Tests.UnitTests.JobStores
{
	using System;
	using System.Data;
	using System.Data.SqlClient;
	using System.IO;
	using NUnit.Framework;
	using Scheduler.JobStores;

	[TestFixture, Ignore("A new database needs to be setup in the build server. And tests need to be updated to be configurable from nant.")]
	public class SqlServerJobStoreTest : PersistentJobStoreTest
	{
		private string connectionString;

		public override void SetUp()
		{
			PurgeAllData();

			base.SetUp();
		}

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			// Use the existing attached Db if there is one.
			connectionString = "server=.; database=SchedulerTestDb;Integrated Security=True";

			try
			{
				PurgeAllData();
			}
			catch (Exception ex1)
			{
				string testProjectBinPath = Path.GetDirectoryName(typeof (SqlServerJobStoreTest).Assembly.Location);
				string dbPath =
					Path.GetFullPath(Path.Combine(testProjectBinPath,
					                              @"..\..\Castle.Components.Scheduler.Db\SqlServer\Data\SchedulerTestDb.mdf"));

				Console.WriteLine("Could not connect to the SchedulerTestDb.\n"
				                  + "Attempting to attach it from {0}.", dbPath);

				connectionString = "Data Source=.;AttachDbFilename=\"" + dbPath +
				                   "\";Initial Catalog=SchedulerTestDb;Integrated Security=True";
				try
				{
					PurgeAllData();
				}
				catch (Exception ex2)
				{
					Assert.Fail("Could not connect to the SchedulerTestDb and could not attach it from {0}.\n\n"
					            + "Initial connection attempt:\n{1}\n\n"
					            + "Attached connection attempt:\n{2}", dbPath, ex1, ex2);
				}
			}
		}

		[Test]
		public void StandardConstructorCreatesDaoWithExpectedConnectionString()
		{
			SqlServerJobStore jobStore = new SqlServerJobStore(connectionString);
			Assert.AreEqual(connectionString, jobStore.ConnectionString);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void StandardConstructorThrowsIfConnectionStringIsNull()
		{
			new SqlServerJobStore((string) null);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void DaoConstructorThrowsIfDaoIsNull()
		{
			new SqlServerJobStore((SqlServerJobStoreDao) null);
		}

		[Test]
		public void ConnectionStringIsSameAsWasOriginallySpecified()
		{
			Assert.AreEqual(connectionString, ((SqlServerJobStore) JobStore).ConnectionString);
		}

		protected override PersistentJobStore CreatePersistentJobStore()
		{
			return new SqlServerJobStore(new InstrumentedSqlServerJobStoreDao(connectionString));
		}

		protected override void SetBrokenConnectionMocking(PersistentJobStore jobStore, bool brokenConnections)
		{
			InstrumentedSqlServerJobStoreDao dao = (InstrumentedSqlServerJobStoreDao) jobStore.JobStoreDao;
			dao.BrokenConnections = brokenConnections;
		}

		protected void PurgeAllData()
		{
			using (SqlConnection connection = new SqlConnection(connectionString))
			{
				SqlCommand command = new SqlCommand("spSCHED_TEST_PurgeAllData", connection);
				command.CommandType = CommandType.StoredProcedure;

				connection.Open();
				command.ExecuteNonQuery();
			}
		}

		private class InstrumentedSqlServerJobStoreDao : SqlServerJobStoreDao
		{
			private bool brokenConnections;

			public InstrumentedSqlServerJobStoreDao(string connectionString)
				: base(connectionString)
			{
			}

			public bool BrokenConnections
			{
				get { return brokenConnections; }
				set { brokenConnections = value; }
			}

			protected override IDbConnection CreateConnection()
			{
				if (brokenConnections)
					throw new Exception("Simulated Db connection failure.");

				return base.CreateConnection();
			}
		}
	}
}