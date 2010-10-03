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

namespace Castle.Components.DictionaryAdapter
{
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
			var rootOverride = new XmlRootAttribute(node.LocalName)
			{
				Namespace = node.NamespaceURI
			};

			var xml = new StringBuilder();
			var settings = new XmlWriterSettings
			{
				OmitXmlDeclaration = true,
				Indent = false
			};
			var namespaces = new XmlSerializerNamespaces();
			namespaces.Add(string.Empty, string.Empty);
			if (string.IsNullOrEmpty(node.NamespaceURI) == false)
			{
				var prefix = result.Context.AddNamespace(node.NamespaceURI);
				namespaces.Add(prefix, node.NamespaceURI);
			}

			var serializer = new XmlSerializer(result.Type, rootOverride);

			using (var writer = XmlWriter.Create(xml, settings))
			{
				serializer.Serialize(writer, value, namespaces);
				writer.Flush();
			}

			node.ReplaceSelf(xml.ToString());

			return true;
		}

		public bool ReadObject(XPathResult result, XPathNavigator node, out object value)
		{
			var rootOverride = new XmlRootAttribute(node.LocalName)
			{
				Namespace = node.NamespaceURI
			};

			var serializer = new XmlSerializer(result.Type, rootOverride);

			using (var reader = node.ReadSubtree())
			{
				reader.MoveToContent();
				if (serializer.CanDeserialize(reader))
				{
					value = serializer.Deserialize(reader);
					return true;
				}				
			}

			value = null;
			return false;
		}
	}
}
