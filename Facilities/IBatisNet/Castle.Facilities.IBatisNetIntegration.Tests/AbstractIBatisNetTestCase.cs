#region Licence
/// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
///  
/// Licensed under the Apache License, Version 2.0 (the "License");
/// you may not use this file except in compliance with the License.
/// You may obtain a copy of the License at
///  
/// http://www.apache.org/licenses/LICENSE-2.0
///  
/// Unless required by applicable law or agreed to in writing, software
/// distributed under the License is distributed on an "AS IS" BASIS,
/// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
/// See the License for the specific language governing permissions and
/// limitations under the License.
/// 
/// -- 
/// 
/// This facility was a contribution kindly 
/// donated by Gilles Bayon <gilles.bayon@gmail.com>
/// 
/// --
#endregion

namespace Castle.Facilities.IBatisNetIntegration.Tests
{
	using System;
	using System.Threading;

	using NUnit.Framework;

	using Castle.Model.Configuration;

	using Castle.Windsor;
	
	using Castle.MicroKernel.SubSystems.Configuration;

	/// <summary>
	/// Summary description for AbstractNHibernateTestCase.
	/// </summary>
	public abstract class AbstractIBatisNetTestCase : BaseTest 
	{
		public const string DATA_MAPPER = "sqlServerSqlMap";

		[SetUp]
		public virtual void InitDb()
		{
		}

		protected virtual IWindsorContainer CreateConfiguredContainer()
		{
			IWindsorContainer container = new WindsorContainer(new DefaultConfigurationStore());

			MutableConfiguration confignode = new MutableConfiguration("facility");

			IConfiguration sqlMap = confignode.Children.Add(new MutableConfiguration("sqlMap"));
			sqlMap.Attributes["id"] = DATA_MAPPER;
			sqlMap.Attributes["config"] = "sqlMap.config";

			container.Kernel.ConfigurationStore.AddFacilityConfiguration("IBatisNet", confignode);

			return container;
		}
	}
}
