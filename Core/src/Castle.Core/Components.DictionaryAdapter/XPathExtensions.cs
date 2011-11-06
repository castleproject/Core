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

namespace Castle.Components.DictionaryAdapter
{
#if !SILVERLIGHT
	using System;
	using System.Linq;
	using System.Xml;

	public static class XPathExtensions
	{
		private const string XmlMetaKey = "XmlMeta";

		public static XmlMetadata GetXmlMeta(this IDictionaryAdapter dictionaryAdapter)
		{
			return GetXmlMeta(dictionaryAdapter, null);
		}

		public static XmlMetadata GetXmlMeta(this IDictionaryAdapter dictionaryAdapter, Type otherType)
		{
			if (otherType == null || otherType.IsInterface)
			{
				var meta = GetDictionaryMeta(dictionaryAdapter, otherType);
				return (XmlMetadata)meta.ExtendedProperties[XmlMetaKey];
			}
			return null;
		}

		public static Type GetXmlSubclass(this IDictionaryAdapter dictionaryAdapter, XmlQualifiedName xmlType, Type otherType)
		{
			if (xmlType == null)
			{
				return null;
			}
			var xmlIncludes = dictionaryAdapter.GetXmlMeta(otherType).XmlIncludes;
			if (xmlIncludes != null)
			{
				var subClass = from xmlInclude in xmlIncludes
				               let xmlIncludeType = dictionaryAdapter.GetXmlMeta(xmlInclude).XmlType
				               where xmlIncludeType.TypeName == xmlType.Name &&
				                     xmlIncludeType.Namespace == xmlType.Namespace
				               select xmlInclude;
				return subClass.FirstOrDefault();
			}
			return null;
		}

		internal static XmlMetadata GetXmlMeta(this DictionaryAdapterMeta dictionaryAdapterMeta)
		{
			return (XmlMetadata)dictionaryAdapterMeta.ExtendedProperties[XmlMetaKey];
		}

		internal static void SetXmlMeta(this DictionaryAdapterMeta dictionaryAdapterMeta, XmlMetadata xmlMeta)
		{
			dictionaryAdapterMeta.ExtendedProperties[XmlMetaKey] = xmlMeta;
		}

		private static DictionaryAdapterMeta GetDictionaryMeta(IDictionaryAdapter dictionaryAdapter, Type otherType)
		{
			var meta = dictionaryAdapter.Meta;
			if (otherType != null && otherType != meta.Type)
			{
				var descriptor = new DictionaryDescriptor();
				dictionaryAdapter.This.Descriptor.CopyBehaviors(descriptor);
				meta = dictionaryAdapter.This.Factory.GetAdapterMeta(otherType, descriptor);
			}
			return meta;
		}
	}
#endif
}
