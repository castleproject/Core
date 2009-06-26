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

namespace Castle.Facilities.NHibernateIntegration.Builders
{
	using System;
	using System.Configuration;
	using System.IO;
	using System.Reflection;
	using Core.Configuration;
	using NHibernate.Event;
	using Configuration=NHibernate.Cfg.Configuration;

	/// <summary>
	/// Default imlementation of <see cref="IConfigurationBuilder"/>
	/// </summary>
	public class DefaultConfigurationBuilder : IConfigurationBuilder
	{
		private const String NHMappingAttributesAssemblyName = "NHibernate.Mapping.Attributes";

		/// <summary>
		/// Builds the Configuration object from the specifed configuration
		/// </summary>
		/// <param name="config"></param>
		/// <returns></returns>
		public virtual Configuration GetConfiguration(IConfiguration config)
		{
			Configuration cfg = new Configuration();

			this.ApplyConfigurationSettings(cfg, config.Children["settings"]);
			this.RegisterAssemblies(cfg, config.Children["assemblies"]);
			this.RegisterResources(cfg, config.Children["resources"]);
			this.RegisterListeners(cfg, config.Children["listeners"]);
			return cfg;
		}

		/// <summary>
		/// Applies the configuration settings.
		/// </summary>
		/// <param name="cfg">The CFG.</param>
		/// <param name="facilityConfig">The facility config.</param>
		protected void ApplyConfigurationSettings(Configuration cfg, IConfiguration facilityConfig)
		{
			if (facilityConfig == null) return;

			foreach (IConfiguration item in facilityConfig.Children)
			{
				String key = item.Attributes["key"];
				String value = item.Value;

				cfg.SetProperty(key, value);
			}
		}
		/// <summary>
		/// Registers the resources.
		/// </summary>
		/// <param name="cfg">The CFG.</param>
		/// <param name="facilityConfig">The facility config.</param>
		protected void RegisterResources(Configuration cfg, IConfiguration facilityConfig)
		{
			if (facilityConfig == null) return;

			foreach (IConfiguration item in facilityConfig.Children)
			{
				String name = item.Attributes["name"];
				String assembly = item.Attributes["assembly"];

				if (assembly != null)
				{
					cfg.AddResource(name, this.ObtainAssembly(assembly));
				}
				else
				{
					cfg.AddXmlFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, name));
				}
			}
		}

		/// <summary>
		/// Registers the listeners.
		/// </summary>
		/// <param name="cfg">The CFG.</param>
		/// <param name="facilityConfig">The facility config.</param>
		protected void RegisterListeners(Configuration cfg, IConfiguration facilityConfig)
		{
			if (facilityConfig == null) return;

			foreach (IConfiguration item in facilityConfig.Children)
			{
				String eventName = item.Attributes["event"];
				String typeName = item.Attributes["type"];

				if (!Enum.IsDefined(typeof(ListenerType), eventName)) throw new ConfigurationErrorsException("An invalid listener type was specified.");

				Type classType = Type.GetType(typeName);
				if (classType == null) throw new ConfigurationErrorsException("The full type name of the listener class must be specified.");

				ListenerType listenerType = (ListenerType)Enum.Parse(typeof(ListenerType), eventName);
				object listenerInstance = Activator.CreateInstance(classType);

				cfg.SetListener(listenerType, listenerInstance);
			}
		}

		/// <summary>
		/// Registers the assemblies.
		/// </summary>
		/// <param name="cfg">The CFG.</param>
		/// <param name="facilityConfig">The facility config.</param>
		protected void RegisterAssemblies(Configuration cfg, IConfiguration facilityConfig)
		{
			if (facilityConfig == null) return;

			foreach (IConfiguration item in facilityConfig.Children)
			{
				String assembly = item.Value;

				cfg.AddAssembly(assembly);

				this.GenerateMappingFromAttributesIfNeeded(cfg, assembly);
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
			if (Array.Exists<AssemblyName>(refAssemblies, delegate(AssemblyName an) { return an.Name.Equals(NHMappingAttributesAssemblyName); }))
			{
				//Obtains, by reflexion, the necessary tools to generate NH mapping from attributes
				Type HbmSerializerType = Type.GetType(String.Concat(NHMappingAttributesAssemblyName, ".HbmSerializer, ", NHMappingAttributesAssemblyName));
				Object hbmSerializer = Activator.CreateInstance(HbmSerializerType);
				PropertyInfo validate = HbmSerializerType.GetProperty("Validate");
				MethodInfo serialize = HbmSerializerType.GetMethod("Serialize", new Type[] { typeof(Assembly) });

				//Enable validation of mapping documents generated from the mapping attributes
				validate.SetValue(hbmSerializer, true, null);

				//Generates a stream of mapping documents from all decorated classes in targetAssembly and add it to NH config
				cfg.AddInputStream((MemoryStream)serialize.Invoke(hbmSerializer, new object[] { Assembly.Load(targetAssembly) }));
			}
		}
		private Assembly ObtainAssembly(String assembly)
		{
			try
			{
				return Assembly.Load(assembly);
			}
			catch (Exception ex)
			{
				String message = String.Format("The assembly {0} could not be loaded.", assembly);

				throw new ConfigurationErrorsException(message, ex);
			}
		}
	}
}