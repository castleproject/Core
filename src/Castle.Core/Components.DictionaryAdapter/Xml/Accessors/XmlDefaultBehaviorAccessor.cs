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

namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Xml;

	public class XmlDefaultBehaviorAccessor : XmlNodeAccessor
	{
		internal static readonly XmlAccessorFactory<XmlDefaultBehaviorAccessor>
			Factory = (property, context) => new XmlDefaultBehaviorAccessor(property, context);

		public XmlDefaultBehaviorAccessor(Type type, IXmlAccessorContext context)
			: base(type, context) { }

		public XmlDefaultBehaviorAccessor(PropertyDescriptor property, IXmlAccessorContext context)
			: base(property, context) { }

		public override IXmlCursor SelectPropertyNode(IXmlNode node, bool mutable)
		{
			var flags = Serializer.Kind == XmlTypeKind.Simple
				? CursorFlags.AllNodes
				: CursorFlags.Elements;
			return node.SelectChildren(KnownTypes, flags.MutableIf(mutable));
		}

		public override IXmlCursor SelectCollectionNode(IXmlNode node, bool mutable)
		{
			return SelectPropertyNode(node, mutable);
		}

		public override IXmlCursor SelectCollectionItems(IXmlNode node, bool mutable)
		{
			var flags = CursorFlags.Elements | CursorFlags.Multiple;
			return node.SelectChildren(KnownTypes, flags.MutableIf(mutable));
		}
	}
}
