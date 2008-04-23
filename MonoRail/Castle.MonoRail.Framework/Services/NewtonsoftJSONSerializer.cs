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
	using System.IO;
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
			JsonSerializer serializer = new JsonSerializer();
			serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

			StringWriter writer = new StringWriter();
			serializer.Serialize(writer, target);

			return writer.GetStringBuilder().ToString();
		}

		/// <summary>
		/// Serializes the specified object.
		/// </summary>
		/// <param name="target">The object.</param>
		/// <param name="converters">The converters.</param>
		/// <returns></returns>
		public string Serialize(object target, params IJSONConverter[] converters)
		{
			JsonSerializer serializer = new JsonSerializer();

			if (converters != null)
			{
				foreach(IJSONConverter converter in converters)
				{
					serializer.Converters.Add(new JsonConverterAdapter(serializer, converter));
				}
			}

			serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;

			StringWriter writer = new StringWriter();
			serializer.Serialize(writer, target);

			return writer.GetStringBuilder().ToString();
		}

		/// <summary>
		/// Serializes the specified object.
		/// </summary>
		/// <param name="target">The target.</param>
		/// <param name="writer">The writer.</param>
		/// <param name="converters">The converters.</param>
		public void Serialize(object target, TextWriter writer, params IJSONConverter[] converters)
		{
			JsonSerializer serializer = new JsonSerializer();

			if (converters != null)
			{
				foreach(IJSONConverter converter in converters)
				{
					serializer.Converters.Add(new JsonConverterAdapter(serializer, converter));
				}
			}

			serializer.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
			serializer.Serialize(writer, target);
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
			private readonly JsonSerializer serializer;
			private readonly IJSONConverter converter;

			public JsonConverterAdapter(JsonSerializer serializer, IJSONConverter converter)
			{
				this.serializer = serializer;
				this.converter = converter;
			}

			public override void WriteJson(JsonWriter writer, object value)
			{
				converter.Write(new JSONWriterAdapter(serializer, writer), value);
			}

			public override bool CanConvert(Type objectType)
			{
				return converter.CanHandle(objectType);
			}

#if DOTNET35
			public override object ReadJson(JsonReader reader, Type objectType)
			{
				return converter.ReadJson(new JSONReaderAdapter(reader), objectType);
			}
#endif
		}

		class JSONWriterAdapter : IJSONWriter
		{
			private readonly JsonSerializer serializer;
			private readonly JsonWriter writer;

			public JSONWriterAdapter(JsonSerializer serializer, JsonWriter writer)
			{
				this.serializer = serializer;
				this.writer = writer;
			}

			public void WriteValue(object value)
			{
				serializer.Serialize(writer, value);
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

#if DOTNET35
		class JSONReaderAdapter : IJSONReader
		{
			private readonly JsonReader reader;

			public JSONReaderAdapter(JsonReader reader)
			{
				this.reader = reader;
			}

			public JsonReader Reader
			{
				get { return reader; }
			}

			public char QuoteChar
			{
				get { return reader.QuoteChar; }
			}

			public object Value
			{
				get { return reader.Value; }
			}

			public Type ValueType
			{
				get { return reader.ValueType; }
			}

			public int Depth
			{
				get { return reader.Depth; }
			}

			public bool Read()
			{
				return reader.Read();
			}

			public void Skip()
			{
				reader.Skip();
			}

			public void Close()
			{
				reader.Close();
			}
		}
#endif
	}
}
