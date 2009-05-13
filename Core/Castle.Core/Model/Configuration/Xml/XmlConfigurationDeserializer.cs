// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

#if !SILVERLIGHT

namespace Castle.Core.Configuration.Xml
{
	using System;
	using System.Text;
	using System.Xml;
	using System.Xml.XPath;


	/// <summary>
	/// Pendent
	/// </summary>
	public class XmlConfigurationDeserializer
	{
		/// <summary>
		/// Deserializes the specified node into an abstract representation of configuration.
		/// </summary>
		/// <param name="node">The node.</param>
		/// <returns></returns>
		public IConfiguration Deserialize(XmlNode node)
		{
			return Deserialize(node);
		}

		public static IConfiguration GetDeserializedNode(XmlNode node)
		{
			ConfigurationCollection configChilds = new ConfigurationCollection();

			StringBuilder configValue = new StringBuilder();

			if (node.HasChildNodes)
			{
				foreach(XmlNode child in node.ChildNodes)
				{
					if (IsTextNode(child))
					{
						configValue.Append(child.Value);
					}
					else if (child.NodeType == XmlNodeType.Element)
					{
						configChilds.Add(GetDeserializedNode(child));
					}
				}
			}

			MutableConfiguration config = new MutableConfiguration(node.Name, GetConfigValue(configValue.ToString()));

			foreach(XmlAttribute attribute in node.Attributes)
			{
				config.Attributes.Add(attribute.Name, attribute.Value);
			}

			config.Children.AddRange(configChilds);

			return config;
		}

		/// <summary>
		/// If a config value is an empty string we return null, this is to keep
		/// backward compability with old code
		/// </summary>
		public static string GetConfigValue(string value)
		{
			return string.IsNullOrEmpty(value) ? null : value.Trim();
		}

		public static bool IsTextNode(XmlNode node)
		{
			return node.NodeType == XmlNodeType.Text || node.NodeType == XmlNodeType.CDATA;
		}
	}
}

#endif