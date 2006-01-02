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

namespace Castle.Facilities.NHibernateIntegration
{
	using System;
	using System.Configuration;
	using System.IO;
	using System.Reflection;

	using NHibernate;
	using Configuration = NHibernate.Cfg.Configuration;

	using Castle.Model.Configuration;

	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;
	using Castle.MicroKernel.SubSystems.Conversion;

	using Castle.Services.Transaction;

	using Castle.Facilities.NHibernateIntegration.Internal;
	
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
	/// <
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
		protected override void Init()
		{
			AssertHasConfig();

			AssertHasAtLeastOneFactoryConfigured();

			RegisterSessionStore();

			RegisterSessionManager();

			RegisterTransactionManager();

			ConfigureFacility();
		}

		#region Set up of components

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
					throw new ConfigurationException("The specified customStore does " + 
						"not implement the interface ISessionStore. Type " + customStore);
				}
			}

			Kernel.AddComponent( "nhfacility.sessionstore", 
				typeof(ISessionStore), sessionStoreType );
		}

		protected void RegisterSessionManager()
		{
			Kernel.AddComponent( "nhfacility.sessionmanager", 
				typeof(ISessionManager), typeof(DefaultSessionManager) );
		}

		protected void RegisterTransactionManager()
		{
			Kernel.AddComponent( "nhibernate.transaction.manager", 
			   typeof(ITransactionManager), typeof(NHibernateTransactionManager) );
		}

		#endregion

		#region Configuration methods

		protected void ConfigureFacility()
		{
			ISessionManager sessionManager = (ISessionManager) Kernel[ typeof(ISessionManager) ];

			bool firstFactory = true;

			foreach(IConfiguration factoryConfig in FacilityConfig.Children)
			{
				if (!"factory".Equals(factoryConfig.Name))
				{
					throw new ConfigurationException("Unexpected node " + factoryConfig.Name);
				}

				ConfigureFactories(factoryConfig, sessionManager, firstFactory);

				firstFactory = false;
			}
		}

		private void ConfigureFactories(IConfiguration config, 
			ISessionManager sessionManager, bool firstFactory)
		{
			String id = config.Attributes["id"];

			if (id == null || String.Empty.Equals(id))
			{
				throw new ConfigurationException("You must provide a " + 
					"valid 'id' attribute for the 'factory' node. This id is used as key for " + 
					"the ISessionFactory component registered on the container");
			}

			String alias = config.Attributes["alias"];

			if (!firstFactory && (alias == null || alias.Length == 0))
			{
				throw new ConfigurationException("You must provide a " + 
					"valid 'alias' attribute for the 'factory' node. This id is used to obtain " + 
					"the ISession implementation from the SessionManager");
			}
			else if (alias == null || alias.Length == 0)
			{
				alias = Constants.DefaultAlias;
			}

			Configuration cfg = new Configuration();

			ApplyConfigurationSettings(cfg, config.Children["settings"]);
			RegisterAssemblies(cfg, config.Children["assemblies"]);
			RegisterResources(cfg, config.Children["resources"]);

			// Registers the Configuration object

			Kernel.AddComponentInstance( String.Format("{0}.cfg", id), cfg );

			// Registers the ISessionFactory as a component

			ISessionFactory sessionFactory = cfg.BuildSessionFactory();

			Kernel.AddComponentInstance( id, typeof(ISessionFactory), sessionFactory );

			// Registers the ISessionFactory within the ISessionManager

			sessionManager.RegisterSessionFactory(alias, sessionFactory);
		}

		protected void ApplyConfigurationSettings(Configuration cfg, IConfiguration facilityConfig)
		{
			if (facilityConfig == null) return;

			foreach(IConfiguration item in facilityConfig.Children)
			{
				String key = item.Attributes["key"];
				String value = item.Value;

				cfg.SetProperty(key, value);
			}
		}

		protected void RegisterResources(Configuration cfg, IConfiguration facilityConfig)
		{
			if (facilityConfig == null) return;

			foreach(IConfiguration item in facilityConfig.Children)
			{
				String name = item.Attributes["name"];
				String assembly = item.Attributes["assembly"];

				if (assembly != null)
				{
					cfg.AddResource(name, ObtainAssembly(assembly));
				}
				else
				{
					cfg.AddXmlFile( Path.Combine( AppDomain.CurrentDomain.BaseDirectory, name ) );
				}
			}
		}

		protected void RegisterAssemblies(Configuration cfg, IConfiguration facilityConfig)
		{
			if (facilityConfig == null) return;

			foreach(IConfiguration item in facilityConfig.Children)
			{
				String assembly = item.Value;

				cfg.AddAssembly(assembly);
			}
		}

		#endregion

		#region Helper methods

		private Assembly ObtainAssembly(String assembly)
		{
			try
			{
				return Assembly.Load(assembly);
			}
			catch(Exception ex)
			{
				String message = String.Format("The assembly {0} could not be loaded.", assembly);
				throw new ConfigurationException( message, ex );
			}
		}

		private void AssertHasAtLeastOneFactoryConfigured()
		{
			IConfiguration factoriesConfig = FacilityConfig.Children["factory"];
	
			if (factoriesConfig == null)
			{
				throw new ConfigurationException("You need to configure at least one factory to use the NHibernateFacility");
			}
		}

		private void AssertHasConfig()
		{
			if (FacilityConfig == null)
			{
				throw new ConfigurationException("The NHibernateFacility requires an external configuration");
			}
		}

		#endregion
	}
}