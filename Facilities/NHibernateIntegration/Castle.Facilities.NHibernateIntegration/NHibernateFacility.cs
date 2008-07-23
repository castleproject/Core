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
 
namespace Castle.Facilities.NHibernateIntegration
{
	using System;
	using System.Configuration;
	using Castle.Core.Configuration;
	using Castle.MicroKernel.Facilities;
	using Castle.MicroKernel.SubSystems.Conversion;
	using Castle.Services.Transaction;
	using Internal;
	using MicroKernel;
	using NHibernate;
	using Configuration=NHibernate.Cfg.Configuration;

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
		private IConfigurationBuilder configurationBuilder;

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
			RegisterConfigurationBuilder();
			RegisterSessionFactoryResolver();
			RegisterSessionStore();
			RegisterSessionManager();
			RegisterTransactionManager();
		}

		/// <summary>
		/// Register <see cref="IConfigurationBuilder"/> the default ConfigurationBuilder or (if present) the one 
		/// specified via "configurationBuilder" attribute.
		/// </summary>
		private void RegisterConfigurationBuilder()
		{
			if (!string.IsNullOrEmpty(FacilityConfig.Attributes["configurationBuilder"]))
				Kernel.AddComponent("nhfacility.configuration.builder", typeof(IConfigurationBuilder),
									Type.GetType(FacilityConfig.Attributes["configurationBuilder"]));
			else
				Kernel.AddComponentInstance("nhfacility.configuration.builder", typeof(IConfigurationBuilder),
											configurationBuilder);
		}

		/// <summary>
		/// Registers <see cref="SessionFactoryResolver"/> as the session factory resolver.
		/// </summary>
		protected void RegisterSessionFactoryResolver()
		{
			Kernel.AddComponent("nhfacility.sessionfactory.resolver",
				typeof(ISessionFactoryResolver), typeof(SessionFactoryResolver));
		}

		/// <summary>
		/// Registers the configured session store.
		/// </summary>
		protected void RegisterSessionStore()
		{
			String isWeb = FacilityConfig.Attributes["isWeb"];
			String customStore = FacilityConfig.Attributes["customStore"];

			// Default implementation
			Type sessionStoreType = typeof(CallContextSessionStore);

			if ("true".Equals(isWeb))
			{
				sessionStoreType = typeof(WebSessionStore);
			}

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

			Kernel.AddComponent( "nhfacility.sessionstore", 
				typeof(ISessionStore), sessionStoreType );
		}

		/// <summary>
		/// Registers <see cref="DefaultSessionManager"/> as the session manager.
		/// </summary>
		protected void RegisterSessionManager()
		{
			string defaultFlushMode = FacilityConfig.Attributes["defaultFlushMode"];

			if (defaultFlushMode != null && defaultFlushMode != string.Empty)
			{
				MutableConfiguration confignode = new MutableConfiguration("nhfacility.sessionmanager");

				IConfiguration properties = new MutableConfiguration("parameters");
				confignode.Children.Add(properties);

				properties.Children.Add(new MutableConfiguration("DefaultFlushMode", defaultFlushMode));

				Kernel.ConfigurationStore.AddComponentConfiguration("nhfacility.sessionmanager", confignode);
			}

			Kernel.AddComponent( "nhfacility.sessionmanager", 
				typeof(ISessionManager), typeof(DefaultSessionManager) );
		}

		/// <summary>
		/// Registers <see cref="DefaultTransactionManager"/> as the transaction manager.
		/// </summary>
		protected void RegisterTransactionManager()
		{
			Kernel.AddComponent( "nhibernate.transaction.manager",
			   typeof(ITransactionManager), typeof(DefaultTransactionManager));
		}

		#endregion

		#region Configuration methods

		/// <summary>
		/// Configures the facility.
		/// </summary>
		protected void ConfigureFacility()
		{
			ISessionFactoryResolver sessionFactoryResolver = (ISessionFactoryResolver) 
				Kernel[typeof(ISessionFactoryResolver)];

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
			String id = config.Attributes["id"];

			if (string.IsNullOrEmpty(id))
			{
				String message = "You must provide a " + 
					"valid 'id' attribute for the 'factory' node. This id is used as key for " + 
					"the ISessionFactory component registered on the container";

				throw new ConfigurationErrorsException(message);
			}

			String alias = config.Attributes["alias"];

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

			IConfigurationBuilder configurationBuilder = Kernel.Resolve<IConfigurationBuilder>();
			Configuration cfg = configurationBuilder.GetConfiguration(config);

			// Registers the Configuration object
			Kernel.AddComponentInstance( String.Format("{0}.cfg", id), cfg );

			// If a Session Factory level interceptor was provided, we use it

			if (Kernel.HasComponent("nhibernate.sessionfactory.interceptor"))
			{
				cfg.Interceptor = (IInterceptor) Kernel["nhibernate.sessionfactory.interceptor"];
			}

			// Registers the ISessionFactory as a component

			ISessionFactory sessionFactory = cfg.BuildSessionFactory();

			Kernel.AddComponentInstance( id, typeof(ISessionFactory), sessionFactory );

			// Registers the ISessionFactory within the ISessionFactoryResolver

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
