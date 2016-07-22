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
	using System.Xml;

	public abstract class XmlNodeAccessor : XmlAccessor, IXmlKnownType, IXmlKnownTypeMap
	{
		private string localName;
		private string namespaceUri;
		private XmlKnownTypeSet knownTypes;

		protected XmlNodeAccessor(Type type, IXmlContext context)
			: this(context.GetDefaultXsiType(type).LocalName, type, context) { }

		protected XmlNodeAccessor(string name, Type type, IXmlContext context)
			: base(type, context)
		{
			if (name == null)
				throw Error.ArgumentNull("name");
			if (name == string.Empty)
				throw Error.InvalidLocalName();

			localName    = XmlConvert.EncodeLocalName(name);
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

		protected IXmlKnownTypeMap KnownTypes
		{
			get
			{
				if (knownTypes != null)
					return knownTypes;
				return this;
			}
		}

		IXmlKnownType IXmlKnownTypeMap.Default
		{
			get { return this; }
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

		protected virtual bool IsMatch(IXmlIdentity xmlIdentity)
		{
			return NameComparer.Equals(localName, xmlIdentity.Name.LocalName)
				&& IsMatchOnNamespaceUri(xmlIdentity)
				&& IsMatchOnXsiType     (xmlIdentity);
		}

		private bool IsMatchOnNamespaceUri(IXmlIdentity xmlIdentity)
		{
			var otherNamespaceUri = xmlIdentity.Name.NamespaceUri;
			if (Context.IsReservedNamespaceUri(otherNamespaceUri))
				return NameComparer.Equals(namespaceUri, otherNamespaceUri);
			return namespaceUri == null
				|| ShouldIgnoreAttributeNamespaceUri(xmlIdentity)
				|| NameComparer.Equals(namespaceUri, otherNamespaceUri);
		}

		private bool IsMatchOnXsiType(IXmlIdentity xmlIdentity)
		{
			var otherXsiType = xmlIdentity.XsiType;
			return otherXsiType == XmlName.Empty
				|| otherXsiType == XsiType;
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
				|| ( Serializer.Kind == XmlTypeKind.Collection
				&&   typeof(IEnumerable).IsAssignableFrom(clrType) );
		}

		protected void ConfigureLocalName(string localName)
		{
			ConfigureField(ref this.localName, localName, States.ConfiguredLocalName);
		}

		protected void ConfigureNamespaceUri(string namespaceUri)
		{
			ConfigureField(ref this.namespaceUri, namespaceUri, States.ConfiguredNamespaceUri);
		}

		private void ConfigureField(ref string field, string value, States mask)
		{
			if (string.IsNullOrEmpty(value))
				return;
			if (0 != (state & mask))
				throw Error.AttributeConflict(localName);
			field  = value;
			state |= mask;
		}

		protected void ConfigureKnownTypesFromParent(XmlNodeAccessor accessor)
		{
			if (knownTypes != null)
				throw Error.AttributeConflict(localName);

			knownTypes = accessor.knownTypes;
		}

		protected void ConfigureKnownTypesFromAttributes<T>(IEnumerable<T> attributes, IXmlBehaviorSemantics<T> semantics)
		{
			foreach (var attribute in attributes)
			{
				var clrType = semantics.GetClrType(attribute);
				if (clrType != null)
				{
					var xsiType = Context.GetDefaultXsiType(clrType);

					var name = new XmlName(
						semantics.GetLocalName   (attribute).NonEmpty() ?? xsiType.LocalName,
						semantics.GetNamespaceUri(attribute)            ?? namespaceUri);

					AddKnownType(name, xsiType, clrType, true);
				}
			}
		}

		public override void Prepare()
		{
			if (knownTypes == null)
				ConfigureIncludedTypes(this);
			else
				ConfigureDefaultAndIncludedTypes();
		}

		private void ConfigureDefaultAndIncludedTypes()
		{
			var configuredKnownTypes = knownTypes.ToArray();

			knownTypes.AddXsiTypeDefaults();

			foreach (var knownType in configuredKnownTypes)
				ConfigureIncludedTypes(knownType);
		}

		private void ConfigureIncludedTypes(IXmlKnownType knownType)
		{
			var includedTypes = Context.GetIncludedTypes(knownType.ClrType);

			foreach (var include in includedTypes)
				AddKnownType(knownType.Name, include.XsiType, include.ClrType, false);
		}

		private void AddKnownType(XmlName name, XmlName xsiType, Type clrType, bool overwrite)
		{
			if (knownTypes == null)
			{
				knownTypes = new XmlKnownTypeSet(ClrType);
				AddSelfAsKnownType();
			}
			knownTypes.Add(new XmlKnownType(name, xsiType, clrType), overwrite);
		}

		private void AddSelfAsKnownType()
		{
			var mask
				= States.ConfiguredLocalName
				| States.ConfiguredNamespaceUri
				| States.ConfiguredKnownTypes;

			var selfIsKnownType
				= (state & mask) != States.ConfiguredKnownTypes;

			if (selfIsKnownType)
			{
				knownTypes.Add(new XmlKnownType(Name, XsiType,       ClrType), true);
				knownTypes.Add(new XmlKnownType(Name, XmlName.Empty, ClrType), true);
			}
		}

		protected static readonly StringComparer
			NameComparer = StringComparer.OrdinalIgnoreCase;
	}
}
#endif
