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
		private IXmlContext context;
		protected States state;

		protected XmlAccessor(Type clrType, IXmlContext context)
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

		public IXmlContext Context
		{
			get { return context; }
			protected set { SetContext(value); }
		}

		public bool IsCollection
		{
			get { return serializer.Kind == XmlTypeKind.Collection; }
		}

		public virtual bool IsIgnored
		{
			get { return false; }
		}

		public bool IsNillable
		{
			get { return 0 != (state & States.Nillable); }
		}

		public bool IsVolatile
		{
			get { return 0 != (state & States.Volatile); }
		}

		public bool IsReference
		{
			get { return 0 != (state & States.Reference); }
		}

		public virtual void ConfigureNillable(bool nillable)
		{
			if (nillable)
				state |= States.Nillable;
		}

		public void ConfigureVolatile(bool isVolatile)
		{
			if (isVolatile)
				state |= States.Volatile;
		}

		public void ConfigureReference(bool isReference)
		{
			if (isReference)
				state |= States.Reference;
		}

		public virtual void Prepare()
		{
			// Do nothing
		}

		protected IXmlContext CloneContext()
		{
			if (0 == (state & States.ConfiguredContext))
			{
				context = context.Clone();
				state |= States.ConfiguredContext;
			}
			return context;
		}

		private void SetContext(IXmlContext value)
		{
			if (null == value)
				throw Error.ArgumentNull("value");

			context = value;
		}

		public virtual bool IsPropertyDefined(IXmlNode parentNode)
		{
			var cursor = IsCollection
				? SelectCollectionNode(parentNode, false)
				: SelectPropertyNode  (parentNode, false);

			return cursor.MoveNext();
		}

		public virtual object GetPropertyValue(IXmlNode parentNode, IDictionaryAdapter parentObject, XmlReferenceManager references, bool orStub)
		{
			if (orStub) orStub &= serializer.CanGetStub;

			var cursor = IsCollection
				? SelectCollectionNode(parentNode, orStub)
				: SelectPropertyNode  (parentNode, orStub);

			return GetValue(cursor, parentObject, references, cursor.MoveNext(), orStub);
		}

		public object GetValue(IXmlNode node, IDictionaryAdapter parentObject, XmlReferenceManager references, bool nodeExists, bool orStub)
		{
			var value = null as object;
			var token = null as object;

			if (nodeExists)
				if (!references.OnGetStarting(ref node, ref value, out token))
					return value;

			value = GetValueCore(node, parentObject, nodeExists, orStub);

			if (nodeExists)
				references.OnGetCompleted(node, value, token);

			return value;
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

		public virtual void SetPropertyValue(IXmlNode parentNode, IDictionaryAdapter parentObject, XmlReferenceManager references,
			object oldValue, ref object value)
		{
			var cursor = IsCollection
				? SelectCollectionNode(parentNode, true)
				: SelectPropertyNode  (parentNode, true);
				
			SetValue(cursor, parentObject, references, cursor.MoveNext(), oldValue, ref value);
		}

		public virtual void SetValue(IXmlCursor cursor, IDictionaryAdapter parentObject, XmlReferenceManager references,
			bool hasCurrent, object oldValue, ref object newValue)
		{
			var hasValue   = null != newValue;
			var isNillable = this.IsNillable; 

			var clrType = hasValue
				? newValue.GetComponentType()
				: this.clrType;

			if (hasValue || isNillable)
			{
				if (hasCurrent)
					Coerce(cursor, clrType, !hasValue && cursor.IsAttribute); // TODO: Refactor. (NB: && isNillable is emplied)
				else
					cursor.Create(clrType);
			}
			else if (!hasCurrent)
			{
				// No node exists + no value to assign + and not nillable = no work to do
				return;
			}

			object token;
			if (references.OnAssigningValue(cursor, oldValue, ref newValue, out token))
			{
			    var givenValue = newValue;

				if (hasValue)
					Serializer.SetValue(cursor, parentObject, this, ref newValue);
				else if (isNillable)
					cursor.IsNil = true;
				else
					{ cursor.Remove(); cursor.RemoveAllNext(); }

			    references.OnAssignedValue(cursor, givenValue, newValue, token);
			}
		}

		private void Coerce(IXmlCursor cursor, Type clrType, bool replace)
		{
			if (replace)
			{
				cursor.Remove();
				cursor.MoveNext();
				cursor.Create(ClrType);
			}
			else cursor.Coerce(clrType);
		}

		//public void SetValueOld(IXmlCursor cursor, IDictionaryAdapter parentObject, XmlReferenceManager references, bool hasCurrent, object oldValue, ref object value)
		//{
		//    if (null != value)
		//        SetValueCore(cursor, parentObject, references, hasCurrent, oldValue, ref value);
		//    else
		//        SetValueToNull(cursor, references, hasCurrent, oldValue);
		//}

		//protected virtual void SetValueToNull(IXmlCursor cursor, XmlReferenceManager references, bool hasCurrent, object oldValue)
		//{
		//    if (IsNillable)
		//        SetNodeToNil(cursor, references, hasCurrent, oldValue);
		//    else if (hasCurrent)
		//        cursor.Clear();
		//    else
		//        cursor.RemoveAllNext();
		//}

		//private void SetValueCore(IXmlCursor cursor, IDictionaryAdapter parentObject, XmlReferenceManager references, bool hasCurrent, object oldValue, ref object newValue)
		//{
		//    var clrType = newValue.GetComponentType();

		//    if (hasCurrent || cursor.MoveNext())
		//        cursor.Coerce(clrType);
		//    else
		//        cursor.Create(clrType);

		//    object token;
		//    if (references.OnAssigningValue(cursor, oldValue, ref newValue, out token))
		//    {
		//        var givenValue = newValue;
		//        Serializer.SetValue(cursor, parentObject, this, ref newValue);
		//        references.OnAssignedValue(cursor, givenValue, newValue, token);
		//    }
		//}

		//private void SetNodeToNil(IXmlCursor cursor, XmlReferenceManager references, bool hasCurrent, object oldValue)
		//{
		//    if (hasCurrent || cursor.MoveNext())
		//        if (cursor.IsAttribute)
		//            ReplaceCurrent(cursor);
		//        else
		//            cursor.Coerce(clrType);
		//    else
		//        cursor.Create(clrType);

		//    var newValue = null as object;
		//    object token;
		//    if (references.OnAssigningValue(cursor, oldValue, ref newValue, out token))
		//    {
		//        cursor.IsNil = true;
		//        references.OnAssignedValue(cursor, newValue, newValue, token);
		//    }
		//}

		//private void ReplaceCurrent(IXmlCursor cursor)
		//{
		//    cursor.Remove();
		//    cursor.MoveNext();
		//    cursor.Create(ClrType);
		//}

		public void GetCollectionItems(IXmlNode parentNode, IDictionaryAdapter parentObject, XmlReferenceManager references, IList values)
		{
			var cursor = SelectCollectionItems(parentNode, false);

			while (cursor.MoveNext())
			{
				IXmlNode node  = cursor;
				object   value = null;
				object   token;

				if (references.OnGetStarting(ref node, ref value, out token))
				{
					value = serializer.GetValue(cursor, parentObject, this);
					references.OnGetCompleted(cursor, value, token);
				}

				values.Add(value);
			}
		}

		protected void RemoveCollectionItems(IXmlNode parentNode, XmlReferenceManager references, object value)
		{
			var collection = value as IXmlCollection;
			if (collection != null)
			{
				collection.Clear();
				return;
			}

			var itemType = clrType.GetCollectionItemType();
			var accessor = GetCollectionAccessor(itemType);
			var cursor   = accessor.SelectCollectionItems(parentNode, true);

			var items = value as IEnumerable;
			if (items != null)
			{
				foreach (var item in items)
				{
					if (!cursor.MoveNext())
						break;
					references.OnAssigningNull(cursor, item);
				}
			}

			cursor.Reset();
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

		[Flags]
		protected enum States
		{
			Nillable               = 0x01, // Set a null value as xsi:nil='true'
			Volatile               = 0x02, // Always get value from XML store; don't cache it
			Reference              = 0x04, // Participate in reference tracking
			ConfiguredContext      = 0x08, // Have created our own IXmlContext instance
			ConfiguredLocalName    = 0x10, // The local name    has been configured
			ConfiguredNamespaceUri = 0x20, // The namespace URI has been configured
			ConfiguredKnownTypes   = 0x40, // Known types have been configured from attributes
		}
	}
}
