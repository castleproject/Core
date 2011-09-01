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

#if !SILVERLIGHT
namespace Castle.Components.DictionaryAdapter.Xml
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Xml.Serialization;

	internal class XmlTypeSerializerCache : SingletonDispenser<Type, XmlTypeSerializer>
	{
		public static readonly XmlTypeSerializerCache
			Instance = new XmlTypeSerializerCache();

		private XmlTypeSerializerCache()
			: base(CreateSerializer) { }

		private static XmlTypeSerializer CreateSerializer(Type type)
		{
		    if (type == typeof(string))
		        return XmlStringSerializer.Instance;
			if (type == typeof(object))
				return XmlDynamicSerializer.Instance;
		    if (type == typeof(Guid))
		        return XmlGuidSerializer.Instance;
		    if (type.IsSimpleType())
		        return XmlSimpleSerializer.Instance;
		    if (type.IsArray)
		        return XmlArraySerializer.Instance;

		    if (type.IsGenericType)
		    {
		        var genericType = type.GetGenericTypeDefinition();
		        if (genericType == typeof(IList<>)       ||
		            genericType == typeof(ICollection<>) ||
		            genericType == typeof(IEnumerable<>) )
		            return XmlListSerializer.Instance;
		        if (genericType == typeof(ISet<>))
		            return XmlListSerializer.Instance; // TODO
		        if (genericType == typeof(BindingList<>))
		            return XmlListSerializer.Instance; // TODO
		        if (genericType == typeof(IDictionary<,>)      ||
		            genericType == typeof(List<>)              ||
		            genericType == typeof(Dictionary<,>)       ||
		            genericType == typeof(HashSet<>)           ||
		            genericType == typeof(Stack<>)             ||
		            genericType == typeof(Queue<>)             ||
		            genericType == typeof(LinkedList<>)        ||
		            genericType == typeof(SortedSet<>)         ||
		            genericType == typeof(SortedDictionary<,>) ||
		            genericType == typeof(SortedList<,>)       )
		            throw Error.UnsupportedCollectionType();
		    }

		    if (type.IsInterface)
		        return XmlComponentSerializer.Instance;
		    if (type.IsEnum)
		        return XmlEnumerationSerializer.Instance;
			if (type.IsCustomSerializable())
				return XmlCustomSerializer.Instance;
			return new XmlDefaultSerializer(type);
		}
	}
}
#endif
