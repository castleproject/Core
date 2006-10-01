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

namespace Castle.Facilities.NHibernateIntegration.Tests
{
	using System;

	using NHibernate.Cfg;
	using NHibernate.Tool.hbm2ddl;
	
	using NUnit.Framework;

	using Castle.Windsor;


	public abstract class AbstractNHibernateTestCase
	{
		protected IWindsorContainer container;

		[SetUp]
		public void Init()
		{
			container = new WindsorContainer( GetContainerConfig() );

			// Reset tables

			Configuration cfg1 = (Configuration) container[ "sessionFactory1.cfg" ];
			SchemaExport export1 = new SchemaExport(cfg1);

			Configuration cfg2 = (Configuration) container[ "sessionFactory2.cfg" ];
			SchemaExport export2 = new SchemaExport(cfg2);

			export1.Create(false, true);
			export2.Create(false, true);

			ConfigureContainer();
		}

		protected virtual void ConfigureContainer()
		{
			
		}

		[TearDown]
		public void Dispose()
		{
			Configuration cfg1 = (Configuration) container[ "sessionFactory1.cfg" ];
			SchemaExport export1 = new SchemaExport(cfg1);

			Configuration cfg2 = (Configuration) container[ "sessionFactory2.cfg" ];
			SchemaExport export2 = new SchemaExport(cfg2);

			export2.Drop(false, true);
			export1.Drop(false, true);

			container.Dispose();

			container = null;
		}

		protected virtual String GetContainerConfig()
		{
			return ConfigHelper.ResolvePath("facilityconfig.xml");
		}
	}
}
