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

		public IXmlAccessorContext Context
		{
			get { return context; }
		}

		public XmlTypeSerializer Serializer
		{
			get { return serializer; }
		}

		public bool IsCollection
		{
			get { return serializer.Kind == XmlTypeKind.Collection; }
		}

		public virtual bool IsIgnored
		{
			get { return false; }
		}

		public virtual bool IsNillable
		{
			get { return false; }
		}

		public virtual bool IsVolatile
		{
			get { return false; }
		}

		public virtual void ConfigureNillable(bool isNillable)
		{
			// Do nothing
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

			return GetCursorValue(cursor, parentObject, orStub);
		}

		public object GetCursorValue(IXmlCursor cursor, IDictionaryAdapter parentObject, bool orStub)
		{
			return GetValueCore(cursor, parentObject, cursor.MoveNext(), orStub);
		}

		public object GetNodeValue(IXmlNode node, IDictionaryAdapter parentObject, bool orStub)
		{
			return GetValueCore(node, parentObject, true, orStub);
		}

		private object GetValueCore(IXmlNode node, IDictionaryAdapter parentObject, bool nodeExists, bool orStub)
		{
			if (nodeExists)
				if (!node.IsNil)
					return serializer.GetValue(node, parentObject, this);
				else if (IsNillable)
					return null;

			return orStub
				? serializer.GetStub(node, parentObject, this)
				: null;
		}

		public virtual void SetPropertyValue(IXmlNode parentNode, IDictionaryAdapter parentObject, ref object value)
		{
			var cursor = IsCollection
				? SelectCollectionNode(parentNode, true)
				: SelectPropertyNode  (parentNode, true);

			SetCursorValue(cursor, parentObject, ref value);
		}

		public void SetCursorValue(IXmlCursor cursor, IDictionaryAdapter parentObject, ref object value)
		{
			if (null != value)
			{
				cursor.MakeNext(value.GetComponentType());
				Serializer.SetValue(cursor, parentObject, this, ref value);
			}
			else SetCursorValueToNull(cursor);
		}

		protected virtual void SetCursorValueToNull(IXmlCursor cursor)
		{
			if (IsNillable)
			{
				cursor.MakeNext(clrType);
				cursor.IsNil = true;
			}
			else cursor.RemoveAllNext();
		}

		public void SetNodeValue(IXmlNode node, IDictionaryAdapter parentObject, ref object value)
		{
			if (null != value)
				Serializer.SetValue(node, parentObject, this, ref value);
			else
				SetNodeValueToNull(node);
		}

		private void SetNodeValueToNull(IXmlNode node)
		{
			if (IsNillable)
				node.IsNil = true;
			else
				node.Clear();
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
			return GetDefaultCollectionAccessor(itemType);
		}

		protected IXmlCollectionAccessor GetDefaultCollectionAccessor(Type itemType)
		{
			var accessor = new XmlDefaultBehaviorAccessor(itemType, Context);
			accessor.ConfigureNillable(true);
			return accessor;
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
