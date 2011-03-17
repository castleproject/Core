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

namespace Castle.Components.DictionaryAdapter
{
	using System;
	using System.Text;
	using System.Xml;
	using System.Xml.Serialization;
	using System.Xml.XPath;

	public class DefaultXmlSerializer : IXPathSerializer
	{
		public static readonly DefaultXmlSerializer Instance = new DefaultXmlSerializer();

		private DefaultXmlSerializer()
		{
		}

		public bool WriteObject(XPathResult result, XPathNavigator node, object value)
		{
			var xml = new StringBuilder();
			var settings = new XmlWriterSettings
			{
				OmitXmlDeclaration = true,
				Indent = false
			};

			using (var writer = XmlWriter.Create(xml, settings))
			{
				if (value is IXmlSerializable)
				{
					SerializeCustom(writer, result, node, (IXmlSerializable)value);
				}
				else
				{
					Serialize(writer, result, node, value);
				}
				writer.Flush();
			}

			node.ReplaceSelf(xml.ToString());
			return true;
		}

		public bool ReadObject(XPathResult result, XPathNavigator node, out object value)
		{
			using (var reader = node.ReadSubtree())
			{
				reader.MoveToContent();
				if (typeof(IXmlSerializable).IsAssignableFrom(result.Type))
				{
					value = DeserializeCustom(reader, result, node);
				}
				else
				{
					value = Deserialize(reader, result, node);
				}
			}

			return (value != null);
		}

		private static void Serialize(XmlWriter writer, XPathResult result, XPathNavigator node, object value)
		{
			var namespaces = new XmlSerializerNamespaces();
			namespaces.Add(string.Empty, string.Empty);
			if (string.IsNullOrEmpty(node.NamespaceURI) == false)
			{
				var prefix = result.Context.AddNamespace(node.NamespaceURI);
				namespaces.Add(prefix, node.NamespaceURI);
			}

			var rootOverride = new XmlRootAttribute(node.LocalName)
			{
				Namespace = node.NamespaceURI
			};

			var serializer = new XmlSerializer(result.Type, rootOverride);
			serializer.Serialize(writer, value, namespaces);
		}

		private static object Deserialize(XmlReader reader, XPathResult result, XPathNavigator node)
		{
			var rootOverride = new XmlRootAttribute(node.LocalName)
			{
				Namespace = node.NamespaceURI
			};

			var serializer = new XmlSerializer(result.Type, rootOverride);
			reader.MoveToContent();
			if (serializer.CanDeserialize(reader))
			{
				return serializer.Deserialize(reader);
			}

			return null;
		}

		private static void SerializeCustom(XmlWriter writer, XPathResult result, XPathNavigator node, IXmlSerializable value)
		{
			if (string.IsNullOrEmpty(node.NamespaceURI))
			{
				writer.WriteStartElement(node.LocalName);
			}
			else
			{
				var prefix = result.Context.AddNamespace(node.NamespaceURI);
				writer.WriteStartElement(prefix, node.LocalName, node.NamespaceURI);
			}
			value.WriteXml(writer);
			writer.WriteEndElement();
		}

		private static IXmlSerializable DeserializeCustom(XmlReader reader, XPathResult result, XPathNavigator node)
		{
			if (reader.NodeType == XmlNodeType.Element && reader.LocalName == node.LocalName && reader.NamespaceURI == node.NamespaceURI)
			{
				var serializable = (IXmlSerializable)Activator.CreateInstance(result.Type);
				serializable.ReadXml(reader);
				return serializable;
			}
			return null;
		}
	}
}
#endif