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

	public class XmlArrayBehaviorAccessor : XmlNodeAccessor,
		IConfigurable<XmlArrayAttribute>,
		IConfigurable<XmlArrayItemAttribute>
	{
		private readonly ItemAccessor itemAccessor;

		internal static readonly XmlAccessorFactory<XmlArrayBehaviorAccessor>
			Factory = (name, type, context) => new XmlArrayBehaviorAccessor(name, type, context);

		public XmlArrayBehaviorAccessor(string name, Type type, IXmlContext context)
			: base(name, type, context)
		{
			if (Serializer.Kind != XmlTypeKind.Collection)
				throw Error.AttributeConflict(name);

			itemAccessor = new ItemAccessor(ClrType.GetCollectionItemType(), this);
		}

		public void Configure(XmlArrayAttribute attribute)
		{
			ConfigureLocalName   (attribute.ElementName);
			ConfigureNamespaceUri(attribute.Namespace  );
			ConfigureNillable    (attribute.IsNullable );
		}

		public void Configure(XmlArrayItemAttribute attribute)
		{
			itemAccessor.Configure(attribute);
		}

		public override void Prepare()
		{
			base.Prepare();
			itemAccessor.Prepare();
		}

		public override IXmlCollectionAccessor GetCollectionAccessor(Type itemType)
		{
			return itemAccessor;
		}

		public override IXmlCursor SelectPropertyNode(IXmlNode node, bool mutable)
		{
			return node.SelectChildren(this, Context, PropertyFlags.MutableIf(mutable));
		}

		private class ItemAccessor : XmlNodeAccessor,
			IConfigurable<XmlArrayItemAttribute>,
			IXmlBehaviorSemantics <XmlArrayItemAttribute>
		{
			private List<XmlArrayItemAttribute> attributes;

			public ItemAccessor(Type itemClrType, XmlNodeAccessor accessor)
				: base(itemClrType, accessor.Context)
			{
				ConfigureNillable(true);
				ConfigureReference(accessor.IsReference);
			}

			public void Configure(XmlArrayItemAttribute attribute)
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
						attributes = new List<XmlArrayItemAttribute>();
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

			public override IXmlCursor SelectCollectionItems(IXmlNode node, bool mutable)
			{
				return node.SelectChildren(KnownTypes, Context, CollectionItemFlags.MutableIf(mutable));
			}

			public string GetLocalName(XmlArrayItemAttribute attribute)
			{
				return attribute.ElementName;
			}

			public string GetNamespaceUri(XmlArrayItemAttribute attribute)
			{
				return attribute.Namespace;
			}

			public Type GetClrType(XmlArrayItemAttribute attribute)
			{
				return attribute.Type;
			}
		}

		private const CursorFlags
			PropertyFlags       = CursorFlags.Elements,
			CollectionItemFlags = CursorFlags.Elements | CursorFlags.Multiple;
	}
}
#endif
