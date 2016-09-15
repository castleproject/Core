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
	using System.Linq;
	using System.Reflection;
	using System.Xml.Serialization;

	public static class TypeExtensions
	{
		public static Type NonNullable(this Type type)
		{
			return type.GetTypeInfo().IsGenericType
				&& type.GetGenericTypeDefinition() == typeof(Nullable<>)
				? type.GetGenericArguments()[0]
				: type;
		}

		public static Type GetCollectionItemType(this Type type)
		{
			if (type.GetTypeInfo().IsArray)
				return type.GetElementType();
			if (type.GetTypeInfo().IsGenericType)
				return type.GetGenericArguments().Single();
			throw Error.NotCollectionType("type");
		}

		public static Type GetComponentType(this object obj)
		{
			var adapter = obj as IDictionaryAdapter;
			return (adapter == null)
				? obj.GetType()
				: adapter.Meta.Type;
		}

		internal static bool IsCustomSerializable(this Type type)
		{
			return typeof(IXmlSerializable).IsAssignableFrom(type);
		}
	}
}
#endif
