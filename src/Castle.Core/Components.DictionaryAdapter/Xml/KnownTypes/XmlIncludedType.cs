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
using System.Collections.Generic;
using System.Xml;

	public class XmlIncludedType
	{
		private readonly string xsiType;
		private readonly Type   clrType;

		public XmlIncludedType(string xsiType, Type clrType)
		{
			if (xsiType == null)
				throw Error.ArgumentNull("xsiType");
			if (clrType == null)
				throw Error.ArgumentNull("clrType");

			this.xsiType = xsiType;
			this.clrType = clrType;
		}

		public string XsiType
		{
			get { return xsiType; }
		}

		public Type ClrType
		{
			get { return clrType; }
		}

		public static readonly IList<XmlIncludedType> DefaultSet = Array.AsReadOnly(new[]
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
			new XmlIncludedType("xsd:dateTime",      typeof(DateTimeOffset)),
			new XmlIncludedType("xsd:duration",      typeof(TimeSpan)),      
			new XmlIncludedType("xsd:base64Binary",  typeof(byte[])),
			new XmlIncludedType("xsd:anyURI",        typeof(Uri)),
			new XmlIncludedType("xsd:QName",         typeof(XmlQualifiedName))
		});
	}
}
