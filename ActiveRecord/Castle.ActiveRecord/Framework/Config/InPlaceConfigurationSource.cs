// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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
	using System;
	using System.Collections.Generic;
	using System.Configuration;
	using System.Text.RegularExpressions;
	using Castle.ActiveRecord.Framework.Scopes;
	using Castle.Core.Configuration;

	/// <summary>
	/// Enum for database types support for configuration construction. 
	/// Not to be confused by databases supported by ActiveRecord
	/// </summary>
	public enum DatabaseType
	{
		/// <summary>
		/// Microsoft SQL Server 2005
		/// </summary>
		MSSQLServer2005, 
		/// <summary>
		/// Microsoft SQL Server 2000
		/// </summary>
		MSSQLServer2000
	}

	/// <summary>
	/// Usefull for test cases.
	/// </summary>
	public class InPlaceConfigurationSource : IConfigurationSource
	{
		private readonly IDictionary<Type, IConfiguration> _type2Config = new Dictionary<Type, IConfiguration>();
		private Type threadScopeInfoImplementation;
		private Type sessionFactoryHolderImplementation;
		private Type namingStrategyImplementation;
		private bool debug;
		private bool isLazyByDefault;
		private bool pluralizeTableNames;
		private bool verifyModelsAgainstDBSchema;

		/// <summary>
		/// Initializes a new instance of the <see cref="InPlaceConfigurationSource"/> class.
		/// </summary>
		public InPlaceConfigurationSource()
		{
		}

		#region IConfigurationSource Members

		/// <summary>
		/// Return a type that implements
		/// the interface <see cref="IThreadScopeInfo"/>
		/// </summary>
		/// <value></value>
		public Type ThreadScopeInfoImplementation
		{
			get { return threadScopeInfoImplementation; }
			set { threadScopeInfoImplementation = value; }
		}

		/// <summary>
		/// Return a type that implements
		/// the interface <see cref="ISessionFactoryHolder"/>
		/// </summary>
		/// <value></value>
		public Type SessionFactoryHolderImplementation
		{
			get { return sessionFactoryHolderImplementation; }
			set { sessionFactoryHolderImplementation = value; }
		}

		/// <summary>
		/// Return a type that implements
		/// the interface NHibernate.Cfg.INamingStrategy
		/// </summary>
		/// <value></value>
		public Type NamingStrategyImplementation
		{
			get { return namingStrategyImplementation; }
			set { namingStrategyImplementation = value; }
		}

		/// <summary>
		/// Return an <see cref="IConfiguration"/> for the specified type.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public IConfiguration GetConfiguration(Type type)
		{
			IConfiguration configuration;
			_type2Config.TryGetValue(type, out configuration);
			return configuration;
		}

		/// <summary>
		/// Gets a value indicating whether this <see cref="IConfigurationSource"/> produce debug information
		/// </summary>
		/// <value><c>true</c> if debug; otherwise, <c>false</c>.</value>
		public bool Debug
		{
			get { return debug; }
		}

		/// <summary>
		/// Gets a value indicating whether the entities should be lazy by default.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if entities should be lazy by default; otherwise, <c>false</c>.
		/// </value>
		public bool IsLazyByDefault
		{
			get { return isLazyByDefault; }
		}

		/// <summary>
		/// Gets a value indicating whether table names are assumed plural by default. 
		/// </summary>
		/// <value>
		/// 	<c>true</c> if table names should be pluralized by default; otherwise, <c>false</c>.
		/// </value>
		public bool PluralizeTableNames
		{
			get { return pluralizeTableNames; }
			set { pluralizeTableNames = value; }
		}

		/// <summary>
		/// Gets or Sets a value indicating whether the models should be verified against the db schema on Initialisation.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if models should be verified; otherwise, <c>false</c>.
		/// </value>
		public bool VerifyModelsAgainstDBSchema
		{
			get { return verifyModelsAgainstDBSchema; }
			set { verifyModelsAgainstDBSchema = value; }
		}

		#endregion

		/// <summary>
		/// Builds a InPlaceConfigurationSource set up to access a MS SQL server database using integrated security.
		/// </summary>
		/// <param name="server">The server.</param>
		/// <param name="initialCatalog">The initial catalog.</param>
		/// <returns></returns>
		public static InPlaceConfigurationSource BuildForMSSqlServer(string server, string initialCatalog)
		{
			if (string.IsNullOrEmpty(server)) throw new ArgumentNullException("server");
			if (string.IsNullOrEmpty(initialCatalog)) throw new ArgumentNullException("initialCatalog");

			return Build(DatabaseType.MSSQLServer2005, "Server=" + server + ";initial catalog=" + initialCatalog + ";Integrated Security=SSPI");
		}

		/// <summary>
		/// Builds a InPlaceConfigurationSource set up to access a MS SQL server database using the specified username and password.
		/// </summary>
		/// <param name="server">The server.</param>
		/// <param name="initialCatalog">The initial catalog.</param>
		/// <param name="username">The username.</param>
		/// <param name="password">The password.</param>
		/// <returns></returns>
		public static InPlaceConfigurationSource BuildForMSSqlServer(string server, string initialCatalog, string username, string password)
		{
			if (string.IsNullOrEmpty(server)) throw new ArgumentNullException("server");
			if (string.IsNullOrEmpty(initialCatalog)) throw new ArgumentNullException("initialCatalog");
			if (string.IsNullOrEmpty(username)) throw new ArgumentNullException("username");
			if (string.IsNullOrEmpty(password)) throw new ArgumentNullException("password");

			return Build(DatabaseType.MSSQLServer2005, "Server=" + server + ";initial catalog=" + initialCatalog + ";User id=" + username + ";password=" + password);
		}

		/// <summary>
		/// Builds an InPlaceConfiguratioSource for the specified database.
		/// </summary>
		/// <param name="database">The database.</param>
		/// <param name="connectionString">The connection string.</param>
		/// <returns></returns>
		public static InPlaceConfigurationSource Build(DatabaseType database, string connectionString)
		{
			if (string.IsNullOrEmpty(connectionString)) throw new ArgumentNullException("connectionString");

			InPlaceConfigurationSource config = new InPlaceConfigurationSource();

			Dictionary<string,string> parameters = new Dictionary<string,string>();
			parameters["connection.provider"] = "NHibernate.Connection.DriverConnectionProvider";
			parameters["cache.use_second_level_cache"] = "false";

			if (database == DatabaseType.MSSQLServer2000)
			{
				parameters["connection.driver_class"] = "NHibernate.Driver.SqlClientDriver";
				parameters["dialect"] = "NHibernate.Dialect.MsSql2000Dialect";
				parameters["connection.connection_string"] = connectionString;
			}
			else if (database == DatabaseType.MSSQLServer2005)
			{
				parameters["connection.driver_class"] = "NHibernate.Driver.SqlClientDriver";
				parameters["dialect"] = "NHibernate.Dialect.MsSql2005Dialect";
				parameters["connection.connection_string"] = connectionString;
			}

			config.Add(typeof(ActiveRecordBase), parameters);

			return config;
		}

		/// <summary>
		/// Sets a value indicating whether this instance is running in web app.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is running in web app; otherwise, <c>false</c>.
		/// </value>
		public bool IsRunningInWebApp
		{
			set { SetUpThreadInfoType(value, null); }
		}

		/// <summary>
		/// Adds the specified type with the properties
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="properties">The properties.</param>
		public void Add(Type type, IDictionary<string,string> properties)
		{
			Add(type, ConvertToConfiguration(properties));
		}

		/// <summary>
		/// Adds the specified type with configuration
		/// </summary>
		/// <param name="type">The type.</param>
		/// <param name="config">The config.</param>
		public void Add(Type type, IConfiguration config)
		{
			ProcessConfiguration(config);

			_type2Config[type] = config;
		}

		/// <summary>
		/// Sets the type of the thread info.
		/// </summary>
		/// <param name="isWeb">if we run in a web context or not</param>
		/// <param name="customType">Type of the custom implementation</param>
		protected void SetUpThreadInfoType(bool isWeb, String customType)
		{
			Type threadInfoType = null;

			if (isWeb)
			{
				threadInfoType = typeof(WebThreadScopeInfo);
			}

			if (customType != null && customType != String.Empty)
			{
				String typeName = customType;

				threadInfoType = Type.GetType(typeName, false, false);

				if (threadInfoType == null)
				{
					String message = String.Format("The type name {0} could not be found", typeName);

					throw new ActiveRecordException(message);
				}
			}

			ThreadScopeInfoImplementation = threadInfoType;
		}

		/// <summary>
		/// Sets the type of the session factory holder.
		/// </summary>
		/// <param name="customType">Custom implementation</param>
		protected void SetUpSessionFactoryHolderType(String customType)
		{
			Type sessionFactoryHolderType = typeof(SessionFactoryHolder);

			if (customType != null && customType != String.Empty)
			{
				String typeName = customType;

				sessionFactoryHolderType = Type.GetType(typeName, false, false);

				if (sessionFactoryHolderType == null)
				{
					String message = String.Format("The type name {0} could not be found", typeName);

					throw new ActiveRecordException(message);
				}
			}

			SessionFactoryHolderImplementation = sessionFactoryHolderType;
		}

		/// <summary>
		/// Sets the type of the naming strategy.
		/// </summary>
		/// <param name="customType">Custom implementation type name</param>
		protected void SetUpNamingStrategyType(String customType)
		{
			if (customType != null && customType != String.Empty)
			{
				String typeName = customType;

				Type namingStrategyType = Type.GetType(typeName, false, false);

				if (namingStrategyType == null)
				{
					String message = String.Format("The type name {0} could not be found", typeName);

					throw new ActiveRecordException(message);
				}

				NamingStrategyImplementation = namingStrategyType;
			}
		}

		/// <summary>
		/// Sets the debug flag.
		/// </summary>
		/// <param name="isDebug">if set to <c>true</c> Active Record will produce debug information.</param>
		protected void SetDebugFlag(bool isDebug)
		{
			debug = isDebug;
		}

		/// <summary>
		/// Set whatever entities are lazy by default or not.
		/// </summary>
		protected void SetIsLazyByDefault(bool lazyByDefault)
		{
			isLazyByDefault = lazyByDefault;
		}

		/// <summary>
		/// Sets the debug flag.
		/// </summary>
		/// <param name="verifyModelsAgainstDBSchema">if set to <c>true</c> Active Record will verify the models against the db schema on startup.</param>
		protected void SetVerifyModelsAgainstDBSchema(bool verifyModelsAgainstDBSchema)
		{
			this.verifyModelsAgainstDBSchema = verifyModelsAgainstDBSchema;
		}
		
		/// <summary>
		/// Sets the pluralizeTableNames flag.
		/// </summary>
		/// <param name="pluralize">if set to <c>true</c> Active Record will pluralize inferred table names.</param>
		protected void SetPluralizeTableNames(bool pluralize)
		{
			pluralizeTableNames = pluralize;
		}

		private static IConfiguration ConvertToConfiguration(IDictionary<string,string> properties)
		{
			MutableConfiguration conf = new MutableConfiguration("Config");

			foreach(KeyValuePair<string,string> entry in properties)
			{
				conf.Children.Add(new MutableConfiguration(entry.Key, entry.Value));
			}

			return conf;
		}

		/// <summary>
		/// Processes the configuration applying any substitutions.
		/// </summary>
		/// <param name="config">The configuration</param>
		private static void ProcessConfiguration(IConfiguration config)
		{
			const string ConnectionStringKey = "connection.connection_string";

			for(int i = 0; i < config.Children.Count; ++i)
			{
				IConfiguration property = config.Children[i];

				if (property.Name.IndexOf(ConnectionStringKey) >= 0)
				{
					String value = property.Value;
					Regex connectionStringRegex = new Regex(@"ConnectionString\s*=\s*\$\{(?<ConnectionStringName>[^}]+)\}");

					if (connectionStringRegex.IsMatch(value))
					{
						string connectionStringName = connectionStringRegex.Match(value).
							Groups["ConnectionStringName"].Value;
						value = ConfigurationManager.ConnectionStrings[connectionStringName].ConnectionString;
						config.Children[i] = new MutableConfiguration(property.Name, value);
					}
				}
			}
		}
	}
}
