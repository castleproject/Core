// Copyright 2003-2004 The Apache Software Foundation
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

namespace Apache.Avalon.Framework
{
	using System;
	using System.Collections;
	using System.IO;
	using System.Xml;

	/// <summary>
	/// A Serializer/Deserializer of a <see cref="DefaultConfiguration"/>.
	/// </summary>
	public class DefaultConfigurationSerializer
	{
		/// <summary>
		/// Makes a serialization of a <see cref="DefaultConfiguration"/> instance.
		/// </summary>
		/// <param name="filename">
		/// The File name where <see cref="DefaultConfiguration"/> instance will be
		/// serialized to.
		/// </param>
		/// <param name="configuration">A <see cref="DefaultConfiguration"/> instance to serialize</param>
		public static void Serialize(string filename, DefaultConfiguration configuration)
		{
			XmlTextWriter writer = new XmlTextWriter(new StreamWriter(filename));

			//Use indentation for readability.
			writer.Formatting = Formatting.Indented;
			writer.Indentation = 4;
			
			writer.WriteStartDocument(true);

			Serialize(writer, configuration);				

			writer.WriteEndDocument(); 
			writer.Close(); 
		}

		/// <summary>
		/// Makes a serialization of a <see cref="DefaultConfiguration"/> instance.
		/// </summary>
		/// <param name="writer"></param>
		/// <param name="configuration">A <see cref="DefaultConfiguration"/> instance to serialize.</param>
		public static void Serialize(XmlWriter writer, DefaultConfiguration configuration)
		{
			// serialize the configuration
			writer.WriteStartElement(configuration.Prefix, configuration.Name, configuration.Namespace); 
			
			// attribute serialization
			foreach (DictionaryEntry attr in configuration.Attributes) 
			{
				writer.WriteAttributeString(attr.Key.ToString(), attr.Value.ToString()); 
			}  

			if (configuration.Value != null)
			{
				writer.WriteString(configuration.Value.ToString());
			}

			// child serialization
			foreach(IConfiguration child in configuration.Children)
			{
				Serialize(writer, (DefaultConfiguration) child);
			}

			writer.WriteEndElement();
		}

		/// <summary>
		/// Makes a deserialization of a <see cref="DefaultConfiguration"/> instance.
		/// </summary>
		/// <param name="fileName">The Name of the file, containing the XML document to deserialize.</param>
		/// <returns>A Deserialized <see cref="DefaultConfiguration"/> instance.</returns>
		public static DefaultConfiguration Deserialize(string fileName)
		{
			DefaultConfiguration configuration = null;

			XmlDocument document = new XmlDocument();
			document.Load(fileName);
			
			XmlNode root = document.DocumentElement; 

			if (root != null)
			{
				configuration = Deserialize(root, null);
			}

			return configuration;
		}

		/// <summary>
		/// Makes a deserialization of <see cref="XmlNode"/> instance.
		/// </summary>
		/// <param name="node">The Node to deserialize.</param>
		/// <param name="parent">A Deserialized <see cref="DefaultConfiguration"/> parent instance.</param>
		/// <returns>A Deserialized <see cref="DefaultConfiguration"/> instance.</returns>
		public static DefaultConfiguration Deserialize(XmlNode node, DefaultConfiguration parent)
		{
			// node deserialization
			DefaultConfiguration configuration = null;

			if ((node.NodeType  == XmlNodeType.CDATA) || (node.NodeType == XmlNodeType.Text))
			{
				if (parent != null)
				{
					parent.Value = string.Concat(parent.Value, node.Value);
				}
			}

			if ((node.NodeType  == XmlNodeType.Document) || (node.NodeType == XmlNodeType.Element))
			{
				configuration = new DefaultConfiguration(node.LocalName, "-", node.NamespaceURI, node.Prefix);
	
				// attribute deserialization
				if (node.Attributes != null)
				{
					foreach (XmlAttribute attr in node.Attributes)
					{
						if (string.Compare(attr.Prefix, string.Empty) == 0)
						{
							configuration.Attributes[attr.Name] = attr.Value;  
						}
					}
				}

				// child deserialization
				foreach (XmlNode child in node.ChildNodes)
				{
					DefaultConfiguration childConfiguration = Deserialize(child, configuration);
					
					if (childConfiguration != null)
					{
						configuration.Children.Add(childConfiguration); 
					}
				}
			}

			return configuration;
		}

		/// <summary>
		/// Makes a deserialization of <see cref="XmlNode"/> instance.
		/// </summary>
		/// <param name="node">The Node to deserialize.</param>
		/// <returns>A Deserialized <see cref="DefaultConfiguration"/> instance.</returns>
		public static DefaultConfiguration Deserialize(XmlNode node)
		{
			return Deserialize(node, null);
		}
	}
}
