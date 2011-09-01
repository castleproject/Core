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
	using System.Linq;
	using System.Xml;
	using System.Xml.Serialization;
	using System.Xml.XPath;

	public class XmlElementBehaviorAccessor : XmlNodeAccessor,
		IConfigurable<XmlElementAttribute>
	{
		private string localName;
		private string namespaceUri;
		private XmlKnownTypeSet knownTypes;
		private bool configured;

		internal static readonly XmlAccessorFactory<XmlElementBehaviorAccessor>
			Factory = (property, knownTypes) => new XmlElementBehaviorAccessor(property, knownTypes);

		public XmlElementBehaviorAccessor(Type type)
			: base(type, null)
		{
			this.localName = type.GetLocalName();
		}

		//public XmlElementBehaviorAccessor(Type type, XmlKnownTypeSet knownTypes)
		//    : base(type, knownTypes)
		//{
		//    this.localName  = type.GetLocalName();
		//    this.knownTypes = knownTypes;
		//}

		public XmlElementBehaviorAccessor(PropertyDescriptor property, IXmlKnownTypeMap knownTypes)
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
			get { return (IXmlKnownTypeMap) knownTypes ?? base.KnownTypes; }
		}

		public void Configure(XmlElementAttribute attribute)
		{
			if (attribute.Type == null)
			{
				if (configured)
					throw Error.AttributeConflict(null);
				if (!string.IsNullOrEmpty(attribute.ElementName))
					localName = attribute.ElementName;
				if (!string.IsNullOrEmpty(attribute.Namespace))
					namespaceUri = attribute.Namespace;
				configured = true;
			}
			else
			{
				if (knownTypes == null)
					knownTypes = new XmlKnownTypeSet();
				knownTypes.Add(attribute);
			}
		}

		public override void Prepare()
		{
			if (knownTypes != null &&
				knownTypes.Any(t => t.ClrType == null))
				knownTypes.Parent = this;
		}

		public override IXmlCollectionAccessor GetCollectionAccessor(Type itemType)
		{
			return new XmlSubaccessor(this, itemType);
		}

		protected internal override XmlIterator SelectPropertyNode(XPathNavigator node, bool create)
		{
			return new XmlElementIterator(node, KnownTypes, false);
		}

		protected internal override XmlIterator SelectCollectionNode(XPathNavigator node, bool create)
		{
			return SelectSelf(node);
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
