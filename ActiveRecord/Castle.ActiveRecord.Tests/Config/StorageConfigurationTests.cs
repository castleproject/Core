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

namespace Castle.ActiveRecord.Tests.Config
{
	using Castle.ActiveRecord.Framework.Config;
	using NHibernate.ByteCode.Castle;
	using NHibernate.Connection;
	using NHibernate.Dialect;
	using NHibernate.Driver;
	using NUnit.Framework;

	[TestFixture]
	public class StorageConfigurationDoesCreateCorrectConfigurationValues
	{
		private FluentStorageConfiguration _configuration;

		[SetUp]
		public void Setup()
		{
			_configuration = Configure.Storage;
		}

		public void AssertConfiguration(IStorageConfiguration storageConfiguration, string name, string value)
		{
			Assert.That(storageConfiguration.ConfigurationValues.ContainsKey(name));
			Assert.That(storageConfiguration.ConfigurationValues[name], Is.EqualTo(value));
		}

		[Test]
		public void ConnectionStringNameIsAdded()
		{
			AssertConfiguration(_configuration.ConnectionStringName("foo"), "connection.connection_string_name", "foo");
		}

		[Test]
		public void ConnectionStringIsAdded()
		{
			AssertConfiguration(_configuration.ConnectionString("foo"), "connection.connection_string", "foo");
		}

		[Test]
		public void DriverClassIsAdded()
		{
			AssertConfiguration(_configuration.Driver<SqlClientDriver>(), "connection.driver_class",
			                    "NHibernate.Driver.SqlClientDriver");
		}

		[Test]
		public void ConnectionProviderClassIsAdded()
		{
			AssertConfiguration(_configuration.ConnectionProvider<DriverConnectionProvider>(), "connection.provider",
			                    "NHibernate.Connection.DriverConnectionProvider");
		}

		[Test]
		public void DialectIsAdded()
		{
			AssertConfiguration(_configuration.Dialect<MsSql2005Dialect>(), "dialect", "NHibernate.Dialect.MsSql2005Dialect");
		}

		[Test]
		public void ProxyFactoryIsAdded()
		{
			AssertConfiguration(_configuration.ProxiedBy<ProxyFactoryFactory>(), "proxyfactory.factory_class",
			                    "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");
		}
	}
}