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

	public class XmlNamedType : XmlType
	{
		private readonly string localName;
		private readonly string namespaceUri;
		private readonly Type   clrType;

		public XmlNamedType(string localName, string namespaceUri, Type clrType)
		{
			if (clrType == null)
				throw Error.ArgumentNull("clrType");
			
			this.clrType      = clrType;
			this.localName    = localName;
			this.namespaceUri = namespaceUri;
		}

		public override string LocalName
		{
			get { return localName; }
		}

		public override string NamespaceUri
		{
			get { return namespaceUri; }
		}

		public override string XsiType
		{
			get { return null; }
		}

		public override Type ClrType
		{
			get { return clrType; }
		}

		protected override bool IsMatch(IXmlType xmlType)
		{
			return (                        NameComparer.Equals(localName,    xmlType.LocalName   ))
				&& (namespaceUri == null || NameComparer.Equals(namespaceUri, xmlType.NamespaceUri));
		}
	}
}
