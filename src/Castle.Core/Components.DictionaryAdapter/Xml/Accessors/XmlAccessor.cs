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

	public abstract class XmlAccessor : XmlType, IXmlPropertyAccessor, IXmlCollectionAccessor
	{
		private readonly Type clrType;
		private readonly XmlTypeSerializer serializer;
		private readonly IXmlTypeMap knownTypes;

		protected XmlAccessor(Type type, IXmlTypeMap knownTypes)
		{
			this.clrType    = type.NonNullable();
			this.serializer = XmlTypeSerializer.For(clrType);
			this.knownTypes = knownTypes ?? DefaultXmlKnownTypeSet.Instance;
		}

		public override Type ClrType
		{
			get { return clrType; }
		}

		public virtual IXmlTypeMap KnownTypes
		{
			get { return knownTypes; }
		}

		public XmlTypeSerializer Serializer
		{
			get { return serializer; }
		}

		public virtual bool IsNillable
		{
			get { return false; }
		}

		public virtual bool IsIgnored
		{
			get { return false; }
		}

		public bool IsVolatile { get; internal set; }

		public virtual void Prepare() { }

		public abstract IXmlCollectionAccessor GetCollectionAccessor(Type itemType);
		public abstract IXmlCursor SelectPropertyNode   (IXmlNode node, bool mutable);
		public abstract IXmlCursor SelectCollectionNode (IXmlNode node, bool mutable);
		public abstract IXmlCursor SelectCollectionItems(IXmlNode node, bool mutable);

		public virtual object GetPropertyValue(IXmlNode parentNode, IDictionaryAdapter parentObject, bool orStub)
		{
			if (orStub) orStub &= serializer.CanGetStub;

			var cursor = Serializer.IsCollection
				? SelectCollectionNode(parentNode, orStub)
				: SelectPropertyNode  (parentNode, orStub);

			if (cursor.MoveNext() && !cursor.IsNil)
				return serializer.GetValue(cursor, parentObject, this);
			if (orStub)
				return serializer.GetStub (cursor, parentObject, this);
			return null;
		}

		public virtual void SetPropertyValue(IXmlNode parentNode, object value)
		{
			var cursor = serializer.IsCollection
				? SelectCollectionNode(parentNode, true)
				: SelectPropertyNode  (parentNode, true);

			if (null != value)
			{
				cursor.MakeNext(value.GetType());
				Serializer.SetValue(cursor, this, value);
			}
			else if (IsNillable)
			{
				cursor.MakeNext(clrType);
				cursor.IsNil = true;
			}
			else
			{
				cursor.RemoveToEnd();
			}
		}

		public void GetCollectionItems(IXmlNode parentNode, IDictionaryAdapter parentObject, IList values)
		{
			var cursor = SelectCollectionItems(parentNode, false);

			while (cursor.MoveNext())
			{
				var value = serializer.GetValue(cursor, parentObject, this);
				values.Add(value);
			}
		}

		public void SetCollectionItems(IXmlNode parentNode, IEnumerable collection)
		{
			var cursor = SelectCollectionItems(parentNode, true);

			foreach (var item in collection)
			{
				cursor.MakeNext(item.GetType());
				serializer.SetValue(cursor, this, item);
			}

			cursor.RemoveToEnd();
		}

		//protected override bool IsMatch(IXmlType xmlType)
		//{
		//    return xmlType.HasNameLike(LocalName, NamespaceUri);
		//}

		protected override bool IsMatch(Type clrType)
		{
			return clrType == this.clrType
				|| (serializer.IsCollection && this.clrType.IsAssignableFrom(clrType));
		}
	}
}
