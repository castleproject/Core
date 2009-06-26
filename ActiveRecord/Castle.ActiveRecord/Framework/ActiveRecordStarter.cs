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

using Castle.ActiveRecord.Framework.Internal.EventListener;

namespace Castle.ActiveRecord
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Data;
	using System.IO;
	using System.Reflection;
	using Attributes;
	using Castle.ActiveRecord.Framework;
	using Castle.ActiveRecord.Framework.Config;
	using Castle.ActiveRecord.Framework.Internal;
	using Castle.ActiveRecord.Framework.Scopes;
	using Castle.Core.Configuration;
	using NHibernate.Cfg;
	using Iesi.Collections;
	using Iesi.Collections.Generic;
	using NHibernate.Criterion;
	using NHibernate.Tool.hbm2ddl;
	using Environment=NHibernate.Cfg.Environment;

	/// <summary>
	/// Delegate for use in <see cref="ActiveRecordStarter.ModelsCreated"/> and <see cref="ActiveRecordStarter.ModelsValidated"/>
	/// </summary>
	/// <param name="holder"></param>
	public delegate void SessionFactoryHolderDelegate(ISessionFactoryHolder holder);

	/// <summary>
	/// Delegate for use in <see cref="ActiveRecordStarter.ModelsCreated"/> and <see cref="ActiveRecordStarter.ModelsValidated"/>
	/// </summary>
	public delegate void ModelsDelegate(ActiveRecordModelCollection models, IConfigurationSource source);

	/// <summary>
	/// Delegate for use in <see cref="ActiveRecordStarter.ModelCreated"/>
	/// </summary>
	public delegate void ModelDelegate(ActiveRecordModel model, IConfigurationSource source);

	/// <summary>
	/// Delegate for AR Facility registration hooks.
	/// </summary>
	/// <param name="contributor"></param>
	public delegate void EventListenerRegistrationDelegate(EventListenerContributor contributor);

	/// <summary>
	/// Performs the framework initialization.
	/// </summary>
	/// <remarks>
	/// This class is not thread safe.
	/// </remarks>
	public static class ActiveRecordStarter
	{
		private static readonly Object lockConfig = new object();

		private static bool isInitialized = false;

		private static readonly List<IModelBuilderExtension> extensions = new List<IModelBuilderExtension>();
		private static readonly ISet<Assembly> registeredAssemblies = new HashedSet<Assembly>();
		private static IDictionary<Type, string> registeredTypes;

		/// <summary>
		/// This is saved so one can invoke <c>RegisterTypes</c> later
		/// </summary>
		private static IConfigurationSource configSource;

		/// <summary>
		/// The schema delimiter that is used by the hbm2ddl tool.
		/// Change the delimiter by calling <see cref="SetSchemaDelimiter"/>.
		/// </summary>
		private static string schemaDelimiter = null;

		/// <summary>
		/// The default schema delimiter. The delimiter of the schema is only set if 
		/// <see cref="schemaDelimiter"/> is different from this default value.
		/// The default should be the same as the default delimiter of the hbm2ddl tool.
		/// </summary>
		private static readonly string defaultSchemaDelimiter = null;

		/// <summary>
		/// So others frameworks can intercept the 
		/// creation and act on the holder instance
		/// </summary>
		public static event SessionFactoryHolderDelegate SessionFactoryHolderCreated;

		/// <summary>
		/// So others frameworks can intercept the 
		/// creation and act on the holder instance after
		/// the mapping was already loaded into the NHibernate 
		/// </summary>
		public static event SessionFactoryHolderDelegate MappingRegisteredInConfiguration;


		/// <summary>
		/// Allows other frameworks to modify the ActiveRecordModel
		/// before the generation of the NHibernate XML configuration.
		/// As an example, this may be used to rewrite table names to
		/// conform to an application-specific standard.  Since the
		/// configuration source is passed in, it is possible to
		/// determine the underlying database type and make changes
		/// if necessary.
		/// </summary>
		public static event ModelsDelegate ModelsCreated;

		/// <summary>
		/// Allows other frameworks to modify the ActiveRecordModel
		/// before the generation of the NHibernate XML configuration.
		/// As an example, this may be used to rewrite table names to
		/// conform to an application-specific standard.  Since the
		/// configuration source is passed in, it is possible to
		/// determine the underlying database type and make changes
		/// if necessary.
		/// </summary>
		public static event ModelsDelegate ModelsValidated;

		/// <summary>
		/// Allows other frameworks to modify the ActiveRecordModel
		/// before the generation of the NHibernate XML configuration.
		/// </summary>
		public static event ModelDelegate ModelCreated;

		/// <summary>
		/// Allows the ActiveRecordFacility to register components as event listeners;
		/// </summary>
		public static event EventListenerRegistrationDelegate EventListenerComponentRegistrationHook;

		/// <summary>
		/// Allows the ActiveRecordFacility to reconfigure registered listeners.
		/// </summary>
		public static event EventListenerRegistrationDelegate EventListenerFacilityConfigurationHook;

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
				
				foreach (Type type in types)
				{
					registeredAssemblies.Add(type.Assembly);
				}

				if (source == null)
				{
					throw new ArgumentNullException("source");
				}
				if (types == null)
				{
					throw new ArgumentNullException("types");
				}

				registeredTypes = new Dictionary<Type, string>();

				configSource = source;

				// First initialization
				ISessionFactoryHolder holder = CreateSessionFactoryHolderImplementation(source);

				holder.ThreadScopeInfo = CreateThreadScopeInfoImplementation(source);

				RaiseSessionFactoryHolderCreated(holder);

				ActiveRecordBase.holder = holder;
				ActiveRecordModel.type2Model.Clear();
				ActiveRecordModel.isDebug = source.Debug;
				ActiveRecordModel.isLazyByDefault = source.IsLazyByDefault;
				ActiveRecordModel.pluralizeTableNames = source.PluralizeTableNames;

				RegisterEventListeners(types);

				if (configSource.Searchable)
				{
					contributors.Add(new NHSearchContributor());
				}

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
			Initialize(new Assembly[] {assembly}, source);
		}

		/// <summary>
		/// Initialize the mappings using the configuration and 
		/// checking all the types on the specified Assemblies
		/// </summary>
		public static void Initialize(Assembly[] assemblies, IConfigurationSource source, params Type[] additionalTypes)
		{
			List<Type> list = new List<Type>(additionalTypes);

			foreach(Assembly assembly in assemblies)
			{
				CollectValidActiveRecordTypesFromAssembly(assembly, list, source);
			}

			Initialize(source, list.ToArray());
		}

		/// <summary>
		/// Initializes the framework reading the configuration from
		/// the <c>AppDomain</c> and checking all the types on the executing <c>Assembly</c>
		/// </summary>
		public static void Initialize()
		{
			IConfigurationSource source = ActiveRecordSectionHandler.Instance;

			Initialize(Assembly.GetCallingAssembly(), source);
		}

		/// <summary>
		/// Registers new assemblies in ActiveRecord
		/// Usefull for dynamic assembly-adding after initialization
		/// </summary>
		/// <param name="assemblies"></param>
		public static void RegisterAssemblies(params Assembly[] assemblies)
		{
			List<Type> types = new List<Type>();

			foreach(Assembly assembly in assemblies)
			{
				CollectValidActiveRecordTypesFromAssembly(assembly, types, configSource);
			}

			RegisterTypes(types.ToArray());
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
				catch(Exception ex)
				{
					throw new ActiveRecordException("Could not create the schema", ex);
				}
			}
		}

		/// <summary>
		/// Generates and executes the creation scripts for the database using 
		/// the specified baseClass to know which database it should create the schema for.
		/// </summary>
		public static void CreateSchema(Type baseClass)
		{
			CheckInitialized();

			Configuration config = ActiveRecordBase.holder.GetConfiguration(baseClass);

			SchemaExport export = CreateSchemaExport(config);

			try
			{
				export.Create(false, true);
			}
			catch(Exception ex)
			{
				throw new ActiveRecordException("Could not create the schema", ex);
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
				catch(Exception ex)
				{
					throw new ActiveRecordException("Could not drop the schema", ex);
				}
			}
		}

		/// <summary>
		/// Generates and executes the Drop scripts for the database using 
		/// the specified baseClass to know which database it should create the scripts for.
		/// </summary>
		public static void DropSchema(Type baseClass)
		{
			CheckInitialized();

			Configuration config = ActiveRecordBase.holder.GetConfiguration(baseClass);

			SchemaExport export = CreateSchemaExport(config);

			try
			{
				export.Drop(false, true);
			}
			catch(Exception ex)
			{
				throw new ActiveRecordException("Could not drop the schema", ex);
			}
		}

		/// <summary>
		/// Generates and executes the creation scripts for the database.
		/// </summary>
		/// <returns>List of exceptions that occurred during the update process</returns>
		public static IList UpdateSchema()
		{
			CheckInitialized();
			ArrayList exceptions = new ArrayList();

			foreach(Configuration config in ActiveRecordBase.holder.GetAllConfigurations())
			{
				SchemaUpdate updater = CreateSchemaUpdate(config);

				try
				{
					updater.Execute(false, true);

					exceptions.AddRange((IList) updater.Exceptions);
				}
				catch(Exception ex)
				{
					throw new ActiveRecordException("Could not update the schema", ex);
				}
			}

			return exceptions;
		}

		/// <summary>
		/// Generates and executes the creation scripts for the database using 
		/// the specified baseClass to know which database it should create the schema for.
		/// </summary>
		public static IList UpdateSchema(Type baseClass)
		{
			CheckInitialized();

			Configuration config = ActiveRecordBase.holder.GetConfiguration(baseClass);

			SchemaUpdate updater = CreateSchemaUpdate(config);

			try
			{
				updater.Execute(false, true);
			}
			catch(Exception ex)
			{
				throw new ActiveRecordException("Could not update the schema", ex);
			}

			return (IList) updater.Exceptions;
		}

		/// <summary>
		/// Generates the drop scripts for the database saving them to the supplied file name. 
		/// </summary>
		/// <remarks>
		/// If ActiveRecord was configured to access more than one database, a file is going
		/// to be generate for each, based on the path and the <c>fileName</c> specified.
		/// </remarks>
		public static void GenerateDropScripts(String fileName)
		{
			CheckInitialized();

			bool isFirstExport = true;
			int fileCount = 1;

			foreach(Configuration config in ActiveRecordBase.holder.GetAllConfigurations())
			{
				SchemaExport export = CreateSchemaExport(config);

				try
				{
					export.SetOutputFile(isFirstExport ? fileName : CreateAnotherFile(fileName, fileCount++));
					export.Drop(false, false);
				}
				catch(Exception ex)
				{
					throw new ActiveRecordException("Could not drop the schema", ex);
				}

				isFirstExport = false;
			}
		}

		/// <summary>
		/// Generates the drop scripts for the database saving them to the supplied file name. 
		/// The baseType is used to identify which database should we act upon.
		/// </summary>
		public static void GenerateDropScripts(Type baseType, String fileName)
		{
			CheckInitialized();

			Configuration config = ActiveRecordBase.holder.GetConfiguration(baseType);

			SchemaExport export = CreateSchemaExport(config);

			try
			{
				export.SetOutputFile(fileName);
				export.Drop(false, false);
			}
			catch(Exception ex)
			{
				throw new ActiveRecordException("Could not generate drop schema scripts", ex);
			}
		}

		/// <summary>
		/// Generates the creation scripts for the database
		/// </summary>
		/// <remarks>
		/// If ActiveRecord was configured to access more than one database, a file is going
		/// to be generate for each, based on the path and the <c>fileName</c> specified.
		/// </remarks>
		public static void GenerateCreationScripts(String fileName)
		{
			CheckInitialized();

			bool isFirstExport = true;
			int fileCount = 1;

			foreach(Configuration config in ActiveRecordBase.holder.GetAllConfigurations())
			{
				SchemaExport export = CreateSchemaExport(config);

				try
				{
					export.SetOutputFile(isFirstExport ? fileName : CreateAnotherFile(fileName, fileCount++));
					export.Create(false, false);
				}
				catch(Exception ex)
				{
					throw new ActiveRecordException("Could not create the schema", ex);
				}

				isFirstExport = false;
			}
		}

		/// <summary>
		/// Generates the creation scripts for the database
		/// The baseType is used to identify which database should we act upon.
		/// </summary>
		public static void GenerateCreationScripts(Type baseType, String fileName)
		{
			CheckInitialized();

			Configuration config = ActiveRecordBase.holder.GetConfiguration(baseType);

			SchemaExport export = CreateSchemaExport(config);

			try
			{
				export.SetOutputFile(fileName);
				export.Create(false, false);
			}
			catch(Exception ex)
			{
				throw new ActiveRecordException("Could not create the schema scripts", ex);
			}
		}

		/// <summary>
		/// Intended to be used only by test cases
		/// </summary>
		public static void ResetInitializationFlag()
		{
			// Make sure we start with it enabled
			Environment.UseReflectionOptimizer = true;
			isInitialized = false;
			registeredAssemblies.Clear();
		}

		/// <summary>
		/// Sets the schema delimiter that is used for the creation of schema scripts.
		/// For example, <see cref="CreateSchema()"/>, <see cref="DropSchema()"/>, 
		/// <see cref="GenerateCreationScripts(string)"/> and <see cref="GenerateDropScripts(string)"/> 
		/// use the delimiter in the schema they create.
		/// </summary>
		/// <param name="newDelimiter">The new schema delimiter.</param>
		public static void SetSchemaDelimiter(string newDelimiter)
		{
			schemaDelimiter = newDelimiter;
		}

		/// <summary>
		/// Gets a value indicating whether ActiveRecord was initialized properly (see the Initialize method).
		/// </summary>
		/// <value>
		/// 	<c>true</c> if it is initialized; otherwise, <c>false</c>.
		/// </value>
		public static bool IsInitialized
		{
			get { return isInitialized; }
		}

		/// <summary>
		/// The current <see cref="IConfigurationSource"/>.
		/// </summary>
		public static IConfigurationSource ConfigurationSource
		{
			get { return configSource; }
		}

		/// <summary>
		/// Retrieves a copy of the types registered within ActiveRecord
		/// </summary>
		/// <returns></returns>
		public static Type[] GetRegisteredTypes()
		{
			return new List<Type>(registeredTypes.Keys).ToArray();
		}

		/// <summary>
		/// Registers a builder extension.
		/// </summary>
		/// <param name="extension">The extension.</param>
		public static void RegisterExtension(IModelBuilderExtension extension)
		{
			if (extension == null)
			{
				throw new ArgumentNullException("extension");
			}
			extensions.Add(extension);
		}

		private static ActiveRecordModelCollection BuildModels(ISessionFactoryHolder holder,
		                                                       IConfigurationSource source,
		                                                       IEnumerable<Type> types, bool ignoreProblematicTypes)
		{
			ActiveRecordModelBuilder builder = new ActiveRecordModelBuilder();

			builder.SetExtension(new ModelBuilderExtensionComposite(extensions));

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
				else if (IsConfiguredAsRootType(type))
				{
					if (TypeDefinesADatabaseBoundary(type))
					{
						SetUpConfiguration(source, type, holder);
						continue;
					}
					else
					{
						throw new ActiveRecordException(
							string.Format(
								"Type `{0}` is not a valid root type.  Make sure it is abstract and does not define a table itself.",
								type.FullName));
					}
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

				if (type.ContainsGenericParameters)
				{
					// Owing to a restriction in NHibernate the reflection optimiser will not work
					// if we have generic types so turn it off. Do we have anywhere we could log this?
					Environment.UseReflectionOptimizer = false;
				}

				ActiveRecordModel model = builder.Create(type);

				if (model == null)
				{
					throw new ActiveRecordException(
						String.Format("ActiveRecordModel for `{0}` could not be created", type.FullName));
				}

				registeredTypes.Add(type, String.Empty);

				if (ModelCreated != null)
				{
					ModelCreated(model, source);
				}
			}

			return models;
		}

		private static bool IsConfiguredAsRootType(Type type)
		{
			return configSource.GetConfiguration(type) != null;
		}

		private static bool TypeDefinesADatabaseBoundary(Type type)
		{
			return (type.IsAbstract && !IsTypeHierarchyBase(type));
		}

		private static bool ShouldIgnoreType(Type type)
		{
			return (registeredTypes.ContainsKey(type) ||
			        type == typeof(ActiveRecordBase) ||
			        type == typeof(ActiveRecordValidationBase) ||
			        type == typeof(ActiveRecordHooksBase));
		}

		private static bool IsTypeHierarchyBase(ICustomAttributeProvider type)
		{
			if (type.IsDefined(typeof(JoinedBaseAttribute), false))
			{
				return true;
			}

			object[] attrs = type.GetCustomAttributes(typeof(ActiveRecordAttribute), false);

			if (attrs != null && attrs.Length > 0)
			{
				ActiveRecordAttribute att = (ActiveRecordAttribute) attrs[0];

				return att.DiscriminatorColumn != null;
			}

			return false;
		}

		private static void AddXmlToNHibernateCfg(ISessionFactoryHolder holder, ActiveRecordModelCollection models)
		{
			XmlGenerationVisitor xmlVisitor = new XmlGenerationVisitor();
			AssemblyXmlGenerator assemblyXmlGenerator = new AssemblyXmlGenerator();
			ISet assembliesGeneratedXmlFor = new HashedSet();
			foreach(ActiveRecordModel model in models)
			{
				Configuration config = holder.GetConfiguration(holder.GetRootType(model.Type));

				if (config == null)
				{
					throw new ActiveRecordException(
						string.Format(
							"Could not find configuration for {0} or its root type {1} this is usually an indication that the configuration has not been setup correctly.",
							model.Type, holder.GetRootType(model.Type)));
				}

				if (!model.IsNestedType && !model.IsDiscriminatorSubClass && !model.IsJoinedSubClass)
				{
					xmlVisitor.Reset();
					xmlVisitor.CreateXml(model);

					String xml = xmlVisitor.Xml;

					if (xml != String.Empty)
					{
						AddXmlString(config, xml, model);
					}
				}
			}
		}

		private static void AddXmlString(Configuration config, string xml, ActiveRecordModel model)
		{
			try
			{
				config.AddXmlString(xml);
			}
			catch(Exception ex)
			{
				throw new ActiveRecordException(
					"Error adding information from class " + model.Type.FullName +
					" to NHibernate. Check the inner exception for more information", ex);
			}
		}

		private static Type[] GetExportedTypesFromAssembly(Assembly assembly)
		{
			try
			{
				return assembly.GetExportedTypes();
			}
			catch(Exception ex)
			{
				throw new ActiveRecordInitializationException(
					"Error while loading the exported types from the assembly: " + assembly.FullName, ex);
			}
		}

		private static SchemaExport CreateSchemaExport(Configuration cfg)
		{
			SchemaExport export = new SchemaExport(cfg);

			// set the delimiter, but only if it is not the default delimiter
			if (schemaDelimiter != defaultSchemaDelimiter)
			{
				export.SetDelimiter(schemaDelimiter);
			}

			return export;
		}

		private static SchemaUpdate CreateSchemaUpdate(Configuration cfg)
		{
			return new SchemaUpdate(cfg);
		}

		/// <summary>
		/// Return true if the type has a [ActiveRecord] attribute
		/// </summary>
		private static bool IsActiveRecordType(ICustomAttributeProvider type)
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
			// ayende comments:	removing this means that tests that using generic entities as base class will break
			//					NH do not support generic entities at the moemnt
			// daveg comments: I have added a test in BuildModels to only turn it off if we find a generic entity
			//Environment.UseReflectionOptimizer = false;

			Configuration cfg = new Configuration();

			foreach(IConfiguration childConfig in config.Children)
			{
				cfg.Properties[childConfig.Name] = childConfig.Value;
			}

			return cfg;
		}

		private static void SetUpConfiguration(IConfigurationSource source, Type type, ISessionFactoryHolder holder)
		{
			IConfiguration config = source.GetConfiguration(type);

			if (config != null)
			{
				Configuration nconf = CreateConfiguration(config);

				if (source.NamingStrategyImplementation != null)
				{
					Type namingStrategyType = source.NamingStrategyImplementation;

					if (!typeof(INamingStrategy).IsAssignableFrom(namingStrategyType))
					{
						String message =
							String.Format("The specified type {0} does " + "not implement the interface INamingStrategy",
							              namingStrategyType.FullName);

						throw new ActiveRecordException(message);
					}

					nconf.SetNamingStrategy((INamingStrategy) Activator.CreateInstance(namingStrategyType));
				}

				AddContributorsToConfig(type, nconf);
				holder.Register(type, nconf);
			}
		}

		private static void AddContributorsToConfig(Type type, Configuration nconf)
		{
			lock (contributors)
			{
				foreach (var contributor in contributors)
				{
					if (contributor.AppliesToRootType(type))
						contributor.Contribute(nconf);
				}
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

		private static void RegisterTypes(ISessionFactoryHolder holder, IConfigurationSource source, IEnumerable<Type> types,
		                                  bool ignoreProblematicTypes)
		{
			lock(lockConfig)
			{
				ActiveRecordModelCollection models = BuildModels(holder, source, types, ignoreProblematicTypes);

				GraphConnectorVisitor connectorVisitor = new GraphConnectorVisitor(models);
				connectorVisitor.VisitNodes(models);

				ModelsDelegate modelsCreatedHandler = ModelsCreated;
				if (modelsCreatedHandler != null)
				{
					modelsCreatedHandler(models, source);
				}

				SemanticVerifierVisitor semanticVisitor = new SemanticVerifierVisitor(models);
				semanticVisitor.VisitNodes(models);

				ModelsDelegate modelsValidatedHandler = ModelsValidated;
				if (modelsValidatedHandler != null)
				{
					modelsValidatedHandler(models, source);
				}

				AddXmlToNHibernateCfg(holder, models);

				AddXmlToNHibernateFromAssmebliesAttributes(holder, models);

				SessionFactoryHolderDelegate registeredInConfigurationHandler = MappingRegisteredInConfiguration;
				if(registeredInConfigurationHandler!=null)
				{
					registeredInConfigurationHandler(holder);
				}

				if (source.VerifyModelsAgainstDBSchema)
				{
					VerifySchema(models);
				}
			}
		}

		private static void RegisterEventListeners(IEnumerable<Type> types)
		{
			var contributor = new EventListenerContributor();
			foreach (var type in types)
			{
				var eventListenerAttributes = type.GetCustomAttributes(typeof(EventListenerAttribute), false);
				if (eventListenerAttributes.Length == 1)
				{
					var attribute = (EventListenerAttribute) eventListenerAttributes[0];
					var config = new EventListenerConfig(type)
					             	{
					             		ReplaceExisting = attribute.ReplaceExisting,
										Ignore=attribute.Ignore,
										SkipEvent=attribute.SkipEvent,
										Singleton = attribute.Singleton,
										Include = attribute.Include,
										Exclude = attribute.Exclude
					             	};

					contributor.Add(config);
				}
			}

			if (EventListenerComponentRegistrationHook != null)
			{
				EventListenerComponentRegistrationHook(contributor);
			}

			var addEventListenerAttributes = new List<EventListenerAssemblyAttribute>();
			var ignoreEventListenerAttributes = new List<EventListenerAssemblyAttribute>();
			foreach (var assembly in registeredAssemblies)
			{
				addEventListenerAttributes.AddRange(
					(AddEventListenerAttribute[]) assembly.GetCustomAttributes(typeof (AddEventListenerAttribute), false));
				ignoreEventListenerAttributes.AddRange((IgnoreEventListenerAttribute[]) assembly.GetCustomAttributes(typeof(IgnoreEventListenerAttribute), false));
			}

			ProcessEventListenerAssemblyAttributes(contributor, ignoreEventListenerAttributes);
			ProcessEventListenerAssemblyAttributes(contributor, addEventListenerAttributes);

			if (EventListenerFacilityConfigurationHook != null)
			{
				EventListenerFacilityConfigurationHook(contributor);
			}

			AddContributor(contributor);
		}

		private static void ProcessEventListenerAssemblyAttributes(EventListenerContributor contributor, IEnumerable<EventListenerAssemblyAttribute> attributes)
		{
			foreach (var attribute in attributes)
			{
				if (attribute.Assembly != null)
				{
					foreach (var type in GetExportedTypesFromAssembly(attribute.Assembly))
					{
						if (EventListenerContributor.GetEventTypes(type).Length > 0)
						{
							var config = contributor.Get(type) ?? contributor.Add(new EventListenerConfig(type));
							ConfigureEventListener(attribute, config);
						}
					}
				}
				if (attribute.Type != null)
				{
					var config = contributor.Get(attribute.Type) ?? contributor.Add(new EventListenerConfig(attribute.Type));
					ConfigureEventListener(attribute, config);
				}
			}
		}

		private static void ConfigureEventListener(EventListenerAssemblyAttribute attribute, EventListenerConfig config)
		{
			if (attribute is IgnoreEventListenerAttribute)
				config.Ignore = true;
			else
			{
				config.Ignore = false;
				var addAttribute = (AddEventListenerAttribute) attribute;
				config.Exclude = addAttribute.Exclude;
				config.Include = addAttribute.Include;
				config.SkipEvent = addAttribute.ExcludeEvent;
				if (addAttribute.IncludeEvent != null)
				{
					Type[] eventtypes = EventListenerContributor.GetEventTypes(config.ListenerType);
					config.SkipEvent = Array.FindAll(eventtypes, type => Array.IndexOf(addAttribute.IncludeEvent, type) < 0);
				}
				config.Singleton = addAttribute.Singleton;
				config.ReplaceExisting = addAttribute.ReplaceExisting;
			}
		}


		private static void AddXmlToNHibernateFromAssmebliesAttributes(ISessionFactoryHolder holder, ActiveRecordModelCollection models)
		{
			ISet<Assembly> assembliesGeneratedXmlFor = new HashedSet<Assembly>();
			AssemblyXmlGenerator assemblyXmlGenerator = new AssemblyXmlGenerator();

			foreach (ActiveRecordModel model in models)
			{
				if (assembliesGeneratedXmlFor.Contains(model.Type.Assembly)) 
					continue;

				assembliesGeneratedXmlFor.Add(model.Type.Assembly);

				Configuration config = holder.GetConfiguration(holder.GetRootType(model.Type));
					
				string[] configurations = assemblyXmlGenerator.CreateXmlConfigurations(model.Type.Assembly);

				foreach (string xml in configurations)
				{
					if (xml != string.Empty)
					{
						config.AddXmlString(xml);
					}
				}
			}

			foreach (Assembly assembly in registeredAssemblies)
			{
				if (assembliesGeneratedXmlFor.Contains(assembly))
					continue;

				assembliesGeneratedXmlFor.Add(assembly);

				Configuration config = holder.GetConfiguration(holder.GetRootType(typeof(ActiveRecordBase)));

				string[] configurations = assemblyXmlGenerator.CreateXmlConfigurations(assembly);

				foreach (string xml in configurations)
				{
					if (xml != string.Empty)
					{
						config.AddXmlString(xml);
					}
				}
			}
		}

		private static void VerifySchema(ActiveRecordModelCollection models)
		{
			foreach(ActiveRecordModel model in models)
			{
				if (!model.Type.IsAbstract)
				{
					try
					{
						ActiveRecordMediator.FindAll(model.Type, Expression.Sql("1=0"));
					}
					catch(Exception ex)
					{
						throw new ActiveRecordException("Error verifying the schema for model " + model.Type.Name, ex);
					}
				}
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
		/// Retrieve all classes decorated with ActiveRecordAttribute or that have been configured
		/// as a AR base class.
		/// </summary>
		/// <param name="assembly">Assembly to retrieve types from</param>
		/// <param name="list">Array to store retrieved types in</param>
		/// <param name="source">IConfigurationSource to inspect AR base declarations from</param>
		private static void CollectValidActiveRecordTypesFromAssembly(Assembly assembly, ICollection<Type> list,
		                                                              IConfigurationSource source)
		{
			registeredAssemblies.Add(assembly);
			Type[] types = GetExportedTypesFromAssembly(assembly);

			foreach(Type type in types)
			{
				if (IsActiveRecordType(type) || IsEventListener(type) || source.GetConfiguration(type) != null)
				{
					list.Add(type);
				}
			}
		}

		private static bool IsEventListener(Type type)
		{
			return type.IsDefined(typeof (EventListenerAttribute), false);
		}

		/// <summary>
		/// Generate a file name based on the original file name specified, using the 
		/// count to give it some order.
		/// </summary>
		/// <param name="originalFileName"></param>
		/// <param name="fileCount"></param>
		/// <returns></returns>
		private static string CreateAnotherFile(string originalFileName, int fileCount)
		{
			string path = Path.GetDirectoryName(originalFileName);
			string fileName = Path.GetFileNameWithoutExtension(originalFileName);
			string extension = Path.GetExtension(originalFileName);

			return Path.Combine(path, string.Format("{0}_{1}{2}", fileName, fileCount, extension));
		}

		/// <summary>
		/// Adds a contributor instance that will be called when a configuration is
		/// prepared for creating a session factory 
		/// </summary>
		/// <param name="contributor">The contributor to add.</param>
		public static void AddContributor(INHContributor contributor)
		{
			lock (contributors)
			{
				contributors.Add(contributor);
			}
		}

		/// <summary>
		/// Clears the contributor registry. Mainly used for tests.
		/// </summary>
		public static void ClearContributors()
		{
			contributors.Clear();
		}

		private static List<INHContributor> contributors = new List<INHContributor>();
	}
}
