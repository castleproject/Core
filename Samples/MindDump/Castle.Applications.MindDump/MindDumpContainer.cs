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

namespace Castle.Applications.MindDump
{
	using System;

	using Castle.Windsor;
	using Castle.Windsor.Configuration.Xml;

	using Castle.MicroKernel;

	using Castle.Facilities.AutomaticTransactionManagement;
	using Castle.Facilities.NHibernateIntegration;

	using Castle.Applications.MindDump.Dao;
	using Castle.Applications.MindDump.Services;


	public class MindDumpContainer : WindsorContainer
	{
		public MindDumpContainer() : this( new XmlConfigurationStore("../app_config.xml") )
		{
		}

		public MindDumpContainer(IConfigurationStore store) : base(store)
		{
			Init();
		}

		public void Init()
		{
			RegisterFacilities();
			RegisterComponents();
		}

		private void RegisterFacilities()
		{
			AddFacility( "nhibernate", new NHibernateFacility() );
			AddFacility( "transaction", new TransactionFacility() );
		}

		protected void RegisterComponents()
		{
			AddComponent( "event.channel", typeof(IMindDumpEventPublisher), 
				typeof(MindDumpEventChannel) );

			AddComponent( "author.dao", typeof(AuthorDao) );
			AddComponent( "blog.dao", typeof(BlogDao) );
			AddComponent( "post.dao", typeof(PostDao) );
			AddComponent( "authentication", typeof(AuthenticationService) );
			AddComponent( "account", typeof(AccountService) );
			AddComponent( "encryption", typeof(EncryptionService) );
			AddComponent( "blogMaintenanceService", typeof(BlogMaintenanceService) );
		}
	}
}
