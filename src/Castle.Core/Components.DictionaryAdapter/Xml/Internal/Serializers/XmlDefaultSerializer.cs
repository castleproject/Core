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

	public class XmlDefaultSerializer : XmlTypeSerializer
	{
		private readonly XmlSerializer serializer;

		public XmlDefaultSerializer(Type type)
		{
			serializer = new XmlSerializer(type, Root);
		}

		public override XmlTypeKind Kind
		{
			get { return XmlTypeKind.Complex; }
		}

		public override object GetValue(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor)
		{
            using (var reader = new XmlSubtreeReader(node, Root))
                return serializer.CanDeserialize(reader)
                    ? serializer.Deserialize(reader)
                    : null;
		}

		public override void SetValue(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor, object oldValue, ref object value)
		{
		    using (var writer = new XmlSubtreeWriter(node))
		        serializer.Serialize(writer, value);
		}

		public static readonly XmlRootAttribute
			Root = new XmlRootAttribute
		{
			ElementName = "Root",
			Namespace   = string.Empty
		};
	}
}
#endif
