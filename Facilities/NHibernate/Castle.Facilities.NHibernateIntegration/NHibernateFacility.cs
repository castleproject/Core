// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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
	using System.IO;
	using System.Reflection;
	using System.Configuration;

	using Castle.Model;
	using Castle.Model.Configuration;

	using Castle.MicroKernel.Facilities;

	using Castle.Services.Transaction;

	using NHibernate;
	using NHibernate.Cfg;


	/// <summary>
	/// Enable components to take advantage of the capabilities 
	/// offered by the NHibernate project.
	/// </summary>
	public class NHibernateFacility : AbstractFacility
	{
		public static readonly String ConfiguredObject = "nhibernate.cfg";

		public NHibernateFacility()
		{
		}

		protected override void Init()
		{
			if (FacilityConfig == null)
			{
				throw new ConfigurationException(
					"The NHibernateFacility requires an external configuration");
			}

			IConfiguration factoriesConfig = FacilityConfig.Children["factory"];

			if (factoriesConfig == null)
			{
				throw new ConfigurationException(
					"You need to configure at least one factory to use the NHibernateFacility");
			}

			Kernel.ComponentModelBuilder.AddContributor( new AutomaticSessionInspector() );

			Kernel.AddComponent( "nhibernate.session.interceptor", typeof(AutomaticSessionInterceptor) );

			Kernel.AddComponent( "nhibernate.transaction.manager", typeof(ITransactionManager), typeof(NHibernateTransactionManager) );

			foreach(IConfiguration factoryConfig in FacilityConfig.Children)
			{
				if (!"factory".Equals(factoryConfig.Name))
				{
					throw new ConfigurationException("Unexpected node " + factoryConfig.Name);
				}

				ConfigureFactories(factoryConfig);
			}
		}

		private void ConfigureFactories(IConfiguration config)
		{
			String id = config.Attributes["id"];

			if (id == null || String.Empty.Equals(id))
			{
				throw new ConfigurationException("You must provide a valid 'id' attribute for the 'factory' node");
			}

			Configuration cfg = new Configuration();

			ApplyConfigurationSettings(cfg, config.Children["settings"]);
			RegisterAssemblies(cfg, config.Children["assemblies"]);
			RegisterResources(cfg, config.Children["resources"]);

			// Registers the Configuration object

			Kernel.AddComponentInstance( String.Format("{0}.cfg", id), cfg );

			// Registers the ISessionFactory with a custom activator

			ComponentModel model = new ComponentModel(id, typeof(ISessionFactory), null);
			model.ExtendedProperties.Add(ConfiguredObject, cfg );
			model.LifestyleType = LifestyleType.Singleton;
			model.CustomComponentActivator = typeof( SessionFactoryActivator );
			Kernel.AddCustomComponent( model );
		}

		protected virtual void ApplyConfigurationSettings(Configuration cfg, IConfiguration facilityConfig)
		{
			if (facilityConfig == null) return;

			foreach(IConfiguration item in facilityConfig.Children)
			{
				String key = item.Attributes["key"];
				String value = item.Value;

				cfg.SetProperty(key, value);
			}
		}

		protected virtual void RegisterResources(Configuration cfg, IConfiguration facilityConfig)
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

		protected virtual void RegisterAssemblies(Configuration cfg, IConfiguration facilityConfig)
		{
			if (facilityConfig == null) return;

			foreach(IConfiguration item in facilityConfig.Children)
			{
				String assembly = item.Value;

				cfg.AddAssembly(assembly);
			}
		}

		private Assembly ObtainAssembly(String assembly)
		{
			try
			{
				return Assembly.Load( assembly );
			}
			catch(Exception ex)
			{
				String message = String.Format("The assembly {0} could not be loaded.", assembly);
				throw new ConfigurationException( message, ex );
			}
		}
	}
}