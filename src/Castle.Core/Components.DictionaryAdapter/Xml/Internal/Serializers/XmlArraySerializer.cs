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
	using System.Collections.Generic;

	public class XmlArraySerializer : XmlTypeSerializer
	{
		public static readonly XmlArraySerializer
			Instance = new XmlArraySerializer();

		protected XmlArraySerializer() { }

		public override XmlTypeKind Kind
		{
			get { return XmlTypeKind.Collection; }
		}

		public override bool CanGetStub
		{
			get { return true; }
		}

		public override object GetStub(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor)
		{
			var itemType = node.ClrType.GetElementType();

			return Array.CreateInstance(itemType, 0);
		}

		public override object GetValue(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor)
		{
			var items      = new ArrayList();
			var itemType   = node.ClrType.GetElementType();
			var references = XmlAdapter.For(parent).References;

			accessor
				.GetCollectionAccessor(itemType)
				.GetCollectionItems(node, parent, references, items);

			return items.ToArray(itemType);
		}

		public override void SetValue(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor, object oldValue, ref object value)
		{
			var source      = (Array) value;
			var target      = (Array) null;
			var originals   = (Array) oldValue;
			var itemType    = source.GetType().GetElementType();
			var subaccessor = accessor.GetCollectionAccessor(itemType);
			var cursor      = subaccessor.SelectCollectionItems(node, true);
			var serializer  = subaccessor.Serializer;
			var references  = XmlAdapter.For(parent).References;

			for (var i = 0; i < source.Length; i++)
			{
				var originalItem = GetItemSafe(originals, i);
				var providedItem = source.GetValue(i);
				var assignedItem = providedItem;

				subaccessor.SetValue(cursor, parent, references, cursor.MoveNext(), originalItem, ref assignedItem);

				if (target != null)
				{
					target.SetValue(assignedItem, i);
				}
				else if (!Equals(assignedItem, providedItem))
				{
					target = Array.CreateInstance(itemType, source.Length);
					Array.Copy(source, target, i);
					target.SetValue(assignedItem, i);
				}
			}

			cursor.RemoveAllNext();

			if (target != null)
				value = target;
		}

		private static object GetItemSafe(Array array, int index)
		{
			return array != null && index >= 0 && index < array.Length
				? array.GetValue(index)
				: null;
		}
	}
}
#endif
