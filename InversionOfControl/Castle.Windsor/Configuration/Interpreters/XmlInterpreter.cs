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
		private static Regex PropertyValidationRegExp = new Regex( @"(\#\{\s*((?:\w|\.)+)\s*\})", RegexOptions.Compiled);
		private IDictionary properties = new HybridDictionary();

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
			using (source)
			{
				XmlDocument doc = new XmlDocument();
			
				doc.Load( source.GetStreamReader() );
			
				Deserialize(doc.DocumentElement, store);
			}
		}

		#region Deserialization methods

		protected void Deserialize(XmlNode section, IConfigurationStore store)
		{
			foreach(XmlNode node in section)
			{
				if (node.NodeType != XmlNodeType.Element)
				{
					continue;
				}

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
			}
		}

		private void DeserializeProperties( XmlNodeList nodes)
		{
			foreach(XmlNode node in nodes)
			{
				if (node.NodeType != XmlNodeType.Element)
				{
					continue;
				}
				
				// Note that new properties values override old ones!
				// properties values can reference another properties

				String value = EvalProperty( 
					GetPreserveSpaceValue(node) ? node.InnerText : node.InnerText.Trim() );

				properties[node.Name] = value;
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
				if (!FacilityNodeName.Equals(node.Name))
				{
					String message = String.Format("Unexpected node under '{0}': Expected '{1}' but found '{2}'", 
						FacilitiesNodeName, FacilityNodeName, node.Name);

					throw new ConfigurationException(message);
				}

				DeserializeFacility(node, store);
			}
		}

		private void DeserializeFacility(XmlNode node, IConfigurationStore store)
		{
			String id = GetRequiredAttributeValue(node, "id");
			
			IConfiguration config = DeserializeNode(node);

			AddFacilityConfig(id, config, store);
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
			String id = GetRequiredAttributeValue(node, "id");
			
			IConfiguration config = DeserializeNode(node);

			AddComponentConfig(id, config, store);
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
						config = new MutableConfiguration(node.Name, EvalProperty(child.Value.Trim()) );
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
				config.Attributes.Add(attribute.Name, EvalProperty( attribute.Value ) );
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

		private string EvalProperty(string value)
		{
			MatchCollection matches = PropertyValidationRegExp.Matches( value );

			if(matches.Count > 0)
			{
				StringBuilder buffer = new StringBuilder(value);
				
				foreach(Match match in matches)
				{
					string propRef = match.Groups[1].Value; 
					string propKey = match.Groups[2].Value;
					string propValue = properties[propKey] as string;

					// if a property is not found we replace its value with an empty string
					if( propValue == null ) propValue = String.Empty;
					
					buffer.Replace( propRef, propValue );
				}

				value = buffer.ToString();
			}

			return value;
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

		private bool GetPreserveSpaceValue(XmlNode node)
		{
			XmlAttribute att = node.Attributes["preserve_spaces"];

			if (att == null)
			{
				return false;
			}

			return att.Value.ToLower() == "true";
		}
		
		private void ProcessInclude(XmlNode includeNode, IConfigurationStore store)
		{
			XmlAttribute resourceUriAtt = includeNode.Attributes["uri"];

			if (resourceUriAtt == null)
			{
				// TODO: Throw proper Exception
			}

			ProcessInclude( resourceUriAtt.Value, store );
		}

		#endregion
	}
}
