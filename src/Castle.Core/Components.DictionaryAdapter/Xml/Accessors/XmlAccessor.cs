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
	using System.Collections;
	using System.Xml.XPath;

	public abstract class XmlAccessor : IXmlPropertyAccessor, IXmlCollectionAccessor
	{
		private readonly Type clrType;
		private readonly XmlTypeSerializer serializer;
		private readonly IXmlKnownTypeMap knownTypes;

		protected XmlAccessor(Type type, IXmlKnownTypeMap knownTypes)
		{
			this.clrType    = UnwrapNullable(type);
			this.serializer = XmlTypeSerializer.For(clrType);
			this.knownTypes = knownTypes ?? DefaultXmlKnownTypeSet.Instance;
		}

		public Type ClrType
		{
			get { return clrType; }
		}

		public virtual IXmlKnownTypeMap KnownTypes
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

		public virtual void Prepare() { }

		private static Type UnwrapNullable(Type type)
		{
			return type.IsGenericType
				&& type.GetGenericTypeDefinition() == typeof(Nullable<>)
				? type.GetGenericArguments()[0]
				: type;
		}

		protected IXmlKnownType MakeXmlKnownType(Type type)
		{
			var localName = type.GetLocalName();
			return new XmlKnownType(localName, null, type);
		}

		public abstract IXmlCollectionAccessor GetCollectionAccessor(Type itemType);

		protected internal abstract XmlIterator SelectPropertyNode   (XPathNavigator node, bool create);
		protected internal abstract XmlIterator SelectCollectionNode (XPathNavigator node, bool create);
		protected internal abstract XmlIterator SelectCollectionItems(XPathNavigator node, bool create);

		protected XmlIterator SelectSelf(XPathNavigator node)
		{
			return new XmlSelfIterator(new XmlTypedNode(node, clrType));
		}

		public virtual object GetPropertyValue(XPathNavigator parentNode, IDictionaryAdapter parentObject, bool orStub)
		{
			if (orStub) orStub &= serializer.CanGetStub;

			var iterator = Serializer.IsCollection
				? SelectCollectionNode(parentNode, orStub && serializer.CanGetStub)
				: SelectPropertyNode  (parentNode, false);

			if (iterator.MoveNext())
				return serializer.GetValue(iterator.Current, parentObject, this);
			if (orStub)
				return serializer.GetStub(iterator, parentObject, this);
			return null;
		}

		public virtual void SetPropertyValue(XPathNavigator parentNode, object value)
		{
			var iterator = serializer.IsCollection
				? SelectCollectionNode(parentNode, true)
				: SelectPropertyNode  (parentNode, true);

			if (null != value)
			{
				Serializer.SetValue(iterator, this, value);
			}
			else if (IsNillable)
			{
				if (!iterator.MoveNext())
					iterator.Create(ClrType);
				iterator.Current.Node.SetToNil();
			}
			else
			{
				if (iterator.MoveNext())
					iterator.Remove();
			}
		}

		public void GetCollectionItems(XPathNavigator parentNode, IDictionaryAdapter parentObject, IConfigurable<XmlTypedNode> collection)
		{
			var iterator = SelectCollectionItems(parentNode, false);

			while (iterator.MoveNext())
				collection.Configure(iterator.Current.Clone());
		}

		public void GetCollectionItems(XPathNavigator parentNode, IDictionaryAdapter parentObject, IList values)
		{
			var iterator = SelectCollectionItems(parentNode, false);

			while (iterator.MoveNext())
				values.Add(serializer.GetValue(iterator.Current, parentObject, this));
		}

		public void SetCollectionItems(XPathNavigator parentNode, IEnumerable collection)
		{
			var iterator = SelectCollectionItems(parentNode, true);

			foreach (var item in collection)
			{
				if (!iterator.MoveNext())
					iterator.Create(item.GetType());
				serializer.SetValue(iterator.Current, this, item);
			}

			while (iterator.MoveNext())
				iterator.Remove();
		}

		public XmlTypedNode AddCollectionItem(XmlTypedNode parentNode, IDictionaryAdapter parentObject, object value)
		{
			throw new NotImplementedException();
		}

		public XmlTypedNode InsertCollectionItem(XmlTypedNode xmlTypedNode, IDictionaryAdapter parentObject, object value)
		{
			throw new NotImplementedException();
		}

		public void RemoveCollectionItem(XmlTypedNode xmlTypedNode)
		{
			throw new NotImplementedException();
		}

		public void RemoveAllCollectionItems(XmlTypedNode parentNode)
		{
			throw new NotImplementedException();
		}
	}
}
#endif
