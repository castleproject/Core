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

namespace BlogSample
{
	using System;
	using System.Collections;
	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework.Config;


	public class App
	{
		public static void Main()
		{
			Hashtable properties = new Hashtable();

			properties.Add("hibernate.connection.driver_class", "NHibernate.Driver.SqlClientDriver");
			properties.Add("hibernate.dialect", "NHibernate.Dialect.MsSql2000Dialect");
			properties.Add("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
			properties.Add("hibernate.connection.connection_string", "Data Source=.;Initial Catalog=test;Integrated Security=SSPI");

			InPlaceConfigurationSource source = new InPlaceConfigurationSource();
			source.Add(typeof(ActiveRecordBase), properties);

			// XmlConfigurationSource source = new XmlConfigurationSource("../appconfig.xml");

			ActiveRecordStarter.Initialize( source, typeof(Blog), typeof(Post) );

			// If you want to let AR to create the schema

			ActiveRecordStarter.CreateSchema();

			// Common usage

			Post.DeleteAll();
			Blog.DeleteAll();

			Blog blog = new Blog("somename");
			blog.Author = "hammett";
			blog.Save();

			Post post = new Post(blog, "title", "contents", "castle");
			post.Save();

			Post.DeleteAll();
			Blog.DeleteAll();

			// Using Session Scope

			using(new SessionScope())
			{
				blog = new Blog("somename");
				blog.Author = "hammett";
				blog.Save();

				post = new Post(blog, "title", "contents", "castle");
				post.Save();
			}

			// Using transaction scope

			Post.DeleteAll();
			Blog.DeleteAll();

			TransactionScope transaction = new TransactionScope();

			try
			{
				blog = new Blog("somename");
				blog.Author = "hammett";
				blog.Save();

				post = new Post(blog, "title", "contents", "castle");
				post.Save();
			}
			catch(Exception)
			{
				transaction.VoteRollBack();
			}
			finally
			{
				transaction.Dispose();
			}

			ActiveRecordStarter.DropSchema();
		}
	}
}
