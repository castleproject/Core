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

namespace Castle.Windsor.Configuration.Interpreters
{
	using System;
	using System.Xml;
	using System.Collections;
	using System.Collections.Specialized;
	using System.Configuration;
	using System.Text;
	using System.Text.RegularExpressions;

	using Castle.Model.Resource;
	using Castle.Model.Configuration;
	
	using Castle.MicroKernel;

	/// <summary>
	/// Reads the configuration from a XmlFile. Sample structure:
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
		/// <summary>
		/// Properties names can contain a-zA-Z0-9_. 
		/// i.e. #{my_node_name} || #{ my.node.name }
		/// spaces are trimmed
		/// </summary>
		private static readonly Regex PropertyValidationRegExp = new Regex(@"(\#\{\s*((?:\w|\.)+)\s*\})", RegexOptions.Compiled);

		private readonly IDictionary properties = new HybridDictionary();
		private readonly XslContext context = new XslContext();
		private readonly XslProcessor processor = new XslProcessor();
		private readonly StringBuilder buffer = new StringBuilder();

		#region Constructors

		public XmlInterpreter()
		{
		}

		public XmlInterpreter(String filename) : base(filename)
		{
		}

		public XmlInterpreter(Castle.Model.Resource.IResource source) : base(source)
		{
		}

		#endregion

		public override void ProcessResource(IResource source, IConfigurationStore store)
		{
			using(source)
			{
				XmlDocument doc = new XmlDocument();

				doc.Load(source.GetStreamReader());

				doc = processor.Process(doc, context);

				Deserialize(doc.DocumentElement, store);
			}
		}

		#region Deserialization methods

		protected void Deserialize(XmlNode section, IConfigurationStore store)
		{
			foreach(XmlNode node in section)
			{
				if (IsTextNode(node))
				{
					throw new ConfigurationException(String.Format("{0} cannot contain text nodes", node.Name));
				}
				else if (node.NodeType == XmlNodeType.Element)
				{
					DeserializeElement(node, store);
				}
			}
		}

		private void DeserializeElement(XmlNode node, IConfigurationStore store)
		{
			if (IncludeNodeName.Equals(node.Name))
			{
				ProcessInclude(node, store);
			}
			else if (PropertiesNodeName.Equals(node.Name))
			{
				DeserializeProperties(node.ChildNodes);
			}
			else if (FacilitiesNodeName.Equals(node.Name))
			{
				DeserializeFacilities(node.ChildNodes, store);
			}
			else if (ComponentsNodeName.Equals(node.Name))
			{
				DeserializeComponents(node.ChildNodes, store);
			}
			else
			{
				throw new ConfigurationException(String.Format("DeserializeElement cannot process element {0}", node.Name));
			}
		}

		private void DeserializeProperties(XmlNodeList nodes)
		{
			foreach(XmlNode node in nodes)
			{
				if (node.NodeType == XmlNodeType.Element)
				{
					properties[node.Name] = GetDeserializedNode(node);
				}
			}
		}

		private void DeserializeFacilities(XmlNodeList nodes, IConfigurationStore store)
		{
			foreach(XmlNode node in nodes)
			{
				if (node.NodeType == XmlNodeType.Element)
				{
					AssertNodeName(node, FacilityNodeName);

					DeserializeFacility(node, store);
				}
				else if (IsTextNode(node))
				{
					DeserializeString(node.Value, FacilityNodeName, store);
				}
			}
		}

		private void DeserializeFacility(XmlNode node, IConfigurationStore store)
		{
			String id = GetRequiredAttributeValue(node, "id");

			IConfiguration config = GetDeserializedNode(node);

			AddFacilityConfig(id, config, store);
		}

		private void DeserializeComponents(XmlNodeList nodes, IConfigurationStore store)
		{
			foreach(XmlNode node in nodes)
			{
				if (node.NodeType == XmlNodeType.Element)
				{
					AssertNodeName(node, ComponentNodeName);

					DeserializeComponent(node, store);
				}
				else if (IsTextNode(node))
				{
					DeserializeString(node.Value, ComponentsNodeName, store);
				}
			}
		}

		private void DeserializeComponent(XmlNode node, IConfigurationStore store)
		{
			String id = GetRequiredAttributeValue(node, "id");

			IConfiguration config = GetDeserializedNode(node);

			AddComponentConfig(id, config, store);
		}

		private IConfiguration GetDeserializedNode(XmlNode node)
		{
			MutableConfiguration config = null;
			ConfigurationCollection configChilds = new ConfigurationCollection();

			StringBuilder configValue = new StringBuilder();

			if (node.HasChildNodes)
			{
				foreach(XmlNode child in node.ChildNodes)
				{
					if (IsTextNode(child))
					{
						IConfiguration tempConfig = GetDeserializedString(child.Value);
						configValue.Append(tempConfig.Value);
						configChilds.AddRange(tempConfig.Children);
					}
					else if (child.NodeType == XmlNodeType.Element)
					{
						configChilds.Add(GetDeserializedNode(child));
					}
				}
			}

			config = new MutableConfiguration(node.Name, GetConfigValue(configValue.ToString()));

			foreach(XmlAttribute attribute in node.Attributes)
			{
				IConfiguration tempConfig = GetDeserializedString(attribute.Value);

				if (tempConfig.Children.Count > 0)
				{
					throw new ConfigurationException("attribute value cannot reference properties with child elements");
				}

				config.Attributes.Add(attribute.Name, tempConfig.Value);
			}

			config.Children.AddRange(configChilds);

			return config;
		}

		private void DeserializeString(string value, string filterElement, IConfigurationStore store)
		{
			IConfiguration config = GetDeserializedString(value);

			foreach(IConfiguration childConfig in config.Children)
			{
				if (childConfig.Name != filterElement)
				{
					throw new ConfigurationException(String.Format("Expect element {0} found {1}", filterElement, childConfig.Name));
				}
				else
				{
					if (childConfig.Name == FacilityNodeName)
					{
						AssertRequiredAttribute(childConfig, "id", FacilityNodeName);
						store.AddFacilityConfiguration(childConfig.Attributes["id"], childConfig);
					}
					else if (childConfig.Name == ComponentNodeName)
					{
						AssertRequiredAttribute(childConfig, "id", ComponentNodeName);
						store.AddComponentConfiguration(childConfig.Attributes["id"], childConfig);
					}
					else
					{
						throw new ConfigurationException(String.Format("DeserializeString cannot handle element {0}", childConfig.Name));
					}
				}
			}
		}

		private IConfiguration GetDeserializedString(string value)
		{
			buffer.Length = 0;

			ConfigurationCollection children = new ConfigurationCollection();

			int pos = 0;
			Match match;

			while((match = PropertyValidationRegExp.Match(value, pos)).Success)
			{
				if (pos < match.Index)
				{
					buffer.Append(value.Substring(pos, match.Index - pos));
				}

				string propKey = match.Groups[2].Value;

				MutableConfiguration prop = properties[propKey] as MutableConfiguration;

				if (prop != null)
				{
					buffer.Append(prop.Value);
					children.AddRange(prop.Children);
				}

				pos += match.Index + match.Length;
			}

			// appending anything left
			if (pos < value.Length)
			{
				buffer.Append(value.Substring(pos, value.Length - pos));
			}

			IConfiguration result = new MutableConfiguration("", GetConfigValue(buffer.ToString()));

			result.Children.AddRange(children);

			return result;
		}

		#endregion		

		private void ProcessInclude(XmlNode includeNode, IConfigurationStore store)
		{
			string uri = GetRequiredAttributeValue(includeNode, "uri");

			ProcessInclude(uri, store);
		}

		/// <summary>
		/// If a config value is an empty string we return null, this is to keep
		/// backward compability with old code
		/// </summary>
		private string GetConfigValue(string value)
		{
			value = value.Trim();

			return value == String.Empty ? null : value;
		}

		private String GetRequiredAttributeValue(XmlNode node, String attName)
		{
			String value = GetAttributeValue(node, attName);

			if (value.Length == 0)
			{
				String message = String.Format("{0} elements expects required non blank attribute {1}",
				                               node.Name, attName);

				throw new ConfigurationException(message);
			}

			return value;
		}

		private String GetAttributeValue(XmlNode node, String attName)
		{
			XmlAttribute att = node.Attributes[attName];

			return (att == null) ? String.Empty : att.Value.Trim();
		}

		private bool IsTextNode(XmlNode node)
		{
			return node.NodeType == XmlNodeType.Text || node.NodeType == XmlNodeType.CDATA;
		}

		private void AssertRequiredAttribute(IConfiguration config, string attrName, string parentName)
		{
			String content = config.Attributes[attrName];

			if (content == null || content.Trim() == String.Empty)
			{
				throw new ConfigurationException(String.Format("{0} expects {1} attribute", parentName, attrName));
			}
		}

		private void AssertNodeName(XmlNode node, string expectedName)
		{
			if (!expectedName.Equals(node.Name))
			{
				String message = String.Format("Unexpected node under '{0}': Expected '{1}' but found '{2}'",
				                               expectedName, expectedName, node.Name);

				throw new ConfigurationException(message);
			}
		}
	}
}