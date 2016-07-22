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
	using System.Collections;

	public class XmlComponentSerializer : XmlTypeSerializer
	{
		public static readonly XmlComponentSerializer
			Instance = new XmlComponentSerializer();

		protected XmlComponentSerializer() { }

		public override XmlTypeKind Kind
		{
			get { return XmlTypeKind.Complex; }
		}

		public override bool CanGetStub
		{
			get { return true; }
		}

		public override object GetStub(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor)
		{
			// TODO: Refactor
			var adapter = new XmlAdapter(node, XmlAdapter.For(parent).References);
			return parent.CreateChildAdapter(accessor.ClrType, adapter);
		}

		public override object GetValue(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor)
		{
			var adapter = new XmlAdapter(node.Save(), XmlAdapter.For(parent).References);
			return parent.CreateChildAdapter(node.ClrType, adapter);
		}

		public override void SetValue(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor, object oldValue, ref object value)
		{
			// Require a dictionary adapter
			var source = value as IDictionaryAdapter;
			if (source == null)
				throw Error.NotDictionaryAdapter("value");

			// Detect assignment of own value
			var sourceAdapter = XmlAdapter.For(source, false);
			if (sourceAdapter != null && node.PositionEquals(sourceAdapter.Node))
				return;

			// Create a fresh component
			var targetAdapter = new XmlAdapter(node.Save(), XmlAdapter.For(parent).References);
			if (sourceAdapter != null)
				targetAdapter.References.UnionWith(sourceAdapter.References);
			var component = (IDictionaryAdapter) parent.CreateChildAdapter(node.ClrType, targetAdapter);

			// Copy value onto fresh component
			source.CopyTo(component);
			value = component;
		}
	}
}
#endif
