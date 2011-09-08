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
	using System.Collections;

	public class XmlListSerializer : XmlCollectionSerializer
	{
		public static readonly XmlListSerializer
			Instance = new XmlListSerializer();

		protected XmlListSerializer() { }

		public override bool CanGetStub { get { return true; } }

		public override object GetStub(IXmlCursor cursor, IDictionaryAdapter parent, IXmlAccessor accessor)
		{
			return GetValue((IXmlNode) cursor, parent, accessor);
		}

		public override object GetValue(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor)
		{
			var itemType    = node.ClrType.GetGenericArguments()[0];
			var listType    = typeof(XmlNodeList<>).MakeGenericType(itemType);
			var subaccessor = accessor.GetCollectionAccessor(itemType);
			return Activator.CreateInstance(listType, node, parent, subaccessor);
		}

		public override void SetValue(IXmlNode node, IXmlAccessor accessor, object value)
		{
			var itemType    = value.GetType().GetGenericArguments()[0];
			var subaccessor = accessor.GetCollectionAccessor(itemType);
			subaccessor.SetCollectionItems(node, (IEnumerable) value);
		}
	}
}
