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
	using System.Linq;
	using System.Collections;

	public static class DictionaryAdapterExtensions
	{
		public static object CreateChildAdapter(this IDictionaryAdapter parent, Type type, XmlAdapter adapter)
		{
			return CreateChildAdapter(parent, type, adapter, null);
		}

		public static object CreateChildAdapter(this IDictionaryAdapter parent, Type type, XmlAdapter adapter, IDictionary dictionary)
		{
			if (null == dictionary)
				dictionary = new Hashtable();

			var descriptor = parent.Meta.CreateDescriptor();
		    parent.This.Descriptor.CopyBehaviors(descriptor);
		    descriptor.AddBehavior(adapter);

		    return parent.This.Factory.GetAdapter(type, dictionary, descriptor);
		}

		public static bool HasAccessor(this PropertyDescriptor property)
		{
			return property.ExtendedProperties.Contains(XmlAccessorKey);
		}

		public static XmlAccessor GetAccessor(this PropertyDescriptor property)
		{
		    return (XmlAccessor) property.ExtendedProperties[XmlAccessorKey];
		}

		public static void SetAccessor(this PropertyDescriptor property, XmlAccessor accessor)
		{
		    property.ExtendedProperties[XmlAccessorKey] = accessor;
		}

		public static bool HasXmlMeta(this DictionaryAdapterMeta meta)
		{
			return meta.ExtendedProperties.Contains(XmlMetaKey);
		}

		public static XmlMetadata GetXmlMeta(this DictionaryAdapterMeta meta)
		{
			return (XmlMetadata) meta.ExtendedProperties[XmlMetaKey];
		}

		public static void SetXmlMeta(this DictionaryAdapterMeta meta, XmlMetadata xmlMeta)
		{
			meta.ExtendedProperties[XmlMetaKey] = xmlMeta;
		}

		public static bool HasXmlType(this DictionaryAdapterMeta meta)
		{
			return meta.ExtendedProperties.Contains(XmlTypeKey);
		}

		public static string GetXmlType(this DictionaryAdapterMeta meta)
		{
			return (string) meta.ExtendedProperties[XmlTypeKey];
		}

		public static void SetXmlType(this DictionaryAdapterMeta meta, string value)
		{
			meta.ExtendedProperties[XmlTypeKey] = value;
		}

		private const string
			XmlAccessorKey  = "XmlAccessor",
			XmlMetaKey      = "XmlMeta",
			XmlTypeKey      = "XmlType";
	}
}
#endif
