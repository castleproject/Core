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
	using System.Collections.Generic;
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
		/// Serializes the specified object.
		/// </summary>
		/// <param name="target">The object.</param>
		/// <param name="converters">The converters.</param>
		/// <returns></returns>
		public string Serialize(object target, params IJSONConverter[] converters)
		{
			List<JsonConverter> adapters = new List<JsonConverter>();

			foreach(IJSONConverter converter in converters)
			{
				adapters.Add(new JsonConverterAdapter(converter));
			}

			return JavaScriptConvert.SerializeObject(target, adapters.ToArray());
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

		class JsonConverterAdapter : JsonConverter
		{
			private readonly IJSONConverter converter;

			public JsonConverterAdapter(IJSONConverter converter)
			{
				this.converter = converter;
			}

			public override void WriteJson(JsonWriter writer, object value)
			{
				converter.Write(new JSONWriterAdapter(writer), value);
			}

			public override bool CanConvert(Type objectType)
			{
				return converter.CanHandle(objectType);
			}
		}

		class JSONWriterAdapter : IJSONWriter
		{
			private readonly JsonWriter writer;

			public JSONWriterAdapter(JsonWriter writer)
			{
				this.writer = writer;
			}

			public JsonWriter Writer
			{
				get { return writer; }
			}

			public void WriteValue(object value)
			{
				new JsonSerializer().Serialize(writer, value);
			}

			public void WriteStartObject()
			{
				writer.WriteStartObject();
			}

			public void WriteEndObject()
			{
				writer.WriteEndObject();
			}

			public void WriteStartArray()
			{
				writer.WriteStartArray();
			}

			public void WriteEndArray()
			{
				writer.WriteEndArray();
			}

			public void WritePropertyName(string name)
			{
				writer.WritePropertyName(name);
			}

			public void WriteEnd()
			{
				writer.WriteEnd();
			}

			public void WriteNull()
			{
				writer.WriteNull();
			}

			public void WriteUndefined()
			{
				writer.WriteUndefined();
			}

			public void WriteRaw(string javaScript)
			{
				writer.WriteRaw(javaScript);
			}

			public void WriteValue(string value)
			{
				writer.WriteValue(value);
			}

			public void WriteValue(int value)
			{
				writer.WriteValue(value);
			}

			public void WriteValue(long value)
			{
				writer.WriteValue(value);
			}

			public void WriteValue(float value)
			{
				writer.WriteValue(value);
			}

			public void WriteValue(bool value)
			{
				writer.WriteValue(value);
			}

			public void WriteValue(short value)
			{
				writer.WriteValue(value);
			}

			public void WriteValue(decimal value)
			{
				writer.WriteValue(value);
			}

			public void WriteValue(DateTime value)
			{
				writer.WriteValue(value);
			}
		}
	}
}
