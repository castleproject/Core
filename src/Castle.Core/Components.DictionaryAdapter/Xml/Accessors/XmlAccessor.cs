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

	public abstract class XmlAccessor : IXmlPropertyAccessor, IXmlCollectionAccessor
	{
		private readonly Type clrType;
		private readonly XmlName xsiType;
		private readonly XmlTypeSerializer serializer;
		private readonly IXmlAccessorContext context;

		protected XmlAccessor(Type clrType, IXmlAccessorContext context)
		{
			if (clrType == null)
				throw Error.ArgumentNull("clrType");
			if (context == null)
				throw Error.ArgumentNull("context");

			clrType = clrType.NonNullable();
			this.clrType    = clrType;
			this.xsiType    = context.GetDefaultXsiType(clrType);
			this.serializer = XmlTypeSerializer.For(clrType);
			this.context    = context;
		}

		public Type ClrType
		{
			get { return clrType; }
		}

		public XmlName XsiType
		{
			get { return xsiType; }
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

		public virtual bool IsVolatile
		{
			get { return false; }
		}

		public bool IsCollection
		{
			get { return serializer.Kind == XmlTypeKind.Collection; }
		}

		public IXmlAccessorContext Context
		{
			get { return context; }
		}

		public virtual void ConfigureVolatile(bool isVolatile)
		{
			// Do nothing
		}

		public virtual void Prepare()
		{
			// Do nothing
		}

		public virtual bool IsPropertyDefined(IXmlNode parentNode)
		{
			var cursor = IsCollection
				? SelectCollectionNode(parentNode, false)
				: SelectPropertyNode  (parentNode, false);

			return cursor.MoveNext();
		}

		public virtual object GetPropertyValue(IXmlNode parentNode, IDictionaryAdapter parentObject, bool orStub)
		{
			if (orStub) orStub &= serializer.CanGetStub;

			var cursor = IsCollection
				? SelectCollectionNode(parentNode, orStub)
				: SelectPropertyNode  (parentNode, orStub);

			if (cursor.MoveNext() && !cursor.IsNil)
				return serializer.GetValue(cursor, parentObject, this);
			if (orStub)
				return serializer.GetStub (cursor, parentObject, this);
			return null;
		}

		public virtual void SetPropertyValue(IXmlNode parentNode, IDictionaryAdapter parentObject, ref object value)
		{
			var cursor = IsCollection
				? SelectCollectionNode(parentNode, true)
				: SelectPropertyNode  (parentNode, true);

			if (null != value)
			{
				cursor.MakeNext(value.GetComponentType());
				Serializer.SetValue(cursor, parentObject, this, ref value);
			}
			else
			{
				SetPropertyToNull(cursor);
			}
		}

		protected virtual void SetPropertyToNull(IXmlCursor cursor)
		{
			if (IsNillable)
			{
				cursor.MakeNext(clrType);
				cursor.IsNil = true;
			}
			else
			{
				cursor.RemoveAllNext();
			}
		}

		public void GetCollectionItems(IXmlNode parentNode, IDictionaryAdapter parentObject, IList values)
		{
			var cursor = SelectCollectionItems(parentNode, false);

			while (cursor.MoveNext())
			{
				var value = serializer.GetValue(cursor.Save(), parentObject, this); // TODO: Do Save() in serializer, only when needed
				values.Add(value);
			}
		}

		protected void RemoveCollectionItems(IXmlNode parentNode)
		{
			var itemType = clrType.GetCollectionItemType();
			var accessor = GetCollectionAccessor(itemType);
			var cursor   = accessor.SelectCollectionItems(parentNode, true);
			cursor.RemoveAllNext();
		}

		public virtual IXmlCollectionAccessor GetCollectionAccessor(Type itemType)
		{
			return new XmlDefaultBehaviorAccessor(itemType, Context);
		}

		public virtual IXmlCursor SelectPropertyNode(IXmlNode parentNode, bool mutable)
		{
			throw Error.NotSupported();
		}

		public virtual IXmlCursor SelectCollectionNode(IXmlNode parentNode, bool mutable)
		{
			return SelectPropertyNode(parentNode, mutable);
		}

		public virtual IXmlCursor SelectCollectionItems(IXmlNode parentNode, bool mutable)
		{
			throw Error.NotSupported();
		}
	}
}
