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

	public class XmlIncludedType : IXmlIncludedType
	{
		private readonly XmlName xsiType;
		private readonly Type    clrType;

		public XmlIncludedType(XmlName xsiType, Type clrType)
		{
			if (xsiType.LocalName == null)
				throw Error.ArgumentNull("xsiType.LocalName");
			if (clrType == null)
				throw Error.ArgumentNull("clrType");

			this.xsiType = xsiType;
			this.clrType = clrType;
		}

		public XmlIncludedType(string localName, string namespaceUri, Type clrType)
			: this(new XmlName(localName, namespaceUri), clrType) { }

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
