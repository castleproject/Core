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

namespace Castle.Facilities.NHibernateIntegration
{
	using System;
	using System.Configuration;
	using Builders;
	using Castle.Core.Configuration;
	using Castle.MicroKernel.Facilities;
	using Castle.MicroKernel.SubSystems.Conversion;
	using Castle.Services.Transaction;
	using Castle.Core;
	using Core.Logging;
	using Internal;
	using MicroKernel;
	using NHibernate;
	using SessionStores;

	/// <summary>
	/// Provides a basic level of integration with the NHibernate project
	/// </summary>
	/// <remarks>
	/// This facility allows components to gain access to the NHibernate's 
	/// objects:
	/// <list type="bullet">
	///   <item><description>NHibernate.Cfg.Configuration</description></item>
	///   <item><description>NHibernate.ISessionFactory</description></item>
	/// </list>
	/// <para>
	/// It also allow you to obtain the ISession instance 
	/// through the component <see cref="ISessionManager"/>, which is 
	/// transaction aware and save you the burden of sharing session
	/// or using a singleton.
	/// </para>
	/// </remarks>
	/// <example>The following sample illustrates how a component 
	/// can access the session.
	/// <code>
	/// public class MyDao
	/// {
	///   private ISessionManager sessionManager;
	/// 
	///   public MyDao(ISessionManager sessionManager)
	///   {
	///     this.sessionManager = sessionManager;
	///   } 
	///   
	///   public void Save(Data data)
	///   {
	///     using(ISession session = sessionManager.OpenSession())
	///     {
	///       session.Save(data);
	///     }
	///   }
	/// }
	/// </code>
	/// </example>
	public class NHibernateFacility : AbstractFacility
	{
		private const string DefaultConfigurationBuilderKey = "nhfacility.configuration.builder";
		private const string ConfigurationBuilderConfigurationKey = "configurationBuilder";
		private const string SessionFactoryResolverKey = "nhfacility.sessionfactory.resolver";
		private const string SessionInterceptorKey = "nhibernate.sessionfactory.interceptor";
		private const string IsWebConfigurationKey = "isWeb";
		private const string CustomStoreConfigurationKey = "customStore";
		private const string DefaultFlushModeConfigurationKey = "defaultFlushMode";
		private const string SessionManagerKey = "nhfacility.sessionmanager";
		private const string TransactionManagerKey = "nhibernate.transaction.manager";
		private const string SessionFactoryIdConfigurationKey = "id";
		private const string SessionFactoryAliasConfigurationKey = "alias";
		private const string SessionStoreKey = "nhfacility.sessionstore";
		private const string ConfigurationBuilderForFactoryFormat = "{0}.configurationBuilder";


		private readonly IConfigurationBuilder configurationBuilder;

		private ILogger log = NullLogger.Instance;

		/// <summary>
		/// Instantiates the facility with the specified configuration builder.
		/// </summary>
		/// <param name="configurationBuilder"></param>
		public NHibernateFacility(IConfigurationBuilder configurationBuilder)
		{
			this.configurationBuilder = configurationBuilder;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="NHibernateFacility"/> class.
		/// </summary>
		public NHibernateFacility() : this(new DefaultConfigurationBuilder())
		{
		}

		/// <summary>
		/// The custom initialization for the Facility.
		/// </summary>
		/// <remarks>It must be overriden.</remarks>
		protected override void Init()
		{
			if (Kernel.HasComponent(typeof(ILoggerFactory)))
			{
				log = ((ILoggerFactory)Kernel[typeof(ILoggerFactory)]).Create(GetType());
			}
			AssertHasConfig();
			AssertHasAtLeastOneFactoryConfigured();
			RegisterComponents();
			ConfigureFacility();
		}

		#region Set up of components

		/// <summary>
		/// Registers the session factory resolver, the session store, the session manager and the transaction manager.
		/// </summary>
		protected virtual void RegisterComponents()
		{
			RegisterDefaultConfigurationBuilder();
			RegisterSessionFactoryResolver();
			RegisterSessionStore();
			RegisterSessionManager();
			RegisterTransactionManager();
		}


		/// <summary>
		/// Register <see cref="IConfigurationBuilder"/> the default ConfigurationBuilder or (if present) the one 
		/// specified via "configurationBuilder" attribute.
		/// </summary>
		private void RegisterDefaultConfigurationBuilder()
		{
			string defaultConfigurationBuilder = FacilityConfig.Attributes[ConfigurationBuilderConfigurationKey];
			if (!string.IsNullOrEmpty(defaultConfigurationBuilder))
				Kernel.AddComponent(DefaultConfigurationBuilderKey, typeof(IConfigurationBuilder), Type.GetType(defaultConfigurationBuilder));
			else
				Kernel.AddComponentInstance(DefaultConfigurationBuilderKey, typeof(IConfigurationBuilder),this.configurationBuilder);
		}


		/// <summary>
		/// Registers <see cref="SessionFactoryResolver"/> as the session factory resolver.
		/// </summary>
		protected void RegisterSessionFactoryResolver()
		{
			Kernel.AddComponent(SessionFactoryResolverKey,
			                    typeof (ISessionFactoryResolver),
			                    typeof (SessionFactoryResolver));
		}

		/// <summary>
		/// Registers the configured session store.
		/// </summary>
		protected void RegisterSessionStore()
		{
			String isWeb = FacilityConfig.Attributes[IsWebConfigurationKey];
			String customStore = FacilityConfig.Attributes[CustomStoreConfigurationKey];

			// Default implementation
			Type sessionStoreType = typeof(CallContextSessionStore);

			if ("true".Equals(isWeb))
				sessionStoreType = typeof(WebSessionStore);

			if (customStore != null)
			{
				ITypeConverter converter = (ITypeConverter) 
					Kernel.GetSubSystem( SubSystemConstants.ConversionManagerKey );
				
				sessionStoreType = (Type) 
					converter.PerformConversion( customStore, typeof(Type) );

				if (!typeof(ISessionStore).IsAssignableFrom(sessionStoreType))
				{
					String message = "The specified customStore does " + 
						"not implement the interface ISessionStore. Type " + customStore;
					throw new ConfigurationErrorsException(message);
				}
			}

			Kernel.AddComponent(SessionStoreKey, 
				typeof(ISessionStore), sessionStoreType );
		}

		/// <summary>
		/// Registers <see cref="DefaultSessionManager"/> as the session manager.
		/// </summary>
		protected void RegisterSessionManager()
		{
			string defaultFlushMode = FacilityConfig.Attributes[DefaultFlushModeConfigurationKey];

			if (!string.IsNullOrEmpty(defaultFlushMode))
			{
				MutableConfiguration confignode = new MutableConfiguration(SessionManagerKey);

				IConfiguration properties = new MutableConfiguration("parameters");
				confignode.Children.Add(properties);

				properties.Children.Add(new MutableConfiguration("DefaultFlushMode", defaultFlushMode));

				Kernel.ConfigurationStore.AddComponentConfiguration(SessionManagerKey, confignode);
			}

			Kernel.AddComponent(SessionManagerKey, 
				typeof(ISessionManager), typeof(DefaultSessionManager) );
		}

		/// <summary>
		/// Registers <see cref="DefaultTransactionManager"/> as the transaction manager.
		/// </summary>
		protected void RegisterTransactionManager()
		{
			if (!Kernel.HasComponent(typeof(ITransactionManager)))
			{
				log.Info("No Transaction Manager registered on Kernel, registering default Transaction Manager");

				Kernel.AddComponent(TransactionManagerKey,
									typeof (ITransactionManager), typeof (DefaultTransactionManager));
			}
		}

		#endregion

		#region Configuration methods

		/// <summary>
		/// Configures the facility.
		/// </summary>
		protected void ConfigureFacility()
		{
			ISessionFactoryResolver sessionFactoryResolver = Kernel.Resolve <ISessionFactoryResolver>();

			ConfigureReflectionOptimizer(FacilityConfig);

			bool firstFactory = true;

			foreach(IConfiguration factoryConfig in FacilityConfig.Children)
			{
				if (!"factory".Equals(factoryConfig.Name))
				{
					String message = "Unexpected node " + factoryConfig.Name;
					throw new ConfigurationErrorsException(message);
				}

				ConfigureFactories(factoryConfig, sessionFactoryResolver, firstFactory);
				firstFactory = false;
			}
		}

		/// <summary>
		/// Reads the attribute <c>useReflectionOptimizer</c> and configure
		/// the reflection optimizer accordingly.
		/// </summary>
		/// <remarks>
		/// As reported on Jira (FACILITIES-39) the reflection optimizer
		/// slow things down. So by default it will be disabled. You
		/// can use the attribute <c>useReflectionOptimizer</c> to turn it
		/// on. 
		/// </remarks>
		/// <param name="config"></param>
		private void ConfigureReflectionOptimizer(IConfiguration config)
		{
			ITypeConverter converter = (ITypeConverter) 
				Kernel.GetSubSystem( SubSystemConstants.ConversionManagerKey );

			String useReflOptAtt = config.Attributes["useReflectionOptimizer"];

			bool useReflectionOptimizer = false;

			if (useReflOptAtt != null)
			{
				useReflectionOptimizer = (bool) 
					converter.PerformConversion(useReflOptAtt, typeof(bool));
			}

			NHibernate.Cfg.Environment.UseReflectionOptimizer = useReflectionOptimizer;
		}

		/// <summary>
		/// Configures the factories.
		/// </summary>
		/// <param name="config">The config.</param>
		/// <param name="sessionFactoryResolver">The session factory resolver.</param>
		/// <param name="firstFactory">if set to <c>true</c> [first factory].</param>
		protected void ConfigureFactories(IConfiguration config, 
			ISessionFactoryResolver sessionFactoryResolver, bool firstFactory)
		{
			String id = config.Attributes[SessionFactoryIdConfigurationKey];

			if (string.IsNullOrEmpty(id))
			{
				String message = "You must provide a " + 
					"valid 'id' attribute for the 'factory' node. This id is used as key for " + 
					"the ISessionFactory component registered on the container";

				throw new ConfigurationErrorsException(message);
			}

			String alias = config.Attributes[SessionFactoryAliasConfigurationKey];

			if (!firstFactory && (string.IsNullOrEmpty(alias)))
			{
				String message = "You must provide a " + 
					"valid 'alias' attribute for the 'factory' node. This id is used to obtain " + 
					"the ISession implementation from the SessionManager";

				throw new ConfigurationErrorsException(message);
			}
			else if (string.IsNullOrEmpty(alias))
			{
				alias = Constants.DefaultAlias;
			}
			string configurationBuilderType = config.Attributes[ConfigurationBuilderConfigurationKey];
			string configurationbuilderKey = string.Format(ConfigurationBuilderForFactoryFormat, id);
			IConfigurationBuilder configurationBuilder;
			if (string.IsNullOrEmpty(configurationBuilderType))
			{
				configurationBuilder = Kernel.Resolve<IConfigurationBuilder>();
			}
			else
			{
				Kernel.AddComponent(configurationbuilderKey,
				                    typeof (IConfigurationBuilder),
				                    Type.GetType(configurationBuilderType));
				configurationBuilder = Kernel.Resolve<IConfigurationBuilder>(configurationbuilderKey);
			}
			
			var cfg = configurationBuilder.GetConfiguration(config);

			// Registers the Configuration object
			Kernel.AddComponentInstance( String.Format("{0}.cfg", id), cfg );

			// If a Session Factory level interceptor was provided, we use it

			if (Kernel.HasComponent(SessionInterceptorKey))
			{
				cfg.Interceptor = (IInterceptor)Kernel[SessionInterceptorKey];
			}
			// Registers the ISessionFactory as a component

			var model = new ComponentModel(id, typeof(ISessionFactory), typeof(Empty));
			model.LifestyleType = LifestyleType.Singleton;
			model.ExtendedProperties[Constants.SessionFactoryConfiguration] = cfg;
			model.CustomComponentActivator = typeof (SessionFactoryActivator);
			Kernel.AddCustomComponent(model);
			sessionFactoryResolver.RegisterAliasComponentIdMapping(alias, id);
		}

		#endregion

		#region Helper methods


		private void AssertHasAtLeastOneFactoryConfigured()
		{
			IConfiguration factoriesConfig = FacilityConfig.Children["factory"];
	
			if (factoriesConfig == null)
			{
				String message = "You need to configure at least one factory to use the NHibernateFacility";

				throw new ConfigurationErrorsException(message);
			}
		}

		private void AssertHasConfig()
		{
			if (FacilityConfig == null)
			{
				String message = "The NHibernateFacility requires an external configuration";

				throw new ConfigurationErrorsException(message);
			}
		}

		#endregion
	}
}
