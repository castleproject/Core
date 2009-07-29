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
	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Scopes;
	using NUnit.Framework;
	using Castle.ActiveRecord.Framework.Config;
	using NHibernate.ByteCode.Castle;
	using NHibernate.Connection;
	using NHibernate.Dialect;
	using NHibernate.Driver;

	[TestFixture]
	public class ConfigureTests
	{
		[Test]
		public void BasicConfigurationApi()
		{
			IActiveRecordConfiguration configuration = Configure.ActiveRecord
				.ForWeb()
				.Flush(DefaultFlushType.Leave)
				.UseThreadScopeInfo<SampleThreadScopeInfo>()
				.UseSessionFactoryHolder<SampleSessionFactoryHolder>()
				.MakeLazyByDefault()
				.VerifyModels()
				.RegisterSearch();

			Assert.That(configuration.ThreadScopeInfoImplementation, Is.EqualTo(typeof (SampleThreadScopeInfo)));
			Assert.That(configuration.SessionfactoryHolderImplementation, Is.EqualTo(typeof (SampleSessionFactoryHolder)));
			Assert.That(configuration.DefaultFlushType, Is.EqualTo(DefaultFlushType.Leave));
			Assert.That(configuration.WebEnabled, Is.True);
			Assert.That(configuration.Lazy, Is.True);
			Assert.That(configuration.Verification, Is.True);
			Assert.That(configuration.Searchable, Is.True);
		}

		[Test]
		public void BasicSyntaxStorageConfiguration()
		{
			IStorageConfiguration configuration = Configure.Storage
				.For
				.AllOtherTypes()
				.MappedBy(new XmlNhibernateMapping().InAssemblyOf<OneOfMyEntities>())
				.As
				.ConnectionStringName("a_string")
				.Driver<SqlClientDriver>()
				.ConnectionProvider<DriverConnectionProvider>()
				.Dialect<MsSql2005Dialect>()
				.ProxiedBy<ProxyFactoryFactory>()
				.ShowSql();

			IStorageConfiguration auditConfiguration = Configure.Storage
				.For
				.SubtypesOf<AuditType>()
				.InNamespaceOf<DefaultAuditorType>() // Logical and - we are narrowing our 
				.MappedBy(new FluentNHibernateMapping().InAssemblyOf<MyMappingClass>())
				.And // Logical or - we start the next StorageTypeSelection
				.TypesInAssemblyOf<MessagingImpl>()
				// No MappedBy defaults to ActiveRecord attributes
				.As
				.DefaultsFor<MsSqlServer2000Configuration>()
				.ConnectionString("server=bla;...");
		}
	}

	public class SampleThreadScopeInfo : HybridWebThreadScopeInfo
	{
	}

	public class SampleSessionFactoryHolder : SessionFactoryHolder
	{
	}

	public abstract class AuditType
	{
	}

	public class DefaultAuditorType : AuditType
	{
	}

	public class MessagingImpl
	{
	}

	public class OneOfMyEntities
	{
	}

	public class MyMappingClass
	{
	}
}