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

using MySql.Data.MySqlClient;
using NHibernate;

namespace Castle.Facilities.NHibernate.Tests
{
	using System;
	using System.Collections;

	using NUnit.Framework;

	using Castle.Windsor;

	using Castle.Model.Configuration;
	
	using Castle.MicroKernel.SubSystems.Configuration;

	/// <summary>
	/// Summary description for NHibernateFacilityTestCase.
	/// </summary>
	[TestFixture]
	public class NHibernateFacilityTestCase
	{
		const string Dialect = "NHibernate.Dialect.MySQLDialect";
		const string ConnectionProvider = "NHibernate.Connection.DriverConnectionProvider";
		const string Driver = "NHibernate.Driver.MySqlDataDriver";
		const string ConnectionString = "Database=Test;Data Source=localhost;User Id=theuser;Password=opauser";

		[SetUp]
		public void InitDb()
		{
			// Reset tables
			
			MySql.Data.MySqlClient.MySqlConnection conn = new MySqlConnection(ConnectionString);
			conn.Open();
			
			try
			{
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

		[Test]
		public void Usage()
		{
			IWindsorContainer container = CreateConfiguredContainer();
			container.AddFacility( "nhibernate", new NHibernateFacility() );

			ISessionFactory factory = (ISessionFactory) container["sessionFactory1"];
			ISession session = factory.OpenSession();


			const string BlogName = "hammett's blog";

			Blog blog = new Blog();
			blog.Name = BlogName;
			blog.Items = new ArrayList();

			ITransaction tx = null;

			try
			{
				tx = session.BeginTransaction();
				session.Save(blog);
				tx.Commit();
			}
			catch (HibernateException e)
			{
				if (tx!=null) tx.Rollback();
				throw e;
			}
			finally
			{
				session.Close();
			}

			blog = (Blog) session.Find( "from Blog as b where b.Name=:name", BlogName, NHibernate. )[0];
			Assert.IsNotNull(blog);
			Assert.AreEqual( 0, blog.Items.Count );

		}

		private IWindsorContainer CreateConfiguredContainer()
		{
			IWindsorContainer container = new WindsorContainer( new DefaultConfigurationStore() );
	
			MutableConfiguration confignode = new MutableConfiguration("facility");
	
			IConfiguration factory = 
				confignode.Children.Add( new MutableConfiguration("factory") );
			factory.Attributes["id"] = "sessionFactory1";

			IConfiguration settings = 
				factory.Children.Add( new MutableConfiguration("settings") );

			settings.Children.Add( 
				new MutableConfiguration("item", 
				                         ConnectionProvider)).Attributes["key"] = 
					"hibernate.connection.provider";
			settings.Children.Add( 
				new MutableConfiguration("item", 
				                         Driver)).Attributes["key"] = 
					"hibernate.connection.driver_class";
			settings.Children.Add( 
				new MutableConfiguration("item", 
				                         ConnectionString)).Attributes["key"] = 
					"hibernate.connection.connection_string";
			settings.Children.Add( 
				new MutableConfiguration("item", 
				                         Dialect)).Attributes["key"] = 
				"hibernate.dialect";
	
			IConfiguration resources = 
				factory.Children.Add( new MutableConfiguration("resources") );

			IConfiguration resource;
			resource = resources.Children.Add( new MutableConfiguration("resource") );
			resource.Attributes["name"] = "Blog.hbm.xml";
			resource = resources.Children.Add( new MutableConfiguration("resource") );
			resource.Attributes["name"] = "BlogItem.hbm.xml";
	
			container.Kernel.ConfigurationStore.AddFacilityConfiguration( "nhibernate", confignode );
			
			return container;
		}
	}
}
