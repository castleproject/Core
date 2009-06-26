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
	using System;
	using System.IO;
	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Config;
	using Castle.ActiveRecord.Framework.Scopes;
	using NUnit.Framework;
	using System.Configuration;

	[TestFixture]
	public class ConfigurationSource
	{
		[Test]
		public void TestLoadDefaultThreadScopeConfig()
		{
			String xmlConfig = @"<activerecord>" + GetDefaultHibernateConfigAndCloseActiveRecordSection();

			//null == use default. == typeof(ThreadScopeInfo);
			Type expectedType = null;

			AssertConfig(xmlConfig, expectedType, null);
		}


		[Test]
		public void TestLoadWebThreadScopeInfo()
		{
			String xmlConfig = @"<activerecord isWeb=""true"">" + GetDefaultHibernateConfigAndCloseActiveRecordSection();

			Type expectedType = typeof(WebThreadScopeInfo);

			AssertConfig(xmlConfig, expectedType, null);
		}


		[Test]
		public void TestDefaultSessionFactoryHolder()
		{
			String xmlConfig = @"<activerecord isWeb=""true"">" + GetDefaultHibernateConfigAndCloseActiveRecordSection();

			Type sfh = typeof(SessionFactoryHolder);

			AssertConfig(xmlConfig, null, sfh);
		}


		[Test]
		public void TestCustomSessionFactoryholder()
		{
			String xmlConfig =
				@"<activerecord isWeb=""true"" sessionfactoryholdertype=""Castle.ActiveRecord.Tests.Config.MySessionFactoryHolder, Castle.ActiveRecord.Tests"">"
						+ GetDefaultHibernateConfigAndCloseActiveRecordSection();

			Type sfh = typeof(MySessionFactoryHolder);

			AssertConfig(xmlConfig, null, sfh);
		}


		[Test]
		public void TestDebug()
		{
			String xmlConfig =
				@"<activerecord isDebug=""true"" isWeb=""true"" sessionfactoryholdertype=""Castle.ActiveRecord.Tests.Config.MySessionFactoryHolder, Castle.ActiveRecord.Tests"">"
						+ GetDefaultHibernateConfigAndCloseActiveRecordSection();

			AssertConfig(xmlConfig, null, null, true, false, false);
		}

		[Test]
		public void TestSearchable()
		{
			String xmlConfig =
				@"<activerecord searchable=""true"">"
						+ GetDefaultHibernateConfigAndCloseActiveRecordSection();

			AssertConfig(xmlConfig, null, null, false, false, false, DefaultFlushType.Classic, true);
		}


		[Test]
		public void TestPluralizeTableNames()
		{
			String xmlConfig1 =
				@"<activerecord pluralizeTableNames=""true"">"
						+ GetDefaultHibernateConfigAndCloseActiveRecordSection();

			AssertConfig(xmlConfig1, null, null, false, true, false);

			String xmlConfig2 =
				@"<activerecord pluralizeTableNames=""false"">"
						+ GetDefaultHibernateConfigAndCloseActiveRecordSection();

			AssertConfig(xmlConfig2, null, null, false, false, false);

			String xmlConfig3 =
				@"<activerecord>"
						+ GetDefaultHibernateConfigAndCloseActiveRecordSection();

			AssertConfig(xmlConfig3, null, null, false, false, false);
		}

		[Test]
		public void TestVerifyModelsAgainstDBSchema()
		{
			String xmlConfig1 =
				@"<activerecord verifyModelsAgainstDBSchema=""true"">"
						+ GetDefaultHibernateConfigAndCloseActiveRecordSection();

			AssertConfig(xmlConfig1, null, null, false, false, true);

			String xmlConfig2 =
				@"<activerecord verifyModelsAgainstDBSchema=""false"">"
						+ GetDefaultHibernateConfigAndCloseActiveRecordSection();

			AssertConfig(xmlConfig2, null, null, false, false, false);

			String xmlConfig3 =
				@"<activerecord>"
						+ GetDefaultHibernateConfigAndCloseActiveRecordSection();

			AssertConfig(xmlConfig3, null, null, false, false, false);
		}

		[Test]
		public void TestDefaultFlushType()
		{
			string xmlConfig;
			xmlConfig = @"<activerecord>"
						+ GetDefaultHibernateConfigAndCloseActiveRecordSection();

			AssertConfig(xmlConfig, null, null, false, false, false, DefaultFlushType.Classic);

			xmlConfig = @"<activerecord flush=""classic"">"
			+ GetDefaultHibernateConfigAndCloseActiveRecordSection();

			AssertConfig(xmlConfig, null, null, false, false, false, DefaultFlushType.Classic);

			xmlConfig = @"<activerecord flush=""auto"">"
			+ GetDefaultHibernateConfigAndCloseActiveRecordSection();

			AssertConfig(xmlConfig, null, null, false, false, false, DefaultFlushType.Auto);

			xmlConfig = @"<activerecord flush=""leave"">"
			+ GetDefaultHibernateConfigAndCloseActiveRecordSection();

			AssertConfig(xmlConfig, null, null, false, false, false, DefaultFlushType.Leave);

			xmlConfig = @"<activerecord flush=""transaction"">"
			+ GetDefaultHibernateConfigAndCloseActiveRecordSection();

			AssertConfig(xmlConfig, null, null, false, false, false, DefaultFlushType.Transaction);

			try
			{
				xmlConfig = @"<activerecord flush=""foo"">" + GetDefaultHibernateConfigAndCloseActiveRecordSection();
				new XmlConfigurationSource(new StringReader(xmlConfig));
				Assert.Fail("Expected exception not thrown for invalid flush attribute on config");
			}
			catch (Exception ex)
			{
				Assert.IsInstanceOf(typeof(ConfigurationErrorsException), ex);
				Assert.IsTrue(ex.Message.ToLower().Contains("flush"));
				Assert.IsTrue(ex.Message.ToLower().Contains("foo"));
				Assert.IsTrue(ex.Message.ToLower().Contains("classic"));
				Assert.IsTrue(ex.Message.ToLower().Contains("auto"));
				Assert.IsTrue(ex.Message.ToLower().Contains("leave"));
				Assert.IsTrue(ex.Message.ToLower().Contains("transaction"));
			}
		}

		private static void AssertConfig(string xmlConfig, Type webinfotype, Type sessionFactoryHolderType)
		{
			AssertConfig(xmlConfig, webinfotype, sessionFactoryHolderType, false, false, false);
		}

		private static void AssertConfig(string xmlConfig, Type webinfotype, Type sessionFactoryHolderType, bool isDebug,
										 bool pluralize, bool verifyModelsAgainstDBSchema)
		{
			AssertConfig(xmlConfig, webinfotype, sessionFactoryHolderType, isDebug, pluralize, verifyModelsAgainstDBSchema, DefaultFlushType.Classic);
		}

		private static void AssertConfig(string xmlConfig, Type webinfotype, Type sessionFactoryHolderType, bool isDebug,
										 bool pluralize, bool verifyModelsAgainstDBSchema, DefaultFlushType defaultFlushType)
		{
			AssertConfig(xmlConfig, webinfotype, sessionFactoryHolderType, isDebug, pluralize, verifyModelsAgainstDBSchema, defaultFlushType, false);
		}


		private static void AssertConfig(string xmlConfig, Type webinfotype, Type sessionFactoryHolderType, bool isDebug,
										 bool pluralize, bool verifyModelsAgainstDBSchema, DefaultFlushType defaultFlushType, bool searchable)
		{
			StringReader sr = new StringReader(xmlConfig);

			XmlConfigurationSource c = new XmlConfigurationSource(sr);

			if (null != webinfotype)
			{
				Assert.IsTrue(c.ThreadScopeInfoImplementation == webinfotype,
							  "Expected {0}, Got {1}", webinfotype, c.ThreadScopeInfoImplementation);
			}

			if (null != sessionFactoryHolderType)
			{
				Assert.IsTrue(c.SessionFactoryHolderImplementation == sessionFactoryHolderType,
							  "Expected {0}, Got {1}", sessionFactoryHolderType, c.SessionFactoryHolderImplementation);
			}

			Assert.IsTrue(c.Debug == isDebug);
			Assert.IsTrue(c.PluralizeTableNames == pluralize);
			Assert.IsTrue(c.VerifyModelsAgainstDBSchema == verifyModelsAgainstDBSchema);
			Assert.IsTrue(c.DefaultFlushType == defaultFlushType);
			Assert.IsTrue(c.Searchable == searchable);
		}

		private static string GetDefaultHibernateConfigAndCloseActiveRecordSection()
		{
			return @"	<config>
							<add key=""connection.driver_class"" value=""NHibernate.Driver.SqlClientDriver"" />
							<add key=""dialect""                 value=""NHibernate.Dialect.MsSql2000Dialect"" />
							<add key=""connection.provider""     value=""NHibernate.Connection.DriverConnectionProvider"" />
							<add key=""connection.connection_string"" value=""Data Source=.;Initial Catalog=test;Integrated Security=True;Pooling=False"" />
						</config>
					</activerecord>";
		}
	}


	public class MySessionFactoryHolder : SessionFactoryHolder
	{
	}
}
