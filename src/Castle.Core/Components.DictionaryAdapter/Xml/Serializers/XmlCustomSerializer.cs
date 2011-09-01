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
	using System.Xml.Serialization;

	public class XmlCustomSerializer : XmlTypeSerializer
	{
		public static readonly XmlCustomSerializer
			Instance = new XmlCustomSerializer();

		private XmlCustomSerializer() { }

		public override bool CanSerializeAsAttribute { get { return false; } }

		public override object GetValue(XmlTypedNode node, IDictionaryAdapter parent, IXmlAccessor accessor)
		{
            var serializable = (IXmlSerializable) Activator.CreateInstance(node.Type);

            using (var reader = node.Node.ReadSubtree())
            {
                reader.MoveToContent();
                serializable.ReadXml(reader);
            }

            return serializable;
		}

		public override void SetValue(XmlTypedNode node, IXmlAccessor accessor, object value)
		{
		    var serializable = (IXmlSerializable) value;
		    node.Node.DeleteChildren();
		    using (var writer = node.Node.AppendChild())
		        serializable.WriteXml(writer);
		}
	}
}
#endif
