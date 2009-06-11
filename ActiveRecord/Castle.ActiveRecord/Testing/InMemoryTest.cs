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

namespace Castle.ActiveRecord.Testing
{
	using System;
	using System.Reflection;
	using System.Collections.Generic;

	using Castle.ActiveRecord;
	using Castle.ActiveRecord.Framework.Config;

	/// <summary>
	/// Base class for in memory unit tests. This class does not contain any
	/// attributes specific to a testing framework.
	/// </summary>
	public abstract class InMemoryTest
	{
		/// <summary>
		/// The common test setup code. To activate it in a specific test framework,
		/// it must be called from a framework-specific setup-Method.
		/// </summary>
		public virtual void SetUp()
		{
			ActiveRecordStarter.ResetInitializationFlag();
			Dictionary<string, string> properties = new Dictionary<string, string>();
			properties.Add("connection.driver_class", "NHibernate.Driver.SQLite20Driver");
			properties.Add("dialect", "NHibernate.Dialect.SQLiteDialect");
			properties.Add("connection.provider", typeof(InMemoryConnectionProvider).AssemblyQualifiedName);
			properties.Add("connection.connection_string", "Data Source=:memory:;Version=3;New=True");
			properties.Add("proxyfactory.factory_class", "NHibernate.ByteCode.Castle.ProxyFactoryFactory, NHibernate.ByteCode.Castle");
			foreach (var p in GetProperties())
			{
				properties[p.Key] = p.Value; 
			}

			var config = new InPlaceConfigurationSource();
			config.Add(typeof(ActiveRecordBase), properties);
			foreach (var type in GetAdditionalBaseClasses())
			{
				config.Add(type, properties);
			}
			Configure(config);
			
			var types = GetTypes();
			var assemblies = GetAssemblies();

			if (types == null) types = new Type[0];
			if (assemblies == null) assemblies = new Assembly[0];

			ActiveRecordStarter.Initialize(assemblies,config,types);
			ActiveRecordStarter.CreateSchema();
		}

		/// <summary>
		/// The common test teardown code. To activate it in a specific test framework,
		/// it must be called from a framework-specific teardown-Method.
		/// </summary>
		public virtual void TearDown()
		{
			ActiveRecordStarter.ResetInitializationFlag();
			InMemoryConnectionProvider.Restart();
		}

		/// <summary>
		/// Method that must be overridden by the test fixtures to return the types
		/// that should be initialized. The stub returns an empty array.
		/// </summary>
		/// <returns>The types to initialize.</returns>
		public virtual Type[] GetTypes()
		{
			return new Type[0];
		}

		/// <summary>
		/// Method that must be overridden by the test fixtures to return the assemblies
		/// that should be initialized. The stub returns an empty array.
		/// </summary>
		/// <returns></returns>
		public virtual Assembly[] GetAssemblies()
		{
			return new Assembly[0];
		}

		/// <summary>
		/// Hook to allow the initialization of additional base classes. <see cref="ActiveRecordBase"/> is 
		/// added everytime and must not be returned.
		/// </summary>
		/// <returns>An array of additional base classes for initialization</returns>
		public virtual Type[] GetAdditionalBaseClasses()
		{
			return new Type[0];
		}

		/// <summary>
		/// Hook to add additional properties for each base class' configuration. As an example, "show_sql" can
		/// be added to verify the behaviour of NHibernate in specific situations.
		/// </summary>
		/// <returns>A dictionary of additional or custom properties.</returns>
		public virtual IDictionary<string, string> GetProperties()
		{
			return new Dictionary<string, string>();
		}

		/// <summary>
		/// Hook for modifying the configuration before initialization
		/// </summary>
		/// <param name="config"></param>
		public virtual void Configure(InPlaceConfigurationSource config)
		{
		}
	}
}