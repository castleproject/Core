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
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if FEATURE_DICTIONARYADAPTER_XML
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Collections.Generic;

	public class XmlContext : XmlContextBase, IXmlContext
	{
		private readonly XmlMetadata metadata;

		public XmlContext(XmlMetadata metadata)
		{
			if (metadata == null)
				throw Error.ArgumentNull("metadata");

			this.metadata = metadata;
		}

		protected XmlContext(XmlContext parent) : base(parent)
		{
			this.metadata = parent.metadata;
		}

		public IXmlContext Clone()
		{
			return new XmlContext(this);
		}

		public string ChildNamespaceUri
		{
			get { return metadata.ChildNamespaceUri; }
		}

		public bool IsReservedNamespaceUri(string namespaceUri)
		{
			return metadata.IsReservedNamespaceUri(namespaceUri);
		}

		public XmlName GetDefaultXsiType(Type clrType)
		{
			return metadata.GetDefaultXsiType(clrType);
		}

		public IEnumerable<IXmlIncludedType> GetIncludedTypes(Type baseType)
		{
			return metadata.GetIncludedTypes(baseType);
		}
	}
}
#endif
