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

		protected XmlNodeAccessor(Type type, IXmlAccessorContext context)
			: this(type.GetLocalName(), type, context) { }

		protected XmlNodeAccessor(PropertyDescriptor property, IXmlAccessorContext context)
			: this(property.PropertyName, property.PropertyType, context) { }

		private XmlNodeAccessor(string key, Type type, IXmlAccessorContext context)
			: base(type, context)
		{
			localName    = XmlConvert.EncodeLocalName(key);
			namespaceUri = context.ChildNamespaceUri;
		}

		public string LocalName
		{
			get { return localName; }
		}

		public string NamespaceUri
		{
			get { return namespaceUri; }
		}

		string IXmlName.XsiType
		{
			get { return null; }
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

		protected virtual bool IsMatch(IXmlName xmlName)
		{
			return NameComparer.Equals(LocalName, xmlName.LocalName)
				&& (NamespaceUri    == null || NameComparer.Equals(NamespaceUri, xmlName.NamespaceUri))
				&& (xmlName.XsiType == null || NameComparer.Equals(XsiType,      xmlName.XsiType));
		}

		protected virtual bool IsMatch(Type clrType)
		{
			return clrType == this.ClrType
				|| (Serializer.Kind == XmlTypeKind.Collection
				 && typeof(IEnumerable).IsAssignableFrom(clrType));
		}

		public bool TryGet(IXmlName xmlName, out IXmlKnownType knownType)
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

		protected void ConfigureKnownTypesFromAttributes<T>(IEnumerable<T> attributes, IXmlTypeFrom<T> reader)
		{
			foreach (var attribute in attributes)
			{
				var clrType = reader.GetClrType(attribute);
				if (clrType != null)
				{
					InitializeKnownTypes();

					knownTypes.Add(new XmlKnownType
					(
						reader .GetLocalName   (attribute) ?? localName,
						reader .GetNamespaceUri(attribute) ?? namespaceUri,
						clrType.GetLocalName(),
						clrType
					));
				}
			}
		}

		protected void ConfigureIncludedTypes(Type baseType)
		{
			foreach (var include in Context.IncludedTypes)
			{
				var shouldAdd
					=  baseType != include.ClrType
					&& baseType.IsAssignableFrom(include.ClrType);

				if (shouldAdd)
				{
					InitializeKnownTypes();

					knownTypes.Add(new XmlKnownType
					(
						localName,
						namespaceUri,
						include.XsiType,
						include.ClrType
					));
				}
			}
		}

		private void InitializeKnownTypes()
		{
			if (knownTypes != null)
				return;

			knownTypes = new XmlKnownTypeSet(ClrType);

			var shouldAddSelf = 0 != (state &
			(
				States.ConfiguredLocalName |
				States.ConfiguredNamespaceUri
			));
			if (shouldAddSelf)
			{
				knownTypes.Add(new XmlKnownType
				(
					localName,
					namespaceUri,
					XsiType,
					ClrType)
				);
			}
		}

		public override void Prepare()
		{
			ConfigureIncludedTypes(ClrType);

			if (knownTypes != null)
				knownTypes.AddXsiTypeDefaults();

			base.Prepare();
		}

		[Flags]
		private enum States
		{
			ConfiguredLocalName    = 0x01,
			ConfiguredNamespaceUri = 0x02,
			ConfiguredClrType      = 0x04,
			Nillable               = 0x08,
			Volatile               = 0x10,
		}

		protected static readonly StringComparer
			NameComparer = StringComparer.OrdinalIgnoreCase;
	}
}
