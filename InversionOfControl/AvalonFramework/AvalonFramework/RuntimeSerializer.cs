// Copyright 2003-2004 The Apache Software Foundation
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

namespace Apache.Avalon.Framework
{
	using System;
	using System.Collections;
	using System.Runtime.Serialization;

	/// <summary>
	///	Utility class that makes it easier to serialize objects.
	/// </summary>
	internal class RuntimeSerializer
	{
		internal static void SerializeIDictionary(SerializationInfo info, IDictionary dictionary, 
			string nameKeys, string nameValues)
		{
			SerializeICollection(info, dictionary.Keys, nameKeys);
			SerializeICollection(info, dictionary.Values, nameValues);
		}

		internal static IDictionary DeserializeIDictionary(SerializationInfo info, string nameKeys, string nameValues)
		{
			Hashtable hashtable = new Hashtable();

			object[] keys = DeserializeArray(info, nameKeys); 
			object[] values = DeserializeArray(info, nameValues);

			for (int i = 0; i < keys.Length; i++)
			{
				hashtable[keys[i]] = values[i];
			}

			return (IDictionary) hashtable;
		}

		internal static void SerializeICollection(SerializationInfo info, ICollection collection, string name)
		{
			object[] elements = new object[collection.Count];
			collection.CopyTo(elements, 0);

			SerializeArray(info, elements, name);
		}

		internal static ICollection DeserializeICollection(SerializationInfo info, string name)
		{
			return (ICollection) DeserializeArray(info, name);
		}

		internal static void SerializeArray(SerializationInfo info, object[] array, string name)
		{
			info.AddValue(name, array, typeof(object[]));
		}

		internal static object[] DeserializeArray(SerializationInfo info, string name)
		{
			return (object[]) info.GetValue(name, typeof(object[]));
		}
	}
}
