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
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if FEATURE_DICTIONARYADAPTER_XML
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Xml;

	public class XmlXmlNodeSerializer : XmlTypeSerializer
	{
		public static readonly XmlXmlNodeSerializer
			Instance = new XmlXmlNodeSerializer();

		private XmlXmlNodeSerializer() { }

		public override XmlTypeKind Kind
		{
			get { return XmlTypeKind.Complex; }
		}

		public override object GetValue(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor)
		{
			var source = node.AsRealizable<XmlNode>();

			return (source != null && source.IsReal)
				? source.Value
				: null;
		}

		public override void SetValue(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor, object oldValue, ref object value)
		{
			var newNode = (XmlNode) value;

			using (var writer = new XmlSubtreeWriter(node))
				newNode.WriteTo(writer);
		}
	}
}
#endif
