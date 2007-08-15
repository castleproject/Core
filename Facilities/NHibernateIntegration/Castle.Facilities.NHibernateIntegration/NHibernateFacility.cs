// Copyright 2004-2007 Castle Project - http://www.castleproject.org/
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

	using Castle.Core.Configuration;

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
		private const String nHMappingAttributesAssemblyName = "NHibernate.Mapping.Attributes";
		
		protected override void Init()
		{
			AssertHasConfig();

			AssertHasAtLeastOneFactoryConfigured();

			RegisterComponents();

			ConfigureFacility();
		}

		#region Set up of components

		protected virtual void RegisterComponents()
		{
			RegisterSessionFactoryResolver();
			RegisterSessionStore();
			RegisterSessionManager();
			RegisterTransactionManager();
		}

		protected void RegisterSessionFactoryResolver()
		{
			Kernel.AddComponent( "nhfacility.sessionfactory.resolver", 
				typeof(ISessionFactoryResolver), typeof(SessionFactoryResolver) );
		}

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

		protected void RegisterSessionManager()
		{
			string defaultFlushMode = FacilityConfig.Attributes["defaultFlushMode"];

			if (defaultFlushMode != null && defaultFlushMode != string.Empty)
			{
				MutableConfiguration confignode = new MutableConfiguration("nhfacility.sessionmanager");

				IConfiguration properties =
					confignode.Children.Add(new MutableConfiguration("parameters"));

				properties.Children.Add(new MutableConfiguration("DefaultFlushMode", defaultFlushMode));

				Kernel.ConfigurationStore.AddComponentConfiguration("nhfacility.sessionmanager", confignode);
			}

			Kernel.AddComponent( "nhfacility.sessionmanager", 
				typeof(ISessionManager), typeof(DefaultSessionManager) );
		}

		protected void RegisterTransactionManager()
		{
			Kernel.AddComponent( "nhibernate.transaction.manager",
			   typeof(ITransactionManager), typeof(DefaultTransactionManager));
		}

		#endregion

		#region Configuration methods

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

		private void ConfigureFactories(IConfiguration config, 
			ISessionFactoryResolver sessionFactoryResolver, bool firstFactory)
		{
			String id = config.Attributes["id"];

			if (id == null || String.Empty.Equals(id))
			{
				String message = "You must provide a " + 
					"valid 'id' attribute for the 'factory' node. This id is used as key for " + 
					"the ISessionFactory component registered on the container";

				throw new ConfigurationErrorsException(message);
			}

			String alias = config.Attributes["alias"];

			if (!firstFactory && (alias == null || alias.Length == 0))
			{
				String message = "You must provide a " + 
					"valid 'alias' attribute for the 'factory' node. This id is used to obtain " + 
					"the ISession implementation from the SessionManager";

				throw new ConfigurationErrorsException(message);
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

		/// <summary>
		/// If <paramref name="targetAssembly"/> has a reference on
		/// <c>NHibernate.Mapping.Attributes</c> : use the NHibernate mapping
		/// attributes contained in that assembly to update NHibernate
		/// configuration (<paramref name="cfg"/>). Else do nothing
		/// </summary>
		/// <remarks>
		/// To avoid an unnecessary dependency on the library
		/// <c>NHibernate.Mapping.Attributes.dll</c> when using this
		/// facility without NHibernate mapping attributes, all calls to that
		/// library are made using reflexion.
		/// </remarks>
		/// <param name="cfg">NHibernate configuration</param>
		/// <param name="targetAssembly">Target assembly name</param>
		protected void GenerateMappingFromAttributesIfNeeded(Configuration cfg, String targetAssembly)
		{
			//Get an array of all assemblies referenced by targetAssembly
			AssemblyName[] refAssemblies = Assembly.Load(targetAssembly).GetReferencedAssemblies();

			//If assembly "NHibernate.Mapping.Attributes" is referenced in targetAssembly
			if (Array.Exists<AssemblyName>(refAssemblies, delegate(AssemblyName an) { return an.Name.Equals(nHMappingAttributesAssemblyName); }))
			{
			//Obtains, by reflexion, the necessary tools to generate NH mapping from attributes
			Type HbmSerializerType = Type.GetType(String.Concat(nHMappingAttributesAssemblyName, ".HbmSerializer, ", nHMappingAttributesAssemblyName));
			Object hbmSerializer = Activator.CreateInstance(HbmSerializerType);
			PropertyInfo validate = HbmSerializerType.GetProperty("Validate");
			MethodInfo serialize = HbmSerializerType.GetMethod("Serialize", new Type[] { typeof(Assembly) });

			//Enable validation of mapping documents generated from the mapping attributes
			validate.SetValue(hbmSerializer, true, null);

			//Generates a stream of mapping documents from all decorated classes in targetAssembly and add it to NH config
			cfg.AddInputStream((MemoryStream)serialize.Invoke(hbmSerializer, new object[] { Assembly.Load(targetAssembly) }));
			}
		}

		protected void RegisterAssemblies(Configuration cfg, IConfiguration facilityConfig)
		{
			if (facilityConfig == null) return;

			foreach(IConfiguration item in facilityConfig.Children)
			{
				String assembly = item.Value;

				cfg.AddAssembly(assembly);

				GenerateMappingFromAttributesIfNeeded(cfg, assembly);
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

				throw new ConfigurationErrorsException(message, ex);
			}
		}

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
