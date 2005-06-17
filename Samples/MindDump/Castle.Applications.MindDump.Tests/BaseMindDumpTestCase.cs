// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.Applications.MindDump.Tests
{
	using System;

	using NUnit.Framework;

	using Castle.Windsor.Configuration.Xml;

	using Castle.Applications.MindDump.Dao;

	using MySql.Data.MySqlClient;


	public abstract class BaseMindDumpTestCase
	{
		private MindDumpContainer _container;

		[SetUp]
		public void InitContainer()
		{
			_container = new MindDumpContainer( 
				new XmlConfigurationStore("../app_test_config.xml") );

			ResetDatabase();
		}

		[TearDown]
		public void DisposeContainer()
		{
			_container.Dispose();
			_container = null;
		}

		public MindDumpContainer Container
		{
			get { return _container; }
		}

		protected void ResetDatabase()
		{
			(_container[ typeof(PostDao) ] as PostDao).DeleteAll();
			(_container[ typeof(BlogDao) ] as BlogDao).DeleteAll();
			(_container[ typeof(AuthorDao) ] as AuthorDao).DeleteAll();
		}
	}
}
