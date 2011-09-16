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

	public class XmlKnownType : IXmlKnownType
	{
		private readonly Type clrType;
		private readonly string localName;
		private readonly string namespaceUri;
		private readonly string xsiType;

		public XmlKnownType(string localName, string namespaceUri, string xsiType, Type clrType)
		{
			if (localName == null)
				throw Error.ArgumentNull("localName");
			if (clrType == null)
				throw Error.ArgumentNull("clrType");

			this.localName    = localName;
			this.namespaceUri = namespaceUri;
			this.xsiType      = xsiType;
			this.clrType      = clrType;
		}

		public Type ClrType
		{
			get { return clrType; }
		}

		public string LocalName
		{
			get { return localName; }
		}

		public string NamespaceUri
		{
			get { return namespaceUri; }
		}

		public string XsiType
		{
			get { return xsiType; }
		}

		//Type IXmlTypeMap.BaseType
		//{
		//    get { return ClrType; }
		//}

		//protected virtual bool IsMatch(IXmlType xmlType)
		//{
		//    return (LocalName == null    || NameComparer.Equals(LocalName, xmlType.LocalName      ))
		//        && (NamespaceUri == null || NameComparer.Equals(NamespaceUri, xmlType.NamespaceUri))
		//        && (                        NameComparer.Equals(XsiType, xmlType.XsiType          ));
		//}

		//protected virtual bool IsMatch(Type clrType)
		//{
		//    return clrType == ClrType;
		//}

		//public bool TryGetClrType(IXmlType xmlType, out Type clrType)
		//{
		//    return IsMatch(xmlType)
		//        ? Try.Success(out clrType, ClrType)
		//        : Try.Failure(out clrType);
		//}

		//public bool TryGetXmlType(Type clrType, out IXmlType xmlType)
		//{
		//    return IsMatch(clrType)
		//        ? Try.Success(out xmlType, this)
		//        : Try.Failure(out xmlType);
		//}

		//protected static readonly StringComparer
		//    NameComparer = StringComparer.OrdinalIgnoreCase;
	}
}
