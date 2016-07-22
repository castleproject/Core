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
	using System.Collections.Generic;
	using System.Xml.Serialization;

	public class XmlElementBehaviorAccessor : XmlNodeAccessor,
		IConfigurable<XmlElementAttribute>,
		IXmlBehaviorSemantics <XmlElementAttribute>
	{
		private ItemAccessor itemAccessor;
		private List<XmlElementAttribute> attributes;

		internal static readonly XmlAccessorFactory<XmlElementBehaviorAccessor>
			Factory = (name, type, context) => new XmlElementBehaviorAccessor(name, type, context);

		public XmlElementBehaviorAccessor(string name, Type type, IXmlContext context)
			: base(name, type, context) { }

		public void Configure(XmlElementAttribute attribute)
		{
			if (attribute.Type == null)
			{
				ConfigureLocalName   (attribute.ElementName);
				ConfigureNamespaceUri(attribute.Namespace  );
				ConfigureNillable    (attribute.IsNullable );
			}
			else
			{
				if (attributes == null)
					attributes = new List<XmlElementAttribute>();
				attributes.Add(attribute);
			}
		}

		public override void Prepare()
		{
			if (attributes != null)
			{
				ConfigureKnownTypesFromAttributes(attributes, this);
				attributes = null;				
			}
			base.Prepare();
		}

		public override void SetValue(IXmlCursor cursor, IDictionaryAdapter parentObject, XmlReferenceManager references,
			bool hasCurrent, object oldValue, ref object newValue)
		{
			if (newValue == null && IsCollection)
				base.RemoveCollectionItems(cursor, references, oldValue);
			else
				base.SetValue(cursor, parentObject, references, hasCurrent, oldValue, ref newValue);
		}

		public override IXmlCollectionAccessor GetCollectionAccessor(Type itemType)
		{
			return itemAccessor ?? (itemAccessor = new ItemAccessor(this));
		}

		public override IXmlCursor SelectPropertyNode(IXmlNode node, bool mutable)
		{
			return node.SelectChildren(KnownTypes, Context, CursorFlags.Elements.MutableIf(mutable));
		}

		public override IXmlCursor SelectCollectionNode(IXmlNode node, bool mutable)
		{
			return node.SelectSelf(ClrType);
		}

		public string GetLocalName(XmlElementAttribute attribute)
		{
			return attribute.ElementName;
		}

		public string GetNamespaceUri(XmlElementAttribute attribute)
		{
			return attribute.Namespace;
		}

		public Type GetClrType(XmlElementAttribute attribute)
		{
			return attribute.Type;
		}

		private class ItemAccessor : XmlNodeAccessor
		{
			public ItemAccessor(XmlNodeAccessor parent)
				: base(parent.ClrType.GetCollectionItemType(), parent.Context)
			{
				ConfigureLocalName   (parent.Name.LocalName   );
				ConfigureNamespaceUri(parent.Name.NamespaceUri);
				ConfigureNillable    (parent.IsNillable       );
				ConfigureReference   (parent.IsReference      );
				ConfigureKnownTypesFromParent(parent);
			}

			public override void Prepare()
			{
				// Don't prepare; parent already did it
			}

			public override IXmlCursor SelectCollectionItems(IXmlNode node, bool mutable)
			{
				return node.SelectChildren(KnownTypes, Context, CursorFlags.Elements.MutableIf(mutable) | CursorFlags.Multiple);
			}
		}
	}
}
#endif
