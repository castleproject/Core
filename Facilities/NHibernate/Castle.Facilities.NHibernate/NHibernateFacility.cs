// Copyright 2004 DigitalCraftsmen - http://www.digitalcraftsmen.com.br/
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

using NHibernate;
using NHibernate.Cfg;

namespace Castle.Facilities.NHibernate
{
	using System;
	using System.IO;
	using System.Reflection;
	using System.Configuration;

	using Castle.MicroKernel;

	using Castle.Model;

	using Castle.Model.Configuration;

	/// <summary>
	/// 
	/// </summary>
	public class NHibernateFacility : IFacility
	{
		public static readonly String ConfiguredObject = "nhibernate.cfg";

		public NHibernateFacility()
		{
		}

		#region IFacility Members

		public void Init(IKernel kernel, IConfiguration facilityConfig)
		{
			if (facilityConfig == null)
			{
				throw new ConfigurationException(
					"The NHibernateFacility requires an external configuration");
			}

			IConfiguration factoriesConfig = facilityConfig.Children["factory"];

			if (factoriesConfig == null)
			{
				throw new ConfigurationException(
					"You need to configure at least one factory for NHibernateFacility");
			}

			ConfigureFactories(kernel, factoriesConfig);
		}

		public void Terminate()
		{
		}

		#endregion

		private void ConfigureFactories(IKernel kernel, IConfiguration config)
		{
			String id = config.Attributes["id"];

			Configuration cfg = new Configuration();

			ApplyConfigurationSettings(cfg, config.Children["settings"]);
			RegisterAssemblies(cfg, config.Children["assemblies"]);
			RegisterResources(cfg, config.Children["resources"]);

			ComponentModel model = new ComponentModel(id, typeof(ISessionFactory), null);
			model.ExtendedProperties.Add(ConfiguredObject, cfg );
			model.LifestyleType = LifestyleType.Singleton;
			model.CustomComponentActivator = typeof( SessionFactoryActivator );

			kernel.AddCustomComponent( model );
		}

		protected virtual void ApplyConfigurationSettings(Configuration cfg, IConfiguration facilityConfig)
		{
			foreach(IConfiguration item in facilityConfig.Children)
			{
				String key = item.Attributes["key"];
				String value = item.Value;

				cfg.SetProperty(key, value);
			}
		}

		protected virtual void RegisterResources(Configuration cfg, IConfiguration facilityConfig)
		{
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
