using System.Configuration;
using Castle.Model.Configuration;
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

namespace Castle.Windsor.Configuration.Xml
{
	using System;
	using System.IO;
	using System.Xml;

	using Castle.MicroKernel.SubSystems.Configuration;

	/// <summary>
	/// Summary description for XmlConfigurationStore.
	/// </summary>
	public class XmlConfigurationStore : DefaultConfigurationStore
	{
		const string FacilitiesNodeName = "facilities";
		const string ComponentsNodeName = "components";

		public XmlConfigurationStore( String filename )
		{
			Deserialize(filename);
		}

		protected XmlConfigurationStore()
		{
		}

		protected void Deserialize(String filename)
		{
			String fullFilename = null;

			if (Path.IsPathRooted(filename))
			{
				fullFilename = filename;
			}
			else
			{
				fullFilename = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filename);
			}

			using(StreamReader streamReader = File.OpenText(fullFilename))
			{
				XmlDocument dom = new XmlDocument();
				dom.Load(streamReader);

				Deserialize(dom.DocumentElement);
			}
		}

		protected void Deserialize(XmlNode section)
		{
			foreach(XmlNode node in section)
			{
				if (node.NodeType != XmlNodeType.Element)
				{
					continue;
				}

				if (FacilitiesNodeName.Equals(node.Name))
				{
					DeserializeFacilities( node.ChildNodes );
				}
				else if (ComponentsNodeName.Equals(node.Name))
				{
					DeserializeComponents( node.ChildNodes );
				}
			}
		}

		private void DeserializeFacilities(XmlNodeList nodes)
		{
			foreach(XmlNode node in nodes)
			{
				if (node.NodeType != XmlNodeType.Element)
				{
					continue;
				}

				DeserializeFacility(node);
			}
		}

		private void DeserializeFacility(XmlNode node)
		{
			String facilityId = GetRequiredAttributeValue(node, "id");
			
			IConfiguration config = DeserializeNode(node);

			base.AddFacilityConfiguration(facilityId, config);
		}

		private void DeserializeComponents(XmlNodeList nodes)
		{
			foreach(XmlNode node in nodes)
			{
				if (node.NodeType != XmlNodeType.Element)
				{
					continue;
				}

				DeserializeComponent(node);
			}
		}

		private void DeserializeComponent(XmlNode node)
		{
			String componentId = GetRequiredAttributeValue(node, "id");
			
			IConfiguration config = DeserializeNode(node);

			base.AddComponentConfiguration(componentId, config);
		}

		private IConfiguration DeserializeNode(XmlNode node)
		{
			MutableConfiguration config;

			if (node.HasChildNodes && node.FirstChild.NodeType == XmlNodeType.Text)
			{
				config = new MutableConfiguration(node.Name, node.FirstChild.Value);
			}
			else
			{
				config = new MutableConfiguration(node.Name);				
			}

			foreach(XmlAttribute attribute in node.Attributes)
			{
				config.Attributes.Add(attribute.Name, attribute.Value);
			}

			if (node.HasChildNodes)
			{
				foreach(XmlNode child in node.ChildNodes)
				{
					if (child.NodeType != XmlNodeType.Element)
					{
						continue;
					}

					config.Children.Add( DeserializeNode(child) );
				}
			}

			return config;
		}

		private String GetRequiredAttributeValue(XmlNode node, String attName)
		{
			String value = GetAttributeValue(node, attName);
						
			if (String.Empty.Equals(value))
			{
				String message = String.Format("Required attribute {0} was not found in node {1}", 
					attName, node.Name);

				throw new ConfigurationException(message);
			}

			return value;
		}

		private String GetAttributeValue(XmlNode node, String attName)
		{
			XmlAttribute att = node.Attributes["id"];

			if (att == null)
			{
				return String.Empty;
			}

			return att.Value;
		}
	}
}
