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

	public class XmlKnownType : IXmlKnownType
	{
		private readonly XmlName name;
		private readonly XmlName xsiType;
		private readonly Type    clrType;

		public XmlKnownType(XmlName name, XmlName xsiType, Type clrType)
		{
			if (name.LocalName == null)
				throw Error.ArgumentNull("name.LocalName");
			if (clrType == null)
				throw Error.ArgumentNull("clrType");

			this.name    = name;
			this.xsiType = xsiType;
			this.clrType = clrType;
		}

		public XmlKnownType(string nameLocalName, string nameNamespaceUri, string xsiTypeLocalName, string xsiTypeNamespaceUri, Type clrType)
			: this
			(
				new XmlName(nameLocalName,    nameNamespaceUri),
				new XmlName(xsiTypeLocalName, xsiTypeNamespaceUri),
				clrType
			)
		{ }

		public XmlName Name
		{
			get { return name; }
		}

		public XmlName XsiType
		{
			get { return xsiType; }
		}

		public Type ClrType
		{
			get { return clrType; }
		}
	}
}
#endif
