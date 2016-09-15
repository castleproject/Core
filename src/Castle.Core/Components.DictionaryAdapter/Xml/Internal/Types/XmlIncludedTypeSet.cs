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

	public class XmlIncludedTypeSet : IXmlIncludedTypeMap, IEnumerable<IXmlIncludedType>
	{
		private readonly Dictionary<XmlName, IXmlIncludedType> itemsByXsiType;
		private readonly Dictionary<Type,    IXmlIncludedType> itemsByClrType;

		public XmlIncludedTypeSet()
		{
			itemsByXsiType = new Dictionary<XmlName, IXmlIncludedType>();
			itemsByClrType = new Dictionary<Type,    IXmlIncludedType>();

			foreach (var includedType in DefaultEntries)
				Add(includedType);
		}

		IXmlIncludedType IXmlIncludedTypeMap.Default
		{
			get { throw Error.NoDefaultKnownType(); }
		}

		public void Add(IXmlIncludedType includedType)
		{
			// Allow only one item per xsi:type
			itemsByXsiType.Add(includedType.XsiType, includedType);

			// Overwrite any prior entry for CLR type
			itemsByClrType[includedType.ClrType] = includedType;
		}

		public bool TryGet(XmlName xsiType, out IXmlIncludedType includedType)
		{
			return itemsByXsiType.TryGetValue(xsiType, out includedType);
		}

		public bool TryGet(Type clrType, out IXmlIncludedType includedType)
		{
			return itemsByClrType.TryGetValue(clrType, out includedType);
		}

		public IEnumerator<IXmlIncludedType> GetEnumerator()
		{
			return itemsByXsiType.Values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public static readonly IList<IXmlIncludedType> DefaultEntries
			= Array.AsReadOnly(new IXmlIncludedType[]
		{
			new XmlIncludedType("anyType",       Xsd .NamespaceUri, typeof(object)),
			new XmlIncludedType("string",        Xsd .NamespaceUri, typeof(string)),
			new XmlIncludedType("boolean",       Xsd .NamespaceUri, typeof(bool)),
			new XmlIncludedType("byte",          Xsd .NamespaceUri, typeof(sbyte)),
			new XmlIncludedType("unsignedByte",  Xsd .NamespaceUri, typeof(byte)),
			new XmlIncludedType("short",         Xsd .NamespaceUri, typeof(short)),
			new XmlIncludedType("unsignedShort", Xsd .NamespaceUri, typeof(ushort)),
			new XmlIncludedType("int",           Xsd .NamespaceUri, typeof(int)),
			new XmlIncludedType("unsignedInt",   Xsd .NamespaceUri, typeof(uint)),
			new XmlIncludedType("long",          Xsd .NamespaceUri, typeof(long)),
			new XmlIncludedType("unsignedLong",  Xsd .NamespaceUri, typeof(ulong)),
			new XmlIncludedType("float",         Xsd .NamespaceUri, typeof(float)),
			new XmlIncludedType("double",        Xsd .NamespaceUri, typeof(double)),
			new XmlIncludedType("decimal",       Xsd .NamespaceUri, typeof(decimal)),
			new XmlIncludedType("guid",          Wsdl.NamespaceUri, typeof(Guid)),          
			new XmlIncludedType("dateTime",      Xsd .NamespaceUri, typeof(DateTime)),
//			new XmlIncludedType("dateTime",      Xsd .NamespaceUri, typeof(DateTimeOffset)), TODO: Find a way to enable this without duplicate key exception.
			new XmlIncludedType("duration",      Xsd .NamespaceUri, typeof(TimeSpan)),      
			new XmlIncludedType("base64Binary",  Xsd .NamespaceUri, typeof(byte[])),
			new XmlIncludedType("anyURI",        Xsd .NamespaceUri, typeof(Uri)),
			new XmlIncludedType("QName",         Xsd .NamespaceUri, typeof(System.Xml.XmlQualifiedName))
		});
	}
}
#endif
