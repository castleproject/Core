// Copyright 2004-2011 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.f
// See the License for the specific language governing permissions and
// limitations under the License.

#if FEATURE_DICTIONARYADAPTER_XML
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Xml.Serialization;

	public class XmlCustomSerializer : XmlTypeSerializer
	{
		public static readonly XmlCustomSerializer
			Instance = new XmlCustomSerializer();

		private XmlCustomSerializer() { }

		public override XmlTypeKind Kind
		{
			get { return XmlTypeKind.Complex; }
		}

		public override object GetValue(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor)
		{
            var serializable = (IXmlSerializable) Activator.CreateInstance(node.ClrType);

			using (var reader = new XmlSubtreeReader(node, XmlDefaultSerializer.Root))
			{
				// Do NOT pre-read containing element
				// ...IXmlSerializable is not a symmetric contract
				serializable.ReadXml(reader);
			}

            return serializable;
		}

		public override void SetValue(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor, object oldValue, ref object value)
		{
		    var serializable = (IXmlSerializable) value;
			var root = XmlDefaultSerializer.Root;

			using (var writer = new XmlSubtreeWriter(node))
			{
				// Pre-write containing element
				writer.WriteStartElement(string.Empty, root.ElementName, root.Namespace);
				serializable.WriteXml(writer);
				writer.WriteEndElement();
			}
		}
	}
}
#endif
