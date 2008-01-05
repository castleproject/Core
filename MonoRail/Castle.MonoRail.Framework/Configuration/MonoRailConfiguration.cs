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

namespace Castle.MonoRail.Framework.Configuration
{
	using System;
	using System.Configuration;
	using System.Xml;
	using Castle.Core.Configuration;
	using Castle.Core.Configuration.Xml;
	using Castle.MonoRail.Framework.Helpers.ValidationStrategy;
	using Castle.MonoRail.Framework.JSGeneration.Prototype;
	using JSGeneration;

	/// <summary>
	/// Represents the MonoRail external configuration
	/// </summary>
	public class MonoRailConfiguration : IMonoRailConfiguration, ISerializedConfig
	{
		private static readonly String SectionName = "monorail";
		private static readonly String AlternativeSectionName = "monoRail";

		private bool matchHostNameAndPath, excludeAppPath;
		private Type customFilterFactory;
		private IConfiguration configurationSection;

		private SmtpConfig smtpConfig;
		private ViewEngineConfig viewEngineConfig;
		private ControllersConfig controllersConfig;
		private ViewComponentsConfig viewComponentsConfig;
		private ScaffoldConfig scaffoldConfig;
		private UrlConfig urlConfig;
		private JSGeneratorConfiguration jsGeneratorConfig;
		private RoutingRuleCollection routingRules;
		private ExtensionEntryCollection extensions;
		private DefaultUrlCollection defaultUrls;
		private IConfiguration servicesConfig;

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
			defaultUrls = new DefaultUrlCollection();
			jsGeneratorConfig = new JSGeneratorConfiguration();

			jsGeneratorConfig.AddLibrary("prototype-1.5.1", typeof(PrototypeGenerator))
				.AddExtension(typeof(CommonJSExtension))
				.AddExtension(typeof(ScriptaculousExtension))
				.AddExtension(typeof(BehaviourExtension))
				.BrowserValidatorIs(typeof(PrototypeWebValidator))
				.SetAsDefault();

			// old routing support related
			matchHostNameAndPath = false;
			excludeAppPath = false;
		}

		/// <summary>
		/// Pendent
		/// </summary>
		/// <param name="section"></param>
		public MonoRailConfiguration(XmlNode section) : this()
		{
			configurationSection = XmlConfigurationDeserializer.GetDeserializedNode(section);
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
				config = ConfigurationManager.GetSection(AlternativeSectionName) as MonoRailConfiguration;
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

			extensions.Deserialize(node);
			routingRules.Deserialize(node);
			defaultUrls.Deserialize(node);

			ProcessFilterFactoryNode(node.SelectSingleNode("customFilterFactory"));
			ProcessMatchHostNameAndPath(node.SelectSingleNode("routing"));
			ProcessExcludeAppPath(node.SelectSingleNode("routing"));

			XmlNode services = node.SelectSingleNode("services");

			if (services != null)
			{
				servicesConfig = XmlConfigurationDeserializer.GetDeserializedNode(services);
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
			set { controllersConfig = value; }
		}

		/// <summary>
		/// Gets the view components config.
		/// </summary>
		/// <value>The view components config.</value>
		public ViewComponentsConfig ViewComponentsConfig
		{
			get { return viewComponentsConfig; }
			set { viewComponentsConfig = value; }
		}

		/// <summary>
		/// Gets the routing rules.
		/// </summary>
		/// <value>The routing rules.</value>
		public RoutingRuleCollection RoutingRules
		{
			get { return routingRules; }
			set { routingRules = value; }
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
		/// Gets the custom filter factory.
		/// </summary>
		/// <value>The custom filter factory.</value>
		public Type CustomFilterFactory
		{
			get { return customFilterFactory; }
			set { customFilterFactory = value; }
		}

		/// <summary>
		/// Gets the scaffold config.
		/// </summary>
		/// <value>The scaffold config.</value>
		public ScaffoldConfig ScaffoldConfig
		{
			get { return scaffoldConfig; }
			set { scaffoldConfig = value; }
		}

		/// <summary>
		/// Gets the url config.
		/// </summary>
		/// <value>The url config.</value>
		public UrlConfig UrlConfig
		{
			get { return urlConfig; }
			set { urlConfig = value; }
		}

		/// <summary>
		/// Gets or sets the JS generator configuration.
		/// </summary>
		/// <value>The JS generator configuration.</value>
		public JSGeneratorConfiguration JSGeneratorConfiguration
		{
			get { return jsGeneratorConfig; }
			set { jsGeneratorConfig = value; }
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
			set { matchHostNameAndPath = value; }
		}

		/// <summary>
		/// Gets a value indicating whether routing should exclude app path.
		/// </summary>
		/// <value><c>true</c> if exclude app path; otherwise, <c>false</c>.</value>
		public bool ExcludeAppPath
		{
			get { return excludeAppPath; }
			set { excludeAppPath = value; }
		}

		/// <summary>
		/// Gets the configuration section.
		/// </summary>
		/// <value>The configuration section.</value>
		public IConfiguration ConfigurationSection
		{
			get { return configurationSection; }
			set { configurationSection = value; }
		}

		/// <summary>
		/// Gets the default urls.
		/// </summary>
		/// <value>The default urls.</value>
		public DefaultUrlCollection DefaultUrls
		{
			get { return defaultUrls; }
		}

		/// <summary>
		/// Gets or sets the services config.
		/// </summary>
		/// <value>The services config.</value>
		public IConfiguration ServicesConfig
		{
			get { return servicesConfig; }
			set { servicesConfig = value; }
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
	}
}
