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

namespace Castle.ActiveRecord.Tests
{
	using System;
	using System.Collections;

	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Config;


	public abstract class AbstractActiveRecordTest
	{
		protected IConfigurationSource GetConfigSource()
		{
			InPlaceConfigurationSource source = new InPlaceConfigurationSource();
			Hashtable properties = new Hashtable();
			properties.Add("hibernate.connection.driver_class", "NHibernate.Driver.SqlClientDriver");
			properties.Add("hibernate.dialect", "NHibernate.Dialect.MsSql2000Dialect");
			properties.Add("hibernate.connection.provider", "NHibernate.Connection.DriverConnectionProvider");
			properties.Add("hibernate.connection.connection_string", "UID=susa;Password=OverKkk;Initial Catalog=test;Data Source=.");

			source.Add( typeof(ActiveRecordBase), properties );

			return source;
		}
	}
}
