// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

namespace Castle.Facilities.NHibernateIntegration.Tests
{
	using System;
	using System.Threading;

	using NUnit.Framework;

	using ByteFX.Data.MySqlClient;

	using Castle.Model.Configuration;

	using Castle.Windsor;
	
	using Castle.MicroKernel.SubSystems.Configuration;

	/// <summary>
	/// Summary description for AbstractNHibernateTestCase.
	/// </summary>
	public abstract class AbstractNHibernateTestCase
	{
		// protected const string Driver = "NHibernate.Driver.MySqlDataDriver";
		protected const string Driver = "Castle.Facilities.NHibernateIntegration.Tests.ByteFxDriver, Castle.Facilities.NHibernateIntegration.Tests";
		protected const string Dialect = "NHibernate.Dialect.MySQLDialect";
		protected const string ConnectionProvider = "NHibernate.Connection.DriverConnectionProvider";
		protected const string ConnectionString = "Database=Test;Data Source=localhost;User Id=theuser;Password=opauser";

		[SetUp]
		public virtual void InitDb()
		{
			// Reset tables

			MySqlConnection conn = new MySqlConnection(ConnectionString);
			conn.Open();

			try
			{
				Thread.CurrentThread.Join(1000);
				MySqlCommand command = conn.CreateCommand();
				command.CommandText = "DELETE FROM BLOGS";
				command.ExecuteNonQuery();
				command.CommandText = "DELETE FROM BLOG_ITEMS";
				command.ExecuteNonQuery();
			}
			finally
			{
				conn.Close();
			}
		}

		protected virtual IWindsorContainer CreateConfiguredContainer()
		{
			IWindsorContainer container = new WindsorContainer(new DefaultConfigurationStore());

			MutableConfiguration confignode = new MutableConfiguration("facility");

			IConfiguration factory =
				confignode.Children.Add(new MutableConfiguration("factory"));
			factory.Attributes["id"] = "sessionFactory1";

			IConfiguration settings =
				factory.Children.Add(new MutableConfiguration("settings"));

			settings.Children.Add(
				new MutableConfiguration("item",
				ConnectionProvider)).Attributes["key"] = "hibernate.connection.provider";
			settings.Children.Add(
				new MutableConfiguration("item",
				Driver)).Attributes["key"] = "hibernate.connection.driver_class";
			settings.Children.Add(
				new MutableConfiguration("item",
				ConnectionString)).Attributes["key"] = "hibernate.connection.connection_string";
			settings.Children.Add(
				new MutableConfiguration("item",
				Dialect)).Attributes["key"] = "hibernate.dialect";

			IConfiguration resources =
				factory.Children.Add(new MutableConfiguration("resources"));

			IConfiguration resource;
			resource = resources.Children.Add(new MutableConfiguration("resource"));
			resource.Attributes["name"] = "Blog.hbm.xml";
			resource = resources.Children.Add(new MutableConfiguration("resource"));
			resource.Attributes["name"] = "BlogItem.hbm.xml";

			container.Kernel.ConfigurationStore.AddFacilityConfiguration("nhibernate", confignode);

			return container;
		}
	}
}
