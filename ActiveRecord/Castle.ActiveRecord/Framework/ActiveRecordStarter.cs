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

using NHibernate.Tool.hbm2ddl;

namespace Castle.ActiveRecord
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Reflection;
	using System.ComponentModel;

	using NHibernate.Cfg;

	using Castle.Model.Configuration;

	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Scopes;
	using Castle.ActiveRecord.Framework.Internal;

	public delegate void SessionFactoryHolderDelegate(ISessionFactoryHolder holder);

	/// <summary>
	/// Performs the framework initialization.
	/// </summary>
	/// <remarks>
	/// This class is not thread safe.
	/// </remarks>
	public class ActiveRecordStarter
	{
		private static readonly EventHandlerList events = new EventHandlerList();
		private static readonly object SessionFactoryHolderCreatedEvent = new object();

		/// <summary>
		/// So others frameworks can intercept the 
		/// creation and act on the holder instance
		/// </summary>
		public static event SessionFactoryHolderDelegate SessionFactoryHolderCreated
		{
			add { events.AddHandler(SessionFactoryHolderCreatedEvent, value); }
			remove { events.RemoveHandler(SessionFactoryHolderCreatedEvent, value); }
		}

		/// <summary>
		/// Initialize the mappings using the configuration and 
		/// the list of types
		/// </summary>
		public static void Initialize( IConfigurationSource source, params Type[] types )
		{
			if (source == null) throw new ArgumentNullException("source");
			if (types == null) throw new ArgumentNullException("types");

			// First initialization
			ISessionFactoryHolder holder = 
				CreateSessionFactoryHolderImplementation(source);

			holder.ThreadScopeInfo = 
				CreateThreadScopeInfoImplementation(source);

			RaiseSessionFactoryHolderCreated(holder);

			ActiveRecordModel.type2Model.Clear();
			ActiveRecordBase._holder = holder;

			// Base configuration
			SetUpConfiguration(source, typeof(ActiveRecordBase), holder);

			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();

			ActiveRecordModelCollection models = builder.Models;

			foreach( Type type in types )
			{
				if ( models.Contains(type) || 
					type == typeof(ActiveRecordBase) || type == typeof(ActiveRecordValidationBase) )
				{
					continue;
				}
				else if (type.IsAbstract && typeof(ActiveRecordBase).IsAssignableFrom(type))
				{
					SetUpConfiguration(source, type, holder);

					continue;
				}
				else if (!IsActiveRecordType(type))
				{
					continue;
				}

				builder.Create( type );
			}

			GraphConnectorVisitor connectorVisitor = new GraphConnectorVisitor(models);
			connectorVisitor.VisitNodes( models );

			SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(models);
			semanticVisitor.VisitNodes( models );

			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();

			foreach(ActiveRecordModel model in models)
			{
				Configuration cfg = holder.GetConfiguration( holder.GetRootType(model.Type) );

				if (!model.IsNestedType && !model.IsDiscriminatorSubClass && !model.IsJoinedSubClass)
				{
					xmlVisitor.Reset(); xmlVisitor.CreateXml(model);

					String xml = xmlVisitor.Xml;
					
					if (xml != String.Empty)
					{
						cfg.AddXmlString(xml);
					}
				}
			}
		}

		/// <summary>
		/// Initialize the mappings using the configuration and 
		/// checking all the types on the specified <c>Assembly</c>
		/// </summary>
		public static void Initialize( Assembly assembly, IConfigurationSource source )
		{
			Type[] types = assembly.GetExportedTypes();

			ArrayList list = new ArrayList();

			foreach( Type type in types )
			{
				if ( !IsActiveRecordType(type) )
				{
					continue;
				}

				list.Add(type);
			}

			Initialize( source, (Type[]) list.ToArray( typeof(Type) ) );
		}

		/// <summary>
		/// Initialize the mappings using the configuration and 
		/// checking all the types on the specified Assemblies
		/// </summary>
		public static void Initialize( Assembly[] assemblies, IConfigurationSource source )
		{
			ArrayList list = new ArrayList();

			foreach(Assembly assembly in assemblies)
			{
				Type[] types = assembly.GetExportedTypes();

				foreach( Type type in types )
				{
					if ( !IsActiveRecordType(type) )
					{
						continue;
					}

					list.Add(type);
				}
			}

			Initialize( source, (Type[]) list.ToArray( typeof(Type) ) );
		}

		/// <summary>
		/// Initializes the framework reading the configuration from
		/// the <c>AppDomain</c> and checking all the types on the executing <c>Assembly</c>
		/// </summary>
		public static void Initialize()
		{
			IConfigurationSource source = System.Configuration.ConfigurationSettings.GetConfig("activerecord") as IConfigurationSource;
			
			if (source == null)
			{
				String message = "Could not obtain configuration from the AppDomain config file." + 
					" Sorry, but you have to fill the configuration or provide a " + 
					"IConfigurationSource instance yourself.";
				throw new System.Configuration.ConfigurationException(message);
			}
			
			Initialize( Assembly.GetExecutingAssembly(), source );
		}

		/// <summary>
		/// Generates and executes the creation scripts for the database.
		/// </summary>
		public static void CreateSchema()
		{
			CheckInitialized();

			foreach(Configuration config in ActiveRecordBase._holder.GetAllConfigurations())
			{
				SchemaExport export = CreateSchemaExport(config);

				try
				{
					export.Create( false, true );
				}
				catch(Exception ex)
				{
					throw new ActiveRecordException( "Could not create the schema", ex );
				}
			}
		}

		/// <summary>
		/// Executes the specified script to create/drop/change the database schema
		/// </summary>
		public static void CreateSchemaFromFile(String scriptFileName)
		{
			CheckInitialized();

			ARSchemaCreator arschema = new ARSchemaCreator( 
				ActiveRecordBase._holder.GetConfiguration( typeof(ActiveRecordBase) ) );

			arschema.Execute( scriptFileName );
		}

		/// <summary>
		/// Executes the specified script to create/drop/change the database schema
		/// against the specified database connection
		/// </summary>
		public static void CreateSchemaFromFile(String scriptFileName, IDbConnection connection)
		{
			CheckInitialized();

			if (connection == null) throw new ArgumentNullException("connection");

			String[] parts = ARSchemaCreator.OpenFileAndStripContents(scriptFileName);
			ARSchemaCreator.ExecuteScriptParts(connection, parts);
		}

		/// <summary>
		/// Generates and executes the Drop scripts for the database.
		/// </summary>
		public static void DropSchema()
		{
			CheckInitialized();

			foreach(Configuration config in ActiveRecordBase._holder.GetAllConfigurations())
			{
				SchemaExport export = CreateSchemaExport(config);

				try
				{
					export.Drop( false, true );
				}
				catch(Exception ex)
				{
					throw new ActiveRecordException( "Could not drop the schema", ex );
				}
			}
		}

		/// <summary>
		/// Generates and executes the drop scripts for the database.
		/// </summary>
		public static void GenerateDropScripts( String fileName )
		{
			// TODO: The export append to the file or erase it?

			CheckInitialized();

			foreach(Configuration config in ActiveRecordBase._holder.GetAllConfigurations())
			{
				SchemaExport export = CreateSchemaExport(config);

				try
				{
					export.SetOutputFile( fileName );
					export.Drop( false, false );
				}
				catch(Exception ex)
				{
					throw new ActiveRecordException( "Could not drop the schema", ex );
				}
			}
		}

		/// <summary>
		/// Generates the creation scripts for the database
		/// </summary>
		public static void GenerateCreationScripts( String fileName )
		{
			// TODO: The export append to the file or erase it?

			CheckInitialized();

			foreach(Configuration config in ActiveRecordBase._holder.GetAllConfigurations())
			{
				SchemaExport export = CreateSchemaExport(config);

				try
				{
					export.SetOutputFile( fileName );
					export.Create( false, false );
				}
				catch(Exception ex)
				{
					throw new ActiveRecordException( "Could not create the schema", ex );
				}
			}
		}

		private static SchemaExport CreateSchemaExport(Configuration cfg)
		{
			return new SchemaExport( cfg );
		}

		/// <summary>
		/// Return true if the type has a [ActiveRecord] attribute
		/// </summary>
		private static bool IsActiveRecordType(Type type)
		{
			return type.IsDefined(typeof(ActiveRecordAttribute), false);
		}

		private static void CheckInitialized()
		{
			if (ActiveRecordBase._holder == null)
			{
				throw new ActiveRecordException("Framework must be Initialize(d) first.");
			}
		}

		private static Configuration CreateConfiguration(IConfiguration config)
		{
			Configuration cfg = new Configuration();

			foreach(IConfiguration childConfig in config.Children)
			{
				cfg.Properties.Add(childConfig.Name, childConfig.Value);
			}

			return cfg;
		}

		private static void SetUpConfiguration(IConfigurationSource source, Type type, ISessionFactoryHolder holder)
		{
			IConfiguration config = source.GetConfiguration(type);
	
			if (config != null)
			{
				Configuration nconf = CreateConfiguration(config);
				holder.Register( type, nconf );
			}
		}

		private static void RaiseSessionFactoryHolderCreated(ISessionFactoryHolder holder)
		{
			SessionFactoryHolderDelegate evtDelegate = 
				(SessionFactoryHolderDelegate) events[SessionFactoryHolderCreatedEvent];

			if (evtDelegate != null)
			{
				evtDelegate(holder);
			}
		}

		private static ISessionFactoryHolder CreateSessionFactoryHolderImplementation(IConfigurationSource source)
		{
			if (source.SessionFactoryHolderImplementation != null)
			{
				Type sessionFactoryHolderType = source.SessionFactoryHolderImplementation;

				if (!typeof(ISessionFactoryHolder).IsAssignableFrom(sessionFactoryHolderType))
				{
					String message = String.Format("The specified type {0} does " + 
						"not implement the interface ISessionFactoryHolder", sessionFactoryHolderType.FullName);

					throw new ActiveRecordException( message );
				}

				return (ISessionFactoryHolder) Activator.CreateInstance(sessionFactoryHolderType);
			}
			else
			{
				return new SessionFactoryHolder();
			}
		}

		private static IThreadScopeInfo CreateThreadScopeInfoImplementation(IConfigurationSource source)
		{
			if (source.ThreadScopeInfoImplementation != null)
			{
				Type threadScopeType = source.ThreadScopeInfoImplementation;

				if (!typeof(IThreadScopeInfo).IsAssignableFrom(threadScopeType))
				{
					String message = String.Format("The specified type {0} does " + 
						"not implement the interface IThreadScopeInfo", threadScopeType.FullName);

					throw new ActiveRecordException( message );
				}

				return (IThreadScopeInfo) Activator.CreateInstance(threadScopeType);
			}
			else
			{
				return new ThreadScopeInfo();
			}
		}
	}
}
