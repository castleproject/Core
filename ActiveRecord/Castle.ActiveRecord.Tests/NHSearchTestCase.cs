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

namespace Castle.ActiveRecord.Tests
{
	using System;
	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Config;
	using Castle.ActiveRecord.Framework.Scopes;
	using Castle.ActiveRecord.Tests.Model;
	using Castle.ActiveRecord.Tests.Model.LazyModel;
	using NHibernate;
	using NHibernate.Search.Event;
	using NUnit.Framework;
	using Castle.ActiveRecord.Tests.Event;
	using Castle.Core.Configuration;

	[TestFixture]
	public class NHSearchTestCase : AbstractEventListenerTest
	{
		

		[Test]
		public void TestListenersAreAddedWhenConfigured()
		{
			var configSource = System.Configuration.ConfigurationManager.GetSection("activerecord") as XmlConfigurationSource;
			AddSearchProperties(configSource);
			bool priorValue = configSource.Searchable;
			configSource.Searchable = true;

			ActiveRecordStarter.Initialize(configSource, typeof(Post), typeof(Blog));
			Recreate();

			Post.DeleteAll();
			Blog.DeleteAll();

			try
			{
				AssertListenerWasRegistered<FullTextIndexEventListener>(listeners => listeners.PostUpdateEventListeners);
				AssertListenerWasRegistered<FullTextIndexEventListener>(listeners => listeners.PostDeleteEventListeners);
				AssertListenerWasRegistered<FullTextIndexEventListener>(listeners => listeners.PostInsertEventListeners);
			}
			finally
			{
				// Restore config source we corrupted before.
				configSource.Searchable = priorValue;
				RemoveSearchProperties(configSource);
			}
		}

		// This is clipboard inheritance for a reason: To show the differences
		// when the configuration is not properly configured for NHibernate.Search
		[Test]
		public void TestListenersNotAreAddedWhenNotConfigured()
		{
			var configSource = System.Configuration.ConfigurationManager.GetSection("activerecord") as XmlConfigurationSource;
			// AddSearchProperties(configSource);
			bool priorValue = configSource.Searchable;
			configSource.Searchable = true;

			ActiveRecordStarter.Initialize(configSource, typeof(Post), typeof(Blog));
			Recreate();

			Post.DeleteAll();
			Blog.DeleteAll();

			try
			{
				AssertListenerWasNotRegistered<FullTextIndexEventListener>(listeners => listeners.PostUpdateEventListeners);
				AssertListenerWasNotRegistered<FullTextIndexEventListener>(listeners => listeners.PostDeleteEventListeners);
				AssertListenerWasNotRegistered<FullTextIndexEventListener>(listeners => listeners.PostInsertEventListeners);
			}
			finally
			{
				// Restore config source we corrupted before.
				configSource.Searchable = priorValue;
				// RemoveSearchProperties(configSource);
			}
		}

		private MutableConfiguration[] searchConfig;

		private void AddSearchProperties(XmlConfigurationSource configSource)
		{
			searchConfig = new MutableConfiguration[3];
			searchConfig[0] = new MutableConfiguration("hibernate.search.default.directory_provider", "NHibernate.Search.Storage.FSDirectoryProvider, NHibernate.Search");
			searchConfig[1] = new MutableConfiguration("hibernate.search.default.indexBase", "~/index");
			searchConfig[2] = new MutableConfiguration("hibernate.search.analyzer", "Lucene.Net.Analysis.Standard.StandardAnalyzer, Lucene.Net");

			var cfg = configSource.GetConfiguration(typeof(ActiveRecordBase));
			cfg.Children.AddRange(searchConfig);
		}

		private void RemoveSearchProperties(XmlConfigurationSource configSource)
		{
			var cfg = configSource.GetConfiguration(typeof(ActiveRecordBase));

			foreach (var prop in searchConfig)
			{
				cfg.Children.Remove(prop);
			}
		}
	}
}
