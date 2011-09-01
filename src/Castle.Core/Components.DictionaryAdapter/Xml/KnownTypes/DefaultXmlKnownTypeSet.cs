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
	using System.Xml;

	public class DefaultXmlKnownTypeSet : XmlKnownTypeSet
	{
		public static readonly DefaultXmlKnownTypeSet
			Instance = new DefaultXmlKnownTypeSet();

		private DefaultXmlKnownTypeSet()
		{
			Add(new XmlKnownType(null, null, "xsd:anyType",       typeof(object)));
			Add(new XmlKnownType(null, null, "xsd:string",        typeof(string)));
			Add(new XmlKnownType(null, null, "xsd:boolean",       typeof(bool)));
			Add(new XmlKnownType(null, null, "xsd:byte",          typeof(sbyte)));
			Add(new XmlKnownType(null, null, "xsd:unsignedByte",  typeof(byte)));
			Add(new XmlKnownType(null, null, "xsd:short",         typeof(short)));
			Add(new XmlKnownType(null, null, "xsd:unsignedShort", typeof(ushort)));
			Add(new XmlKnownType(null, null, "xsd:int",           typeof(int)));
			Add(new XmlKnownType(null, null, "xsd:unsignedInt",   typeof(uint)));
			Add(new XmlKnownType(null, null, "xsd:long",          typeof(long)));
			Add(new XmlKnownType(null, null, "xsd:unsignedLong",  typeof(ulong)));
			Add(new XmlKnownType(null, null, "xsd:float",         typeof(float)));
			Add(new XmlKnownType(null, null, "xsd:double",        typeof(double)));
			Add(new XmlKnownType(null, null, "xsd:decimal",       typeof(decimal)));
			Add(new XmlKnownType(null, null, "wsdl:guid",         typeof(Guid)));           // XmlSerializer requires XmlInclude
			Add(new XmlKnownType(null, null, "xsd:dateTime",      typeof(DateTime)));
			Add(new XmlKnownType(null, null, "xsd:dateTime",      typeof(DateTimeOffset))); // Not XmlSerializer
			Add(new XmlKnownType(null, null, "xsd:duration",      typeof(TimeSpan)));       // Not XmlSerializer
			Add(new XmlKnownType(null, null, "xsd:base64Binary",  typeof(byte[])));
			Add(new XmlKnownType(null, null, "xsd:anyURI",        typeof(Uri)));
			Add(new XmlKnownType(null, null, "xsd:QName",         typeof(XmlQualifiedName)));
		}
	}
}
#endif
