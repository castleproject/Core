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

namespace Castle.ActiveRecord.Framework.Config
{
	using NHibernate.Bytecode;
	using NHibernate.ByteCode.Castle;
	using NHibernate.Connection;
	using NHibernate.Dialect;
	using NHibernate.Driver;

	/// <summary>
	/// Fluent configuration of ActiveRecord storage options.
	/// </summary>
	public class FluentStorageConfiguration : DefaultStorageConfiguration
	{
		/// <summary>
		/// Adds an inital <see cref="FluentStorageTypeSelection"/> that can be
		/// used for specifying the types that use this storage configured.
		/// </summary>
		public FluentStorageTypeSelection For
		{
			get
			{
				var selection = new FluentStorageTypeSelection(this);
				TypeSelectionList.Add(selection);
				return selection;
			}
		}

		/// <summary>
		/// Sets reasonable defaults for the specified type of database.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		///<returns>The fluent configuration itself.</returns>
		public FluentStorageConfiguration DefaultsFor<T>() where T : IDatabaseConfiguration
		{
			return this;
		}

		///<summary>
		/// Sets the connection string per name
		///</summary>
		///<param name="connecctionStringName">The name of the connection string</param>
		///<returns>The fluent configuration itself.</returns>
		public FluentStorageConfiguration ConnectionStringName(string connecctionStringName)
		{
			ConfigurationValues.Add("connection.connection_string_name", connecctionStringName);
			return this;
		}

		///<summary>
		/// Sets the driver for the configuration
		///</summary>
		///<typeparam name="T">The driver class</typeparam>
		///<returns>The fluent configuration itself</returns>
		public FluentStorageConfiguration Driver<T>() where T : IDriver
		{
			ConfigurationValues.Add("connection.driver_class", typeof (T).FullName);
			return this;
		}

		/// <summary>
		/// Sets the connection provider
		/// </summary>
		/// <typeparam name="T">The connection provider class to use.</typeparam>
		/// <returns>The fluent configuration itself.</returns>
		public FluentStorageConfiguration ConnectionProvider<T>() where T : IConnectionProvider
		{
			ConfigurationValues.Add("connection.provider", typeof (T).FullName);
			return this;
		}

		/// <summary>
		/// Sets the dialect.
		/// </summary>
		/// <typeparam name="T">The dialect type to use.</typeparam>
		/// <returns>The fluent configuration itself.</returns>
		public FluentStorageConfiguration Dialect<T>() where T : Dialect
		{
			ConfigurationValues.Add("dialect", typeof (T).FullName);
			return this;
		}

		/// <summary>
		/// The proxy factory to use. This value defaults to 
		/// <see cref="ProxyFactoryFactory"/>.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public FluentStorageConfiguration ProxiedBy<T>() where T : IProxyFactoryFactory
		{
			ConfigurationValues.Add("proxyfactory.factory_class", GetTypeName<T>());
			return this;
		}

		/// <summary>
		/// Sets the connection string
		/// </summary>
		/// <param name="connectionString">The connection string to use.</param>
		/// <returns>The fluent configuration itself.</returns>
		public FluentStorageConfiguration ConnectionString(string connectionString)
		{
			ConfigurationValues.Add("connection.connection_string", connectionString);
			return this;
		}

		/// <summary>
		/// Switches SQL console output on.
		/// </summary>
		/// <returns>The fluent configuration itself.</returns>
		public FluentStorageConfiguration ShowSql()
		{
			return ShowSql(true);
		}

		/// <summary>
		/// Switches SQL console output on or off.
		/// </summary>
		/// <param name="showSql">Whether to show the Sql or not.</param>
		/// <returns>The fluent configuration itself.</returns>
		public FluentStorageConfiguration ShowSql(bool showSql)
		{
			return this;
		}
	}
}