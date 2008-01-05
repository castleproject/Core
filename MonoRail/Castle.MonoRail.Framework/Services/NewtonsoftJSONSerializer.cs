// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.Services
{
	using System;
	using Newtonsoft.Json;

	/// <summary>
	/// Pendent
	/// </summary>
	public class NewtonsoftJSONSerializer : IJSONSerializer
	{
		/// <summary>
		/// Serializes the specified object.
		/// </summary>
		/// <param name="target">The object.</param>
		/// <returns></returns>
		public string Serialize(object target)
		{
			return JavaScriptConvert.SerializeObject(target);
		}

		/// <summary>
		/// Deserializes the specified object.
		/// </summary>
		/// <param name="jsonString">The json representation of an object string.</param>
		/// <param name="expectedType">The expected type.</param>
		/// <returns></returns>
		public object Deserialize(string jsonString, Type expectedType)
		{
			return JavaScriptConvert.DeserializeObject(jsonString, expectedType);
		}

		/// <summary>
		/// Deserializes the specified object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="jsonString">The json representation of an object string.</param>
		/// <returns></returns>
		public T Deserialize<T>(string jsonString)
		{
			return (T) JavaScriptConvert.DeserializeObject(jsonString, typeof(T));
		}

		/// <summary>
		/// Deserializes the specified object.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="jsonString">The json representation of an object string.</param>
		/// <returns></returns>
		public T[] DeserializeArray<T>(string jsonString)
		{
			return (T[]) JavaScriptConvert.DeserializeObject(jsonString, typeof(T));
		}
	}
}
