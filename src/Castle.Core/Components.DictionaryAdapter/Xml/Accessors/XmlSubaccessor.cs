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

	public class XmlSubaccessor : XmlAccessor
	{
		private readonly XmlAccessor accessor;
		private IXmlTypeMap knownTypes;

		public XmlSubaccessor(XmlAccessor accessor, Type clrType)
			: base(clrType, null)
		{
			this.accessor = accessor;
			this.knownTypes = null;
		}

		public override string LocalName
		{
			get { throw new NotImplementedException(); }
		}

		public override string NamespaceUri
		{
			get { throw new NotImplementedException(); }
		}

		public override IXmlTypeMap KnownTypes
		{
			get { return knownTypes ?? accessor.KnownTypes; }
//			internal set { knownTypes = value; }
		}

		public override IXmlCollectionAccessor GetCollectionAccessor(Type itemType)
		{
			return new XmlElementBehaviorAccessor(itemType);
		}

		public override IXmlCursor SelectPropertyNode(IXmlNode node, bool mutable)
		{
			return accessor.SelectPropertyNode(node, mutable);
		}

		public override IXmlCursor SelectCollectionNode(IXmlNode node, bool mutable)
		{
			return accessor.SelectCollectionNode(node, mutable);
		}

		public override IXmlCursor SelectCollectionItems(IXmlNode node, bool mutable)
		{
			return accessor.SelectCollectionItems(node, mutable);
		}
	}
}
