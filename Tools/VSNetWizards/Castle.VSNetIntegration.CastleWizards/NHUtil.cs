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

namespace Castle.VSNetIntegration.CastleWizards
{
	using System;

	public class NHUtil
	{
		private NHUtil() { }
		
		public static Pair[] GetSampleConnectionStrings()
		{
			return new Pair[] {
				new Pair("Oracle", "Data Source=;User ID=;Password=;"), 
				new Pair("MS SQLServer", "Server=(local);Initial Catalog=yourdatabase;Integrated Security=SSPI"), 
				new Pair("MySql", "Database=test;Data Source=IP;User Id=;Password="),
				new Pair("Firebird", "Server=localhost;Database=c:\file.fdb;User=;password=;ServerType=1;Pooling=false"),
				new Pair("PostgreSQL", "Server=localhost;initial catalog=dbname;User ID=;Password=;"),
				new Pair("SQLite", "Data Source=database.name;Version=3"),
			};
		}

		public static Pair[] GetGeneralSettings()
		{
			return new Pair[] {
				new Pair("hibernate.cache.use_query_cache", "true"), 
				new Pair("hibernate.connection.isolation", "ReadCommitted"),
				new Pair("hibernate.show_sql", "false"),
			};
		}

		public static Pair[] GetSettingsFor(String database)
		{
			if (database == "Oracle")
			{
				return new Pair[] {
					new Pair("hibernate.dialect", "NHibernate.Dialect.OracleDialect"), 
					new Pair("hibernate.connection.driver_class", "NHibernate.Driver.OracleClientDriver")
				};
			}
			else if (database == "MS SQLServer")
			{
				return new Pair[] {
					new Pair("hibernate.dialect", "NHibernate.Dialect.MsSql2000Dialect"), 
					new Pair("hibernate.connection.driver_class", "NHibernate.Driver.SqlClientDriver")
					//new Pair("hibernate.query.substitutions", "true 1, false 0, yes 'Y', no 'N'")
				};
			}
			else if (database == "MySql")
			{
				return new Pair[] {
					new Pair("hibernate.dialect", "NHibernate.Dialect.MySQLDialect"), 
					new Pair("hibernate.connection.driver_class", "NHibernate.Driver.MySqlDataDriver")
				};
			}
			else if (database == "Firebird")
			{
				return new Pair[] {
					new Pair("hibernate.dialect", "NHibernate.Dialect.FirebirdDialect"), 
					new Pair("hibernate.connection.driver_class", "NHibernate.Driver.FirebirdDriver"),
					new Pair("hibernate.query.substitutions", "true 1, false 0")
				};
			}
			else if (database == "PostgreSQL")
			{
				return new Pair[] {
					new Pair("hibernate.dialect", "NHibernate.Dialect.PostgreSQLDialect"), 
					new Pair("hibernate.connection.driver_class", "NHibernate.Driver.NpgsqlDriver"),
				};
			}
			else if (database == "SQLite")
			{
				return new Pair[] {
					new Pair("hibernate.dialect", "NHibernate.Dialect.SQLiteDialect"), 
					new Pair("hibernate.connection.driver_class", "NHibernate.Driver.SQLiteDriver"),
					new Pair("hibernate.query.substitutions", "true=1;false=0")
				};
			}

			return null;
		}
	}
}
