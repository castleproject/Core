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

namespace Castle.Windsor.Configuration.Interpreters
{
	using System;
	using System.Xml;
	using System.Configuration;

	using Castle.Model.Configuration;

	using Castle.MicroKernel;

	/// <summary>
	/// Reads the configuration from a XmlFile. Example structure:
	/// <code>
	/// &lt;configuration&gt;
	///   &lt;facilities&gt;
	///     &lt;facility id="myfacility"&gt;
	///     
	///     &lt;/facility&gt;
	///   &lt;/facilities&gt;
	///   
	///   &lt;components&gt;
	///     &lt;component id="component1"&gt;
	///     
	///     &lt;/component&gt;
	///   &lt;/components&gt;
	/// &lt;/configuration&gt;
	/// </code>
	/// </summary>
	public class XmlInterpreter : AbstractInterpreter
	{
		private static readonly String FacilitiesNodeName = "facilities";
		private static readonly String FacilitieNodeName = "facility";
		private static readonly String ComponentsNodeName = "components";
		private static readonly String ComponentNodeName = "component";

		public XmlInterpreter()
		{
		}

		public XmlInterpreter(string filename) : base(filename)
		{
		}

		public XmlInterpreter(IConfigurationSource source) : base(source)
		{
		}

		#region IConfigurationInterpreter Members

		public override void Process(IConfigurationStore store)
		{
			if (store == null) throw new ArgumentNullException("store");

			XmlDocument doc = new XmlDocument();
			
			doc.Load(Source.Contents);
			
			Deserialize(doc.DocumentElement, store);
		}

		#endregion

		protected void Deserialize(XmlNode section, IConfigurationStore store)
		{
			foreach(XmlNode node in section)
			{
				if (node.NodeType != XmlNodeType.Element)
				{
					continue;
				}

				if (FacilitiesNodeName.Equals(node.Name))
				{
					DeserializeFacilities( node.ChildNodes, store );
				}
				else if (ComponentsNodeName.Equals(node.Name))
				{
					DeserializeComponents( node.ChildNodes, store );
				}
			}
		}

		private void DeserializeFacilities(XmlNodeList nodes, IConfigurationStore store)
		{
			foreach(XmlNode node in nodes)
			{
				if (node.NodeType != XmlNodeType.Element)
				{
					continue;
				}
				if (!FacilitieNodeName.Equals(node.Name))
				{
					String message = String.Format("Unexpected node under '{0}': Expected '{1}' but found '{2}'", 
						FacilitiesNodeName, FacilitieNodeName, node.Name);

					throw new ConfigurationException(message);
				}

				DeserializeFacility(node, store);
			}
		}

		private void DeserializeFacility(XmlNode node, IConfigurationStore store)
		{
			String facilityId = GetRequiredAttributeValue(node, "id");
			
			IConfiguration config = DeserializeNode(node);

			store.AddFacilityConfiguration(facilityId, config);
		}

		private void DeserializeComponents(XmlNodeList nodes, IConfigurationStore store)
		{
			foreach(XmlNode node in nodes)
			{
				if (node.NodeType != XmlNodeType.Element)
				{
					continue;
				}
				if (!ComponentNodeName.Equals(node.Name))
				{
					String message = String.Format("Unexpected node under '{0}': Expected '{1}' but found '{2}'", 
						ComponentsNodeName, ComponentNodeName, node.Name);

					throw new ConfigurationException(message);
				}

				DeserializeComponent(node, store);
			}
		}

		private void DeserializeComponent(XmlNode node, IConfigurationStore store)
		{
			String componentId = GetRequiredAttributeValue(node, "id");
			
			IConfiguration config = DeserializeNode(node);

			store.AddComponentConfiguration(componentId, config);
		}

		private IConfiguration DeserializeNode(XmlNode node)
		{
			MutableConfiguration config = null;

			if (node.HasChildNodes)
			{
				foreach(XmlNode child in node.ChildNodes)
				{
					if (child.NodeType == XmlNodeType.Text || child.NodeType == XmlNodeType.CDATA)
					{
						config = new MutableConfiguration(node.Name, child.Value.Trim());
						break;
					}
				}
			}
			
			if (config == null)
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
			String value = GetAttributeValue(node);
						
			if (value == null || value.Length == 0)
			{
				String message = String.Format("Required attribute {0} was not found in node {1}", 
					attName, node.Name);

				throw new ConfigurationException(message);
			}

			return value;
		}

		private String GetAttributeValue(XmlNode node)
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
