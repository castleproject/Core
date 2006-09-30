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

namespace Castle.MonoRail.Framework.Configuration
{
	using System;
	using System.Configuration;
	using System.Xml;

	/// <summary>
	/// Pendent
	/// </summary>
	public class MonoRailConfiguration : ISerializedConfig
	{
		private static readonly String SectionName = "monorail";
		private static readonly String AlternativeSectionName = "monoRail";

		private bool checkClientIsConnected, useWindsorIntegration;
		private Type customFilterFactory;
		private XmlNode configurationSection;

		private SmtpConfig smtpConfig;
		private ViewEngineConfig viewEngineConfig;
		private ControllersConfig controllersConfig;
		private ViewComponentsConfig viewComponentsConfig;
		private ScaffoldConfig scaffoldConfig;

		private RoutingRuleCollection routingRules;
		private ExtensionEntryCollection extensions;
		private ServiceEntryCollection services;
		
		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="section"></param>
		public MonoRailConfiguration(XmlNode section)
		{
			smtpConfig = new SmtpConfig();
			viewEngineConfig = new ViewEngineConfig();
			controllersConfig = new ControllersConfig();
			viewComponentsConfig = new ViewComponentsConfig();
			scaffoldConfig = new ScaffoldConfig();
			routingRules = new RoutingRuleCollection();
			extensions = new ExtensionEntryCollection();
			services = new ServiceEntryCollection();
			
			checkClientIsConnected = false;
			
			configurationSection = section;
		}

		public static MonoRailConfiguration GetConfig()
		{
#if DOTNET2
			MonoRailConfiguration config =
				System.Configuration.ConfigurationManager.GetSection(MonoRailConfiguration.SectionName) as MonoRailConfiguration;
#else
			MonoRailConfiguration config = 
				ConfigurationSettings.GetConfig(MonoRailConfiguration.SectionName) as MonoRailConfiguration;
#endif

			if (config == null)
			{
#if DOTNET2
				config = 
					System.Configuration.ConfigurationManager.GetSection(MonoRailConfiguration.AlternativeSectionName) as MonoRailConfiguration;
#else
				config = 
					ConfigurationSettings.GetConfig(MonoRailConfiguration.AlternativeSectionName) as MonoRailConfiguration;
#endif
			}

			if (config == null)
			{
				throw new ApplicationException("You have to provide a small configuration to use " + 
				                               "MonoRail. Check the samples or the documentation");
			}

			return config;
		}

		#region ISerializedConfig implementation
		
		public void Deserialize(XmlNode node)
		{
			viewEngineConfig.Deserialize(node);
			smtpConfig.Deserialize(node);
			controllersConfig.Deserialize(node);
			viewComponentsConfig.Deserialize(node);
			scaffoldConfig.Deserialize(node);
			
			services.Deserialize(node);
			extensions.Deserialize(node);
			routingRules.Deserialize(node);

			ProcessFilterFactoryNode(node.SelectSingleNode("customFilterFactory"));
			
			XmlAttribute checkClientIsConnectedAtt = node.Attributes["checkClientIsConnected"];

			if (checkClientIsConnectedAtt != null && checkClientIsConnectedAtt.Value != String.Empty)
			{
				checkClientIsConnected = String.Compare(checkClientIsConnectedAtt.Value, "true", true) == 0;
			}
			
			XmlAttribute useWindsorAtt = node.Attributes["useWindsorIntegration"];

			if (useWindsorAtt != null && useWindsorAtt.Value != String.Empty)
			{
				useWindsorIntegration = String.Compare(useWindsorAtt.Value, "true", true) == 0;
				
				ConfigureWindsorIntegration();
			}
		}
		
		#endregion
		
		private void ProcessFilterFactoryNode(XmlNode node)
		{
			if (node == null) return;
			
			XmlAttribute type = node.Attributes["type"];

			if (type == null)
			{
				throw new ConfigurationException("The custom filter factory node must specify a 'type' attribute");
			}

			customFilterFactory = TypeLoadUtil.GetType(type.Value);
		}

		public SmtpConfig SmtpConfig
		{
			get { return smtpConfig; }
		}

		public ViewEngineConfig ViewEngineConfig
		{
			get { return viewEngineConfig; }
		}

		public ControllersConfig ControllersConfig
		{
			get { return controllersConfig; }
		}

		public ViewComponentsConfig ViewComponentsConfig
		{
			get { return viewComponentsConfig; }
		}

		public RoutingRuleCollection RoutingRules
		{
			get { return routingRules; }
		}

		public ExtensionEntryCollection ExtensionEntries
		{
			get { return extensions; }
		}

		public ServiceEntryCollection ServiceEntries
		{
			get { return services; }
		}

		public Type CustomFilterFactory
		{
			get { return customFilterFactory; }
		}

		public ScaffoldConfig ScaffoldConfig
		{
			get { return scaffoldConfig; }
		}

		public bool CheckClientIsConnected
		{
			get { return checkClientIsConnected; }
		}

		public bool UseWindsorIntegration
		{
			get { return useWindsorIntegration; }
		}

		public XmlNode ConfigurationSection
		{
			get { return configurationSection; }
		}

		private void ConfigureWindsorIntegration()
		{
			const String windsorAssembly = "Castle.MonoRail.WindsorExtension";
			
			controllersConfig.CustomControllerFactory = TypeLoadUtil.GetType(
				TypeLoadUtil.GetEffectiveTypeName("Castle.MonoRail.WindsorExtension.WindsorControllerFactory, " + windsorAssembly));
			
			viewComponentsConfig.CustomViewComponentFactory = TypeLoadUtil.GetType(
				TypeLoadUtil.GetEffectiveTypeName("Castle.MonoRail.WindsorExtension.WindsorViewComponentFactory, " + windsorAssembly));
			
			customFilterFactory = TypeLoadUtil.GetType(
				TypeLoadUtil.GetEffectiveTypeName("Castle.MonoRail.WindsorExtension.WindsorFilterFactory, " + windsorAssembly));
		}
		
	}
}
