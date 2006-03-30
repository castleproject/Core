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

namespace Castle.ActiveRecord.Tests.Config
{
	using System;
	using System.IO;

	using NUnit.Framework;
	
	using Castle.ActiveRecord.Framework.Config;
	using Castle.ActiveRecord.Framework.Scopes;
    using Castle.ActiveRecord.Framework;

	[TestFixture]
	public class ConfigurationSource
	{
		[Test]
		public void TestLoadDefaultThreadScopeConfig()
		{
			String xmlConfig = @"<activerecord>
                                  <config>
                                    <add key=""hibernate.connection.driver_class"" value=""NHibernate.Driver.SqlClientDriver"" />
                                    <add key=""hibernate.dialect""                 value=""NHibernate.Dialect.MsSql2000Dialect"" />
                                    <add key=""hibernate.connection.provider""     value=""NHibernate.Connection.DriverConnectionProvider"" />
                                    <add key=""hibernate.connection.connection_string"" value=""Data Source=.;Initial Catalog=test;Integrated Security=True;Pooling=False"" />
                                  </config>
                                </activerecord>";


			//null == use default. == typeof(ThreadScopeInfo);
			Type expectedType = null;

			AssertConfig(xmlConfig, expectedType, null);
		}

		[Test]
		public void TestLoadWebThreadScopeInfo()
		{
			String xmlConfig = @"<activerecord isWeb=""true"">
                                  <config>
                                    <add key=""hibernate.connection.driver_class"" value=""NHibernate.Driver.SqlClientDriver"" />
                                    <add key=""hibernate.dialect""                 value=""NHibernate.Dialect.MsSql2000Dialect"" />
                                    <add key=""hibernate.connection.provider""     value=""NHibernate.Connection.DriverConnectionProvider"" />
                                    <add key=""hibernate.connection.connection_string"" value=""Data Source=.;Initial Catalog=test;Integrated Security=True;Pooling=False"" />
                                  </config>
                                </activerecord>";

			Type expectedType = typeof (WebThreadScopeInfo);

			AssertConfig(xmlConfig, expectedType, null);
		}


        [Test]
        public void TestDefaultSessionFactoryHolder() {
            String xmlConfig = @"<activerecord isWeb=""true"">
                                  <config>
                                    <add key=""hibernate.connection.driver_class"" value=""NHibernate.Driver.SqlClientDriver"" />
                                    <add key=""hibernate.dialect""                 value=""NHibernate.Dialect.MsSql2000Dialect"" />
                                    <add key=""hibernate.connection.provider""     value=""NHibernate.Connection.DriverConnectionProvider"" />
                                    <add key=""hibernate.connection.connection_string"" value=""Data Source=.;Initial Catalog=test;Integrated Security=True;Pooling=False"" />
                                  </config>
                                </activerecord>";

            Type sfh = typeof(SessionFactoryHolder);

            AssertConfig(xmlConfig, null, sfh);
        }


        [Test]
        public void TestCustomSessionFactoryholder() {
            String xmlConfig = @"<activerecord isWeb=""true"" sessionfactoryholdertype=""Castle.ActiveRecord.Tests.Config.MySessionFactoryHolder, Castle.ActiveRecord.Tests"">
                                  <config>
                                    <add key=""hibernate.connection.driver_class"" value=""NHibernate.Driver.SqlClientDriver"" />
                                    <add key=""hibernate.dialect""                 value=""NHibernate.Dialect.MsSql2000Dialect"" />
                                    <add key=""hibernate.connection.provider""     value=""NHibernate.Connection.DriverConnectionProvider"" />
                                    <add key=""hibernate.connection.connection_string"" value=""Data Source=.;Initial Catalog=test;Integrated Security=True;Pooling=False"" />
                                  </config>
                                </activerecord>";

            Type sfh = typeof(MySessionFactoryHolder);

            AssertConfig(xmlConfig, null, sfh);
        }


        [Test]
        public void TestDebug() {
            String xmlConfig = @"<activerecord isDebug=""true"" isWeb=""true"" sessionfactoryholdertype=""Castle.ActiveRecord.Tests.Config.MySessionFactoryHolder, Castle.ActiveRecord.Tests"">
                                  <config>
                                    <add key=""hibernate.connection.driver_class"" value=""NHibernate.Driver.SqlClientDriver"" />
                                    <add key=""hibernate.dialect""                 value=""NHibernate.Dialect.MsSql2000Dialect"" />
                                    <add key=""hibernate.connection.provider""     value=""NHibernate.Connection.DriverConnectionProvider"" />
                                    <add key=""hibernate.connection.connection_string"" value=""Data Source=.;Initial Catalog=test;Integrated Security=True;Pooling=False"" />
                                  </config>
                                </activerecord>";

            AssertConfig(xmlConfig,null,null,true);
        }

		private static void AssertConfig(string xmlConfig, Type webinfotype, Type sessionFactoryHolderType)
		{
            AssertConfig(xmlConfig, webinfotype, sessionFactoryHolderType, false);
        }

        private static void AssertConfig(string xmlConfig, Type webinfotype, Type sessionFactoryHolderType, bool isDebug) {
			StringReader sr = new StringReader(xmlConfig);

			XmlConfigurationSource c = new XmlConfigurationSource(sr);

            if (null != webinfotype) {
                Assert.IsTrue(c.ThreadScopeInfoImplementation == webinfotype,
                    "Expected {0}, Got {1}", webinfotype, c.ThreadScopeInfoImplementation);
            }

            if (null != sessionFactoryHolderType) {
                Assert.IsTrue(c.SessionFactoryHolderImplementation == sessionFactoryHolderType,
                    "Expected {0}, Got {1}", sessionFactoryHolderType, c.SessionFactoryHolderImplementation);
            }

            Assert.IsTrue(c.Debug == isDebug);
		}

	}

    public class MySessionFactoryHolder : SessionFactoryHolder { }
}
