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
	using System.Xml;

	public class DefaultXmlKnownTypeSet : XmlKnownTypeSet
	{
		public static readonly DefaultXmlKnownTypeSet
			Instance = new DefaultXmlKnownTypeSet();

		private DefaultXmlKnownTypeSet()
			: base(typeof(object))
		{
			Add(new XmlIncludedType("xsd:anyType",       typeof(object)));
			Add(new XmlIncludedType("xsd:string",        typeof(string)));
			Add(new XmlIncludedType("xsd:boolean",       typeof(bool)));
			Add(new XmlIncludedType("xsd:byte",          typeof(sbyte)));
			Add(new XmlIncludedType("xsd:unsignedByte",  typeof(byte)));
			Add(new XmlIncludedType("xsd:short",         typeof(short)));
			Add(new XmlIncludedType("xsd:unsignedShort", typeof(ushort)));
			Add(new XmlIncludedType("xsd:int",           typeof(int)));
			Add(new XmlIncludedType("xsd:unsignedInt",   typeof(uint)));
			Add(new XmlIncludedType("xsd:long",          typeof(long)));
			Add(new XmlIncludedType("xsd:unsignedLong",  typeof(ulong)));
			Add(new XmlIncludedType("xsd:float",         typeof(float)));
			Add(new XmlIncludedType("xsd:double",        typeof(double)));
			Add(new XmlIncludedType("xsd:decimal",       typeof(decimal)));
			Add(new XmlIncludedType("wsdl:guid",         typeof(Guid)));           // XmlSerializer requires XmlInclude
			Add(new XmlIncludedType("xsd:dateTime",      typeof(DateTime)));
			Add(new XmlIncludedType("xsd:dateTime",      typeof(DateTimeOffset))); // Not XmlSerializer
			Add(new XmlIncludedType("xsd:duration",      typeof(TimeSpan)));       // Not XmlSerializer
			Add(new XmlIncludedType("xsd:base64Binary",  typeof(byte[])));
			Add(new XmlIncludedType("xsd:anyURI",        typeof(Uri)));
			Add(new XmlIncludedType("xsd:QName",         typeof(XmlQualifiedName)));
		}
	}
}
