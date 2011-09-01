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

#if !SILVERLIGHT
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Xml;
	using System.Xml.Serialization;
	using System.Xml.XPath;

	public class XmlArrayBehaviorAccessor : XmlNodeAccessor,
		IConfigurable<XmlArrayAttribute>,
		IConfigurable<XmlArrayItemAttribute>
	{
		private string localName;
		private string namespaceUri;
		private XmlKnownTypeSet knownTypes;
		private bool configured;

		internal static readonly XmlAccessorFactory<XmlArrayBehaviorAccessor>
			Factory = (property, knownTypes) => new XmlArrayBehaviorAccessor(property, knownTypes);

		public XmlArrayBehaviorAccessor(PropertyDescriptor property, IXmlKnownTypeMap knownTypes)
			: base(property.PropertyType, knownTypes)
		{
			this.localName = XmlConvert.EncodeLocalName(property.PropertyName);
		}

		public override string LocalName
		{
			get { return localName; }
		}

		public override string NamespaceUri
		{
			get { return namespaceUri; }
		}

		public override IXmlKnownTypeMap KnownTypes
		{
			get { return (IXmlKnownTypeMap) knownTypes ?? this; }
		}

		public void Configure(XmlArrayAttribute attrbute)
		{
			if (configured)
				throw Error.AttributeConflict(null);
			if (!string.IsNullOrEmpty(attrbute.ElementName))
				localName = attrbute.ElementName;
			if (!string.IsNullOrEmpty(attrbute.Namespace))
				namespaceUri = attrbute.Namespace;
			configured = true;
		}

		public void Configure(XmlArrayItemAttribute attribute)
		{
			if (knownTypes == null)
				knownTypes = new XmlKnownTypeSet();
			knownTypes.Add(attribute);
		}

		public override IXmlCollectionAccessor GetCollectionAccessor(Type itemType)
		{
			return new XmlSubaccessor(this, itemType);
		}

		protected internal override XmlIterator SelectPropertyNode(XPathNavigator node, bool create)
		{
			return new XmlElementIterator(node, this, false);
		}

		protected internal override XmlIterator SelectCollectionNode(XPathNavigator node, bool create)
		{
			return new XmlElementIterator(node, this, false);
		}

		protected internal override XmlIterator SelectCollectionItems(XPathNavigator node, bool create)
		{
			return new XmlElementIterator(node, KnownTypes, true);
		}

		protected override bool IsMatch(XPathNavigator node)
		{
			return node.HasNameLike(localName, namespaceUri);
		}
	}
}
#endif
