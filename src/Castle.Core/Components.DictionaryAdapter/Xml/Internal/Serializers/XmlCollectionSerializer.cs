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

	public abstract class XmlCollectionSerializer : XmlTypeSerializer
	{
		protected XmlCollectionSerializer() { }

		public override XmlTypeKind Kind
		{
			get { return XmlTypeKind.Collection; }
		}

		public override bool CanGetStub
		{
			get { return true; }
		}

		public abstract Type ListTypeConstructor
		{
			get; // generic type constructor
		}

		public override object GetStub(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor)
		{
			return GetValueCore(node, parent, accessor);
		}

		public override object GetValue(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor)
		{
			return GetValueCore(node.Save(), parent, accessor);
		}

		private object GetValueCore(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor)
		{
			var itemType    = node.ClrType.GetGenericArguments()[0];
			var listType    = ListTypeConstructor.MakeGenericType(itemType);
			var subaccessor = accessor.GetCollectionAccessor(itemType);
			return Activator.CreateInstance(listType, node, parent, subaccessor);
		}

		public override void SetValue(IXmlNode node, IDictionaryAdapter parent, IXmlAccessor accessor, object oldValue, ref object value)
		{
			var current = value as IXmlNodeSource;
			if (current != null && current.Node.PositionEquals(node))
				return;

			var newItems = value as IEnumerable;
			if (newItems == null)
				throw Error.NotSupported();

			var oldCollection = oldValue as ICollectionProjection;
			if (oldCollection != null)
				oldCollection.ClearReferences();

			var newCollection = (ICollectionProjection) GetValue(node, parent, accessor);
			newCollection.Replace(newItems);
			value = newCollection;
		}
	}
}
#endif
