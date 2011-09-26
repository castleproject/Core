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
	using System.Collections.Generic;
	using System.Xml;

	public abstract class XmlNodeAccessor : XmlAccessor, IXmlKnownType, IXmlKnownTypeMap,
		IConfigurable<KeyAttribute>
	{
		private string localName;
		private string namespaceUri;
		private XmlKnownTypeSet knownTypes;
		private States state;

		protected XmlNodeAccessor(Type clrType, IXmlAccessorContext context)
			: this(context.GetDefaultXsiType(clrType).LocalName, clrType, context) { }

		protected XmlNodeAccessor(PropertyDescriptor property, IXmlAccessorContext context)
			: this(property.PropertyName, property.PropertyType, context) { }

		private XmlNodeAccessor(string key, Type type, IXmlAccessorContext context)
			: base(type, context)
		{
			localName    = XmlConvert.EncodeLocalName(key);
			namespaceUri = context.ChildNamespaceUri;
		}

		public XmlName Name
		{
			get { return new XmlName(localName, namespaceUri); }
		}

		XmlName IXmlIdentity.XsiType
		{
			get { return XmlName.Empty; }
		}

		IXmlKnownType IXmlKnownTypeMap.Default
		{
			get { return this; }
		}

		protected IXmlKnownTypeMap KnownTypes
		{
			get { return (IXmlKnownTypeMap) knownTypes ?? this; }
		}

		public override bool IsNillable
		{
			get { return 0 != (state & States.Nillable); }
		}

		public override bool IsVolatile
		{
			get { return 0 != (state & States.Volatile); }
		}

		protected virtual bool IsMatch(IXmlIdentity xmlIdentity)
		{
			return NameComparer.Equals(localName, xmlIdentity.Name.LocalName)
				&& IsMatchOnNamespaceUri(xmlIdentity)
				&& IsMatchOnXsiType     (xmlIdentity);
		}

		private bool IsMatchOnNamespaceUri(IXmlIdentity xmlIdentity)
		{
			return namespaceUri == null 
				|| ShouldIgnoreAttributeNamespaceUri(xmlIdentity)
				|| NameComparer.Equals(namespaceUri, xmlIdentity.Name.NamespaceUri);
		}

		private bool IsMatchOnXsiType(IXmlIdentity xmlIdentity)
		{
			return xmlIdentity.XsiType == XmlName.Empty
				|| xmlIdentity.XsiType == XsiType;
		}

		private bool ShouldIgnoreAttributeNamespaceUri(IXmlIdentity xmlName)
		{
			var xmlNode = xmlName as IXmlNode;
			return xmlNode != null
				&& xmlNode.IsAttribute
				&& 0 == (state & States.ConfiguredNamespaceUri);
		}

		protected virtual bool IsMatch(Type clrType)
		{
			return clrType == this.ClrType
				|| (Serializer.Kind == XmlTypeKind.Collection
				 && typeof(IEnumerable).IsAssignableFrom(clrType));
		}

		public bool TryGet(IXmlIdentity xmlName, out IXmlKnownType knownType)
		{
			return IsMatch(xmlName)
					? Try.Success(out knownType, this)
					: Try.Failure(out knownType);
		}

		public bool TryGet(Type clrType, out IXmlKnownType knownType)
		{
			return IsMatch(clrType)
					? Try.Success(out knownType, this)
					: Try.Failure(out knownType);
		}

		public void Configure(KeyAttribute attrbute)
		{
			ConfigureLocalName(attrbute.Key);
		}

		protected void ConfigureLocalName(string localName)
		{
			ConfigureField(ref this.localName, localName, States.ConfiguredLocalName);
		}

		protected void ConfigureNamespaceUri(string namespaceUri)
		{
			ConfigureField(ref this.namespaceUri, namespaceUri, States.ConfiguredNamespaceUri);
		}

		protected void ConfigureNillable(bool nillable)
		{
			if (nillable)
				state |= States.Nillable;
		}

		public override void ConfigureVolatile(bool isVolatile)
		{
			if (isVolatile)
				state |= States.Volatile;
		}

		private void ConfigureField(ref string field, string value, States mask)
		{
			if (string.IsNullOrEmpty(value))
				return;
			if (0 != (state & mask))
				throw Error.AttributeConflict(null);
			field  = value;
			state |= mask;
		}

		protected void ConfigureKnownTypesFromParent(XmlNodeAccessor accessor)
		{
			if (knownTypes != null)
				throw Error.AttributeConflict(null);

			knownTypes = accessor.knownTypes;
		}

		protected void ConfigureKnownTypesFromAttributes<T>(IEnumerable<T> attributes, IXmlBehaviorSemantics<T> semantics)
		{
			foreach (var attribute in attributes)
			{
				var clrType = semantics.GetClrType(attribute);
				if (clrType != null)
				{
					var name = new XmlName(
						semantics.GetLocalName   (attribute) ?? localName,
						semantics.GetNamespaceUri(attribute) ?? namespaceUri);

					var xsiType = Context.GetDefaultXsiType(clrType);

					AddKnownType(name, xsiType, clrType);
				}
			}
		}

		private void ConfigureIncludedTypes()
		{
			if (knownTypes != null)
				foreach (var knownType in knownTypes)
					ConfigureIncludedTypes(knownType);
			else
				ConfigureIncludedTypes(this);
		}

		private void ConfigureIncludedTypes(IXmlKnownType knownType)
		{
			foreach (var include in Context.GetIncludedTypes(knownType.ClrType))
			{
				AddKnownType(knownType.Name, include.XsiType, include.ClrType);
			}
		}

		private void AddKnownType(XmlName name, XmlName xsiType, Type clrType)
		{
			if (knownTypes == null)
				knownTypes = new XmlKnownTypeSet(ClrType);

			knownTypes.Add(new XmlKnownType(name, xsiType, clrType));
		}

		public override void Prepare()
		{
			ConfigureIncludedTypes();

			if (knownTypes == null)
				return;

			if (0 != (state & States.ConfiguredName))
				knownTypes.Add(new XmlKnownType(Name, XsiType, ClrType));

			knownTypes.AddXsiTypeDefaults();
		}

		[Flags]
		private enum States
		{
			ConfiguredLocalName    = 0x01,
			ConfiguredNamespaceUri = 0x02,
			ConfiguredClrType      = 0x04,
			Nillable               = 0x08,
			Volatile               = 0x10,

			ConfiguredName         = ConfiguredLocalName | ConfiguredNamespaceUri
		}

		protected static readonly StringComparer
			NameComparer = StringComparer.OrdinalIgnoreCase;
	}
}
