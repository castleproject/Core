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

namespace Castle.ActiveRecord
{
	using System;
	using System.Collections;
	using System.Data;
	using System.Reflection;

	using NHibernate.Cfg;
	using NHibernate.Tool.hbm2ddl;
	
	using Castle.Core.Configuration;
	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Config;
	using Castle.ActiveRecord.Framework.Scopes;
	using Castle.ActiveRecord.Framework.Internal;

	public delegate void SessionFactoryHolderDelegate(ISessionFactoryHolder holder);

	/// <summary>
	/// Performs the framework initialization.
	/// </summary>
	/// <remarks>
	/// This class is not thread safe.
	/// </remarks>
	public sealed class ActiveRecordStarter
	{
		private static readonly Object lockConfig = new object();

		private static bool isInitialized = false;
		
		private static IDictionary registeredTypes;
		
		/// <summary>
		/// This is saved so one can invoke <see cref="RegisterTypes"/> later
		/// </summary>
		private static IConfigurationSource configSource;

		/// <summary>
		/// So others frameworks can intercept the 
		/// creation and act on the holder instance
		/// </summary>
		public static event SessionFactoryHolderDelegate SessionFactoryHolderCreated;

		/// <summary>
		/// Initialize the mappings using the configuration and 
		/// the list of types
		/// </summary>
		public static void Initialize(IConfigurationSource source, params Type[] types)
		{
			lock(lockConfig)
			{
				if (isInitialized)
				{
					throw new ActiveRecordInitializationException("You can't invoke ActiveRecordStarter.Initialize more than once");
				}

				if (source == null)
				{
					throw new ArgumentNullException("source");
				}
				if (types == null)
				{
					throw new ArgumentNullException("types");
				}
				
				registeredTypes = new Hashtable();
				
				configSource = source;

				// First initialization
				ISessionFactoryHolder holder = CreateSessionFactoryHolderImplementation(source);

				holder.ThreadScopeInfo = CreateThreadScopeInfoImplementation(source);

				RaiseSessionFactoryHolderCreated(holder);

				ActiveRecordBase.holder = holder;
				ActiveRecordModel.type2Model.Clear();
				ActiveRecordModel.isDebug = source.Debug;

				// Sets up base configuration
				SetUpConfiguration(source, typeof(ActiveRecordBase), holder);

				RegisterTypes(holder, source, types, true);

				isInitialized = true;
			}
		}

		/// <summary>
		/// Initialize the mappings using the configuration and 
		/// checking all the types on the specified <c>Assembly</c>
		/// </summary>
		public static void Initialize(Assembly assembly, IConfigurationSource source)
		{
			ArrayList list = new ArrayList();

			CollectValidActiveRecordTypesFromAssembly(assembly, list, source);

			Initialize(source, (Type[]) list.ToArray(typeof(Type)));
		}

		/// <summary>
		/// Initialize the mappings using the configuration and 
		/// checking all the types on the specified Assemblies
		/// </summary>
		public static void Initialize(Assembly[] assemblies, IConfigurationSource source)
		{
			ArrayList list = new ArrayList();

			foreach(Assembly assembly in assemblies)
			{
                CollectValidActiveRecordTypesFromAssembly(assembly, list, source);
			}

			Initialize(source, (Type[]) list.ToArray(typeof(Type)));
		}

		/// <summary>
		/// Initializes the framework reading the configuration from
		/// the <c>AppDomain</c> and checking all the types on the executing <c>Assembly</c>
		/// </summary>
		public static void Initialize()
		{
			IConfigurationSource source = ActiveRecordSectionHandler.Instance;

			Initialize(Assembly.GetExecutingAssembly(), source);
		}

		/// <summary>
		/// Registers new types in ActiveRecord
		/// Usefull for dynamic type-adding after initialization
		/// </summary>
		/// <param name="types"></param>
		public static void RegisterTypes(params Type[] types)
		{
			RegisterTypes(ActiveRecordBase.holder, configSource, types, false);
		}

		/// <summary>
		/// Generates and executes the creation scripts for the database.
		/// </summary>
		public static void CreateSchema()
		{
			CheckInitialized();

			foreach(Configuration config in ActiveRecordBase.holder.GetAllConfigurations())
			{
				SchemaExport export = CreateSchemaExport(config);

				try
				{
					export.Create(false, true);
				}
				catch (Exception ex)
				{
					throw new ActiveRecordException("Could not create the schema", ex);
				}
			}
		}

		/// <summary>
		/// Executes the specified script to create/drop/change the database schema
		/// </summary>
		public static void CreateSchemaFromFile(String scriptFileName)
		{
			CheckInitialized();

			CreateSchemaFromFile(scriptFileName, ActiveRecordBase.holder.CreateSession(typeof(ActiveRecordBase)).Connection);
		}

		/// <summary>
		/// Executes the specified script to create/drop/change the database schema
		/// against the specified database connection
		/// </summary>
		public static void CreateSchemaFromFile(String scriptFileName, IDbConnection connection)
		{
			CheckInitialized();

			if (connection == null)
			{
				throw new ArgumentNullException("connection");
			}

			String[] parts = ARSchemaCreator.OpenFileAndStripContents(scriptFileName);
			ARSchemaCreator.ExecuteScriptParts(connection, parts);
		}

		/// <summary>
		/// Generates and executes the Drop scripts for the database.
		/// </summary>
		public static void DropSchema()
		{
			CheckInitialized();

			foreach(Configuration config in ActiveRecordBase.holder.GetAllConfigurations())
			{
				SchemaExport export = CreateSchemaExport(config);

				try
				{
					export.Drop(false, true);
				}
				catch (Exception ex)
				{
					throw new ActiveRecordException("Could not drop the schema", ex);
				}
			}
		}

		/// <summary>
		/// Generates and executes the drop scripts for the database.
		/// </summary>
		public static void GenerateDropScripts(String fileName)
		{
			// TODO: The export append to the file or erase it?

			CheckInitialized();

			foreach(Configuration config in ActiveRecordBase.holder.GetAllConfigurations())
			{
				SchemaExport export = CreateSchemaExport(config);

				try
				{
					export.SetOutputFile(fileName);
					export.Drop(false, false);
				}
				catch (Exception ex)
				{
					throw new ActiveRecordException("Could not drop the schema", ex);
				}
			}
		}

		/// <summary>
		/// Generates the creation scripts for the database
		/// </summary>
		public static void GenerateCreationScripts(String fileName)
		{
			// TODO: The export append to the file or erase it?

			CheckInitialized();

			foreach(Configuration config in ActiveRecordBase.holder.GetAllConfigurations())
			{
				SchemaExport export = CreateSchemaExport(config);

				try
				{
					export.SetOutputFile(fileName);
					export.Create(false, false);
				}
				catch (Exception ex)
				{
					throw new ActiveRecordException("Could not create the schema", ex);
				}
			}
		}

		/// <summary>
		/// Intended to be used only by test cases
		/// </summary>
		public static void ResetInitializationFlag()
		{
			isInitialized = false;
		}

		private static ActiveRecordModelCollection BuildModels(ISessionFactoryHolder holder, 
		                                                       IConfigurationSource source,
		                                                       Type[] types, bool ignoreProblematicTypes)
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();

			ActiveRecordModelCollection models = builder.Models;

			foreach(Type type in types)
			{
				if (ShouldIgnoreType(type))
				{
					if (ignoreProblematicTypes)
					{
						continue;
					}
					else
					{
						throw new ActiveRecordException(
							String.Format("Type `{0}` is registered already", type.FullName));
					}
				}
				else if (TypeDefinesADatabaseBoundary(type))
				{
					SetUpConfiguration(source, type, holder);
					
					continue;
				}
				else if (!IsActiveRecordType(type))
				{
					if (ignoreProblematicTypes)
					{
						continue;
					}
					else
					{
						throw new ActiveRecordException(
							String.Format("Type `{0}` is not an ActiveRecord type. Use ActiveRecordAttributes to define one", type.FullName));
					}
				}
				
				ActiveRecordModel model = builder.Create(type);
				
				if (model == null)
				{
					throw new ActiveRecordException(
						String.Format("ActiveRecordModel for `{0}` could not be created", type.FullName));
				}
				
				registeredTypes.Add(type, String.Empty);
			}
			
			return models;
		}

		private static bool TypeDefinesADatabaseBoundary(Type type)
		{
			return (type.IsAbstract &&
			        typeof(ActiveRecordBase).IsAssignableFrom(type) &&
			        !IsTypeHierarchyBase(type));
		}

		private static bool ShouldIgnoreType(Type type)
		{
			return (registeredTypes.Contains(type) ||
			        type == typeof(ActiveRecordBase) ||
			        type == typeof(ActiveRecordValidationBase) ||
			        type == typeof(ActiveRecordHooksBase));
		}

		private static bool IsTypeHierarchyBase(Type type)
		{
			if (type.IsDefined(typeof(JoinedBaseAttribute), false))
			{
				return true;
			}
			
			object[] attrs = type.GetCustomAttributes(typeof(ActiveRecordAttribute), false);

			if (attrs != null && attrs.Length > 0)
			{
				ActiveRecordAttribute att = (ActiveRecordAttribute)attrs[0];

				return att.DiscriminatorColumn != null;
			}
			
			return false;
		}

		private static void AddXmlToNHibernateCfg(ISessionFactoryHolder holder, ActiveRecordModelCollection models)
		{
			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();

			foreach(ActiveRecordModel model in models)
			{
				Configuration cfg = holder.GetConfiguration(holder.GetRootType(model.Type));

				if (!model.IsNestedType && !model.IsDiscriminatorSubClass && !model.IsJoinedSubClass)
				{
					xmlVisitor.Reset();
					xmlVisitor.CreateXml(model);

					String xml = xmlVisitor.Xml;

					if (xml != String.Empty)
					{
						cfg.AddXmlString(xml);
					}
				}
			}
		}

		private static Type[] GetExportedTypesFromAssembly(Assembly assembly)
		{
			try
			{
				return assembly.GetExportedTypes();
			}
			catch (Exception ex)
			{
				throw new ActiveRecordInitializationException(
					"Error while loading the exported types from the assembly: " + assembly.FullName, ex);
			}
		}

		private static SchemaExport CreateSchemaExport(Configuration cfg)
		{
			return new SchemaExport(cfg);
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
			if (ActiveRecordBase.holder == null)
			{
				throw new ActiveRecordException("Framework must be Initialized first.");
			}
		}

		private static Configuration CreateConfiguration(IConfiguration config)
		{
			// hammett comments: I'm gonna test this off for a while
			NHibernate.Cfg.Environment.UseReflectionOptimizer = false;

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
				holder.Register(type, nconf);
			}
		}

		private static void RaiseSessionFactoryHolderCreated(ISessionFactoryHolder holder)
		{
			if (SessionFactoryHolderCreated != null)
			{
				SessionFactoryHolderCreated(holder);
			}
		}

		private static ISessionFactoryHolder CreateSessionFactoryHolderImplementation(IConfigurationSource source)
		{
			if (source.SessionFactoryHolderImplementation != null)
			{
				Type sessionFactoryHolderType = source.SessionFactoryHolderImplementation;

				if (!typeof(ISessionFactoryHolder).IsAssignableFrom(sessionFactoryHolderType))
				{
					String message =
						String.Format("The specified type {0} does " + "not implement the interface ISessionFactoryHolder",
						              sessionFactoryHolderType.FullName);

					throw new ActiveRecordException(message);
				}

				return (ISessionFactoryHolder) Activator.CreateInstance(sessionFactoryHolderType);
			}
			else
			{
				return new SessionFactoryHolder();
			}
		}

		private static void RegisterTypes(ISessionFactoryHolder holder, IConfigurationSource source, Type[] types, bool ignoreProblematicTypes)
		{
			lock(lockConfig)
			{
				ActiveRecordModelCollection models = BuildModels(holder, source, types, ignoreProblematicTypes);

				GraphConnectorVisitor connectorVisitor = new GraphConnectorVisitor(models);
				connectorVisitor.VisitNodes(models);

				SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(models);
				semanticVisitor.VisitNodes(models);

				AddXmlToNHibernateCfg(holder, models);
			}
		}

		private static IThreadScopeInfo CreateThreadScopeInfoImplementation(IConfigurationSource source)
		{
			if (source.ThreadScopeInfoImplementation != null)
			{
				Type threadScopeType = source.ThreadScopeInfoImplementation;

				if (!typeof(IThreadScopeInfo).IsAssignableFrom(threadScopeType))
				{
					String message =
						String.Format("The specified type {0} does " + "not implement the interface IThreadScopeInfo",
						              threadScopeType.FullName);

					throw new ActiveRecordInitializationException(message);
				}

				return (IThreadScopeInfo) Activator.CreateInstance(threadScopeType);
			}
			else
			{
				return new ThreadScopeInfo();
			}
		}

		/// <summary>
		/// Retrive all classies decorated with ActiveRecordAttribute or have been configured
		/// as a AR base class.
		/// </summary>
		/// <param name="assembly">Assembly to retrive types from</param>
		/// <param name="list">Array to store retrived types in</param>
		/// <param name="source">IConfigurationSource to inspect AR base declerations from</param>
		private static void CollectValidActiveRecordTypesFromAssembly(Assembly assembly, ArrayList list, IConfigurationSource source)
		{
			Type[] types = GetExportedTypesFromAssembly(assembly);

			Type activeRecordBaseType = typeof(ActiveRecordBase);

			foreach (Type type in types)
			{
				if (IsActiveRecordType(type) || 
				    (source.GetConfiguration(type) != null && activeRecordBaseType.IsAssignableFrom(type)))
				{
					list.Add(type);
				}
			}
		}
	}
}
