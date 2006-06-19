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
	using System.Collections;
	using System.Configuration;
	using System.IO;
	using System.Xml;
	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Internal;

	/// <summary>
	/// Parses the Xml element that represents
	/// the MonoRail configuration on the xml associated with
	/// the AppDomain (usually web.config)
	/// </summary>
	public class MonoRailSectionHandler : IConfigurationSectionHandler
	{
		#region Constants

		private static readonly String Controllers_Node_Name = "controllers";
		private static readonly String Components_Node_Name = "components";
		private static readonly String Views_Node_Name = "viewEngine";
		private static readonly String Routing_Node_Name = "routing";
		private static readonly String Extensions_Node_Name = "extensions";
		private static readonly String Extension_Node_Name = "extension";
		private static readonly String Custom_Controller_Factory_Node_Name = "customControllerFactory";
		private static readonly String Custom_Component_Factory_Node_Name = "customComponentFactory";
		private static readonly String Custom_Filter_Factory_Node_Name = "customFilterFactory";
		private static readonly String View_Path_Root = "viewPathRoot";
		private static readonly String Cache_Provider = "cacheProvider";
        private static readonly String Scaffolding_Support = "scaffoldingSupport";

		#endregion

		#region IConfigurationSectionHandler implementation

		public virtual object Create(object parent, object configContext, XmlNode section)
		{
			MonoRailConfiguration config = new MonoRailConfiguration();

			config.ConfigSection = section;

			XmlAttribute useWindsorAtt = section.Attributes["useWindsorIntegration"];

			if (useWindsorAtt != null && "true".Equals(useWindsorAtt.Value))
			{
				ConfigureWindsorIntegration(config);
			}

			XmlAttribute smtpHostAtt = section.Attributes["smtpHost"];
			XmlAttribute smtpUserAtt = section.Attributes["smtpUsername"];
			XmlAttribute smtpPwdAtt = section.Attributes["smtpPassword"];

			if (smtpHostAtt != null && smtpHostAtt.Value != String.Empty)
			{
				config.SmtpConfig.Host = smtpHostAtt.Value;
			}
			if (smtpUserAtt != null && smtpUserAtt.Value != String.Empty)
			{
				config.SmtpConfig.Username = smtpUserAtt.Value;
			}
			if (smtpPwdAtt != null && smtpPwdAtt.Value != String.Empty)
			{
				config.SmtpConfig.Password = smtpPwdAtt.Value;
			}

			XmlAttribute checkClientIsConnectedAtt = section.Attributes["checkClientIsConnected"];

			if (checkClientIsConnectedAtt != null && checkClientIsConnectedAtt.Value != String.Empty)
			{
				config.CheckIsClientConnected = String.Compare(checkClientIsConnectedAtt.Value, "true", true) == 0;
			}

			foreach(XmlNode node in section.ChildNodes)
			{
				if (node.NodeType != XmlNodeType.Element)
				{
					continue;
				}

				if (String.Compare(Controllers_Node_Name, node.Name, true) == 0)
				{
					ProcessControllersNode(node, config);
				}
				else if (String.Compare(Components_Node_Name, node.Name, true) == 0)
				{
					ProcessComponentsNode(node, config);
				}
				else if (String.Compare(Views_Node_Name, node.Name, true) == 0)
				{
					ProcessViewNode(node, config);
				}
				else if (String.Compare(Custom_Controller_Factory_Node_Name, node.Name, true) == 0)
				{
					ProcessControllerFactoryNode(node, config);
				}
				else if (String.Compare(Custom_Component_Factory_Node_Name, node.Name, true) == 0)
				{
					ProcessComponentFactoryNode(node, config);
				}
				else if (String.Compare(Custom_Filter_Factory_Node_Name, node.Name, true) == 0)
				{
					ProcessFilterFactoryNode(node, config);
				}
				else if (String.Compare(Routing_Node_Name, node.Name, true) == 0)
				{
					ProcessRoutingNode(node, config);
				}
				else if (String.Compare(Extensions_Node_Name, node.Name, true) == 0)
				{
					ProcessExtensionsNode(node, config);
				}
				else if (String.Compare(Cache_Provider, node.Name, true) == 0)
				{
					ProcessCacheProvider(node, config);
				}
                else if (String.Compare(Scaffolding_Support, node.Name, true) == 0)
                {
                    ProcessScaffoldingSupport(node, config);
                }
			}

			Validate(config);

			return config;
		}

		#endregion

		#region Factories Configuration

		private void ProcessFilterFactoryNode(XmlNode node, MonoRailConfiguration config)
		{
			XmlAttribute type = node.Attributes["type"];

			if (type == null)
			{
				throw new ConfigurationException("The custom filter factory node must specify a 'type' attribute");
			}

			config.CustomFilterFactory = type.Value;
		}

		private void ProcessControllerFactoryNode(XmlNode node, MonoRailConfiguration config)
		{
			XmlAttribute type = node.Attributes["type"];

			if (type == null)
			{
				throw new ConfigurationException("The custom controller factory node must specify a 'type' attribute");
			}

			config.CustomControllerFactory = type.Value;
		}

		private void ProcessComponentFactoryNode(XmlNode node, MonoRailConfiguration config)
		{
			XmlAttribute type = node.Attributes["type"];

			if (type == null)
			{
				throw new ConfigurationException("The custom controller factory node must specify a 'type' attribute");
			}

			config.CustomViewComponentFactory = type.Value;
		}

		#endregion

		#region View Configuration

		private void ProcessViewNode(XmlNode node, MonoRailConfiguration config)
		{
			XmlAttribute viewPath = node.Attributes[View_Path_Root];

			if (viewPath == null)
			{
				throw new ConfigurationException("The views node must specify the '" + View_Path_Root +
					"' attribute");
			}

			String path = viewPath.Value;

			if (!Path.IsPathRooted(path))
			{
				path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, path);
			}

			config.ViewsPhysicalPath = path;

			XmlAttribute xhtmlRendering = node.Attributes["xhtmlRendering"];

			if (xhtmlRendering != null)
			{
				try
				{
					config.ViewsXhtmlRendering = bool.Parse(xhtmlRendering.Value);
				}
				catch(FormatException ex)
				{
					throw new ConfigurationException("The xhtmlRendering attribute of the views node must be a boolean value.", ex);
				}
			}

			XmlAttribute customEngine = node.Attributes["customEngine"];

			if (customEngine != null)
			{
				config.CustomEngineTypeName = customEngine.Value;
			}

			foreach(XmlElement assemblyNode in node.SelectNodes("additionalSources/assembly"))
			{
				String assemblyName = assemblyNode.GetAttribute("name");
				String ns = assemblyNode.GetAttribute("namespace");

				config.AdditionalViewSources.Add(new AssemblySourceInfo(assemblyName, ns));
			}
		}

		#endregion

		#region Controllers Configuration

		private void ProcessControllersNode(XmlNode controllersNode, MonoRailConfiguration config)
		{
			foreach(XmlNode node in controllersNode.ChildNodes)
			{
				if (node.NodeType != XmlNodeType.Element) continue;

				ProcessControllerEntry(node, config);
			}
		}

		private void ProcessControllerEntry(XmlNode controllerEntry, MonoRailConfiguration config)
		{
			if (!controllerEntry.HasChildNodes)
			{
				throw new ConfigurationException("Inside the node controllers, you have to specify the assemblies " +
					"through the value of each child node");
			}

			String assemblyName = controllerEntry.FirstChild.Value;
			config.ControllerAssemblies.Add(assemblyName);
		}

		#endregion

		#region Components Configuration

		private void ProcessComponentsNode(XmlNode controllersNode, MonoRailConfiguration config)
		{
			foreach(XmlNode node in controllersNode.ChildNodes)
			{
				if (node.NodeType != XmlNodeType.Element) continue;

				ProcessComponentEntry(node, config);
			}
		}

		private void ProcessComponentEntry(XmlNode componentNode, MonoRailConfiguration config)
		{
			if (!componentNode.HasChildNodes)
			{
				throw new ConfigurationException("Inside the node components, you have to specify the assemblies " +
					"through the value of each child node");
			}

			String assemblyName = componentNode.FirstChild.Value;
			config.ComponentsAssemblies.Add(assemblyName);
		}

		#endregion

		#region Cache provider

		private void ProcessCacheProvider(XmlNode node, MonoRailConfiguration config)
		{
			XmlAttribute customCacheProvider = node.Attributes["type"];

			if (customCacheProvider != null)
			{
				config.CacheProviderTypeName = customCacheProvider.Value;
			}
		}

		#endregion

        #region Scaffolding Support

        private void ProcessScaffoldingSupport(XmlNode node, MonoRailConfiguration config)
        {
            XmlAttribute scaffoldingSupport = node.Attributes["type"];

            if (scaffoldingSupport != null)
            {
                config.ScaffoldingTypeName = scaffoldingSupport.Value;
            }
        }

        #endregion

        #region Routing Configuration

        private void ProcessRoutingNode(XmlNode routingNode, MonoRailConfiguration config)
		{
			foreach(XmlNode node in routingNode.ChildNodes)
			{
				if (node.NodeType != XmlNodeType.Element) continue;

				ProcessRuleEntry(node, config);
			}
		}

		private void ProcessRuleEntry(XmlNode node, MonoRailConfiguration config)
		{
			XmlNode patternNode = node.SelectSingleNode("pattern");
			XmlNode replaceNode = node.SelectSingleNode("replace");

			if (patternNode == null || patternNode.ChildNodes[0] == null)
			{
				throw new ConfigurationException("A rule node must have a pattern (child) node denoting the regular expression to be matched");
			}
			if (replaceNode == null || replaceNode.ChildNodes[0] == null)
			{
				throw new ConfigurationException("A rule node must have a replace (child) node denoting the string to be replaced");
			}

			String pattern = patternNode.ChildNodes[0].Value;
			String replace = replaceNode.ChildNodes[0].Value;

			config.RoutingRules.Add(new RoutingRule(pattern.Trim(), replace.Trim()));
		}

		#endregion

		#region Extensions Configuration

		private void ProcessExtensionsNode(XmlNode routingNode, MonoRailConfiguration config)
		{
			foreach(XmlNode node in routingNode.ChildNodes)
			{
				if (node.NodeType != XmlNodeType.Element) continue;

				if (node.Name != Extension_Node_Name)
				{
					String message = String.Format("Unexpected node '{0}' within '{1}' node. " +
						"Expected {2}", node.Name, Extensions_Node_Name, Extension_Node_Name);
					throw new ConfigurationException(message);
				}

				XmlAttribute typeAtt = node.Attributes["type"];

				if (typeAtt == null || typeAtt.Value.Length == 0)
				{
					String message = String.Format("An {0} node must have a 'type' attribute " +
						"to declare the extension full type name", Extension_Node_Name);
					throw new ConfigurationException(message);
				}

				config.Extensions.Add(typeAtt.Value);
			}
		}

		#endregion

		#region Configuration Validation

		protected void Validate(MonoRailConfiguration config)
		{
			if (config.CustomControllerFactory != null)
			{
				ValidateTypeImplements(config.CustomControllerFactory, typeof(IControllerFactory));
			}
			else
			{
				if (config.ControllerAssemblies.Count == 0)
				{
					throw new ConfigurationException("Inside the node controllers, you have to specify " +
						"at least one assembly entry");
				}
			}

			IList extensionTypes = new ArrayList();

			foreach(String typeName in config.Extensions)
			{
				Type extensionType = MonoRailConfiguration.GetType(typeName);

				ValidateTypeImplements(extensionType, typeof(IMonoRailExtension));

				extensionTypes.Add(extensionType);
			}

			config.Extensions = extensionTypes;

			if (config.CustomFilterFactory != null)
			{
				ValidateTypeImplements(config.CustomFilterFactory, typeof(IFilterFactory));
			}
			if (config.CacheProviderType != null)
			{
				ValidateTypeImplements(config.CacheProviderType, typeof(ICacheProvider));
			}
            if (config.ScaffoldingType != null)
            {
                ValidateTypeImplements(config.ScaffoldingType, typeof(IScaffoldingSupport));
            }
			if (config.CustomViewComponentFactory != null)
			{
				ValidateTypeImplements(config.CustomViewComponentFactory, typeof(IViewComponentFactory));
			}
			if (config.CustomEngineTypeName != null)
			{
				ValidateTypeImplements(config.CustomEngineTypeName, typeof(IViewEngine));
			}
			if (config.ViewsPhysicalPath == null || config.ViewsPhysicalPath.Length == 0)
			{
				throw new ConfigurationException("You must provide a '" + Views_Node_Name + "' node and a " +
					"absolute or relative path to the views directory with the attribute " + View_Path_Root);
			}

			// Reasonable assumption

			if (config.ControllerAssemblies.Count != 0 && config.ComponentsAssemblies.Count == 0)
			{
				foreach(String entry in config.ControllerAssemblies)
				{
					config.ComponentsAssemblies.Add(entry);
				}
			}
		}

		private void ValidateTypeImplements(String name, Type expectedType)
		{
			Type type = Type.GetType(name, false, false);

			if (type == null)
			{
				String message = String.Format("Type {0} could not be loaded", name);
				throw new ConfigurationException(message);
			}

			ValidateTypeImplements(type, expectedType);
		}

		private void ValidateTypeImplements(Type type, Type expectedType)
		{
			if (!expectedType.IsAssignableFrom(type))
			{
				String message = String.Format("Type {0} does not implement {1}",
				                               type.FullName, expectedType.FullName);
				throw new ConfigurationException(message);
			}
		}

		#endregion

		private void ConfigureWindsorIntegration(MonoRailConfiguration config)
		{
			config.CustomControllerFactory = "Castle.MonoRail.WindsorExtension.WindsorControllerFactory, Castle.MonoRail.WindsorExtension";
			config.CustomFilterFactory = "Castle.MonoRail.WindsorExtension.WindsorFilterFactory, Castle.MonoRail.WindsorExtension";
			config.CustomViewComponentFactory = "Castle.MonoRail.WindsorExtension.WindsorViewComponentFactory, Castle.MonoRail.WindsorExtension";
		}
	}
}
