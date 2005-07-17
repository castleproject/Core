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

namespace Castle.MonoRail.Engine.Configuration
{
	using System;
	using System.IO;
	using System.Xml;
	using System.Configuration;

	using Castle.MonoRail.Framework;
	using Castle.MonoRail.Framework.Internal;


	public class MonoRailSectionHandler : IConfigurationSectionHandler
	{
		private static readonly String Controllers_Node_Name = "controllers";
		private static readonly String Views_Node_Name = "viewEngine";
		private static readonly String Routing_Node_Name = "routing";
		private static readonly String Custom_Controller_Factory_Node_Name = "customControllerFactory";
		private static readonly String Custom_Filter_Factory_Node_Name = "customFilterFactory";
		private static readonly String View_Path_Root = "viewPathRoot";

		public object Create(object parent, object configContext, XmlNode section)
		{
			MonoRailConfiguration config = new MonoRailConfiguration();

			foreach( XmlNode node in section.ChildNodes)
			{
				if (node.NodeType != XmlNodeType.Element)
				{
					continue;
				}

				if ( String.Compare(Controllers_Node_Name, node.Name, true) == 0 )
				{
					ProcessControllersNode(node, config);
				}
				else if ( String.Compare(Views_Node_Name, node.Name, true) == 0 )
				{
					ProcessViewNode(node, config);
				}
				else if ( String.Compare(Custom_Controller_Factory_Node_Name, node.Name, true) == 0 )
				{
					ProcessControllerFactoryNode(node, config);
				}
				else if ( String.Compare(Custom_Filter_Factory_Node_Name, node.Name, true) == 0 )
				{
					ProcessFilterFactoryNode(node, config);
				}
				else if ( String.Compare(Routing_Node_Name, node.Name, true) == 0 )
				{
					ProcessRoutingNode(node, config);
				}
				else
				{
					throw new ConfigurationException("Unknown node: " + node.Name);
				}
			}

			Validate(config);

			return config;
		}

		private void ProcessRoutingNode(XmlNode routingNode, MonoRailConfiguration config)
		{
			foreach(XmlNode node in routingNode.ChildNodes)
			{
				if (node.NodeType != XmlNodeType.Element) continue;

				ProcessRuleEntry( node, config );
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

			config.RoutingRules.Add( new RoutingRule(pattern.Trim(), replace.Trim()) ); 
		}

		private void ProcessFilterFactoryNode(XmlNode node, MonoRailConfiguration config)
		{
			XmlAttribute type = node.Attributes["type"];
	
			if (type == null)
			{
				throw new ConfigurationException("The custom filter factory node must specify a type attribute");
			}
	
			config.CustomFilterFactory = type.Value;
		}

		private void ProcessControllerFactoryNode(XmlNode node, MonoRailConfiguration config)
		{
			XmlAttribute type = node.Attributes["type"];
	
			if (type == null)
			{
				throw new ConfigurationException("The custom controller factory node must specify a type attribute");
			}
	
			config.CustomControllerFactory = type.Value;
		}

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
				path = Path.Combine( AppDomain.CurrentDomain.BaseDirectory, path );
			}

			config.ViewsPhysicalPath = path;
			config.ViewsVirtualPath = viewPath.Value;

			XmlAttribute xhtmlRendering = node.Attributes["xhtmlRendering"];
			if (xhtmlRendering != null)
			{
				try
				{
					config.ViewsXhtmlRendering = bool.Parse(xhtmlRendering.Value);
				}
				catch (FormatException ex)
				{
					config.ViewsXhtmlRendering = false;
					throw new ConfigurationException("The xhtmlRendering attribute of the views node must be a boolean value.", ex);
				}
			}
			else
			{
				config.ViewsXhtmlRendering = false;
			}

			XmlAttribute customEngine = node.Attributes["customEngine"];	
			if (customEngine != null)
			{
				config.CustomEngineTypeName = customEngine.Value;
			}
		}

		private void ProcessControllersNode(XmlNode controllersNode, MonoRailConfiguration config)
		{
			foreach(XmlNode node in controllersNode.ChildNodes)
			{
				if (node.NodeType != XmlNodeType.Element) continue;

				ProcessControllerEntry( node, config );
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
			config.Assemblies.Add( assemblyName );
		}

		protected virtual void Validate(MonoRailConfiguration config)
		{
			if (config.CustomControllerFactory != null)
			{
				ValidateType(config.CustomControllerFactory, typeof(IControllerFactory));
			}
			else
			{
				if (config.Assemblies.Count == 0)
				{
					throw new ConfigurationException("Inside the node controllers, you have to specify " + 
						"at least one assembly entry");
				}
			}
			if (config.CustomFilterFactory != null)
			{
				ValidateType(config.CustomFilterFactory, typeof(IFilterFactory));
			}
			if (config.CustomEngineTypeName != null)
			{
				ValidateType(config.CustomEngineTypeName, typeof(IViewEngine));
			}
			if (config.ViewsPhysicalPath == null || config.ViewsPhysicalPath.Length == 0)
			{
				throw new ConfigurationException("You must provide a '" + Views_Node_Name + "' node and a " + 
					"absolute or relative path to the views directory with the attribute " + View_Path_Root);
			}
		}

		private void ValidateType(String name, Type expectedType)
		{
			Type type = Type.GetType(name, false, false);
	
			if (type == null)
			{
				String message = String.Format("Type {0} could not be loaded", name);
				throw new ConfigurationException(message);
			}

			if (!expectedType.IsAssignableFrom(type))
			{
				String message = String.Format("Type {0} does't implement {1}", 
					type.FullName, expectedType.FullName);
				throw new ConfigurationException(message);
			}
		}
	}
}
