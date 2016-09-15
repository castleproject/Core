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
	using System.ComponentModel;
	using System.Reflection;
	using System.Xml.Serialization;

	internal class XmlTypeSerializerCache : SingletonDispenser<Type, XmlTypeSerializer>
	{
		public static readonly XmlTypeSerializerCache
			Instance = new XmlTypeSerializerCache();

		private XmlTypeSerializerCache()
			: base(CreateSerializer)
		{
			this[typeof(Object)]         = XmlDynamicSerializer.Instance;
			this[typeof(String)]         = XmlStringSerializer.Instance;
			this[typeof(Boolean)]        = XmlSimpleSerializer.ForBoolean;
			this[typeof(Char)]           = XmlSimpleSerializer.ForChar;
			this[typeof(SByte)]          = XmlSimpleSerializer.ForSByte;
			this[typeof(Int16)]          = XmlSimpleSerializer.ForInt16;
			this[typeof(Int32)]          = XmlSimpleSerializer.ForInt32;
			this[typeof(Int64)]          = XmlSimpleSerializer.ForInt64;
			this[typeof(Byte)]           = XmlSimpleSerializer.ForByte;
			this[typeof(UInt16)]         = XmlSimpleSerializer.ForUInt16;
			this[typeof(UInt32)]         = XmlSimpleSerializer.ForUInt32;
			this[typeof(UInt64)]         = XmlSimpleSerializer.ForUInt64;
			this[typeof(Single)]         = XmlSimpleSerializer.ForSingle;
			this[typeof(Double)]         = XmlSimpleSerializer.ForDouble;
			this[typeof(Decimal)]        = XmlSimpleSerializer.ForDecimal;
			this[typeof(TimeSpan)]       = XmlSimpleSerializer.ForTimeSpan;
			this[typeof(DateTime)]       = XmlSimpleSerializer.ForDateTime;
			this[typeof(DateTimeOffset)] = XmlSimpleSerializer.ForDateTimeOffset;
			this[typeof(Guid)]           = XmlSimpleSerializer.ForGuid;
			this[typeof(Byte[])]         = XmlSimpleSerializer.ForByteArray;
			this[typeof(Uri)]            = XmlSimpleSerializer.ForUri;
		}

		private static XmlTypeSerializer CreateSerializer(Type type)
		{
			if (type.GetTypeInfo().IsArray)
				return XmlArraySerializer.Instance;

			if (type.GetTypeInfo().IsGenericType)
			{
				var genericType = type.GetGenericTypeDefinition();
				if (genericType == typeof(IList<>) ||
					genericType == typeof(ICollection<>) ||
					genericType == typeof(IEnumerable<>) ||
					genericType == typeof(IBindingList<>))
					return XmlListSerializer.Instance;
#if DOTNET40
				if (genericType == typeof(ISet<>))
				    return XmlSetSerializer.Instance;
#endif
				if (// Dictionaries are not supported
					genericType == typeof(IDictionary<,>) ||
					genericType == typeof(Dictionary<,>) ||
					genericType == typeof(SortedDictionary<,>) ||
					// Concrete list types are not supported
					genericType == typeof(List<>) ||
					genericType == typeof(Stack<>) ||
					genericType == typeof(Queue<>) ||
					genericType == typeof(LinkedList<>) ||
					genericType == typeof(SortedList<,>) ||
					// Concrete set types are not supported
					genericType == typeof(HashSet<>) ||
#if DOTNET40
					genericType == typeof(SortedSet<>) ||
#endif
					// CLR binding list is not supported; use Castle version
					genericType == typeof(BindingList<>))
					throw Error.UnsupportedCollectionType(type);
			}

			if (type.GetTypeInfo().IsInterface)
				return XmlComponentSerializer.Instance;
			if (type.GetTypeInfo().IsEnum)
				return XmlEnumerationSerializer.Instance;
			if (type.IsCustomSerializable())
				return XmlCustomSerializer.Instance;
			if (typeof(System.Xml.XmlNode).IsAssignableFrom(type))
				return XmlXmlNodeSerializer.Instance;
			return new XmlDefaultSerializer(type);
		}
	}
}
#endif
