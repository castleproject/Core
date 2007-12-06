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

namespace Castle.MonoRail.Framework.Configuration
{
	using System;
	using System.Configuration;
	using System.Xml;

	/// <summary>
	/// Represents the MonoRail external configuration
	/// </summary>
	public class MonoRailConfiguration : ISerializedConfig
	{
		private static readonly String SectionName = "monorail";
		private static readonly String AlternativeSectionName = "monoRail";

		private bool checkClientIsConnected, useWindsorIntegration, matchHostNameAndPath, excludeAppPath;
		private Type customFilterFactory;
		private XmlNode configurationSection;

		private SmtpConfig smtpConfig;
		private ViewEngineConfig viewEngineConfig;
		private ControllersConfig controllersConfig;
		private ViewComponentsConfig viewComponentsConfig;
		private ScaffoldConfig scaffoldConfig;
		private UrlConfig urlConfig;

		private RoutingRuleCollection routingRules;
		private ExtensionEntryCollection extensions;
		private ServiceEntryCollection services;
		private DefaultUrlCollection defaultUrls;

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoRailConfiguration"/> class.
		/// </summary>
		public MonoRailConfiguration()
		{
			smtpConfig = new SmtpConfig();
			viewEngineConfig = new ViewEngineConfig();
			controllersConfig = new ControllersConfig();
			viewComponentsConfig = new ViewComponentsConfig();
			scaffoldConfig = new ScaffoldConfig();
			urlConfig = new UrlConfig();
			routingRules = new RoutingRuleCollection();
			extensions = new ExtensionEntryCollection();
			services = new ServiceEntryCollection();
			defaultUrls = new DefaultUrlCollection();

			checkClientIsConnected = false;
			matchHostNameAndPath = false;
			excludeAppPath = false;
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="section"></param>
		public MonoRailConfiguration(XmlNode section) : this()
		{
			configurationSection = section;
		}

		/// <summary>
		/// Gets the config.
		/// </summary>
		/// <returns></returns>
		public static MonoRailConfiguration GetConfig()
		{
			MonoRailConfiguration config =
				ConfigurationManager.GetSection(SectionName) as MonoRailConfiguration;

			if (config == null)
			{
				config =
					ConfigurationManager.GetSection(AlternativeSectionName) as MonoRailConfiguration;
			}

			if (config == null)
			{
				throw new ApplicationException("You have to provide a small configuration to use " +
				                               "MonoRail. Check the samples or the documentation");
			}

			return config;
		}

		#region ISerializedConfig implementation

		/// <summary>
		/// Deserializes the specified node.
		/// </summary>
		/// <param name="node">The node.</param>
		public void Deserialize(XmlNode node)
		{
			viewEngineConfig.Deserialize(node);
			smtpConfig.Deserialize(node);
			controllersConfig.Deserialize(node);
			viewComponentsConfig.Deserialize(node);
			scaffoldConfig.Deserialize(node);
			urlConfig.Deserialize(node);

			services.Deserialize(node);
			extensions.Deserialize(node);
			routingRules.Deserialize(node);
			defaultUrls.Deserialize(node);

			ProcessFilterFactoryNode(node.SelectSingleNode("customFilterFactory"));
			ProcessMatchHostNameAndPath(node.SelectSingleNode("routing"));
			ProcessExcludeAppPath(node.SelectSingleNode("routing"));

			XmlAttribute checkClientIsConnectedAtt = node.Attributes["checkClientIsConnected"];

			if (checkClientIsConnectedAtt != null && checkClientIsConnectedAtt.Value != String.Empty)
			{
				checkClientIsConnected = String.Compare(checkClientIsConnectedAtt.Value, "true", true) == 0;
			}

			XmlAttribute useWindsorAtt = node.Attributes["useWindsorIntegration"];

			if (useWindsorAtt != null && useWindsorAtt.Value != String.Empty)
			{
				useWindsorIntegration = String.Compare(useWindsorAtt.Value, "true", true) == 0;

				if (useWindsorIntegration)
				{
					ConfigureWindsorIntegration();
				}
			}
		}

		#endregion

		/// <summary>
		/// Gets the SMTP config.
		/// </summary>
		/// <value>The SMTP config.</value>
		public SmtpConfig SmtpConfig
		{
			get { return smtpConfig; }
		}

		/// <summary>
		/// Gets the view engine config.
		/// </summary>
		/// <value>The view engine config.</value>
		public ViewEngineConfig ViewEngineConfig
		{
			get { return viewEngineConfig; }
		}

		/// <summary>
		/// Gets the controllers config.
		/// </summary>
		/// <value>The controllers config.</value>
		public ControllersConfig ControllersConfig
		{
			get { return controllersConfig; }
		}

		/// <summary>
		/// Gets the view components config.
		/// </summary>
		/// <value>The view components config.</value>
		public ViewComponentsConfig ViewComponentsConfig
		{
			get { return viewComponentsConfig; }
		}

		/// <summary>
		/// Gets the routing rules.
		/// </summary>
		/// <value>The routing rules.</value>
		public RoutingRuleCollection RoutingRules
		{
			get { return routingRules; }
		}

		/// <summary>
		/// Gets the extension entries.
		/// </summary>
		/// <value>The extension entries.</value>
		public ExtensionEntryCollection ExtensionEntries
		{
			get { return extensions; }
		}

		/// <summary>
		/// Gets the service entries.
		/// </summary>
		/// <value>The service entries.</value>
		public ServiceEntryCollection ServiceEntries
		{
			get { return services; }
		}

		/// <summary>
		/// Gets the custom filter factory.
		/// </summary>
		/// <value>The custom filter factory.</value>
		public Type CustomFilterFactory
		{
			get { return customFilterFactory; }
		}

		/// <summary>
		/// Gets the scaffold config.
		/// </summary>
		/// <value>The scaffold config.</value>
		public ScaffoldConfig ScaffoldConfig
		{
			get { return scaffoldConfig; }
		}

		/// <summary>
		/// Gets the url config.
		/// </summary>
		/// <value>The url config.</value>
		public UrlConfig UrlConfig
		{
			get { return urlConfig; }	
		}

		/// <summary>
		/// Gets a value indicating whether MR should check for client connection.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if it should check client is connected; otherwise, <c>false</c>.
		/// </value>
		public bool CheckClientIsConnected
		{
			get { return checkClientIsConnected; }
		}

		/// <summary>
		/// Gets a value indicating whether to use windsor integration.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if it should use windsor integration; otherwise, <c>false</c>.
		/// </value>
		public bool UseWindsorIntegration
		{
			get { return useWindsorIntegration; }
		}

		/// <summary>
		/// Gets a value indicating whether match host name and path should be used on 
		/// MonoRail routing.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if it should match host name and path; otherwise, <c>false</c>.
		/// </value>
		public bool MatchHostNameAndPath
		{
			get { return matchHostNameAndPath; }
		}

		/// <summary>
		/// Gets a value indicating whether routing should exclude app path.
		/// </summary>
		/// <value><c>true</c> if exclude app path; otherwise, <c>false</c>.</value>
		public bool ExcludeAppPath
		{
			get { return excludeAppPath; }
		}

		/// <summary>
		/// Gets the configuration section.
		/// </summary>
		/// <value>The configuration section.</value>
		public XmlNode ConfigurationSection
		{
			get { return configurationSection; }
		}

		/// <summary>
		/// Gets the default urls.
		/// </summary>
		/// <value>The default urls.</value>
		public DefaultUrlCollection DefaultUrls
		{
			get { return defaultUrls; }
		}

		private void ProcessFilterFactoryNode(XmlNode node)
		{
			if (node == null) return;

			XmlAttribute type = node.Attributes["type"];

			if (type == null)
			{
				String message = "The custom filter factory node must specify a 'type' attribute";
				throw new ConfigurationErrorsException(message);
			}

			customFilterFactory = TypeLoadUtil.GetType(type.Value);
		}

		private void ProcessMatchHostNameAndPath(XmlNode node)
		{
			if (node == null) return;

			XmlAttribute matchHostNameAndPathAtt = node.Attributes["matchHostNameAndPath"];

			if (matchHostNameAndPathAtt != null && matchHostNameAndPathAtt.Value != String.Empty)
			{
				matchHostNameAndPath = String.Compare(matchHostNameAndPathAtt.Value, "true", true) == 0;
			}
		}

		private void ProcessExcludeAppPath(XmlNode node)
		{
			if (node == null) return;

			// maybe a check to make sure both matchHostNameAndPathAtt & includeAppPath 
			// are not both set as that wouldn't make sense?
			XmlAttribute excludeAppPathAtt = node.Attributes["excludeAppPath"];

			if (excludeAppPathAtt != null && excludeAppPathAtt.Value != String.Empty)
			{
				excludeAppPath = String.Compare(excludeAppPathAtt.Value, "true", true) == 0;
			}
		}

		private void ConfigureWindsorIntegration()
		{
			const string windsorExtensionAssemblyName = "Castle.MonoRail.WindsorExtension";

			services.RegisterService(ServiceIdentification.ControllerTree, TypeLoadUtil.GetType(
			                                                               	TypeLoadUtil.GetEffectiveTypeName(
			                                                               		"Castle.MonoRail.WindsorExtension.ControllerTreeAccessor, " +
			                                                               		windsorExtensionAssemblyName)));

			controllersConfig.CustomControllerFactory = TypeLoadUtil.GetType(
				TypeLoadUtil.GetEffectiveTypeName("Castle.MonoRail.WindsorExtension.WindsorControllerFactory, " +
				                                  windsorExtensionAssemblyName));

			viewComponentsConfig.CustomViewComponentFactory = TypeLoadUtil.GetType(
				TypeLoadUtil.GetEffectiveTypeName("Castle.MonoRail.WindsorExtension.WindsorViewComponentFactory, " +
				                                  windsorExtensionAssemblyName));

			customFilterFactory = TypeLoadUtil.GetType(
				TypeLoadUtil.GetEffectiveTypeName("Castle.MonoRail.WindsorExtension.WindsorFilterFactory, " +
				                                  windsorExtensionAssemblyName));
		}
	}
}
