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

namespace Castle.CastleOnRails.Engine.Configuration
{
	using System;
	using System.Xml;
	using System.Configuration;

	/// <summary>
	/// Summary description for RailsSectionHandler.
	/// </summary>
	public class RailsSectionHandler : IConfigurationSectionHandler
	{
		private static readonly String Controllers_Node_Name = "controllers";
		private static readonly String Views_Node_Name = "views";
		private static readonly String Custom_Controller_Factory_Node_Name = "customControllerFactory";

		public object Create(object parent, object configContext, XmlNode section)
		{
			GeneralConfiguration config = new GeneralConfiguration();

			foreach( XmlNode node in section.ChildNodes)
			{
				if (node.NodeType != XmlNodeType.Element)
				{
					continue;
				}

				if ( String.Compare(Controllers_Node_Name, node.Name, true) == 0 )
				{
					XmlAttribute assemblyControllers = node.Attributes["assembly"];

					if (assemblyControllers == null)
					{
						throw new ConfigurationException("The controllers node must specify an assembly attribute");
					}

					config.ControllersAssembly = assemblyControllers.Value;
				}
				else if ( String.Compare(Views_Node_Name, node.Name, true) == 0 )
				{
					XmlAttribute path = node.Attributes["path"];

					if (path == null)
					{
						throw new ConfigurationException("The views node must specify a path attribute");
					}

					config.ViewsPhysicalPath = path.Value;
				}
				else if ( String.Compare(Custom_Controller_Factory_Node_Name, node.Name, true) == 0 )
				{
					XmlAttribute type = node.Attributes["type"];

					if (type == null)
					{
						throw new ConfigurationException("The custom controller factory node must specify a type attribute");
					}

					config.CustomControllerFactory = type.Value;
				}
			}

			Validate(config);

			return config;
		}

		protected virtual void Validate(GeneralConfiguration config)
		{
			if (config.CustomControllerFactory != null)
			{
				Type type = Type.GetType(config.CustomControllerFactory, false, false);

				if (type == null)
				{
					throw new ConfigurationException("Type for CustomControllerFactory could not be obtained.");
				}
			}
			else if (config.ControllersAssembly == null || config.ControllersAssembly.Length == 0)
			{
				throw new ConfigurationException("The assembly for Rails inspect for controllers must be specified in the controllers node.");
			}
		}
	}
}
