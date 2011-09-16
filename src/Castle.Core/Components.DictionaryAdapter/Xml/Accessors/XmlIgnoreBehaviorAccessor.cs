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

	public class XmlIgnoreBehaviorAccessor : XmlAccessor
	{
		public static readonly XmlIgnoreBehaviorAccessor
			Instance = new XmlIgnoreBehaviorAccessor();

		private XmlIgnoreBehaviorAccessor()
			: base(typeof(object), null) { }

		public override bool IsIgnored
		{
			get { return true; }
		}

		public override IXmlCollectionAccessor GetCollectionAccessor(Type itemType)
		{
			throw Error.NotSupported();
		}

		public override IXmlCursor SelectPropertyNode(IXmlNode node, bool mutable)
		{
			throw Error.NotSupported();
		}

		public override IXmlCursor SelectCollectionNode(IXmlNode node, bool mutable)
		{
			throw Error.NotSupported();
		}

		public override IXmlCursor SelectCollectionItems(IXmlNode node, bool mutable)
		{
			throw Error.NotSupported();
		}
	}
}
