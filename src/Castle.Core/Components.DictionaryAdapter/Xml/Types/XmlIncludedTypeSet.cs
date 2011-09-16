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

	public class XmlIncludedTypeSet : IXmlIncludedTypeMap, IEnumerable<IXmlIncludedType>
	{
		private readonly Dictionary<string, IXmlIncludedType> itemsByXsiType;
		private readonly Dictionary<Type,   IXmlIncludedType> itemsByClrType;

		public XmlIncludedTypeSet()
		{
			itemsByXsiType = new Dictionary<string, IXmlIncludedType>();
			itemsByClrType = new Dictionary<Type,   IXmlIncludedType>();

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

		public bool TryGet(string xsiType, out IXmlIncludedType includedType)
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
			new XmlIncludedType("xsd:anyType",       typeof(object)),
			new XmlIncludedType("xsd:string",        typeof(string)),
			new XmlIncludedType("xsd:boolean",       typeof(bool)),
			new XmlIncludedType("xsd:byte",          typeof(sbyte)),
			new XmlIncludedType("xsd:unsignedByte",  typeof(byte)),
			new XmlIncludedType("xsd:short",         typeof(short)),
			new XmlIncludedType("xsd:unsignedShort", typeof(ushort)),
			new XmlIncludedType("xsd:int",           typeof(int)),
			new XmlIncludedType("xsd:unsignedInt",   typeof(uint)),
			new XmlIncludedType("xsd:long",          typeof(long)),
			new XmlIncludedType("xsd:unsignedLong",  typeof(ulong)),
			new XmlIncludedType("xsd:float",         typeof(float)),
			new XmlIncludedType("xsd:double",        typeof(double)),
			new XmlIncludedType("xsd:decimal",       typeof(decimal)),
			new XmlIncludedType("wsdl:guid",         typeof(Guid)),          
			new XmlIncludedType("xsd:dateTime",      typeof(DateTime)),
//			new XmlIncludedType("xsd:dateTime",      typeof(DateTimeOffset)), TODO: Find a way to enable this without duplicate key exception.
			new XmlIncludedType("xsd:duration",      typeof(TimeSpan)),      
			new XmlIncludedType("xsd:base64Binary",  typeof(byte[])),
			new XmlIncludedType("xsd:anyURI",        typeof(Uri)),
			new XmlIncludedType("xsd:QName",         typeof(System.Xml.XmlQualifiedName))
		});
	}
}
